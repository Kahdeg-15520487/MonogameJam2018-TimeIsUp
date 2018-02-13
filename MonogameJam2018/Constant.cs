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
		None = 0,     //allow player pass through

		DoorClosed = 1, //block player

		DoorOpened = 2, //allow player passing through

		Wall = 4,       //block player
		Block = 8,      //block player
		PushableBlock = 16,

		FloorSwitch = 32,//when the player is stepping on the switch, do something

		Lever = 64,     //when the player flip the lever, do something

		Elevator = 128,   //when the player is standing on the elevator,
						  //bring player's z to this stair'z. z is height

		Hole = 256,     //make the player fall

		Stair = 512,        //bring player's z to this stair's z. z is height

		Ladder = 1024,     //bring player's z to this ladder's next level's z. z is height

		Slab = 2048,       //bring player's z to this slab'z when enter. z is height

		EndPoint = 4096		//if the player touch this, the player win aka cleared the level
	}
}
