﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MSCLoader.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MSCLoader.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Version 1.0.8
        ///	Changed the Keybind setting to allow binding the left and right mousebutton.
        ///	Canceling and deleting keybinds is now done by buttons appearing when binding.
        ///	Deprecated the PartMagnet, BoltMagnet does the exact same thing with no bolts assigned.
        ///	Added &quot;Sources.txt&quot; file in ModUpdater folder - enables mods that don&apos;t natively support Mod Loader Pro to have auto updates
        ///	Mods not natively supported by Mod Loader Pro (and have been added into Sources.txt) will have their info downloaded fr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Changelog {
            get {
                return ResourceManager.GetString("Changelog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] mscloadercanvas {
            get {
                object obj = ResourceManager.GetObject("mscloadercanvas", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to // This file contains sources of mod updates for mods that are not made for Mod Loader Pro.
        ///// If you want to add your own sources, DO NOT DO THAT HERE! This file will be overwritten on each update!
        ///// If you want to add custom sources, do that in UserSources.txt!
        ///
        ///BetterFPSCounter https://www.nexusmods.com/mysummercar/mods/378
        ///DeveloperToolsetII https://www.nexusmods.com/mysummercar/mods/345
        ///FifthGear https://www.nexusmods.com/mysummercar/mods/221
        ///MOP https://github.com/Athlon007/MOP
        ///CDPlayer https [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Sources {
            get {
                return ResourceManager.GetString("Sources", resourceCulture);
            }
        }
    }
}
