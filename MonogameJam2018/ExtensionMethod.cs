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

		public static Microsoft.Xna.Framework.Vector2 ToVector2(this Microsoft.Xna.Framework.Vector3 vector3) {
			return new Microsoft.Xna.Framework.Vector2(vector3.X, vector3.Y);
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

		public static bool IsLever(this SpriteSheetRectName lever) {
			switch (lever) {
				case SpriteSheetRectName.WallSwitchOff_N:
				case SpriteSheetRectName.WallSwitchOff_W:
				case SpriteSheetRectName.WallSwitchOn_N:
				case SpriteSheetRectName.WallSwitchOn_W:
					return true;
				default:
					return false;
			}
		}

		public static bool IsOn(this SpriteSheetRectName swt) {
			switch (swt) {
				case SpriteSheetRectName.WallSwitchOn_W:
				case SpriteSheetRectName.WallSwitchOn_N:

				case SpriteSheetRectName.WallLightOn_W:
				case SpriteSheetRectName.WallLightOn_N:
					return true;
				default:
					return false;
			}
		}

		public static bool IsOff(this SpriteSheetRectName swt) {
			switch (swt) {
				case SpriteSheetRectName.WallSwitchOff_W:
				case SpriteSheetRectName.WallSwitchOff_N:

				case SpriteSheetRectName.WallLightOff_W:
				case SpriteSheetRectName.WallLightOff_N:
					return true;
				default:
					return false;
			}
		}

		public static SpriteSheetRectName FlipSwitch(this SpriteSheetRectName swt) {
			switch (swt) {
				case SpriteSheetRectName.WallSwitchOff_W:
					return SpriteSheetRectName.WallSwitchOn_W;
				case SpriteSheetRectName.WallSwitchOff_N:
					return SpriteSheetRectName.WallSwitchOn_N;

				case SpriteSheetRectName.WallSwitchOn_W:
					return SpriteSheetRectName.WallSwitchOff_W;
				case SpriteSheetRectName.WallSwitchOn_N:
					return SpriteSheetRectName.WallSwitchOff_N;
			}
			return swt;
		}

		public static SpriteSheetRectName TurnLightOn(this SpriteSheetRectName light) {
			switch (light) {
				case SpriteSheetRectName.WallLightOff_N:
					return SpriteSheetRectName.WallLightOn_N;
				case SpriteSheetRectName.WallLightOff_W:
					return SpriteSheetRectName.WallLightOn_W;
				default:
					return light;
			}
		}

		public static SpriteSheetRectName TurnLightOff(this SpriteSheetRectName light) {
			switch (light) {
				case SpriteSheetRectName.WallLightOn_N:
					return SpriteSheetRectName.WallLightOff_N;
				case SpriteSheetRectName.WallLightOn_W:
					return SpriteSheetRectName.WallLightOff_W;
				default:
					return light;
			}
		}

		public static bool IsOpen(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorOpen_W:
				case SpriteSheetRectName.WallDoorOpen_N:
					return true;
				default:
					return false;
			}
		}

		public static bool IsClose(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorClosed_W:
				case SpriteSheetRectName.WallDoorClosed_N:
					return true;
				default:
					return false;
			}
		}

		public static SpriteSheetRectName OpenDoor(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorClosed_W:
					return SpriteSheetRectName.WallDoorOpen_W;
				case SpriteSheetRectName.WallDoorClosed_N:
					return SpriteSheetRectName.WallDoorOpen_N;
			}
			return door;
		}

		public static SpriteSheetRectName CloseDoor(this SpriteSheetRectName door) {
			switch (door) {
				case SpriteSheetRectName.WallDoorOpen_W:
					return SpriteSheetRectName.WallDoorClosed_W;
				case SpriteSheetRectName.WallDoorOpen_N:
					return SpriteSheetRectName.WallDoorClosed_N;
			}
			return door;
		}

		public static CollisionTag GetCollisionTag(this SpriteSheetRectName spriteSheetRectName) {
			var result = CollisionTag.None;
			switch (spriteSheetRectName) {
				case SpriteSheetRectName.None:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.BlockHuge_E:
				case SpriteSheetRectName.BlockLarge_E:
					result = CollisionTag.Block;
					break;

				case SpriteSheetRectName.BlockSmall_E:
					result = CollisionTag.PushableBlock;
					break;

				case SpriteSheetRectName.Button_E:
					result = CollisionTag.FloorSwitch;
					break;
				case SpriteSheetRectName.ButtonPressed_E:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.Floor_E:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.SlabQuarter_S:
					result = CollisionTag.EndPoint;
					break;

				case SpriteSheetRectName.WallDoorClosed_N:
				case SpriteSheetRectName.WallDoorClosed_W:
					result = CollisionTag.DoorClosed;
					break;

				case SpriteSheetRectName.WallDoorOpen_N:
				case SpriteSheetRectName.WallDoorOpen_W:
					result = CollisionTag.DoorOpened;
					break;

				case SpriteSheetRectName.WallDoorway_N:
					result = CollisionTag.None;
					break;

				case SpriteSheetRectName.WallStraight_N:
				case SpriteSheetRectName.WallStraight_W:
					result = CollisionTag.Wall;
					break;

				case SpriteSheetRectName.WallSwitchOff_N:
				case SpriteSheetRectName.WallSwitchOff_W:
				case SpriteSheetRectName.WallSwitchOn_N:
				case SpriteSheetRectName.WallSwitchOn_W:
					result = CollisionTag.Lever;
					break;

				case SpriteSheetRectName.WallWindow_N:
				case SpriteSheetRectName.WallWindow_W:
					result = CollisionTag.Wall;
					break;

				case SpriteSheetRectName.Portal_N:
				case SpriteSheetRectName.Portal_W:
					result = CollisionTag.Portal;
					break;
				default:
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

		public static T[] RemoveAt<T>(this T[] source, int index) {
			T[] dest = new T[source.Length - 1];
			if (index > 0)
				Array.Copy(source, 0, dest, 0, index);

			if (index < source.Length - 1)
				Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

			return dest;
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
