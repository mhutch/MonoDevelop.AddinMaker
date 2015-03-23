//
// SchemaBasedEditorExtension.cs
//
// Author:
//       Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (c) 2015 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;
using MonoDevelop.Xml.Editor;

namespace MonoDevelop.AddinMaker.Editor
{
	//TODO: this schema system could be pushed down into MonoDevelop.Xml
	abstract class SchemaBasedEditorExtension : BaseXmlEditorExtension
	{
		SchemaElement schema;

		public override bool ExtendsEditor (MonoDevelop.Ide.Gui.Document doc, MonoDevelop.Ide.Gui.Content.IEditableTextBuffer editor)
		{
			return base.ExtendsEditor (doc, editor) && doc.HasProject && doc.Project is AddinProject;
		}

		public override void Initialize ()
		{
			base.Initialize ();
			schema = CreateSchema ();
		}

		protected abstract SchemaElement CreateSchema ();

		SchemaElement GetSchemaItem (IEnumerable<XObject> path, out XElement el)
		{
			el = null;
			SchemaElement item = schema;
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
