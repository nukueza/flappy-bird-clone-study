using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

public class BatuBata
{
  private Texture2D _texture;
  public Vector2 Position { get; set; }
  public int Width { get; }
  public int Height { get; }

  private const float GAP_SIZE = 50;
  private float _topPipeY;
  private float _bottomPipeY;

  public BatuBata(ContentManager content, float startX, float gapCenterY)
  {
    _texture = content.Load<Texture2D>("Images/pipe_ds");
    Width = _texture.Width;
    Height = _texture.Height;

    Position = new Vector2(startX, Position.Y);
    _topPipeY = gapCenterY - (GAP_SIZE / 2f) - _texture.Height;
    _bottomPipeY = gapCenterY + (GAP_SIZE / 2f);
  }

  public void Update(float speed, float dt)
  {
    Position = new Vector2(Position.X - (speed * dt), Position.Y);
  }

  public void Draw(SpriteBatch spriteBatch)
  {
    spriteBatch.Draw(
        _texture,
        new Vector2(Position.X, _topPipeY),
        null,
        Color.White,
        0f,
        Vector2.Zero,
        1f,
        SpriteEffects.FlipVertically,
        0f
        );
    spriteBatch.Draw(_texture, new Vector2(Position.X, _bottomPipeY), Color.White);
  }
}

