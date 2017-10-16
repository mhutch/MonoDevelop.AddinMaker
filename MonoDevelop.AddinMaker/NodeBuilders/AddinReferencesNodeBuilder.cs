using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinReferencesNodeBuilder : TypeNodeBuilder
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

		public override Type CommandHandlerType
		{
			get { return typeof (AddinReferenceFolderCommandHandler); }
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var project = ((AddinReferenceCollection)dataObject).Parent.Project;
			var isSdkStyleProject = project.MSBuildProject.Sdk != null;
			if (isSdkStyleProject) {
				nodeInfo.Label = GettextCatalog.GetString ("Extensions");
			} else {
				nodeInfo.Label = GettextCatalog.GetString ("Extension References");
			}
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
			foreach (var addin in ((AddinReferenceCollection)dataObject))
				treeBuilder.AddChild (addin);
		}

		public override void OnNodeAdded (object dataObject)
		{
			var project = ((AddinReferenceCollection)dataObject).Parent.Project;
			project.ProjectItemAdded += OnReferencesChanged;
			project.ProjectItemRemoved += OnReferencesChanged;
			base.OnNodeAdded (dataObject);
		}

		public override void OnNodeRemoved (object dataObject)
		{
			var project = ((AddinReferenceCollection)dataObject).Parent.Project;
			project.ProjectItemAdded -= OnReferencesChanged;
			project.ProjectItemRemoved -= OnReferencesChanged;
			base.OnNodeRemoved (dataObject);
		}

		void OnReferencesChanged (object sender, ProjectItemEventArgs e)
		{
			foreach (var project in e.Select (x => x.SolutionItem).Distinct ()) {
				var dnp = project as DotNetProject;
				if (dnp == null)
					continue;

				var addinFlavor = dnp.AsFlavor<AddinProjectFlavor> ();
				if (addinFlavor == null)
					continue;

				ITreeBuilder builder = Context.GetTreeBuilder (addinFlavor.AddinReferences);
				if (builder != null)
					builder.UpdateChildren ();
			}
		}

		//ensure this is immediately after the references folder
		public override int CompareObjects (ITreeNavigator thisNode, ITreeNavigator otherNode)
		{
			return (otherNode.DataItem is ProjectReferenceCollection) ? 1 : -1;
		}

		public override int GetSortIndex (ITreeNavigator node)
		{
			return -200;
		}

		class AddinReferenceFolderCommandHandler : NodeCommandHandler
		{
			[CommandHandler(AddinCommands.AddAddinReference)]
			public void AddAddinReference ()
			{
				var addins = (AddinReferenceCollection) CurrentNode.DataItem;

				var existingAddins = new HashSet<string> (
					addins.Select (a => a.Include)
				);

				var allAddins = addins.Parent.AddinRegistry.GetAddins ()
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

				addins.AddRange (references);
				IdeApp.ProjectOperations.SaveAsync (addins.Parent.Project);
			}

			public override void ActivateItem ()
			{
				AddAddinReference ();
			}
		}
	}
}