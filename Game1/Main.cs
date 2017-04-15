using Foundation;
using UIKit;

namespace Game1
{
	[Register("AppDelegate")]
	class Program : UIApplicationDelegate
	{
		private static match.pcl.Game1 game;

		internal static void RunGame()
		{
			game = new match.pcl.Game1();
			game.Run();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}

		public override void FinishedLaunching(UIApplication app)
		{
			RunGame();
		}
	}
}
