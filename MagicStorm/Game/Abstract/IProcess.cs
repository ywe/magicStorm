using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicStorm.Game.Abstract
{
    interface IProcess
    {
        void Process(ref Frame frame);
    }
}
