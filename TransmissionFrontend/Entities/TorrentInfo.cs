using System;

namespace TransmissionFrontend
{
	public class TorrentInfo
	{
		public long Id { get; set; }
		public TorrentStatus Status { get; set; }
		public string FileName { get; set; }
		public string Location { get; set; }
	}
}

