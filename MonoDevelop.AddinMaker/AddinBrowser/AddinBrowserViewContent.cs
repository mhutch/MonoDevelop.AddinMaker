using System;
using System.Threading.Tasks;
using Gtk;
using Mono.Addins;
using MonoDevelop.Components;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Gui.Documents;
using MonoDevelop.Ide.Navigation;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinBrowserViewContent : DocumentController, INavigable
	{
		AddinBrowserWidget widget;
		Control control;
		AddinRegistry registry;

		public AddinBrowserViewContent (AddinRegistry registry)
		{
			DocumentTitle = "Addin Browser";
			AccessibilityDescription = DocumentTitle;

			this.registry = registry;
		}

		protected override bool ControllerIsViewOnly => true;

		protected override Control OnGetViewControl (DocumentViewContent view)
		{
			return control ?? (control = widget = new AddinBrowserWidget (registry));
		}

		//TODO: allow opening a specific addin and path
		public static async Task<Document> Open (AddinRegistry registry, object selection = null)
		{
			foreach (var doc in IdeApp.Workbench.Documents) {
				var content = doc.GetContent<AddinBrowserViewContent> ();
				if (content != null && content.widget.TreeView.Registry == registry) {
					content.Document.Select ();
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
			return await IdeApp.Workbench.OpenDocument (newContent, true);
		}

		public NavigationPoint BuildNavigationPoint ()
		{
			//TODO: save the widget's actual selection
			return new AddinNavigationPoint (widget.TreeView.Registry);
		}
	}
}
