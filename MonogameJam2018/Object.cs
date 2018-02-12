using System;
using Humper;
using Microsoft.Xna.Framework;

namespace TimeIsUp {
	internal class Object {
		public Vector3 Position { get; set; }
		public Vector2 Origin { get; set; }
		public SpriteSheetRectName Name { get; set; }
		public Humper.Base.RectangleF BoundingBox { get; set; }
		public IBox CollsionBox { get; set; }
		public CollisionTag CollisionTag { get; set; }
		public Action Activate { get; set; }
		public Action Deactivate { get; set; }
	}
}
