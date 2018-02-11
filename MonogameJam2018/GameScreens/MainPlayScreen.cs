using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Utility.ScreenManager;
using Utility.UI;
using Utility;

namespace TimeIsUp.GameScreens {
	class MainPlayScreen : Screen {
		Canvas canvas;

		public MainPlayScreen(GraphicsDevice device) : base(device, "MainPlayScreen") { }

		public override bool Init() {
			InitUI();

			return base.Init();
		}

		public void InitUI() {
			canvas = new Canvas();
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			canvas.Draw(spriteBatch, gameTime);
		}
	}
}
