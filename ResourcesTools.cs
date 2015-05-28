#region License

/* 
   Copyright (C) 2015, Rafal Skotak
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.
*/

/*
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion License

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public static class ResourcesTools
	{
		/// <summary>
		/// loads a specified resource and returns as a string
		/// </summary>
		/// <param name="assembly">assembly to load resource from</param>
		/// <param name="resourceName">name of the resource to load</param>
		/// <returns>resource value</returns>
		public static String LoadResourceToString(Assembly assembly, String resourceName)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}

			if (null == resourceName)
			{
				throw new ArgumentNullException("resourceName");
			}

			using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
			{
				return sr.ReadToEnd();
			}
		}

		/// <summary>
		/// loads a specified resource and returns as a string
		/// </summary>
		/// <param name="assembly">assembly to load resource from</param>
		/// <param name="resourceName">name of the resource to load</param>
		/// <returns>resource value</returns>
		public static String LoadResourceToString(Assembly assembly, String resourceName, Encoding encoding)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}

			if (null == resourceName)
			{
				throw new ArgumentNullException("resourceName");
			}

			using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName), encoding))
			{
				return sr.ReadToEnd();
			}
		}

		public static Icon LoadIconFromResources(Assembly assembly, String resourceName)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}

			if (null == resourceName)
			{
				throw new ArgumentNullException("resourceName");
			}

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				return new Icon(stream);
			}
		}

		/// <summary>
		/// reads a specified resource and returns as byte array
		/// </summary>
		/// <param name="assembly">assembly to load resource from</param>
		/// <param name="resourceName">name of the resource to load</param>
		/// <returns>resource as array of bytes</returns>
		public static Byte[] LoadResourceBytes(Assembly assembly, String resourceName)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}

			if (null == resourceName)
			{
				throw new ArgumentNullException("resourceName");
			}

			using (BinaryReader br = new BinaryReader(assembly.GetManifestResourceStream(resourceName)))
			{
				Byte[] arr = new Byte[br.BaseStream.Length];

				br.Read(arr, 0, arr.Length);

				return arr;
			}
		}
	}
}
