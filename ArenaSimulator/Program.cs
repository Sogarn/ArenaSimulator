using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Unit redTeam = new Unit("Slammer");
            Unit blueTeam = new Unit("BuffBoi");
            // gain a bunch of levels
            for (int i = 0; i < 10; i++)
            {
                blueTeam.GainXP(250);
                redTeam.GainXP(250);
            }
            Combat.Battle(redTeam, blueTeam);
            Console.ReadLine();

            // TODO everything
        }
    }
}
