using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Xml.Tests
{
	public class TestDataType
	{

		public int IntegerProperty { get; set; }

		public string StringProperty { get; set; }

		public Guid GuidProperty { get; set; }

		public System.ConsoleColor EnumProperty { get; set; }

		public bool BooleanProperty { get; set; }

		public TestDataType()
		{
			Random rand = new Random();
			IntegerProperty = rand.Next();
			GuidProperty = Guid.NewGuid();
			StringProperty = GuidProperty.ToString();
			BooleanProperty = (IntegerProperty % 2) == 0;
			EnumProperty = (ConsoleColor)rand.Next(0, 15);
		}

		public override bool Equals(object obj)
		{
			TestDataType x = (TestDataType)obj;
			return true
				&& this.BooleanProperty == this.BooleanProperty
				&& this.EnumProperty == this.EnumProperty
				&& this.GuidProperty == this.GuidProperty
				&& this.IntegerProperty == x.IntegerProperty
				&& this.StringProperty == this.StringProperty;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}
}
