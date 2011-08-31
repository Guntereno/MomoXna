using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Momo.Core.ObjectPools;

namespace TestGame.Input
{
    public class InputManager
    {
        public readonly int kMaxInputs = 4;

        Pool<InputWrapper> m_wrappers = null;

        public InputWrapper GetInputWrapper(int idx) { return m_wrappers[idx]; }

        public InputManager()
        {
            m_wrappers =  new Pool<InputWrapper>(kMaxInputs);

            InputWrapper inputWrapper = new InputWrapper();
            inputWrapper.Init();

            m_wrappers.AddItem(inputWrapper, true);
        }

        public void Update(ref FrameTime frameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            for (int i = 0; i < m_wrappers.ActiveItemListCount; ++i)
            {
                GetInputWrapper(i).Update(gamePadState, keyboardState);
            }
        }
    }
}
