using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

public class Bird
{
  private Texture2D _texture;
  public Vector2 Position { get; set; }
  public int Width { get; }
  public int Height { get; }
  private Vector2 Velocity;
  private const float GRAVITASY = 980f;
  private const float JUMP_UP = -250;
  private KeyboardState _passKey;

  public Color Color { get; set; } = Color.White;

  public Rectangle Bounds => new Rectangle(
      (int)Position.X,
      (int)Position.Y,
      Width,
      Height
      );

  public Bird(ContentManager content)
  {
    _texture = content.Load<Texture2D>("Images/bird_ds");
    Width = _texture.Width;
    Height = _texture.Height;
  }

  public void Update(float dt)
  {
    KeyboardState currentKey = Keyboard.GetState();
    if (currentKey.IsKeyDown(Keys.Space) && _passKey.IsKeyUp(Keys.Space))
    {
      Velocity.Y = JUMP_UP;
    }

    Velocity.Y += GRAVITASY * dt;
    Position += Velocity * dt;

    Position = new Vector2(Position.X, Math.Clamp(Position.Y, 0, 180 - _texture.Height));

    _passKey = currentKey;
  }

  public void Reset()
  {
    var pos = new Vector2(320 / 2 - _texture.Width / 2, 180 / 2 - _texture.Height / 2);
    Position = pos;
    Velocity = Vector2.Zero;
  }

  public void Draw(SpriteBatch spriteBatch)
  {
    var pos = new Vector2((int)MathF.Floor(Position.X), (int)MathF.Floor(Position.Y));
    spriteBatch.Draw(_texture, pos, Color);
  }
}
