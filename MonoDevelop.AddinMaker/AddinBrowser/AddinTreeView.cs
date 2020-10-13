using Mono.Addins;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
    class AddinTreeViewController : ExtensibleTreeViewController
    {
		public AddinRegistry Registry { get; private set; }

		public AddinTreeViewController (AddinRegistry registry, NodeBuilder [] nodeBuilder, TreePadOption [] option) :base (nodeBuilder, option)
        {
			this.Registry = registry;
		}
	}

	class AddinTreeView : ExtensibleTreeView
	{
		internal AddinTreeViewController Controller;

		public AddinTreeView (AddinTreeViewController controller) : base (
			controller)
        {
			Controller = controller;
		}
		
		public bool SelectObject (object node)
		{
            var n = Controller.GetNodeAtObject (node, true);
            if (n != null) {
                n.Selected = true;
                n.ScrollToNode ();
                return true;
            }
            return false;
		}

		public void Update ()
		{
            Controller.Clear ();

            foreach (var addin in Controller.Registry.GetModules (AddinSearchFlags.IncludeAll)) {
				Controller.AddChild (addin);
            }
        }
	}
}
