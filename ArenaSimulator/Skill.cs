using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaSimulator
{
    // Skills will be options useable in combat
    // Proc ability formulas will be in their own cast logic in the Unit class
    // Skill itself should never exist -- it is the base for Active and Passive skills
    abstract class Skill
    {
        protected string Name { get; set; }
        // Turns between uses
        protected int Cooldown { get; set; }
        protected int RemainingCooldown { get; set; }
        // Damage scaling Physical
        public bool StrengthScaling { get; protected set; }
        // Damage scaling Magical
        public bool MagicScaling { get; protected set; }
        // Attacks vs defense
        public bool DefenseResist { get; protected set; }
        // Attacks vs resistance
        public bool ResistanceResist { get; protected set; }

        // Default constructor
        public Skill()
        {
            // Set all damage types to false so that we can just flag trues
            StrengthScaling = false;
            MagicScaling = false;
            DefenseResist = false;
            ResistanceResist = false;
            // Remaining cooldown always starts at 0
            RemainingCooldown = 0;
        }

        // Check cooldown
        public bool SkillReady()
        {
            return (RemainingCooldown == 0);
        }

        // Use skill
        public virtual void UseSkill()
        {
            RemainingCooldown = Cooldown;
        }

        // Reduce skill cooldowns by one
        public void TurnCooldownUpdate()
        {
            // Lower remaining cooldown by 1 if possible
            RemainingCooldown = (RemainingCooldown > 0 ? RemainingCooldown - 1 : 0);
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
        public ActiveSkills InternalName { get; protected set; }
        // Damage it deals as a multiplier
        public float BaseDamage { get; protected set; }
        // Base accuracy is added to unit accuracy and ranges from +0% to +50%
        public int BaseAccuracy { get; protected set; }

        // Will have custom constructor for every ID enum above
        public ActiveSkill(ActiveSkills skill)
        {
            switch (skill)
            {
                // First skill (melee attack)
                case ActiveSkills.BasicMelee:
                    InternalName = ActiveSkills.BasicMelee;
                    Name = "BasicMelee";
                    BaseDamage = 1f;
                    BaseAccuracy = 0;
                    Cooldown = 0;
                    StrengthScaling = true;
                    DefenseResist = true;
                    break;
                // Stronger melee attack
                case ActiveSkills.StrongMelee:
                    InternalName = ActiveSkills.StrongMelee;
                    Name = "Overpower";
                    BaseDamage = 2f;
                    BaseAccuracy = 50;
                    Cooldown = 4;
                    StrengthScaling = true;
                    DefenseResist = true;
                    break;
                // Magic auto attack
                case ActiveSkills.BasicMagic:
                    InternalName = ActiveSkills.BasicMagic;
                    Name = "BasicMagic";
                    BaseDamage = 1f;
                    BaseAccuracy = 0;
                    Cooldown = 0;
                    MagicScaling = true;
                    ResistanceResist = true;
                    break;
                // Stronger magic attack
                case ActiveSkills.StrongMagic:
                    InternalName = ActiveSkills.StrongMagic;
                    Name = "Arcane Blast";
                    BaseDamage = 2f;
                    BaseAccuracy = 50;
                    Cooldown = 4;
                    MagicScaling = true;
                    ResistanceResist = true;
                    break;
                // // Melee twice at an accuracy penalty
                case ActiveSkills.DoubleMelee:
                    InternalName = ActiveSkills.DoubleMelee;
                    Name = "Dual strike";
                    BaseDamage = 0.9f;
                    BaseAccuracy = 0;
                    Cooldown = 2;
                    StrengthScaling = true;
                    DefenseResist = true;
                    break;
                // Magic twice at an accuracy penalty
                case ActiveSkills.DoubleMagic:
                    InternalName = ActiveSkills.DoubleMagic;
                    Name = "Twincast";
                    BaseDamage = 0.9f;
                    BaseAccuracy = 0;
                    Cooldown = 2;
                    MagicScaling = true;
                    ResistanceResist = true;
                    break;
                // Magic that gives burning debuff (repeat net damage for 2 turns)
                case ActiveSkills.MagicBurning:
                    InternalName = ActiveSkills.MagicBurning;
                    Name = "Fireball";
                    BaseDamage = 1.1f;
                    BaseAccuracy = 25;
                    Cooldown = 3;
                    MagicScaling = true;
                    ResistanceResist = true;
                    break;
                // Magic that gives poison debuff (deal its base damage ignoring defenses for 4 turns)
                case ActiveSkills.MagicPoison:
                    InternalName = ActiveSkills.MagicPoison;
                    Name = "Noxious Fumes";
                    BaseDamage = 0.5f;
                    BaseAccuracy = 25;
                    Cooldown = 3;
                    MagicScaling = true;
                    ResistanceResist = true;
                    break;
                // Melee that gives bleed debuff (deal its base damage ignoring defenses for 4 turns)
                case ActiveSkills.MeleeBleeding:
                    InternalName = ActiveSkills.StrongMagic;
                    Name = "Rupture";
                    BaseDamage = 0.5f;
                    BaseAccuracy = 25;
                    Cooldown = 3;
                    StrengthScaling = true;
                    DefenseResist = true;
                    break;
                // Reduce all other cooldowns and increase speed
                case ActiveSkills.Hasten:
                    InternalName = ActiveSkills.Hasten;
                    Name = "Hasten";
                    Cooldown = 5;
                    break;
                // Remove all debuffs
                case ActiveSkills.Purify:
                    InternalName = ActiveSkills.Purify;
                    Name = "Purify";
                    Cooldown = 3;
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
        public PassiveSkills InternalName { get; protected set; }
        // How this proc scales with skill (1 = 100% of skill stat)
        public float SkillCoefficient { get; protected set; }
        // Offensive triggers on attacking, defensive triggers on getting attacked
        public bool Offensive { get; protected set; }
        public bool Defensive { get; protected set; }
        // Number of turns it lasts (TODO TBH this is going to probably be implemented with buffs / debuffs rather than here)
        protected int Duration { get; set; }
        protected int RemainingDuration { get; set; }
        // Will have custom constructor for every ID enum above
        public PassiveSkill(PassiveSkills skill)
        {
            switch (skill)
            {
                // Note Defensive is set automatically as the opposite of Offensive later on
                // Boost strength and defense low cooldown
                case PassiveSkills.BoostStrength:
                    InternalName = PassiveSkills.BoostStrength;
                    Name = "Bolster";
                    Cooldown = 2;
                    Offensive = true;
                    SkillCoefficient = 1.5f;
                    break;
                // Boost magic and res low cooldown
                case PassiveSkills.BoostMagic:
                    InternalName = PassiveSkills.BoostMagic;
                    Name = "Incanter's Flow";
                    Cooldown = 2;
                    Offensive = true;
                    SkillCoefficient = 1f;
                    break;
                // Gain immunity from next damage source
                case PassiveSkills.ProtectAll:
                    InternalName = PassiveSkills.ProtectAll;
                    Name = "Deflect";
                    Cooldown = 3;
                    Offensive = false;
                    SkillCoefficient = 0.5f;
                    break;
                // Sacrifice boosts all stats at the cost of health
                case PassiveSkills.Sacrifice:
                    InternalName = PassiveSkills.Sacrifice;
                    Name = "Dark Pact";
                    Cooldown = 2;
                    Offensive = true;
                    SkillCoefficient = 0.75f;
                    break;
                // Boost skill and speed low cooldown
                case PassiveSkills.BoostSkill:
                    InternalName = PassiveSkills.BoostSkill;
                    Name = "The Thrill";
                    Cooldown = 2;
                    Offensive = true;
                    SkillCoefficient = 0.5f;
                    break;
                // Chance to cure highest damaging debuff on attack
                case PassiveSkills.Cleanse:
                    InternalName = PassiveSkills.Cleanse;
                    Name = "Cleanse";
                    Cooldown = 3;
                    Offensive = true;
                    SkillCoefficient = 1f;
                    break;
                // Boost HP and Luck
                case PassiveSkills.ReactiveLuck:
                    InternalName = PassiveSkills.ReactiveLuck;
                    Name = "Divine Favor";
                    Cooldown = 3;
                    Offensive = false;
                    SkillCoefficient = 1f;
                    break;
            }
            // Always set RemainingCooldown to 0 to start
            ResetCooldown();
            // Defensive vs offensive is always inverted. Keeping it as a variable for clarity though.
            Defensive = !Offensive;
        }
    }
}
