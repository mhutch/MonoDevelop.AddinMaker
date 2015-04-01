using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class DependencyFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(DependencyCollection); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Dependencies";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Dependencies";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var dependencies = (DependencyCollection)dataObject;
			return dependencies.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var dependencies = (DependencyCollection)dataObject;
			treeBuilder.AddChildren (dependencies);
		}
	}
}
