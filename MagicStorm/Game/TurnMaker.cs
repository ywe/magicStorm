using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;
using MagicStorm.Game.Concrete;
using MagicStorm.Game.DataClasses;

namespace MagicStorm.Game
{
    class TurnMaker
    {
        int[] currentTile = new int[] { 0, 0 };
        int[] currentCommand = new int[] { 0, 0 };
        //string[] current = new string[]{" 0   0", " 0   0"};
        bool leftActive = true;
        /// <summary>
        /// -1 - nothing, 0-14 - commands, 20 - not enough resources, 21 - unreal command, 
        /// 22 - wrong format, 23 - time limit, 24 - runtime error
        /// </summary>
        public void Process(Wizard wizard,  ref Frame frame, IGetKeyboardState keyboard, out int command, out int tile)
        {
            int our = (int)wizard.team;
            int other = (our + 1) % 2;
            command = -1; tile = currentTile[our];

            if (keyboard.GetActionTime(EKeyboardAction.left) == 1 ||
                keyboard.GetActionTime(EKeyboardAction.right) == 1)
            {
                leftActive = !leftActive;
            }
            if (keyboard.GetActionTime(EKeyboardAction.up) == 1)
            {
                if (leftActive)
                {
                    currentCommand[our] = (currentCommand[our] + 1) % Config.COMMAND_COUNT;
                }
                else
                {
                    currentTile[our] = (currentTile[our] + 1) % Config.TileCount;
                }
            }
            if (keyboard.GetActionTime(EKeyboardAction.down) == 1)
            {
                if (leftActive)
                {
                    currentCommand[our] = (currentCommand[our] - 1 + Config.COMMAND_COUNT) % Config.COMMAND_COUNT;
                }
                else
                {
                    currentTile[our] = (currentTile[our] - 1 + Config.TileCount) % Config.TileCount;
                }
            }
            if (keyboard.GetActionTime(EKeyboardAction.Enter)==1)
            {
                command = currentCommand[our];
            }



            Vector2 loc = Config.CommandCorner[our];
            if (wizard.programAddress == null)
            {
                double texty = loc.y + loc.vy / 2 ;

                double leftSwitcher = Config.LetterSize.x * 2;
                double rightSwitcher = Config.LetterSize.x * 6;
                double width = Config.LetterSize.x*8;
                frame.Add(new Sprite(ESprite.activePlayer, new Vector2(loc.x+ loc.vx/2, loc.y+loc.vy/2, 0), new Point2(
                    loc.vx, loc.vy)));

                double hor = leftActive ? leftSwitcher : rightSwitcher;
                
                frame.Add(new Sprite(ESprite.switcher, new Vector2(hor + loc.x,
                    loc.y + (texty-Config.LetterSize.y/2- loc.y) / 2, 0), Config.SwitcherSize));
                frame.Add(new Sprite(ESprite.switcher, new Vector2(hor + loc.x,
                    loc.y + loc.vy - (texty - Config.LetterSize.y / 2 - loc.y) / 2, 180), Config.SwitcherSize));
               

                frame.Add(new Text(EFont.green, true, new Point2(leftSwitcher + loc.x, texty),
                    Config.LetterSize, currentCommand[our].ToString()));
                frame.Add(new Text(EFont.green, true,new Point2(rightSwitcher + loc.x, texty),
                    Config.LetterSize, currentTile[our].ToString()));

                //not active
                Vector2 notActiveRect = Config.CommandCorner[other];
                frame.Add(new Text(EFont.green, true,new Point2(leftSwitcher + notActiveRect.x, texty),
                    Config.LetterSize, currentCommand[other].ToString()));
                frame.Add(new Text(EFont.green, true,new Point2(rightSwitcher + notActiveRect.x, texty),
                    Config.LetterSize, currentTile[other].ToString()));

            }

            
        }
    }
}
