using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Kinectomix.Logic.Gestures;
using System.Xml.Serialization;
using System.IO;

namespace Atomix.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GestureProcessing : GameComponent
    {
        private Skeletons _skeletons;
        private Recognizer _recognizer;
        private string _gesturesLocation;
        private RecognizedGesture _recognizedGesture;

        public GestureProcessing(Game game, Skeletons skeletons, string gesturesLocation)
            : base(game)
        {
            _skeletons = skeletons;
            _recognizer = new Recognizer();
            _gesturesLocation = gesturesLocation;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Gesture));

            string[] gestures = new string[] {
                "LeftHandUp.gst",
                "RightHandUp.gst",
            };
            foreach (string gestureName in gestures)
            {
                using (Stream stream = TitleContainer.OpenStream(_gesturesLocation + gestureName))
                {
                    Gesture gesture = seralizer.Deserialize(stream) as Gesture;

                    _recognizer.AddGesture(gesture);
                }
            }

            _recognizer.MinimalBufferLength = 35;
            _recognizer.Start();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            _recognizer.ProcessSkeleton(_skeletons.TrackedSkeleton);

            if (_recognizer.RecognizedGesture != null)
            {
                _recognizedGesture = _recognizer.RecognizedGesture;
            }

            base.Update(gameTime);
        }
    }
}
