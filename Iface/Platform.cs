using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gdk;

namespace Moscrif.IDE.Iface
{
	public class Platform
	{
		public Platform ()
		{
 			IsWindows = System.IO.Path.DirectorySeparatorChar == '\\';
 			IsMac = !IsWindows && IsRunningOnMac();
			IsX11 = !IsMac && System.Environment.OSVersion.Platform == PlatformID.Unix;
		}

		public bool IsMac { get; private set; }
		public bool IsX11 { get; private set; }
		public bool IsWindows { get; private set; }


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
			} catch {
			} finally {
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return false;
		}
	}
}

