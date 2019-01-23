using Leap;

namespace LeapSetup
{
    // Класс, переопределяющий методы-обработчики событий контроллера
    class MyListener : Leap.Listener
    {
        public override void OnInit(Controller controller)
        {
            //Happens on controller initializtion
        }

        public override void OnConnect(Controller controller)
        {
            //Happens on establishing connection to controller
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        }

        public override void OnDisconnect(Controller controller)
        {
            //Note: not dispatched when running in a debugger.
            //Happens on connection loss with a controller
        }

        public override void OnExit(Controller controller)
        {
        }

        // Метод обработки получаемых кадров
        // Основной источник данных с контроллера
        public override void OnFrame(Controller controller)
        {
            Frame currentFrame = controller.Frame();


            // Считываем и запоминаем информацию о каждом пальце ближайшей руки
            // Обратите внимание на блокирование глобальных объектов для других потоков
            foreach (Finger finger in currentFrame.Hands.Frontmost.Fingers)
            {
                Vector[] fingerPoints = new Vector[FingerInfo.pointNumber];
                fingerPoints[FingerInfo.tip] = finger.TipPosition;
                fingerPoints[FingerInfo.distal] = finger.Bone(Bone.BoneType.TYPE_DISTAL).PrevJoint;
                fingerPoints[FingerInfo.intermediate] = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE).PrevJoint;
                fingerPoints[FingerInfo.proximal] = finger.Bone(Bone.BoneType.TYPE_PROXIMAL).PrevJoint;
                fingerPoints[FingerInfo.metacarpal] = finger.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint;

                switch (finger.Type)
                {
                    case Finger.FingerType.TYPE_THUMB:
                        lock(FingerInfo.Thumb)
                        {
                            FingerInfo.Thumb = fingerPoints;
                        }
                        break;
                    case Finger.FingerType.TYPE_INDEX:
                        lock (FingerInfo.Index)
                        {
                            FingerInfo.Index = fingerPoints;
                        }
                        break;
                    case Finger.FingerType.TYPE_MIDDLE:
                        lock (FingerInfo.Middle)
                        {
                            FingerInfo.Middle = fingerPoints;
                        }
                        break;
                    case Finger.FingerType.TYPE_RING:
                        lock (FingerInfo.Ring)
                        {
                            FingerInfo.Ring = fingerPoints;
                        }
                        break;
                    case Finger.FingerType.TYPE_PINKY:
                        lock (FingerInfo.Pinky)
                        {
                            FingerInfo.Pinky = fingerPoints;
                        }
                        break;
                    default:
                        break;
                }
            }

        }

    }
}
