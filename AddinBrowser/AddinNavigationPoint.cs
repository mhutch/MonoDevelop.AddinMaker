using Mono.Addins;
using MonoDevelop.Ide.Navigation;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinNavigationPoint : NavigationPoint
	{
		readonly AddinRegistry registry;

		public AddinNavigationPoint (AddinRegistry registry)
		{
			this.registry = registry;
		}

		public override string DisplayName {
			get { return "Addin Browser"; }
		}

		public override MonoDevelop.Ide.Gui.Document ShowDocument ()
		{
			return AddinBrowserViewContent.Open (registry);
		}
	}
}
