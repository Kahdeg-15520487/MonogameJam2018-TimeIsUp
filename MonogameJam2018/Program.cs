using System;
using Utility;

namespace TimeIsUp {
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			CONTENT_MANAGER.ParseArguments(args);
            using (var game = new GameManager())
                game.Run();
        }
    }
}
