using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace Utilities.Tests
{
	[TestClass]
	public sealed class BinaryEncoderTests
	{
		private static void TestValues<T>(T[] input, Action<BinaryWriter, T> writeAction, Func<BinaryReader, T> readAction)
			where T : IEquatable<T>
		{
			Byte[] data;

			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					Array.ForEach(input, x => writeAction(writer, x));
				}

				data = stream.ToArray();
			}

			List<T> items = new List<T>();

			using (MemoryStream stream = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					for (Int32 q = 0; q < input.Length; q++)
					{
						T item = readAction(reader);

						items.Add(item);
					}
				}
			}

			CollectionAssert.AreEqual(input, items);
		}

		private readonly BinaryEncoder _encoder = new BinaryEncoder();

		public BinaryEncoderTests()
		{
		}

		[TestMethod]
		public void ReadWriteTestInt32()
		{
			TestValues(
				new Int32[] { 0, 1, 2, 3, 4, 7, 8, 15, 16, 31, 32 },
				(w, x) => _encoder.WriteInt32Encoded(w, x),
				r => _encoder.ReadInt32Decoded(r));
		}

		[TestMethod]
		public void ReadWriteTestInt64()
		{
			TestValues(
				new Int64[] { 0, 1, 2, 3, 4, 7, 8, 15, 16, 31, 32 },
				(w, x) => _encoder.WriteInt64Encoded(w, x),
				r => _encoder.ReadInt64Decoded(r));
		}

		[TestMethod]
		public void ReadWriteTestUInt64()
		{
			TestValues(
				new UInt64[] { 0, 1, 2, 3, 4, 7, 8, 15, 16, 31, 32 },
				(w, x) => _encoder.WriteUInt64Encoded(w, x),
				r => _encoder.ReadUInt64Decoded(r));
		}
	}
}
