//
// SchemaItem.cs
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
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;
using System.Linq;

namespace MonoDevelop.AddinMaker.Editor
{
	class SchemaElement
	{
		readonly Dictionary<string,SchemaElement> children;
		readonly Dictionary<string,SchemaAttribute> attributes;

		public SchemaElement (string name, string description, SchemaElement[] children = null, SchemaAttribute[] attributes = null)
		{
			Name = name;
			Description = description;

			if (children != null) {
				this.children = new Dictionary<string, SchemaElement> ();
				foreach (var c in children) {
					this.children.Add (c.Name, c);
				}
			}

			if (attributes != null) {
				this.attributes = new Dictionary<string, SchemaAttribute> ();
				foreach (var a in attributes) {
					this.attributes.Add (a.Name, a);
				}
			}
		}

		public string Name { get; private set; }
		public string Description { get; private set; }

		public virtual void GetElementCompletions (CompletionDataList list, XElement element)
		{
			if (children == null) {
				return;
			}

			foreach (var c in children) {
				list.Add (c.Key, null, c.Value.Description);
			}
		}

		public virtual void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			if (attributes == null) {
				return;
			}

			foreach (var a in attributes) {
				if (existingAtts.ContainsKey (a.Key)) {
					continue;
				}

				if (a.Value.Exclude != null && a.Value.Exclude.Any (existingAtts.ContainsKey)) {
					continue;
				}

				list.Add (a.Value.Name, null, a.Value.Description);
			}
		}

		public virtual void GetAttributeValueCompletions (CompletionDataList list, IAttributedXObject attributedOb, XAttribute att)
		{
			SchemaAttribute sca;
			if (attributes != null && attributes.TryGetValue (att.Name.FullName, out sca)) {
				sca.GetAttributeValueCompletions (list, attributedOb);
			}
		}

		public virtual SchemaElement GetChild (XElement element)
		{
			SchemaElement child;
			if (children != null && children.TryGetValue (element.Name.FullName, out child)) {
				return child;
			}
			return null;
		}
	}
}
