using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AssembliesFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(AddinAssembliesFolder); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Assemblies";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Assemblies";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var node = (AddinAssembliesFolder)dataObject;
			return node.Module.Assemblies.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var node = (AddinAssembliesFolder)dataObject;
			treeBuilder.AddChildren (node.Module.Assemblies.OfType<string> ().Select (f => new AddinAssembly (node.Module, f)));
		}
	}
}
