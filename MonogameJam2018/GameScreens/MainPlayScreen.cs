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
using System.Text;

namespace TimeIsUp.GameScreens {
	class MainPlayScreen : Screen {

		enum GameState {
			None,
			Ready,
			Ingame,
			Endgame,
			MapEdit,
			Pause
		}

		Stack<GameState> gameStates;
		GameState CurrentGameState {
			get {
				return gameStates.Peek();
			}
			set {
				gameStates.Pop();
				gameStates.Push(value);
			}
		}

		DrawBatch drawBatch;
		Canvas canvas;
		Camera camera;
		Map map;
		MapRenderer mapRenderer;
		public string Mapname { get; set; }
		internal static float maxdepth;
		World world;
		List<IMovableObject> MovableObjects;
		Player player;
		Timer timer;
		Label label_timer;
		Label label_script;

		Vector2 mousepos;

		MessageBox msgbox;
		string[] popups;

		bool isWin;
		bool isDrawCollisionBox = true;
		bool isDrawInteractLink = true;

		internal static Texture2D spritesheet;
		internal static SpriteFont font;
		internal static Dictionary<SpriteSheetRectName, Rectangle> spriterects;
		private Vector2 mapRenderOffset = new Vector2(0, -60);

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
			gameStates = new Stack<GameState>();
			gameStates.Push(GameState.None);

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
			mapRenderer = new MapRenderer(map);
			mapRenderer.LoadContent(spritesheet, font, spriterects, maxdepth);

			isWin = false;

			world = new World(map.Width, map.Height);

			foreach (var collbox in map.Collsion) {
				world.Create(collbox.X, collbox.Y, collbox.Width, collbox.Height);
			}

			MovableObjects = new List<IMovableObject>();

			List<Object> MarkedForRemove = new List<Object>();
			spawnpoint = new Vector2();

			popups = map.Objects.Values.Where(x => x.TileType == SpriteSheetRectName.Popup).ToList().OrderBy(x => x.Name).Select(x => (string)x.MetaData).ToArray();

			foreach (var obj in map.Objects.Values) {

				if (obj.TileType == SpriteSheetRectName.Popup) {
					MarkedForRemove.Add(obj);
					continue;
				}

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
			var pbox = world.Create(spawnpoint.X, spawnpoint.Y, 0.5f, 0.5f).AddTags(CollisionTag.Player);
			Object pobj = new Object() {
				Name = "player",
				CollsionBox = pbox,
				TileType = SpriteSheetRectName.None,
				CollisionTag = CollisionTag.Player
			};
			pobj.CollsionBox.Data = pobj;
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
				Origin = new Vector2(0, -10)
			};
			label_script = new Label("", new Point(10, 40), new Vector2(80, 200), font) {
				Origin = new Vector2(0, -10)
			};
			canvas.AddElement("msgbox", msgbox);
			canvas.AddElement("label_timer", label_timer);
			canvas.AddElement("label_script", label_script);
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		internal void Win() {
			isWin = true;
		}

