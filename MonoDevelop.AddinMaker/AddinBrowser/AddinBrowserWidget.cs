using Gtk;
using Mono.Addins;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserWidget : HPaned
	{
		public AddinTreeView TreeView { get; private set; }

		object detailItem;

		public AddinBrowserWidget (AddinRegistry registry)
		{
			var nodebuilder = new NodeBuilder [] {
				new AddinNodeBuilder (),
				new ExtensionFolderNodeBuilder (),
				new ExtensionNodeBuilder (),
				new ExtensionPointNodeBuilder (),
				new ExtensionPointFolderNodeBuilder (),
				new DependencyFolderNodeBuilder (),
				new AddinDependencyNodeBuilder (),
				new ModulesFolderNodeBuilder (),
				new ModuleNodeBuilder (),
				new AssembliesFolderNodeBuilder (),
				new AddinAssemblyNodeBuilder (),
				new FilesFolderNodeBuilder (),
				new AddinFileNodeBuilder (),
			};

            var controller = new AddinTreeViewController (registry, nodebuilder, new TreePadOption [0]);
			TreeView = new AddinTreeView (controller);
			var gtkTreeView = TreeView.GetNativeWidget<Gtk.Widget> ();

			TreeView.AllowsMultipleSelection = false;
			gtkTreeView.WidthRequest = 300;

			Pack1 (TreeView, false, false);
			SetDetailWidget (null);

			ShowAll ();

			TreeView.Update ();

			TreeView.SelectionChanged += (sender, e) => FillDetailPanel ();
		}

		void SetDetailWidget (Widget detail)
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

		void FillDetailPanel ()
		{
			ITreeNavigator nav = TreeView.Controller.GetSelectedNode ();
			ITreeDetailBuilder tdb;

			if (nav == null) {
				SetDetailWidget (null);
				return;
			}

			do {
				tdb = nav.TypeNodeBuilder as ITreeDetailBuilder;
			} while (nav != null && tdb == null && nav.MoveToParent ());

			if (nav == null || tdb == null) {
				SetDetailWidget (null);
				return;
			}

			if (detailItem == nav.DataItem) {
				return;
			}

			detailItem = nav.DataItem;
			SetDetailWidget (tdb.GetDetailWidget (detailItem));
		}
	}

	interface ITreeDetailBuilder
	{
		Control GetDetailWidget (object dataObject);
	}
}
