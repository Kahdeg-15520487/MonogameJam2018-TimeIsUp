namespace TimeIsUp {
	static class Constant {
		public static readonly float SCALE = 1f / 2;
		public static readonly float TILE_WIDTH = 256f * SCALE;
		public static readonly float TILE_HEIGHT = 149f * SCALE;
		public static readonly float TILE_WIDTH_HALF = TILE_WIDTH / 2;
		public static readonly float TILE_HEIGHT_HALF = TILE_HEIGHT / 2;
		public static readonly Microsoft.Xna.Framework.Vector2 SPRITE_ORIGIN = new Microsoft.Xna.Framework.Vector2(128, 512);

		public static bool OnlyLoadTmxMap = false;

		public static int OBJECT_MEMORY_LIMIT = 100;

		public static string PLAYER_NAME = string.Empty;
		public static string CALCULATION_RESULT = "0";
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

		DoorClosed = 1 << 0, //block player

		DoorOpened = 1 << 1, //allow player passing through

		Wall = 1 << 2,       //block player
		Block = 1 << 3,      //block player
		PushableBlock = 1 << 4,

		FloorSwitch = 1 << 5,//when the player is stepping on the switch, do something

		Lever = 1 << 6,     //when the player flip the lever, do something

		Portal = 1 << 7,   //when the player is standing on the elevator,
						   //bring player's z to this stair'z. z is height

		Hole = 1 << 8,     //block the player 

		PistonRetracted = 1 << 9,        //piston's collision when retracted

		PistonExtended = 1 << 10,        //piston's collision when extended

		Slab = 1 << 11,       //

		EndPoint = 1 << 12,     //if the player touch this, the player win aka cleared the level
		Player = 1 << 13,    //the player
		Trigger = 1 << 14	//a trigger that do something when the player cross it
	}
}
