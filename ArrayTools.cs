using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public static class ArrayTools
	{
		public static T[] Subarray<T>(T[] input, Int32 startIndex, Int32 length)
		{
			if (null == input)
			{
				throw new ArgumentNullException("null");
			}

			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex " + startIndex);
			}

			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length " + length);
			}

			if (startIndex + length > input.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex,length " + input.Length + " " + startIndex + length);
			}

			if (length == 0)
			{
				return EmptyArrayTemplate<T>.Instance;
			}

			T[] result = new T[length];

			Array.Copy(input, startIndex, result, 0, length);

			return result;
		}
	}
}
