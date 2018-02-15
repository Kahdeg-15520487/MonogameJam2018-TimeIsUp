using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;

using TiledSharp;
using Newtonsoft.Json;

using Utility;
using Utility.Storage;

namespace TimeIsUp {
	class MapLoader {

		public static Map LoadTmxMap(string mapname) {
			//load the tiled map in
			var mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".tmx");

			var map = new TmxMap(mappath);

			//define map dimesion
			var mapwidth = map.Width;
			var mapheight = map.Height;

			//declare map data array
			SpriteSheetRectName[,] f;
			SpriteSheetRectName[,] w;
			List<Object> o = new List<Object>();
			List<Annotation> annotations = new List<Annotation>();

			//serialize tiled map data to ingame map data
			f = HelperMethod.Make2DArray(map.Layers["floor"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			w = HelperMethod.Make2DArray(map.Layers["wall"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			var objects = map.ObjectGroups["object"].Objects;

			//generate collision bounding box
			var collision = new List<Humper.Base.RectangleF>();
			int holecount = 0;
			for (int y = 0; y < mapheight; y++) {
				for (int x = 0; x < mapwidth; x++) {
					if (f[y, x] == SpriteSheetRectName.None) {
						var obj = new Object() {
							Name = "hole" + holecount,
							WorldPos = new Vector3(x, y, 0),
							SpriteOrigin = Constant.SPRITE_ORIGIN,
							TileType = SpriteSheetRectName.None,
							BoundingBox = new Humper.Base.RectangleF(x - 0.3f, y - 0.3f, 1, 1),
							CollisionTag = CollisionTag.Hole
						};
						o.Add(obj);
					}

					if (w[y, x].GetCollisionTag() != CollisionTag.None) {
						collision.Add(GetCollsionBox(new Vector2(x, y), w[y, x]));
					}
				}
			}

			//generate map border collision box
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, mapwidth, 0.3f));
			collision.Add(new Humper.Base.RectangleF(mapwidth - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, mapheight - 0.3f, mapwidth, 0.3f));

			//process objects
			List<Object> interactableObj = new List<Object>();
			foreach (var oo in objects) {
				var objname = oo.Name;

				if (oo.Tile is null) {
					if (objname.StartsWith("popup")) {
						//popup
						o.Add(new Object() {
							Name = objname,
							TileType = SpriteSheetRectName.Popup,
							MetaData = oo.Text.Value
						});
					}
					else {
						//map annotation
						var x = (float)oo.X / map.TileHeight - 1;
						var y = (float)oo.Y / map.TileHeight - 1;
						var color = new Color(oo.Text.Color.R, oo.Text.Color.G, oo.Text.Color.B);
						var rotation = HelperFunction.DegreeToRadian((float)oo.Rotation);
						annotations.Add(new Annotation(x, y, oo.Text.Value, color, rotation));
					}
				}
				else {
					//game object
					var objectiletype = (SpriteSheetRectName)(oo.Tile.Gid - 1);
					var x = (float)oo.X / map.TileHeight - 1;
					var y = (float)oo.Y / map.TileHeight - 1;
					if (objectiletype != SpriteSheetRectName.None) {
						var obj = new Object() {
							Name = objname,
							WorldPos = new Vector3(x, y, 0),
							SpriteOrigin = Constant.SPRITE_ORIGIN,
							TileType = objectiletype,
							Activate = BehaviourHelper.NoAction(),
							OnActivate = string.Empty,
							Deactivate = BehaviourHelper.NoAction(),
							OnDeactivate = string.Empty,
							Memory = new Stack<string>()
						};
						if (objectiletype.GetCollisionTag() != CollisionTag.None) {
							obj.BoundingBox = GetCollsionBox(new Vector2(x, y), objectiletype);
							obj.CollisionTag = objectiletype.GetCollisionTag();
						}

						bool isObjInteractable = false;
						if (oo.Properties.ContainsKey("OnActivate")) {
							obj.OnActivate = oo.Properties["OnActivate"];
							isObjInteractable = true;
						}
						if (oo.Properties.ContainsKey("OnDeactivate")) {
							obj.OnDeactivate = oo.Properties["OnDeactivate"];
							isObjInteractable = true;
						}

						Humper.Base.RectangleF portalBase = obj.BoundingBox;
						switch (obj.TileType) {
							case SpriteSheetRectName.Portal_N:
								portalBase = new Humper.Base.RectangleF(obj.WorldPos.X - 0.3f, obj.WorldPos.Y - 0.3f, 1f, 0.2f);
								collision.Add(portalBase);
								break;
							case SpriteSheetRectName.Portal_W:
								portalBase = new Humper.Base.RectangleF(obj.WorldPos.X - 0.3f, obj.WorldPos.Y - 0.3f, 0.2f, 1f);
								collision.Add(portalBase);
								break;

							case SpriteSheetRectName.Portal_S:
								portalBase = new Humper.Base.RectangleF(obj.WorldPos.X - 0.3f, obj.WorldPos.Y + 0.5f, 1f, 0.2f);
								collision.Add(portalBase);
								break;
							case SpriteSheetRectName.Portal_E:
								portalBase = new Humper.Base.RectangleF(obj.WorldPos.X + 0.5f, obj.WorldPos.Y - 0.3f, 0.2f, 1f);
								collision.Add(portalBase);
								break;
						}

						if (isObjInteractable) {
							interactableObj.Add(obj);
							o.Add(obj);
						}
						else {
							o.Add(obj);
						}
					}
				}
			}

			//build a static map without interactable object
			var processedMap = new Map(mapwidth, mapheight, 3, f, w, o, collision.ToArray(), annotations);

			//todo separate line that have the same coord
			//var separator = new Vector2(1, 1);
			//foreach (var link in intLink) {
			//	var sls = intLink.Where(x => x.Equals(link)).ToList();
			//	for (int i = 0; i < sls.Count; i++) {
			//		sls[i].Point1 += separator * i;
			//		sls[i].Point2 += separator * i;
			//	}
			//}

			//parse and generate behaviour for interactable object
			processedMap.InteractLink = ProcessInteractableObject(ref processedMap, interactableObj);

			mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".lvl");
			if (!File.Exists(mappath)) {
				var mapdata = CompressHelper.Zip(JsonConvert.SerializeObject(processedMap));
				File.WriteAllText(mappath, mapdata);

			}

			return processedMap;
		}

