using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;
using MagicStorm.Game.Concrete;
using MagicStorm.Game.DataClasses;

namespace MagicStorm.Game
{
    class Game : IGame
    {
        AnimRecorder _animRecorder = new AnimRecorder();
        State _state = new State();

        int? arrowBlue = null, arrowRed = null;

        int _turn = 0;

        public Frame Process(IGetKeyboardState keyboard)
        {
            if (keyboard.GetActionTime(EKeyboardAction.Esc) == 1) return null;

            Frame frame = new Frame();
            _animRecorder.Process(keyboard, ref frame);

            _state.tiles[4].growingTime = 2;
            _state.tiles[5].growingTime = 15;
            #region draw part 1
            for (int i = 0; i < _state.tiles.Length; i++)
            {
                Tile t = _state.tiles[i];

                //border
                frame.Add(new Sprite(t.growingTime==0? ESprite.rectBoardBright : ESprite.rectBoardDark,
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        Config.FirstTilePos.y, 0),
                        Config.TileBorderSize));

                if (t.growingTime < Config.FlowerGrowingTime)
                {
                    

                    //inner part
                    double height = Config.TileSize.y / Config.FlowerGrowingTime *
                        (Config.FlowerGrowingTime - t.growingTime);
                    double y = Config.FirstTilePos.y + Config.TileSize.y/2 - height / 2;

                    frame.Add(new Sprite(Tile.SPRITES[t.color],
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        y, 0),
                        new Point2(Config.TileSize.x, height)));
                }

                if (t.growingTime > Config.FlowerGrowingTime)
                {
                    frame.Add(new Sprite(ESprite.snowflake,
                        new Vector2(Config.FirstTilePos.x + i * Config.DistBetweenTile,
                        Config.FirstTilePos.y, 0),
                        Config.TileSize));
                }
            }

            foreach (Wizard w in _state.wizards)
            {
                frame.Add(new Sprite(w.team == Wizard.ETeam.first ? ESprite.wizardBlue : ESprite.wizardPink,
                     new Vector2(Config.FirstTilePos.x + w.pos * Config.DistBetweenTile,
                        Config.FloorLine - Config.WizardVert - Config.WizardSize.y / 2, 0), Config.WizardSize));
            }
            #endregion

            #region draw part 2

            #endregion

            #region draw part 3
            frame.Add(new Sprite(ESprite.menuback,
                new Vector2(Config.GameSize.x / 2, Config.MenuLine / 2, 0),
                new Point2(Config.GameSize.x, Config.MenuLine)));

            _state.wizards[0].hp = 45;
            _state.wizards[0].flowersChange[3] = 13;
            foreach (Wizard w in _state.wizards)
            {
                frame.Add(new Text(EFont.lilac,
                    new Point2(w.team == Wizard.ETeam.first ? Config.PlayerInfoLeft : Config.PlayerInfoRight,
                    Config.PlayerInfoLine),
                Config.LetterSize,
                w.hp.ToString() + Change(w.hpChange),
                "огонь - " + w.flowers[0].ToString() + Change(w.flowersChange[0]),
                "вода - " + w.flowers[1].ToString() + Change(w.flowersChange[1]),
                "земля - " + w.flowers[2].ToString() + Change(w.flowersChange[2]),
                "воздух - " + w.flowers[3].ToString() + Change(w.flowersChange[3]))
                );
            }

            frame.Add(new Text(EFont.orange,
                new Point2(Config.GameSize.x / 2 - Config.LetterSize.x, Config.TeamsAndTimeLine),
                Config.LetterSize, _turn.ToString()));

            double separateScore = Config.ScoreRectSize.x / (_state.wizards[0].hp +
                _state.wizards[1].hp) * (_state.wizards[0].hp);

            frame.Add(new Sprite(ESprite.blueStrip, new Vector2(Config.ScoreCorner + new Point2(separateScore/2,0)+ new Point2(0, Config.ScoreRectSize.y /2)),
                new Point2(separateScore, Config.ScoreRectSize.y)));
            frame.Add(new Sprite(ESprite.redStrip, new Vector2(Config.ScoreCorner + new Point2(separateScore, 0) + new Point2((Config.ScoreRectSize.x - separateScore) / 2, 0) + new Point2(0, Config.ScoreRectSize.y /2)),
                new Point2(Config.ScoreRectSize.x - separateScore, Config.ScoreRectSize.y)));

            double rightOffset =  Config.ScoreRectSize.x - (_state.wizards[1].name.Length + 2) * Config.LetterSize.x;
            frame.Add(new Text(EFont.orange, Config.ScoreCorner + new Point2(Config.LetterSize.x * 2, (Config.ScoreRectSize.y - Config.LetterSize.y) / 2) ,
                Config.LetterSize, _state.wizards[0].name));
            frame.Add(new Text(EFont.orange, Config.ScoreCorner + new Point2(rightOffset, (Config.ScoreRectSize.y - Config.LetterSize.y) / 2),
               Config.LetterSize, _state.wizards[1].name));
            
            #endregion



            frame.Add(new Text(EFont.orange, new Point2(0, 0), Config.LetterSize,
                keyboard.MousePosMap.x.ToString("N3"), keyboard.MousePosMap.y.ToString("N3")));
            return frame;
        }
        string Change(int x){
            if(x == 0) return "";
            return  "(" + (x>0? "+":"") + x.ToString()+ ")";
        }
    }
}
