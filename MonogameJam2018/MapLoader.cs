﻿using Microsoft.Xna.Framework;
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
			var mappath = Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", mapname + ".tmx");
			var map = new TmxMap(mappath);

			var mapwidth = map.Width;
			var mapheight = map.Height;
			SpriteSheetRectName[,] f;
			SpriteSheetRectName[,] w;
			List<Object> o = new List<Object>();

			f = HelperMethod.Make2DArray(map.Layers["floor"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			w = HelperMethod.Make2DArray(map.Layers["wall"].Tiles.Select(x => (SpriteSheetRectName)(x.Gid - 1)).ToArray(), mapheight, mapwidth);
			var objects = map.ObjectGroups["object"].Objects;

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
			List<Object> interactableObj = new List<Object>();
			foreach (var oo in objects) {
				var objtiletype = (SpriteSheetRectName)(oo.Tile.Gid - 1);
				var objname = oo.Name;
				var x = (float)oo.X / map.TileHeight - 1;
				var y = (float)oo.Y / map.TileHeight - 1;
				if (objtiletype != SpriteSheetRectName.None) {
					var obj = new Object() {
						Name = objname,
						WorldPos = new Vector3(x, y, 0),
						SpriteOrigin = Constant.SPRITE_ORIGIN,
						TileType = objtiletype,
						Activate = BehaviourHelper.NoAction(),
						OnActivate = string.Empty,
						Deactivate = BehaviourHelper.NoAction(),
						OnDeactivate = string.Empty
					};
					if (objtiletype.GetCollisionTag() != CollisionTag.None) {
						obj.BoundingBox = GetCollsionBox(new Vector2(x, y), objtiletype);
						obj.CollisionTag = objtiletype.GetCollisionTag();
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

					if (isObjInteractable) {
						interactableObj.Add(obj);
						o.Add(obj);
					}
					else {
						o.Add(obj);
					}
				}
			}

			//map border
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, 0 - 0.3f, mapwidth, 0.3f));
			collision.Add(new Humper.Base.RectangleF(mapwidth - 0.3f, 0 - 0.3f, 0.3f, map.Height));
			collision.Add(new Humper.Base.RectangleF(0 - 0.3f, mapheight - 0.3f, mapwidth, 0.3f));

			var processedMap = new Map(mapwidth, mapheight, 3, f, w, o, collision.ToArray());

			foreach (var obj in interactableObj) {
				if (!string.IsNullOrEmpty(obj.OnActivate)) {
					processedMap.Objects[obj.Name].Activate = BehaviourParser(processedMap, obj.OnActivate);
				}

				if (!string.IsNullOrEmpty(obj.OnDeactivate)) {
					processedMap.Objects[obj.Name].Deactivate = BehaviourParser(processedMap, obj.OnDeactivate);
				}
			}
			processedMap.FindAllInteractLink();
			return processedMap;
		}

		private static Behaviour BehaviourParser(Map context, string behaviour) {
			var actions = behaviour.Split(';');
			if (actions.Length == 1) {
				//single action
				return BehaviourHelper.Parse(context, actions[0]);
			}
			else {
				//chained actionss
				return BehaviourHelper.Parse(context, actions);
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
							result.X = pos.X - 0.2f;
							result.Y = pos.Y - 0.4f;
							result.Width = 0.3f;
							result.Height = 0.2f;
							break;

						case Direction.left:
							result.X = pos.X - 0.4f;
							result.Y = pos.Y - 0.2f;
							result.Width = 0.2f;
							result.Height = 0.3f;
							break;
					}
					break;
			}

			return result;
		}
	}
}
