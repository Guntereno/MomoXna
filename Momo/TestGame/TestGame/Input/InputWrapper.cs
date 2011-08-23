using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TestGame.Input
{
    public class InputWrapper
    {
        public InputWrapper()
        {
            m_leftStick = Vector2.Zero;
            m_rightStick = Vector2.Zero;

            m_buttonDict = new Dictionary<Buttons, ButtonInfo>();

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                ButtonInfo buttonInfo = new ButtonInfo();
                buttonInfo.m_state = false;
                buttonInfo.m_previousState = false;

                // Map the keyboard buttons
                switch (button)
                {
                    case Buttons.RightShoulder: buttonInfo.m_mappedKey = Keys.NumPad0; break;
                    default: buttonInfo.m_mappedKey = null; break;
                }

                m_buttonDict[button] = buttonInfo;
            }

            // Map the keyboard buttons
            {
                ButtonInfo buttonInfo;
                buttonInfo = m_buttonDict[Buttons.RightShoulder];
                buttonInfo.m_mappedKey = Keys.NumPad0;
                m_buttonDict[Buttons.RightShoulder] = buttonInfo;
            }

        }

        public Vector2 GetLeftStick() { return m_leftStick; }
        public Vector2 GetRightStick() { return m_rightStick; }

        public float GetLeftTrigger() { return m_leftTrigger; }
        public float GetRightTrigger() { return m_rightTrigger; }


        public bool IsButtonPressed(Buttons button)
        {
            ButtonInfo buttonInfo = m_buttonDict[button];
            return (buttonInfo.m_state & !buttonInfo.m_previousState);
        }

        public bool IsButtonReleased(Buttons button)
        {
            ButtonInfo buttonInfo = m_buttonDict[button];
            return (buttonInfo.m_previousState & !buttonInfo.m_state);
        }

        public bool IsButtonDown(Buttons button)
        {
            return m_buttonDict[button].m_state;
        }

        public void Update(GamePadState currentGamePadState)
        {
            // Update the sticks
            Vector2 padVector;
            padVector = currentGamePadState.ThumbSticks.Left;
            UpdateStick(ref m_leftStick, ref padVector);
            padVector = currentGamePadState.ThumbSticks.Right;
            UpdateStick(ref m_rightStick, ref padVector);

            // Update the triggers
            m_leftTrigger = currentGamePadState.Triggers.Left;
            m_rightTrigger = currentGamePadState.Triggers.Right;

            // Update the buttons
            foreach(Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                ButtonInfo buttonInfo = m_buttonDict[button];
                buttonInfo.m_previousState = buttonInfo.m_state;
                buttonInfo.m_state = currentGamePadState.IsButtonDown(button);
                m_buttonDict[button] = buttonInfo;
            }
        }

        public void Update(GamePadState currentGamePadState, KeyboardState currentKeyboardState)
        {
            // Stop the compiler bitching about unused variables...
            m_previousButtonState = m_buttonState;

            Update(currentGamePadState);

            // Keyboard input overrides joypad
            UpdateStickKeys(ref m_leftStick, ref currentKeyboardState, Keys.W, Keys.A, Keys.S, Keys.D);
            UpdateStickKeys(ref m_rightStick, ref currentKeyboardState, Keys.NumPad8, Keys.NumPad4, Keys.NumPad2, Keys.NumPad6);

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                ButtonInfo buttonInfo = m_buttonDict[button];
                if ( (buttonInfo.m_mappedKey != null) && (currentKeyboardState.IsKeyDown(buttonInfo.m_mappedKey.Value)) )
                {
                    buttonInfo.m_state = true;
                    m_buttonDict[button] = buttonInfo;
                }
            }
        }

        private static void UpdateStick(ref Vector2 resultVector, ref Vector2 padVector)
        {
            const float kAnalogDeadzone = 0.2f;
            const float kAnalogDeadzoneSq = kAnalogDeadzone * kAnalogDeadzone;
            if (padVector.LengthSquared() > kAnalogDeadzoneSq)
            {
                resultVector = padVector;
                resultVector.Y *= -1.0f;
            }
            else
            {
                resultVector = Vector2.Zero;
            }
        }

        private static void UpdateStickKeys(ref Vector2 resultVector, ref KeyboardState currentKeyboardState, Keys upKey, Keys leftKey, Keys downKey, Keys rightKey)
        {
            bool leftKeyDown = currentKeyboardState.IsKeyDown(leftKey);
            bool rightKeyDown = currentKeyboardState.IsKeyDown(rightKey);
            if (leftKeyDown && !rightKeyDown)
            {
                resultVector.X = -1.0f;
            }
            else if (rightKeyDown && !leftKeyDown)
            {
                resultVector.X = 1.0f;
            }

            bool upKeyDown = currentKeyboardState.IsKeyDown(upKey);
            bool downKeyDown = currentKeyboardState.IsKeyDown(downKey);
            if (upKeyDown && !downKeyDown)
            {
                resultVector.Y = -1.0f;
            }
            else if (downKeyDown && !upKeyDown)
            {
                resultVector.Y = 1.0f;
            }
        }


        private struct ButtonInfo
        {
            public bool m_state;
            public bool m_previousState;
            public Keys? m_mappedKey;
        };

        Dictionary<Buttons, ButtonInfo> m_buttonDict;

        private Vector2 m_leftStick = Vector2.Zero;
        private Vector2 m_rightStick = Vector2.Zero;
        private bool[] m_buttonState = null;
        private bool[] m_previousButtonState = null;

        private float m_leftTrigger = 0.0f;
        private float m_rightTrigger = 0.0f;

        private int m_numKeys = Enum.GetNames(typeof(Buttons)).Length;
    }
}
