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
        protected string Name { get; set; }
        // Damage it deals
        protected int BaseDamage { get; private set; }
        // Turns between uses
        protected int Cooldown { get; private set; }
        protected int RemainingCooldown { get; set; }
        // Number of turns it lasts (TODO anything longer than same-turn)
        protected int Duration { get; private set; }
        protected int RemainingDuration { get; set; }
        // Physical damage type
        protected bool Physical { get; private set; }
        // Magical damage type
        protected bool Magical { get; private set; }

        // Default constructor for any skill
        public Skill(string name = "", int baseDamage = 0, int cooldown = 0, int duration = 0, bool physical = true, bool magical = false)
        {
            Name = name;
            BaseDamage = baseDamage;
            Cooldown = cooldown;
            RemainingCooldown = Cooldown;
            Duration = duration;
            RemainingDuration = Duration;
            Physical = physical;
            Magical = magical;
        }

        // The building block of all abilities: input player and target states before ability, return player and target states after ability
        public virtual Unit[] ActivateSkill(Unit player, Unit target) { return new Unit[2]; }
    }

    // Active skills can be chosen on a given turn
    class ActiveSkill : Skill
    {
        // Will probably have some custom features but until then we'll just extend this straight out with custom Activates on each ability
    }

    // Passive skills trigger automatically
    class PassiveSkill : Skill
    {
        // Offensive triggers on attacking, defensive triggers on defending
        protected bool Offensive { get; private set; }
        protected bool Defensive { get; private set; }
        // Will probably have some custom features but until then we'll just extend this straight out with custom Activates on each ability
    }

}
