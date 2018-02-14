using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeIsUp.GameScreens;

namespace TimeIsUp {
	class Map {
		public SpriteSheetRectName[,] Floors { get; private set; }
		public SpriteSheetRectName[,] Walls { get; private set; }
		public Humper.Base.RectangleF[] Collsion { get; private set; }
		public Dictionary<string, Object> Objects { get; private set; }
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		public List<Line> InteractLink { get; private set; }

		Texture2D spritesheet;
		SpriteFont font;
		Dictionary<SpriteSheetRectName, Rectangle> spriterects;
		float maxdepth;

		public Map(int width, int height, int depth, SpriteSheetRectName[,] f, SpriteSheetRectName[,] w, List<Object> o, Humper.Base.RectangleF[] collision) {
			Width = width;
			Height = height;
			Depth = depth;
			Floors = f;
			Walls = w;
			Objects = o.ToDictionary(x => x.Name, x => x);
			Collsion = collision;
		}

		public void FindAllInteractLink() {
			InteractLink = new List<Line>();
			//foreach (var obj in Objects) {
			//	if (!string.IsNullOrEmpty(obj.Target)) {
			//		var target = FindObject(obj.Target);
			//		InteractLink.Add(new Line(obj.WorldPos.ToVector2(), target.WorldPos.ToVector2()));
			//	}
			//}
		}

		public Object FindObject(SpriteSheetRectName obj) {
			return Objects.FirstOrDefault(x => x.Value.TileType == obj).Value;
		}

		public Object FindObject(string objname) {
			return Objects.ContainsKey(objname) ? Objects[objname] : null;
		}

		public Object FindObject(Func<KeyValuePair<string, Object>, bool> predicate) {
			return Objects.FirstOrDefault(predicate).Value;
		}

		public void LoadContent() {
			spritesheet = MainPlayScreen.spritesheet;
			spriterects = MainPlayScreen.spriterects;
			font = MainPlayScreen.font;
			maxdepth = MainPlayScreen.maxdepth;
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			Vector2 spriteOrigin = Constant.SPRITE_ORIGIN;
			for (int y = 0; y < Height; y++) {
				for (int x = 0; x < Width; x++) {
					//var dos = 0.7f - (((x * Constant.TILE_WIDTH_HALF) + (y * Constant.TILE_HEIGHT_HALF)) / maxdepth);
					var z = 0;
					var dos = 0.7f - ((x + y + z) / maxdepth);
					Vector2 IsoPos = (x, y, z).WorldToIso();
					if (Floors[y, x] != SpriteSheetRectName.None) {
						spriteBatch.Draw(spritesheet, IsoPos, spriterects[Floors[y, x]], Color.White, 0f, spriteOrigin, Constant.SCALE, SpriteEffects.None, 0.9f);
					}
					if (Walls[y, x] != SpriteSheetRectName.None) {
						spriteBatch.Draw(spritesheet, IsoPos, spriterects[Walls[y, x]], Color.White, 0f, spriteOrigin, Constant.SCALE, SpriteEffects.None, dos);
						//spriteBatch.DrawString(font, dos.ToString() + Environment.NewLine + new Vector2(x, y).ToString(), IsoPos, Color.Black);
					}
				}
			}
			foreach (var obj in Objects.Values) {
				//var dos = 0.7f - (((obj.Position.X * Constant.TILE_WIDTH_HALF) + (obj.Position.Y * Constant.TILE_HEIGHT_HALF)) / maxdepth);
				//var dos = 0.7f - (((obj.Position.X) + (obj.Position.Y)) / maxdepth);
				var dos = 0.7f - ((obj.WorldPos.X + obj.WorldPos.Y + obj.WorldPos.Z) / maxdepth) - 0.001f;
				Vector2 IsoPos = obj.WorldPos.WorldToIso();
				if (obj.TileType != SpriteSheetRectName.None) {
					spriteBatch.Draw(spritesheet, IsoPos, spriterects[obj.TileType], Color.White, 0f, obj.SpriteOrigin, Constant.SCALE, SpriteEffects.None, dos);
				}
				spriteBatch.DrawString(font, obj.WorldPos.ToString(), IsoPos, Color.Black);
			}
		}
	}

	internal class MapJsonConverter : JsonConverter {

		public override bool CanConvert(Type objectType) {
			return objectType == typeof(Map);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Map map = (Map)value;

			writer.WriteStartObject();
			writer.WritePropertyName("Width");
			serializer.Serialize(writer, map.Width);
			writer.WritePropertyName("Height");
			serializer.Serialize(writer, map.Height);
			writer.WritePropertyName("Depth");
			serializer.Serialize(writer, map.Depth);

			writer.WritePropertyName("Floor");
			writer.WriteStartArray();
			for (int y = 0; y < map.Height; y++) {
				for (int x = 0; x < map.Width; x++) {
					serializer.Serialize(writer, (int)map.Floors[y, x]);
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName("Wall");
			writer.WriteStartArray();
			for (int y = 0; y < map.Height; y++) {
				for (int x = 0; x < map.Width; x++) {
					serializer.Serialize(writer, (int)map.Walls[y, x]);
				}
			}
			writer.WriteEndArray();

			writer.WritePropertyName("Objects");
			serializer.Serialize(writer, map.Objects.Values.ToList());

			writer.WritePropertyName("CollisionCount");
			serializer.Serialize(writer, map.Collsion.Length);

			writer.WritePropertyName("Collisions");
			writer.WriteStartArray();
			for (int i = 0; i < map.Collsion.Length; i++) {
				serializer.Serialize(writer, map.Collsion[i]);
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			Map result;
			int width = 0, height = 0, depth = 0;

			SpriteSheetRectName[,] floors = null;
			SpriteSheetRectName[,] walls = null;
			List<Object> objects = new List<Object>();
			Humper.Base.RectangleF[] collisions = null;

			bool isWidth = false;
			bool isHeight = false;

			while (reader.Read()) {
				if (reader.TokenType != JsonToken.PropertyName) {
					break;
				}

				string propertyName = (string)reader.Value;
				if (!reader.Read()) {
					continue;
				}

				if (isWidth && isHeight && floors is null && walls is null) {
					floors = new SpriteSheetRectName[height, width];
					walls = new SpriteSheetRectName[height, width];
				}

				switch (propertyName) {
					case "Width":
						width = serializer.Deserialize<int>(reader);
						isWidth = true;
						break;
					case "Height":
						height = serializer.Deserialize<int>(reader);
						isHeight = true;
						break;
					case "Depth":
						depth = serializer.Deserialize<int>(reader);
						break;
					case "Floor":
						var t = HelperMethod.Make2DArray(serializer.Deserialize<int[]>(reader), height, width);
						for (int x = 0; x < width; x++) {
							for (int y = 0; y < height; y++) {
								floors[y, x] = (SpriteSheetRectName)t[y, x];
							}
						}
						break;
					case "Wall":
						var tt = HelperMethod.Make2DArray(serializer.Deserialize<int[]>(reader), height, width);
						for (int x = 0; x < width; x++) {
							for (int y = 0; y < height; y++) {
								walls[y, x] = (SpriteSheetRectName)tt[y, x];
							}
						}
						break;

					case "Objects":
						objects = serializer.Deserialize<List<Object>>(reader);
						break;

					case "Collisions":
						collisions = serializer.Deserialize<Humper.Base.RectangleF[]>(reader);
						break;
					default:
						break;
				}
			}

			result = new Map(width, height, depth, floors, walls, objects, collisions);
			return result;
		}
	}
}
