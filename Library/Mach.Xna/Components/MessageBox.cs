using Mach.Xna.Input;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Components
{
    /// <summary>
    /// Message box component for displaying prompts.
    /// </summary>
    public class MessageBox : MessageBoxBase
    {
        static MessageBox()
        {
            Localization.MessageBoxResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="game">The game containing this component.</param>
        /// <param name="inputProvider">The input provider.</param>
        public MessageBox(Game game, IInputProvider inputProvider)
            : base (game, inputProvider)
        {

        }

        /// <summary>
        /// Initializes a message box component.
        /// </summary>
        public override void Initialize()
        {
            _buttonOk = new Button(Game, Localization.MessageBoxResources.OK) { Tag = MessageBoxResult.OK, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonCancel = new Button(Game, Localization.MessageBoxResources.Cancel) { Tag = MessageBoxResult.Cancel, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonYes = new Button(Game, Localization.MessageBoxResources.Yes) { Tag = MessageBoxResult.Yes, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonNo = new Button(Game, Localization.MessageBoxResources.No) { Tag = MessageBoxResult.No, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };

            base.Initialize();
        }
    }
}
