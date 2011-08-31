using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TestGame.Input
{
    public class InputManager
    {
        private InputWrapper m_inputWrapper = new InputWrapper();

        public InputWrapper GetInputWrapper() { return m_inputWrapper; }

        public Input.InputWrapper InputWrapper
        {
            get { return m_inputWrapper; }
        }

        public void Update(ref FrameTime frameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            m_inputWrapper.Update(gamePadState, keyboardState);
        }
    }
}
