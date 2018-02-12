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
		DoorClosed = 0, //block player

		DoorOpened = 1, //allow player passing through

		Wall = 2,       //block player
		Block = 4,      //block player
		PushableBlock = 8,

		FloorSwitch = 16,//when the player is stepping on the switch, do something

		Lever = 32,     //when the player flip the lever, do something

		Elevator = 64,   //when the player is standing on the elevator,
						 //bring player's z to this stair'z. z is height

		Hole = 128,     //make the player fall

		Stair = 256,        //bring player's z to this stair's z. z is height

		Ladder = 512,     //bring player's z to this ladder's next level's z. z is height

		Slab = 1024,       //bring player's z to this slab'z when enter. z is height

		None = 2048     //allow player pass through
	}
}
