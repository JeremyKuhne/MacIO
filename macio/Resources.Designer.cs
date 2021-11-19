﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MacIO {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MacIO.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Lists drive image zero block information
        ///
        /// &gt; macio blockzero {driveImagePath}.
        /// </summary>
        internal static string HelpBlockZero {
            get {
                return ResourceManager.GetString("HelpBlockZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generates an MD5 checksum for the specified file
        ///
        /// &gt; macio checksum {path}
        ///
        ///The result is a UUEncoded MD5 checksum..
        /// </summary>
        internal static string HelpChecksum {
            get {
                return ResourceManager.GetString("HelpChecksum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creates a blank drive image of a specfied size
        ///
        /// &gt; macio createimage {driveImagePath} -size:{size}[KB|MB|GB]
        ///
        ///This creates an image with a 7.3.5 driver partition and a &quot;Free&quot; partition occupying the rest of the drive..
        /// </summary>
        internal static string HelpCreateImage {
            get {
                return ResourceManager.GetString("HelpCreateImage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -size   The size of the image to create.
        /// </summary>
        internal static string HelpCreateImageOptions {
            get {
                return ResourceManager.GetString("HelpCreateImageOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extracts the specified driver
        ///
        /// &gt; macio extractdriver {driveImagePath} [{destinationPath}] [index:{index}]
        ///
        ///This extracts only the bytes actually used by the given driver, not the entire driver partition. If {destinationPath} is not specified, will generate a filename. Will not overwrite existing files. Index defaults to zero..
        /// </summary>
        internal static string HelpExtractDriver {
            get {
                return ResourceManager.GetString("HelpExtractDriver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -index (-i) The driver index (as represented in block zero).
        /// </summary>
        internal static string HelpExtractDriverOptions {
            get {
                return ResourceManager.GetString("HelpExtractDriverOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extracts a partition to a file
        ///
        /// &gt; macio extractpartition {driveImagePath} [{destinationPath}] [-index:{index}]
        ///
        ///If {destinationPath} is not specified, will generate a filename. Will not overwrite existing files. Index defaults to zero..
        /// </summary>
        internal static string HelpExtractPartition {
            get {
                return ResourceManager.GetString("HelpExtractPartition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -index (-i) The partition to extract.
        /// </summary>
        internal static string HelpExtractPartitionOptions {
            get {
                return ResourceManager.GetString("HelpExtractPartitionOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lists drive image zero block information
        ///
        /// &gt; macio hfsinfo {driveOrVolumeImagePath} [-index:{index}]
        ///
        ///If the image has an expected zero block header then the given partition index is used. If the index isn&apos;t specified the first HFS partition is dumped. If the image is not a valid drive it is assumed to be an HFS volume..
        /// </summary>
        internal static string HelpHFSInfo {
            get {
                return ResourceManager.GetString("HelpHFSInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -index (-i) The partition to get info from.
        /// </summary>
        internal static string HelpHFSInfoOptions {
            get {
                return ResourceManager.GetString("HelpHFSInfoOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lists drive image partition info
        ///
        /// &gt; macio listpartitions {driveImage} [-index:{index}].
        /// </summary>
        internal static string HelpListPartitions {
            get {
                return ResourceManager.GetString("HelpListPartitions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -index (-i) Get detailed information for the specified partition.
        /// </summary>
        internal static string HelpListPartitionsOptions {
            get {
                return ResourceManager.GetString("HelpListPartitionsOptions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replaces a partition with a given image
        ///
        /// &gt; macio replacepartition {driveImagePath} {partitionImagePath} [-index:{index}]
        ///
        ///If the partition image is HFS, will prompt to update the partition table if needed. Index defaults to zero..
        /// </summary>
        internal static string HelpReplaceImage {
            get {
                return ResourceManager.GetString("HelpReplaceImage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -index (-i) The partition to replace.
        /// </summary>
        internal static string HelpReplaceImageOptions {
            get {
                return ResourceManager.GetString("HelpReplaceImageOptions", resourceCulture);
            }
        }
    }
}
