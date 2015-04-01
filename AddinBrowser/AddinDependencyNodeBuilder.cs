using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;
using Mono.Addins;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinDependencyNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(AddinDependency); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var dependency = (AddinDependency)dataObject;
			return dependency.Name;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var dependency = (AddinDependency)dataObject;
			nodeInfo.Label = dependency.FullAddinId;
		}

		public override Type CommandHandlerType {
			get { return typeof(AddinDependencyNodeCommandHandler); }
		}

		class AddinDependencyNodeCommandHandler : NodeCommandHandler
		{
			public override void ActivateItem ()
			{
				var dependency = (AddinDependency) CurrentNode.DataItem;

				var tree = (AddinTreeView)Tree;
				var resolved = tree.Registry.GetAddin (dependency.FullAddinId);

				if (resolved != null) {
					//delay the selection, or this will re-select is
					GLib.Timeout.Add (200, () => {
						tree.SelectObject (resolved);
						return false;
					});
				}
			}
		}
	}
}
