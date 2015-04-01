using System;
using Mono.Addins;
using Mono.Addins.Description;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinNodeBuilder : TypeNodeBuilder
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

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			//there will always be dependencies unless it's root, in which case it should have extension points
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var addin = (Addin)dataObject;

			//TODO: support optional modules
			var module = addin.Description.MainModule;
			treeBuilder.AddChild (module.Dependencies);
			treeBuilder.AddChild (module.Extensions);
			treeBuilder.AddChild (addin.Description.ExtensionPoints);
		}
	}
}
