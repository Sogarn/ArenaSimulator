using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// TODO probably need to add variables for debuffs like Unit.Poison bool and its severity
namespace ArenaSimulator
{
    // Base unit class
    class Unit
    {
        // Decided for stats to go negative for combat mechanics and deleveling purposes
        protected Random RNG;
        public string Name { get; private set; }
        // Level will start at 0
        protected int Level { get; private set; }
        protected int XPPerLevel { get; private set; }
        protected int XPToNextLevel { get; private set; }
        // Max active skill count starts at 2 and increases with level
        protected int MaxActiveSkills = 2;
        // List of active skills
        protected List<ActiveSkill> ActiveSkillsLearned = new List<ActiveSkill>();
        // List of unlearned active skills
        protected List<string> ActiveSkillsAvailable = Enum.GetNames(typeof(ActiveSkills)).ToList();
        // Max passive skill count starts at 1 and increases with level
        protected int MaxPassiveSkills = 1;
        // List of passive skills
        protected List<PassiveSkill> PassiveSkillsLearned = new List<PassiveSkill>();
        // List of unlearned passive skills
        protected List<string> PassiveSkillsAvailable = Enum.GetNames(typeof(PassiveSkills)).ToList();
        // TODO add buffs and debuffs
        // Our 8 stats are HP, Strength, Defense, Magic, Resistance, Speed, Skill, and Luck
        // Base stats start at 10 and increase on level up
        protected int BaseHP { get; private set; }
        protected int BaseStrength { get; private set; }
        protected int BaseDefense { get; private set; }
        protected int BaseMagic { get; private set; }
        protected int BaseResistance { get; private set; }
        protected int BaseSpeed { get; private set; }
        protected int BaseSkill { get; private set; }
        protected int BaseLuck { get; private set; }
        // Current stats fluctuate and reset on level up
        public int HP { get; private set; }
        public int Strength { get; private set; }
        public int Defense { get; private set; }
        public int Magic { get; private set; }
        public int Resistance { get; private set; }
        public int Speed { get; private set; }
        public int Skill { get; private set; }
        public int Luck { get; private set; }
        // Growth stats range from 1 to 100 and are the percentage chance of gaining a stat on level up
        public int GrowthHP { get; private set; }
        public int GrowthStrength { get; private set; }
        public int GrowthDefense { get; private set; }
        public int GrowthMagic { get; private set; }
        public int GrowthResistance { get; private set; }
        public int GrowthSpeed { get; private set; }
        public int GrowthSkill { get; private set; }
        public int GrowthLuck { get; private set; }

        public Unit(string name)
        {
            RNG = new Random();
            Level = 0;
            XPPerLevel = 255;
            XPToNextLevel = XPPerLevel;
            // Set all base stats to default values (10)
            InitializeBaseStats();
            // Set all current stats to base stats
            ResetCurrentStats();
            // Set all growths to default values (10)
            InitializeGrowths();
            // Level up once
            LevelUp();
            // Get basic autoattacks
            AddActiveSkill(ActiveSkills.BasicMelee);
            AddActiveSkill(ActiveSkills.BasicMagic);
        }

        // In case we want custom base stats later
        protected virtual void InitializeBaseStats(int defaultStat = 10)
        {
            // Fill with default stats
            BaseHP = defaultStat;
            BaseStrength = defaultStat;
            BaseDefense = defaultStat;
            BaseMagic = defaultStat;
            BaseResistance = defaultStat;
            BaseSpeed = defaultStat;
            BaseSkill = defaultStat;
            BaseLuck = defaultStat;
        }

        // Default growths
        protected virtual void InitializeGrowths(int defaultGrowth = 20)
        {
            // Fill with default growths
            GrowthHP = defaultGrowth;
            GrowthStrength = defaultGrowth;
            GrowthDefense = defaultGrowth;
            GrowthMagic = defaultGrowth;
            GrowthResistance = defaultGrowth;
            GrowthSpeed = defaultGrowth;
            GrowthSkill = defaultGrowth;
            GrowthLuck = defaultGrowth;
        }

        // Reset current stats to base values
        protected void ResetCurrentStats()
        {
            // Copy base stats to current stats
            HP = BaseHP;
            Strength = BaseStrength;
            Defense = BaseDefense;
            Magic = BaseMagic;
            Resistance = BaseResistance;
            Speed = BaseSpeed;
            Skill = BaseSkill;
            Luck = BaseLuck;
            // Reset all skill cooldowns
            foreach(ActiveSkill skill in ActiveSkillsLearned)
            {
                skill.ResetCooldown();
            }
            foreach (PassiveSkill skill in PassiveSkillsLearned)
            {
                skill.ResetCooldown();
            }
        }

        // Gain XP
        public void GainXP(int value)
        {
            // Keep leveling up until require more than 0 xp to level again
            XPToNextLevel -= value;
            while (XPToNextLevel <= 0)
            {
                XPToNextLevel += XPPerLevel;
                LevelUp();
            }
        }

