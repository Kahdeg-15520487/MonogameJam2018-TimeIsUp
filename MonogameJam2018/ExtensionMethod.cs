using Microsoft.Xna.Framework.Input;
using System;

namespace TimeIsUp {
	static class ExtensionMethod {
		public static T ToEnum<T>(this string value) {
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static Microsoft.Xna.Framework.Rectangle ToRectangle(this Humper.Base.RectangleF rectangleF) {
			return new Microsoft.Xna.Framework.Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
		}

		public static Humper.Base.Vector2 ToHumperVector2(this Microsoft.Xna.Framework.Vector2 vector2) {
			return new Humper.Base.Vector2(vector2.X, vector2.Y);
		}

		public static Microsoft.Xna.Framework.Vector2 ToMonogameVector2(this Humper.Base.Vector2 vector2) {
			return new Microsoft.Xna.Framework.Vector2(vector2.X, vector2.Y);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this Microsoft.Xna.Framework.Vector2 world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this (float X, float Y) world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this (int X, int Y) world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this Microsoft.Xna.Framework.Vector3 world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(u, v);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this (float X, float Y, float Z) world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(u, v);
		}

		public static Microsoft.Xna.Framework.Vector2 WorldToIso(this (int X, int Y, int Z) world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new Microsoft.Xna.Framework.Vector2(u, v);
		}

		public static Microsoft.Xna.Framework.Vector2 IsoToWorld(this Microsoft.Xna.Framework.Vector2 iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static Microsoft.Xna.Framework.Vector2 IsoToWorld(this (float X, float Y) iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static Microsoft.Xna.Framework.Vector2 IsoToWorld(this (int X, int Y) iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new Microsoft.Xna.Framework.Vector2(x, y);
		}

		public static double DistanceToOther(this Microsoft.Xna.Framework.Point p, Microsoft.Xna.Framework.Point other, bool isManhattan = false) {
			return isManhattan ? Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y) : Math.Sqrt((p.X - other.X) * (p.X - other.X) + (p.Y - other.Y) * (p.Y - other.Y));
		}

		public static double DistanceToOther(this Microsoft.Xna.Framework.Vector2 p, Microsoft.Xna.Framework.Vector2 other, bool isManhattan = false) {
			return isManhattan ? Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y) : Math.Sqrt((p.X - other.X) * (p.X - other.X) + (p.Y - other.Y) * (p.Y - other.Y));
		}

		public static float Clamp(this float value, float max, float min) {
			return value >= max ? max : value <= min ? min : value;
		}

		public static int Clamp(this int value, int max, int min) {
			return value >= max ? max : value <= min ? min : value;
		}

		public static Microsoft.Xna.Framework.Vector2 Normalizee(this Microsoft.Xna.Framework.Vector2 A) {
			float distance = (float)Math.Sqrt(A.X * A.X + A.Y * A.Y);
			return new Microsoft.Xna.Framework.Vector2(A.X / distance, A.Y / distance);
		}

		public static SpriteSheetRectName FlipSwitch(this SpriteSheetRectName swt) {
			switch (swt) {
				case SpriteSheetRectName.WallSwitchOff_E:
					return SpriteSheetRectName.WallSwitchOn_E;
				case SpriteSheetRectName.WallSwitchOff_S:
					return SpriteSheetRectName.WallSwitchOn_S;
				case SpriteSheetRectName.WallSwitchOff_W:
					return SpriteSheetRectName.WallSwitchOn_W;
				case SpriteSheetRectName.WallSwitchOff_N:
					return SpriteSheetRectName.WallSwitchOn_N;

				case SpriteSheetRectName.WallSwitchOn_E:
					return SpriteSheetRectName.WallSwitchOff_E;
				case SpriteSheetRectName.WallSwitchOn_S:
					return SpriteSheetRectName.WallSwitchOff_S;
				case SpriteSheetRectName.WallSwitchOn_W:
					return SpriteSheetRectName.WallSwitchOff_W;
				case SpriteSheetRectName.WallSwitchOn_N:
					return SpriteSheetRectName.WallSwitchOff_N;
			}
			return swt;
		}

		public static SpriteSheetRectName OpenDoor(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorClosed_E:
					return SpriteSheetRectName.WallDoorOpen_E;
				case SpriteSheetRectName.WallDoorClosed_S:
					return SpriteSheetRectName.WallDoorOpen_S;
				case SpriteSheetRectName.WallDoorClosed_W:
					return SpriteSheetRectName.WallDoorOpen_W;
				case SpriteSheetRectName.WallDoorClosed_N:
					return SpriteSheetRectName.WallDoorOpen_N;
			}
			return door;
		}

