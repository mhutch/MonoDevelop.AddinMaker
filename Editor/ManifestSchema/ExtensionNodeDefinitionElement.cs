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

namespace MonoDevelop.AddinMaker.Editor.ManifestSchema
{
	//TODO: completion for the type attribute
	class ExtensionNodeDefinitionElement : SchemaElement
	{
		readonly AddinProject project;

		public ExtensionNodeDefinitionElement (AddinProject project) : base (
			"ExtensionNode",
			"A type of node allowed in this extension point.",
			new[] {
				new SchemaElement ("Description", "Description of what this kind of node represents.")
			},
			new [] {
				new SchemaAttribute ("name", "Name of the node type. When an element is added to an extension point, its name must match one of the declared node types."),
				new SchemaAttribute ("type", "CLR type that implements this extension node type. It must be a subclass of Mono.Addins.ExtensionNode. If not specified, by default it is Mono.Addins.TypeExtensionNode.")
			}
		)
		{
			this.project = project;
		}
	}
}
