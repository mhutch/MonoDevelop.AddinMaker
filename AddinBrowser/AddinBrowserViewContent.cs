using System;
using Gtk;
using Mono.Addins;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Navigation;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserViewContent : AbstractViewContent, INavigable
	{
		readonly AddinBrowserWidget widget;

		public AddinBrowserViewContent (AddinRegistry registry)
		{
			ContentName = "Addin Browser";
			widget = new AddinBrowserWidget (registry);
		}

		public override Widget Control {
			get { return widget; }
		}

		//TODO: allow opening a specific addin and path
		public static Document Open (AddinRegistry registry, object selection = null)
		{
			foreach (var doc in IdeApp.Workbench.Documents) {
				var content = doc.GetContent<AddinBrowserViewContent> ();
				if (content != null && content.widget.TreeView.Registry == registry) {
					content.WorkbenchWindow.SelectWindow ();
					if (selection != null) {
						content.widget.TreeView.SelectObject (selection);
					}
					return doc;
				}
			}

			var newContent = new AddinBrowserViewContent (registry);
			if (selection != null) {
				newContent.widget.TreeView.SelectObject (selection);
			}
			return IdeApp.Workbench.OpenDocument (newContent, true);
		}

		public override void Load (string fileName)
		{
			throw new NotSupportedException ();
		}

		/*
		protected override void OnWorkbenchWindowChanged (EventArgs e)
		{
			base.OnWorkbenchWindowChanged (e);

			if (WorkbenchWindow != null) {
				var toolbar = WorkbenchWindow.GetToolbar (this);
				widget.SetToolbar (toolbar);
			}
		}
		*/

		public NavigationPoint BuildNavigationPoint ()
		{
			//TODO: save the widget's actual selection
			return new AddinNavigationPoint (widget.TreeView.Registry);
		}
	}
}

