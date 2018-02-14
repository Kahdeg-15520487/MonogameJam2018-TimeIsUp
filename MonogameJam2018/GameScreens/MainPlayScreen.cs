using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Utility.ScreenManager;
using Utility.UI;
using Utility;
using Utility.Drawing;
using Humper;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using LilyPath;
using System;
using Microsoft.Xna.Framework.Input;

namespace TimeIsUp.GameScreens {
	class MainPlayScreen : Screen {

		enum GameState {
			None,
			Ready,
			Ingame,
			Endgame,
			MapEdit
		}

		GameState gameState = GameState.None;

		DrawBatch drawBatch;
		Canvas canvas;
		Camera camera;
		Map map;
		public string Mapname { get; set; }
		internal static float maxdepth;
		World world;
		List<IMovableObject> MovableObjects;
		Player player;
		Timer timer;
		Label label_timer;

		MessageBox msgbox;

		bool isWin;
		bool isDrawCollisionBox = true;
		bool isDrawInteractLink = true;

		internal static Texture2D spritesheet;
		internal static SpriteFont font;
		internal static Dictionary<SpriteSheetRectName, Rectangle> spriterects;
		private Vector2 pppp = new Vector2(0, -60);

		private Vector2 spawnpoint;
		private Vector2 endpoint;
		private bool msgboxMiddleButtonPressed;
		private bool msgboxLeftButtonPressed;
		private bool msgboxRightButtonPressed;

		public MainPlayScreen(GraphicsDevice device) : base(device, "MainPlayScreen") { }

		public override bool Init() {
			drawBatch = new DrawBatch(device);
			camera = new Camera(device.Viewport);
			timer = new Timer();

			spritesheet = CONTENT_MANAGER.Sprites["spritesheet"];
			font = CONTENT_MANAGER.Fonts["default"];

			var temp = File.ReadAllText(@"Content/sprite/spritesheet.json");
			spriterects = JsonConvert.DeserializeObject<List<KeyValuePair<string, Rectangle>>>(temp).ToDictionary(kvp => kvp.Key.ToEnum<SpriteSheetRectName>(), kvp => kvp.Value);

			//InitGame();

			InitUI();

			return base.Init();
		}

		private void InitGame() {
			timer.Reset();

			map = MapLoader.LoadMap(Mapname);
			maxdepth = ((map.Width + 1) + (map.Height + 1) + (map.Depth + 1)) * 10;
			map.LoadContent();

			isWin = false;

			world = new World(map.Width, map.Height);

			foreach (var collbox in map.Collsion) {
				world.Create(collbox.X, collbox.Y, collbox.Width, collbox.Height);
			}

			MovableObjects = new List<IMovableObject>();

			List<Object> MarkedForRemove = new List<Object>();
			spawnpoint = new Vector2();

			foreach (var obj in map.Objects.Values) {
				var box = world.Create(obj.BoundingBox.X, obj.BoundingBox.Y, obj.BoundingBox.Width, obj.BoundingBox.Height);
				box.AddTags(obj.CollisionTag);
				obj.CollsionBox = box;
				if (obj.TileType.GetCollisionTag() == CollisionTag.PushableBlock) {
					MarkedForRemove.Add(obj);
					var block = new Block() {
						CollisionBox = box,
						Object = obj
					};
					MovableObjects.Add(block);
					box.Data = block;
				}
				else {
					box.Data = obj;
				}
			}

			var sp = map.FindObject("spawnpoint");
			if (sp != null) {
				spawnpoint = new Vector2(sp.WorldPos.X, sp.WorldPos.Y);
				world.Remove(sp.CollsionBox);
				MarkedForRemove.Add(sp);
			}

			var ep = map.FindObject("endpoint");
			if (ep != null) {
				endpoint = new Vector2(ep.WorldPos.X, ep.WorldPos.Y);
			}

			foreach (var obj in MarkedForRemove) {
				map.Objects.Remove(obj.Name);
			}

			foreach (var obj in MovableObjects) {
				obj.LoadContent(this);
			}
			var pbox = world.Create(spawnpoint.X, spawnpoint.Y, 0.5f, 0.5f);
			Object pobj = new Object() {
				Name = "player",
				CollsionBox = pbox,
				TileType = SpriteSheetRectName.None
			};
			map.Objects.Add(pobj.Name, pobj);
			player = new Player {
				Object = pobj,
				CollisionBox = pobj.CollsionBox
			};
			player.LoadContent(this);
			player.Origin = new Vector2(128, 512);
			MovableObjects.Add(player);
		}

