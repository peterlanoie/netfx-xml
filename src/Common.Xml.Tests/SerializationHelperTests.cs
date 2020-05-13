using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Common.IO;

namespace Common.Xml.Tests
{
	[TestClass]
	public class SerializationHelperTests
	{
		private const string LOCK_NAME = "testdata";
		private string _fileName;

		public SerializationHelperTests()
		{
			_fileName = PathHelper.GetFullFileName("TestData.xml");
		}

		[TestInitialize]
		public void Init()
		{
			if(File.Exists(_fileName))
			{
				File.Delete(_fileName);
			}
		}

		[TestMethod]
		public void StringSerializationTest()
		{
			StringSerializer<TestDataType> ser = SerializationHelper.GetStringSerializer<TestDataType>();
			TestDataType data = new TestDataType();
			TestDataType data2;
			string dataAsString;

			dataAsString = ser.ObjectToString(data);
			data2 = ser.StringToObject(dataAsString);
			Assert.IsTrue(data.Equals(data2));
		}

		[TestMethod]
		public void FileSerializationTest()
		{
			FileSerializer<TestDataType> ser = SerializationHelper.GetFileSerializer<TestDataType>(LOCK_NAME, _fileName);
			TestDataType data = new TestDataType();
			TestDataType data2;

			ser.WriteObjectToFile(data);
			Assert.IsTrue(File.Exists(_fileName));

			data2 = ser.LoadObjectFromFile();
			Assert.IsTrue(data.Equals(data2));
		}


		[TestMethod]
		public void StringMethods()
		{
			TestDataType data = new TestDataType();
			TestDataType data2;
			string dataAsString;
			dataAsString = SerializationHelper.ObjectToString<TestDataType>(data);
			data2 = SerializationHelper.StringToObject<TestDataType>(dataAsString);
			Assert.IsTrue(data.Equals(data2));
		}

		[TestMethod]
		public void FileMethods()
		{
			TestDataType data = new TestDataType();
			TestDataType data2;
			SerializationHelper.SaveToFile<TestDataType>(data, LOCK_NAME, _fileName);
			Assert.IsTrue(File.Exists(_fileName));
			data2 = SerializationHelper.LoadFromFile<TestDataType>(LOCK_NAME, _fileName);
			Assert.IsTrue(data.Equals(data2));
		}

	}
}
