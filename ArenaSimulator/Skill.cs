﻿using System;
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
        StrongMagic // Stronger magic attack
    }

    // List of all passive spell names
    public enum PassiveSkills
    {
        BoostStrength, // Boost strength on attack
        ProtectAll, // Chance to block damage
        BoostMagic, // Boost magic on attack
        Recklessness, // Boost all stats in exchange for health
    }

    // Skills will be options useable in combat
    // Proc ability formulas will be in their own cast logic in the Unit class
    // Skill itself should never exist -- it is the base for Active and Passive skills
    abstract class Skill
    {
        protected string Name { get; set; }
        // Turns between uses
        protected int Cooldown { get; set; }
        protected int RemainingCooldown { get; set; }
        // Physical damage type
        protected bool Physical { get; set; }
        // Magical damage type
        protected bool Magical { get; set; }

        // Check cooldown
        public bool SkillReady()
        {
            return (RemainingCooldown == 0);
        }

        // Use skill
        public void UseSkill()
        {
            RemainingCooldown = Cooldown;
        }

        // Reset cooldowns
        public void ResetCooldown()
        {
            RemainingCooldown = 0;
        }

        // For outputting name + cooldown string
        public string DisplayCooldown()
        {
            // Set up a string based on cooldown status, either ready now or in X turns
            string check = SkillReady() ? "now" : string.Format("in {0} turns", RemainingCooldown);

            // Return name and cooldown status
            return string.Format("{0}, ready {1}", Name, check);
        }

        // For default string method return name
        public override string ToString()
        {
            return Name;
        }
    }

    // Active skills can be chosen on a given turn
    class ActiveSkill : Skill
    {
        // Internal skill name
        protected ActiveSkills InternalName { get; set; }
        // Damage it deals
        protected int BaseDamage { get; set; }

        // Will have custom constructor for every ID enum above
        public ActiveSkill(ActiveSkills skill)
        {
            switch (skill)
            {
                // First skill (melee attack)
                case ActiveSkills.BasicMelee:
                    InternalName = ActiveSkills.BasicMelee;
                    Name = "BasicMelee";
                    BaseDamage = 0;
                    Cooldown = 0;
                    RemainingCooldown = Cooldown;
                    Physical = true;
                    Magical = false;
                    break;
                // Stronger melee attack
                case ActiveSkills.StrongMelee:
                    InternalName = ActiveSkills.StrongMelee;
                    Name = "Overpower";
                    BaseDamage = 5;
                    Cooldown = 3;
                    RemainingCooldown = Cooldown;
                    Physical = true;
                    Magical = false;
                    break;
                // Magic auto attack
                case ActiveSkills.BasicMagic:
                    InternalName = ActiveSkills.BasicMagic;
                    Name = "BasicMagic";
                    BaseDamage = 0;
                    Cooldown = 0;
                    RemainingCooldown = Cooldown;
                    Physical = false;
                    Magical = true;
                    break;
                // Stronger magic attack
                case ActiveSkills.StrongMagic:
                    InternalName = ActiveSkills.StrongMagic;
                    Name = "Fireball";
                    BaseDamage = 5;
                    Cooldown = 3;
                    RemainingCooldown = Cooldown;
                    Physical = false;
                    Magical = true;
                    break;
                default:
                    break;
            }
        }
    }

    // Passive skills trigger automatically
    class PassiveSkill : Skill
    {
        // Internal skill name
        protected PassiveSkills InternalName { get; set; }
        // Offensive triggers on attacking, defensive is on getting attacked
        // Could add triggers for attack roll vs damage roll but currently all are on attack
        protected bool Offensive { get; set; }
        protected bool Defensive { get; set; }
        // Number of turns it lasts (TODO TBH this is going to probably be implemented with buffs / debuffs rather than here)
        protected int Duration { get; set; }
        protected int RemainingDuration { get; set; }
        // Will have custom constructor for every ID enum above
        public PassiveSkill(PassiveSkills skill)
        {
            switch (skill)
            {
                // Note Defensive is set automatically as the opposite of Offensive later on
                // Boost strength no cooldown
                case PassiveSkills.BoostStrength:
                    InternalName = PassiveSkills.BoostStrength;
                    Name = "Bolster";
                    Cooldown = 0;
                    Offensive = true;
                    Physical = true;
                    Magical = false;
                    break;
                // Boost magic no cooldown
                case PassiveSkills.BoostMagic:
                    InternalName = PassiveSkills.BoostMagic;
                    Name = "Study";
                    Cooldown = 0;
                    Offensive = true;
                    Physical = true;
                    Magical = false;
                    break;
                // Massively increase defense/res for one turn on getting attacked
                case PassiveSkills.ProtectAll:
                    InternalName = PassiveSkills.ProtectAll;
                    Name = "Deflect";
                    Cooldown = 1;
                    Offensive = false;
                    Physical = true;
                    Magical = true;
                    break;
                // Recklessness boosts all stats at the cost of health
                case PassiveSkills.Recklessness:
                    InternalName = PassiveSkills.Recklessness;
                    Name = "Recklessness";
                    Cooldown = 0;
                    Offensive = false;
                    Physical = true;
                    Magical = true;
                    break;
            }
            // Always set RemainingCooldown to 0 to start
            ResetCooldown();
            // Defensive vs offensive is always inverted. Keeping it as a variable for clarity though.
            Defensive = !Offensive;
        }
    }
}
