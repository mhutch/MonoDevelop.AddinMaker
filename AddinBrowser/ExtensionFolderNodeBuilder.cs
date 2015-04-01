using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ExtensionFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ExtensionCollection); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Extensions";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Extensions";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var extensions = (ExtensionCollection)dataObject;
			return extensions.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var extensions = (ExtensionCollection)dataObject;
			treeBuilder.AddChildren (extensions);
		}
	}
}