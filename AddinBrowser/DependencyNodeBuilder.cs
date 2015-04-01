using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class DependencyNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Dependency); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var dependency = (Dependency)dataObject;
			return dependency.Name;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var dependency = (Dependency)dataObject;
			nodeInfo.Label = dependency.Name;
		}
	}
}
