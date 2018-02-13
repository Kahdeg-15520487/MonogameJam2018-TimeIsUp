using Humper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeIsUp.GameScreens;

namespace TimeIsUp {
	interface IMovableObject {
		IBox CollisionBox { get; set; }
		Vector2 IsoPos { get; }
		Vector2 Origin { get; set; }
		Vector2 WorldPos { get; }

		void LoadContent(MainPlayScreen screen);
		void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState);
		void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth);
	}
}