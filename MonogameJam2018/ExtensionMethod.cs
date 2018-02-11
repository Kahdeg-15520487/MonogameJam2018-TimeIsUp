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
	}

	static class HelperMethod {
		public static bool IsKeyPress(Microsoft.Xna.Framework.Input.KeyboardState currentKeyboardState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyboardState, Microsoft.Xna.Framework.Input.Keys k) {
			return currentKeyboardState.IsKeyUp(k) && lastKeyboardState.IsKeyDown(k);
		}
		public static bool IsKeyHold(Microsoft.Xna.Framework.Input.KeyboardState currentKeyboardState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyboardState, Microsoft.Xna.Framework.Input.Keys k) {
			return currentKeyboardState.IsKeyDown(k) && lastKeyboardState.IsKeyDown(k);
		}

		public static bool IsLeftMousePressed(Microsoft.Xna.Framework.Input.MouseState currentMouseState, Microsoft.Xna.Framework.Input.MouseState lastMouseState) {
			return currentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && lastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
		}

		public static bool IsRightMousePressed(Microsoft.Xna.Framework.Input.MouseState currentMouseState, Microsoft.Xna.Framework.Input.MouseState lastMouseState) {
			return currentMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && lastMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
		}

		public static bool IsMouseScrollWheelUp(Microsoft.Xna.Framework.Input.MouseState currentMouseState, Microsoft.Xna.Framework.Input.MouseState lastMouseState) {
			return currentMouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
		}

		public static bool IsMouseScrollWheelDown(Microsoft.Xna.Framework.Input.MouseState currentMouseState, Microsoft.Xna.Framework.Input.MouseState lastMouseState) {
			return currentMouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
		}
	}
}
