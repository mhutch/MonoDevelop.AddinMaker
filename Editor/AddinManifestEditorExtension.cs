using MonoDevelop.AddinMaker.Editor.ManifestSchema;

namespace MonoDevelop.AddinMaker.Editor
{
	class AddinManifestEditorExtension : SchemaBasedEditorExtension
	{
		public override bool ExtendsEditor (MonoDevelop.Ide.Gui.Document doc, MonoDevelop.Ide.Gui.Content.IEditableTextBuffer editor)
		{
			return base.ExtendsEditor (doc, editor) && doc.HasProject && doc.Project is AddinProject;
		}

		protected override SchemaElement CreateSchema ()
		{
			var project = (AddinProject)Document.Project;

			var addinContents = new SchemaElement[] {
				new RuntimeSchemaElement (),
				new DependenciesSchemaElement (),
				new LocalizerSchemaElement (),
				new ExtensionSchemaElement (project),
				new ExtensionPointSchemaElement (project),
				new SchemaElement ("ExtensionNodeSet", "Declares an extension node set"),
				new SchemaElement ("ConditionType", "Declares a global condition type"),
			};

			return new SchemaElement (null, null, new[] {
				new SchemaElement ("Addin", "Root element for add-in and add-in root descriptions", addinContents),
				new SchemaElement ("ExtensionModel", "Root element for add-in and add-in root descriptions", addinContents)
			});
		}
	}
}
