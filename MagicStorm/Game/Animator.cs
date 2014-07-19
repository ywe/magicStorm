using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;
using MagicStorm.Game.Concrete;
using MagicStorm.Game.DataClasses;

namespace MagicStorm.Game
{
    class Animator
    {
        readonly Point2 UP = new Point2(0, -10);

        List<Moving> move = new List<Moving>();

        /// <summary>
        /// Вернет, закончил или нет
        /// </summary>
        public bool Process(int turn)
        {
            bool r = true; 
            foreach (var a in move) r = r && a.Update();
            for (int i = 0; i < move.Count; i++) if (move[i].FinalPointReached) move.RemoveAt(i--);
            return r;
        }

        public void SetAnimation(Wizard w, Wizard enemy, int turn, ECommand command, int parameter = -1)
        {
            switch (command)
            {
                case ECommand.spy:
                    Moving m = new Moving(ESprite.eyes, Config.EyeSize, AboveTile(w.pos)+UP);
                    m.AddMove(AboveTile(w.pos),t(50),Linear);
                    m.AddMove(AboveTile(w.pos), t(10), Linear);
                    m.AddMove(AboveTile(w.pos) + UP, t(50), Linear);
                    move.Add(m);
                    break;
            }
        }

        public int t(int standard)
        {
            return (int)(standard* Config.AnimSpeed);
        }

        public void DrawAll(ref Frame frame)
        {
            foreach (var a in move) a.Draw(ref frame);
        }


        Point2 AboveTile(int tile)
        {
            return new Point2(Config.FirstTilePos.x + Config.DistBetweenTile*tile , Config.IndLine);
        }

        //-----------дальше функции для изменения позиции--------------
        public Point2 Linear(Point2 start, Point2 finish, double stage)
        {
            return start + (finish - start) * stage;
        }
    }
}
