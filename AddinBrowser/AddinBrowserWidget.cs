using Gtk;
using Mono.Addins;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserWidget : HPaned
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
					new ModulesFolderNodeBuilder (),
					new ModuleNodeBuilder (),
					new AssembliesFolderNodeBuilder (),
					new AddinAssemblyNodeBuilder (),
					new FilesFolderNodeBuilder (),
					new AddinFileNodeBuilder (),
				},
				new TreePadOption[0]
			);
			treeView.Tree.Selection.Mode = SelectionMode.Single;
			treeView.WidthRequest = 300;

			Pack1 (treeView, false, false);
			SetDetail (null);

			ShowAll ();
		}

		void SetDetail (Widget detail)
		{
			var child2 = Child2;
			if (child2 != null) {
				Remove (child2);
			}

			detail = detail ?? new Label ();
			detail.WidthRequest = 300;
			detail.Show ();

			Pack2 (detail, true, false);
		}

		public void SetToolbar (DocumentToolbar toolbar)
		{
		}

		public AddinRegistry Registry {
			get; private set;
		}
	}
}
