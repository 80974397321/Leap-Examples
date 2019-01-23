using Leap;
using System;
using System.Collections.Generic;

namespace LeapTest2._1
{
    delegate void OutputStream(string s);
    
    // Класс, переопределяющий методы обработки событий контроллера
    class MyListener : Leap.Listener
    {
        private object thisLock;
        public OutputStream output;
        Bone.BoneType[] bonesInFinger = Enum.GetValues(typeof(Bone.BoneType)) as Bone.BoneType[];
        public MyListener() : base()
        {
            thisLock = new object();
        }

        private void PutLine(String line)
        {
            lock (thisLock)
            {
                output(line);
            }
        }

        public override void OnConnect(Controller controller)
        {
            //PutLine("Connected");
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        }

        public override void OnFrame(Controller controller)
        {
            Frame currentFrame = controller.Frame();

            List<Point3D[]> frameBones = new List<Point3D[]>(30);
            List<Point3D> frameFingertips = new List<Point3D>(8);
            Point3D start, end;

            foreach (Hand hand in currentFrame.Hands)
            {
                foreach (Finger finger in hand.Fingers)
                {
                    for (int i = 0; i < bonesInFinger.Length; i++)
                    {
                        Bone bone = finger.Bone(bonesInFinger[i]);
                        start = vectorToPoint(bone.PrevJoint);
                        end = vectorToPoint(bone.NextJoint);
                        frameBones.Add(new Point3D[] { start, end });
                    }

                    start = vectorToPoint(hand.WristPosition);
                    end = vectorToPoint(finger.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
                    frameBones.Add(new Point3D[] { start, end });

                    HandParameters.palm = vectorToPoint(hand.WristPosition);

                    frameFingertips.Add(vectorToPoint(finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint));
                }
            }

            HandParameters.bones = frameBones;
            HandParameters.fingertips = frameFingertips;
        }

        private Point3D vectorToPoint(Vector v)
        {
            return new Point3D(v.x, v.y, v.z);
        }
    }
}
