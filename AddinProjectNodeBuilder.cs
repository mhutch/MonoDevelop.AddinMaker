using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNodeBuilder: NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(DotNetProject).IsAssignableFrom (dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof(AddinProjectCommandHandler); }
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var project = ((DotNetProject)dataObject);
			return project.HasFlavor<AddinProjectFlavor> ();
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var project = ((DotNetProject)dataObject);
			if (project.HasFlavor<AddinProjectFlavor> ()) {
				treeBuilder.AddChild (new AddinReferenceFolder (project));
			}
		}

		public override void OnNodeAdded (object dataObject)
		{
			var project = ((DotNetProject)dataObject);
			project.ProjectItemAdded += OnReferencesChanged;
			project.ProjectItemRemoved += OnReferencesChanged;
			base.OnNodeAdded (dataObject);
		}

		public override void OnNodeRemoved (object dataObject)
		{
			var project = ((DotNetProject)dataObject);
			project.ProjectItemAdded -= OnReferencesChanged;
			project.ProjectItemRemoved -= OnReferencesChanged;
			base.OnNodeRemoved (dataObject);
		}

		void OnReferencesChanged (object sender, ProjectItemEventArgs e)
		{
			foreach (var project in e.Select (x => (Project)x.SolutionItem).Distinct ()) {
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
