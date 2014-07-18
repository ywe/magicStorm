using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.DataClasses
{
    class State
    {
        public enum EStage { turn, animation, runCommand, finish }
        public EStage stage = EStage.turn;
        public Tile[] tiles ;
        public bool[] walls;
        public Wizard[] wizards;
        public int activeWizard = 0;

        public State()
        {
            tiles = new Tile[Config.TileCount];
            int curColor = 0;
            for (int i = 0; i < Config.TileCount / 2; i++)
            {
                tiles[i]= new Tile(curColor); 
                tiles[Config.TileCount-1-i] = new Tile(curColor);
                curColor = (curColor + 1) % 4;
            }

            wizards = new Wizard[2];
            wizards[0] = new Wizard(Wizard.ETeam.first);
            wizards[1] = new Wizard(Wizard.ETeam.second);
            wizards[0].pos = 3;
            wizards[1].pos = 16;
            wizards[0].name = "я слева";
            wizards[1].name = "never will be killed";
        }
    }
}
