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
using System.Threading.Tasks;

namespace Utilities
{
	public sealed class FasterRollingBuffer : IRollingBuffer
	{
		private Int32 _bufferMaxLength;

		public Int32 MaxBufferLength
		{
			get
			{
				return _bufferMaxLength;
			}
		}

		private readonly Int16[] _dataBuffer;

		private readonly Object _synchro = new Object();

		private Int32 _startPointer = 0;

		private Int32 _endPointer = 0;

		public FasterRollingBuffer(Int32 maxBufferlength)
		{
			if (maxBufferlength <= 0)
			{
				throw new ArgumentOutOfRangeException("maxBufferlength");
			}

			_bufferMaxLength = maxBufferlength;

			_dataBuffer = new Int16[_bufferMaxLength * 16];
		}

		public Int32 Count
		{
			get
			{
				lock (_synchro)
				{
					return _endPointer - _startPointer;
				}
			}
		}

		private static void BlockCopyInt16(Int16[] src, Int32 sourceIndex, Int16[] dst, Int32 targetIndex, Int32 items)
		{
			Buffer.BlockCopy(src, sourceIndex * 2, dst, targetIndex * 2, items * 2);
		}

		/// <summary>
		/// adds data to the internal data buffer
		/// </summary>
		/// <param name="data">data to add to buffer</param>
		public void AddData(Int16[] data)
		{
			if (null == data)
			{
				throw new ArgumentNullException("data");
			}

			lock (_synchro)
			{
				if (data.Length <= _bufferMaxLength)
				{
					if (_endPointer + data.Length >= _dataBuffer.Length)
					{
						Int32 items = _endPointer - _startPointer - data.Length;

						if (items > 0)
						{
							BlockCopyInt16(_dataBuffer, _startPointer + data.Length, _dataBuffer, 0, items);
						}

						_startPointer = 0;

						_endPointer = items;
					}

					BlockCopyInt16(data, 0, _dataBuffer, _endPointer, data.Length);

					_endPointer += data.Length;

					_startPointer = _endPointer - _bufferMaxLength;

					if (_startPointer < 0)
					{
						_startPointer = 0;
					}
				}
				else
				{
					BlockCopyInt16(data, data.Length - _bufferMaxLength, _dataBuffer, 0, _bufferMaxLength);

					_startPointer = 0;

					_endPointer = _bufferMaxLength;
				}
			}
		}

		/// <summary>
		/// gets current data
		/// </summary>
		/// <returns></returns>
		public Int16[] GetData()
		{
			lock (_synchro)
			{
				Int16[] result = new Int16[_endPointer - _startPointer];

				if (result.Length > 0)
				{
					BlockCopyInt16(_dataBuffer, _startPointer, result, 0, result.Length);
				}

				return result;
			}
		}
	}
}
