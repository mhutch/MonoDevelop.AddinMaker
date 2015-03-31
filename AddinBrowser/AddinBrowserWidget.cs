using Gtk;
using Mono.Addins;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using System;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserWidget : VBox
	{
		ExtensibleTreeView treeView;

		public AddinBrowserWidget (AddinRegistry registry)
		{
			Registry = registry;

			Build ();

			foreach (var addin in registry.GetAddins ()) {
				treeView.AddChild (addin);
			}
		}

		void Build ()
		{
			//TODO: make extensible
			treeView = new ExtensibleTreeView (
				new NodeBuilder[] {
					new AddinNodeBuilder ()
				},
				new TreePadOption[0]
			);
			treeView.Tree.Selection.Mode = SelectionMode.Single;

			PackStart (treeView, true, true, 0);
			ShowAll ();
		}

		public void SetToolbar (DocumentToolbar toolbar)
		{
		}

		public AddinRegistry Registry {
			get; private set;
		}
	}

	class AddinNodeBuilder : TypeNodeBuilder
	{
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var item = (Addin)dataObject;
			return item.Id;
		}

		public override Type NodeDataType {
			get { return typeof(Addin); }
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var item = (Addin)dataObject;
			nodeInfo.Label = item.Namespace + (string.IsNullOrEmpty (item.Namespace)? "" : ".") +  item.LocalId;
		}
	}
}
