using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kinectomix
{
    public static class SpriteBatchExtensions
    {
        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + 1, position.Y + 1), Color.Black * 0.8f);
            spriteBatch.DrawString(spriteFont, text, position, color);
        }
    }
}