		public static SpriteSheetRectName CloseDoor(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorOpen_E:
					return SpriteSheetRectName.WallDoorClosed_E;
				case SpriteSheetRectName.WallDoorOpen_S:
					return SpriteSheetRectName.WallDoorClosed_S;
				case SpriteSheetRectName.WallDoorOpen_W:
					return SpriteSheetRectName.WallDoorClosed_W;
				case SpriteSheetRectName.WallDoorOpen_N:
					return SpriteSheetRectName.WallDoorClosed_N;
			}
			return door;
		}

		public static CollisionTag GetCollisionTag(this SpriteSheetRectName spriteSheetRectName) {
			CollisionTag result = CollisionTag.Wall;
			switch (spriteSheetRectName) {
				case SpriteSheetRectName.None:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.BlockHuge_E:
				case SpriteSheetRectName.BlockHuge_N:
				case SpriteSheetRectName.BlockHuge_S:
				case SpriteSheetRectName.BlockHuge_W:
				case SpriteSheetRectName.BlockHugeCornerInner_E:
				case SpriteSheetRectName.BlockHugeCornerInner_N:
				case SpriteSheetRectName.BlockHugeCornerInner_S:
				case SpriteSheetRectName.BlockHugeCornerInner_W:
				case SpriteSheetRectName.BlockHugeCornerOuter_E:
				case SpriteSheetRectName.BlockHugeCornerOuter_N:
				case SpriteSheetRectName.BlockHugeCornerOuter_S:
				case SpriteSheetRectName.BlockHugeCornerOuter_W:
				case SpriteSheetRectName.BlockHugeSlant_E:
				case SpriteSheetRectName.BlockHugeSlant_N:
				case SpriteSheetRectName.BlockHugeSlant_S:
				case SpriteSheetRectName.BlockHugeSlant_W:

				case SpriteSheetRectName.BlockLarge_E:
				case SpriteSheetRectName.BlockLarge_N:
				case SpriteSheetRectName.BlockLarge_S:
				case SpriteSheetRectName.BlockLarge_W:

				case SpriteSheetRectName.BlockLargeSlant_E:
				case SpriteSheetRectName.BlockLargeSlant_N:
				case SpriteSheetRectName.BlockLargeSlant_S:
				case SpriteSheetRectName.BlockLargeSlant_W:
					result = CollisionTag.Block;
					break;

				case SpriteSheetRectName.BlockSmall_E:
				case SpriteSheetRectName.BlockSmall_N:
				case SpriteSheetRectName.BlockSmall_S:
				case SpriteSheetRectName.BlockSmall_W:
					result = CollisionTag.PushableBlock;
					break;

				case SpriteSheetRectName.Button_E:
				case SpriteSheetRectName.Button_N:
				case SpriteSheetRectName.Button_S:
				case SpriteSheetRectName.Button_W:
					result = CollisionTag.FloorSwitch;
					break;

				case SpriteSheetRectName.ButtonPressed_E:
				case SpriteSheetRectName.ButtonPressed_N:
				case SpriteSheetRectName.ButtonPressed_S:
				case SpriteSheetRectName.ButtonPressed_W:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.Column_E:
				case SpriteSheetRectName.Column_N:
				case SpriteSheetRectName.Column_S:
				case SpriteSheetRectName.Column_W:
				case SpriteSheetRectName.ColumnBlocks_E:
				case SpriteSheetRectName.ColumnBlocks_N:
				case SpriteSheetRectName.ColumnBlocks_S:
				case SpriteSheetRectName.ColumnBlocks_W:
				case SpriteSheetRectName.ColumnCorner_E:
				case SpriteSheetRectName.ColumnCorner_N:
				case SpriteSheetRectName.ColumnCorner_S:
				case SpriteSheetRectName.ColumnCorner_W:
					result = CollisionTag.Wall;
					break;

				case SpriteSheetRectName.Floor_E:
				case SpriteSheetRectName.Floor_N:
				case SpriteSheetRectName.Floor_S:
				case SpriteSheetRectName.Floor_W:

				case SpriteSheetRectName.FloorGrass_E:
				case SpriteSheetRectName.FloorGrass_N:
				case SpriteSheetRectName.FloorGrass_S:
				case SpriteSheetRectName.FloorGrass_W:

				case SpriteSheetRectName.FloorGrassRound_E:
				case SpriteSheetRectName.FloorGrassRound_N:
				case SpriteSheetRectName.FloorGrassRound_S:
				case SpriteSheetRectName.FloorGrassRound_W:

				case SpriteSheetRectName.FloorHalf_E:
				case SpriteSheetRectName.FloorHalf_N:
				case SpriteSheetRectName.FloorHalf_S:
				case SpriteSheetRectName.FloorHalf_W:

				case SpriteSheetRectName.FloorQuarter_E:
				case SpriteSheetRectName.FloorQuarter_N:
				case SpriteSheetRectName.FloorQuarter_S:
				case SpriteSheetRectName.FloorQuarter_W:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.Ladder_E:
				case SpriteSheetRectName.Ladder_N:
				case SpriteSheetRectName.Ladder_S:
				case SpriteSheetRectName.Ladder_W:
					result = CollisionTag.Ladder;
					break;

				case SpriteSheetRectName.Slab_E:
				case SpriteSheetRectName.Slab_N:
				case SpriteSheetRectName.Slab_S:
				case SpriteSheetRectName.Slab_W:

				case SpriteSheetRectName.SlabHalf_E:
				case SpriteSheetRectName.SlabHalf_N:
				case SpriteSheetRectName.SlabHalf_S:
				case SpriteSheetRectName.SlabHalf_W:

				case SpriteSheetRectName.SlabQuarter_E:
				case SpriteSheetRectName.SlabQuarter_N:
				case SpriteSheetRectName.SlabQuarter_S:
				case SpriteSheetRectName.SlabQuarter_W:
					result = CollisionTag.Slab;
					break;

				case SpriteSheetRectName.StepsLarge_E:
				case SpriteSheetRectName.StepsLarge_N:
				case SpriteSheetRectName.StepsLarge_S:
				case SpriteSheetRectName.StepsLarge_W:


				case SpriteSheetRectName.StepsSmall_E:
				case SpriteSheetRectName.StepsSmall_N:
				case SpriteSheetRectName.StepsSmall_S:
				case SpriteSheetRectName.StepsSmall_W:


				case SpriteSheetRectName.StepsSmallCornerInner_E:
				case SpriteSheetRectName.StepsSmallCornerInner_N:
				case SpriteSheetRectName.StepsSmallCornerInner_S:
				case SpriteSheetRectName.StepsSmallCornerInner_W:

				case SpriteSheetRectName.StepsSmallCornerOuter_E:
				case SpriteSheetRectName.StepsSmallCornerOuter_N:
				case SpriteSheetRectName.StepsSmallCornerOuter_S:
				case SpriteSheetRectName.StepsSmallCornerOuter_W:


				case SpriteSheetRectName.StepsSmallSlabs_E:
				case SpriteSheetRectName.StepsSmallSlabs_N:
				case SpriteSheetRectName.StepsSmallSlabs_S:
				case SpriteSheetRectName.StepsSmallSlabs_W:


				case SpriteSheetRectName.StepsSmallSlabsCornerInner_E:
				case SpriteSheetRectName.StepsSmallSlabsCornerInner_N:
				case SpriteSheetRectName.StepsSmallSlabsCornerInner_S:
				case SpriteSheetRectName.StepsSmallSlabsCornerInner_W:

				case SpriteSheetRectName.StepsSmallSlabsCornerOuter_E:
				case SpriteSheetRectName.StepsSmallSlabsCornerOuter_N:
				case SpriteSheetRectName.StepsSmallSlabsCornerOuter_S:
				case SpriteSheetRectName.StepsSmallSlabsCornerOuter_W:
					result = CollisionTag.Stair;
					break;

				case SpriteSheetRectName.WallCorner_E:
				case SpriteSheetRectName.WallCorner_N:
				case SpriteSheetRectName.WallCorner_S:
				case SpriteSheetRectName.WallCorner_W:
					result = CollisionTag.Wall;
					break;

				case SpriteSheetRectName.WallDoorClosed_E:
				case SpriteSheetRectName.WallDoorClosed_N:
				case SpriteSheetRectName.WallDoorClosed_S:
				case SpriteSheetRectName.WallDoorClosed_W:
					result = CollisionTag.DoorClosed;
					break;

				case SpriteSheetRectName.WallDoorOpen_E:
				case SpriteSheetRectName.WallDoorOpen_N:
				case SpriteSheetRectName.WallDoorOpen_S:
				case SpriteSheetRectName.WallDoorOpen_W:
					result = CollisionTag.DoorOpened;
					break;

				case SpriteSheetRectName.WallDoorway_E:
				case SpriteSheetRectName.WallDoorway_N:
				case SpriteSheetRectName.WallDoorway_S:
				case SpriteSheetRectName.WallDoorway_W:

				case SpriteSheetRectName.WallDoorwayLeft_E:
				case SpriteSheetRectName.WallDoorwayLeft_N:
				case SpriteSheetRectName.WallDoorwayLeft_S:
				case SpriteSheetRectName.WallDoorwayLeft_W:

				case SpriteSheetRectName.WallDoorwayMiddle_E:
				case SpriteSheetRectName.WallDoorwayMiddle_N:
				case SpriteSheetRectName.WallDoorwayMiddle_S:
				case SpriteSheetRectName.WallDoorwayMiddle_W:

				case SpriteSheetRectName.WallDoorwayRight_E:
				case SpriteSheetRectName.WallDoorwayRight_N:
				case SpriteSheetRectName.WallDoorwayRight_S:
				case SpriteSheetRectName.WallDoorwayRight_W:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.WallHalfCastle_E:
				case SpriteSheetRectName.WallHalfCastle_N:
				case SpriteSheetRectName.WallHalfCastle_S:
				case SpriteSheetRectName.WallHalfCastle_W:

				case SpriteSheetRectName.WallHalfCorner_E:
				case SpriteSheetRectName.WallHalfCorner_N:
				case SpriteSheetRectName.WallHalfCorner_S:
				case SpriteSheetRectName.WallHalfCorner_W:

				case SpriteSheetRectName.WallHalfStraight_E:
				case SpriteSheetRectName.WallHalfStraight_N:
				case SpriteSheetRectName.WallHalfStraight_S:
				case SpriteSheetRectName.WallHalfStraight_W:

				case SpriteSheetRectName.WallStraight_E:
				case SpriteSheetRectName.WallStraight_N:
				case SpriteSheetRectName.WallStraight_S:
				case SpriteSheetRectName.WallStraight_W:

				case SpriteSheetRectName.WallWindow_E:
				case SpriteSheetRectName.WallWindow_N:
				case SpriteSheetRectName.WallWindow_S:
				case SpriteSheetRectName.WallWindow_W:

				case SpriteSheetRectName.WallWindowLeft_E:
				case SpriteSheetRectName.WallWindowLeft_N:
				case SpriteSheetRectName.WallWindowLeft_S:
				case SpriteSheetRectName.WallWindowLeft_W:

				case SpriteSheetRectName.WallWindowMiddle_E:
				case SpriteSheetRectName.WallWindowMiddle_N:
				case SpriteSheetRectName.WallWindowMiddle_S:
				case SpriteSheetRectName.WallWindowMiddle_W:

				case SpriteSheetRectName.WallWindowRight_E:
				case SpriteSheetRectName.WallWindowRight_N:
				case SpriteSheetRectName.WallWindowRight_S:
				case SpriteSheetRectName.WallWindowRight_W:
					result = CollisionTag.Wall;
					break;

				case SpriteSheetRectName.WallSwitchOff_E:
				case SpriteSheetRectName.WallSwitchOff_N:
				case SpriteSheetRectName.WallSwitchOff_S:
				case SpriteSheetRectName.WallSwitchOff_W:

				case SpriteSheetRectName.WallSwitchOn_E:
				case SpriteSheetRectName.WallSwitchOn_N:
				case SpriteSheetRectName.WallSwitchOn_S:
				case SpriteSheetRectName.WallSwitchOn_W:
					result = CollisionTag.Lever;
					break;
			}
			return result;
		}

