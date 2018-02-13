using Humper;
using Humper.Responses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeIsUp.GameScreens;
using Utility;
using Utility.Drawing.Animation;
using VT2 = Microsoft.Xna.Framework.Vector2;

namespace TimeIsUp {
	class Player : IMovableObject {
		AnimatedEntity animatedEntity;
		SpriteFont font;
		Map map;
		MainPlayScreen screen;

		Direction dir;

		Object lastSteppedFloorWitch = null;
		Object lastLever = null;
		Object lastLadder = null;
		Object lastHole = null;

		public IBox CollisionBox { get; set; }
		public VT2 WorldPos { get { return new VT2(CollisionBox.X, CollisionBox.Y); } }
		public VT2 IsoPos { get { return WorldPos.WorldToIso(); } }
		public VT2 Origin {
			get {
				return animatedEntity.Origin;
			}
			set {
				animatedEntity.Origin = value;
			}
		}
		public VT2 Velocity { get; set; } = VT2.Zero;
		public float MovementSpeed { get; set; } = 0.07f;

		public bool isFalling = false;

		public void LoadContent(MainPlayScreen screen) {
			this.screen = screen;
			var animationSpriteSheet = CONTENT_MANAGER.Sprites["animation"];
			font = MainPlayScreen.font;
			var frames = JsonConvert.DeserializeObject<List<KeyValuePair<string, Rectangle>>>(File.ReadAllText(@"Content/spritesheet/animation.json"));
			var anims = new List<Animation>();
			var animnames = Enum.GetNames(typeof(AnimationName));
			foreach (var an in animnames) {
				anims.Add(LoadAnimation(an, frames));
			}

			animatedEntity = new AnimatedEntity(IsoPos, VT2.Zero, Color.White, 0.4f, Constant.SCALE);
			animatedEntity.LoadContent(animationSpriteSheet);
			animatedEntity.AddAnimation(anims);
			animatedEntity.PlayAnimation(AnimationName.idle_right.ToString());
		}

		private Animation LoadAnimation(string animname, List<KeyValuePair<string, Rectangle>> frames) {
			Animation anim = new Animation(animname, true, 14, animname);
			var anim_frame = (from f in frames
							  where f.Key.StartsWith(animname)
							  select f.Value).ToList();
			//foreach (var f in anim_frame) {
			for (int i = 1; i < anim_frame.Count; i++) {
				anim.AddKeyFrame(anim_frame[i]);
			}
			return anim;
		}

		public void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {

			MovePlayer(currentKeyboardState, lastKeyboardState, gameTime);

			Interact(currentKeyboardState, lastKeyboardState);

			if (isFalling) {
				Origin -= new VT2(0, 1f);
			}

			animatedEntity.Update(gameTime);
		}

		private void Interact(KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			//todo make interactable object a universal method
			if (HelperMethod.IsKeyPress(Keys.E, currentKeyboardState, lastKeyboardState)) {
				if (lastLever != null) {
					lastLever.TileType = lastLever.TileType.FlipSwitch();
					if (lastLever.TileType.IsOn()) {
						lastLever.Activate();
					}
					else if (lastLever.TileType.IsOff()) {
						lastLever.Deactivate();
					}
				}
				if (lastLadder != null) {
					//next level stuff
				}
			}
		}

		private void MovePlayer(KeyboardState currentKeyboardState, KeyboardState lastKeyboardState, GameTime gameTime) {
			var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			var curpos = new VT2(CollisionBox.X, CollisionBox.Y);
			var direction = Direction.none;

			if (HelperMethod.IsKeyHold(Keys.A, currentKeyboardState, lastKeyboardState)) {
				direction = Direction.left;
			}
			if (HelperMethod.IsKeyHold(Keys.D, currentKeyboardState, lastKeyboardState)) {
				direction = Direction.right;
			}
			if (HelperMethod.IsKeyHold(Keys.W, currentKeyboardState, lastKeyboardState)) {
				direction = Direction.up;
			}
			if (HelperMethod.IsKeyHold(Keys.S, currentKeyboardState, lastKeyboardState)) {
				direction = Direction.down;
			}

			switch (direction) {
				case Direction.up:
					Velocity = new VT2(0, -MovementSpeed);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_up.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_up.ToString());
					}
					break;
				case Direction.down:
					Velocity = new VT2(0, MovementSpeed);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_down.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_down.ToString());
					}
					break;
				case Direction.right:
					Velocity = new VT2(MovementSpeed, 0);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_right.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_right.ToString());
					}
					break;
				case Direction.left:
					Velocity = new VT2(-MovementSpeed, 0);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_left.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_left.ToString());
					}
					break;
				case Direction.none:
					var anme = "idle_" + dir.ToString();
					if (animatedEntity.CurntAnimationName != anme) {
						animatedEntity.PlayAnimation(anme);
					}
					break;
			}

			if (isFalling) {
				//Velocity = VT2.Zero;
				Velocity = (lastHole.WorldPos.ToVector2() - WorldPos);
			}

			var move = CollisionBox.Move(curpos.X + Velocity.X, curpos.Y + Velocity.Y, x => {

				if (x.Other.HasTag(CollisionTag.Hole)) {
					isFalling = true;
					lastHole = (Object)x.Other.Data;
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.FloorSwitch)) {
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.Lever)) {
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.DoorOpened)) {
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.Ladder)) {
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.EndPoint)) {
					screen.Win();
					return CollisionResponses.Cross;
				}

				if (x.Other.HasTag(CollisionTag.PushableBlock)) {
					return CollisionResponses.Slide;
				}

				return CollisionResponses.Slide;
			});

			var floorswitch = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.FloorSwitch));
			if (floorswitch != null) {
				Object obj = (Object)floorswitch.Box.Data;
				obj.TileType = SpriteSheetRectName.ButtonPressed_E;
				obj.Activate();
				lastSteppedFloorWitch = obj;
			}
			else {
				if (lastSteppedFloorWitch != null) {
					lastSteppedFloorWitch.TileType = SpriteSheetRectName.Button_E;
					lastSteppedFloorWitch.Deactivate();
					lastSteppedFloorWitch = null;
				}
			}

			var lever = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.Lever));
			if (lever != null) {
				Object obj = (Object)lever.Box.Data;
				lastLever = obj;
			}
			else {
				if (lastLever != null) {
					lastLever = null;
				}
			}

			var ladder = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.Ladder));
			if (ladder != null) {
				Object obj = (Object)ladder.Box.Data;
				lastLadder = obj;
			}
			else {
				if (lastLadder != null) {
					lastLadder = null;
				}
			}

			var block = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.PushableBlock));
			if (block != null) {
				Block obj = (Block)block.Box.Data;
				var n = block.Normal;
				obj.Velocity = new VT2(n.X * n.X, n.Y * n.Y) * Velocity;
			}

			animatedEntity.Position = IsoPos;
			Velocity = VT2.Zero;
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth) {
			spriteBatch.DrawString(font, WorldPos.ToString(), IsoPos, Color.Black);
			animatedEntity.Draw(spriteBatch, gameTime, isFalling ? 1f : depth);
		}
	}
}
