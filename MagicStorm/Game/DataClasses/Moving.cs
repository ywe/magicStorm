using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.DataClasses
{
    class Moving
    {
        public delegate Point2 CalculatePosition(Point2 start, Point2 finish, double stage);

        List<Point2> points = new List<Point2>();
        List<int> times = new List<int>();
        List<CalculatePosition> func = new List<CalculatePosition>();
        public ESprite sprite;
        public Point2 spriteSize;
        public bool changeAngle;
        public bool pause = false;

        Vector2 cur = new Vector2(-100, -100, 0);
        int part = 1;
        double stage = 0;
        public Moving(ESprite spr, Point2 sprSize, Point2 start, bool changeAngle = false)
        {
            this.sprite = spr;
            this.spriteSize = sprSize;
            this.changeAngle = changeAngle;
            points.Add(start);
            cur = new Vector2(start);
            times.Add(0);
            func.Add(null);
        }
        public void AddMove(Point2 point, int time, CalculatePosition func)
        {
            points.Add(point);
            times.Add(time);
            this.func.Add(func);
        }
        public void Draw(ref Frame frame)
        {
            frame.Add(new Sprite(sprite, cur, spriteSize));
        }
        public bool Update()
        {
            if (part < points.Count && !pause)
            {
                Point2 newPos = func[part](points[part - 1], points[part], stage);
                double angle = (new Vector2(cur.point, newPos - cur.point)).angleDeg;
                cur = new Vector2(newPos, changeAngle ? angle : 0, 1);

                stage += 1.0 / times[part];
                if ( stage > 0.999999)
                {
                    stage = 0;
                    part++;
                }
            }
            return part == points.Count; //финальная точка достигнута
        }

        public void ClearAllPoints()
        {
            points.Clear();
            times.Clear();
            func.Clear();

            points.Add(cur.point);
            times.Add(0);
            func.Add(null);

            stage = 0;
            part = 1;

        }

        public bool FinalPointReached { get { return part == points.Count; } }
    }
}
