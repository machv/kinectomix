using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atomix
{
    public interface IHandStateTracker
    {
        void ProcessDepthData(DepthImageFrame depthFrame);
        void ProcessSkeletonData(SkeletonFrame frame);
        void Update(bool leftHanded);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, float scale, Vector2 renderOffset);
    }
}
