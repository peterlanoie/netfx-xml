using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.Xml
{
	/// <summary>
	/// Provides methods for XML serialization.
	/// </summary>
	/// <typeparam name="T">The type to be de/serialized by this serializer.</typeparam>
	public class StringSerializer<T> where T : new()
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public StringSerializer()
		{
		}

		/// <summary>
		/// Serializes an object into an XML string using the object's type serializer.
		/// </summary>
		/// <param name="obj">Object to </param>
		/// <returns></returns>
		public string ObjectToString(T obj)
		{
			XmlSerializer objXS = new XmlSerializer(obj.GetType());
			string strResult;
			using(System.IO.StringWriter objSW = new System.IO.StringWriter())
			{
				objXS.Serialize(objSW, obj);
				strResult = objSW.ToString();
				objSW.Close();
			}
			return strResult;
		}

		/// <summary>
		/// Serializes an object into an XML string using the object's type serializer.
		/// </summary>
		/// <param name="obj">Object to </param>
		/// <param name="ExtraTypes">Extra types for serialization</param>
		/// <returns></returns>
		public string ObjectToString(T obj, Type[] ExtraTypes)
		{
			XmlSerializer objXS = new XmlSerializer(obj.GetType(), ExtraTypes);
			string strResult;
			using(System.IO.StringWriter objSW = new System.IO.StringWriter())
			{
				objXS.Serialize(objSW, obj);
				strResult = objSW.ToString();
				objSW.Close();
			}
			return strResult;
		}

		/// <summary>
		/// Deserializes a string of XML data into an object of type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="xml">Serialized object data.</param>
		/// <returns></returns>
		public T StringToObject(string xml)
		{
			XmlSerializer objXS = new XmlSerializer(typeof(T));
			object objData;

			using(System.IO.StringReader objSR = new System.IO.StringReader(xml))
			{
				objData = objXS.Deserialize(objSR);
				objSR.Close();
			}
			return (T)objData;
		}

	}

}
