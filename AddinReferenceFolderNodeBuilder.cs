using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
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
			[CommandHandler(AddinCommands.AddAddinReference)]
			public void AddAddinReference ()
			{
				var arc = (AddinReferenceCollection) CurrentNode.DataItem;

				var existingAddins = new HashSet<string> (
					arc.Project.AddinReferences.Select (a => a.Id)
				);

				var allAddins = arc.Project.AddinRegistry.GetAddins ()
					.Where (a => !existingAddins.Contains (AddinHelpers.GetUnversionedId (a)))
					.ToArray ();

				if (allAddins.Length  == 0) {
					MessageService.ShowMessage (
						GettextCatalog.GetString ("You have already referenced all available addins")
					);
					return;
				}

				var dialog = new AddAddinReferenceDialog (allAddins);
				Addin[] selectedAddins;
				try {
					if (MessageService.RunCustomDialog (dialog) != (int)Gtk.ResponseType.Ok)
						return;
					selectedAddins = dialog.GetSelectedAddins ();
				} finally {
					dialog.Destroy ();
				}

				//HACK: we have to ToList() or the event handlers attached to the
				//collection will all enumerate the list and get different copies
				var references = selectedAddins.Select (a => new AddinReference {
					Id = AddinHelpers.GetUnversionedId (a)
				}).ToList ();

				arc.Project.AddinReferences.AddRange (references);
				IdeApp.ProjectOperations.Save (arc.Project);
			}

			public override void ActivateItem ()
			{
				AddAddinReference ();
			}
		}
	}
}