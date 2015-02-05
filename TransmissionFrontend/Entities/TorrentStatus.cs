using System;

namespace TransmissionFrontend
{
	public enum TorrentStatus
	{
		Queued,
		Idle,
		Downloading,
		Stopped,
		Seeding,
		Finished,
		Unknown
	}
}

