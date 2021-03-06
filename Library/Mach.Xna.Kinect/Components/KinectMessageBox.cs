using Microsoft.Xna.Framework;
using Mach.Xna.Input;
using Mach.Xna.Components;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KinectMessageBox : MessageBoxBase
    {
        private KinectCursor _cursor;

        static KinectMessageBox()
        {
            Localization.KinectMessageBoxResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectMessageBox" /> component.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="inputProvider">The input provider.</param>
        /// <param name="cursor">Kinect cursor used in game.</param>
        public KinectMessageBox(Game game, IInputProvider inputProvider, KinectCursor cursor)
            : base(game, inputProvider)
        {
            _cursor = cursor;
        }

        /// <summary>
        /// Prepares default buttons.
        /// </summary>
        public override void Initialize()
        {
            _buttonOk = new KinectButton(Game, _cursor, Localization.KinectMessageBoxResources.OK) { Tag = MessageBoxResult.OK, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonCancel = new KinectButton(Game, _cursor, Localization.KinectMessageBoxResources.Cancel) { Tag = MessageBoxResult.Cancel, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonYes = new KinectButton(Game, _cursor, Localization.KinectMessageBoxResources.Yes) { Tag = MessageBoxResult.Yes, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonNo = new KinectButton(Game, _cursor, Localization.KinectMessageBoxResources.No) { Tag = MessageBoxResult.No, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };

            base.Initialize();
        }
    }
}
