using System.IO;
using System.Xml;

namespace Common.Xml
{

	/// <summary>
	/// Defines helper methods for instances of an <see cref="System.Xml.XmlDocument"/>.
	/// </summary>
	public class XmlDocumentHelper
	{

		/// <summary>
		/// Writes an XmlDocument to a string using an XmlWriter.
		/// </summary>
		/// <param name="xmlDoc">The source XmlDocument.</param>
		/// <param name="settings">Optional settings to use for the internal XmlWriter.</param>
		/// <returns></returns>
		public static string WriteForString(XmlDocument xmlDoc, XmlWriterSettings settings)
		{
			using(var stringWriter = new StringWriter())
			using(var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
			{
				xmlDoc.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				return stringWriter.ToString();
			}
		}
	}
}
