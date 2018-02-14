using Humper.Responses;
using System.Collections.Generic;
using System.Linq;

namespace TimeIsUp {
	internal delegate void Behaviour(Object activator);
	static class BehaviourHelper {
		internal static List<Object> GetAllTarget(Map context,string[] actions) {
			return actions.Select(x => GetTarget(context, x)).ToList();
		}

		internal static Object GetTarget(Map context,string action) {
			return context.FindObject(action.Split()[1]);
		}

		internal static Behaviour Parse(Map context, string self, string[] actions) {
			List<Behaviour> parsedActions = new List<Behaviour>();

			Object s = context.FindObject(self);

			foreach (var action in actions) {
				parsedActions.Add(Parse(context, s, action));
			}

			return RunAction(s, parsedActions.ToArray());
		}

		internal static Behaviour Parse(Map context, string self, string action) {
			Object s = context.FindObject(self);
			return Parse(context, s, action);
		}

		internal static Behaviour Parse(Map context, Object self, string action) {
			var temp = action.Split();
			var verb = temp[0];
			var targetName = temp[1];
			Object target = context.FindObject(targetName);
			if (target is null) {
				//todo handle object not found
			}
			//todo check object type

			switch (verb) {
				case "open":
					return OpenDoor(target);
				case "close":
					return CloseDoor(target);
				case "turnon":
					return TurnLightOn(target);
				case "turnoff":
					return TurnLightOff(target);
				case "teleportto":
					return TeleportTo(target);
				default:
					return NoAction();
			}
		}

		internal static Behaviour OpenDoor(Object door) {
			return (activator) => {
				if (door.TileType.IsOpen()) {
					return;
				}
				door.TileType = door.TileType.OpenDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorClosed);
				door.CollsionBox.AddTags(CollisionTag.DoorOpened);
			};
		}

		internal static Behaviour CloseDoor(Object door) {
			return (activator) => {
				if (door.TileType.IsClose()) {
					return;
				}
				door.TileType = door.TileType.CloseDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorOpened);
				door.CollsionBox.AddTags(CollisionTag.DoorClosed);
			};
		}

		internal static Behaviour RunAction(Object self, params Behaviour[] actions) {
			return (activator) => {
				self.Memory.Clear();
				foreach (var action in actions) {
					action(activator);
				}
			};
		}

		internal static Behaviour NoAction() {
			return (activator) => { };
		}

		internal static Behaviour ExtendPistol(Object pistol) {
			return (activator) => { };
		}

		internal static Behaviour RetractPistol(Object pistol) {
			return (activator) => { };
		}

		internal static Behaviour TurnLightOn(Object light) {
			return (activator) => {
				if (light.TileType.IsOn()) {
					return;
				}
				light.TileType = light.TileType.TurnLightOn();
			};
		}

		internal static Behaviour TurnLightOff(Object light) {
			return (activator) => {
				if (light.TileType.IsOff()) {
					return;
				}
				light.TileType = light.TileType.TurnLightOff();
			};
		}

		internal static Behaviour TeleportTo(Object otherPortal) {
			return (activator) => {
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
				}
				activator.CollsionBox.Move(destX, destY, (c) => CollisionResponses.Cross);
			};
		}
	}
}
