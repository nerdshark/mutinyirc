﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrtzIRC.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ServerStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ServerStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OrtzIRC.Resources.ServerStrings", typeof(ServerStrings).Assembly);
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
        ///   Looks up a localized string similar to Attempting to reconnect....
        /// </summary>
        internal static string AttemptingReconnect {
            get {
                return ResourceManager.GetString("AttemptingReconnect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connecting to {0} ({1})....
        /// </summary>
        internal static string ConnectingMessage {
            get {
                return ResourceManager.GetString("ConnectingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not connect: {0}.
        /// </summary>
        internal static string ConnectionFailedMessage {
            get {
                return ResourceManager.GetString("ConnectionFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --Disconnected--.
        /// </summary>
        internal static string Disconnected {
            get {
                return ResourceManager.GetString("Disconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --Disconnected: {0} (Socket error {1}).
        /// </summary>
        internal static string DisconnectSocketError {
            get {
                return ResourceManager.GetString("DisconnectSocketError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The nick &apos;{0}&apos; was taken. Trying &apos;{1}&apos;.
        /// </summary>
        internal static string NickTakenMessage {
            get {
                return ResourceManager.GetString("NickTakenMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All of your alternate nicks were taken, so one was randomly chosen for you..
        /// </summary>
        internal static string RandomNickMessage {
            get {
                return ResourceManager.GetString("RandomNickMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} {1}.
        /// </summary>
        internal static string ServerErrorMessage {
            get {
                return ResourceManager.GetString("ServerErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Status: {0} on {1} ({2}:{3}).
        /// </summary>
        internal static string ServerFormTitleBar {
            get {
                return ResourceManager.GetString("ServerFormTitleBar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This will disconnect you from {0}?.
        /// </summary>
        internal static string WarnDisconnect {
            get {
                return ResourceManager.GetString("WarnDisconnect", resourceCulture);
            }
        }
    }
}
