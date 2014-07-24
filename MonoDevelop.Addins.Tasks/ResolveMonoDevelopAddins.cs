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

			var reg = new AddinRegistry (
				ConfigDir,
				Path.Combine (InstallRoot, "bin"),
				AddinsDir,
				DatabaseDir
			);

			bool success = true;

			var resolvedDeps = new HashSet<AssemblyName> ();
			var unresolvedDeps = new Dictionary<AssemblyName,string> ();

			foreach (var addinName in AddinReferences) {
				//TODO: respect versions
				var addin = reg.GetAddin (addinName.ItemSpec);
				if (addin == null) {
					Log.LogError ("Could not resolve addin reference {0}", addinName);
					success = false;
					continue;
				}

				//TODO: reference optional modules if this addin depends on the addins they depend on
				var dir = Path.GetDirectoryName (addin.AddinFile);
				foreach (var asmFile in addin.Description.MainModule.Assemblies) {
					var asmFilePath = Path.Combine (dir, asmFile);
					if (!File.Exists (asmFilePath)) {
						Log.LogWarning ("Could not find assembly '{0}' in addin '{1}'", asmFilePath, addinName);
						continue;
					}
					assemblies.Add (asmFilePath);

					//also reference indirect dependencies one hop out
					//TODO: cache the resolve map
					Assembly asm = Assembly.ReflectionOnlyLoadFrom (asmFilePath);
					var asmName = asm.GetName ();
					unresolvedDeps.Remove (asmName);
					resolvedDeps.Add (asmName);

					var searchPath = Path.GetDirectoryName (asmFilePath);
					foreach (var refName in asm.GetReferencedAssemblies ()) {
						if (resolvedDeps.Contains (refName))
							continue;
						var possible = Path.Combine (searchPath, refName.Name + ".dll");
						if (File.Exists (possible)) {
							assemblies.Add (possible);
							resolvedDeps.Add (refName);
							unresolvedDeps.Remove (refName);
							continue;
						}
						Assembly gacAsm;
						try {
							gacAsm = Assembly.ReflectionOnlyLoad (asmName.ToString ());
							if (gacAsm != null) {
								resolvedDeps.Add (refName);
								unresolvedDeps.Remove (refName);
								assemblies.Add (gacAsm.Location);
								continue;
							}
						} catch (FileNotFoundException) {
						}
						unresolvedDeps[refName] = addinName.ItemSpec;
					}
				}
			}

			foreach (var u in unresolvedDeps) {
				Log.LogWarning ("Could not find assembly '{0}' referenced by addin '{1}'", u.Key, u.Value);
			}


			AssemblyReferences = assemblies.Select (a => {
				var item = new TaskItem (a);
				item.SetMetadata ("Private", "False");
				return item;
			}).ToArray ();

			return success;
		}
	}
}

