using MonoDevelop.AddinMaker.Editor.ManifestSchema;

namespace MonoDevelop.AddinMaker.Editor
{
	class AddinManifestEditorExtension : SchemaBasedEditorExtension
	{
		public override bool ExtendsEditor (MonoDevelop.Ide.Gui.Document doc, MonoDevelop.Ide.Gui.Content.IEditableTextBuffer editor)
		{
			return base.ExtendsEditor (doc, editor) && doc.HasProject && doc.Project is AddinProject;
		}

		protected override SchemaItem CreateSchema ()
		{
			var project = (AddinProject)Document.Project;

			var addinContents = new SchemaItem[] {
				new ExtensionSchemaItem (project),
				new SchemaItem ("ExtensionPoint", "Declares an extension point"),
				new SchemaItem ("ExtensionNodeSet", "Declares an extension node set"),
				new SchemaItem ("Runtime", "Declares what files belong to the add-in"),
				new SchemaItem ("Module", "Declares an optional extension module"),
				new SchemaItem ("Localizer", "Declares a localizer for the add-in"),
				new SchemaItem ("ConditionType", "Declares a global condition type"),
				new SchemaItem ("Dependencies", "Declares dependencies"),
			};

			return new SchemaItem (null, null, new[] {
				new SchemaItem ("Addin", "Root element for add-in and add-in root descriptions", addinContents),
				new SchemaItem ("ExtensionModel", "Root element for add-in and add-in root descriptions", addinContents)
			});
		}
	}
}
