using System;

namespace TransmissionFrontend
{
	public enum TorrentStatus
	{
		Queued,
		Idle,
		Downloading,
		UpDown,
		Uploading,
		Stopped,
		Seeding,
		Finished,
		Unknown
	}
}

