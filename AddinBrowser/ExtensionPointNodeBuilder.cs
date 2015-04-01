using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ExtensionPointNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(ExtensionPoint); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var extensionPoint = (ExtensionPoint)dataObject;
			return extensionPoint.Path;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var extensionPoint = (ExtensionPoint)dataObject;
			nodeInfo.Label = extensionPoint.Path;
		}
	}
}