using System;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace MonoDevelop.Addins.Tasks
{
	public class GetMonoDevelopTargetFramework : Task
	{
		[Required]
		public string MDBinDir { get; set; }

		[Output]
		public string MDTargetFrameworkMoniker { get; set; }

		[Output]
		public string MDTargetFrameworkVersion { get; set; }

		public override bool Execute ()
		{
			var mdCoreDll = Path.Combine (MDBinDir, "MonoDevelop.Core.dll");
			if (!File.Exists (mdCoreDll)) {
				Log.LogError ("Invalid MDBinDir value");
				return false;
			}

			using (var asm = Mono.Cecil.AssemblyDefinition.ReadAssembly (mdCoreDll)) {
				MDTargetFrameworkMoniker = GetTargetFramework (asm);
			}

			if (!ParseTargetFrameworkMoniker (MDTargetFrameworkMoniker, out string identifier, out string version, out string profile)) {
				Log.LogError ("TargetFrameworkAttribute is missing or invalid");
				return false;
			}
			MDTargetFrameworkVersion = version;
			return true;
		}

		string GetTargetFramework (AssemblyDefinition asm)
		{
			foreach (var att in asm.MainModule.GetCustomAttributes ()) {
				if (att.AttributeType.FullName == typeof (TargetFrameworkAttribute).FullName) {
					 return (string)att.ConstructorArguments [0].Value;
				}
			}
			throw new InvalidOperationException ($"Assembly {asm.MainModule.FileName} does not have a TargetFrameworkAttribute");
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

			return Version.TryParse (version[0] == 'v'? version.Substring (1) : version, out Version v);
		}
	}
}
