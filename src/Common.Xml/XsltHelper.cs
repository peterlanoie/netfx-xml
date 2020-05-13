using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Xml
{

	/// <summary>
	/// Defines XSLT transform functionality.
	/// </summary>
	public class XsltHelper
	{

		/// <summary>
		/// Transforms an XML stream to a string of HTML using the provided transformation file.
		/// </summary>
		/// <param name="xmlStream">Incoming XML stream to transform.</param>
		/// <param name="xsltFilePath">Path to the XSLT file to use for the transformation.</param>
		/// <returns></returns>
		public static string XmlToHtmlString(Stream xmlStream, string xsltFilePath)
		{
			return XmlToHtmlString(xmlStream, xsltFilePath, null);
		}


		/// <summary>
		/// Transforms an XML stream to a string of HTML using the provided transformation file and arguments.
		/// </summary>
		/// <param name="xmlStream">Incoming XML stream to transform.</param>
		/// <param name="xsltFilePath">Path to the XSLT file to use for the transformation.</param>
		/// <param name="xsltArgs">Collection of XSLT arguments.</param>
		/// <returns></returns>
		public static string XmlToHtmlString(Stream xmlStream, string xsltFilePath, XsltArgumentList xsltArgs)
		{
			Stream objHtmlStream;
			StreamReader objStreamReader;
			XmlTextWriter objHtmlWriter;
			string strResult;
			string strXsltFile = xsltFilePath;

			//Transform XML string to HTML stream
			objHtmlStream = new MemoryStream();
			objHtmlWriter = new XmlTextWriter(objHtmlStream, Encoding.Default);
			objHtmlWriter.Formatting = Formatting.Indented;
			TransformToHtml(xmlStream, xsltFilePath, xsltArgs, objHtmlStream);
			objHtmlStream.Position = 0;
			objStreamReader = new StreamReader(objHtmlStream);
			strResult = objStreamReader.ReadToEnd();
			objStreamReader.Close();
			return strResult;
		}

		/// <summary>
		/// Transforms an XML stream into HTML.
		/// </summary>
		/// <param name="xmlStream"></param>
		/// <param name="xsltFilePath"></param>
		/// <param name="xsltArgs"></param>
		/// <param name="outStream"></param>
		public static void TransformToHtml(Stream xmlStream, string xsltFilePath, XsltArgumentList xsltArgs, Stream outStream)
		{
			XslCompiledTransform objXsl;
			XmlReader objXmlReader;
			XmlTextWriter objHtmlWriter;

			objXsl = VerifyAndLoadXslt(xsltFilePath);

			//Transform XML string to HTML stream
			xmlStream.Position = 0;
			objXmlReader = new XmlTextReader(xmlStream);
			objHtmlWriter = new XmlTextWriter(outStream, Encoding.Default);
			objHtmlWriter.Formatting = Formatting.Indented;
			objXsl.Transform(objXmlReader, xsltArgs, objHtmlWriter);
		}

		/// <summary>
		/// Performs an XSLT transformation of an XML file to a string.
		/// </summary>
		/// <param name="xmlFilePath">Path to the source XML file.</param>
		/// <param name="xsltFilePath">Path to the XSLT transformation to use.</param>
		/// <param name="xsltArgs">Optional XSLT arguments.  Can be NULL.</param>
		/// <returns></returns>
		public static string TransformFileToString(string xmlFilePath, string xsltFilePath, XsltArgumentList xsltArgs)
		{
			string result;
			XslCompiledTransform objXsl = VerifyAndLoadXslt(xsltFilePath);
			StringWriter writer = new StringWriter();
			objXsl.Transform(xmlFilePath, xsltArgs, writer);
			result = writer.ToString();
			writer.Close();
			return result;
		}

		private static XslCompiledTransform VerifyAndLoadXslt(string xsltFilePath)
		{
			XslCompiledTransform objXsl = new XslCompiledTransform();
			//First, check for existing xslt file.
			if(!File.Exists(xsltFilePath))
			{
				throw new FileNotFoundException("XSLT File not found.", xsltFilePath);
			}
			objXsl.Load(xsltFilePath);
			return objXsl;
		}

	}
}
