﻿using System;
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

		public VT2 Velocity { get; set; }
		public float MovementSpeed { get; set; }

		private Object lastSteppedFloorSwitch;

		public void LoadContent(MainPlayScreen screen) {
			spritesheet = MainPlayScreen.spritesheet;
		}

		public void Update(GameTime gameTime, KeyboardState currentKeyboardState, KeyboardState lastKeyboardState) {
			var delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			Velocity *= 0.1f;
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

				if (x.Other.HasTag(CollisionTag.Portal)) {
					return CollisionResponses.Cross;
				}

				return CollisionResponses.Slide;
			});

			var floorswitch = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.FloorSwitch));
			if (floorswitch != null) {
				Object obj = (Object)floorswitch.Box.Data;
				obj.TileType = SpriteSheetRectName.ButtonPressed_E;
				obj.Activate(Object, obj);
				lastSteppedFloorSwitch = obj;
			}
			else {
				if (lastSteppedFloorSwitch != null) {
					lastSteppedFloorSwitch.TileType = SpriteSheetRectName.Button_E;
					lastSteppedFloorSwitch.Deactivate(Object, lastSteppedFloorSwitch);
					lastSteppedFloorSwitch = null;
				}
			}

			var block = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.PushableBlock));
			if (block != null) {
				Block obj = (Block)block.Box.Data;
				var n = block.Normal;
				obj.Velocity = new VT2(n.X * n.X, n.Y * n.Y) * Velocity;
			}

			var portal = move.Hits.FirstOrDefault(c => c.Box.HasTag(CollisionTag.Portal));
			if (portal != null) {
				Object obj = (Object)portal.Box.Data;
				obj.Activate(Object, obj);
			}

			Object.WorldPos = new Vector3(WorldPos.X, WorldPos.Y, 0);
		}

		public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float depth) {
			var dos = 0.5f - ((Object.WorldPos.X + Object.WorldPos.Y + Object.WorldPos.Z) / MainPlayScreen.maxdepth) - 0.001f;
			VT2 IsoPos = Object.WorldPos.WorldToIso();
			spriteBatch.Draw(spritesheet, IsoPos, MainPlayScreen.spriterects[Object.TileType], Color.White, 0f, Object.SpriteOrigin, Constant.SCALE, SpriteEffects.None, dos);
			//spriteBatch.DrawString(MainPlayScreen.font, dos.ToString() + Environment.NewLine + Object.Position.ToString(), IsoPos, Color.Black);
		}
	}
}
