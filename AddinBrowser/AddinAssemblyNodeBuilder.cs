using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{

	class AddinAssemblyNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(AddinAssembly); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var assembly = (AddinAssembly)dataObject;
			return assembly.Assembly;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var assembly = (AddinAssembly)dataObject;
			nodeInfo.Label = assembly.Assembly;
		}
	}
}
