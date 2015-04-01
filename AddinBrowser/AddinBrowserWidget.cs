using Gtk;
using Mono.Addins;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;

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
			//TODO: make extensible?
			treeView = new ExtensibleTreeView (
				new NodeBuilder[] {
					new AddinNodeBuilder (),
					new ExtensionFolderNodeBuilder (),
					new ExtensionNodeBuilder (),
					new ExtensionPointNodeBuilder (),
					new ExtensionPointFolderNodeBuilder (),
					new DependencyFolderNodeBuilder (),
					new DependencyNodeBuilder (),
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
}
