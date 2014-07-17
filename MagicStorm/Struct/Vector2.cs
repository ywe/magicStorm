using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  

namespace MagicStorm
{
    /// <summary>
    /// вектор с заданным началом(или горизонтальный прямоугольник), несколько вспомогательных функции
    /// </summary>
    public struct Vector2
    {
        //координаты начала и вектор
        public double x, y, vx, vy ;

        //вспомогательные свойства
        public Point2 point { get { return new Point2(x, y); } }
        public Point2 vector { get { return new Point2(vx, vy); } }
        /// <summary>
        /// число от 0 до 6,28....
        /// </summary>
        public double angleRad { get { return (Math.Atan2(vy, vx) + Math.PI*2) % (Math.PI*2); } }
        /// <summary>
        /// число от 0 до 360
        /// </summary>
        public double angleDeg { get { return angleRad / Math.PI * 180; } }

        //конструкторы
        /// <summary>
        /// x,y - начало
        /// vx,vy - сам вектор 
        /// </summary>
        public Vector2(double x, double y, double vx, double vy)
        {
            this.vx = vx;
            this.vy = vy;
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// начало и - сам вектор
        /// </summary>
        public Vector2(Point2 point, Point2 vector)
        {
            this.vx = vector.x;
            this.vy = vector.y;
            this.x = point.x;
            this.y = point.y;
        }
        /// <summary>
        /// создаем из точки, vx=1, vy=0
        /// </summary>
        public Vector2(Point2 point)
        {
            this.vx =1;
            this.vy = 0;
            this.x = point.x;
            this.y = point.y;
        }
        /// <summary>
        /// задаем начало и направление(в градусах). Вектор будет храниться в нормализованном виде
        /// </summary>
        public Vector2(double x, double y, double angleDeg)
        {
            this.x = x;
            this.y = y;
            double angleRad = angleDeg / 180 * Math.PI;
            this.vx = Math.Cos(angleRad);
            this.vy = Math.Sin(angleRad);
        }
        /// <summary>
        /// начало в декартовых, сам вектор в полярных координатах
        /// </summary>
        public Vector2(Point2 point, double angleDeg, double length)
        {
            this.x = point.x;
            this.y = point.y;
            double angleRad = angleDeg / 180 * Math.PI;
            this.vx = Math.Cos(angleRad)*length;
            this.vy = Math.Sin(angleRad)*length;
        }

        
        //вспомогательные функции
        public void Normalize()
        {
            double m = Math.Sqrt(vx * vx + vy * vy);
            vx /= m;
            vy /= m;
        }
        /// <summary>
        /// в градусах
        /// </summary>
        public void Rotate(double angleDeg)
        {
            double len = this.Length();
            this = new Vector2(x, y, (this.angleDeg + angleDeg) % 360);
            vx *= len; vy *= len;
        }
        public double Length()
        {
            return Math.Sqrt(vx * vx + vy * vy);
        }
    }
}
