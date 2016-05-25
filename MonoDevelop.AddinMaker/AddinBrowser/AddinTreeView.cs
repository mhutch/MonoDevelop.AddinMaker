using Mono.Addins;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinTreeView : ExtensibleTreeView
	{
		public AddinRegistry Registry { get; private set; }

		public AddinTreeView (AddinRegistry registry) : base (new NodeBuilder[] {
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
		},
			new TreePadOption[0])
		{
			this.Registry = registry;
		}

		public bool SelectObject (object node)
		{
			var n = GetNodeAtObject (node, true);
			if (n != null) {
				n.Selected = true;
				n.ScrollToNode ();
				return true;
			}
			return false;
		}

		public void Update ()
		{
			Clear ();

			foreach (var addin in Registry.GetModules (AddinSearchFlags.IncludeAll)) {
				AddChild (addin);
			}
		}
	}
}
