using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Nodes.Cameras;
using Momo.Core.GameEntities;
using Momo.Fonts;
using Momo.Debug;


// The DebugFly behaviour is incomplete so it's currently removed from the cycle order (see Update())


namespace Game.Systems
{
    public class CameraController
    {
        public CameraNode Camera { get; set; }
        public Vector2 FollowPosition { get; set; }

        private Vector3 mCameraVelocity = Vector3.Zero;

        private Vector3 mDebugEuler = Vector3.Zero;

        private Momo.Maths.CriticallyDampenedSpring mSpring = new Momo.Maths.CriticallyDampenedSpring();

        private enum Behaviour
        {
            Follow,
            DebugPan,
            DebugFly
        }
        private Behaviour mBehaviour = Behaviour.Follow;

        private const int kDebugStringLength = 64;
        protected MutableString mDebugString = new MutableString(kDebugStringLength);

        private Vector3 mDebugYawPitchRoll;

        public CameraController()
        {

        }

        public void Update(ref FrameTime frameTime, ref Input.InputWrapper input)
        {
            if (Camera == null)
                return;

            // DebugFly behaviour currently doesn't work, so I've removed it here
            Behaviour[] nextBehaviours = { Behaviour.DebugPan, /*Behaviour.DebugFly*/ Behaviour.Follow, Behaviour.Follow };
            if (input.IsButtonPressed(Buttons.LeftStick))
            {
                mBehaviour = nextBehaviours[(int)mBehaviour];
                InitBehaviour();
            }

            switch (mBehaviour)
            {
                case Behaviour.DebugPan:
                    UpdateDebug(ref input);
                    UpdateTopDown(frameTime);
                    break;

                case Behaviour.Follow:
                    UpdateFollow();
                    UpdateTopDown(frameTime);
                    break;

                case Behaviour.DebugFly:
                    UpdateDebugFly(frameTime, ref input);
                    break;
            }
        }

        private void InitBehaviour()
        {
            switch (mBehaviour)
            {
                case Behaviour.DebugPan:
                case Behaviour.Follow:
                    // Nothing
                    break;

                case Behaviour.DebugFly:
                    mDebugYawPitchRoll = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
            }
        }

        private void UpdateDebugFly(FrameTime frameTime, ref Input.InputWrapper input)
        {
            // Update orientation
            const float kMaxRot = 3.0f;
            const float kExtreme = (float)(Math.PI * 0.5) - float.Epsilon;
            mDebugYawPitchRoll.X += -input.GetRightStick().Y * frameTime.Dt * kMaxRot;
            mDebugYawPitchRoll.Y += -input.GetRightStick().X * frameTime.Dt * kMaxRot;
            mDebugYawPitchRoll.X = MathHelper.Clamp(mDebugYawPitchRoll.X, -kExtreme, kExtreme);
           // mDebugYawPitchRoll.Y = MathHelper.Clamp(mDebugYawPitchRoll.Y, -kExtreme, kExtreme);

            Matrix yawMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, mDebugYawPitchRoll.Y);
            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, mDebugYawPitchRoll.X);

            Matrix curOrientation = yawMatrix * pitchMatrix;

            // Transform the axis vectors
            Vector3 forward = Vector3.Transform(Vector3.Forward, curOrientation);
            Vector3 right = Vector3.Transform(Vector3.Right, curOrientation);
            Vector3 up = Vector3.Transform(Vector3.Up, curOrientation);

            // Create translation
            const float kMaxMove = 600.0f;
            Vector3 posDelta =  (forward * -input.GetLeftStick().Y * frameTime.Dt * kMaxMove) +
                                (right * input.GetLeftStick().X * frameTime.Dt * kMaxMove);

            Vector3 curPosition = Camera.LocalTranslation + posDelta;

            Camera.Matrix = curOrientation;
            Camera.LocalTranslation = curPosition;
        }

        static float mDebugRotVal = 0.0f;

        private void UpdateTopDown(FrameTime frameTime)
        {
            mSpring.Update(frameTime.Dt);

            Vector2 springPos = mSpring.GetCurrentValue();
            const float kCamHeight = 700.0f;
            Camera.LocalTranslation = new Vector3(
                springPos.X,
                springPos.Y,
                kCamHeight);

            Matrix cameraMatrix = Matrix.Identity;
            // Lets rotate the camera 180 in the z so that the map world renders nicely.
            cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
            cameraMatrix.Translation = Camera.LocalTranslation;

            Camera.Matrix = cameraMatrix;
        }

        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            mDebugString.Clear();
            mDebugString.Append("(");
            mDebugString.Append(Camera.Matrix.Translation.ToString());
            mDebugString.Append(") ");
            mDebugString.Append(mBehaviour.ToString());
            mDebugString.EndAppend();

            debugTextBatchPrinter.AddToDrawList(mDebugString.GetCharacterArray(), Color.White, Color.Black, new Vector2(16.0f, 670.0f), debugTextStyle);
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
