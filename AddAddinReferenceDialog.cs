using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Mono.Addins;
using MonoDevelop.Components;
using MonoDevelop.Core;

namespace MonoDevelop.AddinMaker
{
	//TODO: add a filter entry
	class AddAddinReferenceDialog : Dialog
	{
		readonly Button addButton = new Button (Stock.Add);
		readonly TreeView treeView = new TreeView ();
		readonly ListStore store = new ListStore (typeof(Addin), typeof(bool));

		const int COL_ADDIN = 0;
		const int COL_SELECTED = 1;

		int selectedCount;

		public AddAddinReferenceDialog (Addin[] allAddins)
		{
			if (allAddins == null || allAddins.Length == 0)
				throw new ArgumentException ();

			Title = GettextCatalog.GetString ("Add Addin Reference");
			DestroyWithParent = true;
			Modal = true;
			HasSeparator = false;
			WidthRequest = 400;
			HeightRequest = 400;
			AllowShrink = false;
			Resizable = true;

			AddActionWidget (new Button (Stock.Cancel), ResponseType.Cancel);
			AddActionWidget (addButton, ResponseType.Ok);

			treeView.HeadersVisible = false;
			treeView.Model = store;

			var column = new TreeViewColumn ();
			var toggleRenderer = new CellRendererToggle ();
			column.PackStart (toggleRenderer, false);
			column.SetCellDataFunc (toggleRenderer, ToggleCellDataFunc);

			toggleRenderer.Toggled += HandleToggled;

			var textRenderer = new CellRendererText ();
			column.PackStart (textRenderer, true);
			column.SetCellDataFunc (textRenderer, TextCellDataFunc);

			treeView.AppendColumn (column);
			var sw = new CompactScrolledWindow {
				Child = treeView
			};
			VBox.PackStart (sw);

			foreach (var addin in allAddins.OrderBy (a => a.Name)) {
				store.AppendValues (addin, false);
			}

			addButton.Sensitive = false;

			ShowAll ();
		}

		void HandleToggled (object o, ToggledArgs args)
		{
			TreeIter iter;
			Check (store.GetIter (out iter, new TreePath (args.Path)));
			var val = !(bool)store.GetValue (iter, COL_SELECTED);
			store.SetValue (iter, COL_SELECTED, val);

			selectedCount += val ? 1 : -1;
			addButton.Sensitive = selectedCount > 0;
		}

		static void ToggleCellDataFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var selected = (bool)model.GetValue (iter, COL_SELECTED);
			var cellRendererToggle = (CellRendererToggle)cell;
			cellRendererToggle.Active = selected;
		}

		static void TextCellDataFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var addin = (Addin)model.GetValue (iter, COL_ADDIN);
			var markup = string.Format (
				"<b>{0}</b>\n{1}",
				GLib.Markup.EscapeText (AddinHelpers.GetUnversionedId (addin)),
				GLib.Markup.EscapeText (addin.Name)
			);
			var cellRendererText = (CellRendererText)cell;
			cellRendererText.Markup = markup;
		}

		public Addin[] GetSelectedAddins ()
		{
			var selected = new List<Addin>();

			TreeIter iter;
			Check (store.GetIterFirst (out iter));
			do {
				if ((bool)store.GetValue (iter, COL_SELECTED)) {
					selected.Add ((Addin)store.GetValue (iter, COL_ADDIN));
				}
			} while (store.IterNext (ref iter));

			return selected.ToArray ();
		}

		static void Check (bool val)
		{
			if (!val)
				throw new Exception ("TreeStore state is corrupt");
		}
	}
}