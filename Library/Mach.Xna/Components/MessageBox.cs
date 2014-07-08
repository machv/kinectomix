using Mach.Xna.Input;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Components
{
    public class MessageBox : MessageBoxBase
    {
        public MessageBox(Game game, IInputProvider inputProvider)
            : base (game, inputProvider)
        {

        }

        public override void Initialize()
        {
            _buttonOk = new Button(Game, "OK") { Tag = MessageBoxResult.OK, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonCancel = new Button(Game, "Cancel") { Tag = MessageBoxResult.Cancel, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonYes = new Button(Game, "Yes") { Tag = MessageBoxResult.Yes, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonNo = new Button(Game, "No") { Tag = MessageBoxResult.No, Width = ButtonsWidth, Height = ButtonsHeight, InputProvider = InputProvider, Background = Color.DarkGray, BorderColor = Color.White };

            base.Initialize();
        }
    }
}
