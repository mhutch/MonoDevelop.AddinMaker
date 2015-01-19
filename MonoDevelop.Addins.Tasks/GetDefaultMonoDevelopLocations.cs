using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using System.Collections.Generic;

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
				BinDir = Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.ProgramFilesX86),
					"Xamarin Studio", "bin"
				);
			} else if (Platform.IsMac) {
				BinDir = "/Applications/Xamarin Studio.app/Contents/Resources/lib/monodevelop/bin";
				//fall back to old pre-Yosemite location
				if (!Directory.Exists (BinDir)) {
					BinDir = "/Applications/Xamarin Studio.app/Contents/MacOS/lib/monodevelop/bin";
				}
			} else {
				BinDir = GetBinDir(Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"));
			}

			//TODO: check locations are valid
			return true;
		}

		private string GetBinDir(string libraryPaths) {
			//string libraryPaths = Environment.GetEnvironmentVariable ("LD_LIBRARY_PATH");
			List<string> tempDir = new List<string> ();
			string dir = "";

			int pos = 1;
			bool foundColon = false;


			if(libraryPaths.Contains("monodevelop/lib") && !libraryPaths.Contains("monodevelop/bin")) {
				while(!foundColon) {
					if(libraryPaths[libraryPaths.IndexOf("monodevelop/lib")-pos] != ':') {
						tempDir.Insert (0, libraryPaths [libraryPaths.IndexOf ("monodevelop/lib") - pos].ToString ());
						pos++;
					} else {
						foundColon = true;
						tempDir.Add ("monodevelop/lib/monodevelop/bin");
					}
				}
			}

			foreach(string s in tempDir) {
				dir += s;
			}

			return dir;
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
