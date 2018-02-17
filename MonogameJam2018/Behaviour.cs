using Humper.Responses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeIsUp.GameScreens;
using Utility;
using Utility.ScreenManager;

namespace TimeIsUp {
	internal delegate void Behaviour(Object activator, Object self);
	static class BehaviourHelper {
		static Random random = new Random(1);

		internal static List<Object> GetAllTarget(Map context, string[] actions) {
			return actions.Select(x => GetTarget(context, x)).ToList();
		}

		internal static Object GetTarget(Map context, string action) {
			return action.Split().Length == 1 ? null : context.FindObject(action.Split()[1]);
		}

		internal static string[] GetAllToken(string source) {
			if (!source.Contains(" ")) {
				return new string[] { source };
			}

			bool isInString = false;
			List<string> result = new List<string>();
			StringBuilder token = new StringBuilder();
			bool isTokenDone = false;
			for (int i = 0; i < source.Length; i++) {
				switch (source[i]) {
					case '"':
						isInString = !isInString;
						isTokenDone = true;
						break;

					case ' ':
						if (!isInString) {
							isTokenDone = true;
						}
						else {
							token.Append(' ');
						}
						break;

					default:
						token.Append(source[i]);
						break;
				}

				if (i == source.Length - 1) {
					isTokenDone = true;
				}

				if (isTokenDone) {
					if (token.Length > 0) {
						result.Add(token.ToString());
						token.Clear();
					}
					isTokenDone = false;
				}
			}
			return result.ToArray();
		}

		internal static Behaviour Parse(Map context, string self, params string[] actions) {
			List<Behaviour> parsedActions = new List<Behaviour>();

			Object s = context.FindObject(self);
			bool ConditionalFlag = false;

			foreach (var action in actions) {
				var temp = GetAllToken(action);
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

							case "concat":
								a = Concat();
								break;

							case "dup":
								a = Duplicate();
								break;
							case "clearmem":
								a = ClearMemory();
								break;

							case "random":
								a = RandomNumber();
								break;

							case "dialog":
								a = ShowDialog();
								break;
							case "setplayername":
								a = SetPlayerName();
								break;
							case "loadplayername":
								a = LoadPlayerName();
								break;
							case "getplayername":
								a = GetPlayerName();
								break;
							case "win":
								a = Win();
								break;
							case "exit":
								a = ExitGame();
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
								a = (TurnOn(target));
								break;
							case "turnoff":
								a = (TurnOff(target));
								break;

							case "getstate":
								a = (GetState(target));
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

							case "dialog":
								a = ShowDialog(temp[1]);
								break;
							case "win":
								a = Win(temp[1]);
								break;

							case "push":
								a = Push(temp[1]);
								break;

							case "checklevel":
								a = CheckLevelUnlock(temp[1]);
								break;
							case "unlocklevel":
								a = UnlockLevel(temp[1]);
								break;
						}
						break;
					case 2:
						var target1 = context.FindObject(temp[1]);
						var target2 = context.FindObject(temp[2]);
						switch (verb) {

							case "link":
								a = LinkPortal(target1,target2);
								break;

							case "unlink":
								a = UnlinkPortal(target1, target2);
								break;
						}
						break;
				}

				if (ConditionalFlag && a != null) {
					ConditionalFlag = false;
					a = ConditionalAction(a);
				}

				if (a != null) {
					parsedActions.Add(a);
				}
			}

