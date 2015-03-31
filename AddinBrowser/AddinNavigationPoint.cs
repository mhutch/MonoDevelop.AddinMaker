using MonoDevelop.Ide.Navigation;
using Mono.Addins;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinNavigationPoint : NavigationPoint
	{
		AddinRegistry registry;

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
