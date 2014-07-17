using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;

namespace MagicStorm.Game.Concrete
{
    class AnimRecorder
    {
        enum EState { none, recording, saving, end };
        EState _state = EState.none;

        int time=0;
        List<Point2> pos = new List<Point2>();

        double MAX_ANGLE_DIF = 20;
        public void Process(IGetKeyboardState keyboard, ref Frame frame)
        {

            bool b = Between(50, 40, 60);
            b = Between(50, 60, 40);
            b = Between(5, 10, 340);
            b = Between(180, 5, 5);
            b = Between(85, 5, 190);
            double k = CompAngles(10, 180);
            return;
            if (keyboard.MouseClick)
            {
                _state++; if (_state == EState.end) _state = EState.none;
            }

            if (_state == EState.recording)
            {
                pos.Add(keyboard.MousePosScreen);
                time++;
            }
        }

        void NextState()
        {
            switch (_state)
            {
                case EState.none:
                    _state = EState.recording;
                    break;
                case EState.recording:
                    _state = EState.saving;
                    SetAccelerations();
                    break;
            }
        }

        List<Point2> acc = new List<Point2>();
        List<int> timeLine = new List<int>();
        void SetAccelerations()
        {
            Point2 axe = new Point2(1,0);
            double last = 0;

            Point2 prev = pos[1] - pos[0];
            double smallAngle = prev.angleTo(axe), bigAngle = prev.angleTo(axe);

            for (int i = 1; i < pos.Count - 1; i++)
            {
                Point2 d = pos[i + 1] - pos[i];
                double angle = d.angleTo(axe);
                if (!Between(angle, smallAngle, bigAngle)) {
                    if (CompAngles(angle, smallAngle) > 0) smallAngle = angle;
                    else bigAngle = angle;
                }

                if (CompAngles(bigAngle, smallAngle) < 0 || CompAngles(bigAngle, smallAngle) > MAX_ANGLE_DIF)
                {
                    
                }
                //double smaller = CompAgles(d.angleTo(axe);
            }
        }

        Point2 GetA(Point2 start, Point2 finish, Point2 speed, int time)
        {
            double x = Math.Sqrt(2.0 / time/time * (start.x - finish.x - speed.x * time));
            double y = Math.Sqrt(2.0 / time / time * (start.y - finish.y - speed.y * time));
            return new Point2(x, y);
        }

        double CompAngles(double a, double b)
        {
            double r= (a - b + 360) % 360;
            if (r > 180) r -= 360;
            return r;
        }

        bool Between(double x, double a, double b)
        {
            return (CompAngles(x, a) * CompAngles(b, a) > 0);
        }
    }
}
