using System;
using System.Collections.Generic;
using log4net;
using System.Text.RegularExpressions;

namespace TransmissionFrontend
{
	public class TorrentListCommand
	{
		readonly TransmissionRemoteWrapper _transmissionRemoteWrapper;
		readonly ILog _logger;

		public TorrentListCommand(TransmissionRemoteWrapper transmissionRemoteWrapper)
		{
			_transmissionRemoteWrapper = transmissionRemoteWrapper;
			_logger = LogManager.GetLogger(GetType());
		}

		public List <TorrentInfo> GetTorrents()
		{
			List<TorrentInfo> ret = new List<TorrentInfo> ();
			string output = _transmissionRemoteWrapper.ExecuteCommand("-l");
			string[] outputLines = output.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
			if (outputLines[0].StartsWith("ID"))
			{
				for (int line = 1; line < outputLines.Length - 2; line++)
				{
					ret.Add (parseTorrentInfo (outputLines [line]));
				}
			}
			return ret;
		}

		TorrentInfo parseTorrentInfo (string torrenInfo)
		{
			string text = torrenInfo.Trim ().Replace ("\t", " ");
			Regex regex = new Regex ("\\s(\\s)+");
			text = regex.Replace (text, "\t");
			string[] fields = text.Split ('\t');
			TorrentInfo torrent = new TorrentInfo {
				Id = long.Parse (fields [0].Replace("*", string.Empty)),
				FileName = fields [8]
			};
			TorrentStatus status;
			if (!Enum.TryParse (homogenizeStatus(fields [7]), out status)) {
				_logger.ErrorFormat ("The torrent status \"{0}\" is unknown.", fields [7]);
				status = TorrentStatus.Unknown;
			}
			torrent.Status = status;
			return torrent;
		}

		string homogenizeStatus(string status)
		{
			return status.Replace (" & ", string.Empty);
		}
	}
}

