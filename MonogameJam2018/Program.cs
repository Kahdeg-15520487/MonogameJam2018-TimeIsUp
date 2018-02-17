using System;
using System.IO;
using System.Security.Permissions;
using Utility;

namespace TimeIsUp {
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread, SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
		static void Main(string[] args) {
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += CurrentDomain_UnhandledException;

			CONTENT_MANAGER.ParseArguments(args);
			using (var game = new GameManager())
				game.Run();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			Exception ex = (Exception)e.ExceptionObject;
			string message = "==========" + Environment.NewLine +
							 ex.Message + Environment.NewLine +
							 ex.StackTrace + Environment.NewLine +
							 ex.TargetSite;
			string logfilename = "crashlog_" + DateTime.Now.ToString(@"dd_MM_yyyy_HH_mm") + ".txt";
			try {
				File.WriteAllText(logfilename, message);
			}
			catch (Exception exx) {
				throw exx;
			}
		}
	}
}
