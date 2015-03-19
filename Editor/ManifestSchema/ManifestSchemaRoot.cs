//
// ManifestSchema.cs
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
using System;

namespace MonoDevelop.AddinMaker.Editor.ManifestSchema
{
	class ManifestSchemaRoot : SchemaItem
	{
		public ManifestSchemaRoot (AddinProject project) : base (null, null, CreateChildren (project))
		{
		}

		static SchemaItem[] CreateChildren (AddinProject project)
		{
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

			return new[] {
				new SchemaItem ("Addin", "Root element for add-in and add-in root descriptions", addinContents),
				new SchemaItem ("ExtensionModel", "Root element for add-in and add-in root descriptions", addinContents)
			};
		}
	}
}

