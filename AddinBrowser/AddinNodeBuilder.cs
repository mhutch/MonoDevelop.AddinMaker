using System;
using Mono.Addins;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinNodeBuilder : ModuleNodeBuilder
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
	}
}
