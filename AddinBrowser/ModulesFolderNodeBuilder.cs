using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ModulesFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ModuleCollection); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Optional Modules";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Optional Modules";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var modules = (ModuleCollection)dataObject;
			return modules.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var modules = (ModuleCollection)dataObject;
			treeBuilder.AddChildren (modules);
		}
	}

}
