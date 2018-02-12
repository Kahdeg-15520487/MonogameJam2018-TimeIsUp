using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using Utility;

namespace TimeIsUp {
	class MapLoader {
		public static Map LoadMap(string mapname) {
			var mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname);

			var map = new TmxMap(mappath);

			var mapwidth = map.Width;
			var mapheight = map.Height;
			SpriteSheetRectName[][] f = new SpriteSheetRectName[mapheight][];
			SpriteSheetRectName[][] w = new SpriteSheetRectName[mapheight][];
			List<Object> o = new List<Object>();

			var floors = HelperMethod.Make2DArray(map.Layers["floor"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			var walls = HelperMethod.Make2DArray(map.Layers["wall"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			var objects = HelperMethod.Make2DArray(map.Layers["object"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);

			var collision = new List<Humper.Base.RectangleF>();

			for (int y = 0; y < mapheight; y++) {
				f[y] = new SpriteSheetRectName[mapwidth];
				w[y] = new SpriteSheetRectName[mapwidth];

				for (int x = 0; x < mapwidth; x++) {
					f[y][x] = floors[y, x];
					w[y][x] = walls[y, x];
					var temp = objects[y, x];
					if (temp != SpriteSheetRectName.None) {
						var obj = new Object() { Position = new Vector3(x, y, 0), Origin = Constant.SPRITE_ORIGIN, Name = temp };
						if (temp.GetCollisionTag() != CollisionTag.None) {
							obj.BoundingBox = GetCollsionBox(new Vector2(x, y), temp);
							obj.CollisionTag = temp.GetCollisionTag();
						}
						o.Add(obj);
					}

					if (f[y][x] == SpriteSheetRectName.None) {
						collision.Add(new Humper.Base.RectangleF(x - 0.3f, y - 0.3f, 1, 1));
					}

					if (w[y][x].GetCollisionTag() != CollisionTag.None) {
						collision.Add(GetCollsionBox(new Vector2(x, y), w[y][x]));
					}
				}
			}

			//map border
			//collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, mapwidth, 0.3f));
			collision.Add(new Humper.Base.RectangleF(mapwidth - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, mapheight - 0.3f, mapwidth, 0.3f));

			return new Map(mapwidth, mapheight, 3, f, w, o, collision.ToArray());
		}

		private static Humper.Base.RectangleF GetCollsionBox(Vector2 pos, SpriteSheetRectName spriteSheetRectName) {
			var result = new Humper.Base.RectangleF();
			CollisionTag collisionTag = spriteSheetRectName.GetCollisionTag();
			var dir = spriteSheetRectName.GetSpriteDirection();
			switch (collisionTag) {
				case CollisionTag.DoorClosed:
				case CollisionTag.DoorOpened:
				case CollisionTag.Wall:
					result.X = pos.X - 0.3f;
					result.Y = pos.Y - 0.3f;

					switch (dir) {
						case Direction.up:
							result.Width = 1;
							result.Height = 0.3f;
							break;

						case Direction.left:
							result.Width = 0.3f;
							result.Height = 1;
							break;
					}
					break;

				case CollisionTag.Block:
					result.X = pos.X - 0.3f;
					result.Y = pos.Y - 0.3f;
					result.Width = 1;
					result.Height = 1;
					break;

				case CollisionTag.PushableBlock:
					result.X = pos.X ;
					result.Y = pos.Y ;
					result.Width = 0.3f;
					result.Height = 0.3f;
					break;

				case CollisionTag.FloorSwitch:
					result.X = pos.X - 0.1f;
					result.Y = pos.Y - 0.1f;
					result.Width = 0.5f;
					result.Height = 0.5f;
					break;

				case CollisionTag.Lever:
					result.X = pos.X - 0.1f;
					result.Y = pos.Y - 0.1f;
					result.Width = 0.5f;
					result.Height = 0.5f;
					break;

				case CollisionTag.Elevator:
					break;
				case CollisionTag.Hole:
					break;
				case CollisionTag.Stair:
					break;
				case CollisionTag.Ladder:
					result.X = pos.X;
					result.Y = pos.Y-0.3f;
					result.Width = 0.3f;
					result.Height = 0.2f;
					break;
				case CollisionTag.Slab:
					break;
				case CollisionTag.None:
					break;
			}

			return result;
		}
	}
}
