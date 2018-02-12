using System;
using System.Collections.Generic;
using System.Linq;
using TiledSharp;

namespace TimeIsUp {
	class Map {
		public SpriteSheetRectName[][] Floors { get; private set; }
		public SpriteSheetRectName[][] Walls { get; private set; }
		public Humper.Base.RectangleF[] Collsion { get; private set; }
		public List<Object> Objects { get; private set; }
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		public Map(int width, int height, int depth, SpriteSheetRectName[][] f, SpriteSheetRectName[][] w, List<Object> o, Humper.Base.RectangleF[] collision) {
			Width = width;
			Height = height;
			Depth = depth;
			Floors = f;
			Walls = w;
			Objects = o;
			Collsion = collision;
		}

		public Object FindObject(SpriteSheetRectName obj) {
			try {
				return Objects.FirstOrDefault(x => x.Name == obj);
			}
			catch (Exception e) {
				return null;
			}
		}

		public Object FindObject(Func<Object, bool> predicate) {
			return Objects.FirstOrDefault(predicate);
		}
	}
}
