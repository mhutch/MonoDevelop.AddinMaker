// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.AddinMaker.Pads
{
	class EditorTagVisualizerPad : PadContent
	{
		Control control;

		public override Control Control => control;

		protected override void Initialize (IPadWindow window)
		{
			base.Initialize (window);
			control = new EditorTagVisualizer ();
		}

		public override void Dispose ()
		{
			base.Dispose ();
			control?.Dispose ();
			control = null;
		}
	}
}
