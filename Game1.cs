using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;

namespace Hedgehog
{
    public class Game1 : Game
    {
        const int SCREENWIDTH = 1920, SCREENHEIGHT = 1080;
        const bool FULLSCREEN = true;
        private GraphicsDeviceManager graphics;
        PresentationParameters pp;
        private SpriteBatch spriteBatch;
        static public int screenW, screenH;
        static public Vector2 screenCenter;

        //INPUT
        Input inp;

        //TEXTURES
        Texture2D farBackground, midBackground;
        Texture2D tilesImages;

        //SOURCE RECTANGLES
        Rectangle screenRect, desktopRect;

        //POSITIONS
        static public Vector2 backGroundPos;

        //RENDERTARGETS
        RenderTarget2D MainTarget;

        public Game1()
        {
            int initialScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 10; // -10 убрать по завершению
            int initialScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 10;


            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = initialScreenWidth,
                PreferredBackBufferHeight = initialScreenHeight,
                IsFullScreen = FULLSCREEN, PreferredDepthStencilFormat = DepthFormat.Depth16
            };

            Window.IsBorderless = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MainTarget = new RenderTarget2D(GraphicsDevice, SCREENWIDTH, SCREENHEIGHT);
            pp = GraphicsDevice.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            screenW = MainTarget.Width;
            screenH = MainTarget.Height;
            desktopRect = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            screenRect = new Rectangle(0, 0, screenW, screenH);
            screenCenter = new Vector2(screenW / 2.0f, screenH / 2.0f) - new Vector2(32f, 32f);

            inp = new Input();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //LOAD GRAPHICS
            farBackground = Content.Load<Texture2D>("back");
            midBackground = Content.Load<Texture2D>("midBackground");
            //tilesImages = Content.Load<Texture2D>("tiles1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            inp.Update();

            if (inp.Keypress(Keys.Escape)) Exit();

            if (inp.KeyDown(Keys.Left)) backGroundPos.X++;
            if (inp.KeyDown(Keys.Right)) backGroundPos.X--;
            if (inp.KeyDown(Keys.Down)) backGroundPos.Y--;
            if (inp.KeyDown(Keys.Up)) backGroundPos.Y++;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(MainTarget);

            //DRAW OPAQUE FAR BACKGROUND
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap);
            spriteBatch.Draw(farBackground, screenRect, new Rectangle((int)(-backGroundPos.X * 0.5f), 0, farBackground.Width, farBackground.Height), Color.White);
            spriteBatch.End();
            //DRAF MID BACKGROUND(S)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap);
            spriteBatch.Draw(midBackground, screenRect, new Rectangle((int)(-backGroundPos.X), (int)(-backGroundPos.Y), farBackground.Width, farBackground.Height), Color.White);
            spriteBatch.End();
            //DRAW MAINTARGET TO BACKBUFFER
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(MainTarget, desktopRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}