using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.IO;
using log4net;

namespace Common.Xml
{

	/// <summary>
	/// Automatically loads, monitors and reloads an object stored as an XML file.
	/// </summary>
	/// <typeparam name="Type"></typeparam>
	public class CachedFileStoredObject<Type> where Type : class,  new()
	{
		private readonly ILog _log = LogManager.GetLogger("Common.Xml.CachedFileStoredObject");
		private Type _current;
		private FileSystemWatcher _watcher;
		private string _lockName = Guid.NewGuid().ToString();

		/// <summary>
		/// Event to notify a consumer that the underlying file for this cached object has been changed and that it will reload on the next .Current request.
		/// </summary>
		public event Action FileChanged;

		///// <summary>
		///// Get/set the name used for exclusive locking of the config file read operation(s).
		///// </summary>
		//public string LockName
		//{
		//    get { return _lockName; }
		//    set { _lockName = value; }
		//}
		
		/// <summary>
		/// Get/set the file name of the config file.
		/// </summary>
		public string FileName { get; private set; }

		///// <summary>
		///// Get/set the directory in which the configuration file lives.
		///// Can be left null to use the current runtime directory.
		///// (Must be used in a web application: supply it with the resolved location of '~\bin'.)
		///// </summary>
		//public string ConfigDir { get; set; }

		/// <summary>
		/// Returns the current config.  The first access will load it from the specified file, cache it and set up a file watch on it.
		/// </summary>
		public Type Current
		{
			get { return GetCurrent(); }
		}

		/// <summary>
		/// Creates a new instance of the class with the specified config file name.
		/// </summary>
		/// <param name="fileName">File name of the config file.</param>
		public CachedFileStoredObject(string fileName)
		{
			FileName = fileName;
		}

		///// <summary>
		///// Creates a new instance of the class.
		///// </summary>
		//public CachedConfigFile()
		//{
		//}

		private Type GetCurrent()
		{
			if(_current == null)
			{
				string dir;
				string strFile = PathHelper.GetFullFileName(FileName); //resolve full file path based on runtime env
				if(!File.Exists(strFile))
				{
					throw new FileNotFoundException(string.Format("Expected file '{0}' missing.", strFile));
				}
				_current = SerializationHelper.LoadFromFile<Type>(_lockName, strFile);
				if(_watcher == null)
				{
					dir = PathHelper.GetDirectory(strFile);
					_log.DebugFormat("creating file watcher on directory '{0}' to monitor file '{1}'", dir, Path.GetFileName(strFile));
					_watcher = new FileSystemWatcher(dir);
					_watcher.Changed += new FileSystemEventHandler(_watcher_Event);
					_watcher.Created += new FileSystemEventHandler(_watcher_Event);
					_watcher.Deleted += new FileSystemEventHandler(_watcher_Event);
					_watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
					_watcher.EnableRaisingEvents = true;
				}
			}
			return _current;
		}

		void _watcher_Renamed(object sender, RenamedEventArgs e)
		{
			CheckForChange(e.Name, e.ChangeType);
			CheckForChange(e.OldName, e.ChangeType);
		}

		private void _watcher_Event(object sender, FileSystemEventArgs e)
		{
			CheckForChange(e.Name, e.ChangeType);
		}

		private void CheckForChange(string fileName, WatcherChangeTypes changeType)
		{
			if(fileName == Path.GetFileName(FileName))
			{
				if(FileChanged != null)
				{
					FileChanged();
				}
				_log.DebugFormat("detected file change event '{0}' for file '{1}', clearing cached object for reload", changeType, FileName);
				_current = null;
			}
		}

	}
}
