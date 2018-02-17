using Humper;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Utility;
using Utility.Drawing;
using Utility.ScreenManager;
using Utility.UI;

namespace TimeIsUp.GameScreens {
	class MapEditorScreen : Screen {
		Canvas canvas;
		Map map;
		World world;

		DrawBatch drawBatch;
		Camera camera;
		MapRenderer mapRenderer;

		public string Mapname { get; set; }
		float maxdepth;

		MessageBox msgbox;

		Vector2 cursorSpriteOrigin = new Vector2(128, 612);

		bool isDrawCollisionBox = true;
		bool isDrawInteractLink = true;

		public MapEditorScreen(GraphicsDevice device) : base(device, "MapEditorScreen") { }

		public override bool Init() {
			InitUI();

			return base.Init();
		}

		public void InitUI() {
			canvas = new Canvas();

			InputBox inputBox = new InputBox("", new Point(400, 50), new Vector2(80, 30), CONTENT_MANAGER.Fonts["hack"], Color.White, Color.Black);
			canvas.AddElement("inputBox", inputBox);
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		private void MoveCamera(KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			if (HelperMethod.IsKeyHold(Keys.A, currentKeyboardState, lastKeyboardState)) {
				camera.Centre -= new Vector2(10, 0);
			}
			if (HelperMethod.IsKeyHold(Keys.D, currentKeyboardState, lastKeyboardState)) {
				camera.Centre += new Vector2(10, 0);
			}
			if (HelperMethod.IsKeyHold(Keys.W, currentKeyboardState, lastKeyboardState)) {
				camera.Centre -= new Vector2(0, 10);
			}
			if (HelperMethod.IsKeyHold(Keys.S, currentKeyboardState, lastKeyboardState)) {
				camera.Centre += new Vector2(0, 10);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			spriteBatch.BeginSpriteBatch();
			canvas.Draw(spriteBatch, gameTime);
			spriteBatch.EndSpriteBatch();
		}
	}
}
