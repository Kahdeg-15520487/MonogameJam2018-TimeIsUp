using Humper;
using Humper.Base;
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
using System.Text;
using System.Threading.Tasks;
using Utility.Drawing.Animation;
using VT2 = Microsoft.Xna.Framework.Vector2;

namespace TimeIsUp {
	class Player {
		AnimatedEntity animatedEntity;
		SpriteFont font;
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

		public void LoadContent(ContentManager content) {
			var animationSpriteSheet = content.Load<Texture2D>(@"spritesheet/animation");
			font = content.Load<SpriteFont>("default");
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

			MovePlayer(currentKeyboardState, lastKeyboardState);

			animatedEntity.Update(gameTime);
		}

		Direction dir;

		private void MovePlayer(KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			var curpos = new VT2(CollisionBox.X, CollisionBox.Y);
			var direction = Direction.none;
			var velocity = VT2.Zero;
			if (HelperMethod.IsKeyHold(currentKeyboardState, lastKeyboardState, Keys.A)) {
				direction = Direction.left;
			}
			if (HelperMethod.IsKeyHold(currentKeyboardState, lastKeyboardState, Keys.D)) {
				direction = Direction.right;
			}
			if (HelperMethod.IsKeyHold(currentKeyboardState, lastKeyboardState, Keys.W)) {
				direction = Direction.up;
			}
			if (HelperMethod.IsKeyHold(currentKeyboardState, lastKeyboardState, Keys.S)) {
				direction = Direction.down;
			}

			switch (direction) {
				case Direction.up:
					velocity = new VT2(0, -0.05f);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_up.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_up.ToString());
					}
					break;
				case Direction.down:
					velocity = new VT2(0, 0.05f);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_down.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_down.ToString());
					}
					break;
				case Direction.right:
					velocity = new VT2(0.05f, 0);
					dir = direction;
					if (animatedEntity.CurntAnimationName != AnimationName.walk_right.ToString()) {
						animatedEntity.PlayAnimation(AnimationName.walk_right.ToString());
					}
					break;
				case Direction.left:
					velocity = new VT2(-0.05f, 0);
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

			CollisionBox.Move(curpos.X + velocity.X, curpos.Y + velocity.Y, x => {
				if (x.Other.HasTag(CollisionTag.FloorSwitch)) {
					IBox door = (Box)x.Other.Data;
					if (door.HasTag(CollisionTag.DoorClosed)) {
						door.RemoveTags(CollisionTag.DoorClosed);
						door.AddTags(CollisionTag.DoorOpened);
						var doorpos = (Point)door.Data;
						GameManager.map.Walls[doorpos.Y][doorpos.X] = SpriteSheetRectName.WallDoorOpen_S;
						GameManager.map.FindObject(SpriteSheetRectName.Slab_E).Origin += new VT2(0, 1);
						Origin += new VT2(0, 1);
					}
					return CollisionResponses.Cross;
				}
				if (x.Other.HasTag(CollisionTag.DoorOpened)) {
					return CollisionResponses.Cross;
				}
				return CollisionResponses.Slide;
			});
			animatedEntity.Position = IsoPos;
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth) {
			spriteBatch.DrawString(font, depth.ToString() + Environment.NewLine + WorldPos.ToString(), IsoPos, Color.Black);
			animatedEntity.Draw(spriteBatch, gameTime, depth);
		}
	}
}
