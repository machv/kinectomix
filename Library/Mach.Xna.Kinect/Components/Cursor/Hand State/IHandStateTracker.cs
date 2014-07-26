using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Tracks status of the active hand.
    /// </summary>
    public interface IHandStateTracker
    {
        /// <summary>
        /// Gets if tracked hand state is active.
        /// </summary>
        /// <returns>True if tracked hand state is active.</returns>
        bool IsStateActive { get; }
        /// <summary>
        /// Processes depth image frame.
        /// </summary>
        /// <param name="depthFrame">Depth frame to process.</param>
        void ProcessDepthData(DepthImageFrame depthFrame);
        /// <summary>
        /// Processes skeleton frame.
        /// </summary>
        /// <param name="frame">Skeleton frame to process.</param>
        void ProcessSkeletonData(SkeletonFrame frame);
        /// <summary>
        /// Updates status of the tracked state.
        /// </summary>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="cursorPosition">Current position of the cursor.</param>
        void Update(bool leftHanded, Vector2 cursorPosition);
        /// <summary>
        /// Draws debugging information about tracking progress.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <param name="spriteBatch"><see cref="SpriteBatch"/> used for drawing.</param>
        /// <param name="font">Font for drawing debug information.</param>
        /// <param name="scale">Scale for displayed information.</param>
        /// <param name="renderOffset">Offset for displayed information.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset);
    }
}
