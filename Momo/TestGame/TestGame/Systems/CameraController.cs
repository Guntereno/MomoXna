using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Nodes.Cameras;
using Momo.Core.GameEntities;
using Momo.Fonts;
using Momo.Debug;



namespace Game.Systems
{
    public class CameraController
    {
        public CameraNode Camera { get; set; }
        public Vector2 FollowPosition { get; set; }

        Vector3 mCameraVelocity = Vector3.Zero;

        Momo.Maths.CriticallyDampenedSpring mSpring = new Momo.Maths.CriticallyDampenedSpring();

        private enum Behaviour
        {
            Debug,
            Follow
        }
        Behaviour mBehaviour = Behaviour.Debug;

        const int kDebugStringLength = 64;
        protected MutableString mDebugString = new MutableString(kDebugStringLength);


        public CameraController()
        {

        }

        public void Update(ref FrameTime frameTime, ref Input.InputWrapper input)
        {
            if (Camera == null)
                return;

            if (input.IsButtonDown(Buttons.LeftStick))
            {
                mBehaviour = Behaviour.Debug;
            }
            else
            {
                mBehaviour = Behaviour.Follow;
            }

            switch (mBehaviour)
            {
                case Behaviour.Debug:
                    UpdateDebug(ref input);
                    break;

                case Behaviour.Follow:
                    UpdateFollow();
                    break;
            }

            mSpring.Update(frameTime.Dt);

            Vector2 springPos = mSpring.GetCurrentValue();
            const float kCamHeight = 1500.0f;
            Camera.LocalTranslation = new Vector3(
                (float)Math.Floor(springPos.X),
                (float)Math.Floor(springPos.Y),
                kCamHeight);

            Matrix cameraMatrix = Matrix.Identity;
            // Lets rotate the camera 180 in the z so that the map world renders nicely.
            cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
            cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
            cameraMatrix.Translation = Camera.LocalTranslation;

            Camera.Matrix = cameraMatrix;
        }

        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            Vector2 curPos = mSpring.GetCurrentValue();

            mDebugString.Clear();
            mDebugString.Append("(");
            mDebugString.Append(curPos, 0);
            mDebugString.Append(")");
            mDebugString.EndAppend();

            debugTextBatchPrinter.AddToDrawList(mDebugString.GetCharacterArray(), Color.White, Color.Black, new Vector2(0.0f, 0.0f), debugTextStyle);
        }

        private void UpdateFollow()
        {
            mSpring.SetTarget(FollowPosition);
        }

        private void UpdateDebug(ref Input.InputWrapper input)
        {
            Vector2 pos = mSpring.GetTarget();
            const float kMaxSpeed = 30.0f;
            Vector2 inputVector = input.GetRightStick();
            pos += inputVector * kMaxSpeed;
            mSpring.SetTarget(pos);
        }
    }
}
