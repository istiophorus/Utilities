using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace Utilities.Tests
{
	[TestClass]
	public sealed class FasterRollingBufferGenTest
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct SomeData : IEquatable<SomeData>
		{
			public SomeData(Int32 number, Int32 otherNumber, Byte someByte)
			{
				Number = number;

				OtherNumber = otherNumber;

				SomeByte = someByte;
			}

			[FieldOffset(0)]
			public Int32 Number;

			[FieldOffset(4)]
			public Int32 OtherNumber;

			[FieldOffset(8)]
			public Byte SomeByte;

			public Boolean Equals(SomeData other)
			{
				return other.Number == Number &&
					other.OtherNumber == OtherNumber &&
					other.SomeByte == SomeByte;
			}

			public override Boolean Equals(Object obj)
			{
				if (null == obj)
				{
					return false;
				}

				if (!(obj is SomeData))
				{
					return false;
				}

				SomeData oter = (SomeData)obj;

				return Equals(oter);
			}
		}

		private readonly Fixture _fixture = new Fixture();

		[TestMethod]
		public void SimpleWriteTest()
		{
			FasterRollingBufferGen<SomeData> buffer = new FasterRollingBufferGen<SomeData>(6);

			SomeData[] input = _fixture.CreateMany<SomeData>(6).ToArray();

			buffer.AddData(input.ToArray());

			SomeData[] output = buffer.GetData();

			CollectionAssert.AreEqual(input, output);
		}

		[TestMethod]
		public void SimpleOverwriteTest()
		{
			FasterRollingBufferGen<SomeData> buffer = new FasterRollingBufferGen<SomeData>(4);

			SomeData[] input = _fixture.CreateMany<SomeData>(6).ToArray();

			buffer.AddData(input.ToArray());

			SomeData[] output = buffer.GetData();

			SomeData[] inputReduced = ArrayTools.Subarray(input, 2, 4);

			CollectionAssert.AreEqual(inputReduced, output);
		}
	}
}
