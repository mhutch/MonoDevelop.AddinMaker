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
using System.Threading;
using System.Threading.Tasks;

namespace MonoDevelop.AddinMaker.Editor
{
	//TODO: this schema system could be pushed down into MonoDevelop.Xml
	abstract class SchemaBasedEditorExtension : BaseXmlEditorExtension
	{
		SchemaElement schema;

		public override bool IsValidInContext (MonoDevelop.Ide.Editor.DocumentContext context)
		{
			return base.IsValidInContext (context) && context.HasProject && context.Project.HasFlavor<AddinProjectFlavor> ();
		}

		protected override void Initialize ()
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

		protected override Task<CompletionDataList> GetElementCompletions (CancellationToken token)
		{
			var list = new CompletionDataList();
			AddMiscBeginTags (list);

			XElement el;
			var item = GetSchemaItem (GetCurrentPath (), out el);
			if (item != null) {
				item.GetElementCompletions (list, el);
			}
			return Task.FromResult (list);
		}

		protected override Task<CompletionDataList> GetAttributeCompletions (IAttributedXObject attributedOb, Dictionary<string, string> existingAtts, CancellationToken token)
		{
			XElement el;
			var item = GetSchemaItem (GetCurrentPath (), out el);
			var list = new CompletionDataList ();

			if (item != null) {
				item.GetAttributeCompletions (list, attributedOb, existingAtts);
			}
			return Task.FromResult (list);
		}

		protected override Task<CompletionDataList> GetAttributeValueCompletions (IAttributedXObject attributedOb, XAttribute att, CancellationToken token)
		{
			XElement el;
			var path = GetCurrentPath ();
			var item = GetSchemaItem (path.Take (path.Count - 1), out el);
			var list = new CompletionDataList ();

			if (item != null) {
				item.GetAttributeValueCompletions (list, attributedOb, att);
			}
			return Task.FromResult (list);
		}
	}
}
