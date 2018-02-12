using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humper;
using Humper.Responses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeIsUp.GameScreens;
using Utility;
using VT2 = Microsoft.Xna.Framework.Vector2;

namespace TimeIsUp {
	class Block : IMovableObject {
		public IBox CollisionBox { get; set; }

		public Object Object { get; set; }
		private Texture2D spritesheet;

		public VT2 IsoPos => WorldPos.WorldToIso();

		public VT2 Origin { get; set; } = Constant.SPRITE_ORIGIN;

		public VT2 WorldPos => new VT2(CollisionBox.X, CollisionBox.Y);

		public VT2 Velocity;
		private Object lastSteppedFloorWitch;

		public void LoadContent() {
			spritesheet = MainPlayScreen.spritesheet;
		}

		public void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			Velocity *= 0.2f;
			var move = CollisionBox.Move(CollisionBox.X + delta * Velocity.X, CollisionBox.Y + delta * Velocity.Y, x => {
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

				return CollisionResponses.Slide;
			});

			var floorswitch = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.FloorSwitch));
			if (floorswitch != null) {
				Object obj = (Object)floorswitch.Box.Data;
				obj.Name = SpriteSheetRectName.ButtonPressed_E;
				obj.Activate();
				lastSteppedFloorWitch = obj;
			}
			else {
				if (lastSteppedFloorWitch != null) {
					lastSteppedFloorWitch.Name = SpriteSheetRectName.Button_E;
					lastSteppedFloorWitch.Deactivate();
					lastSteppedFloorWitch = null;
				}
			}

			Object.Position = new Vector3(WorldPos.X, WorldPos.Y, 0);
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth) {
			var dos = 0.5f - ((Object.Position.X + Object.Position.Y + Object.Position.Z) / MainPlayScreen.maxdepth) - 0.001f;
			VT2 IsoPos = Object.Position.WorldToIso();
			spriteBatch.Draw(spritesheet, IsoPos, MainPlayScreen.spriterects[Object.Name], Color.White, 0f, Object.Origin, Constant.SCALE, SpriteEffects.None, dos);
			//spriteBatch.DrawString(MainPlayScreen.font, dos.ToString() + Environment.NewLine + Object.Position.ToString(), IsoPos, Color.Black);
		}
	}
}
