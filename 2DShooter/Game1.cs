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
            }

            players[0].Position = new Vector2(172, 233);
            players[1].Position = new Vector2(325, 255);
            players[2].Position = new Vector2(486, 433);
            players[3].Position = new Vector2(642, 197);

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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");

            backgroundTexture = Content.Load<Texture2D>("background");
            foregroundTexture = Content.Load<Texture2D>("foreground");

            carriageTexture = Content.Load<Texture2D>("carriage");
            cannonTexture = Content.Load<Texture2D>("cannon");
            rocketTexture = Content.Load<Texture2D>("rocket");
            smokeTexture = Content.Load<Texture2D>("smoke");

            SetUpPlayers();
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
                spriteBatch.Draw(rocketTexture, rocketPosition, null, players[currentPlayer].Color, rocketAngle, new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
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
