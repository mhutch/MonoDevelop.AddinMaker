using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Addins;
using Mono.Addins.Description;

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

		public string CoreVersionOverride { get; set; }

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

				string version = null;

				var versionMetadata = addinName.GetMetadata ("Version");
				if (!string.IsNullOrEmpty (versionMetadata)) {
					version = versionMetadata;
					Log.LogMessage (MessageImportance.Normal, "Overriding '{0}' reference version with '{1}'", addinName, version);
					if (version != null && !addin.SupportsVersion (version)) {
						Log.LogWarning (
							"Addin '{0}' version '{1}' does not appear to be compatible with version override '{2}'",
							addinName, addin.Version, version
						);
					}
				} else if (!string.IsNullOrEmpty (CoreVersionOverride)) {
					if (addin.Namespace == "MonoDevelop" && addin.LocalId == "Core") {
						if (!string.IsNullOrEmpty (CoreVersionOverride)) {
							version = CoreVersionOverride;
							Log.LogMessage (MessageImportance.Normal, "Overriding '{0}' reference version with '{1}'", addinName.ItemSpec, version);
						}
					} else {
						var coreDep = GetCoreDependencyVersion (addin);
						if (coreDep == null) {
							Log.LogError ("Could not resolve core dependency version for '{0}'", addinName.ItemSpec);
							success = false;
							continue;
						}
						if (coreDep == addin.Version) {
							version = CoreVersionOverride;
							Log.LogMessage (MessageImportance.Normal, "Overriding '{0}' reference version with '{1}'", addinName.ItemSpec, version);
						}
					}
				}

				if (version == null) {
					version = addin.Version;
					Log.LogMessage (MessageImportance.Normal, "Resolved '{0}' reference to version '{1}'", addinName.ItemSpec, version);
				}

				var item = new TaskItem (addinName);
				item.SetMetadata ("Version", version);
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

		string GetCoreDependencyVersion (Addin addin)
		{
			foreach (AddinDependency dep in addin.Description.MainModule.Dependencies) {
				if (Addin.GetFullId (addin.Namespace, dep.AddinId, null) == "MonoDevelop.Core"){
					return dep.Version;
				}
			}
			return null;
		}

		void CollectCoreReferences (string addinAssembly, Dictionary<string,string> coreReferences)
		{
			using (var asm = Mono.Cecil.AssemblyDefinition.ReadAssembly (addinAssembly)) {
				foreach (var r in asm.MainModule.AssemblyReferences) {
					if (coreReferences.ContainsKey (r.Name))
						continue;
					var p = Path.Combine (BinDir, r.Name + ".dll");
					if (!File.Exists (p)) {
						continue;
					}

					var fullPath = Path.GetFullPath (p);
					coreReferences.Add (r.Name, fullPath);
					Log.LogMessage (MessageImportance.Low, "Added transitive reference '{0}'", r);
					CollectCoreReferences (fullPath, coreReferences);
				}
			}
		}
	}
}

