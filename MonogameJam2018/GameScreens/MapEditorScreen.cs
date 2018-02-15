using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Utility;
using Utility.Drawing;
using Utility.ScreenManager;
using Utility.UI;

namespace TimeIsUp.GameScreens {
	class MapEditorScreen : Screen {
		Canvas canvas;
		Map map;

		DrawBatch drawBatch;
		Camera camera;

		public MapEditorScreen(GraphicsDevice device) : base(device, "MapEditorScreen") { }

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
