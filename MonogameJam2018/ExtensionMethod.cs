using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using VT2 = Microsoft.Xna.Framework.Vector2;
using rect = Microsoft.Xna.Framework.Rectangle;
using Utility;

namespace TimeIsUp {
	static class ExtensionMethod {

		public static string ProcessAnnotation(this string annotation) {
			var result = annotation;
			result = result.Replace("{P}", Constant.PLAYER_NAME);
			result = result.Replace("{C}", Constant.CALCULATION_RESULT);
			return result;
		}

		public static T ToEnum<T>(this string value) {
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static rect ToRectangle(this Humper.Base.RectangleF rectangleF) {
			return new rect((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
		}

		public static Humper.Base.Vector2 ToHumperVector2(this VT2 vector2) {
			return new Humper.Base.Vector2(vector2.X, vector2.Y);
		}

		public static VT2 ToMonogameVector2(this Humper.Base.Vector2 vector2) {
			return new VT2(vector2.X, vector2.Y);
		}

		public static VT2 ToVector2(this Microsoft.Xna.Framework.Vector3 vector3) {
			return new VT2(vector3.X, vector3.Y);
		}

		public static VT2 WorldToIso(this VT2 world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(x, y);
		}

		public static VT2 WorldToIso(this (float X, float Y) world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(x, y);
		}

		public static VT2 WorldToIso(this (int X, int Y) world) {
			var x = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var y = (world.X + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(x, y);
		}

		public static VT2 WorldToIso(this Microsoft.Xna.Framework.Vector3 world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(u, v);
		}

		public static VT2 WorldToIso(this (float X, float Y, float Z) world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(u, v);
		}

		public static VT2 WorldToIso(this (int X, int Y, int Z) world) {
			var u = (world.X - world.Y) * Constant.TILE_WIDTH_HALF;
			var v = (world.X + 2 * world.Z + world.Y) * Constant.TILE_HEIGHT_HALF;
			return new VT2(u, v);
		}

		public static VT2 IsoToWorld(this VT2 iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new VT2(x, y);
		}

		public static VT2 IsoToWorld(this (float X, float Y) iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new VT2(x, y);
		}

		public static VT2 IsoToWorld(this (int X, int Y) iso) {
			var x = ((iso.X / Constant.TILE_WIDTH_HALF) + (iso.Y / Constant.TILE_HEIGHT_HALF)) / 2;
			var y = ((iso.Y / Constant.TILE_WIDTH_HALF) - (iso.X / Constant.TILE_HEIGHT_HALF)) / 2;
			return new VT2(x, y);
		}

		public static double DistanceToOther(this Microsoft.Xna.Framework.Point p, Microsoft.Xna.Framework.Point other, bool isManhattan = false) {
			return isManhattan ? Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y) : Math.Sqrt((p.X - other.X) * (p.X - other.X) + (p.Y - other.Y) * (p.Y - other.Y));
		}

		public static double DistanceToOther(this VT2 p, VT2 other, bool isManhattan = false) {
			return isManhattan ? Math.Abs(p.X - other.X) + Math.Abs(p.Y - other.Y) : Math.Sqrt((p.X - other.X) * (p.X - other.X) + (p.Y - other.Y) * (p.Y - other.Y));
		}

		public static float Clamp(this float value, float max, float min) {
			return value >= max ? max : value <= min ? min : value;
		}

		public static int Clamp(this int value, int max, int min) {
			return value >= max ? max : value <= min ? min : value;
		}

		public static VT2 Normalizee(this VT2 A) {
			float distance = (float)Math.Sqrt(A.X * A.X + A.Y * A.Y);
			return new VT2(A.X / distance, A.Y / distance);
		}

		public static T[] RemoveAt<T>(this T[] source, int index) {
			T[] dest = new T[source.Length - 1];
			if (index > 0)
				Array.Copy(source, 0, dest, 0, index);

			if (index < source.Length - 1)
				Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

			return dest;
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) {
			if (source != null) {
				foreach (T obj in source) {
					return false;
				}
			}
			return true;
		}
	}

	static class SpriteRectNameExtensionMethod {

		public static bool IsExtended(this SpriteSheetRectName piston) {
			switch (piston) {
				case SpriteSheetRectName.PistolExtended_N:
				case SpriteSheetRectName.PistolExtended_W:
				case SpriteSheetRectName.PistolExtended_S:
				case SpriteSheetRectName.PistolExtended_E:
					return true;
				default:
					return false;
			}
		}

		public static bool IsRetracted(this SpriteSheetRectName piston) {
			switch (piston) {
				case SpriteSheetRectName.PistolRetracted_N:
				case SpriteSheetRectName.PistolRetracted_W:
				case SpriteSheetRectName.PistolRetracted_S:
				case SpriteSheetRectName.PistolRetracted_E:
					return true;
				default:
					return false;
			}
		}

		public static SpriteSheetRectName ExtendPiston(this SpriteSheetRectName piston) {
			switch (piston) {
				case SpriteSheetRectName.PistolRetracted_N:
					return SpriteSheetRectName.PistolExtended_N;
				case SpriteSheetRectName.PistolRetracted_W:
					return SpriteSheetRectName.PistolExtended_W;
				case SpriteSheetRectName.PistolRetracted_S:
					return SpriteSheetRectName.PistolExtended_S;
				case SpriteSheetRectName.PistolRetracted_E:
					return SpriteSheetRectName.PistolExtended_E;

				default:
					return piston;
			}
		}

		public static SpriteSheetRectName RetractPiston(this SpriteSheetRectName piston) {
			switch (piston) {
				case SpriteSheetRectName.PistolExtended_N:
					return SpriteSheetRectName.PistolRetracted_N;
				case SpriteSheetRectName.PistolExtended_W:
					return SpriteSheetRectName.PistolRetracted_W;
				case SpriteSheetRectName.PistolExtended_S:
					return SpriteSheetRectName.PistolRetracted_S;
				case SpriteSheetRectName.PistolExtended_E:
					return SpriteSheetRectName.PistolRetracted_E;
				default:
					return piston;
			}
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

		public static bool IsLight(this SpriteSheetRectName light) {
			switch (light) {
				case SpriteSheetRectName.WallLightOff_N:
				case SpriteSheetRectName.WallLightOff_W:
				case SpriteSheetRectName.WallLightOn_N:
				case SpriteSheetRectName.WallLightOn_W:
					return true;
				default:
					return false;
			}
		}

		public static bool IsPortal(this SpriteSheetRectName portal) {
			switch (portal) {
				case SpriteSheetRectName.PortalOff_E:
				case SpriteSheetRectName.PortalOff_W:
				case SpriteSheetRectName.PortalOff_N:
				case SpriteSheetRectName.PortalOff_S:
				case SpriteSheetRectName.Portal_E:
				case SpriteSheetRectName.Portal_W:
				case SpriteSheetRectName.Portal_N:
				case SpriteSheetRectName.Portal_S:
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

				case SpriteSheetRectName.Portal_E:
				case SpriteSheetRectName.Portal_W:
				case SpriteSheetRectName.Portal_N:
				case SpriteSheetRectName.Portal_S:

				case SpriteSheetRectName.ButtonPressed_E:
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

				case SpriteSheetRectName.PortalOff_E:
				case SpriteSheetRectName.PortalOff_W:
				case SpriteSheetRectName.PortalOff_N:
				case SpriteSheetRectName.PortalOff_S:

				case SpriteSheetRectName.Button_E:
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

		public static SpriteSheetRectName PressOnFloorSwitch(this SpriteSheetRectName floorswitch) {
			switch (floorswitch) {
				case SpriteSheetRectName.Button_E:
					return SpriteSheetRectName.ButtonPressed_E;
			}
			return floorswitch;
		}

		public static SpriteSheetRectName ReleaseFloorSwitch(this SpriteSheetRectName floorswitch) {
			switch (floorswitch) {
				case SpriteSheetRectName.ButtonPressed_E:
					return SpriteSheetRectName.Button_E;
			}
			return floorswitch;
		}

		public static bool IsFloorSwitch(this SpriteSheetRectName spriteSheetRectName) {
			switch (spriteSheetRectName) {
				case SpriteSheetRectName.ButtonPressed_E:
				case SpriteSheetRectName.Button_E:
					return true;
				default:
					return false;
			}
		}

		public static SpriteSheetRectName TurnOn(this SpriteSheetRectName thingy) {
			switch (thingy) {
				case SpriteSheetRectName.WallLightOff_N:
					return SpriteSheetRectName.WallLightOn_N;
				case SpriteSheetRectName.WallLightOff_W:
					return SpriteSheetRectName.WallLightOn_W;

				case SpriteSheetRectName.PortalOff_E:
					return SpriteSheetRectName.Portal_E;
				case SpriteSheetRectName.PortalOff_W:
					return SpriteSheetRectName.Portal_W;
				case SpriteSheetRectName.PortalOff_N:
					return SpriteSheetRectName.Portal_N;
				case SpriteSheetRectName.PortalOff_S:
					return SpriteSheetRectName.Portal_S;
				default:
					return thingy;
			}
		}

		public static SpriteSheetRectName TurnOff(this SpriteSheetRectName thingy) {
			switch (thingy) {
				case SpriteSheetRectName.WallLightOn_N:
					return SpriteSheetRectName.WallLightOff_N;
				case SpriteSheetRectName.WallLightOn_W:
					return SpriteSheetRectName.WallLightOff_W;

				case SpriteSheetRectName.Portal_E:
					return SpriteSheetRectName.PortalOff_E;
				case SpriteSheetRectName.Portal_W:
					return SpriteSheetRectName.PortalOff_W;
				case SpriteSheetRectName.Portal_N:
					return SpriteSheetRectName.PortalOff_N;
				case SpriteSheetRectName.Portal_S:
					return SpriteSheetRectName.PortalOff_S;
				default:
					return thingy;
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
				case SpriteSheetRectName.ButtonPressed_E:
					result = CollisionTag.FloorSwitch;
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
				case SpriteSheetRectName.WallStraight_S:
				case SpriteSheetRectName.WallStraight_E:
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
				case SpriteSheetRectName.Portal_S:
				case SpriteSheetRectName.Portal_E:
				case SpriteSheetRectName.PortalOff_E:
				case SpriteSheetRectName.PortalOff_W:
				case SpriteSheetRectName.PortalOff_N:
				case SpriteSheetRectName.PortalOff_S:
					result = CollisionTag.Portal;
					break;

				case SpriteSheetRectName.PistolExtended_N:
				case SpriteSheetRectName.PistolExtended_W:
				case SpriteSheetRectName.PistolExtended_S:
				case SpriteSheetRectName.PistolExtended_E:
					result = CollisionTag.PistonExtended;
					break;

				case SpriteSheetRectName.PistolRetracted_N:
				case SpriteSheetRectName.PistolRetracted_W:
				case SpriteSheetRectName.PistolRetracted_S:
				case SpriteSheetRectName.PistolRetracted_E:
					result = CollisionTag.PistonRetracted;
					break;

				case SpriteSheetRectName.Trigger:
					result = CollisionTag.Trigger;
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

		public static List<VT2> CutIntoAxisAlignVector2(VT2 head, VT2 tail) {
			var center = new VT2((head.X + tail.X) / 2, (head.Y + tail.Y) / 2);

			return new List<VT2>() {
				head,
				new VT2(head.X,center.Y),
				center,
				new VT2(center.X,tail.Y),
				tail
			};
		}

		public static byte[] GetMD5CheckSum(string filename) {
			using (var md5 = System.Security.Cryptography.MD5.Create()) {
				using (var stream = System.IO.File.OpenRead(filename)) {
					return md5.ComputeHash(stream);
				}
			}
		}
	}
}
