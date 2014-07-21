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
        List<Moving> wingsMove= new List<Moving>(); //для порядка отрисовки

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
            foreach (var a in wingsMove) r = r & a.Update();
            foreach (Moving m in alive.Values) r = r & m.Update();
            foreach (Moving m in wizards) r = r & m.Update();

            for (int i = 0; i < move.Count; i++) if (move[i].FinalPointReached) move.RemoveAt(i--);
            for (int i = 0; i < wingsMove.Count; i++) if (wingsMove[i].FinalPointReached) wingsMove.RemoveAt(i--);

            if (expWait != -1) r = false;
            ProcessWizardSprite();

            return r;
        }


        public void DrawAll(ref Frame frame)
        {
            foreach (var a in move) a.Draw(ref frame);
            foreach (Moving m in alive.Values) m.Draw(ref frame);
            foreach (var a in wingsMove) a.Draw(ref frame);
            foreach (var a in wizards) a.Draw(ref frame);
            ExplosionProcess(ref frame); 
        }

        #region simple animations
        public void ShowFist(int pos)
        {
            ShowSimple(ESprite.fist, Config.FistSize, pos);
        }

        public void ShowSpy(int pos)
        {
            ShowSimple(ESprite.eyes, Config.EyeSize, pos);
        }

        public void ShowHeal(int pos, int? wizardHealFormPoison)
        {
            ShowSimple(ESprite.plus, Config.PlusSize, pos);
            if (wizardHealFormPoison != null)
            {
                changeWizardSprTime[(int)wizardHealFormPoison] = t(55);
            }
        }

        public void ShowChangeColor(int pos, int color)
        {
            ESprite spr;
            if (color == 1) spr = ESprite.arrowRed;
            else if (color == 2) spr = ESprite.arrowBlue;
            else if (color == 3) spr = ESprite.arrowGreen;
            else spr = ESprite.arrowWhite;
            ShowSimple(spr, Config.ArrowSize, pos);
        }

        void ShowSimple(ESprite sprite, Point2 size, int pos)
        {
            Moving m = new Moving(sprite, size, AboveTile(pos) + UP);
            m.AddMove(AboveTile(pos), t(50), SinSwingRefined);
            m.AddMove(AboveTile(pos), t(10), SinSwingRefined);
            m.AddMove(AboveTile(pos) + UP, t(50), SinSwingRefined);
            move.Add(m);
        }

        #endregion

        #region fire, lightning, wind
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
        #endregion

        #region move, fly, wall
        //может столкнуться со стеной или другим магом
        public void MoveWizard(int num, int pos, int d, int destroyWall=-1, bool wizardCollision=false, bool fly = false)
        {
            bool lastStepFailed = wizardCollision || destroyWall != -1;

            Moving m = wizards[num];
            int moveTime = t(30) * (int)Math.Abs(d);
            int failTime = t(26);
            int wingsTime = fly? t(8):0;
            if (fly) { moveTime = t(15) * (int)Math.Abs(d); failTime = t(14); }

            m.cur = new Vector2(PosX(pos), Config.FloorLine - Config.WizardVert - Config.WizardSize.y/2, 0);//на всякий случай
            m.ClearAllPoints();

            // ждем пока появятся крылья
            
            m.AddMove(new Point2(PosX(pos), m.cur.y), wingsTime, LinearSwing);

            m.AddMove(new Point2(PosX(pos + d), m.cur.y), moveTime, LinearSwing);
            if (lastStepFailed)
            {
                double bound;
                if (d < 0) bound = (PosX(pos + d - 1) + PosX(pos + d)) / 2 + Config.WizardSize.x / 2;
                else bound = (PosX(pos + d + 1) + PosX(pos + d)) / 2 - Config.WizardSize.x / 2;

                m.AddMove(new Point2(bound, m.cur.y), failTime/2, LinearSwing);
                m.AddMove(new Point2(PosX(pos + d), m.cur.y), failTime/2, LinearSwing);
            }

            if (destroyWall != -1)
            {
                WallOutAfter(destroyWall, moveTime + wingsTime + failTime / 2);
            }

            if (fly)
            {
                Moving wing = new Moving(ESprite.wings, new Point2(0,0), new Point2(PosX(pos ), m.cur.y)+new Point2(0,Config.WingsVert), true);
                wing.AddMove(wing.cur.point , wingsTime, LinearSwing, Config.WingsSize);
                wing.AddMove(new Point2(PosX(pos + d), wing.cur.y), moveTime, LinearSwing, Config.WingsSize);
                if (lastStepFailed)
                {
                    double bound;
                    if (d < 0) bound = (PosX(pos + d - 1) + PosX(pos + d)) / 2 + Config.WizardSize.x / 2;
                    else bound = (PosX(pos + d + 1) + PosX(pos + d)) / 2 - Config.WizardSize.x / 2;

                    wing.AddMove(new Point2(bound, wing.cur.y), failTime / 2, LinearSwing, Config.WingsSize);
                    wing.AddMove(new Point2(PosX(pos + d), wing.cur.y), failTime / 2, LinearSwing, Config.WingsSize);
                }
                wing.AddMove(new Point2(PosX(pos+d), wing.cur.y), wingsTime, LinearSwing, new Point2(0, 0));
                wingsMove.Add(wing);
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

        public void WallOutAfter(int id, int time)
        {
            Moving m = alive[id];
            alive.Remove(id);
            m.AddMove(m.cur.point, time, LinearSwing, Config.WallSize);
            m.AddMove(m.cur.point, t(30), LinearSwing, new Point2(0, Config.WallSize.y));
            move.Add(m);
        }
        #endregion

        #region explosion, poison(with sprite change)
        double expWait=-1;
        double expStage = 0;
        Point2 expLoc;
        public void ShowExplosion(int pos, int? firstWallId=null, int? secondWallId = null)
        {
            Point2 loc = expLoc = new Point2(PosX(pos), Config.FloorLine - Config.WizardSize.y / 2);
            Point2 start = new Point2(PosX(pos), Config.IndLine + UP.y);
            Moving m = new Moving(ESprite.pointRed, Config.PointSize, start);
            m.AddMove(loc, t(40), SinSwingRefined);
            move.Add(m);

            expWait =  t(40);

            if (firstWallId != null)
            {
                WallOutAfter((int)firstWallId, t(80));
            }
        }

        void ExplosionProcess(ref Frame frame)
        {
            if (expWait > 0) { expWait--; }
            else if (expWait == 0)
            {
                if (expStage <1)
                {
                    frame.Add(new Sprite(ESprite.explosion, new Vector2(expLoc),
                        Config.ExplosionSize, (int)(expStage * 81)));
                    expStage += 1.0 / t(81);
                    if (expStage > 0.9999999) { expWait = -1; expStage = 0; }
                }
            }
        }

        int[] changeWizardSprTime = new int[] { -1, -1 };
        public void ShowPoison(int numWizard, int pos)
        {
            Point2 loc =  new Point2(PosX(pos), Config.FloorLine - Config.WizardSize.y / 2);
            Point2 start = new Point2(PosX(pos), Config.IndLine + UP.y);
            Moving m = new Moving(ESprite.tileMarker, Config.PointSize, start);
            m.AddMove(loc, t(40), SinSwingRefined);
            move.Add(m);
            changeWizardSprTime[numWizard] = Math.Max(1,t(30));
        }


        public void ProcessWizardSprite()
        {
            for (int i = 0; i < 2; i++)
            {
                if (changeWizardSprTime[i] > 0) changeWizardSprTime[i]--;
                if(changeWizardSprTime[i]==0)
                {
                    if (i == 0)
                        wizards[i].sprite = wizards[i].sprite == ESprite.wizardBlue ? ESprite.wizardSick : ESprite.wizardBlue;
                    else
                        wizards[i].sprite = wizards[i].sprite == ESprite.wizardPink ? ESprite.wizardSick : ESprite.wizardPink;
                    changeWizardSprTime[i] = -1;
                }
                
            }
        }
        #endregion

        #region frigidity, energyBall, boulder
        void ShowFrigidity(int pos)
        {

        }
        #endregion

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
