using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator
{
    // Skills will be options useable in combat
    // Physical procs triggered by physical abilities and magical by magic
    class Skill
    {
        // Turns between uses
        protected int Cooldown { get; set; }
        // Damage it deals
        protected int BaseDamage { get; set; }
        // Number of turns it lasts
        protected int Duration { get; set; }
        // Physical damage type
        protected bool Physical { get; set; }
        // Magical damage type
        protected bool Magical { get; set; }
    }

    // Active skills can be chosen on a given turn
    class ActiveSkill : Skill
    {

    }

    // Passive skills trigger automatically
    class PassiveSkill : Skill
    {

    }
}
