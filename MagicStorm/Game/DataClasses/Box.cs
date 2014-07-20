using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.DataClasses
{
    class Box
    {
        public bool sameTime=false; //with previous
        public int id;
        public int first = -1; //params, e.g. hp change; digit and so on
        public int second = -1;
        public int third = -1;
        public enum EType { fireIn, fireOut, digit, attackFire, changeHp }
        public EType type;

        public Box(EType type)
        {
            this.type = type;
        }
    }
}
