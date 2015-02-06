using System;
using TransmissionFrontend;
using System.Collections.Generic;
using log4net.Config;
using log4net;
using System.Reflection;
using System.Linq;
using System.IO;

namespace MoveAfterSeeding
{
	class MainClass
	{
		static string _server;
		static string _user;
		static string _password;
		static string _destination;
		static ILog _logger;

		public static void Main (string[] args)
		{
			XmlConfigurator.Configure();
			_logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			if (parseArgs (args)
				&& checkDestination()) 
			{
				TransmissionRemoteWrapper transmissionRemoteWrapper = new TransmissionRemoteWrapper (_server, _user, _password);
				TorrentListCommand torrentList = new TorrentListCommand (transmissionRemoteWrapper);
				List<TorrentInfo> torrents = torrentList.GetTorrents ();
				TorrentInfoCommand torrentInfo = new TorrentInfoCommand (transmissionRemoteWrapper);
				TorrentRemoveCommand torrentRemove = new TorrentRemoveCommand (transmissionRemoteWrapper);
				if (torrents.All (x => x.Status != TorrentStatus.Finished))
					_logger.Info ("There aren't finished torrents to move.");
				foreach (TorrentInfo torrent in torrents.Where(x => x.Status == TorrentStatus.Finished))
				{
					torrentInfo.FillInfo (torrent);
					torrentRemove.Remove (torrent);
					if (!Directory.Exists (torrent.Location))
					{
						_logger.ErrorFormat ("The location of the torrent \"{0}\" (\"{1}\") is not a folder.", torrent.FileName, torrent.Location);
					}
					else
					{
						DirectoryInfo destinationFolder = new DirectoryInfo (trimPath(_destination));
						DirectoryInfo originFolder = new DirectoryInfo (trimPath(torrent.Location));
						DirectoryInfo[] subfolders = originFolder.GetDirectories ();
						FileInfo[] files = originFolder.GetFiles ();
						bool isFolder = subfolders != null && subfolders.Any (x => x.Name == torrent.FileName);
						bool isFile = files != null && files.Any (x => x.Name == torrent.FileName);
						if (!isFolder && !isFile)
						{
							_logger.ErrorFormat ("The torrent \"{0}\" cannot be found on its supposed location (\"{1}\").", torrent.FileName, torrent.Location);
						}
						else if (destinationFolder.FullName == originFolder.FullName)
						{
							_logger.InfoFormat ("The torrent \"{1}\" is already in the destination folder: \"{0}\".", torrent.Location, torrent.FileName);
						}
						else if (isFile || isFolder)
						{
							string destinationPath = destinationFolder.FullName + Path.DirectorySeparatorChar + torrent.FileName;
							if (File.Exists (destinationPath))
							{
								File.Delete (destinationPath);
								_logger.InfoFormat ("The destination file \"{0}\" already exists. It has been deleted.", destinationPath);
							}
							else if (Directory.Exists(destinationPath))
							{
								Directory.Delete (destinationPath);
								_logger.InfoFormat ("The destination folder \"{0}\" already exists. It has been deleted.", destinationPath);
							}
							string originPath = originFolder.FullName + Path.DirectorySeparatorChar + torrent.FileName;
							if (isFile) File.Move (originPath, destinationPath);
							else Directory.Move (originPath, destinationPath);
							if (_logger.IsDebugEnabled) _logger.DebugFormat ("The {2} \"{0}\" has been moved to \"{1}\".", originPath, destinationPath, isFile ? "file" : "folder");
							else _logger.InfoFormat ("\"{0}\" moved.", torrent.FileName);
						}
					}
				}
			}
		}

		private static bool checkDestination()
		{
			bool ret = true;

			if (File.Exists(_destination))
			{
				_logger.ErrorFormat ("The destination provided (\"{0}\") is the path to a file, and therefore cannot be used.", _destination);
				ret = false;
			} else if (!Directory.Exists(_destination))
			{
				Directory.CreateDirectory(_destination);
				_logger.InfoFormat ("The destination folder \"{0}\" has been created.", _destination);
			}

			return ret;
		}

		static string trimPath(string path)
		{
			return path.TrimEnd (Path.DirectorySeparatorChar).TrimEnd (Path.AltDirectorySeparatorChar);
		}

		private static bool parseArgs(string[] args)
		{
			bool argsOk = args != null && args.Length > 0;

			if (argsOk)
			{
				foreach (string arg in args)
				{
					if (arg.Trim().Length < 4)
					{ 
						argsOk = false;
						break;
					}

					switch (arg.Trim().Substring(0, 3))
					{
					case "-s=":
						_server = getArgValue(arg);
						break;
					case "-u=":
						_user = getArgValue(arg);
						break;
					case "-p=":
						_password = getArgValue(arg);
						break;
					case "-d=":
						_destination = getArgValue(arg);
						break;
					default:
						argsOk = false;
						break;
					}

					if (!argsOk) break;
				}
			}

			if (!argsOk) Console.WriteLine("Expected: -s=<server[:port]> -u=<user> -p=<pwd> -d=<destFolder>");
			else _logger.DebugFormat("user: {0}\r\npwd: {1}\r\nserver: {2}\r\ndestFolder: {3}", _user, _password, _server, _destination);

			return argsOk;
		}

		static string getArgValue(string arg)
		{
			return arg.Trim().Substring(3);
		}
	}
}
