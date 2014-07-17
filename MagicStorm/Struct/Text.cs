using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm
{
    struct Text
    {
        #region fields, properties
        EFont font;
        List<string> lines;
        Vector2 position;
        double letterWidth, letterHeight; 

        public List<string> Lines
        {
            get { return lines; }
            set { lines = value; }
        }
        public Vector2 Pos
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// ширина и высота всего текста
        /// </summary>
        public Point2 TextSize
        {
            get { return new Point2(letterWidth*maxLineLength() , letterHeight*lines.Count); }
            set
            {
                letterWidth = value.x / maxLineLength();
                letterHeight = value.y / lines.Count;
            }
        }
        /// <summary>
        /// ширина и высота одной буквы
        /// </summary>
        public Point2 LetterSize
        {
            get{return new Point2(letterWidth, letterHeight);}
            set{ 
                letterWidth = value.x; letterHeight = value.y;
            }
                
        }
        #endregion

        #region constructors
        /// <summary>
        /// Горизонтальный текст - задаем прямоугольник первой буквы. 
        /// </summary>
        public Text(EFont font, Vector2 firstLetterRect, params string[] lines)
        {
            this.lines = new List<string>(lines);
            this.font = font;
            this.letterWidth = firstLetterRect.vx;
            this.letterHeight = firstLetterRect.vy;
            this.position = new Vector2(firstLetterRect.x, firstLetterRect.y, 0);
        }
        /// <summary>
        /// Горизонтальный текст. Задается верхний левый угол, ширина и высота буквы 
        /// </summary>
        public Text(EFont font, Point2 location, Point2 letterSize, params string[] lines)
        {
            this.lines = new List<string>(lines);
            this.font = font;
            this.letterWidth = letterSize.x;
            this.letterHeight = letterSize.y;
            this.position = new Vector2( location.x, location.y, 0);
        }

        /// <summary>
        /// Горизонтальный текст. Задается верхний левый угол, ширина и высота буквы 
        /// </summary>
        public Text(EFont font, double x, double y,  double letterWidth, double letterHeight, params string[] lines)
        {
            this.lines = new List<string>(lines);
            this.font = font;
            this.letterWidth = letterWidth;
            this.letterHeight = letterHeight;
            this.position = new Vector2(x, y, 0);
        }

        /// <summary>
        /// Наклонный текст. Задается ширина и высота отдельной буквы, верхний левый угол
        /// </summary>
        public Text(EFont font, Vector2 position, Point2 letterSize, params string[] lines)
        {
            this.lines = new List<string>(lines);
            this.font = font;
            this.letterWidth = letterSize.x;
            this.letterHeight = letterSize.y;
            this.position = position;

            throw new NotImplementedException();//todo тут надо через центр все красиво повернуть
        }
        #endregion

        #region Вспомогательные для опенгл
        /// <summary>
        /// Чтобы отрисовать, нужно сначала координаты сдвинуть и повернуть на pos
        /// </summary>
        public List<Sprite> GetSpritesWithRelativePos()
        {
            List<Sprite> res = new List<Sprite>();

            Vector2 pos = new Vector2(position.x + TextSize.x / 2, position.y + TextSize.y / 2,position.angleDeg);

            for(int i = 0; i < lines.Count; i++)
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (Config.FontLetters.Contains(lines[i][j]))
                    {
                        Vector2 translation = new Vector2(0,0,-TextSize.x / 2 + j * letterWidth,
                            -TextSize.y / 2 + i * letterHeight);
                        translation.Rotate(pos.angleDeg);

                        Sprite toAdd = new Sprite(ESprite.end, new Vector2(pos.x + translation.vx, pos.y + translation.vy, pos.angleDeg), letterWidth, letterHeight, Config.FontLetters.IndexOf(lines[i][j]));
                        toAdd.texture = font.ToString();
                        /*
                        Sprite toAdd = new Sprite(ESprite.end, letterWidth, letterHeight,
                            new Vector2(-width / 2 + j * letterWidth, -height / 2 + i * letterHeight, 0),
                            Config.FontLetters.IndexOf(lines[i][j]));
                        */

                        res.Add(toAdd);
                    }
                    //иначе будет пустое место - пробел
                }

            return res;
        }

        

        int maxLineLength()
        {
            int r = 0;
            foreach (string s in lines)
                if (s.Length > r) r = s.Length;
            return r;

        }
        #endregion
    }
}
