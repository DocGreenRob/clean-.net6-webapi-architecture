﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CGE.CleanCode.Api {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CGE.CleanCode.Api.Resource", typeof(Resource).Assembly);
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
        ///   Looks up a localized string similar to Value must be Greater than or equal to 0.
        /// </summary>
        internal static string ERRGreaterThanEqualToZero {
            get {
                return ResourceManager.GetString("ERRGreaterThanEqualToZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot have a negative {PropertyName}.
        /// </summary>
        internal static string ERRGreaterThanOrEqualToZero {
            get {
                return ResourceManager.GetString("ERRGreaterThanOrEqualToZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be Greater than 0.
        /// </summary>
        internal static string ErrGreaterThanZero {
            get {
                return ResourceManager.GetString("ErrGreaterThanZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} should be a valid date..
        /// </summary>
        internal static string ERRInvalidDate {
            get {
                return ResourceManager.GetString("ERRInvalidDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} should be a valid email address..
        /// </summary>
        internal static string ERRInvalidEmail {
            get {
                return ResourceManager.GetString("ERRInvalidEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} is Invalid.
        /// </summary>
        internal static string ERRInvalidValue {
            get {
                return ResourceManager.GetString("ERRInvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} is Required.
        /// </summary>
        internal static string ERRIsRequired {
            get {
                return ResourceManager.GetString("ERRIsRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} cannot be more than 50 characters.
        /// </summary>
        internal static string ERRMaxLength50 {
            get {
                return ResourceManager.GetString("ERRMaxLength50", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} must be Numeric.
        /// </summary>
        internal static string ERRMustBeNumeric {
            get {
                return ResourceManager.GetString("ERRMustBeNumeric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported Patch operation.
        /// </summary>
        internal static string ERRUnsupportedOperation {
            get {
                return ResourceManager.GetString("ERRUnsupportedOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} should be 2 Characters.
        /// </summary>
        internal static string ERRValueLongerThan2 {
            get {
                return ResourceManager.GetString("ERRValueLongerThan2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {PropertyName} must be in collection.
        /// </summary>
        internal static string ERRValueMustBeInCollection {
            get {
                return ResourceManager.GetString("ERRValueMustBeInCollection", resourceCulture);
            }
        }
    }
}
