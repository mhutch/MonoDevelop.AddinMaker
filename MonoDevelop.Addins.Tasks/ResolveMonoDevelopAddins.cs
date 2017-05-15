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

			//core addins don't declare imports for assemblies in bin, and these can change between releases
			//so we inspect core addin assemblies' references and transitively add them all
			var coreReferences = new Dictionary<string,string> ();
			string coreRoot = Path.GetFullPath (Path.Combine (BinDir, "..")) + Path.DirectorySeparatorChar;

			foreach (var addinName in AddinReferences) {
				var addin = Registry.GetAddin (addinName.ItemSpec);
				if (addin == null) {
					Log.LogError ("Could not resolve addin reference '{0}'", addinName);
					success = false;
					continue;
				}

				var overrideVersion = addinName.GetMetadata ("Version");
				if (string.IsNullOrEmpty (overrideVersion)) {
					overrideVersion = null;
				}

				if (overrideVersion != null && !addin.SupportsVersion (overrideVersion)) {
					Log.LogWarning (
						"Addin '{0}' version '{1}' does not appear to be compatible with version override '{2}'",
						addinName, addin.Version, overrideVersion
					);
				}

				var item = new TaskItem (addinName);
				item.SetMetadata ("Version", overrideVersion ?? addin.Version);
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

					//for assemblies that ship in MD, search their dependencies and add references
					if (addinAssemblyPath.StartsWith (coreRoot, StringComparison.OrdinalIgnoreCase)) {
						CollectCoreReferences (addinAssemblyPath, coreReferences);
					}
				}
			}

			//TODO: define a basic range of constants for minor versions between Version and CompatVersion
			var core = Registry.GetAddin ("MonoDevelop.Core");
			if (core != null) {
				VersionDefines = "MD_" + core.Description.CompatVersion.Replace ('.', '_');
			}

			AssemblyReferences = (assemblies.Concat (coreReferences.Values.Where (v => v != null))).Select (a => {
				var item = new TaskItem (Path.GetFileNameWithoutExtension (a));
				item.SetMetadata ("Private", "False");
				item.SetMetadata ("HintPath", a);
				return item;
			}).ToArray ();

			ResolvedAddins = resolvedAddins.ToArray ();

			return success;
		}

		void CollectCoreReferences (string addinAssembly, Dictionary<string,string> coreReferences)
		{
			var asm = Assembly.ReflectionOnlyLoadFrom (addinAssembly);
			var referenced = asm.GetReferencedAssemblies ();
			foreach (var r in referenced) {
				if (coreReferences.ContainsKey (r.Name))
					continue;
				var p = Path.Combine (BinDir, r.Name + ".dll");
				if (File.Exists (p)) {
					var fullPath = Path.GetFullPath (p);
					coreReferences.Add (r.Name, fullPath);
					Log.LogMessage (MessageImportance.Low, "Added transitive reference '{0}'", r);
					CollectCoreReferences (fullPath, coreReferences);
				} else {
					coreReferences.Add (r.Name, null);
				}
			}
		}
	}
}

