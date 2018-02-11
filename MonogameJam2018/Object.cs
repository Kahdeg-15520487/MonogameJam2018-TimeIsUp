﻿using Microsoft.Xna.Framework;

namespace TimeIsUp {
	internal class Object {
		public Vector3 Position { get; set; }
		public Vector2 Origin { get; set; }
		public SpriteSheetRectName Name { get; set; }
		public Rectangle BoundingBox { get; set; }
	}
}