		public static Map LoadLvlMap(string mapname) {
			var mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".lvl");
			var map = JsonConvert.DeserializeObject<Map>(CompressHelper.UnZip(File.ReadAllText(mappath)));

			var interactableObj = from obj in map.Objects
								  where !string.IsNullOrEmpty(obj.Value.OnActivate) || !string.IsNullOrEmpty(obj.Value.OnDeactivate)
								  select obj.Value;

			map.InteractLink = ProcessInteractableObject(ref map, interactableObj);

			return map;
		}

		public static Map LoadMap(string mapname) {

			if (Constant.OnlyLoadTmxMap) {
				return LoadTmxMap(mapname);
			}

			if (File.Exists(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".lvl"))) {
				return LoadLvlMap(mapname);
			}
			else {
				return LoadTmxMap(mapname);
			}
		}

		private static List<Line> ProcessInteractableObject(ref Map map, IEnumerable<Object> interactableObj) {

			List<Line> intLink = new List<Line>();
			foreach (var obj in interactableObj) {
				if (!string.IsNullOrEmpty(obj.OnActivate)) {
					map.Objects[obj.Name].Activate = BehaviourParser(map, obj.Name, obj.OnActivate);
					foreach (var target in BehaviourHelper.GetAllTarget(map, obj.OnActivate.Split(';'))) {
						if (target is null) {
							continue;
						}
						var ps = HelperMethod.CutIntoAxisAlignVector2(obj.WorldPos.ToVector2(), target.WorldPos.ToVector2()).ToArray();
						intLink.Add(new Line(Color.Cyan, ps));
					}
				}

				if (!string.IsNullOrEmpty(obj.OnDeactivate)) {
					map.Objects[obj.Name].Deactivate = BehaviourParser(map, obj.Name, obj.OnDeactivate);
					foreach (var target in BehaviourHelper.GetAllTarget(map, obj.OnDeactivate.Split(';'))) {
						var ps = HelperMethod.CutIntoAxisAlignVector2(obj.WorldPos.ToVector2(), target.WorldPos.ToVector2()).ToArray();
						intLink.Add(new Line(Color.OrangeRed, ps));
					}
				}
			}

			return intLink;
		}

		private static Behaviour BehaviourParser(Map context, string objname, string behaviour) {
			var actions = behaviour.Split(';');
			if (actions.Length == 1) {
				//single action
				return BehaviourHelper.Parse(context, objname, actions[0]);
			}
			else {
				//chained actionss
				return BehaviourHelper.Parse(context, objname, actions);
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

					switch (dir) {
						case Direction.up:
							result.X = pos.X - 0.3f;
							result.Y = pos.Y - 0.3f;
							result.Width = 1;
							result.Height = 0.3f;
							break;

						case Direction.left:
							result.X = pos.X - 0.3f;
							result.Y = pos.Y - 0.3f;
							result.Width = 0.3f;
							result.Height = 1;
							break;

						case Direction.down:
							result.X = pos.X - 0.3f;
							result.Y = pos.Y + 0.4f;
							result.Width = 1;
							result.Height = 0.3f;
							break;

						case Direction.right:
							result.X = pos.X + 0.4f;
							result.Y = pos.Y - 0.3f;
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

				case CollisionTag.Slab:
					break;
				case CollisionTag.None:
					result.X = pos.X - 0.3f;
					result.Y = pos.Y - 0.3f;
					result.Width = 1;
					result.Height = 1;
					break;
				case CollisionTag.EndPoint:
					result.X = pos.X - 0.3f;
					result.Y = pos.Y - 0.3f;
					result.Width = 1;
					result.Height = 1;
					break;

				case CollisionTag.Portal:
					switch (dir) {
						case Direction.up:
							result.X = pos.X - 0.1f;
							result.Y = pos.Y - 0.15f;
							result.Width = 0.5f;
							result.Height = 0.1f;
							break;

						case Direction.left:
							result.X = pos.X - 0.15f;
							result.Y = pos.Y - 0.1f;
							result.Width = 0.1f;
							result.Height = 0.5f;
							break;

						case Direction.down:
							result.X = pos.X - 0.1f;
							result.Y = pos.Y + 0.45f;
							result.Width = 0.5f;
							result.Height = 0.1f;
							break;

						case Direction.right:
							result.X = pos.X + 0.45f;
							result.Y = pos.Y - 0.1f;
							result.Width = 0.1f;
							result.Height = 0.5f;
							break;
					}
					break;

				case CollisionTag.PistonRetracted:
					switch (dir) {
						case Direction.up:
							result.X = pos.X - 0.35f;
							result.Y = pos.Y - 0.1f;
							result.Width = 0.75f;
							result.Height = 0.2f;
							break;

						case Direction.left:
							result.X = pos.X - 0.1f;
							result.Y = pos.Y - 0.35f;
							result.Width = 0.2f;
							result.Height = 0.75f;
							break;

						case Direction.down:
							result.X = pos.X - 0.2f;
							result.Y = pos.Y + 0.1f;
							result.Width = 0.75f;
							result.Height = 0.2f;
							break;

						case Direction.right:
							result.X = pos.X + 0.1f;
							result.Y = pos.Y - 0.2f;
							result.Width = 0.2f;
							result.Height = 0.75f;
							break;
					}
					break;

				case CollisionTag.PistonExtended:
					switch (dir) {
						case Direction.up:
							result.X = pos.X - 0.45f;
							result.Y = pos.Y + 0.35f;
							result.Width = 0.75f;
							result.Height = 0.2f;
							break;

						case Direction.left:
							result.X = pos.X + 0.35f;
							result.Y = pos.Y - 0.45f;
							result.Width = 0.2f;
							result.Height = 0.75f;
							break;

						case Direction.down:
							result.X = pos.X - 0.25f;
							result.Y = pos.Y - 0.55f;
							result.Width = 0.75f;
							result.Height = 0.2f;
							break;

						case Direction.right:
							result.X = pos.X - 0.55f;
							result.Y = pos.Y - 0.25f;
							result.Width = 0.2f;
							result.Height = 0.75f;
							break;
					}
					break;
			}

			return result;
		}
	}
}
