using Leap;
using System;

namespace LeapTest2._1
{
    delegate void OutputStream(string s);

    class MyListener : Leap.Listener
    {
        private object thisLock;

        public float translationProbabilityThreshhold { get; set; }
        public float rotationProbabilityThreshhold { get; set; }
        public float scalingProbabilityThreshhold { get; set; }

        public float translationMultiplier { get; set; }
        public float rotationMultiplier { get; set; }
        public float scalingMultiplier { get; set; }


        public float suspendScaling { get; set; }
        public float suspendRotation { get; set; }
        public float suspendTranslation { get; set; }

        public float transitionLockThreshhold { get; set; }
        public float rotationLockThreshhold { get; set; }
        public float scalingLockThreshhold { get; set; }

        public OutputStream output;

        public MyListener() : base()
        {
            thisLock = new object();

            translationProbabilityThreshhold = 0.5f;
            rotationProbabilityThreshhold = 0.7f;
            scalingProbabilityThreshhold = 0.6f;

            suspendScaling = 0.6f;
            suspendRotation = 0.6f;
            suspendTranslation = 0.6f;

            transitionLockThreshhold = 2;
            rotationLockThreshhold = 0.01f;
            scalingLockThreshhold = 0.01f;

            translationMultiplier = 0.1f;
            rotationMultiplier = 30;
            scalingMultiplier = 1;
    }

        // Потокобезопасный вывод
        private void PutLine(String line)
        {
            lock (thisLock)
            {
                output(line);
            }
        }

        public override void OnConnect(Controller controller)
        {
            PutLine("Connected");
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);

            TransformLock.lockTransition = false;
        }

