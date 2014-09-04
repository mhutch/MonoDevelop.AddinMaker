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

		// Based on logic in MonoDevelop.Core.UserProfile
		public override bool Execute ()
		{
			string profileID = ProfileName;

			if (string.IsNullOrEmpty (profileID)) {
				if (Platform.IsWindows || Platform.IsMac) {
					profileID = "XamarinStudio";
				} else {
					profileID = "MonoDevelop";
				}
			}

			profileID = profileID + "-" + ProfileVersion;

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
				InstallRoot = Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.ProgramFilesX86),
					"Xamarin Studio"
				);
			} else if (Platform.IsMac) {
				InstallRoot = "/Applications/Xamarin Studio.app/Contents/MacOS/lib/monodevelop";
			} else {
				InstallRoot = "/usr/lib/monodevelop";
			}

			//TODO: check locations are valid
			return true;
		}

		[Output]
		public string ConfigDir { get; set; }

		[Output]
		public string AddinsDir { get; set; }

		[Output]
		public string DatabaseDir { get; set; }

		[Output]
		public string InstallRoot { get; set; }
	}
}