using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Xml.Editor;

namespace MonoDevelop.AddinMaker
{
	class AddinManifestEditorExtension : BaseXmlEditorExtension
	{
		protected override void GetElementCompletions (CompletionDataList list)
		{
			AddMiscBeginTags (list);

			var path = GetCurrentPath ();

			if (path.Count == 0) {
				list.Add ("Addin", null, "Root element for add-in and add-in root descriptions");
				list.Add ("ExtensionModel", null, "Root element for add-in and add-in root descriptions");
				return;
			}

			if (path.Count > 0) {
				var el = path [0] as XElement;
				if (el == null) {
					return;
				}

				var name = el.Name.FullName;
				if (name != "Addin" && name != "ExtensionModel") {
					return;
				}

				list.Add ("Extension", null, "Declares an extension");
				list.Add ("ExtensionPoint", null, "Declares an extension point");
				list.Add ("ExtensionNodeSet", null, "Declares an extension node set");
				list.Add ("Runtime", null, "Declares what files belong to the add-in");
				list.Add ("Module", null, "Declares an optional extension module");
				list.Add ("Localizer", null, "Declares a localizer for the add-in");
				list.Add ("ConditionType", null, "Declares a global condition type");
				list.Add ("Dependencies", null, "Declares dependencies");
				return;
			}
		}
	}
}
