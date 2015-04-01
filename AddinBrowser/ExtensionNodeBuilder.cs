using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ExtensionNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Extension); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var extension = (Extension)dataObject;
			return extension.Path;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var extension = (Extension)dataObject;
			nodeInfo.Label = extension.Path;
		}
	}
}