		public static Direction GetSpriteDirection(this SpriteSheetRectName spriteSheetRectName) {
			var spritename = spriteSheetRectName.ToString();
			var lc = spritename.Substring(spritename.Length - 1);
			switch (lc) {
				case "E":
					return Direction.right;
				case "W":
					return Direction.left;
				case "N":
					return Direction.up;
				case "S":
					return Direction.down;
			}
			return Direction.none;
		}
	}

	static class HelperMethod {
		public static bool IsKeyPress(Keys k, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			return currentKeyboardState.IsKeyUp(k) && lastKeyboardState.IsKeyDown(k);
		}
		public static bool IsKeyHold(Keys k, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			return currentKeyboardState.IsKeyDown(k) && lastKeyboardState.IsKeyDown(k);
		}

		public static bool IsLeftMousePressed(MouseState currentMouseState, MouseState lastMouseState) {
			return currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed;
		}

		public static bool IsRightMousePressed(MouseState currentMouseState, MouseState lastMouseState) {
			return currentMouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed;
		}

		public static bool IsMouseScrollWheelUp(MouseState currentMouseState, MouseState lastMouseState) {
			return currentMouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
		}

		public static bool IsMouseScrollWheelDown(MouseState currentMouseState, MouseState lastMouseState) {
			return currentMouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
		}

		public static T[,] Make2DArray<T>(T[] input, int height, int width) {
			T[,] output = new T[height, width];
			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					output[i, j] = input[i * width + j];
				}
			}
			return output;
		}
	}
}
