﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrtzIRC.WPF.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ChannelStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ChannelStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OrtzIRC.WPF.Resources.ChannelStrings", typeof(ChannelStrings).Assembly);
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
        ///   Looks up a localized string similar to -- {0} {1}.
        /// </summary>
        internal static string Action {
            get {
                return ResourceManager.GetString("Action", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string ErrorMessageCaption {
            get {
                return ResourceManager.GetString("ErrorMessageCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Joined: ({0}) ({1}).
        /// </summary>
        internal static string Joined {
            get {
                return ResourceManager.GetString("Joined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Kick: ({0}) was kicked by ({1}) {2}.
        /// </summary>
        internal static string Kick {
            get {
                return ResourceManager.GetString("Kick", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Nick: ({0}) is now known as ({1}).
        /// </summary>
        internal static string NickChange {
            get {
                return ResourceManager.GetString("NickChange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Parted: ({0}) ({1}).
        /// </summary>
        internal static string Part {
            get {
                return ResourceManager.GetString("Part", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Parted: ({0}) ({1}) {2}.
        /// </summary>
        internal static string PartWithReason {
            get {
                return ResourceManager.GetString("PartWithReason", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: {1}.
        /// </summary>
        internal static string PublicMessage {
            get {
                return ResourceManager.GetString("PublicMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Quit: ({0}) ({1}) {2}.
        /// </summary>
        internal static string Quit {
            get {
                return ResourceManager.GetString("Quit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Topic: ({0}).
        /// </summary>
        internal static string TopicRecieved {
            get {
                return ResourceManager.GetString("TopicRecieved", resourceCulture);
            }
        }
    }
}
