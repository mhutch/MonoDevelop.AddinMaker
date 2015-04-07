using System;
using Mono.Addins;
using MonoDevelop.Ide.Gui.Components;
using Gtk;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinNodeBuilder : ModuleNodeBuilder, ITreeDetailBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Addin); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var addin = (Addin)dataObject;
			return addin.Id;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var addin = (Addin)dataObject;
			nodeInfo.Label = addin.Namespace + (string.IsNullOrEmpty (addin.Namespace)? "" : ".") +  addin.LocalId;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var addin = (Addin)dataObject;

			if (addin.Description.OptionalModules.Count > 0) {
				treeBuilder.AddChild (addin.Description.OptionalModules);
			}

			if (addin.Description.ExtensionPoints.Count > 0) {
				treeBuilder.AddChild (addin.Description.ExtensionPoints);
			}

			base.BuildChildNodes (treeBuilder, addin.Description.MainModule);
		}

		public Widget GetDetailWidget (object dataObject)
		{
			return new AddinDetailWidget ((Addin)dataObject);
		}
	}

	class AddinDetailWidget : VBox
	{
		public Addin Addin { get; private set; }

		public AddinDetailWidget (Addin addin)
		{
			this.Addin = addin;

			BorderWidth = 12;

			var desc = Addin.Description;
			PackStart (new Label { Markup = string.Format ("<big><tt>{0}</tt></big>\n{1}\n{2}", desc.AddinId, desc.Name, desc.Description)}, true, false, 0);

			ShowAll ();
		}
	}
}
