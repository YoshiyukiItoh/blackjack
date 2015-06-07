using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    interface ResultPersistence
    {
        public void Save(string playerName);
    }
}