			return RunActions(parsedActions.ToArray());
		}

		private static Behaviour LinkPortal(Object target1,Object target2) {
			return (a, s) => {
				target1.Activate = TeleportTo(target2);
				target2.Activate = TeleportTo(target1);
			};
		}

		private static Behaviour UnlinkPortal(Object target1, Object target2) {
			return (a, s) => {
				target1.Activate = NoAction();
				target2.Activate = NoAction();
			};
		}

		private static Behaviour CheckLevelUnlock(string level) {
			return (a, s) => {
				s.Memory.Push(SaveGame.IsLevelUnlocked(int.Parse(level)).ToString());
			};
		}

		private static Behaviour UnlockLevel(string level) {
			return (a, s) => {
				SaveGame.UnlockLevel(int.Parse(level));
			};
		}

		private static Behaviour ExitGame() {
			MainPlayScreen mainPlayScreen = (MainPlayScreen)SCREEN_MANAGER.GetScreen("MainPlayScreen");
			return (a, s) => {
				mainPlayScreen.isExit = true;
				mainPlayScreen.CurrentGameState = GameState.Exit;
				mainPlayScreen.msgbox.Show("Exitting game.", "OK");
			};
		}

		private static Behaviour Win(string nextlvl = null) {
			MainPlayScreen mainPlayScreen = (MainPlayScreen)SCREEN_MANAGER.GetScreen("MainPlayScreen");
			return (activator, self) => {
				mainPlayScreen.isWin = true;

				if (!string.IsNullOrEmpty(nextlvl)) {
					mainPlayScreen.isBehaviourChangeMap = true;
					mainPlayScreen.MapName = nextlvl;
				}
			};
		}

		private static Behaviour LoadPlayerName() {
			return (a, s) => {
				Constant.PLAYER_NAME = SaveGame.GetPlayerName();
			};
		}

		private static Behaviour GetPlayerName() {
			return (a, s) => {
				s.Memory.Push(Constant.PLAYER_NAME);
			};
		}

		private static Behaviour SetPlayerName() {
			return (a, s) => {
				if (s.Memory.Count == 0) {
					return;
				}

				Constant.PLAYER_NAME = s.Memory.Pop();
				SaveGame.SetPlayerName();
			};
		}

		private static Behaviour Duplicate() {
			return (a, s) => {
				if (s.Memory.Count == 0) {
					return;
				}

				var t = s.Memory.Pop();
				s.Memory.Push(t);
				s.Memory.Push(t);
			};
		}

		private static Behaviour Concat() {
			return (a, s) => {
				if (s.Memory.Count < 2) {
					return;
				}

				var s1 = s.Memory.Pop();
				var s2 = s.Memory.Pop();

				s.Memory.Push(s1 + s2);
			};
		}

		private static Behaviour Push(string value) {
			return (a, s) => {
				s.Memory.Push(value.Replace("\"", ""));
			};
		}

		private static Behaviour RandomNumber() {
			return (a, s) => {
				s.Memory.Push(random.Next(0, 1000000).ToString("D6"));
			};
		}

		private static Behaviour AnnotateMap(Map map, string content) {
			return (activator, self) => {

			};
		}

		private static Behaviour ShowDialog(string content) {
			MainPlayScreen mainPlayScreen = (MainPlayScreen)SCREEN_MANAGER.GetScreen("MainPlayScreen");
			return (activator, self) => {
				mainPlayScreen.timer.Stop();
				mainPlayScreen.msgbox.Show(content.ProcessAnnotation());
				mainPlayScreen.gameStates.Push(GameState.ShowDialog);
			};
		}

		private static Behaviour ShowDialog() {
			MainPlayScreen mainPlayScreen = (MainPlayScreen)SCREEN_MANAGER.GetScreen("MainPlayScreen");
			return (activator, self) => {
				if (self.Memory.Count == 0) {
					return;
				}

				mainPlayScreen.timer.Stop();
				var content = self.Memory.Pop();
				mainPlayScreen.msgbox.Show(content.ProcessAnnotation());
				mainPlayScreen.gameStates.Push(GameState.ShowDialog);
			};
		}

		internal static Behaviour OpenDoor(Object door) {
			SoundEffectInstance dooropensfx = CONTENT_MANAGER.Sounds["door_open"].CreateInstance();
			return (activator, self) => {
				if (door.TileType.IsOpen()) {
					return;
				}
				dooropensfx.Play();
				door.TileType = door.TileType.OpenDoor();
				door.CollsionBox.RemoveTags(CollisionTag.DoorClosed);
				door.CollsionBox.AddTags(CollisionTag.DoorOpened);
			};
		}

		internal static Behaviour CloseDoor(Object door) {
			SoundEffectInstance doorclosesfx = CONTENT_MANAGER.Sounds["door_close"].CreateInstance();
			return (activator, self) => {
				if (door.TileType.IsClose()) {
					return;
				}
				doorclosesfx.Play();
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

		internal static Behaviour TurnOn(Object thingy) {
			if (thingy.TileType.IsLight()) {
				return TurnOnLight(thingy);
			}
			else if (thingy.TileType.IsPortal()) {
				return TurnOnPortal(thingy);
			}
			return NoAction();
		}

		internal static Behaviour TurnOnLight(Object light) {
			SoundEffectInstance lightonsfx = CONTENT_MANAGER.Sounds["light_on"].CreateInstance();
			return (activator, self) => {
				if (light.TileType.IsOn()) {
					return;
				}
				light.TileType = light.TileType.TurnOn();
				lightonsfx.Play();
				if (!string.IsNullOrEmpty(light.OnActivate)) {
					light.Activate(self, light);
				}
			};
		}

		internal static Behaviour TurnOnPortal(Object portal) {
			return (activator, self) => {
				if (portal.TileType.IsOn()) {
					return;
				}
				portal.TileType = portal.TileType.TurnOn();
			};
		}

		internal static Behaviour TurnOff(Object thingy) {
			if (thingy.TileType.IsLight()) {
				return TurnOffLight(thingy);
			}
			else if (thingy.TileType.IsPortal()) {
				return TurnOffPortal(thingy);
			}
			return NoAction();
		}

		internal static Behaviour TurnOffLight(Object light) {
			SoundEffectInstance lightoffsfx = CONTENT_MANAGER.Sounds["light_off"].CreateInstance();
			return (activator, self) => {
				if (light.TileType.IsOff()) {
					return;
				}
				light.TileType = light.TileType.TurnOff();
				lightoffsfx.Play();
				if (!string.IsNullOrEmpty(light.OnDeactivate)) {
					light.Deactivate(self, light);
				}
			};
		}

		internal static Behaviour TurnOffPortal(Object portal) {
			return (activator, self) => {
				if (portal.TileType.IsOff()) {
					return;
				}
				portal.TileType = portal.TileType.TurnOff();
			};
		}

		internal static Behaviour TeleportTo(Object otherPortal) {
			SoundEffectInstance telesfx = CONTENT_MANAGER.Sounds["tele"].CreateInstance();
			return (activator, self) => {
				if (activator.CollisionTag == CollisionTag.Player || activator.CollisionTag == CollisionTag.PushableBlock) {

					//play teleport sound
					telesfx.Play();

					//teleport IMovableObject
					var destX = otherPortal.WorldPos.X;
					var destY = otherPortal.WorldPos.Y;

					var dir = otherPortal.TileType.GetSpriteDirection();
					var displacement = 0.3f;
					if (activator.CollisionTag == CollisionTag.PushableBlock) {
						displacement = 0.5f;
					}

					switch (dir) {
						case Direction.up:
							destY += displacement;
							break;
						case Direction.left:
							destX += displacement;
							break;
						case Direction.down:
							destY -= displacement;
							break;
						case Direction.right:
							destX -= displacement;
							break;
					}
					activator.CollsionBox.Move(destX, destY, (c) => CollisionResponses.None);
					activator.WorldPos = new Vector3(destY, destY, 0);
				}
			};
		}

		internal static Behaviour GetState(Object thingy) {
			return (activator, self) => {
				self.Memory.Push(thingy.TileType.IsOn().ToString());
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
