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
using System.Linq;
using System.Text;

namespace Utilities
{
	public sealed class BitsShifter
	{
		private static readonly Byte[] LeftMasks = new Byte[]
			{
				0x00,
				0x80,
				0xC0,
				0xE0,
				0xF0,
				0xF8,
				0xFC,
				0xFE,
				0xFF
			};

		private static readonly Byte[] RightMasks = new Byte[]
			{
				0xFF,
				0x7F,
				0x3F,
				0x1F,
				0x0F,
				0x07,
				0x03,
				0x01,
				0x00
			};

		private const Int32 BitsPerByte = 8;

		internal static readonly Int32 MaxAllowedBinarySize = 8000;

		public static Byte[] RotateBitsRight(Byte[] input, Int32 bits)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			if (input.Length == 0)
			{
				return EmptyArrayTemplate<Byte>.Instance;
			}

			if (input.Length > MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Large binary objects are not supported");
			}

			Int32 bitsValue = bits;

			if (bitsValue == 0) //// trivial case
			{
				return input;
			}

			bitsValue = (Byte)(bitsValue % (input.Length * BitsPerByte));

			return RotateBitsLeft(input, input.Length * BitsPerByte - bitsValue);
		}

		public static Byte[] RotateBitsLeft(Byte[] input, Int32 bits)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			if (input.Length == 0)
			{
				return EmptyArrayTemplate<Byte>.Instance;
			}

			if (input.Length > MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Large binary objects are not supported");
			}

			Int32 bitsValue = bits;

			if (bitsValue == 0) //// trivial case
			{
				return input;
			}

			bitsValue = (Byte)(bitsValue % (input.Length * BitsPerByte));

			Byte[] result = (Byte[])input.Clone();

			Int32 bytesOffset = bitsValue / 8;
			Int32 bitsOffset = bitsValue % 8;

			for (Int32 q = 0; q < result.Length; q++)
			{
				Byte firstByte = 0;

				if (q + bytesOffset < result.Length)
				{
					firstByte = input[q + bytesOffset];
				}
				else
				{
					firstByte = input[q + bytesOffset - result.Length];
				}

				Byte secondByte = 0;

				if (1 + q + bytesOffset < result.Length)
				{
					secondByte = input[1 + q + bytesOffset];
				}
				else
				{
					secondByte = input[1 + q + bytesOffset - result.Length];
				}

				if (firstByte != 0 || secondByte != 0)
				{
					result[q] = (Byte)(
						((firstByte & RightMasks[bitsOffset]) << bitsOffset) |
						((secondByte & LeftMasks[bitsOffset]) >> (8 - bitsOffset)));
				}
				else
				{
					result[q] = 0;
				}
			}

			return result;
		}

		public static Byte[] ShiftBitsLeft(Byte[] input, Int32 bits)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			if (input.Length == 0)
			{
				return EmptyArrayTemplate<Byte>.Instance;
			}

			if (input.Length > MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Large binary objects are not supported");
			}

			Int32 bitsValue = bits;

			if (bitsValue == 0) //// trivial case
			{
				return input;
			}

			Byte[] result = (Byte[])input.Clone();

			Int32 bytesOffset = bitsValue / 8;
			Int32 bitsOffset = bitsValue % 8;

			for (Int32 q = 0; q < result.Length; q++)
			{
				Byte firstByte = 0;

				if (q + bytesOffset < result.Length)
				{
					firstByte = result[q + bytesOffset];
				}

				Byte secondByte = 0;

				if (1 + q + bytesOffset < result.Length)
				{
					secondByte = result[1 + q + bytesOffset];
				}

				if (firstByte != 0 || secondByte != 0)
				{
					result[q] = (Byte)(
						((firstByte & RightMasks[bitsOffset]) << bitsOffset) |
						((secondByte & LeftMasks[bitsOffset]) >> (8 - bitsOffset)));
				}
				else
				{
					result[q] = 0;
				}
			}

			return result;
		}

		public static Byte[] ShiftBitsRight(Byte[] input, Int32 bits)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			if (input.Length == 0)
			{
				return EmptyArrayTemplate<Byte>.Instance;
			}

			if (input.Length > MaxAllowedBinarySize)
			{
				throw new ArgumentOutOfRangeException("Large binary objects are not supported");
			}

			Int32 bitsValue = bits;

			if (bitsValue == 0) //// trivial case
			{
				return input;
			}

			Byte[] result = (Byte[])input.Clone();

			Int32 bytesOffset = bitsValue / 8;
			Int32 bitsOffset = bitsValue % 8;

			for (Int32 q = result.Length - 1; q >= 0; q--)
			{
				Byte firstByte = 0;

				if (q - bytesOffset >= 0)
				{
					firstByte = result[q - bytesOffset];
				}

				Byte secondByte = 0;

				if (q - bytesOffset - 1 >= 0)
				{
					secondByte = result[q - bytesOffset - 1];
				}

				if (firstByte != 0 || secondByte != 0)
				{
					result[q] = (Byte)(
						((firstByte & LeftMasks[8 - bitsOffset]) >> bitsOffset) |
						((secondByte & RightMasks[8 - bitsOffset]) << (8 - bitsOffset)));
				}
				else
				{
					result[q] = 0;
				}
			}

			return result;
		}
	}
}
