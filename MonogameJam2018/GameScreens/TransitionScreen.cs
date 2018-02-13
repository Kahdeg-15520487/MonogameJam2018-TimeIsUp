using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.ScreenManager;
using Utility.UI;

namespace imeIsUp.GameScreens {
	class TransitionScreen : Screen {
		Canvas canvas;

		public TransitionScreen(GraphicsDevice device) : base(device, "TransitionScreen") { }

		public override bool Init() {
			InitUI();

			return base.Init();
		}

		public void InitUI() {
			canvas = new Canvas();
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime, CONTENT_MANAGER.CurrentInputState, CONTENT_MANAGER.LastInputState);
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			canvas.Draw(spriteBatch, gameTime);
		}
	}
}
