using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atomix
{
    public interface IHandStateTracker
    {
        bool IsStateActive { get; }
        void ProcessDepthData(DepthImageFrame depthFrame);
        void ProcessSkeletonData(SkeletonFrame frame);
        void Update(bool leftHanded, Vector2 cursorPosition);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset);
    }
}
