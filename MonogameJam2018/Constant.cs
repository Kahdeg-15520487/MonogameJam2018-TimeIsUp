using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeIsUp {
	static class Constant {
		public static readonly float SCALE = 1f / 2;
		public static readonly float TILE_WIDTH = 256f * SCALE;
		public static readonly float TILE_HEIGHT = 149f * SCALE;
		public static readonly float TILE_WIDTH_HALF = TILE_WIDTH / 2;
		public static readonly float TILE_HEIGHT_HALF = TILE_HEIGHT / 2;
		public static readonly Microsoft.Xna.Framework.Vector2 SPRITE_ORIGIN = new Microsoft.Xna.Framework.Vector2(128, 512);
	}
	enum Direction {
		none,
		up,
		down,
		right,
		left
	}
	enum CollisionTag {
		DoorClosed,
		DoorOpened,
		Wall,
		FloorSwitch,
		Elevator
	}
}