		public override void Update(GameTime gameTime) {

			if (HelperMethod.IsKeyPress(Keys.Escape, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
				if (CurrentGameState != GameState.Pause) {
					msgbox.Show("Game Paused!", "Continue", "Exit", "Restart");
					timer.Stop();
					gameStates.Push(GameState.Pause);
				}
			}

			if (HelperMethod.IsKeyPress(Keys.R, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
				gameStates.Clear();
				gameStates.Push(GameState.None);
			}

			switch (CurrentGameState) {
				case GameState.None:
					CurrentGameState = GameState.Ready;
					InitGame();
					msgbox.Show("Ready?\n" + string.Join("\n", popups), "Go");
					break;

				case GameState.Ready:
					if (msgboxMiddleButtonPressed) {
						msgboxMiddleButtonPressed = false;
						CurrentGameState = GameState.Ingame;
						msgbox.Hide();
						timer.Start();
					}
					break;
				case GameState.Ingame:
					if (isWin) {
						CurrentGameState = GameState.Endgame;
						timer.Stop();
						msgbox.Show("You finished the level in:" + Environment.NewLine + timer.ElapsedTime + " s" + Environment.NewLine + "Restart?", "Next lvl", "Exit", "Restart");
						break;
					}

					DisplayObjectScript(player.WorldPos);

					timer.Update(gameTime);

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
						break;
					}
					if (msgboxLeftButtonPressed) {
						msgboxLeftButtonPressed = false;
						//goto main menu
						CurrentGameState = GameState.None;
						SCREEN_MANAGER.GoBack();
						break;
					}
					if (msgboxRightButtonPressed) {
						msgboxRightButtonPressed = false;
						//restart level
						CurrentGameState = GameState.None;
						break;
					}
					break;
				case GameState.Pause:
					if (msgboxMiddleButtonPressed) {
						msgboxMiddleButtonPressed = false;
						//continue
						msgbox.Hide();
						gameStates.Pop();
						timer.Continue();
						break;
					}
					if (msgboxLeftButtonPressed) {
						msgboxLeftButtonPressed = false;
						//goto main menu
						CurrentGameState = GameState.None;
						SCREEN_MANAGER.GoBack();
						break;
					}
					if (msgboxRightButtonPressed) {
						msgboxRightButtonPressed = false;
						//restart level
						CurrentGameState = GameState.None;
						break;
					}
					break;
				default:
					break;
			}

			camera.Centre = player.IsoPos;

			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		private void DisplayObjectScript(Vector2 pos) {
			var x = (int)(pos.X + 0.3f);
			var y = (int)(pos.Y + 0.3f);

			var obj = map.FindObject(kvp => kvp.Value.WorldPos.X == x && kvp.Value.WorldPos.Y == y);

			var script = new StringBuilder();
			script.AppendLine(string.Format("{0}:{1}", x, y));

			script.AppendLine("OnActivate:");
			if (obj != null && !string.IsNullOrEmpty(obj.OnActivate))
				script.Append(string.Join("\n", obj.OnActivate.Split(';')));
			script.AppendLine();
			script.AppendLine("OnDeactivate:");
			if (obj != null && !string.IsNullOrEmpty(obj.OnDeactivate))
				script.Append(string.Join("\n", obj.OnDeactivate.Split(';')));

			label_script.Text = script.ToString();
		}

		private Vector2 GetMousePos(MouseState currentMouseState) {
			var p1 = currentMouseState.Position.ToVector2() - mapRenderOffset;
			p1 = camera.TranslateFromScreenToWorld(p1);
			p1 = p1.IsoToWorld();
			return p1;
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
			if (map is null) {
				return;
			}

			spriteBatch.BeginSpriteBatchWithCamera(camera, SpriteSortMode.BackToFront);

			mapRenderer.Draw(spriteBatch);

			var depthoffset = 0.7f - ((player.WorldPos.X + player.WorldPos.Y) / maxdepth) - 0.02f;
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

		IEnumerable<Vector2> InteractLink;

		private void DrawLine(Line line) {
			var pp = mapRenderOffset;
			if (line.Color == Color.Cyan) {
				//a bit up and left
				pp += new Vector2(1, 1);
				pp += new Vector2(1, 1);
			}
			else if (line.Color == Color.OrangeRed) {
				//a bit down and right
				pp -= new Vector2(1, 1);
				pp -= new Vector2(1, 1);
			}

			InteractLink = line.Points.Select(x => camera.TranslateFromWorldToScreen(x.WorldToIso()) + pp);

			foreach (var p in InteractLink) {
				InteractLink.Aggregate((v1, v2) => {
					drawBatch.DrawLine(new Pen(line.Color), v1, v2);
					return v2;
				});
			}
		}

		private void DrawBox(IBox box) {
			DrawRect(box.X, box.Y, box.Width, box.Height);
		}

		private void DrawRect(float x, float y, float width, float height) {
			var p0 = new Vector2(x, y).WorldToIso();
			var p1 = new Vector2(x + width, y).WorldToIso();
			var p2 = new Vector2(x + width, y + height).WorldToIso();
			var p3 = new Vector2(x, y + height).WorldToIso();

			p0 = camera.TranslateFromWorldToScreen(p0);
			p1 = camera.TranslateFromWorldToScreen(p1);
			p2 = camera.TranslateFromWorldToScreen(p2);
			p3 = camera.TranslateFromWorldToScreen(p3);

			p0 += mapRenderOffset;
			p1 += mapRenderOffset;
			p2 += mapRenderOffset;
			p3 += mapRenderOffset;

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
