using System;
using Leap;

namespace LeapTest
{
    // Пользовательский класс, переопределяющий виртуатьные методы
    // обработки событий контроллера (инициаллизация, подключение, отключение, очередной кадр)
    // В демострационных целях приведены пустые реализации
    class MyListener : Listener
    {

        public override void OnInit(Controller controller)
        {
        }

        public override void OnConnect(Controller controller)
        {
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        }

        public override void OnDisconnect(Controller controller)
        {
            //Note: not dispatched when running in a debugger.
        }

        public override void OnExit(Controller controller)
        {
        }

        // Метод-обработчик получения нового кадра
        // Основной метод обрабокти данных контроллера
        public override void OnFrame(Controller controller)
        {
            //Получает последний необработанный кадр из буфера
            Frame frame = controller.Frame();

            Utilities.LastRead = DateTime.Now;
            Finger pointing = null;

            //Определение указательного пальца ближайшей к контроллеру руки
            foreach (Finger finger in frame.Hands.Frontmost.Fingers)
            {
                if (finger.Type == Finger.FingerType.TYPE_INDEX)
                { 
                    pointing = finger;
                    break;
                }
            }

            // Логика приложения
            // При пересечении пальца с заданной плоскостью сохранить движения
            if (pointing != null)
            {
                Vector tip = pointing.TipPosition;
                if (tip.x <= Utilities.controllerMaximumX && tip.x >= Utilities.controllerMinimumX)
                {
                    if (tip.y <= Utilities.controllerMaximumY && tip.y >= Utilities.controllerMinimumY)
                    {
                        if (tip.z <= Utilities.maximalZToTrack && tip.z >= Utilities.minimalZToTrack)
                        {
                            System.Drawing.Point point = Utilities.ToScreenCoords(tip.x, tip.y);
                            int width = (int)((tip.z - Utilities.minimalZToTrack) / ((Utilities.maximalZToTrack - Utilities.minimalZToTrack) / Utilities.widthSections)) + 1;
                            

                            if (MyLines.LastPoint == null)
                                MyLines.LastPoint = point;
                            else
                            {
                                if (Utilities.Distance(point, MyLines.LastPoint.Value) > Utilities.minimalViableDistance)
                                {
                                    MyLines.Lines.Add(new Section(MyLines.LastPoint.Value, point, Utilities.currentColor, width));
                                    MyLines.LastPoint = point;
                                }
                            }
                        }
                    }
                    else
                        MyLines.LastPoint = null;
                }
                else
                    MyLines.LastPoint = null;
            }

            
        }
    }
}
