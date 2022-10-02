using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Runner
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont scoreFont;

        Camera cam = new Camera();
        Random rnd = new Random();
        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        List<IBlock> blocks = new List<IBlock>();
        List<Flier> fliers = new List<Flier>();

        Vector3 lastColour = new Vector3(1, 0, 0);
        float colourDelta = 0.3f;
        int minFlierNumber = 10;
        int maxFlierNumber = 20;

        float score = 0;
        float playerSpeed = 0.005f;
        RunnerState runnerState = RunnerState.Default;
        ButtonState lastButtonState = ButtonState.Released;
        bool isGameOver = false;
        double jumpStart = 0;

        enum RunnerState
        {
            Default,
            Crouching,
            Jumping,
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Window.AllowUserResizing = true;
            graphics.ToggleFullScreen();
            Mouse.SetPosition(100, 100);

            cam.AspectRatio = (float)Window.ClientBounds.Width / Window.ClientBounds.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load font
            scoreFont = Content.Load<SpriteFont>("scoreFont");

            // Load textures
            textures["defaultTexture"] = Content.Load<Texture2D>("defaultTexture2");
            textures["targetTexture"] = Content.Load<Texture2D>("targetTexture");
            textures["crosshair"] = Content.Load<Texture2D>("crosshair");

            // Add first seven blocks
            for (int i = 0; i < 7; i++)
            {
                blocks.Add(new ForwardBlock(new Vector3(0, 0, 4 * i), lastColour, false, false, textures, GraphicsDevice));
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle game over
            if (isGameOver)
                return;

            // Score++
            score += playerSpeed * gameTime.ElapsedGameTime.Milliseconds;

            // Movement and keyboard
            var ks = Keyboard.GetState();
            cam.Position.Z += playerSpeed * gameTime.ElapsedGameTime.Milliseconds;

            if (ks.IsKeyDown(Keys.W) && runnerState == RunnerState.Default)
            {
                runnerState = RunnerState.Jumping;
                jumpStart = gameTime.TotalGameTime.TotalSeconds;
            }
            else if (runnerState == RunnerState.Jumping)
            {
                double v0 = 4.5;
                double g = 8;

                double t = gameTime.TotalGameTime.TotalSeconds - jumpStart;
                cam.Position.Y = (float)(1.8 + v0 * t - (g / 2) * t * t);
                if (cam.Position.Y <= 1.8f)
                {
                    cam.Position.Y = 1.8f;
                    runnerState = RunnerState.Default;
                }
            }
            else
            {
                if (ks.IsKeyDown(Keys.S))
                {
                    runnerState = RunnerState.Crouching;
                    cam.Position.Y = MathHelper.Max(0.8f, cam.Position.Y - 0.01f * gameTime.ElapsedGameTime.Milliseconds);
                }
                if (ks.IsKeyDown(Keys.A))
                {
                    cam.Position.X += 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                }
                if (ks.IsKeyDown(Keys.D))
                {
                    cam.Position.X -= 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                }

                if (runnerState == RunnerState.Crouching && ks.IsKeyUp(Keys.S))
                {
                    cam.Position.Y += 0.01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (cam.Position.Y >= 1.8f)
                    {
                        cam.Position.Y = 1.8f;
                        runnerState = RunnerState.Default;
                    }
                }
            }

            // Mouse
            var ms = Mouse.GetState();
            var pos = ms.Position;
            Mouse.SetPosition(100, 100);
            var delta = pos - new Point(100, 100);

            var currentVAngle = Math.Acos(Vector3.Dot(new Vector3(0, 1, 0), cam.Direction)) * 180 / MathHelper.Pi;
            var verticalDelta = MathHelper.Clamp((-delta.Y / 200f) * 180 / MathHelper.Pi,
                                                 (float)currentVAngle - 160,
                                                 (float)currentVAngle - 20) * MathHelper.Pi / 180;

            var currentHAngle = Math.Acos(Vector3.Dot(new Vector3(-1, 0, 0), Vector3.Normalize(new Vector3(cam.Direction.X, 0, cam.Direction.Z)))) * 180 / MathHelper.Pi;
            var horizontalDelta = MathHelper.Clamp((delta.X / 200f) * 180 / MathHelper.Pi,
                                                   (float)currentHAngle - 160,
                                                   (float)currentHAngle - 20) * MathHelper.Pi / 180;

            var m = Matrix.CreateRotationY(-horizontalDelta) * Matrix.CreateFromAxisAngle(cam.Right, verticalDelta);
            cam.Direction = Vector3.Transform(cam.Direction, m);

            // Shot
            if (ms.LeftButton == ButtonState.Pressed && lastButtonState == ButtonState.Released)
            {
                bool shotConnected = false;
                
                foreach (var block in blocks)
                {
                    if (block.ShotCollision(cam.Position, cam.Direction))
                    {
                        score += 100;
                        shotConnected = true;
                        break;
                    }
                }

                if (!shotConnected)
                {
                    score -= 100;
                }
            }
            lastButtonState = ms.LeftButton;

            // Spawn / despawn blocks
            if (cam.Position.Z > blocks[1].GetPosition().Z + 2f)
            {
                // Despawn block
                blocks.RemoveAt(0);

                // Spawn new block
                lastColour = new Vector3(MathHelper.Clamp(lastColour.X + ((float)rnd.NextDouble() * 2 - 1) * colourDelta, 0, 1),
                    MathHelper.Clamp(lastColour.Y + ((float)rnd.NextDouble() * 2 - 1) * colourDelta, 0, 1),
                    MathHelper.Clamp(lastColour.Z + ((float)rnd.NextDouble() * 2 - 1) * colourDelta, 0, 1));

                int blockType = rnd.Next(5);

                if (blockType == 0)
                {
                    bool goRight = rnd.Next(10) < 5;

                    blocks.Add(new DiagonalBlock(blocks[blocks.Count - 1].GetNextPosition(), lastColour, goRight, textures, GraphicsDevice));
                }
                else
                {
                    bool createObstacle = rnd.Next(5) > 1;
                    bool createTarget = rnd.Next(5) == 0;

                    blocks.Add(new ForwardBlock(blocks[blocks.Count - 1].GetNextPosition(), lastColour, createObstacle, createTarget, textures, GraphicsDevice));
                }
            }

            // Move fliers
            for (int i = 0; i < fliers.Count; i++)
            {
                fliers[i].Step(gameTime.ElapsedGameTime.Milliseconds);
            }

            // Despawn fliers
            for (int i = fliers.Count - 1; i >= 0; i--)
            {
                if (fliers[i].IsFarAway(cam.Position))
                {
                    fliers.RemoveAt(i);
                }
            }

            // Spawn fliers
            int fliersToSpawn = MathHelper.Max(minFlierNumber - fliers.Count, rnd.Next(maxFlierNumber - fliers.Count));
            for (int i = 0; i < fliersToSpawn; i++)
            {
                fliers.Add(new Flier(cam.Position, playerSpeed, GraphicsDevice));
            }

            // Collision detection and game over
            foreach (var block in blocks)
            {
                if (cam.Position.Z >= block.GetPosition().Z && cam.Position.Z <= (block.GetPosition().Z + 4))
                {
                    if (block.Collision(cam.Position))
                    {
                        isGameOver = true;
                        break;
                    }
                }
            }

            // Speed++
            playerSpeed = 0.005f + 0.001f * (int)gameTime.TotalGameTime.TotalMinutes;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (Window.ClientBounds.Width != GraphicsDevice.Viewport.Width ||
                Window.ClientBounds.Height != GraphicsDevice.Viewport.Height)
            {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();

                cam.AspectRatio = (float)Window.ClientBounds.Width / Window.ClientBounds.Height;
            }

            GraphicsDevice.Clear(Color.Black);

            // SpriteBatch messes these up, so we set them correctly
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            foreach (var block in blocks)
            {
                block.Draw(cam);
            }

            foreach (var flier in fliers)
            {
                flier.Draw(cam);
            }

            // Draw score and crosshair
            spriteBatch.Begin();
            spriteBatch.DrawString(scoreFont, "Score: " + ((int)score).ToString(), new Vector2(7, 0), Color.White);
            spriteBatch.Draw(textures["crosshair"],
                new Vector2(GraphicsDevice.Viewport.Width / 2f - 8, GraphicsDevice.Viewport.Height / 2f - 8),
                Color.White);
            if (isGameOver)
                spriteBatch.DrawString(scoreFont, "Game Over", new Vector2(7, 22), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
