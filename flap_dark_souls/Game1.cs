using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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

  private List<BatuBata> _batuBata = new List<BatuBata>();
  private float _batuBataSpawnTimer = 0;
  private const float BATU_SPAWN_INTERVAL = 2.5f;
  private const float BATU_SPEED = 60f;
  private Random _rand = new();

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

  // game state
  private GameState _state = GameState.Start;

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

    switch (_state)
    {
      case GameState.Start:
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
          _state = GameState.Play;
        }
        break;
      case GameState.Play:
        Playing(dt);
        break;
      case GameState.GameOver:
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
          Reset();
          _state = GameState.Play;
        }
        break;
    }


    base.Update(gameTime);
  }

  private void Playing(float dt)
  {

    _bgScroll += BG_SPEED * dt;
    if (_bgScroll >= _bg.Width)
      _bgScroll -= _bg.Width;

    _groundScroll += GROUND_SPEED * dt;
    if (_groundScroll >= _ground.Width)
      _groundScroll -= _ground.Width;

    _batuBataSpawnTimer += dt;
    if (_batuBataSpawnTimer >= BATU_SPAWN_INTERVAL)
    {
      _batuBataSpawnTimer = 0f;
      int minGapY = 40;
      int maxGapY = VH - 60;
      float randomGapY = _rand.Next(minGapY, maxGapY);
      _batuBata.Add(new BatuBata(Content, VW, randomGapY));
    }

    for (int i = _batuBata.Count - 1; i >= 0; i--)
    {
      _batuBata[i].Update(BATU_SPEED, dt);
      if (_batuBata[i].Position.X + _batuBata[i].Width < 0 )
      {
        _batuBata.RemoveAt(i);
      }
    }

    _bird.Update(dt);

    if (CheckCollision())
    {
      _bird.Color = Color.Red;
      _state = GameState.GameOver;
    }
    else
    {
      _bird.Color = Color.White;
    }
  }

  private void Reset()
  {
    _bird.Reset();
    _batuBata.Clear();
    _groundScroll = 0;
    _bgScroll = 0;
    _batuBataSpawnTimer = 0;
  }

  private bool CheckCollision()
  {
    var groundBound = new Rectangle(
        0,
        VH - _ground.Height,
        _ground.Width,
        _ground.Height
        );

    if (_bird.Bounds.Intersects(groundBound)) return true;

    foreach (var batubata in _batuBata)
    {
      if (_bird.Bounds.Intersects(batubata.BottomPipe) || _bird.Bounds.Intersects(batubata.TopPipe))
      {
        return true;
      }
    }
    return false;
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
    foreach (var batuBata in _batuBata)
    {
      batuBata.Draw(_spriteBatch);
    }
    _spriteBatch.End();

    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    _spriteBatch.Draw(_renderer, new Rectangle(0, 0, CW, CH), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}

public enum GameState
{
  Start,
  Play,
  GameOver
}

