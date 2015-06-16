using MonoDevelop.AddinMaker.Editor.ManifestSchema;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;
using System.Linq;

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
				new ExtensionElement (project),
				new ExtensionPointSchemaElement (project),
				new SchemaElement ("ExtensionNodeSet", "Declares an extension node set"),
				new SchemaElement ("ConditionType", "Declares a global condition type"),
			};

			return new SchemaElement (null, null, new[] {
				new SchemaElement (
					"Addin",
					"Root element for add-in and add-in root descriptions",
					addinContents,
					new[] {
						new SchemaAttribute ("id", "The identifier of the add-in. It is mandatory for add-in roots and for add-ins that can be extended, optional for other add-ins."),
						new SchemaAttribute ("namespace", "Namespace of the add-in. The full ID of an add-in is composed by 'namespace.name'."),
						new SchemaAttribute ("version", "The version of the add-in. It is mandatory for add-in roots and for add-ins that can be extended."),
						new SchemaAttribute ("compatVersion", "Version of the add-in with which this add-in is backwards compatible (optional)."),
						new SchemaAttribute ("name", "Display name of the add-in."),
						new SchemaAttribute ("description", "Description of the add-in."),
						new SchemaAttribute ("author", "Author of the add-in."),
						new SchemaAttribute ("url", "Url of a web page with more information about the add-in."),
						new SchemaAttribute ("defaultEnabled", "When set to 'false', the add-in won't be enabled until it is explicitly enabled by the user. The default is 'true'."),
						//TODO: enable this if we ever support arbitrary addins
						//new SchemaAttribute ("isroot", "Must be true if this manifest belongs to an add-in root.")
					}
				),
				new SchemaElement ("ExtensionModel", "Root element for add-in and add-in root descriptions", addinContents)
			});
		}

		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			var pathCompletion = HandlePathCompletion ();
			if (pathCompletion != null) {
				return pathCompletion;
			}

			return base.HandleCodeCompletion (completionContext, completionChar, ref triggerWordLength);
		}

		ICompletionDataList HandlePathCompletion ()
		{
			var valueState = Tracker.Engine.CurrentState as MonoDevelop.Xml.Parser.XmlAttributeValueState;

			if (valueState != null) {
				var att = Tracker.Engine.Nodes.OfType<XAttribute> ().FirstOrDefault ();

				if (att != null && att.IsNamed && att.Name.FullName == "path") {
					// completionChar may be a space even if the current char isn't, when ctrl-space is fired t
					int currentPosition = Editor.Caret.Offset;
					if (currentPosition > 0) {
						string s = Editor.GetTextBetween (currentPosition - Tracker.Engine.CurrentStateLength, currentPosition);
						if (s.EndsWith ("/", System.StringComparison.Ordinal)) {
							var item = GetSchemaItem ();
							var ext = item as ExtensionElement;
							if (ext != null) {
								return ext.GetPathCompletions (s);
							}
						}
					}
				}
			}

			return null;
		}
	}
}
