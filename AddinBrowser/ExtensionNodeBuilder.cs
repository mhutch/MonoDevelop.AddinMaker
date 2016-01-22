using System;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;
using Gtk;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class ExtensionNodeBuilder : TypeNodeBuilder, ITreeDetailBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Extension); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			var extension = (Extension)dataObject;
			return extension.Path;
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var extension = (Extension)dataObject;
			nodeInfo.Label = extension.Path;
		}

		public Widget GetDetailWidget (object dataObject)
		{
			return new ExtensionDetailWidget ((Extension)dataObject);
		}
	}

	class ExtensionDetailWidget : VBox
	{
		public Extension Extension { get; private set; }

		public ExtensionDetailWidget (Extension ext)
		{
			this.Extension = ext;

			BorderWidth = 12;

			PackStart (new Label { Markup = string.Format ("<big><tt>{0}</tt></big>", ext.Path)}, true, false, 0);

			ShowAll ();
		}
	}
}
