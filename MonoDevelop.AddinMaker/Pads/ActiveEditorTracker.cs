// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects.Text;

namespace MonoDevelop.AddinMaker.Pads
{
	class ActiveEditorTracker : IDisposable
	{
		Document activeDocument;
		ITextView textView;
		bool disposed;

		public ITextView TextView => textView;

		public ActiveEditorTracker ()
		{
			IdeApp.Workbench.ActiveDocumentChanged += ActiveDocumentChanged;
			ActiveDocumentChanged (null, null);
		}

		void ActiveDocumentChanged (object sender, EventArgs e)
		{
			if (activeDocument != null) {
				activeDocument.ViewChanged -= ActiveViewChanged;
			}
			activeDocument = IdeApp.Workbench.ActiveDocument;

			if (activeDocument != null) {
				activeDocument.ViewChanged += ActiveViewChanged;
			}

			ActiveViewChanged (null, null);
		}

		void ActiveViewChanged (object sender, EventArgs e)
		{
			//FIXME there doesn't seem to be a better way to determine whether the view is an editor
			//or to pull out the focused view when it's split e.g. diff view
			var oldView = textView;
			if (activeDocument?.ActiveView?.GetContent<ITextFile> () != null) {
				textView = activeDocument.Editor.TextView;
			} else {
				textView = null;
			}

			if (textView != oldView) {
				ActiveEditorChanged?.Invoke (this, new ActiveEditorChangedEventArgs (textView, oldView));
			}
		}

		public event EventHandler<ActiveEditorChangedEventArgs> ActiveEditorChanged;

		public void Dispose ()
		{
			if (disposed) {
				return;
			}
			disposed = true;

			IdeApp.Workbench.ActiveDocumentChanged -= ActiveDocumentChanged;
			if (activeDocument != null) {
				activeDocument.ViewChanged -= ActiveViewChanged;
			}
		}
	}

	class ActiveEditorChangedEventArgs : EventArgs
	{
		public ActiveEditorChangedEventArgs (ITextView newView, ITextView oldView)
		{
			NewView = newView;
			OldView = oldView;
		}

		public ITextView NewView { get; }
		public ITextView OldView { get; }
	}
}
