using System;
using System.Collections.Generic;
using Humper;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace TimeIsUp {
	internal class Object {
		public string Name { get; set; }
		public Vector3 WorldPos { get; set; }
		public Vector2 SpriteOrigin { get; set; }
		public SpriteSheetRectName TileType { get; set; }
		public Humper.Base.RectangleF BoundingBox { get; set; }
		public IBox CollsionBox { get; set; }
		public CollisionTag CollisionTag { get; set; }
		public Behaviour Activate { get; set; }
		public Behaviour Deactivate { get; set; }
		public string OnActivate { get; set; }
		public string OnDeactivate { get; set; }
		public Stack<string> Memory { get; set; }
		public object MetaData { get; set; }
	}

	internal class ObjectJsonConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(Object);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Object obj = (Object)value;
			writer.WriteStartObject();
			writer.WritePropertyName("Name");
			serializer.Serialize(writer, obj.Name);
			writer.WritePropertyName("WorldPos");
			serializer.Serialize(writer, obj.WorldPos);
			writer.WritePropertyName("SpriteOrigin");
			serializer.Serialize(writer, obj.SpriteOrigin);
			writer.WritePropertyName("TiltType");
			serializer.Serialize(writer, (int)obj.TileType);
			writer.WritePropertyName("BoundingBox");
			serializer.Serialize(writer, obj.BoundingBox);
			writer.WritePropertyName("CollisionTag");
			serializer.Serialize(writer, (int)obj.CollisionTag);
			writer.WritePropertyName("OnActivate");
			serializer.Serialize(writer, obj.OnActivate);
			writer.WritePropertyName("OnDeactivate");
			serializer.Serialize(writer, obj.OnDeactivate);
			writer.WritePropertyName("Metadata");
			serializer.Serialize(writer, obj.MetaData);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			string objname = string.Empty;
			Vector3 worldpos = Vector3.Zero;
			Vector2 spriteorigin = Vector2.Zero;
			SpriteSheetRectName tiletype = SpriteSheetRectName.None;
			Humper.Base.RectangleF boundingbox = Humper.Base.RectangleF.Empty;
			CollisionTag collisionTag = CollisionTag.None;
			string onactivate = string.Empty;
			string ondeactivate = string.Empty;
			object metadata = null;

			while (reader.Read()) {
				if (reader.TokenType != JsonToken.PropertyName) {
					break;
				}

				string propertyName = (string)reader.Value;
				if (!reader.Read()) {
					continue;
				}

				switch (propertyName) {
					case "Name":
						objname = serializer.Deserialize<string>(reader);
						break;
					case "WorldPos":
						worldpos = serializer.Deserialize<Vector3>(reader);
						break;
					case "SpriteOrigin":
						spriteorigin = serializer.Deserialize<Vector2>(reader);
						break;
					case "TiltType":
						tiletype = (SpriteSheetRectName)serializer.Deserialize<int>(reader);
						break;
					case "BoundingBox":
						boundingbox = serializer.Deserialize<Humper.Base.RectangleF>(reader);
						break;
					case "CollisionTag":
						collisionTag = (CollisionTag)serializer.Deserialize<int>(reader);
						break;
					case "OnActivate":
						onactivate = serializer.Deserialize<string>(reader);
						break;
					case "OnDeactivate":
						ondeactivate = serializer.Deserialize<string>(reader);
						break;
					case "Metadata":
						metadata = serializer.Deserialize<object>(reader);
						break;
				}
			}

			return new Object() {
				Name = objname,
				WorldPos = worldpos,
				SpriteOrigin = spriteorigin,
				TileType = tiletype,
				BoundingBox = boundingbox,
				CollisionTag = collisionTag,
				OnActivate = onactivate,
				OnDeactivate = ondeactivate,
				MetaData = metadata
			};
		}
	}

	internal class RectangleFJsonConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(Humper.Base.RectangleF);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Humper.Base.RectangleF rect = (Humper.Base.RectangleF)value;
			writer.WriteStartObject();
			writer.WritePropertyName("X");
			serializer.Serialize(writer, rect.X);
			writer.WritePropertyName("Y");
			serializer.Serialize(writer, rect.Y);
			writer.WritePropertyName("W");
			serializer.Serialize(writer, rect.Width);
			writer.WritePropertyName("H");
			serializer.Serialize(writer, rect.Height);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			float x = 0, y = 0, w = 0, h = 0;

			while (reader.Read()) {
				if (reader.TokenType != JsonToken.PropertyName) {
					break;
				}

				string propertyName = (string)reader.Value;
				if (!reader.Read()) {
					continue;
				}

				switch (propertyName) {
					case "X":
						x = serializer.Deserialize<float>(reader);
						break;
					case "Y":
						y = serializer.Deserialize<float>(reader);
						break;
					case "W":
						w = serializer.Deserialize<float>(reader);
						break;
					case "H":
						h = serializer.Deserialize<float>(reader);
						break;
				}
			}

			return new Humper.Base.RectangleF(x, y, w, h);
		}
	}
}
