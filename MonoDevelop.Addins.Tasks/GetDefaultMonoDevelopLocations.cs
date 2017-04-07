using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MonoDevelop.Addins.Tasks
{
	public class GetDefaultMonoDevelopLocations : Task
	{
		[Required]
		public string ProfileVersion { get; set; }

		public string ProfileName { get; set; }

		public string ProfilePath { get; set; }

		public string ReferencePath { get; set; }

		// Based on logic in MonoDevelop.Core.UserProfile
		public override bool Execute ()
		{
			//HACK to allow building on addins.monodevelop.com
			if (ReferencePath != null && ReferencePath.IndexOf ("cydin-files/AppReleases", StringComparison.Ordinal) > 0) {
				BinDir = ReferencePath;
				return true;
			}

			string profileID = ProfileName;

			var isPreV7 = string.Equals (ProfileVersion, "6.0", StringComparison.Ordinal);

			if (isPreV7) {
				if (string.IsNullOrEmpty (profileID)) {
					if (Platform.IsWindows || Platform.IsMac) {
						profileID = "XamarinStudio";
					} else {
						profileID = "MonoDevelop";
					}
				}
				profileID = profileID + "-" + ProfileVersion;
			} else {
				if (string.IsNullOrEmpty (profileID)) {
					if (Platform.IsMac) {
						profileID = "VisualStudio";
					} else {
						profileID = "MonoDevelop";
					}
				}
				profileID = Path.Combine (profileID, ProfileVersion);
			}

			if (!string.IsNullOrEmpty (ProfilePath)) {
				ConfigDir = Path.Combine (ProfilePath, profileID, "Config");
				AddinsDir = Path.Combine (ProfilePath, profileID, "LocalInstall", "Addins");
				DatabaseDir = Path.Combine (ProfilePath, profileID, "Cache");
			} else if (Platform.IsWindows) {
				string local = Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData);
				string roaming = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
				ConfigDir = Path.Combine (roaming, profileID, "Config");
				AddinsDir = Path.Combine (local, profileID, "LocalInstall", "Addins");
				DatabaseDir = Path.Combine (local, profileID, "Cache");
			} else if (Platform.IsMac) {
				string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				string library = Path.Combine (home, "Library");
				ConfigDir = Path.Combine (library, "Preferences", profileID);
				AddinsDir = Path.Combine (library, "Application Support", profileID, "LocalInstall", "Addins");
				DatabaseDir = Path.Combine (library, "Caches", profileID);
			} else {
				string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				string xdgDataHome = Environment.GetEnvironmentVariable ("XDG_DATA_HOME");
				if (string.IsNullOrEmpty (xdgDataHome))
					xdgDataHome = Path.Combine (home, ".local", "share");
				string xdgConfigHome = Environment.GetEnvironmentVariable ("XDG_CONFIG_HOME");
				if (string.IsNullOrEmpty (xdgConfigHome))
					xdgConfigHome = Path.Combine (home, ".config");
				string xdgCacheHome = Environment.GetEnvironmentVariable ("XDG_CACHE_HOME");
				if (string.IsNullOrEmpty (xdgCacheHome))
					xdgCacheHome = Path.Combine (home, ".cache");
				ConfigDir = Path.Combine (xdgConfigHome, profileID);
				AddinsDir = Path.Combine (xdgDataHome, profileID, "LocalInstall");
				DatabaseDir = Path.Combine (xdgCacheHome, profileID);
			}

			if (Platform.IsWindows) {
				BinDir = Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.ProgramFilesX86),
					"Xamarin Studio", "bin"
				);
			} else if (Platform.IsMac) {
				string appName = isPreV7? "Xamarin Studio" : "Visual Studio";
				BinDir = $"/Applications/{appName}.app/Contents/Resources/lib/monodevelop/bin";
				//fall back to old pre-Yosemite location
				if (!Directory.Exists (BinDir)) {
					BinDir = $"/Applications/{appName}.app/Contents/MacOS/lib/monodevelop/bin";
				}
			} else {
				BinDir = "/usr/lib/monodevelop/bin";
			}

			//TODO: check all locations are valid
			if (!File.Exists (Path.Combine (BinDir, "MonoDevelop.Ide.dll"))) {
				Log.LogError ("MonoDevelop location not found");
			}
			return true;
		}

		[Output]
		public string ConfigDir { get; set; }

		[Output]
		public string AddinsDir { get; set; }

		[Output]
		public string DatabaseDir { get; set; }

		[Output]
		public string BinDir { get; set; }
	}
}
