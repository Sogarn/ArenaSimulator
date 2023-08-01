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
            Unit blueTeam = new Unit("BuffBoi");
            Unit redTeam = new Unit("Slammer");
            // gain a bunch of levels
            for (int i = 0; i < 10; i++)
            {
                blueTeam.GainXP(250);
                redTeam.GainXP(250);
            }
            while (blueTeam.IsAlive() && redTeam.IsAlive())
            {
                if (blueTeam.IsAlive())
                {
                    blueTeam.OutgoingAttack(blueTeam.ActiveSkillsLearned[0], redTeam);
                }
                if (redTeam.IsAlive())
                {
                    redTeam.OutgoingAttack(redTeam.ActiveSkillsLearned[0], blueTeam);
                }
            }
            Console.ReadLine();

            // TODO everything
        }
    }
}
