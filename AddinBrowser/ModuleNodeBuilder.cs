using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;
using System.Collections.Generic;
using System.Linq;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ModuleNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ModuleDescription); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Module";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			//modules do no have names/IDs, but genrally the only reason they exist
			//is because they have additional, optional dependencies
			//so find the dependencies that are not referenced in other modules
			//and use one as the label
			var module = (ModuleDescription)dataObject;
			var deps = new HashSet<string> ();
			foreach (Dependency dep in module.Dependencies) {
				deps.Add (dep.Name);
			}

			foreach (ModuleDescription other in module.ParentAddinDescription.AllModules) {
				if (other == module) {
					continue;
				}
				foreach (Dependency dep in other.Dependencies) {
					deps.Remove (dep.Name);
				}
			}

			if (deps.Count > 0) {
				nodeInfo.Label = deps.First ().Split (new[] { ' '})[0];
			} else {
				nodeInfo.Label = "Module";
			}
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var module = (ModuleDescription)dataObject;

			if (module.Dependencies.Count > 0) {
				treeBuilder.AddChild (module.Dependencies);
			}

			if (module.Extensions.Count > 0) {
				treeBuilder.AddChild (module.Extensions);
			}

			if (module.DataFiles.Count > 0) {
				treeBuilder.AddChild (new AddinFilesFolder (module));
			}

			if (module.Assemblies.Count > 0) {
				treeBuilder.AddChild (new AddinAssembliesFolder (module));
			}
		}
	}
}
