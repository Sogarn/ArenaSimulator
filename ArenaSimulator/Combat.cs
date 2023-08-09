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
            // Turn counter
            int turn = 0;
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
                turn += 1;
                // Do turns
                if (redTurn)
                {
                    if (red.IsAlive())
                    {
                        Console.WriteLine("Turn {0}: {1}", turn, red.Name);
                        // Reduces cooldowns
                        red.NextTurn();
                        // Attacks with a randomly available attack
                        red.InitialAttack(red.GetNextAttack(), blue);
                        speedTest -= blue.Speed;
                    }
                }
                else
                {
                    if (blue.IsAlive())
                    {
                        Console.WriteLine("Turn {0}: {1}", turn, blue.Name);
                        blue.NextTurn();
                        blue.InitialAttack(blue.GetNextAttack(), red);
                        speedTest += red.Speed;
                    }
                }
            }
        }
    }
}