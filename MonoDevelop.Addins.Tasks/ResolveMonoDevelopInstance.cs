using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace MonoDevelop.Addins.Tasks
{
	public class ResolveMonoDevelopInstance : Task
	{
		//use a custom user profile, typically set by $MONODEVELOP_TEST_PROFILE
		public string ProfilePath { get; set; }

		[Output]
		//the bin directory of the target instance. if a value is not provided,
		//it will be resolved from the global installation.
		public string BinDir { get; set; }

		[Output]
		//the app directory of the target instance. this is a convenience
		//to compute an BinDir value.
		public string AppDir { get; set; }

		[Output]
		//the profile version of the target version. if a value is not provided,
		//it will be determined from the target instance
		public string ProfileVersion { get; set; }

		[Output]
		// the moniker of the framework targeted by the target instance
		public string TargetFrameworkMoniker { get; set; }

		[Output]
		// the version of the framework targeted by the target instance
		public string TargetFrameworkVersion { get; set; }

		[Output]
		// the config directory of the target user profile
		public string ConfigDir { get; set; }

		[Output]
		// the addins directory of the target user profile
		public string AddinsDir { get; set; }

		[Output]
		// the database directory of the target user profile
		public string DatabaseDir { get; set; }

		// Based on logic in MonoDevelop.Core.UserProfile
		public override bool Execute ()
		{
			if (!ResolveBinDir ()) {
				return false;
			}

			string targetFrameworkMoniker, compatVersion;
			using (var asm = AssemblyDefinition.ReadAssembly (Path.Combine (BinDir, "MonoDevelop.Core.dll"))) {
				GetAttributes (asm, out targetFrameworkMoniker, out compatVersion);
			}

			if (targetFrameworkMoniker == null || !ParseTargetFrameworkMoniker (targetFrameworkMoniker, out string fxIdentifier, out string fxVersion, out string fxProfile)) {
				Log.LogError ("TargetFrameworkAttribute is missing or invalid in MonoDevelop.Core.dll");
				return false;
			}

			if (compatVersion == null) {
				Log.LogError ("AddinRootAttribute.CompatValue is missing or invalid in MonoDevelop.Core.dll");
				return false;
			}

			if (!string.Equals (ProfileVersion, compatVersion)) {
				Log.LogWarning ($"The provided ProfileVersion '{ProfileVersion}' does not match the target instance. Overriding with '{compatVersion}'.");
			}

			if (!string.Equals (TargetFrameworkVersion, fxVersion)) {
				Log.LogWarning ($"The TargetFrameworkVersion '{TargetFrameworkVersion}' does not the target instance. Overriding with '{fxVersion}'.");
			}

			ProfileVersion = compatVersion;
			TargetFrameworkVersion = fxVersion;
			TargetFrameworkMoniker = targetFrameworkMoniker;

			ResolveProfile ();

			return true;
		}

		bool ResolveBinDir ()
		{
			bool MDCoreExists() => File.Exists (Path.Combine (BinDir, "MonoDevelop.Core.dll"));

			if (!string.IsNullOrEmpty (AppDir)) {
				BinDir = Path.Combine (AppDir, "Contents", "Resources", "lib", "monodevelop", "bin");
				if (!MDCoreExists ()) {
					Log.LogError ("The provided MDAppDir value does not point to a valid instance of instance of Visual Studio for Mac or MonoDevelop.");
					return false;
				}
				return true;
			}

			if (!string.IsNullOrEmpty (BinDir)) {
				if (!MDCoreExists ()) {
					Log.LogError ("The provided MDBinDir value does not point to a valid instance of instance of Visual Studio for Mac or MonoDevelop.");
					return false;
				}
				return true;
			}

			if (Platform.IsWindows) {
				BinDir = Path.Combine (
				   Environment.GetFolderPath (Environment.SpecialFolder.ProgramFilesX86),
					"Xamarin Studio", "bin"
				);
			} else if (Platform.IsMac) {
				string appName = "Visual Studio";
				BinDir = $"/Applications/{appName}.app/Contents/Resources/lib/monodevelop/bin";
				//fall back to old pre-Yosemite location
				if (!Directory.Exists (BinDir)) {
					BinDir = $"/Applications/{appName}.app/Contents/MacOS/lib/monodevelop/bin";
				}
			} else {
				BinDir = "/usr/lib/monodevelop/bin";
			}

			if (!MDCoreExists ()) {
				Log.LogError ("Could not find a global instance of Visual Studio for Mac or MonoDevelop. Try providing an explicit value with MDBinDir.");
				return false;
			}

			return true;
		}

		void ResolveProfile ()
		{
			string profileID = Platform.IsMac ? "VisualStudio" : "MonoDevelop";
			profileID = Path.Combine (profileID, ProfileVersion);

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
		}

		static void GetAttributes (AssemblyDefinition asm, out string targetFramework, out string compatVersion)
		{
			targetFramework = compatVersion = null;
			foreach (var att in asm.MainModule.GetCustomAttributes ()) {
				switch (att.AttributeType.FullName) {
				case "System.Runtime.Versioning.TargetFrameworkAttribute":
					targetFramework = (string)att.ConstructorArguments [0].Value;
					break;
				case "Mono.Addins.AddinRootAttribute":
					compatVersion = (string)att.Properties.FirstOrDefault (f => f.Name == "CompatVersion").Argument.Value;
					break;
				} 
			}
		}

		//Based on MonoDevelop's TargetFrameworkMoniker class
		static bool ParseTargetFrameworkMoniker (string tfm, out string identifier, out string version, out string profile)
		{
			const string versionSeparator = ",Version=";
			const string profileSeparator = ",Profile=";

			profile = null;
			version = null;

			int versionIdx = tfm.IndexOf (',');

			identifier = tfm.Substring (0, versionIdx);

			if (tfm.IndexOf (versionSeparator, versionIdx, versionSeparator.Length, StringComparison.Ordinal) != versionIdx) {
				return false;
			}
			versionIdx += versionSeparator.Length;

			int profileIdx = tfm.IndexOf (',', versionIdx);
			if (profileIdx < 0) {
				version = tfm.Substring (versionIdx);
			} else {
				version = tfm.Substring (versionIdx, profileIdx - versionIdx);
				profile = tfm.Substring (profileIdx + profileSeparator.Length);
			}

			return Version.TryParse (version [0] == 'v' ? version.Substring (1) : version, out Version v);
		}
	}
}
