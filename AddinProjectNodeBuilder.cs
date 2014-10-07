using System;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;
using System.Collections.Generic;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNodeBuilder: NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(AddinProject).IsAssignableFrom (dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof(AddinProjectCommandHandler); }
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var project = (AddinProject)dataObject;
			treeBuilder.AddChild (project.AddinReferences);
		}

		public override void OnNodeAdded (object dataObject)
		{
			var project = (AddinProject)dataObject;
			project.AddinReferenceAdded += OnItemsChanged;
			project.AddinReferenceRemoved += OnItemsChanged;
			base.OnNodeAdded (dataObject);
		}

		public override void OnNodeRemoved (object dataObject)
		{
			var project = (AddinProject)dataObject;
			project.AddinReferenceAdded -= OnItemsChanged;
			project.AddinReferenceRemoved -= OnItemsChanged;
			base.OnNodeRemoved (dataObject);
		}

		void OnItemsChanged (object sender, ProjectItemEventArgs e)
		{
			var projects = new HashSet<DotNetProject> ();
			foreach (ProjectItemEventInfo evt in e)
				if (evt.Item is AddinReference)
					projects.Add ((DotNetProject)evt.SolutionItem);
			foreach (var project in projects) {
				ITreeBuilder builder = Context.GetTreeBuilder (project);
				if (builder != null)
					builder.UpdateChildren ();
			}
		}

		class AddinProjectCommandHandler : NodeCommandHandler
		{
		}
	}
}
