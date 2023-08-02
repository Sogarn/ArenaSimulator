﻿using System;
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
        // All units share the same RNG object
        protected static readonly Random RNG = new Random();
        public string Name { get; protected set; }
        // Level will start at 0
        protected int Level { get; set; }
        protected int XPPerLevel { get; set; }
        protected int XPToNextLevel { get; set; }
        // Max active skill count starts at 2 and increases with level
        protected int MaxActiveSkills = 2;
        // List of active skills
        public List<ActiveSkill> ActiveSkillsLearned = new List<ActiveSkill>();
        // List of unlearned active skills
        protected List<string> ActiveSkillsAvailable = Enum.GetNames(typeof(ActiveSkills)).ToList();
        // Max passive skill count starts at 1 and increases with level
        protected int MaxPassiveSkills = 1;
        // List of passive skills
        public List<PassiveSkill> PassiveSkillsLearned = new List<PassiveSkill>();
        // List of unlearned passive skills
        protected List<string> PassiveSkillsAvailable = Enum.GetNames(typeof(PassiveSkills)).ToList();
        // TODO add buffs and debuffs
        // Our 8 stats are HP, Strength, Defense, Magic, Resistance, Speed, Skill, and Luck
        // Base stats start at 10 and increase on level up
        protected int BaseHP { get; set; }
        protected int BaseStrength { get; set; }
        protected int BaseDefense { get; set; }
        protected int BaseMagic { get; set; }
        protected int BaseResistance { get; set; }
        protected int BaseSpeed { get; set; }
        protected int BaseSkill { get; set; }
        protected int BaseLuck { get; set; }
        // Current stats fluctuate and reset on level up
        public int HP { get; protected set; }
        public int Strength { get; protected set; }
        public int Defense { get; protected set; }
        public int Magic { get; protected set; }
        public int Resistance { get; protected set; }
        public int Speed { get; protected set; }
        public int Skill { get; protected set; }
        public int Luck { get; protected set; }
        // Growth stats range from 1 to 100 and are the percentage chance of gaining a stat on level up
        protected int GrowthHP { get; set; }
        protected int GrowthStrength { get; set; }
        protected int GrowthDefense { get; set; }
        protected int GrowthMagic { get; set; }
        protected int GrowthResistance { get; set; }
        protected int GrowthSpeed { get; set; }
        protected int GrowthSkill { get; set; }
        protected int GrowthLuck { get; set; }

        public Unit(string name)
        {
            Name = name;
            Level = 0;
            XPPerLevel = 250;
            XPToNextLevel = XPPerLevel;
            // Set all base stats to default values (10)
            InitializeBaseStats();
            // Set all current stats to base stats
            ResetCurrentStats();
            // Set all growths to default values (20)
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
            // Fill with default stats, HP starts doubled
            BaseHP = defaultStat * 2;
            BaseStrength = defaultStat;
            BaseDefense = defaultStat;
            BaseMagic = defaultStat;
            BaseResistance = defaultStat;
            BaseSpeed = defaultStat;
            BaseSkill = defaultStat;
            BaseLuck = defaultStat;
        }

        // Default growths
        protected virtual void InitializeGrowths(int defaultGrowth = 10)
        {
            // Fill with default growths (messing around with random scaling for testing)
            // Initial rolls are between 1x and 9x
            // Bad luck protection (every time RNG.Next rolls below 3 its minimum goes up by 1 until it reaches 3)
            int currentMin = 1;
            // Good luck punishment (every time RNG.Next rolls 7+ its maximum goes down by 1 until it reaches 7)
            int currentMax = 10;
            // Final multipliers
            int[] statMultiplier = new int [8];
            // Multpliers that we will pull from
            List<int> jumbledMultiplier = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                // Give roll to list
                jumbledMultiplier.Add(RNG.Next(currentMin, currentMax));
                // We will automatically stop adding to the minimum when it is 3 and subtracting from max when it is 8 (7 inclusive)
                if (jumbledMultiplier[i] < 3)
                {
                    currentMin += 1;
                }
                else if (jumbledMultiplier[i] > 8)
                {
                    currentMax -= 1;
                }
            }
            // Pull randomly from multipliers to fill stat multiplier list
            int nextIndex;
            for (int i = 0; i < statMultiplier.Length; i++)
            {
                // Get random eligible index
                nextIndex = RNG.Next(0, jumbledMultiplier.Count);
                // Add index to stat multiplier
                statMultiplier[i] = jumbledMultiplier[nextIndex];
                // Remove eligible index
                jumbledMultiplier.Remove(nextIndex);
            }
            // Apply multipliers
            GrowthHP = defaultGrowth * statMultiplier[0];
            GrowthStrength = defaultGrowth * statMultiplier[1];
            GrowthDefense = defaultGrowth * statMultiplier[2];
            GrowthMagic = defaultGrowth * statMultiplier[3];
            GrowthResistance = defaultGrowth * statMultiplier[4];
            GrowthSpeed = defaultGrowth * statMultiplier[5];
            GrowthSkill = defaultGrowth * statMultiplier[0];
            GrowthLuck = defaultGrowth * RNG.Next(1, 10);
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
            foreach (ActiveSkill skill in ActiveSkillsLearned)
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
            Console.WriteLine("{0} leveled up! Level {1} -> {2}", Name, Level, Level + 1);
            Level += 1;
            // If our level is divisible by 5 then gain an active and passive skill slot
            if (Level % 5 == 0)
            {
                MaxActiveSkills += 1;
                MaxPassiveSkills += 1;
            }
            // If our level is divisible by 3 then learn a random active skill
            if (Level % 3 == 0)
            {
                LearnRandomActiveSkill();
            }
            // If our level is divisible by 4 then learn a random passive skill
            if (Level % 4 == 0)
            {
                LearnRandomPassiveSkill();
            }

            // Add 1 to base stat if we rolled equal to or below the growth stat
            // If we did not gain a stat, roll luck for bad luck protection
            #region Stat growth RNG
            // HP growths are 2x
            if (GetRNG() <= GrowthHP)
            {
                Console.WriteLine("Gained HP ({0}%)! {1} -> {2}", GrowthHP, BaseHP, BaseHP + 2);
                BaseHP += 2;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! HP {0} -> {1}", BaseHP, BaseHP + 2);
                BaseHP += 2;
            }
            // Other growths are 1x
            if (GetRNG() <= GrowthStrength)
            {
                Console.WriteLine("Gained Strength ({0}%)! {1} -> {2}", GrowthStrength, BaseStrength, BaseStrength + 1);
                BaseStrength += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Strength {0} -> {1}", BaseStrength, BaseStrength + 1);
                BaseStrength += 1;
            }
            if (GetRNG() <= GrowthDefense)
            {
                Console.WriteLine("Gained Defense ({0}%)! {1} -> {2}", GrowthDefense, BaseDefense, BaseDefense + 1);
                BaseDefense += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Defense {0} -> {1}", BaseDefense, BaseDefense + 1);
                BaseDefense += 1;
            }
            if (GetRNG() <= GrowthMagic)
            {
                Console.WriteLine("Gained Magic ({0}%)! {1} -> {2}", GrowthMagic, BaseMagic, BaseMagic + 1);
                BaseMagic += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Magic {0} -> {1}", BaseMagic, BaseMagic + 1);
                BaseMagic += 1;
            }
            if (GetRNG() <= GrowthResistance)
            {
                Console.WriteLine("Gained Resistance ({0}%)! {1} -> {2}", GrowthResistance, BaseResistance, BaseResistance + 1);
                BaseResistance += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Resistance {0} -> {1}", BaseResistance, BaseResistance + 1);
                BaseResistance += 1;
            }
            if (GetRNG() <= GrowthSpeed)
            {
                Console.WriteLine("Gained Speed ({0}%)! {1} -> {2}", GrowthSpeed, BaseSpeed, BaseSpeed + 1);
                BaseSpeed += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Speed {0} -> {1}", BaseSpeed, BaseSpeed + 1);
                BaseSpeed += 1;
            }
            if (GetRNG() <= GrowthSkill)
            {
                Console.WriteLine("Gained Skill ({0}%)! {1} -> {2}", GrowthSkill, BaseSkill, BaseSkill + 1);
                BaseSkill += 1;
            }
            else if (GetRNG() <= Luck)
            {
                Console.WriteLine("Lucky! Skill {0} -> {1}", BaseSkill, BaseSkill + 1);
                BaseSkill += 1;
            }
            if (GetRNG() <= GrowthLuck)
            {
                Console.WriteLine("Gained Luck ({0}%)! {1} -> {2}", GrowthLuck, BaseLuck, BaseLuck + 1);
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
            if (ActiveSkillsLearned.Count < MaxActiveSkills)
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
            if (PassiveSkillsLearned.Count < MaxPassiveSkills)
            {
                PassiveSkillsLearned.Add(new PassiveSkill(skill));
                // Update available skill list
                PassiveSkillsAvailable.Remove(Enum.GetName(typeof(PassiveSkills), skill));
            }
        }

        // Get new active skill
        protected void LearnRandomActiveSkill()
        {
            // Find a random available skill if there are any left
            if (ActiveSkillsAvailable.Count > 0)
            {
                string randomSkill = ActiveSkillsAvailable.ElementAt(RNG.Next(0, ActiveSkillsAvailable.Count));
                // Save it as enum
                Enum.TryParse(randomSkill, out ActiveSkills active);
                AddActiveSkill(active);
            }
        }

        // Get new passive skill
        protected void LearnRandomPassiveSkill()
        {
            // Find a random available skill if there are any left
            if (PassiveSkillsAvailable.Count > 0)
            {
                // Find a random available skill
                string randomSkill = PassiveSkillsAvailable.ElementAt(RNG.Next(0, PassiveSkillsAvailable.Count));
                // Save it as enum
                Enum.TryParse(randomSkill, out PassiveSkills passive);
                AddPassiveSkill(passive);
            }
        }

        // Outgoing attack
        public void OutgoingAttack(ActiveSkill attack, Unit target)
        {
            // Check all passive rolls
            foreach (PassiveSkill passive in PassiveSkillsLearned)
            {
                // Make sure it is offensive
                if (passive.Offensive && passive.SkillReady())
                {
                    TriggerPassive(passive);
                }
            }
            // Final hit chance is the attack's accuracy + hit chance
            int hitChance = attack.BaseAccuracy + CalculateBaseHitChance();
            // Get base damage of attack and either strength or magic (or whichever one our opponent is worse at if we can choose)
            bool usePhysical;
            if (attack.Physical && attack.Magical)
            {
                // Invert high defense
                usePhysical = !HighDefense();
            }
            else
            {
                usePhysical = attack.Physical;
            }
            // Attack damage is a multiplier
            int baseDamage = MultiplyStat((usePhysical ? Strength : Magic), attack.BaseDamage, true);
            // With our hit chance and damage we can tell the target to figure it out
            target.IncomingAttack(Name, hitChance, CalculateCritChance(), baseDamage, usePhysical);
        }

        // Incoming attack
        public void IncomingAttack(string enemyName, int enemyHitChance, int enemyCritChance, int enemyDamage, bool physicalAttack)
        {
            // Check all passive rolls
            foreach (PassiveSkill passive in PassiveSkillsLearned)
            {
                // Make sure it is defensive and available
                if (passive.Defensive && passive.SkillReady())
                {
                    TriggerPassive(passive);
                }
            }
            int dodgeChance = CalculateDodgeChance();
            // crit avoid is opposite of crit
            int critAvoidChance = CalculateCritChance();
            // Roll to dodge (hit chance - dodge chance)
            // temp hit roll output
            int hitRNG = GetRNG();
            // Store attack output line
            string attackOutcome;
            // Store damage
            int netDamage = 0;
            // Store chance to get hit
            int enemyNetHitChance = enemyHitChance - dodgeChance;
            // Check if attack connects
            if (hitRNG <= enemyNetHitChance)
            {
                int blockingStat;
                int critRNG = GetRNG();
                int bonusCrit;
                int finalCritChance;
                // Crit chance is their base crit - our base crit + if their hit chance was over 100% add the difference
                bonusCrit = (enemyNetHitChance > 100 ? enemyNetHitChance - 100 : 0);
                finalCritChance = enemyCritChance - critAvoidChance + bonusCrit;
                // Roll crit, if true then ignore defenses)
                if (critRNG <= (finalCritChance))
                {
                    blockingStat = 0;
                    // Crits ignore defenses they do not do additional damage on top
                    netDamage = enemyDamage;
                    attackOutcome = string.Format("{0} ({1}% hit)({2}% crit) attacks {3} and crits for {4} damage!", enemyName, enemyNetHitChance, finalCritChance, Name, netDamage);
                }
                else // regular hit
                {
                    // Blocking stat is Defense if attack is physical otherwise resistance
                    blockingStat = (physicalAttack) ? Defense : Resistance;
                    // Calculate net damage
                    netDamage = enemyDamage - blockingStat;
                    // Test to make sure positive damage
                    netDamage = netDamage > 0 ? netDamage : 0;
                    attackOutcome = string.Format("{0} ({1}% hit) attacks {2} and hits for {3} - {4} = {5} damage!", enemyName, enemyNetHitChance, Name, enemyDamage, blockingStat, netDamage);
                }
            }
            else
            {
                attackOutcome = string.Format("{0} ({1}% hit) attacks {2} and misses!", enemyName, enemyNetHitChance, Name, dodgeChance);
            }
            Console.WriteLine(attackOutcome);
            // Check if damage was dealt
            if (netDamage > 0)
            {
                // Get new HP value
                int newHP = HP - netDamage;
                if (newHP <= 0)
                {
                    Console.WriteLine("{0} is slain! Overkill: {1}", Name, newHP);
                }
                else
                {
                    Console.WriteLine("{0} HP: {1}/{2} -> {3}/{2}", Name, HP, BaseHP, newHP);
                }
                HP = newHP;
            }
        }

        // Calculate base hit chance
        protected int CalculateBaseHitChance()
        {
            // Double of (Speed + 1.5x Skill + 0.5x Luck)
            return 2 * (Speed + MultiplyStat(Skill, 1.5f, true) + MultiplyStat(Luck, 0.5f, true));
        }

        // Calculate base dodge chance
        protected int CalculateDodgeChance()
        {
            // 1.5x Speed + Skill + 0.5x Luck
            return MultiplyStat(Speed, 1.5f, true) + Skill + MultiplyStat(Luck, 0.5f, true);
        }

        // Calculate base crit chance
        protected int CalculateCritChance()
        {
            // Skill + 0.25x Luck
            return Skill + MultiplyStat(Luck, 0.25f, true);
        }

        // Trigger passive effects
        protected void TriggerPassive(PassiveSkill passiveSkill)
        {
            // Set proc chance (Skill stat * Coefficient * (1 + Luck stat / 100) (rounded up)
            int procChance = (int)Math.Ceiling(Skill * passiveSkill.SkillCoefficient * GetLuckCoefficient());
            // General format is "Roll for whether it procs -> do effects"
            // Do not even proceed unless the skill procs
            if (GetRNG() <= procChance)
            {
                // Put the skill on cooldown
                passiveSkill.UseSkill(Name);
                // Look up based on internal name
                switch (passiveSkill.InternalName)
                {
                    // Effects are mostly placeholders so don't mind these
                    case PassiveSkills.BoostMagic:
                        Magic = MultiplyStat(Magic, 1.2f, true);
                        Resistance = MultiplyStat(Resistance, 1.2f, true);
                        break;
                    case PassiveSkills.BoostStrength:
                        Strength = MultiplyStat(Strength, 1.2f, true);
                        Defense = MultiplyStat(Defense, 1.2f, true);
                        break;
                    case PassiveSkills.ProtectAll:
                        // TODO set up buffs / debuffs and add immunity as a buff
                        break;
                    case PassiveSkills.Sacrifice:
                        HP = MultiplyStat(HP, 0.8f, false);
                        if (HP <= 0) { HP = 1; }
                        Strength = MultiplyStat(Strength, 1.1f, true);
                        Magic = MultiplyStat(Magic, 1.1f, true);
                        Defense = MultiplyStat(Defense, 1.1f, true);
                        Resistance = MultiplyStat(Resistance, 1.1f, true);
                        Speed = MultiplyStat(Speed, 1.1f, true);
                        Skill = MultiplyStat(Skill, 1.1f, true);
                        Luck = MultiplyStat(Luck, 1.1f, true);
                        break;
                    case PassiveSkills.BoostSkill:
                        Skill = MultiplyStat(Skill, 1.2f, true);
                        Speed = MultiplyStat(Speed, 1.2f, true);
                        break;
                    case PassiveSkills.Cleanse:
                        // TODO buffs/debuffs
                        break;
                    case PassiveSkills.ReactiveLuck:
                        HP += MultiplyStat(BaseHP, 0.2f, true);
                        Luck = MultiplyStat(Luck, 1.2f, true);
                        break;
                }
            }
        }

        // Saves constantly doing casts and floats
        protected int MultiplyStat(int stat, float multiplier, bool roundUp)
        {
            if (roundUp)
            {
                return (int)Math.Ceiling(stat * multiplier);
            }
            else
            {
                return (int)Math.Floor(stat * multiplier);
            }
        }

        // Check if alive
        public bool IsAlive()
        {
            return (HP > 0);
        }

        // Return if our Defense is higher than our Resistance for targetting logic checks
        public bool HighDefense()
        {
            return (Defense > Resistance);
        }

        // Get luck coefficient (1f + Luck / 100f)
        protected float GetLuckCoefficient()
        {
            return (1f + Luck / 100f);
        }

        // TODO add "recieve damage" (check passives for on-damage-taken as a step) and "deal damage" (check passives for damage-dealt as a step)
        // and "try attack" vs "recieve attack" for when attacking or attacked
    }
}