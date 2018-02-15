using Humper.Responses;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeIsUp {
	internal delegate void Behaviour(Object activator, Object self);
	static class BehaviourHelper {
		internal static List<Object> GetAllTarget(Map context, string[] actions) {
			return actions.Select(x => GetTarget(context, x)).ToList();
		}

		internal static Object GetTarget(Map context, string action) {
			return action.Split().Length == 1 ? null : context.FindObject(action.Split()[1]);
		}

		internal static Behaviour Parse(Map context, string self, params string[] actions) {
			List<Behaviour> parsedActions = new List<Behaviour>() { ClearMemory() };

			Object s = context.FindObject(self);
			bool ConditionalFlag = false;

			foreach (var action in actions) {
				var temp = action.Split();
				var verb = temp[0];

				var argumentCount = temp.Length - 1;
				Behaviour a = null;

				switch (argumentCount) {
					case 0:
						switch (verb) {
							case "iftrue":
								ConditionalFlag = true;
								break;
							case "negate":
								a = Negate();
								break;
							case "and":
								a = And();
								break;
							case "or":
								a = Or();
								break;

							case "clearmem":
								a = ClearMemory();
								break;
						}
						break;

					case 1:

						var targetName = temp[1];
						Object target = context.FindObject(targetName);
						if (target is null) {
							//todo handle object not found
						}
						//todo check object type

						switch (verb) {
							case "open":
								a = (OpenDoor(target));
								break;
							case "close":
								a = (CloseDoor(target));
								break;

							case "turnon":
								a = (TurnLightOn(target));
								break;
							case "turnoff":
								a = (TurnLightOff(target));
								break;

							case "getlight":
								a = (GetLightState(target));
								break;

							case "teleportto":
								a = (TeleportTo(target));
								break;

							case "extend":
								a = (ExtendPiston(target));
								break;
							case "retract":
								a = (RetractPiston(target));
								break;

							case "annotate":
								a = AnnotateMap(context, temp[1]);
								break;
						}
						break;
				}

				if (ConditionalFlag && a!= null) {
					ConditionalFlag = false;
					a = ConditionalAction(a);
				}

				if (a != null) {
					parsedActions.Add(a);
				}
			}

			return RunActions(parsedActions.ToArray());
		}

		private static Behaviour AnnotateMap(Map map, string content) {
			return (activator, self) => {

			};
		}

		internal static Behaviour OpenDoor(Object door) {
			return (activator, self) => {
				if (door.TileType.IsOpen()) {
					return;
				}
				door.TileType = door.TileType.OpenDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorClosed);
				door.CollsionBox.AddTags(CollisionTag.DoorOpened);
			};
		}

		internal static Behaviour CloseDoor(Object door) {
			return (activator, self) => {
				if (door.TileType.IsClose()) {
					return;
				}
				door.TileType = door.TileType.CloseDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorOpened);
				door.CollsionBox.AddTags(CollisionTag.DoorClosed);
			};
		}

		internal static Behaviour RunActions(params Behaviour[] actions) {
			return (activator, self) => {
				foreach (var action in actions) {
					action(activator, self);
				}
			};
		}

		internal static Behaviour RunAction(Behaviour action) {
			return (activator, self) => {
				action(activator, self);
			};
		}

		internal static Behaviour NoAction() {
			return (activator, self) => { };
		}

		internal static Behaviour ExtendPiston(Object piston) {
			return (activator, self) => {
				if (piston.TileType.IsExtended()) {
					return;
				}
				piston.TileType = piston.TileType.ExtendPiston();

				var destX = piston.WorldPos.X;
				var destY = piston.WorldPos.Y;

				var dir = piston.TileType.GetSpriteDirection();
				switch (dir) {
					case Direction.up:
						destX += -0.45f;
						destY += 0.35f;
						break;
					case Direction.left:
						destX += 0.35f;
						destY += -0.45f;
						break;

					case Direction.down:
						destX += -0.25f;
						destY += -0.55f;
						break;
					case Direction.right:
						destX += -0.55f;
						destY += -0.25f;
						break;
				}

				var Velocity = new Vector2(destX - piston.WorldPos.X, destY - piston.WorldPos.Y);

				var move = piston.CollsionBox.Move(destX, destY, c => {
					if (c.Other.HasTag(CollisionTag.Player) || c.Other.HasTag(CollisionTag.PushableBlock)) {
						return CollisionResponses.Slide;
					}
					return CollisionResponses.Cross;
				});

				var player = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.Player));
				if (player != null) {
					Player obj = (Player)player.Box.Data;
					var n = player.Normal;
					obj.Velocity = new Vector2(n.X * n.X, n.Y * n.Y) * Velocity;
				}

				var blocks = move.Hits.Where(c => c.Box.HasTag(CollisionTag.PushableBlock));
				if (!blocks.IsNullOrEmpty()) {
					foreach (var block in blocks) {
						Block obj = (Block)block.Box.Data;
						var n = block.Normal;
						obj.Velocity = new Vector2(n.X * n.X, n.Y * n.Y) * Velocity;
					}
				}
			};
		}

		internal static Behaviour RetractPiston(Object piston) {
			return (activator, self) => {
				if (piston.TileType.IsRetracted()) {
					return;
				}
				piston.TileType = piston.TileType.RetractPiston();

				var destX = piston.WorldPos.X;
				var destY = piston.WorldPos.Y;

				var dir = piston.TileType.GetSpriteDirection();
				switch (dir) {
					case Direction.up:
						destX += -0.35f;
						destY += -0.1f;
						break;
					case Direction.left:
						destX += -0.1f;
						destY += -0.35f;
						break;

					case Direction.down:
						destX += -0.2f;
						destY += 0.1f;
						break;

					case Direction.right:
						destX += 0.1f;
						destY += -0.2f;
						break;
				}

				var move = piston.CollsionBox.Move(destX, destY, c => {
					if (c.Other.HasTag(CollisionTag.Player) || c.Other.HasTag(CollisionTag.PushableBlock)) {
						return CollisionResponses.Slide;
					}
					return CollisionResponses.Cross;
				});
			};
		}

		internal static Behaviour TurnLightOn(Object light) {
			return (activator, self) => {
				if (light.TileType.IsOn()) {
					return;
				}
				light.TileType = light.TileType.TurnLightOn();
				if (!string.IsNullOrEmpty(light.OnActivate)) {
					light.Activate(self, light);
				}
			};
		}

		internal static Behaviour TurnLightOff(Object light) {
			return (activator, self) => {
				if (light.TileType.IsOff()) {
					return;
				}
				light.TileType = light.TileType.TurnLightOff();
				if (!string.IsNullOrEmpty(light.OnDeactivate)) {
					light.Deactivate(self, light);
				}
			};
		}

		internal static Behaviour TeleportTo(Object otherPortal) {
			return (activator, self) => {
				if (activator.CollisionTag == CollisionTag.Player || activator.CollisionTag == CollisionTag.PushableBlock) {
					//todo some how teleport the activator to the other portal

					var destX = otherPortal.WorldPos.X;
					var destY = otherPortal.WorldPos.Y;

					var dir = otherPortal.TileType.GetSpriteDirection();
					switch (dir) {
						case Direction.up:
							destY += 0.2f;
							break;
						case Direction.left:
							destX += 0.2f;
							break;
						case Direction.down:
							destY -= 0.2f;
							break;
						case Direction.right:
							destX -= 0.2f;
							break;
					}
					activator.CollsionBox.Move(destX, destY, (c) => CollisionResponses.Cross);
					activator.WorldPos = new Vector3(destY, destY, 0);
				}
			};
		}

		internal static Behaviour GetLightState(Object light) {
			return (activator, self) => {
				self.Memory.Push(light.TileType.IsOn().ToString());
			};
		}

		internal static Behaviour ConditionalAction(Behaviour action) {
			return (activator, self) => {
				if (self.Memory.Count == 0) {
					return;
				}

				if (bool.TryParse(self.Memory.Pop(), out bool t) && t) {
					action(activator, self);
				}
			};
		}

		internal static Behaviour Negate() {
			return (activator, self) => {
				if (self.Memory.Count == 0) {
					return;
				}

				if (bool.TryParse(self.Memory.Pop(), out bool t)) {
					self.Memory.Push((!t).ToString());
				}
			};
		}

		internal static Behaviour And() {
			return (activator, self) => {
				if (self.Memory.Count < 2) {
					return;
				}

				if (bool.TryParse(self.Memory.Pop(), out bool t1)) {
					if (bool.TryParse(self.Memory.Pop(), out bool t2)) {
						self.Memory.Push((t1 && t2).ToString());
					}
				}
			};
		}

		internal static Behaviour Or() {
			return (activator, self) => {
				if (self.Memory.Count < 2) {
					return;
				}

				if (bool.TryParse(self.Memory.Pop(), out bool t1)) {
					if (bool.TryParse(self.Memory.Pop(), out bool t2)) {
						self.Memory.Push((t1 || t2).ToString());
					}
				}
			};
		}

		internal static Behaviour ClearMemory() {
			return (activator, self) => {
				self.Memory.Clear();
			};
		}
	}
}
