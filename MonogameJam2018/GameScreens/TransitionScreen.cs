using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using Utility.Drawing;
using Utility.Screens;
using Utility.UI;

namespace TimeIsUp.GameScreens {
	class TransitionScreen : FileBrowsingScreen {

		Texture2D minimap = null;
		private Texture2D spritesheet;
		private SpriteFont font;
		private Dictionary<SpriteSheetRectName, Rectangle> spriterects;

		public TransitionScreen(GraphicsDevice device) : base(device) {
			Name = "TransitionScreen";
		}

		public override bool Init() {
			base.Init();

			InputBox inputBox = new InputBox("", new Point(400, 50), null, font, Color.Black, Color.WhiteSmoke);
			canvas.AddElement("inputBox", inputBox);

			//todo implement some kind of map preview
			#region minimap render
			//spritesheet = CONTENT_MANAGER.Sprites["spritesheet"];
			//font = CONTENT_MANAGER.Fonts["default"];
			//var temp = File.ReadAllText(@"Content/sprite/spritesheet.json");
			//spriterects = JsonConvert.DeserializeObject<List<KeyValuePair<string, Rectangle>>>(temp).ToDictionary(kvp => kvp.Key.ToEnum<SpriteSheetRectName>(), kvp => kvp.Value);

			//var bo = canvas.GetElement("button_open"); ;
			//var bts = canvas.GetElements().Where(x => x.GetType() == typeof(Button) && x != bo).Select(x => (Button)x).ToList();

			//foreach (var bt in bts) {
			//	bt.MouseClick += (o, e) => {
			//		Button btt = (Button)o;
			//		Map map = MapLoader.LoadMap(btt.Text);
			//		MapRenderer mapRenderer = new MapRenderer(map);
			//		var maxdepth = ((map.Width + 1) + (map.Height + 1) + (map.Depth + 1)) * 10;
			//		mapRenderer.LoadContent(spritesheet, font, spriterects, maxdepth);
			//		minimap = TextureRenderer.Render(mapRenderer.Draw, CONTENT_MANAGER.spriteBatch, new Vector2(800, 600), new Vector2(400, 0), Color.WhiteSmoke, 0.25f);
			//	};
			//}
			#endregion

			return true;
		}

		public override void Update(GameTime gameTime) {

			if (HelperMethod.IsKeyPress(Keys.Escape, CONTENT_MANAGER.CurrentInputState.keyboardState, CONTENT_MANAGER.LastInputState.keyboardState)) {
				CONTENT_MANAGER.gameInstance.Exit();
			}

			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		//public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
		//	base.Draw(spriteBatch, gameTime);

		//	spriteBatch.BeginSpriteBatch();

		//	if (minimap != null) {
		//		spriteBatch.Draw(minimap, new Vector2(300, 200), Color.White);
		//	}

		//	spriteBatch.EndSpriteBatch();
		//}
	}
}
