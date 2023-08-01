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
            RNG = new Random();
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
        protected virtual void InitializeGrowths(int defaultGrowth = 10)
        {
            // Fill with default growths (messing around with random scaling for testing)
            GrowthHP = defaultGrowth * RNG.Next(1,10);
            GrowthStrength = defaultGrowth * RNG.Next(1, 10);
            GrowthDefense = defaultGrowth * RNG.Next(1, 10);
            GrowthMagic = defaultGrowth * RNG.Next(1, 10);
            GrowthResistance = defaultGrowth * RNG.Next(1, 10);
            GrowthSpeed = defaultGrowth * RNG.Next(1, 10);
            GrowthSkill = defaultGrowth * RNG.Next(1, 10);
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
            // If our level is divisible by 4 then gain an active and passive skill slot
            if (Level % 4 == 0)
            {
                MaxActiveSkills += 1;
                MaxPassiveSkills += 1;
            }
            // If our level is divisible by 2 then learn a random active skill
            if (Level % 2 == 0)
            {
                LearnRandomActiveSkill();
            }
            // If our level is divisible by 3 then learn a random passive skill
            if (Level % 3 == 0)
            {
                LearnRandomPassiveSkill();
            }

            // Add 1 to base stat if we rolled equal to or below the growth stat
            // If we did not gain a stat, roll luck for bad luck protection
            #region Stat growth RNG
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
            // Calculate base hit chance (50 + speed + 1.5x Skill + 0.5x Luck)
            int hitChance = 50 + Speed + MultiplyStat(Skill, 1.5f, true) + MultiplyStat(Luck, 0.5f, true);
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
            int baseDamage = attack.BaseDamage + (usePhysical ? Strength : Magic);
            // With our hit chance and damage we can tell the target to figure it out
            target.IncomingAttack(Name, hitChance, baseDamage, usePhysical);
        }

        // Incoming attack
        public void IncomingAttack(string enemyName, int enemyHitPercent, int enemyDamage, bool physicalAttack)
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
            // Dodge chance is 1.5x Speed + Skill + 0.5x Luck;
            int dodgeChance = MultiplyStat(Speed, 1.5f, true) + Skill + MultiplyStat(Luck, 0.5f, true);
            // Roll to dodge (hit chance - dodge chance)
            // temp hit roll output
            int rng = GetRNG();
            // Check if attack connects
            if (rng <= (enemyHitPercent - dodgeChance))
            {
                int netDamage;
                int blockingStat;
                // Check if physical
                if (physicalAttack)
                {
                    blockingStat = Defense;
                }
                else { blockingStat = Resistance; }
                // Calculate net damage
                netDamage = enemyDamage - blockingStat;
                // Test to make sure positive damage
                netDamage = netDamage > 0 ? netDamage : 0;
                Console.WriteLine("{0} ({1}% hit) attacks {2} ({3}% dodge) and hits for {4} - {5} = {6} damage!", enemyName, enemyHitPercent, Name, dodgeChance, enemyDamage, blockingStat, netDamage);
                if (HP - netDamage < 0)
                {
                    Console.WriteLine("{0} is slain! Overkill: {1}", Name, HP - netDamage);
                }
                else
                {
                    Console.WriteLine("{0} HP: {1} -> {2}", Name, HP, HP - netDamage);
                }
                
                HP -= netDamage;
            }
            else
            {
                Console.WriteLine("{0} ({1}% hit) attacks {2} ({3}% dodge) and misses!", enemyName, enemyHitPercent, Name, dodgeChance);
            }
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
                        break;
                    case PassiveSkills.BoostStrength:
                        Strength = MultiplyStat(Strength, 1.2f, true);
                        break;
                    case PassiveSkills.ProtectAll:
                        // TODO set up buffs / debuffs and add immunity as a buff
                        break;
                    case PassiveSkills.Sacrifice:
                        HP = MultiplyStat(HP, 0.8f, false);
                        Strength = MultiplyStat(Strength, 1.1f, true);
                        Magic = MultiplyStat(Magic, 1.1f, true);
                        Defense = MultiplyStat(Defense, 1.1f, true);
                        Resistance = MultiplyStat(Resistance, 1.1f, true);
                        Speed = MultiplyStat(Speed, 1.1f, true);
                        Skill = MultiplyStat(Skill, 1.1f, true);
                        Luck = MultiplyStat(Luck, 1.1f, true);
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