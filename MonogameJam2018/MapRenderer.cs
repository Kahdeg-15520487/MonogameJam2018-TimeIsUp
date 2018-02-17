using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimeIsUp.GameScreens;
using Utility;

namespace TimeIsUp {
	class MapRenderer {

		Texture2D spritesheet;
		SpriteFont font;
		Dictionary<SpriteSheetRectName, Rectangle> spriterects;
		float maxdepth;

		Map map;

		public MapRenderer(Map map) {
			this.map = map;
		}

		public void LoadContent(Texture2D spritesheet, SpriteFont font, Dictionary<SpriteSheetRectName, Rectangle> spriterects, float maxdepth) {
			this.spritesheet = spritesheet;
			this.spriterects = spriterects;
			this.font = font;
			this.maxdepth = maxdepth;
		}

		public void Draw(SpriteBatch spriteBatch) {
			Vector2 spriteOrigin = Constant.SPRITE_ORIGIN;
			for (int y = 0; y < map.Height; y++) {
				for (int x = 0; x < map.Width; x++) {
					//var dos = 0.7f - (((x * Constant.TILE_WIDTH_HALF) + (y * Constant.TILE_HEIGHT_HALF)) / maxdepth);
					var z = 0;
					var dos = 0.7f - ((x + y + z) / maxdepth);
					Vector2 IsoPos = (x, y, z).WorldToIso();
					if (map.Floors[y, x] != SpriteSheetRectName.None) {
						spriteBatch.Draw(spritesheet, IsoPos, spriterects[map.Floors[y, x]], Color.White, 0f, spriteOrigin, Constant.SCALE, SpriteEffects.None, 0.9f);
					}
					if (map.Walls[y, x] != SpriteSheetRectName.None) {
						spriteBatch.Draw(spritesheet, IsoPos, spriterects[map.Walls[y, x]], Color.White, 0f, spriteOrigin, Constant.SCALE, SpriteEffects.None, dos);
						//spriteBatch.DrawString(font, dos.ToString() + Environment.NewLine + new Vector2(x, y).ToString(), IsoPos, Color.Black);
					}
				}
			}
			foreach (var obj in map.Objects.Values) {
				//var dos = 0.7f - (((obj.Position.X * Constant.TILE_WIDTH_HALF) + (obj.Position.Y * Constant.TILE_HEIGHT_HALF)) / maxdepth);
				//var dos = 0.7f - (((obj.Position.X) + (obj.Position.Y)) / maxdepth);
				var dos = 0.7f - ((obj.WorldPos.X + obj.WorldPos.Y + obj.WorldPos.Z) / maxdepth) - 0.001f;
				Vector2 IsoPos = obj.WorldPos.WorldToIso();
				if (obj.TileType != SpriteSheetRectName.None) {
					spriteBatch.Draw(spritesheet, IsoPos, spriterects[obj.TileType], Color.White, 0f, obj.SpriteOrigin, Constant.SCALE, SpriteEffects.None, dos);
				}
				//spriteBatch.DrawString(font, obj.WorldPos.ToString(), IsoPos, Color.Black);
			}
			foreach (var annotation in map.Annotations) {
				Vector2 IsoPos = annotation.WorldPos.WorldToIso();
				spriteBatch.DrawString(CONTENT_MANAGER.Fonts["hack"], annotation.Content.ProcessAnnotation(), IsoPos, annotation.Color, annotation.Rotation, Vector2.Zero, 0.75f, SpriteEffects.None, 0.8f);
			}
		}
	}
}
