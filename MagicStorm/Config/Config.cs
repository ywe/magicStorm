using System;
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
        wizardBlue, wizardPink, arrowBlue, arrowRed, 
        boulder, energyBall, explosion, eyes,
        fire, fist, lightning, plus, 
        rectBoardBright, rectBoardDark, 
        rectBlue, rectRed, rectGreen, rectWhite,
        rectWall, snowflake, windArrow, wings,
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


        #region nested sprite config class
        public class SpriteConfig
        {
            public readonly string file;
            public readonly int horFrames, vertFrames;
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

        public static string WindowName = "2D Framework"; 
        public const double ScreenWidth = 100;
        public const double ScreenHeight = 75;
        public const int TimePerFrame = 20; //в миллисекундах
        
        #region game constants
        public static Point2 GameSize = new Point2(100, 35); //width better not to change
        public static int TileCount = 20;
        public static int FlowerGrowingTime = 5;
        public static int StartHP = 100;
        public static int StartRes= 10;
        public static int StartPos = 9;
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
        public static double TileGap = 1.0/TileWidthToTileGapCoeff;
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
        public static Point2 EyeSize = new Point2(TileSize.x, TileSize.x / 2);
        public static Point2 WingsSize = new Point2(TileSize.x, TileSize.y / 2);
        public static double WingsVert = - WizardSize.y / 128 * 44;
        public static Point2 FireSie = new Point2(TileSize.x * 3 + TileGap*2, TileSize.x);
        public static Point2 LightningSize = new Point2(TileSize.x * 1.8, TileSize.x * 1.8);
        public static double LightningHor = LightningSize.x / 8;
        public static Point2 WallSize = new Point2(TileGap, TileGap * 8);
        public static double WallVert = 0;
        public static Point2 BoulderSize = new Point2(WizardSize.y, WizardSize.y);
        public static Point2 EnergyBallSize = new Point2(TileSize.x / 3, TileSize.x / 3);
        public static Point2 ExplosionSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 WindArrowSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 PlusSize = new Point2(TileSize.x, TileSize.x);
        public static Point2 SwitcherSize = new Point2(2, 0.6);
        public static Point2 MarkerSize = new Point2(1, 1);
        //----------------------------------------------------


        #endregion

        static Config()
        {
            LoadSpritesAuto();

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
