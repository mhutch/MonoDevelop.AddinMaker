using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ExtensionPointFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ExtensionPointCollection); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Extension Points";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Extension Points";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var extensionPoints = (ExtensionPointCollection)dataObject;
			return extensionPoints.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var extensionPoints = (ExtensionPointCollection)dataObject;
			treeBuilder.AddChildren (extensionPoints);
		}
	}
}
