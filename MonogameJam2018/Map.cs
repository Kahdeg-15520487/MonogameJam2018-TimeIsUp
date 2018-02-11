﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeIsUp {
	class Map {
		public SpriteSheetRectName[][] Floors { get; private set; }
		public SpriteSheetRectName[][] Walls { get; private set; }
		public List<Object> Objects { get; private set; }
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		public Map(int width, int height, int depth, SpriteSheetRectName[][] f, SpriteSheetRectName[][] w, List<Object> o) {
			Width = width;
			Height = height;
			Depth = depth;
			Floors = f;
			Walls = w;
			Objects = o;
		}

		internal Object FindObject(SpriteSheetRectName obj) {
			try {
				return Objects.First(x => x.Name == obj);
			}
			catch (Exception e) {
				return null;
			}
		}
	}
}