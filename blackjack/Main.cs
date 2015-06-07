using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    class Main
    {
        Game game;
        ResultPersistence rp;
        public Main(Game game, ResultPersistence rp)
        {
            this.game = game;
            this.rp = rp;
        }

        public void Start()
        {
            rp.Save(game.Play());
        }
    }
}
