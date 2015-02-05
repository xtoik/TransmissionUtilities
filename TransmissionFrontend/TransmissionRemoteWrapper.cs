using System;
using log4net;
using System.Diagnostics;

namespace TransmissionFrontend
{
	public class TransmissionRemoteWrapper
	{
		readonly string _server;
		readonly string _user;
		readonly string _password;
		readonly ILog _logger;

		public TransmissionRemoteWrapper (string server, string user, string password)
		{
			_server = server;
			_user = user;
			_password = password;
			_logger = LogManager.GetLogger(GetType());
		}

		public string ExecuteCommand (string command)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.FileName = "transmission-remote";
			processStartInfo.Arguments = string.Format("{0} -n {1}:{2} {3}", _server, _user, _password, command);

			_logger.DebugFormat("Launching command: {0} {1}", processStartInfo.FileName, processStartInfo.Arguments);

			string output;
			using(Process process = Process.Start(processStartInfo))
			{
				output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
			}

			_logger.DebugFormat("Command output:\r\n{0}", output);

			return output;
		}
	}
}

