using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace TimeIsUp {
	struct Annotation {
		public Vector2 WorldPos { get; set; }
		public string Content { get; set; }
		public Color Color { get; set; }
		public float Rotation { get; set; }

		public Annotation(Vector2 pos, string c, Color cl, float r) {
			WorldPos = pos;
			Content = c;
			Color = cl;
			Rotation = r;
		}
		public Annotation(float x, float y, string c, Color cl, float r) {
			WorldPos = new Vector2(x, y);
			Content = c;
			Color = cl;
			Rotation = r;
		}
	}

	class Map {
		public SpriteSheetRectName[,] Floors { get; private set; }
		public SpriteSheetRectName[,] Walls { get; private set; }
		public Humper.Base.RectangleF[] Collsion { get; private set; }
		public Dictionary<string, Object> Objects { get; private set; }
		public List<Annotation> Annotations { get; private set; }
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		public List<Line> InteractLink { get; set; }
		public string Metadata { get; set; }


		public Map(int width, int height, int depth, SpriteSheetRectName[,] f, SpriteSheetRectName[,] w, List<Object> o, Humper.Base.RectangleF[] collision, List<Annotation> a) {
			Width = width;
			Height = height;
			Depth = depth;
			Floors = f;
			Walls = w;
			Objects = o.ToDictionary(x => x.Name, x => x);
			Collsion = collision;
			Annotations = a;
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

		public void AddAnnotation(Vector2 pos, string c, Color cl, float r) {
			Annotations.Add(new Annotation(pos, c, cl, r));
		}

		public void RemoveAnnotations(Vector2 pos, string c, Color cl, float r) {
			Annotations.RemoveAll(a => a.WorldPos == pos && a.Content == c && a.Color == cl && a.Rotation == r);
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

			writer.WritePropertyName("Annotations");
			serializer.Serialize(writer, map.Annotations);

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			Map result;
			int width = 0, height = 0, depth = 0;

			SpriteSheetRectName[,] floors = null;
			SpriteSheetRectName[,] walls = null;
			List<Object> objects = new List<Object>();
			Humper.Base.RectangleF[] collisions = null;
			List<Annotation> annotations = new List<Annotation>();

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

					case "Annotations":
						annotations = serializer.Deserialize<List<Annotation>>(reader);
						break;
				}
			}

			result = new Map(width, height, depth, floors, walls, objects, collisions, annotations);
			return result;
		}
	}
}
