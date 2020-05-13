using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using log4net;
using Common.IO;

namespace Common.Xml
{
	/// <summary>
	/// Provides serialization functionality for managed types stored as XML files.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FileSerializer<T> where T : new()
	{
		private readonly ILog _log = LogManager.GetLogger("Common.Xml.FileSerializer");
		private string _mutexName;
		private string _file;

		/// <summary>
		/// Get/set the file to be read from or written to.
		/// </summary>
		public string XmlFile
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		/// Create a new instance of a type <typeparamref name="T"/> file serializer with a lock name and file path.
		/// </summary>
		/// <param name="mutexName">Name of the lock mutex to use to block file operations.</param>
		/// <param name="file">Path to the file that will be read from and/or written to.</param>
		public FileSerializer(string mutexName, string file) : this(mutexName, file, false)
		{
		}

		/// <summary>
		/// Create a new instance of a type <typeparamref name="T"/> file serializer with a lock name, file path and global option.
		/// A relative file path will be resolve to a full runtime path.
		/// </summary>
		/// <param name="mutexName">Name of the lock mutex to use to block file operations.</param>
		/// <param name="file">Path to the file that will be read from and/or written to.</param>
		/// <param name="isGlobal">Whether the lock is global to the OS or limited to the running app domain.</param>
		public FileSerializer(string mutexName, string file, bool isGlobal)
		{
			string globPref = @"Global\";
			_mutexName = mutexName;
			if(isGlobal && !_mutexName.StartsWith(globPref))
			{
				_mutexName = string.Concat(globPref, _mutexName);
			}
			_file = PathHelper.GetFullFileName(file);
		}

		/// <summary>
		/// Writes the specified object using Xml serialization.
		/// </summary>
		/// <param name="obj">The object to write.</param>
		public void WriteObjectToFile(T obj)
		{
			WithLock(delegate()
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				using(TextWriter writer = new StreamWriter(_file))
				{
					serializer.Serialize(writer, obj);
				}
			});
		}

		/// <summary>
		/// Loads and returns the object to be used, via Xml deserialization.
		/// </summary>
		/// <returns>The deserialized object.</returns>
		public T LoadObjectFromFile()
		{
			return LoadObjectFromFile(false);
		}

		/// <summary>
		/// Loads and returns the object to be used, via Xml deserialization.
		/// </summary>
		/// <returns>The deserialized object.</returns>
		/// <param name="createDefault">Whether or not to create a default type instance and write out to a file.</param>
		public T LoadObjectFromFile(bool createDefault)
		{
			T ret = default(T);

			_log.DebugFormat("checking existence of file '{0}'", _file);
			if(!File.Exists(_file))
			{
				_log.DebugFormat("file doesn't exist, create it? {0}", createDefault);
				if(createDefault)
				{
					T defaults = new T();
					WriteObjectToFile(defaults);
					ret = defaults;
				}
			}
			else
			{
				_log.Debug("file exists, deserialize it");
				WithLock(delegate()
				{
					// file exists, so deserialise it
					_log.DebugFormat("creating instance of XmlSerializer for type '{0}'", typeof(T));
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					_log.DebugFormat("opening reader for file '{0}'", _file);
					using(TextReader reader = new StreamReader(_file))
					{
						_log.DebugFormat("deserializing file '{0}'", _file);
						ret = (T)serializer.Deserialize(reader);
					}
				});
			}
			return ret;
		}

		private void WithLock(Action lockedAction)
		{
			Mutex fileLock = new Mutex(false, _mutexName);
			_log.DebugFormat("waiting for mutex '{0}' availability", _mutexName);
			fileLock.WaitOne(); // wait for any other process to finish handling the file
			try
			{
				lockedAction();
			}
			finally
			{
				_log.DebugFormat("releasing mutex '{0}'", _mutexName);
				fileLock.ReleaseMutex();
			}
		}

	}
}
