using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeIsUp {
	class Map {
		public SpriteSheetRectName[,] Floors { get; private set; }
		public SpriteSheetRectName[,] Walls { get; private set; }
		public Humper.Base.RectangleF[] Collsion { get; private set; }
		public List<Object> Objects { get; private set; }
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		public Map(int width, int height, int depth, SpriteSheetRectName[,] f, SpriteSheetRectName[,] w, List<Object> o, Humper.Base.RectangleF[] collision) {
			Width = width;
			Height = height;
			Depth = depth;
			Floors = f;
			Walls = w;
			Objects = o;
			Collsion = collision;
		}

		public Object FindObject(SpriteSheetRectName obj) {
			try {
				return Objects.FirstOrDefault(x => x.Name == obj);
			}
			catch (Exception e) {
				return null;
			}
		}

		public Object FindObject(Func<Object, bool> predicate) {
			return Objects.FirstOrDefault(predicate);
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
			serializer.Serialize(writer, map.Objects);

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
