using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeIsUp {
	static class Behaviour {
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
		internal static Action RunAction(Action action) {
			return () => action();
		}
		internal static Action NoAction() {
			return () => { };
		}
	}
}
