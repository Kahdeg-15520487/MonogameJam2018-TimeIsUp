using System.Collections.Generic;
using Humper;
using Microsoft.Xna.Framework;

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
}
