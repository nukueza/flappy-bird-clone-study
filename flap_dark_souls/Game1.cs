using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace flap_dark_souls;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;

  // texture
  private Texture2D _bg;
  private Texture2D _ground;

  private Bird _bird;

  // scroll
  private float _bgScroll = 0;
  private float _groundScroll = 0;
  private const float BG_SPEED = 40;
  private const float GROUND_SPEED = 80;

  // render target
  private RenderTarget2D _renderer;
  private const int VW = 320;
  private const int VH = 180;
  private const int CW = 1280;
  private const int CH = 720;

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;

    _graphics.PreferredBackBufferWidth = CW;
    _graphics.PreferredBackBufferHeight = CH;
    _graphics.ApplyChanges();
  }

  protected override void Initialize()
  {
    TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 120.0);
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _renderer = new RenderTarget2D(GraphicsDevice, VW, VH);
    _bg = Content.Load<Texture2D>("Images/bg_ds");
    _ground = Content.Load<Texture2D>("Images/ground_ds");
    _bird = new Bird(Content);
    _bird.Position = new Vector2(VW / 2 - _bird.Width / 2, VH / 2 - _bird.Height / 2);
  }

  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();

    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
    _bgScroll += BG_SPEED * dt;
    if (_bgScroll >= _bg.Width)
      _bgScroll -= _bg.Width;

    _groundScroll += GROUND_SPEED * dt;
    if (_groundScroll >= _ground.Width)
      _groundScroll -= _ground.Width;

    _bird.Update(dt);


    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.SetRenderTarget(_renderer);
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    _spriteBatch.Draw(_bg, new Vector2((int)MathF.Floor(-_bgScroll), 0), Color.White);
    _spriteBatch.Draw(_bg, new Vector2((int)MathF.Floor(-_bgScroll + _bg.Width), 0), Color.White);
    _spriteBatch.Draw(_ground, new Vector2((int)MathF.Floor(-_groundScroll), VH - _ground.Height), Color.White);
    _spriteBatch.Draw(_ground, new Vector2((int)MathF.Floor(-_groundScroll + _ground.Width), VH - _ground.Height), Color.White);
    _bird.Draw(_spriteBatch);
    _spriteBatch.End();

    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _spriteBatch.Draw(_renderer, new Rectangle(0, 0, CW, CH), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}
