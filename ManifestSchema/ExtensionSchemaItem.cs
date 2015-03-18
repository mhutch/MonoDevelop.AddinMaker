//
// ExtensionSchemaItem.cs
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
using Mono.Addins.Description;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;

namespace MonoDevelop.AddinMaker.ManifestSchema
{
	class ExtensionSchemaItem : SchemaItem
	{
		readonly AddinProject project;

		public ExtensionSchemaItem (AddinProject project) : base ("Extension", "Declares an extension")
		{
			this.project = project;
		}

		public override void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			if (!existingAtts.ContainsKey ("path")) {
				list.Add ("path", null, "The path of the extension");
			}
		}

		public override void GetAttributeValueCompletions (CompletionDataList list, IAttributedXObject attributedOb, XAttribute att)
		{
			if (att.Name.FullName != "path") {
				return;
			}

			foreach (var addin in project.GetReferencedAddins ()) {
				foreach (ExtensionPoint ep in addin.Description.ExtensionPoints) {
					list.Add (ep.Path, null, ep.Description);
				}
			}
		}
	}
}
