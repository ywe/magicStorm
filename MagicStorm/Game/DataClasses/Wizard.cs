using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.DataClasses
{
    class Wizard
    {
        public enum ETeam { first, second };
        public ETeam team;
        public int hp;
        public int hpChange = 0;
        public int[] flowers = new int[4];
        public int[] flowersChange = new int[4];
        public int pos;
        public string name;
        public Wizard(ETeam team)
        {
            for(int i = 0; i < flowers.Length;i++) flowers[i] = Config.StartRes;
            this.team = team;
            hp = Config.StartHP;
        }

        //это надо подписать и вызывать, когда новый ход только начался 
        public void EveryTurnReset()
        {
            hpChange = 0;
        }
    }
}
