using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;

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
			project.AddinReferenceAdded += OnReferencesChanged;
			project.AddinReferenceRemoved += OnReferencesChanged;
			base.OnNodeAdded (dataObject);
		}

		public override void OnNodeRemoved (object dataObject)
		{
			var project = (AddinProject)dataObject;
			project.AddinReferenceAdded -= OnReferencesChanged;
			project.AddinReferenceRemoved -= OnReferencesChanged;
			base.OnNodeRemoved (dataObject);
		}

		void OnReferencesChanged (object sender, AddinReferenceEventArgs e)
		{
			foreach (var project in e.Select (x => x.Project).Distinct ()) {
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
