using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Momo.Core.ObjectPools;

namespace Game.Input
{
    public class InputManager
    {
        public readonly int kMaxInputs = 4;

        InputWrapper[] m_wrappers = null;

        public InputWrapper GetInputWrapper(int idx) { return m_wrappers[idx]; }

        public InputManager()
        {
            m_wrappers = new InputWrapper[kMaxInputs]; ;

            for (int i = 0; i < kMaxInputs; ++i)
            {
                InputWrapper inputWrapper = new InputWrapper();
                m_wrappers[i] = inputWrapper;
            }

            m_wrappers[0].Init();
        }

        public void Update(ref FrameTime frameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            for (int i = 0; i < kMaxInputs; ++i)
            {
                InputWrapper wrapper = GetInputWrapper(i);

                GamePadState gamePadState = GamePad.GetState((PlayerIndex)i);

                // Handle connection status
                if(gamePadState.IsConnected)
                {
                    if (!wrapper.GetIsActive())
                    {
                        wrapper.Init();
                    }
                }
                else
                {
                    if (wrapper.GetIsActive())
                    {
                        wrapper.Deactivate();
                    }
                }

                if (i == 0)
                {
                    GetInputWrapper(i).Update(gamePadState, keyboardState);
                }
                else
                {
                    GetInputWrapper(i).Update(gamePadState);
                }
            }
        }
    }
}
