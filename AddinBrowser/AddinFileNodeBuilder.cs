using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;

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

		public override Type CommandHandlerType {
			get { return typeof(AddinFileNodeCommandHandler); }
		}

		class AddinFileNodeCommandHandler : NodeCommandHandler
		{
			public override void ActivateItem ()
			{
				var file = (AddinFile) CurrentNode.DataItem;
				var path = System.IO.Path.Combine (file.Module.ParentAddinDescription.BasePath, file.File);
				IdeApp.Workbench.OpenDocument (path, null, true);
			}
		}
	}
}
