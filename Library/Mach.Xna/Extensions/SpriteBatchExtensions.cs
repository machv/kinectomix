using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="SpriteBatch" /> class.
    /// </summary>
    public static class SpriteBatchExtensions
    {

        /// <summary>
        /// Renders the string with shadow.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="spriteFont">The font for text.</param>
        /// <param name="text">The text to render.</param>
        /// <param name="position">Position of the text on the screen.</param>
        /// <param name="color">Foreground color.</param>
        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(spriteFont, text, new Vector2(position.X + 1, position.Y + 1), Color.Black * 0.8f);
            spriteBatch.DrawString(spriteFont, text, position, color);
        }
    }
}