        // Метод обработки кадров контроллера
        public override void OnFrame(Controller controller)
        {
            Frame currentFrame = controller.Frame(), previousFrame = controller.Frame(1);
            if (!previousFrame.IsValid) return;

            Hand controlHand = currentFrame.Hands.Frontmost;

            // Получаем и сохраняем преобразования с контроллера
            // Если недавно выполнялось такое преобразование - игнорировать обратное преобразование
            // чтобы можно было выполнять преобразования несколько раз последовательно
            if (controlHand.ScaleProbability(previousFrame) > scalingProbabilityThreshhold)
            {
                float scaleFactor = controlHand.ScaleFactor(previousFrame);
                scaleFactor *= scalingMultiplier;

                ScalingDirection direction, oppositeDirection;
                if (scaleFactor > 1)
                {
                    direction = ScalingDirection.Increase;
                    oppositeDirection = ScalingDirection.Decrease;
                }
                else
                {
                    direction = ScalingDirection.Decrease;
                    oppositeDirection = ScalingDirection.Increase;
                }


                for (int i = 0; i < TransformParameters.Scaling.Length; i++)
                {
                    if (TransformLock.ScalingUnlocked(Axis.x, direction))
                    {
                        TransformParameters.Scaling[0] *= scaleFactor;
                        if (Math.Abs(scaleFactor - 1) > scalingLockThreshhold)
                        {
                            TransformLock.LockScaling(Axis.x, oppositeDirection, suspendScaling);
                        }
                    }
                    if (TransformLock.ScalingUnlocked(Axis.y, direction))
                    {
                        TransformParameters.Scaling[1] *= scaleFactor;
                        if (Math.Abs(scaleFactor - 1) > scalingLockThreshhold)
                        {
                            TransformLock.LockScaling(Axis.y, oppositeDirection, suspendScaling);
                        }
                    }
                    if (TransformLock.ScalingUnlocked(Axis.z, direction))
                    {
                        TransformParameters.Scaling[2] *= scaleFactor;
                        if (Math.Abs(scaleFactor - 1) > scalingLockThreshhold)
                        {
                            TransformLock.LockScaling(Axis.z, oppositeDirection, suspendScaling);
                        }
                    }
                }

                PutLine(string.Format("Scaling for {0}", TransformParameters.Scaling[0]));
            }

            if (controlHand.RotationProbability(previousFrame) > rotationProbabilityThreshhold)
            {
                float xRotation, yRotation, zRotation;
                RotationDirection xDirection, yDirection, zDirection,
                    oppositexDirection, oppositeYDirection, oppositeZDirection;


                xRotation = controlHand.RotationAngle(previousFrame, Vector.XAxis);
                if (xRotation <= 0)
                {
                    xDirection = RotationDirection.Clockwise;
                    oppositexDirection = RotationDirection.Counterclockwise;
                }
                else
                {
                    xDirection = RotationDirection.Counterclockwise;
                    oppositexDirection = RotationDirection.Clockwise;
                }

                yRotation = controlHand.RotationAngle(previousFrame, Vector.YAxis);
                if (yRotation <= 0)
                {
                    yDirection = RotationDirection.Clockwise;
                    oppositeYDirection = RotationDirection.Counterclockwise;
                }
                else
                {
                    yDirection = RotationDirection.Counterclockwise;
                    oppositeYDirection = RotationDirection.Clockwise;
                }

                zRotation = controlHand.RotationAngle(previousFrame, Vector.ZAxis);
                if (zRotation <= 0)
                {
                    zDirection = RotationDirection.Clockwise;
                    oppositeZDirection = RotationDirection.Counterclockwise;
                }
                else
                {
                    zDirection = RotationDirection.Counterclockwise;
                    oppositeZDirection = RotationDirection.Clockwise;
                }

                xRotation *= rotationMultiplier;
                yRotation *= rotationMultiplier;
                zRotation *= rotationMultiplier;

                if (TransformLock.RotationUnlocked(Axis.x, xDirection))
                {
                    TransformParameters.Rotation[(int)Axis.x] += xRotation;
                    if (Math.Abs(xRotation) > rotationLockThreshhold) TransformLock.LockRotation(Axis.x, oppositexDirection, suspendRotation);
                }

                if (TransformLock.RotationUnlocked(Axis.y, yDirection))
                {
                    TransformParameters.Rotation[(int)Axis.y] += yRotation;
                    if (Math.Abs(yRotation) > rotationLockThreshhold) TransformLock.LockRotation(Axis.y, oppositeYDirection, suspendRotation);
                }

                if (TransformLock.RotationUnlocked(Axis.z, zDirection))
                {
                    TransformParameters.Rotation[(int)Axis.z] += zRotation;
                    if (Math.Abs(zRotation) > rotationLockThreshhold) TransformLock.LockRotation(Axis.z, oppositeZDirection, suspendRotation);
                }

                PutLine(string.Format("Rotating for {0} around x; {1} around y; {2} around z", xRotation, yRotation, zRotation));
            }

            if (controlHand.TranslationProbability(previousFrame) > scalingProbabilityThreshhold)
            {
                Vector handTranslation = controlHand.Translation(previousFrame) * translationMultiplier;
                TransitionDirection xDirection = handTranslation.x > 0 ? TransitionDirection.Forward : TransitionDirection.Backward,
                                    yDirection = handTranslation.y > 0 ? TransitionDirection.Forward : TransitionDirection.Backward,
                                    zDirection = handTranslation.z > 0 ? TransitionDirection.Forward : TransitionDirection.Backward,
                                    oppositeXDirection = handTranslation.x > 0 ? TransitionDirection.Backward : TransitionDirection.Forward,
                                    oppositeYDirection = handTranslation.y > 0 ? TransitionDirection.Backward : TransitionDirection.Forward,
                                    oppositeZDirection = handTranslation.z > 0 ? TransitionDirection.Backward : TransitionDirection.Forward;



                if (TransformLock.TransitionUnlocked(Axis.x, xDirection))
                {
                    TransformParameters.Translation[(int)Axis.x] += handTranslation.x;
                    if (handTranslation.x > transitionLockThreshhold) TransformLock.LockTransition(Axis.x, oppositeXDirection, suspendTranslation);
                }


                if (TransformLock.TransitionUnlocked(Axis.y, yDirection))
                {
                    TransformParameters.Translation[(int)Axis.y] += handTranslation.y;
                    if (handTranslation.y > transitionLockThreshhold) TransformLock.LockTransition(Axis.y, oppositeYDirection, suspendTranslation);
                }

                if (TransformLock.TransitionUnlocked(Axis.z, zDirection))
                {
                    TransformParameters.Translation[(int)Axis.z] += handTranslation.z;
                    if (handTranslation.z > transitionLockThreshhold) TransformLock.LockTransition(Axis.z, oppositeZDirection, suspendTranslation);
                }


                PutLine(string.Format("Translating for {0} along x; {1} along y; {2} along z", handTranslation.x, handTranslation.y, handTranslation.z));
            }
        }

    }
}
