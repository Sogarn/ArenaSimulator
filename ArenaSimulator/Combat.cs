using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator
{
    class Combat
    {
        // Basic battle
        public static void Battle(Unit red, Unit blue)
        {
            // Speed test
            // Whenever we are positive speed, Red goes and subtract Blue, and when negative, Blue goes and add Red
            int speedTest = red.Speed - blue.Speed;
            // Tiebreaker is skill stat then RNG
            bool redTurn = false;
            // Check they are both alive
            while (red.IsAlive() && blue.IsAlive())
            {
                // skip binary checks by defaulting redTurn to false and flipping it to true if conditions apply
                redTurn = false;
                // If speed is positive or wins tiebreaker, red goes
                if (speedTest > 0)
                {
                    redTurn = true;
                }
                else if (speedTest == 0)
                {
                    // Skill test for tiebreaker
                    if (red.Skill > blue.Skill)
                    {
                        redTurn = true;
                    }
                    else if (red.Skill == blue.Skill)
                    {
                        Random RNG = new Random();
                        redTurn = RNG.Next(0,2) == 0;
                    }
                }
                // Do turns
                if (redTurn)
                {
                    if (red.IsAlive())
                    {
                        red.OutgoingAttack(red.ActiveSkillsLearned[0], blue);
                        speedTest -= blue.Speed;
                    }
                }
                else
                {
                    if (blue.IsAlive())
                    {
                        blue.OutgoingAttack(blue.ActiveSkillsLearned[0], red);
                        speedTest += red.Speed;
                    }
                }
            }
        }
    }
}