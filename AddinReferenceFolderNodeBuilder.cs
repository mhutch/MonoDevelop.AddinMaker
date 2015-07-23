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
			get { return typeof (AddinReferenceFolder); }
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
			return ((AddinReferenceFolder) dataObject).Project;
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
			return ((AddinReferenceFolder)dataObject).Project.Items.OfType<AddinReference> ().Any ();
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			foreach (var addin in ((AddinReferenceFolder)dataObject).Project.Items.OfType<AddinReference> ())
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
				var arf = (AddinReferenceFolder) CurrentNode.DataItem;

				var existingAddins = new HashSet<string> (
					arf.Project.Items.OfType<AddinReference> ().Select (a => a.Include)
				);

				var allAddins = arf.Project.GetFlavor<AddinProjectFlavor> ().AddinRegistry.GetAddins ()
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
				var references = selectedAddins.Select (a => new AddinReference (AddinHelpers.GetUnversionedId (a))).ToList ();

				arf.Project.Items.AddRange (references);
				IdeApp.ProjectOperations.SaveAsync (arf.Project);
			}

			public override void ActivateItem ()
			{
				AddAddinReference ();
			}
		}
	}
}