		public void InitUI() {
			canvas = new Canvas();
			msgbox = new MessageBox(new Point(280, 200), "text", "OK");
			msgbox.MiddleButtonPressed += (o, e) => msgboxMiddleButtonPressed = true;
			msgbox.LeftButtonPressed += (o, e) => msgboxLeftButtonPressed = true;
			msgbox.RightButtonPressed += (o, e) => msgboxRightButtonPressed = true;
			label_timer = new Label("", new Point(10, 10), new Vector2(50, 30), font) {
				Origin = new Vector2(0, -20)
			};
			canvas.AddElement("msgbox", msgbox);
			canvas.AddElement("label_timer", label_timer);
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		internal void Win() {
			isWin = true;
		}

		public override void Update(GameTime gameTime) {

			if (HelperMethod.IsKeyPress(Keys.R, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
				gameState = GameState.None;
			}

			switch (gameState) {
				case GameState.None:
					gameState = GameState.Ready;
					InitGame();
					msgbox.Show("Ready?", "Go");
					break;
				case GameState.Ready:
					if (msgboxMiddleButtonPressed) {
						msgboxMiddleButtonPressed = false;
						gameState = GameState.Ingame;
						msgbox.Hide();
						timer.Start();
					}
					break;
				case GameState.Ingame:
					if (isWin) {
						gameState = GameState.Endgame;
						timer.Stop();
						msgbox.Show("You finished the level in:" + Environment.NewLine + timer.ElapsedTime + " s" + Environment.NewLine + "Restart?", "Next lvl", "Exit", "Restart");
						break;
					}

					timer.Update(gameTime);

					if (HelperMethod.IsKeyPress(Keys.P, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
						gameState = GameState.None;
					}

					if (HelperMethod.IsKeyPress(Keys.L, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
						isDrawCollisionBox = !isDrawCollisionBox;
					}

					if (HelperMethod.IsKeyPress(Keys.O, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
						isDrawInteractLink = !isDrawInteractLink;
					}

					label_timer.Text = timer.ElapsedTime.ToString();

					for (int i = 0; i < MovableObjects.Count; i++) {
						MovableObjects[i].Update(gameTime, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState);
					}
					break;
				case GameState.Endgame:
					if (msgboxMiddleButtonPressed) {
						msgboxMiddleButtonPressed = false;
						//goto next level
						//gameState = GameState.None;
						break;
					}
					if (msgboxLeftButtonPressed) {
						msgboxLeftButtonPressed = false;
						//goto main menu
						//gameState = GameState.None;
						break;
					}
					if (msgboxRightButtonPressed) {
						msgboxRightButtonPressed = false;
						//restart next level
						gameState = GameState.None;
						break;
					}
					break;
				default:
					break;
			}

			camera.Centre = player.IsoPos;

			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		private void MoveCamera(KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			camera.Centre = player.IsoPos;
			//if (HelperMethod.IsKeyHold(Keys.Left, currentKeyboardState, lastKeyboardState)) {
			//	camera.Centre -= new Vector2(10, 0);
			//}
			//if (HelperMethod.IsKeyHold(Keys.Right, currentKeyboardState, lastKeyboardState)) {
			//	camera.Centre += new Vector2(10, 0);
			//}
			//if (HelperMethod.IsKeyHold(Keys.Up, currentKeyboardState, lastKeyboardState)) {
			//	camera.Centre -= new Vector2(0, 10);
			//}
			//if (HelperMethod.IsKeyHold(Keys.Down, currentKeyboardState, lastKeyboardState)) {
			//	camera.Centre += new Vector2(0, 10);
			//}

			//if (HelperMethod.IsKeyHold(Keys.J, currentKeyboardState, lastKeyboardState)) {
			//	pppp -= new Vector2(1, 0);
			//}
			//if (HelperMethod.IsKeyHold(Keys.L, currentKeyboardState, lastKeyboardState)) {
			//	pppp += new Vector2(1, 0);
			//}
			//if (HelperMethod.IsKeyHold(Keys.I, currentKeyboardState, lastKeyboardState)) {
			//	pppp -= new Vector2(0, 1);
			//}
			//if (HelperMethod.IsKeyHold(Keys.K, currentKeyboardState, lastKeyboardState)) {
			//	pppp += new Vector2(0, 1);
			//}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			spriteBatch.BeginSpriteBatchWithCamera(camera, SpriteSortMode.BackToFront);

			map.Draw(spriteBatch, gameTime);

			var depthoffset = 0.7f - ((player.WorldPos.X + player.WorldPos.Y) / maxdepth) - 0.002f;
			MovableObjects.ForEach(x => x.Draw(spriteBatch, gameTime, depthoffset));
			spriteBatch.EndSpriteBatch();

			if (isDrawCollisionBox) {
				drawBatch.Begin();
				var b = world.Bounds.ToRectangle();
				var b1 = new Vector2(b.X, b.Y);
				world.DrawDebug((int)b1.X, (int)b1.Y, b.Width, b.Height, DrawCell, DrawBox, DrawString);
				drawBatch.End();
			}

			if (isDrawInteractLink) {
				drawBatch.Begin();
				foreach (var link in map.InteractLink) {
					DrawLine(link);
				}
				drawBatch.End();
			}

			spriteBatch.BeginSpriteBatch(SpriteSortMode.FrontToBack);
			canvas.Draw(spriteBatch, gameTime);
			spriteBatch.EndSpriteBatch();
		}

		private void DrawLine(Line line) {
			var p1 = camera.TranslateFromWorldToScreen(line.Point1.WorldToIso()) + pppp;
			var p2 = camera.TranslateFromWorldToScreen(line.Point2.WorldToIso()) + pppp;
			drawBatch.DrawLine(Pen.Cyan, p1, p2);
		}

		private void DrawBox(IBox box) {
			var p0 = new Vector2(box.X, box.Y).WorldToIso();
			var p1 = new Vector2(box.X + box.Width, box.Y).WorldToIso();
			var p2 = new Vector2(box.X + box.Width, box.Y + box.Height).WorldToIso();
			var p3 = new Vector2(box.X, box.Y + box.Height).WorldToIso();

			p0 = camera.TranslateFromWorldToScreen(p0);
			p1 = camera.TranslateFromWorldToScreen(p1);
			p2 = camera.TranslateFromWorldToScreen(p2);
			p3 = camera.TranslateFromWorldToScreen(p3);

			p0 += pppp;
			p1 += pppp;
			p2 += pppp;
			p3 += pppp;

			drawBatch.DrawLine(Pen.Red, p0, p1);
			drawBatch.DrawLine(Pen.Red, p1, p2);
			drawBatch.DrawLine(Pen.Red, p2, p3);
			drawBatch.DrawLine(Pen.Red, p3, p0);
		}

		private void DrawCell(int x, int y, int w, int h, float alpha) {
			return;
		}

		private void DrawString(string message, int x, int y, float alpha) {
			return;
		}
	}
}
