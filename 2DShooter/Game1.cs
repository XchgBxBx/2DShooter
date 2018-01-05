using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DShooter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public struct PlayerData
        {
            public Vector2 Position;
            public bool IsAlive;
            public Color Color;
            public float Angle;
            public float Power;
        }


        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        SpriteBatch spriteBatch;


        int screenWidth = 800;
        int screenHeight = 600;

        SpriteFont font;

        Texture2D backgroundTexture;
        Texture2D foregroundTexture;
        Texture2D groundTexture;

        Texture2D carriageTexture;
        Texture2D cannonTexture;
        Texture2D rocketTexture;
        Texture2D smokeTexture;

        PlayerData[] players;
        int numOfPlayers = 4;

        float playersScale;

        int currentPlayer = 0;

        bool rocketFlying = false;
        Vector2 rocketPosition;
        Vector2 rocketDirection;
        float rocketAngle;
        float rocketScaling = 0.1f;

        List<Vector2> smokeList = new List<Vector2>();
        Random randomizer = new Random();

        int[] terrainContour;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            device = graphics.GraphicsDevice;

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            graphics.IsFullScreen = false;
            Window.Title = "2D Shooter";

            Content.RootDirectory = "Content";
        }

        private void SetUpPlayers()
        {
            Color[] playersColors = new Color[10];

            playersColors[0] = Color.Red;
            playersColors[1] = Color.Green;
            playersColors[2] = Color.Blue;
            playersColors[3] = Color.Purple;
            playersColors[4] = Color.Orange;
            playersColors[5] = Color.Indigo;
            playersColors[6] = Color.Yellow;
            playersColors[7] = Color.SaddleBrown;
            playersColors[8] = Color.Tomato;
            playersColors[9] = Color.Turquoise;

            players = new PlayerData[numOfPlayers];
            for(int i = 0; i < numOfPlayers; i++)
            {
                players[i].IsAlive = true;
                players[i].Color = playersColors[i];
                players[i].Angle = MathHelper.ToRadians(90);
                players[i].Power = 100;
                players[i].Position = new Vector2();
                players[i].Position.X = screenWidth / (numOfPlayers + 1) * (i + 1);
                players[i].Position.Y = terrainContour[(int)players[i].Position.X];
            }

            playersScale = 50.0f / carriageTexture.Width;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        private void GenerateContours()
        {
            terrainContour = new int[screenWidth];

            double rand1 = randomizer.NextDouble() + 1;
            double rand2 = randomizer.NextDouble() + 2;
            double rand3 = randomizer.NextDouble() + 3;

            float offset = screenHeight / 2;
            float peakHeight = 100;
            float flatness = 100;
            
            for (int x = 0; x < screenWidth; x++)
            {
                double height = peakHeight / rand1  * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakHeight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakHeight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        private void GenerateForeground()
        {
            Color[] foregroundColors = new Color[screenWidth * screenHeight];
            Color[,] groundColors = TextureTo2DArray(groundTexture);

            for (int x = 0; x < screenWidth; x++)
            {
                for(int y = 0; y < screenHeight; y++)
                {
                    if (y > terrainContour[x])
                        foregroundColors[x + y * screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    else
                        foregroundColors[x + y * screenWidth] = Color.Transparent;
                }
            }

            foregroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);
        }

        private void FlattenTerrainBelowPlayers()
        {
            foreach(PlayerData player in players)
            {
                if (player.IsAlive)
                    for (int x = 0; x < 55; x++)
                        terrainContour[(int)player.Position.X + x] = terrainContour[(int)player.Position.X];
            }
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1d = new Color[texture.Width * texture.Height];
            texture.GetData(colors1d);

            Color[,] colors2d = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2d[x, y] = colors1d[x + y * texture.Width];

            return colors2d;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            font = Content.Load<SpriteFont>("Font");

            backgroundTexture = Content.Load<Texture2D>("background");
            groundTexture = Content.Load<Texture2D>("ground");

            carriageTexture = Content.Load<Texture2D>("carriage");
            cannonTexture = Content.Load<Texture2D>("cannon");
            rocketTexture = Content.Load<Texture2D>("rocket");
            smokeTexture = Content.Load<Texture2D>("smoke");
            
            GenerateContours();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            GenerateForeground();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        private void ProcessKeyboard()
        {
            KeyboardState kbdState = Keyboard.GetState();

            if (kbdState.IsKeyDown(Keys.Left))
                players[currentPlayer].Angle -= 0.01f;
            if (kbdState.IsKeyDown(Keys.Right))
                players[currentPlayer].Angle += 0.01f;


            if (players[currentPlayer].Angle > MathHelper.PiOver2)
                players[currentPlayer].Angle = -MathHelper.PiOver2;
            if (players[currentPlayer].Angle < -MathHelper.PiOver2)
                players[currentPlayer].Angle = MathHelper.PiOver2;


            if (kbdState.IsKeyDown(Keys.Down))
                players[currentPlayer].Power -= 1;
            if (kbdState.IsKeyDown(Keys.Up))
                players[currentPlayer].Power += 1;
            if (kbdState.IsKeyDown(Keys.PageDown))
                players[currentPlayer].Power -= 20;
            if (kbdState.IsKeyDown(Keys.PageUp))
                players[currentPlayer].Power += 20;

            if (players[currentPlayer].Power > 1000)
                players[currentPlayer].Power = 1000;
            if (players[currentPlayer].Power < 0)
                players[currentPlayer].Power = 0;

            if(kbdState.IsKeyDown(Keys.Enter) || kbdState.IsKeyDown(Keys.Space))
            {
                rocketFlying = true;

                rocketPosition = players[currentPlayer].Position;
                rocketPosition.X += 27;
                rocketPosition.Y -= 11;
                rocketAngle = players[currentPlayer].Angle;

                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(rocketAngle);
                rocketDirection = Vector2.Transform(up, rotMatrix);
                rocketDirection *= players[currentPlayer].Power / 50.0f;
            }
        }

        private void UpdateRocket()
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                rocketDirection += gravity / 10.0f;
                rocketPosition += rocketDirection;
                rocketAngle = (float)Math.Atan2(rocketDirection.X, -rocketDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = rocketPosition;
                    smokePos.X += randomizer.Next(10) - 5;
                    smokePos.Y += randomizer.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ProcessKeyboard();

            UpdateRocket();

            base.Update(gameTime);
        }

        private void DrawText()
        {
            PlayerData player = players[currentPlayer];
            int currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            spriteBatch.DrawString(font, "Cannon Angle: " + currentAngle, new Vector2(20, 20), player.Color);
            spriteBatch.DrawString(font, "Cannon Power: " + player.Power, new Vector2(20, 45), player.Color);
        }

        private void DrawScene()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        private void DrawRocket()
        {
            if(rocketFlying)
            {
                spriteBatch.Draw(rocketTexture, rocketPosition, null, players[currentPlayer].Color, rocketAngle, new Vector2(42, 240), rocketScaling, SpriteEffects.None, 1);
            }
        }

        private void DrawSmoke()
        {
            foreach(Vector2 smokePos in smokeList)
            {
                spriteBatch.Draw(smokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
            }
        }

        private void DrawPlayers()
        {
            foreach(PlayerData player in players)
            {
                if(player.IsAlive)
                {
                    int xPos = (int)player.Position.X + 27;
                    int yPos = (int)player.Position.Y - 11;
                    Vector2 cannonOrigin = new Vector2(11, 50);

                    spriteBatch.Draw(cannonTexture, new Vector2(xPos, yPos), null, player.Color, player.Angle, cannonOrigin, playersScale, SpriteEffects.None, 1);

                    spriteBatch.Draw(carriageTexture, player.Position, null, player.Color, 0, new Vector2(0, carriageTexture.Height), playersScale, SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            DrawScene();
            DrawPlayers();
            DrawText();
            DrawRocket();
            DrawSmoke();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
