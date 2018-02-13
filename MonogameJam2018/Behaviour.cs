﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeIsUp {
	internal delegate void Behaviour(Object activator);
	static class BehaviourHelper {
		internal static Behaviour Parse(Map context, string[] actions) {
			List<Behaviour> parsedActions = new List<Behaviour>();

			foreach (var action in actions) {
				parsedActions.Add(Parse(context, action));
			}

			return RunAction(parsedActions.ToArray());
		}

		internal static Behaviour Parse(Map context, string action) {
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

		internal static Behaviour RunAction(params Behaviour[] actions) {
			return (activator) => {
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
	}
}
