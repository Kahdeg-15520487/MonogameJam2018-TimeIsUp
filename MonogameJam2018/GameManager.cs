using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeIsUp.GameScreens;
using Utility;
using Utility.Drawing;
using Utility.ScreenManager;

namespace TimeIsUp {
	public class GameManager : Game {
		GraphicsDeviceManager graphics;
		internal static object map;

		public GameManager() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			CONTENT_MANAGER.Content = Content;
		}

		protected override void Initialize() {
			CONTENT_MANAGER.LocalRootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
			DrawingHelper.Initialize(GraphicsDevice);
			CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);
			CONTENT_MANAGER.gameInstance = this;
			CONTENT_MANAGER.CurrentInputState = new InputState(Mouse.GetState(), Keyboard.GetState());
			base.Initialize();
		}

		private void InitScreen() {
			SCREEN_MANAGER.add_screen(new MainPlayScreen(GraphicsDevice));

			SCREEN_MANAGER.goto_screen("MainPlayScreen");

			SCREEN_MANAGER.Init();
		}

		protected override void LoadContent() {
			CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);

			#region Load spritesheet
			#endregion

			#region Load audio
			#endregion

			InitScreen();
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			CONTENT_MANAGER.CurrentInputState = new InputState(Mouse.GetState(), Keyboard.GetState());

			SCREEN_MANAGER.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			CONTENT_MANAGER.BeginSpriteBatch();
			{
				SCREEN_MANAGER.Draw(CONTENT_MANAGER.spriteBatch, gameTime);
			}
			CONTENT_MANAGER.EndSpriteBatch();

			base.Draw(gameTime);
		}
	}
}
