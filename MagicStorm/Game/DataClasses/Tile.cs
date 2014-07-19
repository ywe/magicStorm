using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.DataClasses
{
    class Tile
    {
        public static readonly ESprite[] SPRITES = new ESprite[] { ESprite.rectRed, ESprite.rectBlue, ESprite.rectGreen, ESprite.rectWhite };
        
        public int growingTime ; // если больше максимума, значит заморозка
        public int color; //0-3
        public bool wallLeft = false;
        public bool wallRight = false;
        public Tile(int color)
        {
            growingTime = 0;
            this.color = color;
        }

        //это надо подписать и вызывать, когда новый ход только начался 
        public void EveryTurnReset()
        {
            if (growingTime > 0) growingTime--;
        }
    }
}
