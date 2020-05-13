using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace System.Xml
{
	/// <summary>
	/// Defines extension methods for System.Xml.XmlDocument
	/// </summary>
	public static class Ext_XmlDocument
	{
		/// <summary>
		/// Returns the document's InnerXml formatted using 2 space indentation.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static string GetFormattedInnerXml(this XmlDocument doc)
		{
			return GetFormattedInnerXml(doc, null, null);
		}

		/// <summary>
		/// Returns the document's InnerXml formatted using the specificed character and indentation count.
		/// </summary>
		/// <param name="doc">The XmlDocument instance.</param>
		/// <param name="indentChar">The character to use for indentation.</param>
		/// <param name="indentation">The number of characters to use for each indentation.</param>
		/// <returns></returns>
		public static string GetFormattedInnerXml(this XmlDocument doc, char indentChar, int indentation)
		{
			return GetFormattedInnerXml(doc, indentChar, indentation);
		}

		private static string GetFormattedInnerXml(this XmlDocument doc, char? indentChar, int? indentation)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			XmlTextWriter xtw = null;
			try
			{
				xtw = new XmlTextWriter(sw);
				if(indentChar != null)
				{
					xtw.IndentChar = (char)indentChar;
				}
				if(indentation != null)
				{
					xtw.Indentation = (int)indentation;
				}
				xtw.Formatting = Formatting.Indented;
				doc.WriteTo(xtw);
			}
			finally
			{
				if(xtw != null)
				{
					xtw.Close();
				}
			}
			return sb.ToString();
		}

	}
}
