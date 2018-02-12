using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using Utility;
using Utility.Storage;

namespace TimeIsUp {
	class MapLoader {
		public static Map LoadMap(string mapname) {
			var mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".txt");
			if (File.Exists(mappath)) {
				//load txt map
				var mapdata = CompressHelper.UnZip(File.ReadAllText(mappath));
				return JsonConvert.DeserializeObject<Map>(mapdata);
			}
			else {
				//process tmx map and spit out txt map
				mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".tmx");
				var map = new TmxMap(mappath);

				var mapwidth = map.Width;
				var mapheight = map.Height;
				SpriteSheetRectName[,] f = new SpriteSheetRectName[mapheight, mapwidth];
				SpriteSheetRectName[,] w = new SpriteSheetRectName[mapheight, mapwidth];
				List<Object> o = new List<Object>();

				f = HelperMethod.Make2DArray(map.Layers["floor"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
				w = HelperMethod.Make2DArray(map.Layers["wall"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
				//var objects = HelperMethod.Make2DArray(map.Layers["object"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
				var objects = map.ObjectGroups["object"].Objects;

				var collision = new List<Humper.Base.RectangleF>();

				for (int y = 0; y < mapheight; y++) {
					for (int x = 0; x < mapwidth; x++) {
						if (f[y, x] == SpriteSheetRectName.None) {
							collision.Add(new Humper.Base.RectangleF(x - 0.3f, y - 0.3f, 1, 1));
						}

						if (w[y, x].GetCollisionTag() != CollisionTag.None) {
							collision.Add(GetCollsionBox(new Vector2(x, y), w[y, x]));
						}
					}
				}
				List<(string, string, Object)> interactableObj = new List<(string, string, Object)>();
				foreach (var oo in objects) {
					var objtiletype = (SpriteSheetRectName)(oo.Tile.Gid - 1);
					var objname = oo.Name;
					var x = (float)oo.X / map.TileHeight - 1;
					var y = (float)oo.Y / map.TileHeight - 1;
					if (objtiletype != SpriteSheetRectName.None) {
						var obj = new Object() { Name = objname, Position = new Vector3(x, y, 0), Origin = Constant.SPRITE_ORIGIN, TileType = objtiletype };
						if (objtiletype.GetCollisionTag() != CollisionTag.None) {
							obj.BoundingBox = GetCollsionBox(new Vector2(x, y), objtiletype);
							obj.CollisionTag = objtiletype.GetCollisionTag();
						}
						if (oo.Properties.ContainsKey("Target") && oo.Properties.ContainsKey("Action")) {
							var target = oo.Properties["Target"];
							var action = oo.Properties["Action"];
							interactableObj.Add((target, action, obj));
						}
						else {
							o.Add(obj);
						}
					}
				}

				collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, mapwidth, 0.3f));
				collision.Add(new Humper.Base.RectangleF(mapwidth - 0.3f, 0 - 0.3f, 0.3f, map.Height));
				collision.Add(new Humper.Base.RectangleF(0 - 0.3f, mapheight - 0.3f, mapwidth, 0.3f));

				var processedMap = new Map(mapwidth, mapheight, 3, f, w, o, collision.ToArray());

				foreach (var io in interactableObj) {
					var obj = io.Item3;
					switch (io.Item2) {
						case "open":
							obj.Activate = Behaviour.OpenDoor(processedMap.FindObject(io.Item1));
							obj.Deactivate = Behaviour.CloseDoor(processedMap.FindObject(io.Item1));
							break;
						case "up":
						case "down":
							obj.Activate = Behaviour.NoAction();
							obj.Deactivate = Behaviour.NoAction();
							break;
					}
					processedMap.Objects.Add(obj);
				}

				//map border
				//collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, 0.3f, map.Height));
				//mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".txt");

				//File.WriteAllText(mappath, CompressHelper.Zip(JsonConvert.SerializeObject(processedMap)));

				return processedMap;
			}
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
					result.X = pos.X;
					result.Y = pos.Y;
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
					result.Y = pos.Y - 0.3f;
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
