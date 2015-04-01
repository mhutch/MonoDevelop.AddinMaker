using System;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinFileNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(AddinFile); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var dependency = (AddinFile)dataObject;
			return dependency.File;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var dependency = (AddinFile)dataObject;
			nodeInfo.Label = dependency.File;
		}
	}
}
