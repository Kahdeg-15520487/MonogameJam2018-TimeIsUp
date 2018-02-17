//using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utility;
using Utility.ScreenManager;
using Utility.UI;

namespace Wartorn.Screens {
	class MainMenuScreen : Screen {
		Canvas canvas;

		public MainMenuScreen(GraphicsDevice device) : base(device, "MainMenuScreen") { }

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
			canvas.Update(gameTime,CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			spriteBatch.BeginSpriteBatch();
			canvas.Draw(spriteBatch, gameTime);
			spriteBatch.EndSpriteBatch();
		}
	}
}
