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
        
        PlayerData[] players;
        int numOfPlayers = 4;

        float playersScale;

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

            playersScale = 70.0f / carriageTexture.Width;
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        private void DrawScene()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        private void DrawPlayers()
        {
            foreach(PlayerData player in players)
            {
                if(player.IsAlive)
                {
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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
