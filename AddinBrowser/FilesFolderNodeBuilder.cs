using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class FilesFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(AddinFilesFolder); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "Files";
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Files";
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var node = (AddinFilesFolder)dataObject;
			return node.Module.DataFiles.Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var node = (AddinFilesFolder)dataObject;
			treeBuilder.AddChildren (node.Module.DataFiles.OfType<string> ().Select (f => new AddinFile (node.Module, f)));
		}
	}
}
