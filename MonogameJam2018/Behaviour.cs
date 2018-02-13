using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeIsUp {
	static class Behaviour {
		internal static Action Parse(Map context, string[] actions) {
			List<Action> parsedActions = new List<Action>();

			foreach (var action in actions) {
				parsedActions.Add(Parse(context, action));
			}

			return RunAction(parsedActions.ToArray());
		}

		internal static Action Parse(Map context, string action) {
			var temp = action.Split();
			var verb = temp[0];
			var target = temp[1];
			Object tg = context.FindObject(target);
			if (tg is null) {
				//todo handle object not found
			}
			//todo check object type

			switch (verb) {
				case "open":
					return OpenDoor(tg);
				case "close":
					return CloseDoor(tg);
				case "turnon":
					return TurnLightOn(tg);
				case "turnoff":
					return TurnLightOff(tg);
				default:
					return NoAction();
			}
		}

		internal static Action OpenDoor(Object door) {
			return () => {
				if (door.TileType.IsOpen()) {
					return;
				}
				door.TileType = door.TileType.OpenDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorClosed);
				door.CollsionBox.AddTags(CollisionTag.DoorOpened);
			};
		}

		internal static Action CloseDoor(Object door) {
			return () => {
				if (door.TileType.IsClose()) {
					return;
				}
				door.TileType = door.TileType.CloseDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorOpened);
				door.CollsionBox.AddTags(CollisionTag.DoorClosed);
			};
		}

		internal static Action RunAction(params Action[] actions) {
			return () => {
				foreach (var action in actions) {
					action();
				}
			};
		}

		internal static Action NoAction() {
			return () => { };
		}

		internal static Action ExtendPistol(Object pistol) {
			return () => { };
		}

		internal static Action RetractPistol(Object pistol) {
			return () => { };
		}

		internal static Action TurnLightOn(Object light) {
			return () => { };
		}

		internal static Action TurnLightOff(Object light) {
			return () => { };
		}
	}
}
