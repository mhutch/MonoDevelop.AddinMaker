using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Addins;
using Mono.Addins.Database;

namespace MonoDevelop.Addins.Tasks
{
	public abstract class AddinTask : Task
	{
		[Required]
		public string ConfigDir { get; set; }

		[Required]
		public string AddinsDir { get; set; }

		[Required]
		public string DatabaseDir { get; set; }

		[Required]
		public string BinDir { get; set; }

		protected bool InitializeAddinRegistry ()
		{
			if (string.IsNullOrEmpty (ConfigDir))
				Log.LogError ("ConfigDir must be specified");

			if (string.IsNullOrEmpty (AddinsDir))
				Log.LogError ("AddinsDir must be specified");

			if (string.IsNullOrEmpty (DatabaseDir))
				Log.LogError ("DatabaseDir must be specified");

			if (string.IsNullOrEmpty (BinDir))
				Log.LogError ("BinDir must be specified");

			ConfigDir = Path.GetFullPath (ConfigDir);
			BinDir = Path.GetFullPath (BinDir);
			AddinsDir = Path.GetFullPath (AddinsDir);
			DatabaseDir = Path.GetFullPath (DatabaseDir);

			bool rebuild = false;

			//the registry can get confused if we switch bindirs
			var markerFile = Path.Combine (DatabaseDir, "lastbin.txt");
			if (Directory.Exists (DatabaseDir)) {
				if (!File.Exists (markerFile) || File.ReadAllText (markerFile) != BinDir) {
					rebuild = true;
				}
			}

			Registry = new AddinRegistry (
				ConfigDir,
				BinDir,
				AddinsDir,
				DatabaseDir
			);

			Registry.RegisterExtension (new CecilReflectorExtension ());

			var progress = new LogProgressStatus (Log, 2);
			if (rebuild) {
				Log.LogMessage (MessageImportance.Normal, "Rebuilding addin database at {0}", DatabaseDir);
				Registry.Rebuild (progress);
			} else {
				Log.LogMessage (MessageImportance.Normal, "Updating addin database at {0}", DatabaseDir);
				Registry.Update (progress);
			}

			File.WriteAllText (markerFile, BinDir);

			return !Log.HasLoggedErrors;
		}

		protected AddinRegistry Registry { get; private set; }
	}

	class CecilReflectorExtension : AddinFileSystemExtension
	{
		//force it to use the cecil reflector. rhe SR one breaks easily under MSBuild
		public override IAssemblyReflector GetReflectorForFile (IAssemblyLocator locator, string path)
		{
			string asmFile = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Mono.Addins.CecilReflector.dll");
			Assembly asm = Assembly.LoadFrom (asmFile);
			Type t = asm.GetType ("Mono.Addins.CecilReflector.Reflector");
			var reflector = (IAssemblyReflector)Activator.CreateInstance (t);
			reflector.Initialize (locator);
			return reflector;
		}

		//mono.addins uses an appdomain even when using the cecil reflector
		//but that breaks us because the appdomain's base directory is the msbuild
		//bindir so it can't load the assembly. force it to run inproc instead.
		public override bool RequiresIsolation {
			get { return false; }
		}
	}
}

