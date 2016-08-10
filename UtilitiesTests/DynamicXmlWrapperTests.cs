using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Tests
{
	[TestClass]
	public sealed class DynamicXmlWrapperTests
	{
		[TestMethod]
		public void TestItem()
		{
			dynamic wrapper = new DynamicXmlWrapper("<root><item>aaaa</item></root>", true);

			Assert.IsNotNull(wrapper.root);

			Assert.IsNotNull(wrapper.root.item);

			Assert.AreEqual("aaaa", wrapper.root.item);
		}

		[TestMethod]
		public void TestNullItem()
		{
			dynamic wrapper = new DynamicXmlWrapper("<root><item>aaaa</item></root>", true);

			Assert.IsNotNull(wrapper.root);

			Assert.IsNotNull(wrapper.root.item);

			Assert.IsNull(wrapper.root.item2);
		}

		[TestMethod]
		public void TestItemsArray()
		{
			dynamic wrapper = new DynamicXmlWrapper("<root><item>aaaa</item><item>bbbb</item></root>", true);

			Assert.IsNotNull(wrapper.root);

			Assert.IsNotNull(wrapper.root.item[0]);

			Assert.IsNotNull(wrapper.root.item[1]);

			Assert.AreEqual("aaaa", wrapper.root.item[0]);

			Assert.AreEqual("bbbb", wrapper.root.item[1]);
		}

		[TestMethod]
		public void TestAttr()
		{
			dynamic wrapper = new DynamicXmlWrapper("<root><item a1=\"value1\">aaaa</item><item>bbbb</item></root>", true);

			Assert.IsNotNull(wrapper.root);

			Assert.IsNotNull(wrapper.root.item[0]);

			Assert.IsNotNull(wrapper.root.item[0].a1);

			Assert.AreEqual("value1", wrapper.root.item[0].a1);

			Assert.AreEqual("value1", wrapper.root.item[0]["a1"]);
		}

		[TestMethod]
		public void TestNullAttr()
		{
			dynamic wrapper = new DynamicXmlWrapper("<root><item a1=\"value1\">aaaa</item><item>bbbb</item></root>", true);

			Assert.IsNotNull(wrapper.root);

			Assert.IsNotNull(wrapper.root.item[0]);

			Assert.IsNotNull(wrapper.root.item[0].a1);

			Assert.IsNull(wrapper.root.item[0].a2);
		}
	}
}
