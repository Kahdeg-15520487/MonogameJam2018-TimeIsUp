using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;
using TimeIsUp.GameScreens;
using Utility;
using Utility.Drawing;
using Utility.ScreenManager;

namespace TimeIsUp {
	public class GameManager : Game {
		GraphicsDeviceManager graphics;
		internal static Map map;

		public GameManager() {
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			CONTENT_MANAGER.Content = Content;
			IsMouseVisible = true;
			
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

			#region Load font
			CONTENT_MANAGER.LoadFont("default");
			#endregion

			#region Load spritesheet
			CONTENT_MANAGER.LoadSprites("animation", "spritesheet");
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
			GraphicsDevice.Clear(Color.WhiteSmoke);

			SCREEN_MANAGER.Draw(CONTENT_MANAGER.spriteBatch, gameTime);

			base.Draw(gameTime);
		}
	}
}
