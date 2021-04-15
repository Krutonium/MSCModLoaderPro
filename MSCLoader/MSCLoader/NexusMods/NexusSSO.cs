﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine.UI;
using MSCLoader.Helper;

namespace MSCLoader.NexusMods
{
    class NexusSSO : MonoBehaviour
    {
        static NexusSSO instance;
        internal static NexusSSO Instance => instance;

        bool isActive;
        string output;
        int waitTime;
        const int WaitTimeMax = 60 * 5; // Wait maximum of 5 mintues (this is how long the token is valid on Nexus).
        Process p;
        ModPrompt promptCancel;

        protected string apiKey, token;
        protected UserInfo userInfo;

        const string NexusUserInfo = "https://api.nexusmods.com/v1/users/validate.json";

        string NexusDataFolder => Path.Combine(ModUpdater.UpdaterDirectory, "Nexus");

        internal string ApiKey => apiKey;

        internal bool IsValid => userInfo != null;
        internal bool IsPremium => userInfo.IsPremium;
        internal string Name => userInfo.Name;
        internal string ProfilePic => userInfo.ProfilePic;

        bool isReady;
        internal bool IsReady => isReady;

        Text uiUserName, notLoggedIn, status;
        Texture defaultPfp;
        bool forceDownloadNewPfp;

        NexusMenuUI ui => ModLoader.UICanvas.GetComponentInChildren<NexusMenuUI>();

        public NexusSSO()
        {
            instance = this;

            defaultPfp = ui.profilePicture.texture;

            string data = SecureStorage.Load();
            if (string.IsNullOrEmpty(data))
            {
                // User is not logged in.
                isReady = true;

                ui.loggedIn.text = "<color=yellow>LOG IN</color>";
                ui.hoverText.oldText = "<color=yellow>LOG IN</color>";
                ui.hoverText.newText = "<color=yellow>LOG IN</color>";
                ui.userName.text = "";
                ui.memberStatus.text = "";
            }
            else
            {
                string[] arr = data.Split('\n');
                apiKey = arr[0].Split(':')[1].Trim();
                token = arr[1].Split(':')[1].Trim();

                ui.loggedIn.text = "<color=yellow>GETTING DATA...</color>";
                ui.userName.text = "";
                ui.memberStatus.text = "";
                ui.hoverText.oldText = "<color=yellow>GETTING DATA...</color>";
                ui.hoverText.newText = "<color=yellow>GETTING DATA...</color>";

                VerifyAccount();
            }
        }

        internal void RequestLogin()
        {
            if (isActive)
            {
                ModConsole.Log("[Nexus SSO] Busy!");
                return;
            }

            if (IsValid)
            {
                ModPrompt.CreateYesNoPrompt("Are you sure you want to log out?", "NexusMods Login", Logout);
                return;
            }

            request = RequestingLogin();
            StartCoroutine(request);
        }

        void Logout()
        {
            ui.profilePicture.texture = defaultPfp;
            ui.loggedIn.text = "<color=yellow>LOG IN</color>";
            ui.userName.text = "";
            ui.memberStatus.text = "";
            ui.hoverText.oldText = "<color=yellow>LOG IN</color>";
            ui.hoverText.newText = "<color=yellow>LOG IN</color>";

            userInfo = null;
            apiKey = null;
            token = null;

            SecureStorage.Delete();
        }

        IEnumerator request;
        bool cancel;
        IEnumerator RequestingLogin()
        {
            isActive = true;

            ModConsole.Log("[NexusSSO] Starting CoolUpdater...");

            ModPrompt prompt = ModPrompt.CreateButtonlessPrompt();
            prompt.Text = "You will now be taken to NexusMods...";
            prompt.Title = "NexusMods Login";
            yield return new WaitForSeconds(1);

            output = "";

            string args = "nexus-login";
            if (!string.IsNullOrEmpty(token))
            {
                args += " " + token;
            }
            p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ModUpdater.UpdaterPath,
                    Arguments = args,
                    WorkingDirectory = ModUpdater.UpdaterDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            waitTime = 0;
            while (!p.HasExited)
            {
                waitTime++;
                if (waitTime > WaitTimeMax || cancel)
                {
                    cancel = false;
                    ModConsole.LogError($"[Nexus SSO] Login token timed-out.");
                    promptCancel.gameObject.SetActive(false);
                    p.Close();
                    yield break;
                }

                yield return new WaitForSeconds(1);

                if (waitTime == 2 && prompt != null)
                {
                    prompt.gameObject.SetActive(false);
                    prompt = null;
                    promptCancel = ModPrompt.CreateCustomPrompt();
                    promptCancel.Text = "Waiting for user to login...";
                    promptCancel.Title = "NexusMods Login";
                    promptCancel.AddButton("CANCEL", CancelLogin);
                }
            }

            p.Close();
            promptCancel?.gameObject.SetActive(false);

            string[] arr = output.Split('\n');
            SecureStorage.Save(output);
            apiKey = arr[0].Split(':')[1].Replace("\"", "").Replace(",", "").Trim();
            token = arr[1].Split(':')[1].Replace("\"", "").Replace(",", "").Trim();
            isActive = false;
            forceDownloadNewPfp = true;
            VerifyAccount();
        }

