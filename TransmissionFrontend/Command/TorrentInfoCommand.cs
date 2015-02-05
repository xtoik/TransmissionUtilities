using System;
using log4net;

namespace TransmissionFrontend
{
	public class TorrentInfoCommand
	{
		readonly TransmissionRemoteWrapper _transmissionRemoteWrapper;
		readonly ILog _logger;

		public TorrentInfoCommand (TransmissionRemoteWrapper transmissionRemoteWrapper)
		{
			_transmissionRemoteWrapper = transmissionRemoteWrapper;
			_logger = LogManager.GetLogger(GetType());
		}

		public void FillInfo(TorrentInfo torrent)
		{
			string torrentInfo = _transmissionRemoteWrapper.ExecuteCommand(string.Format("-t {0} -i", torrent.Id));
			foreach (string line in torrentInfo.Split(new []{Environment.NewLine}, StringSplitOptions.None)) 
			{
				string attribute = line.Trim ();
				if (attribute.StartsWith ("Location:")) 
				{
					torrent.Location = attribute.Replace ("Location: ", string.Empty);
				}
			}
			_logger.DebugFormat ("Obtained the information of the torrent \"{0}\".", torrent.FileName);
		}
	}
}

