using System;

namespace TransmissionFrontend
{
	public enum TorrentStatus
	{
		Queued,
		Idle,
		Downloading,
		UpDown,
		Stopped,
		Seeding,
		Finished,
		Unknown
	}
}

