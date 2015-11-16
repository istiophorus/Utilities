using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

		public BinaryEncoderTests()
        {
            _prepareValueForUInt32 = x =>
                {
                    _random.NextBytes(x);

                    Int32 zeroCount = _random.Next() % x.Length;

                    for (Int32 w = x.Length - zeroCount; w < x.Length; w++)
                    {
                        x[w] = 0;
                    }

                    return BitConverter.ToUInt32(x, 0);
                };
        }

        private readonly Random _random = new Random(Environment.TickCount);

        private void Test<T>(
            Int32 testsCount, 
            Func<Byte[], T> prepareValue, 
            Func<BinaryWriter, T, Int32> serializationAction, 
            Func<BinaryReader, T> deserializationAction) where T : struct, IComparable
        {
            var r = new Random(Environment.TickCount);

            var buffer = new Byte[Marshal.SizeOf(typeof(T))];

            for (Int32 q = 0; q < testsCount; q++)
            {
                T testValue = prepareValue(buffer);

                SingleValueTest(serializationAction, deserializationAction, testValue);
            }
        }

        private static void SingleValueTest<T>(
            Func<BinaryWriter, T, Int32> serializationAction, 
            Func<BinaryReader, T> deserializationAction, 
            T testValue) where T : struct, IComparable
        {
            Byte[] bytes;

            using (var ms = new MemoryStream())
            {
                Int32 bytesCount;

                using (var writer = new BinaryWriter(ms))
                {
                    bytesCount = serializationAction(writer, testValue);
                }

                bytes = ms.ToArray();

                Assert.AreEqual(bytesCount, bytes.Length, "Returned bytes count is different than buffer size " + bytesCount + " " + bytes.Length);
            }

            using (var ms = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(ms))
                {
                    T value = deserializationAction(reader);

                    Assert.AreEqual(testValue, value, "Values are different " + testValue + " " + value);
                }
            }
        }

        private const Int32 TestsCount = 100000;

		[TestMethod]
        public void TestRandomInt64()
        {
            Test<Int64>(
                TestsCount, 
                x =>
                    {
                        _random.NextBytes(x);

                        Int32 zeroCount = _random.Next() % x.Length;

                        for (Int32 w = x.Length - zeroCount; w < x.Length; w++)
                        {
                            x[w] = 0;
                        }

                        return BitConverter.ToInt64(x, 0);
                    }, 
                (w, v) => _encoder.WriteInt64Encoded(w, v), 
                r => _encoder.ReadInt64Decoded(r));
        }

		[TestMethod]
        public void TestRandomUInt64()
        {
            Test<UInt64>(
                TestsCount, 
                x =>
                    {
                        _random.NextBytes(x);

                        Int32 zeroCount = _random.Next() % x.Length;

                        for (Int32 w = x.Length - zeroCount; w < x.Length; w++)
                        {
                            x[w] = 0;
                        }

                        return BitConverter.ToUInt64(x, 0);
                    }, 
                (w, v) => _encoder.WriteUInt64Encoded(w, v), 
                r => _encoder.ReadUInt64Decoded(r));
        }

		[TestMethod]
        public void TestRandomInt32()
        {
            Test<Int32>(
                TestsCount, 
                x =>
                    {
                        _random.NextBytes(x);

                        Int32 zeroCount = _random.Next() % x.Length;

                        for (Int32 w = x.Length - zeroCount; w < x.Length; w++)
                        {
                            x[w] = 0;
                        }

                        return BitConverter.ToInt32(x, 0);
                    }, 
                (w, v) => _encoder.WriteInt32Encoded(w, v), 
                r => _encoder.ReadInt32Decoded(r));
        }

        private readonly Func<Byte[], UInt32> _prepareValueForUInt32;

		[TestMethod]
        public void TestRandomUInt32()
        {
            Test<UInt32>(
                TestsCount, 
                _prepareValueForUInt32, 
                (w, v) => _encoder.WriteUInt32Encoded(w, v), 
                r => _encoder.ReadUInt32Decoded(r));
        }

		[TestMethod]
        public void TestZero()
        {
            SingleValueAllTests(0);
        }

		[TestMethod]
        public void TestMax()
        {
            SingleValueTest<Int32>(
                (w, v) => _encoder.WriteInt32Encoded(w, v), 
                r => _encoder.ReadInt32Decoded(r), 
                Int32.MaxValue);

            SingleValueTest<Int32>(
                (w, v) => _encoder.WriteInt32Encoded(w, v), 
                r => _encoder.ReadInt32Decoded(r), 
                Int32.MinValue);

            SingleValueTest<UInt32>(
                (w, v) => _encoder.WriteUInt32Encoded(w, v), 
                r => _encoder.ReadUInt32Decoded(r), 
                UInt32.MaxValue);

            SingleValueTest<UInt32>(
                (w, v) => _encoder.WriteUInt32Encoded(w, v), 
                r => _encoder.ReadUInt32Decoded(r), 
                UInt32.MinValue);

            SingleValueTest<Int64>(
                (w, v) => _encoder.WriteInt64Encoded(w, v), 
                r => _encoder.ReadInt64Decoded(r), 
                0);

            SingleValueTest<Int64>(
                (w, v) => _encoder.WriteInt64Encoded(w, v), 
                r => _encoder.ReadInt64Decoded(r), 
                Int64.MaxValue);

            SingleValueTest<Int64>(
                (w, v) => _encoder.WriteInt64Encoded(w, v), 
                r => _encoder.ReadInt64Decoded(r), 
                Int64.MinValue);

            SingleValueTest<UInt64>(
                (w, v) => _encoder.WriteUInt64Encoded(w, v), 
                r => _encoder.ReadUInt64Decoded(r), 
                UInt64.MaxValue);

            SingleValueTest<UInt64>(
                (w, v) => _encoder.WriteUInt64Encoded(w, v), 
                r => _encoder.ReadUInt64Decoded(r), 
                UInt64.MinValue);
        }

		[TestMethod]
        public void TestSeries()
        {
            for (Int32 q = 0; q < 10000000; q += 13)
            {
                Int64 testValue = q;

                SingleValueAllTests(testValue);
            }
        }

        private void SingleValueAllTests(Int64 testValue)
        {
            if (testValue <= Int32.MaxValue && testValue >= Int32.MinValue)
            {
                SingleValueTest<Int32>(
                    (w, v) => _encoder.WriteInt32Encoded(w, v), 
                    r => _encoder.ReadInt32Decoded(r), 
                    (Int32)testValue);

                SingleValueTest<UInt32>(
                    (w, v) => _encoder.WriteUInt32Encoded(w, v), 
                    r => _encoder.ReadUInt32Decoded(r), 
                    (UInt32)testValue);
            }

            SingleValueTest<Int64>(
                (w, v) => _encoder.WriteInt64Encoded(w, v), 
                r => _encoder.ReadInt64Decoded(r), 
                (Int64)testValue);

            SingleValueTest<UInt64>(
                (w, v) => _encoder.WriteUInt64Encoded(w, v), 
                r => _encoder.ReadUInt64Decoded(r), 
                (UInt64)testValue);
        }

		[TestMethod]
        public void TestBorderValues()
        {
            SingleValueAllTests(-1);

            SingleValueAllTests(0);

            SingleValueAllTests(1);

            SingleValueAllTests(0x80 - 1);

            SingleValueAllTests(0x80);

            SingleValueAllTests(0x4000 - 1);

            SingleValueAllTests(0x4000);

            SingleValueAllTests(0x200000 - 1);

            SingleValueAllTests(0x200000);

            SingleValueAllTests(0x10000000 - 1);

            SingleValueAllTests(0x10000000);

            SingleValueAllTests(0x0800000000 - 1);

            SingleValueAllTests(0x0800000000);

            SingleValueAllTests(0x040000000000 - 1);

            SingleValueAllTests(0x040000000000);

            SingleValueAllTests(0x02000000000000 - 1);

            SingleValueAllTests(0x02000000000000);

            SingleValueAllTests(0x0100000000000000 - 1);

            SingleValueAllTests(0x0100000000000000);
        }

		[TestMethod]
        public void TestBorderValuesRanges()
        {
            var ranges = new Int64[]
                {
                    0, 
                    0x80, 
                    0x4000, 
                    0x200000, 
                    0x10000000, 
                    0x0800000000, 
                    0x040000000000, 
                    0x02000000000000, 
                    0x0100000000000000
                };

            for (Int32 q = 0; q < ranges.Length; q++)
            {
                Int64 range = ranges[q];

                for (Int64 w = range - 16; w < range + 16384; w++)
                {
                    SingleValueAllTests(w);
                }
            }
        }
 	}
}