        // Level up sequence
        protected void LevelUp()
        {
            Console.WriteLine("Leveled up! Level {0} -> {1}", Level, Level + 1);
            Level += 1;

            // Add 1 to base stat if we rolled equal to or below the growth stat
            // If we did not gain a stat, roll luck for bad luck protection
            #region Stat growth RNG
            // If LevelUpLuckRoll has any length then luck roll passed
            if (GetRNG() <= GrowthHP)
            {
                Console.WriteLine("Gained HP! {0} -> {1}", BaseHP, BaseHP + 1);
                BaseHP += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! HP {0} -> {1}", BaseHP, BaseHP + 1);
                BaseHP += 1;
            }
            if (GetRNG() <= GrowthStrength)
            {
                Console.WriteLine("Gained Strength! {0} -> {1}", BaseStrength, BaseStrength + 1);
                BaseStrength += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Strength {0} -> {1}", BaseStrength, BaseStrength + 1);
                BaseStrength += 1;
            }
            if (GetRNG() <= GrowthDefense)
            {
                Console.WriteLine("Gained Defense! {0} -> {1}", BaseDefense, BaseDefense + 1);
                BaseDefense += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Defense {0} -> {1}", BaseDefense, BaseDefense + 1);
                BaseDefense += 1;
            }
            if (GetRNG() <= GrowthMagic)
            {
                Console.WriteLine("Gained Magic! {0} -> {1}", BaseMagic, BaseMagic + 1);
                BaseMagic += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Magic {0} -> {1}", BaseMagic, BaseMagic + 1);
                BaseMagic += 1;
            }
            if (GetRNG() <= GrowthResistance)
            {
                Console.WriteLine("Gained Resistance! {0} -> {1}", BaseResistance, BaseResistance + 1);
                BaseResistance += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Resistance {0} -> {1}", BaseResistance, BaseResistance + 1);
                BaseResistance += 1;
            }
            if (GetRNG() <= GrowthSpeed)
            {
                Console.WriteLine("Gained Speed! {0} -> {1}", BaseSpeed, BaseSpeed + 1);
                BaseSpeed += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Speed {0} -> {1}", BaseSpeed, BaseSpeed + 1);
                BaseSpeed += 1;
            }
            if (GetRNG() <= GrowthSkill)
            {
                Console.WriteLine("Gained Skill! {0} -> {1}", BaseSkill, BaseSkill + 1);
                BaseSkill += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Skill {0} -> {1}", BaseSkill, BaseSkill + 1);
                BaseSkill += 1;
            }
            if (GetRNG() <= GrowthLuck)
            {
                Console.WriteLine("Gained Luck! {0} -> {1}", BaseLuck, BaseLuck + 1);
                BaseLuck += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Luck {0} -> {1}", BaseLuck, BaseLuck + 1);
                BaseLuck += 1;
            }
            #endregion
            OutputBaseStats();
            // Reset stats on level up
            ResetCurrentStats();
        }

        // Get next random int between 1 and 100 inclusive
        protected int GetRNG()
        {
            return RNG.Next(1, 101);
        }

        // Output base stats
        public void OutputBaseStats()
        {
            Console.WriteLine("HP: {0}\nStrength: {1}\nDefense {2}\nMagic: : {3}\nResistance: {4}\nSpeed: {5}\nSkill: {6}\nLuck: {7}\n",
                BaseHP, BaseStrength, BaseDefense, BaseMagic, BaseResistance, BaseSpeed, BaseSkill, BaseLuck);
        }

        // Add new active skill to list
        protected void AddActiveSkill(ActiveSkills skill)
        {
            // Check that we have space
            // Also TODO if we already have too many skills overwrite one the same way but add the overwrited one to ActiveSkillsAvailable
            if (ActiveSkillsLearned.Count <= MaxActiveSkills)
            {
                ActiveSkillsLearned.Add(new ActiveSkill(skill));
                // Update available skill list
                ActiveSkillsAvailable.Remove(Enum.GetName(typeof(ActiveSkills), skill));
            }
        }

        // Add new passive skill to list
        protected void AddPassiveSkill(PassiveSkills skill)
        {
            // Check that we have space
            // Also TODO if we already have too many skills overwrite one the same way but add the overwrited one to PassiveSkillsAvailable
            if (PassiveSkillsLearned.Count <= MaxPassiveSkills) 
            {
                PassiveSkillsLearned.Add(new PassiveSkill(skill));
                // Update available skill list
                PassiveSkillsAvailable.Remove(Enum.GetName(typeof(PassiveSkills), skill));
            }
        }

        // Get new active skill
        protected void LearnRandomActiveSkill()
        {
            // Find a random available skill
            string randomSkill = ActiveSkillsAvailable.ElementAt(RNG.Next(0, ActiveSkillsAvailable.Count));
            // Save it as enum
            Enum.TryParse(randomSkill, out ActiveSkills active);
            AddActiveSkill(active);
        }

        // Get new passive skill
        protected void LearnRandomPassiveSkill()
        {
            // Find a random available skill
            string randomSkill = PassiveSkillsAvailable.ElementAt(RNG.Next(0, PassiveSkillsAvailable.Count));
            // Save it as enum
            Enum.TryParse(randomSkill, out PassiveSkills passive);
            AddPassiveSkill(passive);
        }
        // TODO add "recieve damage" (check passives for on-damage-taken as a step) and "deal damage" (check passives for damage-dealt as a step)
        // and "try attack" vs "recieve attack" for when attacking or attacked
    }
}