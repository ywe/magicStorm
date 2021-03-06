﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicStorm.Opengl;

namespace MagicStorm
{
    //вынесены для краткости кода
    //Доступные спрайты. end - чтобы можно было легко пробежать по всем
    public enum ESprite { menuback, blueStrip, redStrip, 
        activePlayer, switcher, tileMarker,
        wizardBlue, wizardPink, wizardSick, 
        arrowBlue, arrowRed, arrowGreen, arrowWhite,
        boulder, energyBall, explosion, eyes,
        fire, fist, lightning, plus, 
        rectBoardBright, rectBoardDark, 
        rectBlue, rectRed, rectGreen, rectWhite, rectBlack,
        wall, snowflake, windArrow, wings,
        orange2, pointRed,
        end }
    public enum EFont { orange, fiol, green, lilac, end }
    
    //действия, которые поддерживает клавиатура. Должны быть привязаны конкретные кнопки в конструкторе
    public enum EKeyboardAction { Fire, Esc, Enter, left, right, up, down, end };

    //чтобы убрать из игры команду, достаточно перенести ее в конец и уменьшить command_count
    public enum ECommand
    { 
        //neutral
        move = 0,
        fly = 1,
        wall = 2,
        heal = 3,
        spy = 4,
        changeColor = 5,
        frigidity = 6,

        //attack
        fist = 7,
        energyBall = 8,
        explosion = 9,
        boulder = 10,
        poison = 11,
        fire = 12,
        lightning = 13,
        wind = 14
    }
    class Config
    {
        public static int COMMAND_COUNT = 15;

        public static Dictionary<ECommand, int[]> Cost = new Dictionary<ECommand, int[]>
        {
            //red blue green white
            {ECommand.move , new int[]{0,0,0,0}},
            {ECommand.fly , new int[]{0,0,0,4}},
            {ECommand.wall , new int[]{0,7,0,0}},
            {ECommand.heal , new int[]{0,0,10,0}},
            {ECommand.spy , new int[]{1,1,1,1}},
            {ECommand.changeColor , new int[]{3,3,3,3}},
            {ECommand.frigidity , new int[]{0,9,0,0}},

            {ECommand.fist , new int[]{0,0,0,0}},
            {ECommand.energyBall , new int[]{2,2,2,2}},
            {ECommand.explosion , new int[]{12,0,0,0}},
            {ECommand.boulder , new int[]{0,0,20,0}},
            {ECommand.poison , new int[]{0,0,0,14}},
            {ECommand.fire , new int[]{31,0,0,0}},
            {ECommand.lightning , new int[]{0,26,0,0}},
            {ECommand.wind , new int[]{0,0,0,18}},
        };

        public static Dictionary<ECommand, int?> Damage = new Dictionary<ECommand, int?>
        {
            //red blue green white
            {ECommand.move , 0},
            {ECommand.fly , 0},
            {ECommand.wall , 0},
            {ECommand.heal , 6},
            {ECommand.spy , 0},
            {ECommand.changeColor , 0},
            {ECommand.frigidity , 0},

            {ECommand.fist , null}, //null - особый расчет
            {ECommand.energyBall , 3},
            {ECommand.explosion , 4},
            {ECommand.boulder , null},
            {ECommand.poison , 2},
            {ECommand.fire , 10},
            {ECommand.lightning , 25},
            {ECommand.wind , 0},
        };

        #region nested sprite config class
        public class SpriteConfig
        {
            public readonly string file;
            public  int horFrames, vertFrames;
            /// <summary>
            /// имя относительно екзешника, количество кадров по горизонтали и вертикали в файле
            /// </summary>
            public SpriteConfig(string file, int horFrames, int vertFrames)
            {
                this.file = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, file);
                this.horFrames = horFrames;
                this.vertFrames = vertFrames;
            }
            public SpriteConfig(string file) : this(file, 1, 1) { }
        }
        #endregion

        //Сюда кидаем и спрайты, и фонты. Ключ должен быть EFont.ToString() или ESprite.ToString()
        static public readonly Dictionary<string, SpriteConfig> Sprites = new Dictionary<string, SpriteConfig>();
        //сопоставили действия клавиатуры с конкретными клавишами
        static public readonly Dictionary<EKeyboardAction, byte> Keys= new Dictionary<EKeyboardAction,byte>();

        public static string WindowName = "Magic Storm"; 
        public const double ScreenWidth = 100;
        public const double ScreenHeight = 75;
        public const int TimePerFrame = 20; //в миллисекундах
        public static double AnimSpeed = 1.0; //стандартное время умножается на это число
        public const int TimePerTurn = 2000; //милилсекунды, время выполнения программы
        public const int MaxOutputNumbers = 5;

        #region game constants
        public static Point2 GameSize = new Point2(100, 35); //width better not to change
        public static int TileCount = 20;
        public static int FlowerGrowingTime = 5;
        public static int StartHP = 100;
        public static int StartRes= 100;
       // public static int StartPos = 9;
        public static double TileWidthToTileGapCoeff = 3.5;
        public static double TileHeightToTileWidthCoeff = 1.0;

