using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using TiledSharp;
using TimeIsUp.GameScreens;
using Utility;
using Utility.Drawing;
using Utility.ScreenManager;

namespace TimeIsUp {
	public class GameManager : Game {
		GraphicsDeviceManager graphics;

		public GameManager() {
			graphics = new GraphicsDeviceManager(this) {
				PreferredBackBufferWidth = 800,
				PreferredBackBufferHeight = 600
			};
			graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			CONTENT_MANAGER.Content = Content;
			IsMouseVisible = true;

			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new MapJsonConverter());
				settings.Converters.Add(new ObjectJsonConverter());
				settings.Converters.Add(new RectangleFJsonConverter());
				return settings;
			};


		}

		protected override void Initialize() {
			CONTENT_MANAGER.LocalRootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
			DrawingHelper.Initialize(GraphicsDevice);
			CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);
			CONTENT_MANAGER.gameInstance = this;
			CONTENT_MANAGER.CurrentInputState = new InputState(Mouse.GetState(), Keyboard.GetState());

			Primitive2DActionGenerator.CreateThePixel(CONTENT_MANAGER.spriteBatch);

			base.Initialize();
		}

		private void InitScreen() {

			var playscreen = new MainPlayScreen(GraphicsDevice) {
				MapName = CONTENT_MANAGER.MapName ?? "menu"
			};

			TransitionScreen transitionScreen = new TransitionScreen(GraphicsDevice) {
				StartingDirectory = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map"),
				SearchPattern = "*.tmx",
				CallBack = x => {
					playscreen.MapName = x;
					SCREEN_MANAGER.GotoScreen("MainPlayScreen");
				}
			};

			SCREEN_MANAGER.AddScreen(playscreen);
			SCREEN_MANAGER.AddScreen(transitionScreen);

			SCREEN_MANAGER.GotoScreen("MainPlayScreen");

			SCREEN_MANAGER.Init();
		}

		protected override void LoadContent() {
			CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);

			#region Load font
			CONTENT_MANAGER.LoadFont("default", "hack");
			#endregion

			#region Load spritesheet
			CONTENT_MANAGER.LoadSprites("animation", "spritesheet");
			#endregion

			#region Load audio
			CONTENT_MANAGER.LoadSound("door_close", "door_open", "footstep", "lever", "switch_pressed", "switch_released", "tele", "light_on", "light_off");
			#endregion

			InitScreen();
		}

		protected override void Update(GameTime gameTime) {
			//if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			//	Exit();

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
