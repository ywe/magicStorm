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
        Dictionary<int, Moving> alive = new Dictionary<int, Moving>(); //те, которые сразу не умирают
        Moving[] wizards = new Moving[2];

        public Animator(Wizard[] wizards)
        {
            for (int i = 0; i < 2; i++)
            {
                ESprite spr = i==0? ESprite.wizardBlue: ESprite.wizardPink;
                this.wizards[i] = new Moving(spr, Config.WizardSize,
                    new Point2(Config.FirstTilePos.x + wizards[i].pos * Config.DistBetweenTile,
                        Config.FloorLine - Config.WizardVert - Config.WizardSize.y / 2));
            }
        }
        /// <summary>
        /// Вернет, закончил или нет
        /// </summary>
        public bool Process()
        {
            bool r = true; 
            foreach (var a in move) r = r & a.Update();
            for (int i = 0; i < move.Count; i++) if (move[i].FinalPointReached) move.RemoveAt(i--);

            foreach (Moving m in alive.Values) r = r & m.Update();

            foreach (Moving m in wizards) r = r & m.Update();

            return r;
        }

        public void SetAnimation(Wizard w, Wizard enemy, int turn, ECommand command, int parameter = -1)
        {
            switch (command)
            {
                case ECommand.spy:
                    Moving m = new Moving(ESprite.eyes, Config.EyeSize, AboveTile(enemy.pos) + UP);
                    m.AddMove(AboveTile(enemy.pos), t(50), SinSwingRefined);
                    m.AddMove(AboveTile(enemy.pos), t(10), SinSwingRefined);
                    m.AddMove(AboveTile(enemy.pos) + UP, t(50), SinSwingRefined);
                    move.Add(m);
                    break;
            }
        }

       

        public void DrawAll(ref Frame frame)
        {
            foreach (var a in wizards) a.Draw(ref frame);
            foreach (var a in move) a.Draw(ref frame);
            foreach (Moving m in alive.Values) m.Draw(ref frame);
            
        }


        public void FireIn(int id, int pos)
        {
            Moving m = new Moving(ESprite.fire, new Point2(Config.FireSize.x, 0), Ground(pos, 0), true, false);
            m.AddMove(Ground(pos, Config.FireSize.y), t(100), LinearSwing,  Config.FireSize);
            alive.Add(id, m);
        }

        public void FireOut(int id)
        {
            Moving m = alive[id];
            alive.Remove(id);
            move.Add(m);
            m.AddMove(new Point2(m.cur.x, Config.FloorLine), t(50), LinearSwing, new Point2(Config.FireSize.x,0));
        }

        public void LightningShow(int pos)
        {
            double x = PosX(pos) + Config.LightningHor;
            double y = Config.IndLine;
            Moving m = new Moving(ESprite.lightning, Config.LightningSize, new Point2(x,y));
            for (int i = 0; i < 5; i++)
            {
                m.AddMove(new Point2(x, y), t(7), LinearSwing);
                m.AddMove(new Point2(x, y) + UP, 0, LinearSwing);
                if(i != 4){
                    m.AddMove(new Point2(x, y)+UP, t(12), LinearSwing);
                    m.AddMove(new Point2(x, y) , 0, LinearSwing);
                }
            }
            move.Add( m);
        }

        public void WindShow(int pos, bool toRight)
        {
            double left, right;
            if (toRight)
            {
                left = (PosX(pos) + PosX(pos - 1)) / 2 - Config.WindArrowSize.x / 2;
                right = (PosX(pos + 2) + PosX(pos + 3)) / 2 + Config.WindArrowSize.x / 2;
            }
            else
            {
                right = (PosX(pos + 1) + PosX(pos)) / 2 + Config.WindArrowSize.x / 2;
                left = (PosX(pos - 3) + PosX(pos - 2)) / 2 - Config.WindArrowSize.x / 2;
            }
            if (!toRight) { double p = left; left = right; right = p; }
            
            double dist = Config.WindArrowSize.y * 1.2;
            double height = Config.IndLine + dist;
            for (int i = 0; i < 4; i++)
            {
                Moving m = new Moving(ESprite.windArrow, Config.WindArrowSize, new Point2(left, height),changeAngle:true);
                m.AddMove(new Point2(right, height), t(20), LinearSwing);
                m.AddMove(new Point2(left, height), 0, LinearSwing);
                m.AddMove(new Point2(right, height), t(20), LinearSwing);
                m.AddMove(new Point2(left, height), 0, LinearSwing);
                m.AddMove(new Point2(right, height), t(20), LinearSwing);

                Moving rect1 = new Moving(ESprite.rectBlack, Config.WindArrowSize, new Point2(left, height));
                Moving rect2 = new Moving(ESprite.rectBlack, Config.WindArrowSize, new Point2(right, height));
                rect1.AddMove(new Point2(left, height), t(20) * 3 + 3, LinearSwing);
                rect2.AddMove(new Point2(right, height), t(20) * 3 + 3, LinearSwing);

                move.Add(m); move.Add(rect1); move.Add(rect2);
                height -= dist;
            }
        }

        public void MoveWizard(int num, int pos, int d, bool lastStepFailed=false)
        {
            Moving m = wizards[num];
            m.cur = new Vector2(PosX(pos), Config.FloorLine - Config.WizardVert - Config.WizardSize.y/2, 0);//на всякий случай
            m.ClearAllPoints();
            m.AddMove(new Point2(PosX(pos + d), m.cur.y), t(30) * (int)Math.Abs(d), LinearSwing);
            if (lastStepFailed)
            {
                double bound;
                if (d < 0) bound = (PosX(pos + d - 1) + PosX(pos + d)) / 2 + Config.WizardSize.x / 2;
                else bound = (PosX(pos + d + 1) + PosX(pos + d)) / 2 - Config.WizardSize.x / 2;

                m.AddMove(new Point2(bound, m.cur.y), t(13), LinearSwing);
                m.AddMove(new Point2(PosX(pos + d), m.cur.y), t(13), LinearSwing);
            }

        }

        public void WallIn(int id, int pos)
        {
            Point2 loc = new Point2(
                (PosX(pos - 1) + PosX(pos)) / 2,
                Config.FloorLine + Config.TileSize.y - Config.WallSize.y / 2);
            Moving m = new Moving(ESprite.wall, new Point2(0, Config.WallSize.y), loc, true);
            m.AddMove(loc, t(30), LinearSwing, Config.WallSize);
            alive.Add(id, m);
        }

        public void WallOut(int id)
        {
            Moving m = alive[id];
            alive.Remove(id);
            m.AddMove(m.cur.point, t(30), LinearSwing, new Point2(0, Config.WallSize.y));
            move.Add(m);
        }

        public void WallOutAfter(int id, int time)
        {
            Moving m = alive[id];
            alive.Remove(id);
            m.AddMove(m.cur.point, time, LinearSwing, m.curSize);
            m.AddMove(m.cur.point, t(30), LinearSwing, new Point2(0, Config.WallSize.y));
            move.Add(m);
        }

        public void DigitShow(int pos, int digit)
        {
            int letter = Config.FontLetters.IndexOf(char.Parse(digit.ToString()));

            Moving m = new Moving(ESprite.orange2, Config.LetterSize, AboveTile(pos) + UP, frameSprite:letter);
            m.AddMove(AboveTile(pos), t(50), LinearSwing);
            m.AddMove(AboveTile(pos), t(10), LinearSwing);
            m.AddMove(AboveTile(pos) + UP, t(50), LinearSwing);
            move.Add(m);
        }

        public void ShowDamage(int wizardNum)
        {

        }

        Point2 AboveTile(int tile)
        {
            return new Point2(Config.FirstTilePos.x + Config.DistBetweenTile*tile , Config.IndLine);
        }

        

        Point2 Ground(int tile, double height)
        {
            return new Point2(Config.FirstTilePos.x + Config.DistBetweenTile * tile, Config.FloorLine - height / 2);
        }

        double PosX(int tile)
        {
            return Config.FirstTilePos.x + Config.DistBetweenTile * tile;
        }

        public int t(int standard)
        {
            return (int)Math.Ceiling(standard * Config.AnimSpeed);
        }
        //-----------дальше функции для изменения позиции--------------
        public Point2 LinearSwing(Point2 start, Point2 finish, double stage)
        {
            return start + (finish - start) * stage;
        }

        double SinSwing(double start, double append, double stage)
        {
            return start + append * ((-Math.Cos(stage * Math.PI) / 2) + 0.5);
        }

        public static Point2 SinSwingRefined(Point2 start, Point2 finish, double stage)
        {
            Point2 append = finish - start;
            double term, k = 1.8, acc = 0.15;
            if (stage < acc) term = (-Math.Cos(stage / k * Math.PI) / 2) + 0.5;
            else if (stage < 1 - acc) term = ((-Math.Cos(((stage - 0.5) / ((0.5 - acc) / (0.5 - acc / k)) + 0.5) * Math.PI) / 2) + 0.5);
            else term = ((-Math.Cos(((stage - 1) / k + 1) * Math.PI) / 2) + 0.5);
            return start + append * term;
        }
    }
}
