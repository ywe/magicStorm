using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;
using MagicStorm.Game.Concrete;
using MagicStorm.Game.DataClasses;
using System.IO;

namespace MagicStorm.Game
{
    class Game : IGame
    {
        static int ID_NOT_USE=0;
        static int NextID { get { return ID_NOT_USE++; } }

        AnimRecorder _animRecorder = new AnimRecorder();
        TurnMaker _turnMaker = new TurnMaker();
        Animator _animator;
        State _state = new State();

        int? arrowBlue = null, arrowRed = null;

        int _turn = 0;

        public Game(ParamsFromFormToGame p)
        {
            _state.wizards[0].programAddress = p.firstAddress;
            _state.wizards[1].programAddress = p.secondAddress;
            _state.tiles = new Tile[p.map.Length];
            for(int i = 0; i < p.map.Length; i++)
            {
                _state.tiles[i] = new Tile(p.map[i]-1);
            }
            int center = p.map.Length / 2 - 1;
            _state.wizards[0].pos = center;
            _state.wizards[1].pos = center + 1;
            _state.tiles[center].growingTime = _state.tiles[center + 1].growingTime = Config.FlowerGrowingTime;
            Config.AnimSpeed = p.animTime;

            for(int i = 0; i < 2; i++)
            {
                if (_state.wizards[i].programAddress == null)
                    _state.wizards[i].name = "Человек";
                else
                {
                    string file = Path.GetFileName(_state.wizards[i].programAddress);
                    _state.wizards[i].name = file.Substring(0, file.Length - 4);
                }
            }
            

            _turnMaker.logfile = p.logfile;
            _animator = new Animator(_state.wizards);
            CreateLines(20);
        }

        public Frame Process(IGetKeyboardState keyboard)
        {
            if (keyboard.GetActionTime(EKeyboardAction.Esc) == 1) return null;

            Frame frame = new Frame(); 
            
            _animRecorder.Process(keyboard, ref frame);


            Frame turnFrame = new Frame();
            #region turn control part
            int command, tile; TurnMaker.EStatus status;
            _turnMaker.Process(_state.stage == State.EStage.turn,Active, Enemy(Active), _state.tiles, _turn, ref turnFrame, keyboard, out command, out tile, out status);
            
            //тут комманд != -1 гарантирует, что ход сделан, и без статуса
            if (command != -1 && _state.stage == State.EStage.turn)
            {
                if (status == TurnMaker.EStatus.ok)
                    RunCommand((ECommand)command, tile);
                else
                {
                    //todo сделать, что будет при ошибочной команде
                }
                _state.stage = State.EStage.animationAfter;
            }
            if (_state.stage == State.EStage.animationAfter)
            {
                bool finished = Animation(lineAfter[0]);
                if (finished)
                {
                    _state.stage = State.EStage.animationBefore; //тут важно, чтоб гейм финиш еще мог переделать состояние
                    CheckGameFinish();
                    GoToNewLoop();
                }

            }
            if (_state.stage == State.EStage.animationBefore)
            {
                bool finished = Animation(lineBefore[0]);
                if (finished)
                {
                    _state.stage = State.EStage.turn;
                    CheckGameFinish();
                }
            }
            #endregion

            Frame tilesFrame = new Frame();
            #region draw part 1
            for (int i = 0; i < _state.tiles.Length; i++)
            {
                Tile t = _state.tiles[i];

                //border
                tilesFrame.Add(new Sprite(t.growingTime==0? ESprite.rectBoardBright : ESprite.rectBoardDark,
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        Config.FirstTilePos.y, 0),
                        Config.TileBorderSize));

                if (t.growingTime < Config.FlowerGrowingTime)
                {
                    

                    //inner part
                    double height = Config.TileSize.y / Config.FlowerGrowingTime *
                        (Config.FlowerGrowingTime - t.growingTime);
                    double y = Config.FirstTilePos.y + Config.TileSize.y/2 - height / 2;

                    tilesFrame.Add(new Sprite(Tile.SPRITES[t.color],
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        y, 0),
                        new Point2(Config.TileSize.x, height)));
                }

