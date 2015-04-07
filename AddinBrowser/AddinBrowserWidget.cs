using Gtk;
using Mono.Addins;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserWidget : HPaned
	{
		public AddinTreeView TreeView { get; private set; }

		object detailItem;

		public AddinBrowserWidget (AddinRegistry registry)
		{
			TreeView = new AddinTreeView (registry);

			TreeView.Tree.Selection.Mode = SelectionMode.Single;
			TreeView.WidthRequest = 300;

			Pack1 (TreeView, false, false);
			SetDetailWidget (null);

			ShowAll ();

			TreeView.Update ();

			TreeView.Tree.Selection.Changed += (sender, e) => FillDetailPanel ();
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
			ITreeNavigator nav = TreeView.GetSelectedNode ();
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

		public void SetToolbar (DocumentToolbar toolbar)
		{
		}
	}

	interface ITreeDetailBuilder
	{
		Widget GetDetailWidget (object dataObject);
	}
}
