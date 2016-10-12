using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Addins;

namespace MonoDevelop.Addins.Tasks
{
	class LogProgressStatus : IProgressStatus
	{
		readonly TaskLoggingHelper log;
		readonly int logLevel;
		bool isCanceled;

		public LogProgressStatus (TaskLoggingHelper log, int logLevel = 0)
		{
			this.logLevel = logLevel;
			this.log = log;
		}

		public void SetMessage (string msg)
		{
			log.LogMessage (MessageImportance.Low, msg);
		}

		public void SetProgress (double progress)
		{
		}

		public void Log (string msg)
		{
			log.LogMessage (MessageImportance.Low, msg);
		}

		public void ReportWarning (string message)
		{
			log.LogWarning (message);
		}

		public void ReportError (string message, Exception exception)
		{
			if (exception == null)
				log.LogError ("{0}", message);
			else
				log.LogError ("{0}: {1}", message, exception);
		}

		public void Cancel ()
		{
			isCanceled = true;
		}

		public int LogLevel {
			get { return logLevel; }
		}

		public bool IsCanceled {
			get { return isCanceled; }
		}
	}
}
