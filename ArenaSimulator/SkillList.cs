using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator
{
    // List of all active spell names
    public enum ActiveSkills
    {
        BasicMelee, // Basic melee auto
        BasicMagic, // Basic magic auto
        StrongMelee, // Stronger melee attack
        StrongMagic, // Stronger magic attack
        DoubleMelee, // Melee twice at an accuracy penalty
        DoubleMagic, // Magic twice at an accuracy penalty
        MagicBurning, // Magic that gives burning debuff
        MagicPoison, // Magic that gives poison debuff
        MeleeBleeding, // Melee that gives bleed debuff
        Hasten, // Reduce all other cooldowns and increase speed
        Purify, // Remove all debuffs
    }

    // List of all passive spell names
    public enum PassiveSkills
    {
        BoostStrength, // Boost strength and defense on attack
        ProtectAll, // Chance to block next hit
        BoostMagic, // Boost magic and resistance on attack
        Sacrifice, // Boost all stats in exchange for health
        BoostSkill, // Boost skill and speed on attack
        Cleanse, // Chance to cure the highest damaging debuff
        ReactiveLuck, // Boost HP and Luck when attacked
    }
}