                if (t.growingTime > Config.FlowerGrowingTime)
                {
                    tilesFrame.Add(new Sprite(ESprite.snowflake,
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        Config.FirstTilePos.y, 0),
                        Config.TileSize));
                }
            }
            #endregion


            Frame menuFrame = new Frame();
            #region draw part 2
            menuFrame.Add(new Sprite(ESprite.menuback,
                new Vector2(Config.GameSize.x / 2, Config.MenuLine / 2, 0),
                new Point2(Config.GameSize.x, Config.MenuLine)));

            foreach (Wizard w in _state.wizards)
            {
                menuFrame.Add(new Text(EFont.lilac,
                    new Point2(w.team == Wizard.ETeam.first ? Config.PlayerInfoLeft : Config.PlayerInfoRight,
                    Config.PlayerInfoLine),
                Config.LetterSize,
                "HP - "+w.hp.ToString() + Change(w.hpChange),
                "огонь - " + w.flowers[0].ToString() + Change(w.flowersChange[0]),
                "вода - " + w.flowers[1].ToString() + Change(w.flowersChange[1]),
                "земля - " + w.flowers[2].ToString() + Change(w.flowersChange[2]),
                "воздух - " + w.flowers[3].ToString() + Change(w.flowersChange[3]))
                );
            }


            double separateScore = Config.ScoreRectSize.x / (_state.wizards[0].hp +
                _state.wizards[1].hp) * (_state.wizards[0].hp);

            menuFrame.Add(new Sprite(ESprite.blueStrip, new Vector2(Config.ScoreCorner + new Point2(separateScore/2,0)+ new Point2(0, Config.ScoreRectSize.y /2)),
                new Point2(separateScore, Config.ScoreRectSize.y)));
            menuFrame.Add(new Sprite(ESprite.redStrip, new Vector2(Config.ScoreCorner + new Point2(separateScore, 0) + new Point2((Config.ScoreRectSize.x - separateScore) / 2, 0) + new Point2(0, Config.ScoreRectSize.y /2)),
                new Point2(Config.ScoreRectSize.x - separateScore, Config.ScoreRectSize.y)));

            double rightOffset =  Config.ScoreRectSize.x - (_state.wizards[1].name.Length + 0) * Config.LetterSize.x;
            menuFrame.Add(new Text(EFont.lilac, new Point2(Config.ScoreCorner.x , Config.TeamsAndTimeLine),
                Config.LetterSize, _state.wizards[0].name));
            menuFrame.Add(new Text(EFont.lilac, new Point2(Config.ScoreCorner.x + rightOffset, Config.TeamsAndTimeLine),
               Config.LetterSize, _state.wizards[1].name));
            
            //clock
            double clockLine = Config.CommandCorner[0].y + Config.CommandCorner[0].vy / 2;
            menuFrame.Add(new Text(EFont.green, true, new Point2(50, clockLine),
                Config.LetterSizeBig,  _turn.ToString()));
            #endregion


            #region draw part magic
            

            _animator.DrawAll(ref frame);
            frame.Add(false, menuFrame, turnFrame, tilesFrame);
            #endregion

            frame.Add(new Text(EFont.orange, new Point2(0, 0), Config.LetterSize,
                keyboard.MousePosMap.x.ToString("N3"), keyboard.MousePosMap.y.ToString("N3")));

            
            return frame;
        }
        string Change(int x){
            if(x == 0) return "";
            return  "(" + (x>0? "+":"") + x.ToString()+ ")";
        }

        Wizard Active
        {
            get { return _state.wizards[_state.activeWizard]; }
        }

        Wizard Enemy(Wizard w)
        {
            return w == _state.wizards[0] ? _state.wizards[1] : _state.wizards[0];
        }


        //1 - при переходе задаем все анимашки
        //2 - во время секции анимации их выполняем
        List<Queue<Box>> lineBefore;
        List<Queue<Box>> lineAfter; //индекс - через сколько ходов
        public void CreateLines(int count)
        {
            lineBefore = new List<Queue<Box>>(count);
            lineAfter = new List<Queue<Box>>(count);
            for(int i = 0; i < count; i++){
                lineBefore.Add( new Queue<Box>());
                lineAfter.Add(  new Queue<Box>());
            }
        }
        int iddd;
        public void GoToNewLoop()
        {
            _animator.WindShow(8, false);
            _animator.WindShow(10, true);
            _animator.MoveWizard(0, 2, 0,fly:true,wizardCollision:true);
            if (_turn  == 0) { iddd = NextID; _animator.WallIn(iddd, 13); }
           // if (_turn % 2 == 1) _animator.WallOutAfter(iddd, 50);
           // _animator.MoveWizard(1, 10, 2, iddd);
            if (_turn == 1)
                _animator.ShowExplosion(13, firstWallId: iddd);
            if(_turn < 3)
                _animator.ShowPoison(1, 10);
           // if(_turn == 3)
            //_animator.ShowHeal(10,1);

            //_animator.ShowFist(10);
           // _animator.ShowChangeColor(10, 4);
            _animator.ShowSpy(10);

            lineBefore.RemoveAt(0);
            lineBefore.Add(new Queue<Box>());
            lineAfter.RemoveAt(0);
            lineAfter.Add(new Queue<Box>());
            _state.activeWizard = (_state.activeWizard + 1) % 2;
            for (int i = 0; i < 4; i++) Active.flowersChange[i] = 0;
            _turn++;
        }

        bool Animation(Queue<Box> line) //возвращает - завершилась или нет
        {
            if (!_animator.Process()) return false;
            if (line.Count == 0) return true;

            bool first = true;
            while(line.Count > 0 && (first || line.Peek().sameTime ))
            {
                first = false;

                Box box = line.Dequeue();
                switch (box.type)
                {
                    case Box.EType.digit:
                        _animator.DigitShow(box.first, box.second);
                        break;
                    case Box.EType.fireIn:
                        _animator.FireIn(box.id, box.first);
                        break;
                    case Box.EType.fireOut:
                        _animator.FireOut(box.id);
                        break;
                    case Box.EType.attackFire:
                        AttackFire(box.first);
                        break;


                    case Box.EType.changeHp:
                        ChangeHP(box.first, box.second); //тут только отбор хп не ниже, чем до 0. Проверка конца игры в другом месте 
                        break;
                }
            }

            return false;
        }


        void RunCommand(ECommand command, int parameter)
        {
            if (command == ECommand.fire)
            {
                lineAfter[0].Enqueue(new Box(Box.EType.digit ){ first=parameter, second = 1});//сама цифра
                int ID = NextID;
                lineBefore[2].Enqueue(new Box(Box.EType.fireIn) { id = ID, first = parameter });
                lineBefore[2].Enqueue(new Box(Box.EType.attackFire) { first = parameter });
                lineBefore[4].Enqueue(new Box(Box.EType.attackFire) { first = parameter });
                lineBefore[6].Enqueue(new Box(Box.EType.attackFire) { first = parameter });
                lineBefore[6].Enqueue(new Box(Box.EType.fireOut) { id = ID });
            }

            for (int i = 0; i < 4; i++)
            {
                Active.flowers[i] -= Config.Cost[command][i];
                Active.flowersChange[i] = -Config.Cost[command][i];
            }
        }

        //-------------small-------------------------------------------------
        void AttackFire(int pos)
        {
            bool first = true;
            for (int i = Math.Max(0, pos - 1); i <= Math.Min(Config.TileCount - 1, pos + 1); i++)
            {
                int wiz = WizardInTile(i);
                if (wiz != -1)
                {
                    first = false;
                    _animator.ShowDamage(wiz);
                    lineBefore[0].Enqueue(new Box(Box.EType.changeHp){ sameTime = !first, first = wiz,second= -(int)Config.Damage[ECommand.fire]});
                }
            }
        }
        int WizardInTile(int tile)
        {
            foreach (var a in _state.wizards) if (a.pos == tile) return (int)a.team;
            return -1;
        }

        void ChangeHP(int wizard, int hp)
        {
            Wizard w = _state.wizards[wizard];
            if(w.hp + hp<0) hp = -w.hp;
            w.hpChange = hp;
            w.hp += hp;
        }

        void CheckGameFinish()
        {

        }
        //----------------------------------------------------------------
    }
}
