
namespace LeapTest2._1
{
    // Вспомагательные классы и типы для преобразований
    // и их флаги их блокировки
    enum ScalingDirection
    {
        Increase, Decrease
    }
    enum RotationDirection
    {
        Clockwise, Counterclockwise
    }
    enum TransitionDirection
    {
        Forward, Backward
    }

    class Lock
    {
        private float _delay;

        public static float tick { get; set; }
        public float delay
        {
            get
            {
                return _delay;
            }
            set
            {
                if (value < 0) _delay = 0;
                else _delay = value;
            }
        }

        static Lock()
        {
            tick = 0.1f;
        }

        public Lock(float initDelay = 0)
        {
            delay = initDelay;
        }

        public void Tick()
        {
            delay -= tick;
        }
    }


    class RotationLock : Lock
    {
        public RotationDirection direction { get; set; }

        public RotationLock(RotationDirection rd, float d) : base(d)
        {
            direction = rd;
        }
    }

    class TransitionLock : Lock
    {
        public TransitionDirection direction { get; set; }
        public TransitionLock(TransitionDirection td, float d) : base(d)
        {
            direction = td;
        }
    }

    class ScalingLock : Lock
    {
        public ScalingDirection direction { get; set; }
        public ScalingLock(ScalingDirection sd, float d) : base(d)
        {
            direction = sd;
        }

    }

    static class TransformLock
    {
        private static readonly Lock[] locks;

        public static bool lockTransition { get; set; }
        public static bool lockScaling { get; set; }
        public static bool lockRotation { get; set; }

        private static ScalingLock xScalingLock { get; set; }
        private static ScalingLock yScalingLock { get; set; }
        private static ScalingLock zScalingLock { get; set; }

        private static RotationLock xRotationLock { get; set; }
        private static RotationLock yRotationLock { get; set; }
        private static RotationLock zRotationLock { get; set; }

        private static TransitionLock xTransitionLock { get; set; }
        private static TransitionLock yTransitionLock { get; set; }
        private static TransitionLock zTransitionLock { get; set; }
        static TransformLock()
        {
            xTransitionLock = new TransitionLock(TransitionDirection.Forward, 0);
            yTransitionLock = new TransitionLock(TransitionDirection.Forward, 0);
            zTransitionLock = new TransitionLock(TransitionDirection.Forward, 0);
            xRotationLock = new RotationLock(RotationDirection.Clockwise, 0);
            yRotationLock = new RotationLock(RotationDirection.Clockwise, 0);
            zRotationLock = new RotationLock(RotationDirection.Clockwise, 0);
            xScalingLock = new ScalingLock(ScalingDirection.Increase, 0);
            yScalingLock = new ScalingLock(ScalingDirection.Increase, 0);
            zScalingLock = new ScalingLock(ScalingDirection.Increase, 0);

            locks = new Lock[] {xTransitionLock, yTransitionLock, zTransitionLock,
                                xRotationLock, yRotationLock, zRotationLock,
                                xScalingLock, yScalingLock, zScalingLock};

            lockTransition = true;
            lockRotation = true;
            lockScaling = true;

        }

        public static void Tick()
        {
            foreach (Lock item in locks)
            {
                item.Tick();
            }
        }

        public static void LockTransition(Axis a, TransitionDirection td, float d)
        {
            if (!lockTransition) return;

            switch (a)
            {
                case Axis.x:
                    xTransitionLock.direction = td;
                    xTransitionLock.delay = d;
                    break;
                case Axis.y:
                    yTransitionLock.direction = td;
                    yTransitionLock.delay = d;
                    break;
                case Axis.z:
                    zTransitionLock.direction = td;
                    zTransitionLock.delay = d;
                    break;
                default:
                    break;
            }
        }

        public static void LockRotation(Axis a, RotationDirection td, float d)
        {
            if (!lockRotation) return;

            switch (a)
            {
                case Axis.x:
                    xRotationLock.direction = td;
                    xRotationLock.delay = d;
                    break;
                case Axis.y:
                    yRotationLock.direction = td;
                    yRotationLock.delay = d;
                    break;
                case Axis.z:
                    zRotationLock.direction = td;
                    zRotationLock.delay = d;
                    break;
                default:
                    break;
            }
        }

        public static void LockScaling(Axis a, ScalingDirection td, float d)
        {
            if (!lockScaling) return;

            switch (a)
            {
                case Axis.x:
                    xScalingLock.delay = d;
                    xScalingLock.direction = td;
                    break;
                case Axis.y:
                    yScalingLock.delay = d;
                    yScalingLock.direction = td;
                    break;
                case Axis.z:
                    zScalingLock.delay = d;
                    zScalingLock.direction = td;
                    break;
                default:
                    break;
            }
        }

        public static bool TransitionUnlocked(Axis a, TransitionDirection td)
        {
            bool result;
            switch (a)
            {
                case Axis.x:
                    result = xTransitionLock.direction != td || xTransitionLock.delay == 0;
                    break;
                case Axis.y:
                    result = yTransitionLock.direction != td || yTransitionLock.delay == 0;
                    break;
                case Axis.z:
                    result = zTransitionLock.direction != td || zTransitionLock.delay == 0;
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }

        public static bool RotationUnlocked(Axis a, RotationDirection td)
        {
            bool result;
            switch (a)
            {
                case Axis.x:
                    result = xRotationLock.direction != td || xRotationLock.delay == 0;
                    break;
                case Axis.y:
                    result = yRotationLock.direction != td || yRotationLock.delay == 0;
                    break;
                case Axis.z:
                    result = zRotationLock.direction != td || zRotationLock.delay == 0;
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }

        public static bool ScalingUnlocked(Axis a, ScalingDirection td)
        {
            bool result;
            switch (a)
            {
                case Axis.x:
                    result = xScalingLock.direction != td || xScalingLock.delay == 0;
                    break;
                case Axis.y:
                    result = yScalingLock.direction != td || yScalingLock.delay == 0;
                    break;
                case Axis.z:
                    result = zScalingLock.direction != td || zScalingLock.delay == 0;
                    break;
                default:
                    result = true;
                    break;
            }
            return result;
        }
    }
}
