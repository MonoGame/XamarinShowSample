using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Develop
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D icon;
		SpriteFont font;
        SoundEffect fire;
		float iconRotation;

		Model model;
		Vector3 cameraPosition = new Vector3 (0f, 0f, -500f);
		float modelRotation;
		
		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this) {
                PreferredBackBufferWidth = 800,
				PreferredBackBufferHeight = 600,
				IsFullScreen = false,
			};
			graphics.SupportedOrientations =
                DisplayOrientation.Portrait |
				DisplayOrientation.LandscapeLeft |
				DisplayOrientation.LandscapeRight;
			
			Content.RootDirectory = "Content";
			Window.AllowUserResizing = true;
            IsMouseVisible = true;

            TouchPanel.EnabledGestures = GestureType.DoubleTap;
		}

		protected override void Initialize ()
		{
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			
			font = Content.Load<SpriteFont> ("Font");
			icon = Content.Load<Texture2D> ("icon");
            model = Content.Load<Model> ("ship");
            fire = Content.Load<SoundEffect>("fire");

		}

		protected override void Update (GameTime gameTime)
		{
			var keyboardState = Keyboard.GetState ();
			var gamePadState = GamePad.GetState (PlayerIndex.One);
			var touchState = TouchPanel.GetState ();


#if !__IOS__ && !__TVOS__
			if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown (Keys.Escape))
				Exit ();
#endif

			float rotateBy = 0f;
            if (keyboardState.IsKeyDown (Keys.Left) || gamePadState.ThumbSticks.Right.X < 0f) {
				rotateBy = -0.2f;
			}
			if (keyboardState.IsKeyDown (Keys.Right) || gamePadState.ThumbSticks.Right.X > 0f) {
				rotateBy = 0.2f;
			}
            if (keyboardState.IsKeyDown (Keys.Space) || gamePadState.IsButtonDown (Buttons.A))
            {
               fire.Play();
            }
			foreach (var touch in touchState) {
				if (touch.State != TouchLocationState.Released) {
                    if (touch.Position.X < (TouchPanel.DisplayWidth / 2)) {
						rotateBy = -0.2f;
					}
					if (touch.Position.X > (TouchPanel.DisplayWidth / 2)) {
						rotateBy = 0.2f;
					}
				}
			}

            while (TouchPanel.IsGestureAvailable)
            {
                var gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.DoubleTap)
                {
                    fire.Play();
                }
            }

			modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians (rotateBy);

			iconRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians (0.1f);

			base.Update (gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{

			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
			Matrix projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (45.0f), aspectRatio,
				1.0f, 10000.0f);
			Matrix view = Matrix.CreateLookAt (cameraPosition,
				Vector3.Zero, Vector3.Up);
			Vector3 position = Vector3.Zero;
			Matrix [] transforms = new Matrix [model.Bones.Count];
			model.CopyBoneTransformsTo (transforms);
			foreach (ModelMesh mesh in model.Meshes) {
				foreach (BasicEffect effect in mesh.Effects) {
					effect.EnableDefaultLighting ();
                    effect.View = view;
					effect.Projection = projection;
					effect.World = Matrix.CreateRotationX (0.4f) *
						Matrix.CreateRotationY (modelRotation) *
						transforms [mesh.ParentBone.Index] *
						Matrix.CreateTranslation (position);
				}
				mesh.Draw ();
			}

			spriteBatch.Begin ();
			spriteBatch.Draw (icon, new Vector2 (30, 30), Color.White);
			var v = font.MeasureString ("MonoGame Rocks!");
			spriteBatch.DrawString (font, "MonoGame Rocks!",
				new Vector2 (GraphicsDevice.Viewport.Width - v.X - 30, 30),
				Color.MonoGameOrange);
			spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

