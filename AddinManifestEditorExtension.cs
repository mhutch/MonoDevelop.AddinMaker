using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Xml.Editor;
using MonoDevelop.AddinMaker.ManifestSchema;

namespace MonoDevelop.AddinMaker
{
	//TODO: this schema system could be psuhed down into MonoDevelop.Xml
	class AddinManifestEditorExtension : BaseXmlEditorExtension
	{
		ManifestSchemaRoot schema;

		public override bool ExtendsEditor (MonoDevelop.Ide.Gui.Document doc, MonoDevelop.Ide.Gui.Content.IEditableTextBuffer editor)
		{
			return base.ExtendsEditor (doc, editor) && doc.HasProject && doc.Project is AddinProject;
		}

		public override void Initialize ()
		{
			base.Initialize ();
			schema = new ManifestSchemaRoot (Project);
		}

		AddinProject Project {
			get { return (AddinProject) Document.Project; }
		}

		SchemaItem GetSchemaItem (IEnumerable<XObject> path, out XElement el)
		{
			el = null;
			SchemaItem item = schema;
			foreach (var val in path) {
				el = val as XElement;
				if (el == null) {
					return null;
				}
				item = item.GetChild (el);
				if (item == null) {
					return null;
				}
			}
			return item;
		}

		protected override void GetElementCompletions (CompletionDataList list)
		{
			AddMiscBeginTags (list);

			XElement el;
			var item = GetSchemaItem (GetCurrentPath (), out el);
			if (item != null) {
				item.GetElementCompletions (list, el);
			}
		}

		protected override CompletionDataList GetAttributeCompletions (IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			XElement el;
			var item = GetSchemaItem (GetCurrentPath (), out el);
			if (item != null) {
				var list = new CompletionDataList ();
				item.GetAttributeCompletions (list, attributedOb, existingAtts);
				return list;
			}
			return null;
		}

		protected override CompletionDataList GetAttributeValueCompletions (IAttributedXObject attributedOb, XAttribute att)
		{
			XElement el;
			var path = GetCurrentPath ();
			var item = GetSchemaItem (path.Take (path.Count - 1), out el);
			if (item != null) {
				var list = new CompletionDataList ();
				item.GetAttributeValueCompletions (list, attributedOb, att);
				return list;
			}
			return null;
		}
	}
}
