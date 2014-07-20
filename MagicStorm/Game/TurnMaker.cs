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
    class TurnMaker
    {
        public string logfile = null;

        int[] currentTile = new int[] { 0, 0 };
        int[] currentCommand = new int[] { 0, 0 };
        int [] currentColor =new int[]{1,1};
        bool leftActive = true;


        public enum EStatus{ok, noOutput, unrealCommand, wrongFormat, timeLimit, runtimeError }
        EStatus status;
        /// <summary>
        /// если статус ок, то возвращает уже проверенный со всех сторон результат(т.е. команда, которая возможна)
        /// -1 - nothing, 0-14 - commands
        /// </summary>
        public void Process(bool getComamand, Wizard wizard, Wizard enemy, Tile[] tiles, int turnNumber, ref Frame frame
            , IGetKeyboardState keyboard, out int command, out int tile, out EStatus status )
        {
            int our = (int)wizard.team;
            int other = (our + 1) % 2;
            status = EStatus.ok;
            command = -1; tile = currentTile[our];

            //такой вот код когда спешишь
            if (getComamand&& currentCommand[our] > 50) currentCommand[our] -= 100;
            //-----
            Vector2 loc = Config.CommandCorner[our]; 
            frame.Add(new Sprite(ESprite.activePlayer, new Vector2(loc.x + loc.vx / 2, loc.y + loc.vy / 2, 0), new Point2(
                    loc.vx, loc.vy)));

            double texty = loc.y + loc.vy / 2;

            double leftSwitcher = Config.LetterSize.x * 2;
            double rightSwitcher = Config.LetterSize.x * 6;
            double width = Config.LetterSize.x * 8;

            if (getComamand == true)
            {
                if (wizard.programAddress == null)
                {
                    if (keyboard.GetActionTime(EKeyboardAction.left) == 1 ||
                    keyboard.GetActionTime(EKeyboardAction.right) == 1)
                    {
                        leftActive = !leftActive;
                        if (!IsThereParam(currentCommand[our])) leftActive = true;
                    }
                    if (keyboard.GetActionTime(EKeyboardAction.up) == 1)
                    {
                        if (leftActive)
                        {
                            currentCommand[our] = (currentCommand[our] + 1) % Config.COMMAND_COUNT;
                        }
                        else
                        {
                            if ((ECommand)currentCommand[our] == ECommand.changeColor)
                            {
                                currentColor[our]++;
                                if (currentColor[our] == 5) currentColor[our] = 1;
                            }
                            else
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
                            if ((ECommand)currentCommand[our] == ECommand.changeColor)
                            {
                                currentColor[our]--;
                                if (currentColor[our] == 0) currentColor[our] = 4;
                            }
                            else
                                currentTile[our] = (currentTile[our] - 1 + Config.TileCount) % Config.TileCount;
                        }
                    }
                    if (keyboard.GetActionTime(EKeyboardAction.Enter) == 1)
                    {
                        command = currentCommand[our];
                    }


                    double hor = leftActive ? leftSwitcher : rightSwitcher;

                    frame.Add(new Sprite(ESprite.switcher, new Vector2(hor + loc.x,
                        loc.y + (texty - Config.LetterSize.y / 2 - loc.y) / 2, 0), Config.SwitcherSize));
                    frame.Add(new Sprite(ESprite.switcher, new Vector2(hor + loc.x,
                        loc.y + loc.vy - (texty - Config.LetterSize.y / 2 - loc.y) / 2, 180), Config.SwitcherSize));

                    int markerPos = MarkerPos(wizard, enemy, currentCommand[our], currentTile[our]);
                    frame.Add(new Sprite(ESprite.tileMarker, new Vector2(
                                Config.FirstTilePos.x + Config.DistBetweenTile * markerPos, Config.IndLine, 0),
                                Config.MarkerSize));
                }
                else
                {
                    ExternalProgramExecuter executer = new ExternalProgramExecuter(wizard.programAddress, "input.txt", "output.txt");

                    string output, comment;
                    string input = GetInput(wizard, enemy, tiles);
                    ExternalProgramExecuteResult res = executer.Execute(input, Config.TimePerTurn,
                        out output, out comment);
                    wizard.savedNumbers.Clear();

                    Log(turnNumber, input, output);

                    if (res == ExternalProgramExecuteResult.InternalError)
                        status = EStatus.runtimeError;
                    else if (res == ExternalProgramExecuteResult.NotStarted ||
                        res == ExternalProgramExecuteResult.WriteInputError ||
                        res == ExternalProgramExecuteResult.OtherError)
                    {
                        throw new Exception("Внутренняя ошибка сервера");
                    }
                    else if (res == ExternalProgramExecuteResult.EmptyOutput ||
                        res == ExternalProgramExecuteResult.ReadOutputError)
                    {
                        status = EStatus.wrongFormat;
                    }
                    else if (res == ExternalProgramExecuteResult.NoOutput)
                    {
                        status = EStatus.noOutput;
                    }
                    else if (res == ExternalProgramExecuteResult.TimeOut)
                    {
                        status = EStatus.timeLimit;
                    }

                    if (status == EStatus.ok) //последняя проверка на содержимое
                    {
                        try
                        {
                            StringReader reader = new StringReader(output);
                            string[] ss = reader.ReadLine().Split(' ');
                            command = int.Parse(ss[0]);
                            tile = -1;
                            if(IsThereParam(command))
                                tile = int.Parse(ss[1]);
                            if (command < 0 || command >= Config.COMMAND_COUNT) throw new Exception();

                            if ((ECommand)command == ECommand.changeColor)
                            {
                                if (tile < 1 || tile >= 4) throw new Exception();
                            }
                            else
                            {
                                if (tile < 0 || tile >= Config.TileCount) throw new Exception();
                            }

                            string s = reader.ReadLine();
                            if (s != null && s.Length >= 1)
                            {
                                ss = s.Split(' ');
                                for (int i = 0; i < Math.Min(ss.Length, Config.MaxOutputNumbers); i++)
                                {
                                    wizard.savedNumbers.Add(int.Parse(ss[i]));
                                }
                            }

                        }
                        catch (Exception exc) //все, что поймалось - неверный формат
                        {
                            status = EStatus.wrongFormat;
                        }
                    }

                    if (status == EStatus.ok)
                    {
                        currentCommand[our] = command;
                        if ((ECommand)command == ECommand.changeColor)
                            currentColor[our] = tile;
                        else if (IsThereParam(command))
                        {
                            currentTile[our] = tile;
                        }
                    }
                    else
                    {
                        currentCommand[our] = -1;
                    }
                }
            }

            //может быть, что команда неправильная по логике игры, хотя все остальное в рамках
            if (getComamand && status == EStatus.ok && command != -1)
            {
                if (!IsCommandAvailable(command, tile, wizard, enemy, tiles))
                {
                    currentCommand[our] +=100; //:D потом не забыть вернуть
                    status = EStatus.unrealCommand;
                }
            }
            //---------

            if (currentCommand[our] == -1 || currentCommand[our] > 50)
            {
                frame.Add(new Text(EFont.green, true, new Point2(loc.vx/2 + loc.x, texty),
                        Config.LetterSize, "ERROR"));
            }
            else
            {
                frame.Add(new Text(EFont.green, true, new Point2(leftSwitcher + loc.x, texty),
                        Config.LetterSize, currentCommand[our].ToString()));
                if (IsThereParam(currentCommand[our]))
                {
                    int param = (ECommand)currentCommand[our] == ECommand.changeColor ? currentColor[our] : currentTile[our];
                    frame.Add(new Text(EFont.green, true, new Point2(rightSwitcher + loc.x, texty),
                        Config.LetterSize, param.ToString()));
                }
            }

            Vector2 notActiveRect = Config.CommandCorner[other];
            if (currentCommand[other] == -1 || currentCommand[other] > 50)
            {
                frame.Add(new Text(EFont.green, true, new Point2(notActiveRect.vx / 2 + notActiveRect.x, texty),
                        Config.LetterSize, "ERROR"));
            }
            else
            {
                //not active
                frame.Add(new Text(EFont.green, true, new Point2(leftSwitcher + notActiveRect.x, texty),
                    Config.LetterSize, currentCommand[other].ToString()));
                if (IsThereParam(currentCommand[other]))
                {
                    int param = (ECommand)currentCommand[other] == ECommand.changeColor ? currentColor[other] : currentTile[other];
                    frame.Add(new Text(EFont.green, true, new Point2(rightSwitcher + notActiveRect.x, texty),
                        Config.LetterSize, param.ToString()));
                }
            }

            if(our==1)
                tile = Config.TileCount - 1 - tile;//преобразовали обратно

            
        }

        bool IsThereParam(int command)
        {
            ECommand com = (ECommand)command;
            return !(com == ECommand.heal || com == ECommand.spy || com == ECommand.fist 
                ||com ==  ECommand.energyBall  || com ==  ECommand.boulder || com == ECommand.poison
                || com == ECommand.wind);
        }

        void Log(int turnNumber, string input, string output)
        {
            string r = "Номер хода : "+turnNumber.ToString() + "\r\n" + 
                "input.txt:\r\n"+
                input + "\r\n"+
                "output.txt:\r\n" +
                output +"\r\n";
            if (logfile != null)
            {
                File.AppendAllText(logfile, r);
            }
        }

        int MarkerPos(Wizard w, Wizard enemy, int command, int tile)
        {
            int r;
            ECommand com = (ECommand)command;
            if (com == ECommand.changeColor || com == ECommand.heal) r = w.pos;
            else if (!IsThereParam(command))
            {
                r = enemy.pos;
            }
            else
                r = w.team == Wizard.ETeam.first ? tile : Config.TileCount - 1 - tile;
            return r;
        }

        string GetInput(Wizard w, Wizard enemy, Tile[] tiles)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(w.hp.ToString() + " " + enemy.hp.ToString());

            int[] tileData =  new int[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                int data = tiles[i].growingTime == 0? tiles[i].color+1 : 0;
                if (w.pos == i) data = 10;
                if (enemy.pos == i) data = 20;

                tileData[w.team == Wizard.ETeam.first? i : Config.TileCount - 1 - i ] = data;
            }

            foreach (int a in tileData)
                sb.Append(a.ToString() + " ");
            sb.Remove(sb.Length - 1, 1);

            sb.AppendLine("");
            sb.AppendLine(w.savedNumbers.Count.ToString());

            foreach (int a in w.savedNumbers)
                sb.Append(a.ToString() + " ");
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        bool IsCommandAvailable(int command, int param, Wizard w, Wizard enemy, Tile[] tiles)
        {
            ECommand com = (ECommand)command;

            for (int i = 0; i < 4; i++)
                if (w.flowers[i] < Config.Cost[com][i])
                    return false;

            if(com != ECommand.changeColor)
                param = w.team == Wizard.ETeam.first? param: Config.TileCount-1-param;

            if (com == ECommand.move && param == w.pos ) return false;
            if(com == ECommand.fly && (
                (w.team == Wizard.ETeam.first && param >= w.pos) ||
                (w.team == Wizard.ETeam.second && param <= w.pos))) return false;
            if (com == ECommand.wall && (
                (w.team == Wizard.ETeam.first && tiles[param].wallLeft) ||
                (w.team == Wizard.ETeam.second && tiles[param].wallRight))) return false;
            //тут я подумал и решил, что кто-то захочет перезаморозить клетку, и закомментил
           // if (com == ECommand.frigidity && tiles[param].growingTime > Config.FlowerGrowingTime) return false;
            if (com == ECommand.explosion && (w.pos == param || enemy.pos == param)) return false;

            return true;
        }
    }
}
