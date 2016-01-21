using Mono.Addins;
using MonoDevelop.Ide.Navigation;
using System.Threading.Tasks;

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

		public override Task<MonoDevelop.Ide.Gui.Document> ShowDocument ()
		{
			return Task.FromResult (AddinBrowserViewContent.Open (registry));
		}
	}
}
