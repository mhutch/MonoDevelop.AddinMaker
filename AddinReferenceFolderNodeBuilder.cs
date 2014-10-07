using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.AddinMaker
{
	class AddinReferenceFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType
		{
			get { return typeof (AddinReferenceCollection); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "AddinReferenceFolder";
		}

		public override string ContextMenuAddinPath {
			get {
				return "/MonoDevelop/AddinMaker/ContextMenu/ProjectPad/AddinReferenceFolder";
			}
		}

		public override object GetParentObject (object dataObject)
		{
			return ((AddinReferenceCollection) dataObject).Project;
		}

		public override Type CommandHandlerType
		{
			get { return typeof (AddinReferenceFolderCommandHandler); }
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = GettextCatalog.GetString ("Addin References");
			//TODO: better icons
			nodeInfo.Icon = Context.GetIcon (Stock.OpenReferenceFolder);
			nodeInfo.ClosedIcon = Context.GetIcon (Stock.ClosedReferenceFolder);
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return ((AddinReferenceCollection)dataObject).Count > 0;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			foreach (var addin in (AddinReferenceCollection) dataObject)
				treeBuilder.AddChild (addin);
		}

		public override int CompareObjects (ITreeNavigator thisNode, ITreeNavigator otherNode)
		{
			return (otherNode.DataItem is MonoDevelop.Projects.ProjectReferenceCollection) ? 1 : -1;
		}

		class AddinReferenceFolderCommandHandler : NodeCommandHandler
		{
		}
	}
}