using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;

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

		public override Type CommandHandlerType {
			get { return typeof(AddinAssemblyNodeCommandHandler); }
		}

		class AddinAssemblyNodeCommandHandler : NodeCommandHandler
		{
			public override void ActivateItem ()
			{
				var assembly = (AddinAssembly) CurrentNode.DataItem;
				var path = System.IO.Path.Combine (assembly.Module.ParentAddinDescription.BasePath, assembly.Assembly);
				IdeApp.Workbench.OpenDocument (path, null, true);
			}
		}
	}
}
