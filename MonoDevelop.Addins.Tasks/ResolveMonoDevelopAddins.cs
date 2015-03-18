using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Addins;

namespace MonoDevelop.Addins.Tasks
{
	public class ResolveMonoDevelopAddins : AddinTask
	{
		[Required]
		public ITaskItem[] AddinReferences { get; set; }

		[Output]
		public ITaskItem[] AssemblyReferences { get; set; }

		[Output]
		public ITaskItem[] ResolvedAddins { get; set; }

		[Output]
		public string VersionDefines { get; set; }

		public override bool Execute ()
		{
			if (!InitializeAddinRegistry ())
				return false;

			var assemblies = new List<string> ();

			bool success = true;

			var resolvedAddins = new List<ITaskItem> ();

			foreach (var addinName in AddinReferences) {
				var addin = Registry.GetAddin (addinName.ItemSpec);
				if (addin == null) {
					Log.LogError ("Could not resolve addin reference '{0}'", addinName);
					success = false;
					continue;
				}

				var expectedVersion = addinName.GetMetadata ("Version");
				if (!string.IsNullOrEmpty (expectedVersion) && !addin.SupportsVersion (expectedVersion)) {
					Log.LogError ("Addin '{0}' does not satisfy specified version '{1}", addinName, expectedVersion);
					success = false;
					continue;
				}

				var item = new TaskItem (addinName);
				item.SetMetadata ("Version", addin.Version);
				item.SetMetadata ("AddinFile", addin.AddinFile);
				resolvedAddins.Add (item);

				Log.LogMessage (MessageImportance.Low, "Resolving assemblies for addin {0}", addinName);

				//TODO: reference optional modules if this addin depends on the addins they depend on
				var dir = Path.GetDirectoryName (addin.AddinFile);
				foreach (var a in addin.Description.MainModule.Assemblies) {
					var addinAssemblyPath = Path.Combine (dir, a);
					if (!File.Exists (addinAssemblyPath)) {
						Log.LogWarning ("Could not find assembly '{0}' in addin '{1}'", addinAssemblyPath, addinName);
						continue;
					}

					assemblies.Add (addinAssemblyPath);
					Log.LogMessage (MessageImportance.Low, "Found addin assembly at path '{0}'", addinAssemblyPath);
				}
			}

			//TODO: define a basic range of constants for minor versions between Version and CompatVersion
			var core = Registry.GetAddin ("MonoDevelop.Core");
			if (core != null) {
				VersionDefines = "MD_" + core.Description.CompatVersion.Replace ('.', '_');
			}

			AssemblyReferences = assemblies.Select (a => {
				var item = new TaskItem (Path.GetFileNameWithoutExtension (a));
				item.SetMetadata ("Private", "False");
				item.SetMetadata ("HintPath", a);
				return item;
			}).ToArray ();

			ResolvedAddins = resolvedAddins.ToArray ();

			return success;
		}
	}
}