        void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            output += e.Data + "\n";
        }

        void ErrorHandler(object sender, DataReceivedEventArgs e)
        {
            ModConsole.Log(e.Data);
        }

        void CancelLogin()
        {
            cancel = true;
            promptCancel.gameObject.SetActive(false);
            p.Kill();
            isActive = false;
            ModPrompt.CreatePrompt("Log in procedure has been canceled.", "NexusMods Login");
        }

        void VerifyAccount()
        {
            StartCoroutine(VerifyRoutine());
        }

        IEnumerator VerifyRoutine()
        {
            isActive = true;

            // we are checking if user has a premium account and if key is valid.
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ModUpdater.UpdaterPath,
                    Arguments = "get-metafile " + string.Join(" ", new string[] { NexusUserInfo, apiKey }),
                    WorkingDirectory = ModUpdater.UpdaterDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            output = "";
            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            int downloadTime = 0;
            while (!p.HasExited)
            {
                downloadTime++;
                if (downloadTime > 10)
                {
                    ModConsole.LogError($"[Nexus SSO] Getting metadata of User timed-out.");
                    break;
                }
                yield return new WaitForSeconds(1);
            }

            if (string.IsNullOrEmpty(output) || output.Contains("\"message\": \"Please provide a valid API Key\"") || output.Contains("Unknown WebSocket error"))
            {
                userInfo = null;
                isActive = false;
                yield break;
            }

            userInfo = new UserInfo();
            
            string[] arr = ReadMetadataToArray(output);
            foreach (string s in arr)
            {
                try
                {
                    if (s.Contains("name"))
                    {
                        userInfo.Name = s.Split(':')[1].Replace("\"", "").Replace(",", "").Trim();
                    }

                    if (s.Contains("profile_url"))
                    {
                        string[] splitted = s.Split(':');
                        userInfo.ProfilePic = ("https:" + splitted[2]).Replace("\"", "").Replace(",", "").Trim();
                    }

                    if (s.Contains("is_premium?"))
                    {
                        userInfo.IsPremium = s.Contains("true");
                    }

                    if (s.Contains("is_supporter?"))
                    {
                        userInfo.IsSupporter = s.Contains("true");
                    }
                }
                catch (Exception ex)
                {
                    ui.loggedIn.text = "<color=red>FAILED TO GET USER INFO :(</color>";
                    ui.hoverText.oldText = "<color=yellow>LOG IN</color>";
                    ui.hoverText.newText = "<color=yellow>LOG IN</color>";
                    ui.userName.text = "";
                    ui.memberStatus.text = "";
                    ModConsole.LogError(ex.ToString());
                }
            }

            if (!Directory.Exists(NexusDataFolder))
            {
                Directory.CreateDirectory(NexusDataFolder);
            }

            ui.loggedIn.text = "<color=lime>LOGGED IN</color>";
            ui.userName.text = userInfo.Name;
            ui.memberStatus.text = userInfo.IsPremium ? "PREMIUM" : "NON-PREMIUM";
            ui.hoverText.oldText = "<color=lime>LOGGED IN</color>";
            ui.hoverText.newText = "<color=red>LOG OUT</color>";

            // Download profile pic.
            string pfpPath = Path.Combine(NexusDataFolder, userInfo.Name + ".png");

            if (File.Exists(pfpPath))
            {
                FileInfo fi = new FileInfo(pfpPath);
                DateTime weekAgo = DateTime.Now.AddDays(-7);
                if (fi.LastWriteTime < weekAgo)
                {
                    ModConsole.Log("[Nexus SSO] Refreshing the user profile picture!");
                    forceDownloadNewPfp = true;
                }
            }

            if (!File.Exists(pfpPath) || forceDownloadNewPfp)
            {
                forceDownloadNewPfp = false;
                pfpPath = "\"" + pfpPath + "\"";
                p = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ModUpdater.UpdaterPath,
                        Arguments = "get-file " + string.Join(" ", new string[] { userInfo.ProfilePic, pfpPath, apiKey }),
                        WorkingDirectory = ModUpdater.UpdaterDirectory,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                downloadTime = 0;
                while (!p.HasExited)
                {
                    downloadTime++;
                    if (downloadTime > 10)
                    {
                        ModConsole.LogError($"[Nexus SSO] Getting profile pic timed-out.");
                        break;
                    }
                    yield return new WaitForSeconds(1);
                }
                SetProfilePic();
            }
            else
            {
                SetProfilePic();
            }

            isActive = false;
            isReady = true;
        }

        static string[] ReadMetadataToArray(string input)
        {
            return string.IsNullOrEmpty(input) ? new string[] { "" } : input.Split('\n');
        }

        void SetProfilePic()
        {
            ui.profilePicture.texture = ModAssets.LoadTexturePNG(Path.Combine(NexusDataFolder, userInfo.Name + ".png"));
        }
    }
}