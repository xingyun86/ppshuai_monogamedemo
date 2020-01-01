#region Using Statements
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

#endregion

namespace MonoGame.Shared
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont fpsFont;
        private readonly FrameCounter frameCounter = new FrameCounter();

        private Texture2D _backgroundTexture;
        private Texture2D _ballTexture;
        private Texture2D _goalkeeperTexture;
        private int _screenWidth;
        private int _screenHeight;
        private Rectangle _backgroundRectangle;
        private Vector2 _initialBallPosition;
        private Vector2 _ballPosition;
        private Rectangle _ballRectangle;
        private double _goalLinePosition;
        private bool _isBallMoving;
        private Vector2 _ballVelocity;
        private bool _isBallHit;
        private TimeSpan _startMovement;
        private Rectangle _goalkeeperRectangle;
        private int _goalKeeperWidth;
        private int _goalKeeperHeight;
        private int _goalkeeperPositionX;
        private int _goalkeeperPositionY;
        private double _aCoef;
        private double _deltaCoef;
        private SpriteFont _soccerFont;
        private String _scoreText = "";
        private int _scorePosition;
        private int _userScore;
        private int _computerScore;
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            TouchPanel.EnableMouseTouchPoint = true;

            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            TouchPanel.EnabledGestures = GestureType.Flick | GestureType.FreeDrag;
        }

        private void ResetWindowSize()
        {
            _screenWidth = Window.ClientBounds.Width;
            _screenHeight = Window.ClientBounds.Height;
            _backgroundRectangle = new Rectangle(0, 0, _screenWidth, _screenHeight);
            _initialBallPosition = new Vector2(_screenWidth / 2.0f, _screenHeight * 0.8f);
            var ballDimension = (_screenWidth > _screenHeight) ?
                (int)(_screenWidth * 0.02) :
                (int)(_screenHeight * 0.035);
            _ballPosition.X = (int)_initialBallPosition.X;
            _ballPosition.Y = (int)_initialBallPosition.Y;
            _ballRectangle = new Rectangle((int)_initialBallPosition.X, (int)_initialBallPosition.Y,
                ballDimension, ballDimension);

            _goalLinePosition = _screenHeight * 0.05;

            _goalKeeperWidth = (int)(_screenWidth * 0.05);
            _goalKeeperHeight = (int)(_screenWidth * 0.005);
            _goalkeeperPositionX = (_screenWidth - _goalKeeperWidth)/2;
            _goalkeeperPositionY = (int)(_screenHeight * 0.12);
            
            _scorePosition = (int)((_screenWidth) / 2.0);
        }

        private void ResetGame()
        {
            _ballPosition = new Vector2(_initialBallPosition.X, _initialBallPosition.Y);
            _goalkeeperPositionX = (_screenWidth - _goalKeeperWidth) / 2;
            _isBallMoving = false;
            _isBallHit = false;
            _scoreText = string.Format("{0} x {1}", _userScore, _computerScore);
            var scoreSize = _soccerFont.MeasureString(_scoreText);
            _scorePosition = (int)((_screenWidth - scoreSize.X) / 2.0);
            while (TouchPanel.IsGestureAvailable)
            {
                TouchPanel.ReadGesture();
            }
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
            ResetWindowSize();
            Window.ClientSizeChanged += (s, e) => ResetWindowSize();
            TouchPanel.EnabledGestures = GestureType.Flick;

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

            //TODO: use this.Content to load your game content here 
            fpsFont = this.Content.Load<SpriteFont>("fpsfont");

            _backgroundTexture = Content.Load<Texture2D>("SoccerField");
            _ballTexture = Content.Load<Texture2D>("SoccerBall");

            _goalkeeperTexture = Content.Load<Texture2D>("Goalkeeper");
            _soccerFont = this.Content.Load<SpriteFont>("soccerfont");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
#endif
            // TODO: Add your update logic here	

            /*_ballPosition.X -= 0.5f;
            _ballPosition.Y -= 3;
            if (_ballPosition.Y < _goalLinePosition)
                _ballPosition = new Vector2(_initialBallPosition.X, _initialBallPosition.Y);
            _ballRectangle.X = (int)_ballPosition.X;
            _ballRectangle.Y = (int)_ballPosition.Y;*/

            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
            {
                var touchPoint = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                var hitRectangle = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, _ballTexture.Width,
                    _ballTexture.Height);
                hitRectangle.Inflate(20, 20);
                _isBallHit = hitRectangle.Contains(touchPoint);
                if (!_isBallMoving && TouchPanel.IsGestureAvailable)
                {
                    // Read the next gesture    
                    GestureSample gesture = TouchPanel.ReadGesture();
                    if (gesture.GestureType == GestureType.Flick && _isBallHit)
                    {
                        _isBallMoving = true;
                        _isBallHit = false;
                        _startMovement = gameTime.TotalGameTime;
                        _ballVelocity = gesture.Delta * (float)TargetElapsedTime.TotalSeconds / 5.0f;
                        var rnd = new Random();
                        _aCoef = rnd.NextDouble() * 0.005;
                        _deltaCoef = rnd.NextDouble() * Math.PI / 2;
                    }
                }
            }
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                var mousePoint = new Point((int)mouse.Position.X, (int)mouse.Position.Y);
                var hitRectangle = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, _ballTexture.Width,
                    _ballTexture.Height);
                hitRectangle.Inflate(100, 20);
                _isBallHit = hitRectangle.Contains(mousePoint);
                if (_isBallHit)
                {
                    _isBallMoving = true;
                    _isBallHit = false;
                    _startMovement = gameTime.TotalGameTime;
                    Vector2 delta;
                    delta.X = (mouse.Position.X - hitRectangle.X - 100) * 10;
                    delta.Y = -900;
                    _ballVelocity = delta * (float)TargetElapsedTime.TotalSeconds / 5.0f;
                    var rnd = new Random();
                    _aCoef = rnd.NextDouble() * 0.005;
                    _deltaCoef = rnd.NextDouble() * Math.PI / 2;
                }
            }
            if (_isBallMoving)
            {
                _ballPosition += _ballVelocity;
                var timeInMovement = (gameTime.TotalGameTime - _startMovement).TotalSeconds;
                var isTimeout = timeInMovement > 5.0;
                // reached goal line
                if (_ballPosition.Y < _goalLinePosition || isTimeout)
                {
                    _ballPosition = new Vector2(_initialBallPosition.X, _initialBallPosition.Y);
                    _isBallMoving = false;
                    _isBallHit = false;
                    while (TouchPanel.IsGestureAvailable)
                    {
                        TouchPanel.ReadGesture();
                    }
                    bool isGoal = !isTimeout &&
                                    (_ballPosition.X > _screenWidth * 0.375) &&
                                    (_ballPosition.X < _screenWidth * 0.623);
                    if (isGoal)
                    {
                        _userScore++;
                    }
                    else
                    {
                        _computerScore++;
                    }
                    ResetGame();
                }
                _ballRectangle.X = (int)_ballPosition.X;
                _ballRectangle.Y = (int)_ballPosition.Y;
                _goalkeeperPositionX = (int)((_screenWidth * 0.11) *
                      Math.Sin(_aCoef * gameTime.TotalGameTime.TotalMilliseconds +
                      _deltaCoef) + (_screenWidth * 0.75) / 2.0 + _screenWidth * 0.11);
                _goalkeeperRectangle = new Rectangle(_goalkeeperPositionX, _goalkeeperPositionY,
                    _goalKeeperWidth, _goalKeeperHeight);
                if (_goalkeeperRectangle.Intersects(_ballRectangle))
                {
                    _computerScore++;
                    ResetGame();
                }

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //TODO: Add your drawing code here
            frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            var fps = string.Format("FPS: ({0})", frameCounter.AverageFramesPerSecond);

            // Set the position for the background    
            var screenWidth = Window.ClientBounds.Width;
            var screenHeight = Window.ClientBounds.Height;
            var rectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            // TODO: Add your drawing code here
            // We'll start all of our drawing here:
            spriteBatch.Begin();

            //////////////////////////////////////////////////////////////////////

            // Draw the background    
            spriteBatch.Draw(_backgroundTexture, rectangle, Color.White);
            // Draw the ball
            spriteBatch.Draw(_ballTexture, _ballRectangle, Color.White);
            // Draw the goalkeeper
            spriteBatch.Draw(_goalkeeperTexture, _goalkeeperRectangle, Color.White);
            // Draw the score
            spriteBatch.DrawString(_soccerFont, _scoreText,
                 new Vector2(_scorePosition, _screenHeight * 0.9f), Color.White);
            //////////////////////////////////////////////////////////////////////

            // FPS
            spriteBatch.DrawString(fpsFont, fps, Vector2.One, Color.Blue);

            // End renders all sprites to the screen:
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
