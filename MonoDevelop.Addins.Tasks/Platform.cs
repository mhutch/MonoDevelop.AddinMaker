using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoDevelop.Addins.Tasks
{
	static class Platform
	{
		public readonly static bool IsWindows;
		public readonly static bool IsMac;

		static Platform ()
		{
			IsWindows = Path.DirectorySeparatorChar == '\\';
			IsMac = !IsWindows && IsRunningOnMac ();
		}

		[DllImport ("libc")]
		static extern int uname (IntPtr buf);

		//From Managed.Windows.Forms/XplatUI
		static bool IsRunningOnMac ()
		{
			IntPtr buf = IntPtr.Zero;
			try {
				buf = Marshal.AllocHGlobal (8192);
				// This is a hacktastic way of getting sysname from uname ()
				if (uname (buf) == 0) {
					string os = Marshal.PtrToStringAnsi (buf);
					if (os == "Darwin")
						return true;
				}
			// Analysis disable once EmptyGeneralCatchClause
			} catch {
			} finally {
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return false;
		}
	}
}