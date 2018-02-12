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
using VT2 = Microsoft.Xna.Framework.Vector2;

namespace TimeIsUp {
	class Block : IMovableObject {
		public IBox CollisionBox { get; set; }

		public VT2 IsoPos => WorldPos.WorldToIso();

		public VT2 Origin { get; set; } = Constant.SPRITE_ORIGIN;

		public VT2 WorldPos => new VT2(CollisionBox.X, CollisionBox.Y);

		public VT2 Velocity;

		public void LoadContent() {

		}

		public void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			var move = CollisionBox.Move(CollisionBox.X + delta * Velocity.X, CollisionBox.Y + delta * Velocity.Y, (collision) =>
			{
				if (collision.Other.HasTag(CollisionTag.FloorSwitch)) {
					return CollisionResponses.Cross;
				}

				return CollisionResponses.Slide;
			});
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth) {
			
		}
	}
}
