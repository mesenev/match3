using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace match
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
    {

        const int TOTAL_SECONDS = 60;
        public const int INDENT = 152;
        public const int WIDTH = 90;
        public const int GAMEFIELD = 720;
        public const float GLOBAL_SPEED = 1f;
        private GameProcess GameProcess = new GameProcess ();

        GraphicsDeviceManager Graphics;
        SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D mainMenu;
        private Texture2D buttonTextureStartNewGame;
        private Texture2D buttonTextureStartNewGameHover;
        private Texture2D buttonNewgame;
        private Texture2D buttonStartString;
        private Texture2D meteor;
        private SpriteFont Font;
        private SpriteFont Font2;

        private Rectangle button_back = new Rectangle (362, 437, 300, 100);
        private Rectangle button_back_score = new Rectangle (INDENT + 102, 800, 500, 150);

        private Rectangle playArea = new Rectangle (INDENT - 5, INDENT - 5, GAMEFIELD - 5, GAMEFIELD - 5);
        private List<Texture2D> TexturesElements = new List <Texture2D> ();

        private static int NominalWidth = 1024;
        private static int NominalHeight = 1024;

        // Strings
        public string strScore = "Score for this round  ";
        public string strEstTime = "Seconds left: ";
        public string mainMenuString = "MAIN MENU";
        public string strScoreAmount = "";

        // Mouse declaration
        private Vector2 cursorPos;
        private Texture2D cursorTex;
        private MouseState mouseState = new MouseState ();



        public int[] getCursorPos ()
        {
            var cursorX = mouseState.X;
            var cursorY = mouseState.Y;

            int[] mousePos = { cursorX, cursorY };
            return mousePos;
        }


        public Game1 ()
        {
            Graphics = new GraphicsDeviceManager (this);
            // установка параметров экрана

            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 1024;
            Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;


		}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize ()
        {
            base.Initialize ();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent ()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch (GraphicsDevice);
            Content.RootDirectory = "Content";

            base.LoadContent ();

            cursorTex = Content.Load<Texture2D> ("cursor2");
            background = Content.Load<Texture2D> ("background");
		    mainMenu =  Content.Load<Texture2D> ("temple");
			meteor = Content.Load<Texture2D> ("meteor");
			Font = Content.Load<SpriteFont>("Hexa24");
			Font2 = Content.Load<SpriteFont>("Hexa36");

			buttonStartString = Content.Load<Texture2D> ("START");
            buttonTextureStartNewGame = Content.Load<Texture2D> ("button_green");
            buttonTextureStartNewGameHover = Content.Load<Texture2D> ("button_green_over");

            for (int _ = 1; _ < 8; _++)
                TexturesElements.Add (Content.Load<Texture2D> ("0" + _));
            TexturesElements.Add (Content.Load<Texture2D> ("invoker"));
            TexturesElements.Add (Content.Load<Texture2D> ("techies"));

            //TODO: use this.Content to load your game content here 
        }


        public void DrawRectangle (Rectangle coords, Color color)
        {
            var rect = new Texture2D (GraphicsDevice, 1, 1);
            rect.SetData (new[] { color });
            spriteBatch.Draw (rect, coords, color);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update (GameTime gameTime)
        {
            base.Update (gameTime);
            if (GameProcess.IsGame)
                GameProcess.startedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GameProcess.startedTime > TOTAL_SECONDS) {
                GameProcess.FinishGame ();
            }
            var lastMouseState = mouseState;
            mouseState = Mouse.GetState ();
            cursorPos = new Vector2 (mouseState.X, mouseState.Y);

            if (GameProcess.mainMenu) {
                buttonNewgame = button_back.Contains (cursorPos) ? buttonTextureStartNewGame : buttonTextureStartNewGameHover;
                        
            } else if (GameProcess.IsGame) {
                Element.AnimationByMouseMovement (mouseState.X, mouseState.Y);
                Element.CountMouseAnimation ();

            } else if (GameProcess.scoreMenu) {
                buttonNewgame = button_back_score.Contains (cursorPos) ? buttonTextureStartNewGameHover : buttonTextureStartNewGame;
            }

            foreach (var b in GameProcess.Boulders.ToArray ())
                Element.markToDestroy (b.GetXcoord (), b.GetYcoord ());

            // Get the mouse state relevant for this frame
            // Recognize a single click of the left mouse button
            if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed) {
                Console.WriteLine ($"LMB was pressed in ({mouseState.X},{mouseState.Y})");

                if (GameProcess.mainMenu) {
                    if (button_back.Contains (cursorPos))
                        GameProcess.NewGame ();
                } else if (GameProcess.IsGame) {
                    if (playArea.Contains (cursorPos))
                        Element.CursorClickHandler (cursorPos);
                } else if (GameProcess.scoreMenu) {
                    if (button_back_score.Contains (cursorPos))
                        GameProcess.Restart ();
                }
            }


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw (GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
            //TODO: Add your drawing code here


            spriteBatch.Begin ();



            if (GameProcess.mainMenu) {

                spriteBatch.Draw (mainMenu, new Rectangle (0, 0, 1024, 1024), Color.White);
                spriteBatch.Draw (buttonNewgame, button_back, Color.White);
                spriteBatch.Draw (buttonStartString, new Rectangle (422, 457, 180, 40), Color.White);

            } else if (GameProcess.scoreMenu) {
                spriteBatch.Draw (background, new Rectangle (0, 0, 1024, 1024), Color.White);

                DrawRectangle (playArea, Color.Black * 0.7f);
                int i = 0;
                foreach (var s in GameProcess.Score) {
                    spriteBatch.Draw (TexturesElements [i],
                        new Vector2 (INDENT + 30, 180 + WIDTH * i), //position
                        null, //source rectangle
                        Color.White,
                        0, //rotation
                        new Vector2 (0, 0), //origin
                        1,
                        SpriteEffects.None,
                        1);
                    spriteBatch.DrawString (Font,
                        GameProcess.Score [i].ToString (),
                        new Vector2 (INDENT + 102, 200 + WIDTH * i), Color.White
                        , 0,
                        new Vector2 (0, 0), 1, SpriteEffects.None, 0);
                    i++;

                }
                spriteBatch.DrawString (Font,
                    strScore + GameProcess.TotalScore,
                    new Vector2 (INDENT + 35, GAMEFIELD), Color.White
                    , 0, new Vector2 (0, 0), 1, SpriteEffects.None, 0);
                
                spriteBatch.Draw (buttonNewgame, button_back_score, Color.White);

                spriteBatch.DrawString (Font2, mainMenuString, new Vector2 (INDENT + 202, GAMEFIELD + 117), Color.White);

                
            } else if (GameProcess.IsGame) {

                spriteBatch.Draw (background, new Rectangle (0, 0, 1024, 1024), Color.White);

                spriteBatch.DrawString (Font,
                    strEstTime + (TOTAL_SECONDS - (int)GameProcess.startedTime),
                    new Vector2 (20, 10), Color.White
                    , 0,
                    new Vector2 (0, 0), 1, SpriteEffects.None, 0);

                DrawRectangle (playArea, Color.Black * 0.7f);

                foreach (var e in GameProcess.ListElements.ToArray ()) {
                    foreach (var u in e.ToArray ()) {
                        spriteBatch.Draw (
                            TexturesElements [(u.Bonus > 0) ? ((u.Bonus == 1) ? 7 : 8) : u.TypeOf]
                            , new Rectangle (u.getXcoord (), u.getYcoord (), (int)Math.Round ((WIDTH - 20) * u.Rate), (int)Math.Round ((WIDTH - 20) * u.Rate)) //dest rectangle
                            , null
                            , Color.White * u.Opacity
                            , u.TextureRotationAngle () //rotation
                            , new Vector2 ((WIDTH - 20) * u.XTextureRotationMultiplier, (WIDTH - 20) * u.YTextureRotationMultiplier) //new Vector2 (0, 0)//, 
                        , SpriteEffects.None
                        , 1
                        );
                    }
                }
                foreach (var b in GameProcess.Boulders.ToArray ()) {
                    spriteBatch.Draw (
                        meteor,
                        new Rectangle (b.GetXcoord (), b.GetYcoord (), Game1.WIDTH, Game1.WIDTH) //dest rectangle
                        , null
                        , Color.White
                        , b.Rotation
                        , new Vector2 (0, 0) //origin
                        , SpriteEffects.None
                        , 1
                    );
                }
            } 
            spriteBatch.Draw (cursorTex, cursorPos, Color.White);
            spriteBatch.End ();

            base.Draw (gameTime);

        }
    }
}

