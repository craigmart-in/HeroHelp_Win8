using BattleNet.D3;
using BattleNet.D3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroHelper.Domain
{
    public class CalculateStats
    {
        public static CalculatedStats CalculateStatsFromHero(Hero hero)
        {
            CalculatedStats calcStats = new CalculatedStats();
            Dictionary<string, Set> charSets = new Dictionary<string, Set>();

            double totalArmor = 0;
            double armFromItems = 0;

            double allResFromItems = 0;
            Dictionary<string, double> resFromItems = new Dictionary<string, double>();
            resFromItems.Add("Fire", 0);
            resFromItems.Add("Lightning", 0);
            resFromItems.Add("Poison", 0);
            resFromItems.Add("Physical", 0);
            resFromItems.Add("Arcane", 0);
            resFromItems.Add("Cold", 0);
            double totalAllRes = 0;

            double baseDR = 0;
            double armDR = 0;
            double resDR = 0;

            double totalStr = 0;
            double strFromChar = 0;
            double strFromItems = 0;
            double baseStr = 8;
            double strPerLvl = 1;

            double totalDex = 0;
            double dexFromChar = 0;
            double dexFromItems = 0;
            double baseDex = 8;
            double dexPerLvl = 1;

            double totalInt = 0;
            double intFromChar = 0;
            double intFromItems = 0;
            double baseInt = 8;
            double intPerLvl = 1;

            double totalVit = 0;
            double vitFromChar = 0;
            double vitFromItems = 0;
            double baseVit = 9;
            double vitPerLvl = 2;

            double lifePctFromItems = 0;
            int healthVitMult = hero.Level < 35 ? 10 : hero.Level - 25;

            double critDamage = 0.5;
            double critChance = 0.05;
            double ias = 0;
            double aps = 1;

            double eleDmg = 0;
            double loh = 0;
            double minDmg = 0;
            double maxDmg = 0;
            double eliteBonus = 0;
            double demonBonus = 0;
            double ls = 0;
            double lifeRegen = 0;

            double blockChance = 0;
            double blockMin = 0;
            double blockMax = 0;

            // Class bonuses.
            switch (hero.Class)
            {
                case D3Client.Barbarian:
                    baseStr = 10;
                    strPerLvl = 3;
                    baseDR = .3;
                    break;
                case D3Client.Monk:
                    baseDex = 10;
                    dexPerLvl = 3;
                    baseDR = .3;
                    break;
                case D3Client.DemonHunter:
                    baseDex = 10;
                    dexPerLvl = 3;
                    break;
                default: // Wizard or Witch-Doctor
                    baseInt = 10;
                    intPerLvl = 3;
                    break;
            }

            foreach (KeyValuePair<string, Item> item in hero.Items)
            {
                bool isWeapon = (item.Key == "offHand" && hero.Items[item.Key].AttacksPerSecond != null) || item.Key == "mainHand";
                // Get stats from item
                CalculateStatsFromRawAttributes(hero.Items[item.Key].AttributesRaw, isWeapon, ref allResFromItems, ref strFromItems,
                            ref dexFromItems, ref intFromItems, ref vitFromItems, ref lifePctFromItems, ref armFromItems,
                            ref critDamage, ref critChance, ref ias, ref aps, ref resFromItems,
                            ref eleDmg, ref loh, ref minDmg, ref maxDmg,
                            ref eliteBonus, ref demonBonus, ref ls, ref lifeRegen,
                            ref blockChance, ref blockMin, ref blockMax);

                // Get stats from gems
                foreach (SocketedGem gem in hero.Items[item.Key].Gems)
                {
                    CalculateStatsFromRawAttributes(gem.AttributesRaw, false, ref allResFromItems, ref strFromItems,
                            ref dexFromItems, ref intFromItems, ref vitFromItems, ref lifePctFromItems, ref armFromItems,
                            ref critDamage, ref critChance, ref ias, ref aps, ref resFromItems,
                            ref eleDmg, ref loh, ref minDmg, ref maxDmg,
                            ref eliteBonus, ref demonBonus, ref ls, ref lifeRegen,
                            ref blockChance, ref blockMin, ref blockMax);
                }

                // Monitor sets
                if (hero.Items[item.Key].Set != null)
                {
                    Set tempSet = new Set();
                    // If set is already monitored, increment the count.
                    if (charSets.ContainsKey(hero.Items[item.Key].Set.Slug))
                    {
                        tempSet = charSets[hero.Items[item.Key].Set.Slug];
                        tempSet.CharCount++;
                    }
                    else // Else create a new monitor
                    {
                        tempSet = hero.Items[item.Key].Set;
                        tempSet.CharCount = 1;
                    }

                    charSets[hero.Items[item.Key].Set.Slug] = tempSet;
                }
            }

            // Incorporate Sets
            foreach (KeyValuePair<string, Set> set in charSets)
            {
                foreach (Rank rank in set.Value.Ranks)
                {
                    if (set.Value.CharCount >= rank.Required)
                    {
                        Dictionary<string, MinMax> attributesRaw = D3Client.ParseAttributesRawFromAttributes(rank.Attributes);

                        // Get stats from Set Bonuses
                        CalculateStatsFromRawAttributes(attributesRaw, false, ref allResFromItems, ref strFromItems,
                            ref dexFromItems, ref intFromItems, ref vitFromItems, ref lifePctFromItems, ref armFromItems,
                            ref critDamage, ref critChance, ref ias, ref aps, ref resFromItems,
                            ref eleDmg, ref loh, ref minDmg, ref maxDmg,
                            ref eliteBonus, ref demonBonus, ref ls, ref lifeRegen,
                            ref blockChance, ref blockMin, ref blockMax);
                    }
                }
            }

            // Calculate Strength
            strFromChar = baseStr + (strPerLvl * (hero.Level - 1)) + (strPerLvl * hero.ParagonLevel);
            totalStr = strFromChar + strFromItems;

            // Calculate Dexterity
            dexFromChar = baseDex + (dexPerLvl * (hero.Level - 1)) + (dexPerLvl * hero.ParagonLevel);
            totalDex = dexFromChar + dexFromItems;

            // Calculate Intelligence
            intFromChar = baseInt + (intPerLvl * (hero.Level - 1)) + (intPerLvl * hero.ParagonLevel);
            totalInt = intFromChar + intFromItems;

            // Calculate Vitality
            vitFromChar = baseVit + (vitPerLvl * (hero.Level - 1)) + (vitPerLvl * hero.ParagonLevel);
            totalVit = vitFromChar + vitFromItems;

            // Calculate Armor
            totalArmor = armFromItems + totalStr;

            // Calculate All Res
            totalAllRes = allResFromItems + (totalInt / 10);

            // Class Mainstat.
            double mainStat = GetMainStatFromClass(hero.Class, totalStr, totalDex, totalInt);

            // Class passive skills
            foreach (SkillRune passive in hero.Skills.Passive)
            {
                switch (passive.Skill.Slug)
                {
                    case "ruthless":
                        critChance += 0.05;
                        critDamage += 0.5;
                        break;
                    case "bloodthirst":
                        ls += 0.03;
                        break;
                    case "nerves-of-steel":
                        totalArmor += totalVit;
                        break;
                    case "tough-as-nails":
                        totalArmor += totalArmor * 0.25;
                        break;
                    case "archery":
                    case "weapons-master":
                        switch (hero.Items["mainHand"].Type.Id)
                        {
                            case "Mace":
                            case "Axe":
                            case "Mace2H":
                            case "Axe2H":
                            case "HandXBow":
                                critChance += 0.1;
                                break;
                            case "Crossbow":
                                critDamage += .5;
                                break;
                            case "Polearm":
                            case "Spear":
                                ias += .1;
                                break;
                        }
                        break;
                    case "perfectionist":
                        lifePctFromItems += 0.1;
                        totalArmor += totalArmor * 0.1;
                        totalAllRes += totalAllRes * 0.1;
                        break;
                    case "one-with-everything":
                        double highRes = 0;
                        foreach (KeyValuePair<string, double> res in resFromItems)
                            if (res.Value > highRes)
                                highRes = res.Value;
                        totalAllRes += highRes;
                        break;
                    case "seize-the-initiative":
                        totalArmor += totalDex * 0.5;
                        break;
                }
            }

            armDR = totalArmor / ((50 * 63) + totalArmor);
            resDR = totalAllRes / ((5 * 63) + totalAllRes);

            double multDR = ((1 - armDR) * (1 - resDR) * (1 - baseDR));

            double dr = 1 - multDR;
            double hp = (36 + (4 * hero.Level) + (healthVitMult * totalVit)) * (1 + lifePctFromItems);
            double ehp = hp / multDR;

            calcStats.EHP = ehp;
            calcStats.DPS = CalculateDPS(mainStat, critChance, critDamage,
                hero.Items["mainHand"], hero.Items["offHand"], ias, minDmg, maxDmg, eleDmg);

            // Base stats
            calcStats.BaseStats.Add(new CalculatedStat("Strength", new double[] { totalStr }, "{0:N0}"));
            calcStats.BaseStats.Add(new CalculatedStat("Dexterity", new double[] { totalDex }, "{0:N0}"));
            calcStats.BaseStats.Add(new CalculatedStat("Intelligence", new double[] { totalInt }, "{0:N0}"));
            calcStats.BaseStats.Add(new CalculatedStat("Vitality", new double[] { totalVit }, "{0:N0}"));
            calcStats.BaseStats.Add(new CalculatedStat("Armor", new double[] { totalArmor }, "{0:N0}"));

            // Offense
            aps = CalculateR(hero.Items["mainHand"], hero.Items["offHand"], ias);
            calcStats.DamageStats.Add(new CalculatedStat("Attacks per Second", new double[] { aps }, "{0:N}"));
            calcStats.DamageStats.Add(new CalculatedStat("+% Attack Speed", new double[] { ias }, "{0:P}"));
            calcStats.DamageStats.Add(new CalculatedStat("Critical Hit Chance", new double[] { critChance }, "{0:P}"));
            calcStats.DamageStats.Add(new CalculatedStat("Critical Hit Damage", new double[] { critDamage }, "{0:P}"));

            // Defense
            calcStats.DefenseStats.Add(new CalculatedStat("Block Amount", new double[] { blockMin, blockMax }, "{0:N0} - {1:N0}"));
            calcStats.DefenseStats.Add(new CalculatedStat("Block Chance", new double[] { blockChance }, "{0:P}"));
            double dodgeChance = CalculateDodgeChance(totalDex);
            calcStats.DefenseStats.Add(new CalculatedStat("Dodge Chance", new double[] { dodgeChance }, "{0:P}"));
            calcStats.DefenseStats.Add(new CalculatedStat("Armor Damage Reduction", new double[] { armDR }, "{0:P}"));
            calcStats.DefenseStats.Add(new CalculatedStat("Resist Damage Reduction", new double[] { resDR }, "{0:P}"));
            calcStats.DefenseStats.Add(new CalculatedStat("Damage Reduction", new double[] { dr }, "{0:P}"));
            calcStats.DefenseStats.Add(new CalculatedStat("All Resist", new double[] { totalAllRes }, "{0:N0}"));

            // Life
            calcStats.LifeStats.Add(new CalculatedStat("Maximum Life", new double[] { hp }, "{0:N0}"));
            calcStats.LifeStats.Add(new CalculatedStat("Total Life Bonus", new double[] { lifePctFromItems }, "{0:P0}"));
            calcStats.LifeStats.Add(new CalculatedStat("Life per Second", new double[] { lifeRegen }, "{0:N}"));
            calcStats.LifeStats.Add(new CalculatedStat("Life Steal", new double[] { ls }, "{0:P}"));
            //calcStats.LifeStats.Add(new CalculatedStat("Life per Kill", lifePctFromItems, "{0:N}"));
            calcStats.LifeStats.Add(new CalculatedStat("Life per Hit", new double[] { loh }, "{0:N}"));
            //calcStats.LifeStats.Add(new CalculatedStat("Health Globe Healing Bonus", lifePctFromItems, "{0:N}"));
            //calcStats.LifeStats.Add(new CalculatedStat("Bonus to Gold/Globe radius", lifePctFromItems, "{0:N}"));

            return calcStats;
        }

        private static void CalculateStatsFromRawAttributes(Dictionary<string, MinMax> attributesRaw, bool isAWeapon,
            ref double allResFromItems, ref double strFromItems, ref double dexFromItems, ref double intFromItems,
            ref double vitFromItems, ref double lifePctFromItems, ref double armFromItems, ref double critDamage,
            ref double critChance, ref double ias, ref double aps, ref Dictionary<string, double> resFromItems,
            ref double eleDmg, ref double loh, ref double minDmg, ref double maxDmg,
            ref double eliteBonus, ref double demonBonus, ref double ls, ref double lifeRegen,
            ref double blockChance, ref double blockMin, ref double blockMax)
        {
            foreach (KeyValuePair<string, MinMax> attributeRaw in attributesRaw)
            {
                switch (attributeRaw.Key)
                {
                    case "Armor_Item":
                    case "Armor_Bonus_Item":
                        armFromItems += attributeRaw.Value.Min;
                        break;
                    case "Resistance_All":
                        allResFromItems += attributeRaw.Value.Min;
                        break;
                    case "Resistance#Fire":
                    case "Resistance#Lightning":
                    case "Resistance#Poison":
                    case "Resistance#Physical":
                    case "Resistance#Arcane":
                    case "Resistance#Cold":
                        resFromItems[attributeRaw.Key.Split('#')[1]] += attributeRaw.Value.Min;
                        break;
                    case "Strength_Item":
                        strFromItems += attributeRaw.Value.Min;
                        break;
                    case "Dexterity_Item":
                        dexFromItems += attributeRaw.Value.Min;
                        break;
                    case "Intelligence_Item":
                        intFromItems += attributeRaw.Value.Min;
                        break;
                    case "Vitality_Item":
                        vitFromItems += attributeRaw.Value.Min;
                        break;
                    case "Hitpoints_Max_Percent_Bonus_Item":
                        lifePctFromItems += attributeRaw.Value.Min;
                        break;
                    case "Crit_Damage_Percent":
                        critDamage += attributeRaw.Value.Min;
                        break;
                    case "Crit_Percent_Bonus_Capped":
                        critChance += attributeRaw.Value.Min;
                        break;
                    case "Attacks_Per_Second_Percent":
                    case "Attacks_Per_Second_Item_Percent":
                        if (!isAWeapon) // Don't add for weapons.
                            ias += attributeRaw.Value.Min;
                        break;
                    case "Attacks_Per_Second_Item_Bonus":
                        aps += attributeRaw.Value.Min;
                        break;
                    case "Hitpoints_On_Hit":
                        loh += attributeRaw.Value.Min;
                        break;
                    case "Damage_Type_Percent_Bonus#Fire":
                    case "Damage_Type_Percent_Bonus#Lightning":
                    case "Damage_Type_Percent_Bonus#Poison":
                    case "Damage_Type_Percent_Bonus#Arcane":
                    case "Damage_Type_Percent_Bonus#Cold":
                        eleDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Min#Physical":
                        minDmg += attributeRaw.Value.Min;
                        maxDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Delta#Physical":
                        maxDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Percent_Bonus_Vs_Elites":
                        eliteBonus += attributeRaw.Value.Min;
                        break;
                    case "Damage_Percent_Bonus_Vs_Monster_Type#Demon":
                        demonBonus += attributeRaw.Value.Min;
                        break;
                    case "Steal_Health_Percent":
                        ls += attributeRaw.Value.Min;
                        break;
                    case "Hitpoints_Regen_Per_Second":
                        lifeRegen += attributeRaw.Value.Min;
                        break;
                    case "Block_Chance_Item":
                    case "Block_Chance_Bonus_Item":
                        blockChance += attributeRaw.Value.Min;
                        break;
                    //case "Damage_Percent_Reduction_From_Melee":
                    //    meleeDR += attributeRaw.Value.Min;
                    //    break;
                    //case "Damage_Percent_Reduction_From_Elites":
                    //    eliteDR += attributeRaw.Value.Min;
                    //    break;
                    //case "Damage_Percent_Reduction_From_Ranged":
                    //    rangedDR += attributeRaw.Value.Min;
                    //    break;
                    case "Block_Amount_Item_Min":
                        blockMin += attributeRaw.Value.Min;
                        blockMax += attributeRaw.Value.Min;
                        break;
                    case "Block_Amount_Item_Delta":
                        blockMax += attributeRaw.Value.Min;
                        break;
                    //case "Health_Globe_Bonus_Health":
                    //    globes += attributeRaw.Value.Min;
                    //    break;
                }
            }
        }

        private static void CalculateWeaponDamageFromRawAttributes(Dictionary<string, MinMax> attributesRaw,
            out double physMinDmg, out double physMaxDmg, out double totMinDmg, out double totMaxDmg)
        {
            double minBaseDmg = 0;
            double minBonusDmg = 0;
            double minPhysBonusDmg = 0;
            double deltaBaseDmg = 0;
            double deltaBonusDmg = 0;
            double deltaPhysBonusDmg = 0;
            double dmgPercent = 0;

            foreach (KeyValuePair<string, MinMax> attributeRaw in attributesRaw)
            {
                switch (attributeRaw.Key)
                {
                    case "Damage_Weapon_Min#Physical":
                    case "Damage_Min#Physical":
                        minBaseDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Bonus_Min#Physical":
                        minBaseDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Weapon_Bonus_Min#Physical":
                        minPhysBonusDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Weapon_Delta#Physical":
                    case "Damage_Delta#Physical":
                        deltaBaseDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Weapon_Bonus_Delta#Physical":
                        deltaPhysBonusDmg += attributeRaw.Value.Min;
                        break;
                    case "Damage_Weapon_Percent_Bonus#Physical":
                        dmgPercent += attributeRaw.Value.Min;
                        break;
                    default:
                        if (attributeRaw.Key.StartsWith("Damage_Weapon_Min#") || attributeRaw.Key.StartsWith("Damage_Weapon_Bonus_Min#")) {
                            minBonusDmg += attributeRaw.Value.Min;
                        } else if (attributeRaw.Key.StartsWith("Damage_Weapon_Delta#") || attributeRaw.Key.StartsWith("Damage_Weapon_Bonus_Delta")) {
                            deltaBonusDmg += attributeRaw.Value.Min;
                        }
                    break;
                }
            }

            physMinDmg = (minBaseDmg + minPhysBonusDmg) * (1 + dmgPercent);
            physMaxDmg = (Math.Max(minBaseDmg + minPhysBonusDmg, minBaseDmg + deltaBaseDmg - 1) + 1 + deltaPhysBonusDmg) * (1 + dmgPercent);
            totMinDmg = physMinDmg + minBonusDmg;
            totMaxDmg = physMaxDmg + minBonusDmg + deltaBonusDmg;
        }

        private static double CalculateDodgeChance(double dex)
        {
            double dodgeChance = 0;

            if (dex <= 100)
            {
                dodgeChance = dex * 0.1;
            }
            else if (dex <= 500)
            {
                dodgeChance = 10 + ((dex - 100) * 0.025);
            }
            else if (dex <= 1000)
            {
                dodgeChance = 20 + ((dex - 500) * 0.02);
            }
            else if (dex <= 8000)
            {
                dodgeChance = 30 + ((dex - 1000) * 0.01);
            }

            return dodgeChance / 100;
        }

        private static double GetMainStatFromClass(string charClass, double totalStr, double totalDex, double totalInt)
        {
            switch (charClass)
            {
                case D3Client.Barbarian:
                    return totalStr;
                case D3Client.Monk:
                case D3Client.DemonHunter:
                    return totalDex;
                default:
                    return totalInt;
            }
        }

        private static double CalculateDPS(double mainStat, double critChance, double critDamage,
            Item mh, Item oh, double ias, double minDmg, double maxdmg, double eleDmg)
        {
            double s = 1 + (mainStat * 0.01);
            double c = 1 + (critChance * critDamage);
            double r = CalculateR(mh, oh, ias);
            double a = CalculateA(mh, oh, minDmg, maxdmg, eleDmg);
            double m = 1;

            return s * c * r * a * m;
        }

        private static double CalculateR(Item mh, Item oh, double ias)
        {
            double r = 0;

            if (oh.AttacksPerSecond != null)
            {
                double mhaps = mh.AttacksPerSecond.Min * (1.15 + ias);
                double ohaps = oh.AttacksPerSecond.Min * (1.15 + ias);
                r = (mhaps + ohaps) / 2;
            }
            else
            {
                r = mh.AttacksPerSecond.Min * (1 + ias);
            }

            return r;
        }

        private static double CalculateA(Item mh, Item oh, double minDmg, double maxdmg, double eleDmg)
        {
            double a = 0;
            double mhPhysMinDmg;
            double mhPhysMaxDmg;
            double mhTotMinDmg;
            double mhTotMaxDmg;
            CalculateWeaponDamageFromRawAttributes(mh.AttributesRaw, out mhPhysMinDmg, out mhPhysMaxDmg, out mhTotMinDmg, out mhTotMaxDmg);

            double mhBonusDmg = (mhPhysMinDmg + minDmg + mhPhysMaxDmg + maxdmg) / 2 * (eleDmg * 100);
            double mhAvg = ((mhTotMinDmg + minDmg + mhTotMaxDmg + maxdmg) / 2) + mhBonusDmg;

            a = mhAvg;

            if (oh.AttacksPerSecond != null)
            {
                double ohPhysMinDmg;
                double ohPhysMaxDmg;
                double ohTotMinDmg;
                double ohTotMaxDmg;
                CalculateWeaponDamageFromRawAttributes(oh.AttributesRaw, out ohPhysMinDmg, out ohPhysMaxDmg, out ohTotMinDmg, out ohTotMaxDmg);

                double ohBonusDmg = (ohPhysMinDmg + minDmg + ohPhysMaxDmg + maxdmg) / 2 * (eleDmg * 100);
                double ohAvg = ((ohTotMinDmg + minDmg + ohTotMaxDmg + maxdmg) / 2) + ohBonusDmg;

                a = (mhAvg + ohAvg) / 2;
            }

            return a;
        }
    }
}
