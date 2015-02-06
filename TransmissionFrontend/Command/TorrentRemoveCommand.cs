using System;
using log4net;

namespace TransmissionFrontend
{
	public class TorrentRemoveCommand
	{
		readonly TransmissionRemoteWrapper _transmissionRemoteWrapper;
		readonly ILog _logger;

		public TorrentRemoveCommand (TransmissionRemoteWrapper transmissionRemoteWrapper)
		{
			_transmissionRemoteWrapper = transmissionRemoteWrapper;
			_logger = LogManager.GetLogger(GetType());
		}

		public bool Remove(TorrentInfo torrent)
		{
			string output = _transmissionRemoteWrapper.ExecuteCommand(string.Format("-t {0} -r", torrent.Id));
			bool ret = output.TrimEnd ().EndsWith ("responded: \"success\"");
			if (!ret) _logger.ErrorFormat ("The torrent \"{0}\" cannot be removed: {1}.", torrent.FileName, output);
			else _logger.DebugFormat ("The torrent \"{0}\" has been removed properly.", torrent.FileName);
			return ret;
		}
	}
}

