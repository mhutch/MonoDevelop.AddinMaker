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

namespace MonoDevelop.AddinMaker.Editor
{
	class SchemaItem
	{
		readonly Dictionary<string,SchemaItem> children;

		public SchemaItem (string name, string description, SchemaItem[] children = null)
		{
			Name = name;
			Description = description;

			if (children != null) {
				this.children = new Dictionary<string, SchemaItem> ();
				foreach (var c in children) {
					this.children.Add (c.Name, c);
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
				list.Add (c.Value.Name, null, c.Value.Description);
			}
		}

		public virtual void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
		}

		public virtual void GetAttributeValueCompletions (CompletionDataList list, IAttributedXObject attributedOb, XAttribute att)
		{
		}

		public virtual SchemaItem GetChild (XElement el)
		{
			SchemaItem child;
			if (children != null && children.TryGetValue (el.Name.FullName, out child)) {
				return child;
			}
			return null;
		}
	}
}
