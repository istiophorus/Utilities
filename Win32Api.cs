using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public static class Win32Api
	{
		public static class Kernel32
		{
			[DllImport("Kernel32.dll", EntryPoint = "CopyMemory")]
			public static extern void CopyMemory(IntPtr pDest, IntPtr pSrc, Int32 length);
		}
	}
}
