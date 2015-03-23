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
				new ExtensionSchemaElement (project),
				new SchemaElement ("ExtensionPoint", "Declares an extension point"),
				new SchemaElement ("ExtensionNodeSet", "Declares an extension node set"),
				new RuntimeSchemaItem (),
				new SchemaElement ("Module", "Declares an optional extension module"),
				new SchemaElement ("Localizer", "Declares a localizer for the add-in"),
				new SchemaElement ("ConditionType", "Declares a global condition type"),
				new SchemaElement ("Dependencies", "Declares dependencies"),
			};

			return new SchemaElement (null, null, new[] {
				new SchemaElement ("Addin", "Root element for add-in and add-in root descriptions", addinContents),
				new SchemaElement ("ExtensionModel", "Root element for add-in and add-in root descriptions", addinContents)
			});
		}
	}
}
