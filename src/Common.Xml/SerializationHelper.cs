using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using log4net;
using Common.IO;
using System.Xml.Serialization;

namespace Common.Xml
{
	/// <summary>
	/// Provides methods for managing XML based configuration files of strongly typed configurations.
	/// Maintains an in-memory collection of serializers and reuses them appropriately.
	/// </summary>
	public static class SerializationHelper
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(SerializationHelper));

		private static OrderedDictionary _serializers = new OrderedDictionary();

		private delegate object CreateSerializerDelegate();

		private static T GetSerializer<T>(string name, CreateSerializerDelegate createSerializerCallback)
		{
			object objSerializer;
			T objConcreteSerializer;

			_log.Debug("loading serializer");
			objSerializer = _serializers[name];
			if(objSerializer != null && objSerializer is T)
			{
				_log.DebugFormat("serializer exists for {0}", typeof(T));
				objConcreteSerializer = (T)objSerializer;
			}
			else
			{
				_log.DebugFormat("creating serializer for {0}", typeof(T));
				objConcreteSerializer = (T)createSerializerCallback();
				_serializers[name] = objConcreteSerializer;
			}
			return objConcreteSerializer;
		}

		/// <summary>
		/// Gets a file serializer for type <typeparamref name="T"/>.
		/// Serializers are cached for quicker subsequent retrieval.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static FileSerializer<T> GetFileSerializer<T>(string fileLockName, string fileName) where T : new()
		{
			return GetSerializer<FileSerializer<T>>(string.Format("{0}_file", typeof(T)), delegate()
			{
				_log.DebugFormat("creating new file serializer for type '{0}' with lock '{1}' for file '{2}'", typeof(T), fileLockName, fileName);
				return new FileSerializer<T>(fileLockName, fileName);
			});
		}

		/// <summary>
		/// Gets a string serializer for type <typeparamref name="T"/>.
		/// Serializers are cached for quicker subsequent retrieval.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static StringSerializer<T> GetStringSerializer<T>() where T : new()
		{
			return GetSerializer<StringSerializer<T>>(string.Format("{0}_string", typeof(T)), delegate()
			{
				_log.DebugFormat("creating new string serializer for type '{0}'", typeof(T));
				return new StringSerializer<T>();
			});
		}

		/// <summary>
		/// Load an instance of type <typeparamref name="T"/> from <paramref name="fileName"/>.
		/// </summary>
		/// <typeparam name="T">The type of the class to be populated from the serialized file.</typeparam>
		/// <param name="fileLockName">The name of the lock to use when reading the file.</param>
		/// <param name="fileName">The name of the file. A relative name will resolve to the runtime directory.</param>
		/// <returns></returns>
		public static T LoadFromFile<T>(string fileLockName, string fileName) where T : new()
		{
			FileSerializer<T> objSerializer = GetFileSerializer<T>(fileLockName, fileName);
			_log.DebugFormat("loading type instance from file {0}", objSerializer.XmlFile);
			return objSerializer.LoadObjectFromFile(false);
		}

		/// <summary>
		/// Saves instance <paramref name="obj"/> of type <typeparamref name="T"/> as XML to <paramref name="fileName"/>.
		/// </summary>
		/// <typeparam name="T">The type of the class to be save to the serialized file.</typeparam>
		/// <param name="obj">Object instance to save.</param>
		/// <param name="fileLockName">The name of the lock to use when writing the file.</param>
		/// <param name="fileName">The name of the file. A relative name will resolve to the runtime directory.</param>
		/// <returns></returns>
		public static void SaveToFile<T>(T obj, string fileLockName, string fileName) where T : new()
		{
			FileSerializer<T> objSerializer = GetFileSerializer<T>(fileLockName, fileName);
			objSerializer.WriteObjectToFile(obj);
		}

		/// <summary>
		/// Converts a string of XML into an instance of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml">String of serialized XML that will be convert to an object.</param>
		/// <returns></returns>
		public static T StringToObject<T>(string xml) where T : new()
		{
			StringSerializer<T> objSerializer = GetStringSerializer<T>();
			_log.DebugFormat("loading type instance from xml string");
			return objSerializer.StringToObject(xml);
		}

		/// <summary>
		/// Converts an instance of type <typeparamref name="T"/> into its XML representation as a string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">Object instance to convert.</param>
		/// <returns></returns>
		public static string ObjectToString<T>(T obj) where T : new()
		{
			StringSerializer<T> objSerializer = GetStringSerializer<T>();
			_log.DebugFormat("serializing type instance to xml string");
			return objSerializer.ObjectToString(obj);
		}

		/// <summary>
		/// Serializes an object to a Stream of XML.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Stream ObjectToXmlStream(object obj)
		{
			Stream objXmlStream;
			XmlSerializer objSerializer;

			//Serialize the object
			objSerializer = new XmlSerializer(obj.GetType(), string.Empty);
			objXmlStream = new MemoryStream();
			objSerializer.Serialize(objXmlStream, obj);

			return objXmlStream;
		}

	}
}