        public static Vector2 TileFieldRect = new Vector2(1.5, 12.5, 97, 22.5);
        //calc-------------------------------------
        public static Point2 TileSize = new Point2(
            TileFieldRect.vx  / (TileCount  + (double)(TileCount+2)/TileWidthToTileGapCoeff),
            TileFieldRect.vx  / (TileCount  + (double)(TileCount+2)/TileWidthToTileGapCoeff)
            );
        public static Point2 FirstTilePos = new Point2( TileFieldRect.x + TileSize.x*(0.5 + 1.0/TileWidthToTileGapCoeff), 30);
        public static double DistBetweenTile = TileSize.x * (1 + 1.0 / TileWidthToTileGapCoeff);
        public static double TileGap = 1.0/TileWidthToTileGapCoeff*TileSize.x;
        //-----------------------------------------

        public static Point2 WizardSize = new Point2(TileSize.x, TileSize.x * 2);
        //Locations and labels--------------------------------
        public static double FloorLine = FirstTilePos.y - TileSize.y/2;
        public static double MenuLine = 12.5;
        public static double IndLine = FloorLine - WizardSize.y * 1.5;

        public static Point2 ScoreCorner = new Point2(25,3.5);
        public static Point2 LetterSize = new Point2(1, 2);
        public static Point2 LetterSizeBig = new Point2(2.5, 4);
        public static double PlayerInfoLine = 1;
        public static double PlayerInfoLeft = 5;
        public static double PlayerInfoRight = 84;
        public static double TeamsAndTimeLine = 1 ;
        public static Point2 ScoreRectSize = new Point2(50, 3);

        public static Vector2[] CommandCorner = new Vector2[] { new Vector2(33, 7.5, 8, 4), new Vector2(60, 7.5, 8, 4) }; 
        //----------------------------------

        //game sprite sizes--------------------------------------
        public static Point2 TileBorderSize = new Point2(TileSize.x, TileSize.y );
        public static double WizardVert = WizardSize.y / 10;
        public static Point2 ArrowSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 FistSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 EyeSize = new Point2(TileSize.x/2, TileSize.x / 4);
        public static Point2 WingsSize = new Point2(TileSize.x*2, TileSize.y );
        public static double WingsVert = -WizardSize.y / 128 * 5;
        public static Point2 FireSize = new Point2(TileSize.x * 3 + TileGap*2, TileSize.x);
        public static Point2 LightningSize = new Point2(TileSize.x * 1.8, TileSize.x * 1.8);
        public static double LightningHor = -LightningSize.x / 8;
        public static Point2 WallSize = new Point2(TileGap/2, TileSize.y * 3);
        public static double WallVert = 0;
        public static Point2 BoulderSize = new Point2(WizardSize.y, WizardSize.y);
        public static Point2 EnergyBallSize = new Point2(TileSize.x / 3, TileSize.x / 3);
        public static Point2 ExplosionSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 PointSize = new Point2(0.2, 0.2);
        public static Point2 WindArrowSize = new Point2(TileSize.x, TileSize.x/3);
        public static Point2 PlusSize = new Point2(TileSize.x/2, TileSize.x/2);
        public static Point2 SwitcherSize = new Point2(2, 0.6);
        public static Point2 MarkerSize = new Point2(1, 1);
        //----------------------------------------------------


        #endregion

        static Config()
        {
            LoadSpritesAuto();
            Sprites[ESprite.orange2.ToString()].horFrames = 16;
            Sprites[ESprite.orange2.ToString()].vertFrames = 10;
            Sprites[ESprite.explosion.ToString()].horFrames = 9;
            Sprites[ESprite.explosion.ToString()].vertFrames = 9;
            Keys.Add(EKeyboardAction.Fire, 32);
            Keys.Add(EKeyboardAction.Esc, 27);
            Keys.Add(EKeyboardAction.Enter, 13);
            Keys.Add(EKeyboardAction.left, 37);
            Keys.Add(EKeyboardAction.up, 38);
            Keys.Add(EKeyboardAction.right, 39);
            Keys.Add(EKeyboardAction.down, 40);
        }

        //потом подправить кадры элементарно, если анимация
        static void LoadSpritesAuto()
        {
            for (ESprite i = (ESprite)0; i != ESprite.end; i++)
            {
                Sprites.Add(i.ToString(), new SpriteConfig("tex//"+i.ToString()+".png", 1, 1));
            }
            for (EFont i = (EFont)0; i != EFont.end; i++)
            {
                Sprites.Add(i.ToString(), new SpriteConfig("fonts//" + i.ToString() + ".png", 16, 10));
            }
        }

        public const string FontLetters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя!@#$%^&*()_+=,./?<>[]\{}|1234567890~`‘“№→-";



    }
}
