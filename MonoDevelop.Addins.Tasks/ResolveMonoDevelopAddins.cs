using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Mono.Addins;
using System.Reflection;

namespace MonoDevelop.Addins.Tasks
{
	public class ResolveMonoDevelopAddins : Task
	{
		[Required]
		public string ConfigDir { get; set; }

		[Required]
		public string AddinsDir { get; set; }

		[Required]
		public string DatabaseDir { get; set; }

		[Required]
		public string InstallRoot { get; set; }

		[Required]
		public ITaskItem[] AddinReferences { get; set; }

		[Output]
		public ITaskItem[] AssemblyReferences { get; set; }

		public override bool Execute ()
		{
			var assemblies = new List<string> ();

			var binDir = Path.GetFullPath (Path.Combine (InstallRoot, "bin"));

			var reg = new AddinRegistry (
				ConfigDir,
				binDir,
				AddinsDir,
				DatabaseDir
			);

			bool success = true;

			foreach (var addinName in AddinReferences) {
				//TODO: respect versions
				var addin = reg.GetAddin (addinName.ItemSpec);
				if (addin == null) {
					Log.LogError ("Could not resolve addin reference {0}", addinName);
					success = false;
					continue;
				}

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

			AssemblyReferences = assemblies.Select (a => {
				var item = new TaskItem (Path.GetFileNameWithoutExtension (a));
				item.SetMetadata ("Private", "False");
				item.SetMetadata ("HintPath", a);
				return item;
			}).ToArray ();

			return success;
		}
	}
}

