using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Tests
{
	[TestClass]
	public sealed class ArrayToolsSubarrayTests
	{
		[TestMethod]
		public void StaticTest1()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(d1, ArrayTools.Subarray(d1, 0, d1.Length));
		}

		[TestMethod]
		public void StaticTest2()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5
                },
							ArrayTools.Subarray(d1, 0, d1.Length - 1));
		}

		[TestMethod]
		public void StaticTest3()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new[]
                {
                    1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                },
							ArrayTools.Subarray(d1, 1, d1.Length - 1));
		}

		[TestMethod]
		public void StaticTest4()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new[]
                {
                    1.2, 1.3, 1.4, 1.5, 1.6
                },
							ArrayTools.Subarray(d1, 2, d1.Length - 2));
		}

		[TestMethod]
		public void StaticTest5()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new[]
                {
                    1.6
                },
							ArrayTools.Subarray(d1, d1.Length - 1, 1));
		}

		[TestMethod]
		public void StaticTest6()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new[]
                {
                    1.5, 1.6
                },
							ArrayTools.Subarray(d1, d1.Length - 2, 2));
		}

		[TestMethod]
		public void StaticTest7()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new Double[0],
							ArrayTools.Subarray(d1, d1.Length, 0));
		}

		[TestMethod]
		public void StaticTest8()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			CollectionAssert.AreEqual(new Double[0],
							ArrayTools.Subarray(d1, 0, 0));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError1()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, -1, d1.Length);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError2()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, d1.Length + 1, 1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError3()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, 0, d1.Length + 1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError4()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, 0, -1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError5()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, 5, -1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError6()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, 1, d1.Length);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void StaticTestError7()
		{
			var d1 = new[]
                {
                    1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6
                };

			ArrayTools.Subarray(d1, d1.Length, 1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void StaticTestError8()
		{
			ArrayTools.Subarray<Double>(null, 0, 123);
		}

	}
}
