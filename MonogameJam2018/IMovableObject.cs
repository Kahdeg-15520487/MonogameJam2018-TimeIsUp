using Humper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TimeIsUp {
	interface IMovableObject {
		IBox CollisionBox { get; set; }
		Vector2 IsoPos { get; }
		Vector2 Origin { get; set; }
		Vector2 WorldPos { get; }

		void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth);
		void LoadContent();
		void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState);
	}
}