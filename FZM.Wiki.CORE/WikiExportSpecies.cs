﻿using Eco.Gameplay.Systems.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Eco.Simulation.Types;
using Eco.Simulation;
using static Eco.Simulation.Types.PlantSpecies;
using Eco.World.Blocks;
using Eco.Shared.Utils;
using System.Linq;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Gameplay.Systems;
using Eco.Gameplay.Blocks;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using System.Globalization;

/*
 * This script is an extension by FZM based on the work done by Pradoxzon.
 * 
 * Most code was re-written to make use of changed or new additions to the Eco source code
 * and to change the reliance on Pradoxzon Core Utilities mod.
 *  
 */

namespace FZM.Wiki
{
    public partial class WikiDetails
    {
        // dictionary of plants and their dictionary of stats
        private static SortedDictionary<string, Dictionary<string, string>> EveryPlant = new SortedDictionary<string, Dictionary<string, string>>();

        // dictionary of trees and their dictionary of stats
        private static SortedDictionary<string, Dictionary<string, string>> EveryTree = new SortedDictionary<string, Dictionary<string, string>>();

        // dictionary of animals and their dictionary of stats
        private static SortedDictionary<string, Dictionary<string, string>> EveryAnimal = new SortedDictionary<string, Dictionary<string, string>>();

        public static void PlantDetails()
        {
            // dictionary of plant properties
            Dictionary<string, string> plantDetails = new Dictionary<string, string>()
            {             
                // INFO
                { "untranslated","nil" },
                { "isDecorative", "nil" }, // Is the plant considered decorative. Not simulated after spawn.
                { "doesSpread", "nil" }, // The plant will spawn others like it nearby given enough time not dying and not harvested

                // LIFETIME
                { "maturity", "nil" }, // Age for full maturity and reproduction.

                // GENERATION
                { "isWater", "nil" }, // Does the species live underwater.
                { "height", "nil" }, // Plant height in meters.

                // FOOD
                { "calorieValue", "nil" }, // The base calories this species provides to it's consumers.

                // RESOURCES
                { "requireHarvestable", "nil" }, // Does this plant require to have reached a harvestable stage before you can harvest it, you will get no resources for this if its not at a harvestable stage. 
                { "pickableAtPercent", "nil" }, // This plant will be pickable at this percent and you will get some resources.
                { "experiencePerHarvest", "nil" }, // Base experience you get per harvest.
                { "harvestTool", "nil" }, // The tool required to harvest this plant, nil means hands.
                { "killOnHarvest", "nil" }, // Does the plant die on harvest.
                { "postHarvestGrowth", "nil" }, // What % growth does the plant return to after harvest.
                { "scytheKills", "nil" }, // Will using a Scythe/Sickle on this plant kill it.
                { "resourceItem", "nil" }, // The item you get from harvesting this plant.
                { "resourceMin", "nil" }, // The minimum number of items returned.
                { "resourceMax", "nil" }, // The maximum number of items returned.
                { "resourceBonus", "nil" }, // The bonus items returned for allowing it to grow.

                // WORLD LAYERS
                { "carbonRelease", "nil" }, // The amount of carbon dioxide released by this species. (Plants & Trees are negative values)
                { "idealGrowthRate", "nil" }, // In ideal conditions, what is the rate of growth. (%)
                { "idealDeathRate", "nil" }, // In ideal conditions what is the rate of death. (%)
                { "spreadRate", "nil" }, // In ideal conditions what is the rate of spread, if it does spread.
                { "nitrogenHalfSpeed", "nil" }, // At what nitrogen value will the growth speed reduce to half.
                { "nitrogenContent", "nil" }, // What nitrogen content is ideal.
                { "phosphorusHalfSpeed", "nil" }, // At what phosphorus value will the growth speed reduce to half.
                { "phosphorusContent", "nil" }, // What phosphorus content is ideal.
                { "potassiumHalfSpeed", "nil" }, // At what potassium value will the growth speed reduce to half.
                { "potassiumContent", "nil" }, // What potassium content is ideal.
                { "soilMoistureHalfSpeed", "nil" }, // At what moisture value will the growth speed reduce to half.
                { "soilMoistureContent", "nil" }, // What moisture content is ideal.
                { "consumedFertileGround", "nil" }, // How much of the area deemed Fertile Ground does this plant take up, this is almost always more than the in game physical space.
                { "consumedCanopySpace", "nil" }, // How much of the area deemed Canopy Space does this plant take up, this is almost always more than the in game physical space.
                { "consumedUnderwaterFertileGround", "nil" }, // How much of the area deemed Underwater Fertile Ground does this plant take up, this is almost always more than the in game physical space.
                { "consumedShrubSpace", "nil" }, // How much of the area deemed Shrub Space does this plant take up, this is almost always more than the in game physical space.
                { "extremeTempMin", "nil" }, // The lowest temperature before this plant stops growth.
                { "idealTempMin", "nil" }, // The lowest temperature of the ideal growth range (max growth).
                { "idealTempMax", "nil" }, // The highest temperature of the ideal growth range (max growth).
                { "extremeTempMax", "nil" }, // The highest temperature before this plant stops growth.
                { "extremeMoistureMin", "nil" }, // The lowest moisture content before this plant stops growth.
                { "idealMoistureMin", "nil" }, // The lowest moisture content of the ideal growth range (max growth).
                { "idealMoistureMax", "nil" }, // The highest moisture content of the ideal growth range (max growth).
                { "extremeMoistureMax", "nil" },// The highest moisture content before this plant stops growth.
                { "extremeSaltMin", "nil" }, // The lowest salt content before this plant stops growth.
                { "idealSaltMin", "nil" }, // The lowest salt contente of the ideal growth range (max growth).
                { "idealSaltMax", "nil" }, // The highest salt content of the ideal growth range (max growth).
                { "extremeSaltMax", "nil" }, // The highest Sslt content before this plant stops growth.
                { "maxPollutionDensity", "nil" }, // The highest pollution density before this plant stops growing.
                { "pollutionTolerance", "nil" } // The pollution density at which this plant slows growth, spread and carbon dioxide absorbtion.
            };

            IEnumerable<Species> species = EcoSim.AllSpecies;

            foreach (Species s in species)
            {
                if (s is PlantSpecies && !(s is TreeSpecies))
                {
                    PlantSpecies plant = s as PlantSpecies;
                    if (!EveryPlant.ContainsKey(plant.DisplayName))
                    {
                        string plantName = plant.DisplayName;
                        EveryPlant.Add(plantName, new Dictionary<string, string>(plantDetails));

                        #region INFO
                        EveryPlant[plantName]["untranslated"] = $"'{plant.DisplayName.NotTranslated}'";
                        EveryPlant[plantName]["isDecorative"] = plant.Decorative ? $"'{Localizer.DoStr("Decorative")}'" : "nil"; 
                        EveryPlant[plantName]["doesSpread"] = plant.NoSpread ? $"'{Localizer.DoStr("No")}'" : $"'{Localizer.DoStr("Yes")}'"; 
                        #endregion

                        #region LIFETIME

                        EveryPlant[plantName]["maturity"] = "'" + plant.MaturityAgeDays.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region GENERATION
                        EveryPlant[plantName]["isWater"] = plant.Water ? $"'{Localizer.DoStr("Underwater")}'" : "nil"; 
                        EveryPlant[plantName]["height"] = "'" + plant.Height.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region FOOD
                        EveryPlant[plantName]["calorieValue"] = "'" + plant.CalorieValue.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region RESOURCES                       
                        EveryPlant[plantName]["requireHarvestable"] = plant.RequireHarvestable ? $"'{Localizer.DoStr("Yes")}'" : "nil";

                        EveryPlant[plantName]["pickableAtPercent"] = "'" + (plant.PickableAtPercent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";

                        EveryPlant[plantName]["experiencePerHarvest"] = "'" + (plant.ExperiencePerHarvest).ToString("F1", CultureInfo.InvariantCulture) + "'";

                        if (Block.Is<Reapable>(plant.BlockType))
                            EveryPlant[plantName]["harvestTool"] = $"'{Localizer.DoStr("Scythe")}'";
                        else if (Block.Is<Diggable>(plant.BlockType))
                            EveryPlant[plantName]["harvestTool"] = $"'{Localizer.DoStr("Shovel")}'";

                        if (plant.PostHarvestingGrowth == 0)
                            EveryPlant[plantName]["killOnHarvest"] = $"'{Localizer.DoStr("Yes")}'";
                        else
                            EveryPlant[plantName]["killOnHarvest"] = $"'{Localizer.DoStr("No")}'";

                        if (plant.PostHarvestingGrowth != 0)
                            EveryPlant[plantName]["postHarvestGrowth"] = "'" + (plant.PostHarvestingGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";

                        EveryPlant[plantName]["scytheKills"] = plant.ScythingKills ? $"'{Localizer.DoStr("Yes")}'" : "nil";

                        if (plant.ResourceItemType != null) { EveryPlant[plantName]["resourceItem"] = "'[[" + Localizer.DoStr(SplitName(RemoveItemTag(plant.ResourceItemType.Name))) + "]]'"; }

                        EveryPlant[plantName]["resourceMin"] = "'" + plant.ResourceRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryPlant[plantName]["resourceMax"] = "'" + plant.ResourceRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryPlant[plantName]["resourceBonus"] = "'" + (plant.ResourceBonusAtGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'"; 

                        #endregion

                        #region VISUALS

                        #endregion

                        #region WORLDLAYERS
                        EveryPlant[plantName]["carbonRelease"] = "'" + plant.ReleasesCO2TonsPerDay.ToString("F4", CultureInfo.InvariantCulture) + "'"; 

                        EveryPlant[plantName]["idealGrowthRate"] = "'" + plant.MaxGrowthRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        EveryPlant[plantName]["idealDeathRate"] = "'" + plant.MaxDeathRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        EveryPlant[plantName]["spreadRate"] = "'" + plant.SpreadRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        #region Resource Constraints
                        if (plant.ResourceConstraints != null)
                        {
                            foreach (ResourceConstraint r in plant.ResourceConstraints)
                            {
                                if (r.LayerName == "Nitrogen")
                                {
                                    EveryPlant[plantName]["nitrogenHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryPlant[plantName]["nitrogenContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "Phosphorus")
                                {
                                    EveryPlant[plantName]["phosphorusHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryPlant[plantName]["phosphorusContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "Potassium")
                                {
                                    EveryPlant[plantName]["potassiumHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryPlant[plantName]["potassiumContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "SoilMoisture")
                                {
                                    EveryPlant[plantName]["soilMoistureHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryPlant[plantName]["soilMoistureContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                            }
                        }
                        #endregion

                        #region Capacity Constraints
                        if (plant.CapacityConstraints != null)
                        {
                            foreach (CapacityConstraint c in plant.CapacityConstraints)
                            {
                                if (c.CapacityLayerName == "FertileGorund")
                                    EveryPlant[plantName]["consumedFertileGround"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "CanopySpace")
                                    EveryPlant[plantName]["consumedCanopySpace"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "UnderwaterFertileGorund")
                                    EveryPlant[plantName]["consumedUnderwaterFertileGround"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "ShrubSpace")
                                    EveryPlant[plantName]["consumedShrubSpace"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                            }
                        }
                        #endregion

                        #region Environment Ranges

                        // Temperature
                        EveryPlant[plantName]["extremeTempMin"] = "'" + plant.TemperatureExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealTempMin"] = "'" + plant.IdealTemperatureRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealTempMax"] = "'" + plant.IdealTemperatureRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["extremeTempMax"] = "'" + plant.TemperatureExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        // Moisture
                        EveryPlant[plantName]["extremeMoistureMin"] = "'" + plant.MoistureExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealMoistureMin"] = "'" + plant.IdealMoistureRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealMoistureMax"] = "'" + plant.IdealMoistureRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["extremeMoistureMax"] = "'" + plant.MoistureExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        // Salt Content
                        EveryPlant[plantName]["extremeSaltMin"] = "'" + plant.WaterExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealSaltMin"] = "'" + plant.IdealWaterRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["idealSaltMax"] = "'" + plant.IdealWaterRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["extremeSaltMax"] = "'" + plant.WaterExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        #endregion

                        EveryPlant[plantName]["maxPollutionDensity"] = "'" + plant.MaxPollutionDensity.ToString("F4", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["pollutionTolerance"] = "'" + plant.PollutionDensityTolerance.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        #endregion

                        #region UNCATEGORISED
                        #endregion

                        #region OBSOLETE

                        /*
                         
                         * SEEDS
                         
                        if (plant.SeedItemType != null) { EveryPlant[plantName]["seedDrop"] = "'" + SplitName(RemoveItemTag(plant.SeedItemType.Name)) + "'"; }   
                        EveryPlant[plantName]["seedDropChance"] = "'" + (plant.SeedDropChance * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["seedAtGrowth"] = "'" + (plant.SeedsAtGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["seedBonusGrowth"] = "'" + (plant.SeedsBonusAtGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["seedMax"] = "'" + plant.SeedRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryPlant[plantName]["seedMin"] = "'" + plant.SeedRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";


                        */

                        #endregion
                    }
                }
            }
            WriteDictionaryToFile("Wiki_Module_PlantData.txt", "plants", EveryPlant);

        }

        public static void TreeDetails()
        {
            // dictionary of plant properties
            Dictionary<string, string> treeDetails = new Dictionary<string, string>()
            {
                // INFO
                { "untranslated", "nil" },
                { "isDecorative", "nil" },
                { "doesSpread", "nil" },

                // LIFETIME
                { "maturity", "nil" },
                { "treeHealth", "nil" }, // The health of the tree for chopping.
                { "logHealth", "nil" }, // The health of the log for chopping.

                // GENERATION
                { "isWater", "nil" },
                { "height", "nil" },

                // FOOD
                { "calorieValue", "nil" },

                // RESOURCES
                { "requireHarvestable", "nil" },
                { "pickableAtPercent", "nil" },
                { "experiencePerHarvest", "nil" },
                { "harvestTool", "nil" },
                { "killOnHarvest", "nil" },
                { "postHarvestGrowth", "nil" },
                { "scytheKills", "nil" },
                { "resourceItem", "nil" },
                { "resourceMin", "nil" },
                { "resourceMax", "nil" },
                { "debrisSpawnChance", "nil" }, // Chance to spawn debris.
                { "debrisType", "nil" }, // The debris created when chopping this tree. BlockType.
                { "debrisResources", "nil" }, // The resources returned for chopping the debris.
                { "trunkResources", "nil" }, // The resources returned for chopping the trunk.

                // WORLD LAYERS
                { "carbonRelease", "nil" },
                { "idealGrowthRate", "nil" },
                { "idealDeathRate", "nil" },
                { "spreadRate", "nil" },
                { "nitrogenHalfSpeed", "nil" },
                { "nitrogenContent", "nil" },
                { "phosphorusHalfSpeed", "nil" },
                { "phosphorusContent", "nil" },
                { "potassiumHalfSpeed", "nil" },
                { "potassiumContent", "nil" },
                { "soilMoistureHalfSpeed", "nil" },
                { "soilMoistureContent", "nil" },
                { "consumedFertileGround", "nil" },
                { "consumedCanopySpace", "nil" },
                { "consumedUnderwaterFertileGround", "nil" },
                { "consumedShrubSpace", "nil" },
                { "extremeTempMin", "nil" },
                { "idealTempMin", "nil" },
                { "idealTempMax", "nil" },
                { "extremeTempMax", "nil" },
                { "extremeMoistureMin", "nil" },
                { "idealMoistureMin", "nil" },
                { "idealMoistureMax", "nil" },
                { "extremeMoistureMax", "nil" },
                { "extremeSaltMin", "nil" },
                { "idealSaltMin", "nil" },
                { "idealSaltMax", "nil" },
                { "extremeSaltMax", "nil" },
                { "maxPollutionDensity", "nil" },
                { "pollutionTolerance", "nil" }
            };

            IEnumerable<Species> species = EcoSim.AllSpecies;
            foreach (Species s in species)
            {
                if (s is TreeSpecies)
                {
                    TreeSpecies tree = s as TreeSpecies;
                    //Console.WriteLine(tree.Name);
                    if (!EveryTree.ContainsKey(tree.DisplayName))
                    {
                        string treeName = tree.DisplayName;
                        EveryTree.Add(treeName, new Dictionary<string, string>(treeDetails));

                        #region INFO
                        EveryTree[treeName]["untranslated"] = $"'{tree.DisplayName.NotTranslated}'";
                        EveryTree[treeName]["isDecorative"] = tree.Decorative ? $"'{Localizer.DoStr("Decorative")}'" : "nil"; 
                        EveryTree[treeName]["doesSpread"] = tree.NoSpread ? $"'{Localizer.DoStr("No")}'" : $"'{Localizer.DoStr("Yes")}'";
                        #endregion

                        #region LIFETIME
                        EveryTree[treeName]["maturity"] = "'" + tree.MaturityAgeDays.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryTree[treeName]["treeHealth"] = "'" + tree.TreeHealth.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryTree[treeName]["logHealth"] = "'" + tree.LogHealth.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region GENERATION
                        EveryTree[treeName]["isWater"] = tree.Water ? $"'{Localizer.DoStr("Underwater")}'" : "nil"; 
                        EveryTree[treeName]["height"] = "'" + tree.Height.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region FOOD
                        EveryTree[treeName]["calorieValue"] = "'" + tree.CalorieValue.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        #endregion

                        #region RESOURCES
                        EveryTree[treeName]["requireHarvestable"] = tree.RequireHarvestable ? $"'{Localizer.DoStr("Yes")}'" : "nil";
                        EveryTree[treeName]["pickableAtPercent"] = "'" + (tree.PickableAtPercent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["experiencePerHarvest"] = "'" + (tree.ExperiencePerHarvest).ToString("F1", CultureInfo.InvariantCulture) + "'";

                        if (tree.PostHarvestingGrowth == 0)
                            EveryTree[treeName]["killOnHarvest"] = $"'{Localizer.DoStr("Yes")}'";
                        else
                            EveryTree[treeName]["killOnHarvest"] = $"'{Localizer.DoStr("No")}'";

                        if (tree.PostHarvestingGrowth != 0)
                            EveryTree[treeName]["postHarvestGrowth"] = "'" + (tree.PostHarvestingGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";

                        EveryTree[treeName]["scytheKills"] = tree.ScythingKills ? $"'{Localizer.DoStr("Yes")}'" : "nil";

                        if (tree.ResourceItemType != null) { EveryTree[treeName]["resourceItem"] = "'[[" + Localizer.DoStr(SplitName(RemoveItemTag(tree.ResourceItemType.Name))) + "]]'"; }

                        EveryTree[treeName]["resourceMin"] = "'" + tree.ResourceRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryTree[treeName]["resourceMax"] = "'" + tree.ResourceRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'"; 
                        EveryTree[treeName]["resourceBonus"] = "'" + (tree.ResourceBonusAtGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'"; 

                        // Debris
                        EveryTree[treeName]["debrisSpawnChance"] = "'" + (tree.ChanceToSpawnDebris * 100).ToString("F0", CultureInfo.InvariantCulture) + "'"; 
                        EveryTree[treeName]["debrisType"] = "'" + Localizer.DoStr(SplitName(RemoveItemTag(tree.DebrisType.Name))) + "'"; 

                        // The resources returned for chopping the debris.
                        var debrisResources = new StringBuilder();
                        tree.DebrisResources.ForEach(kvp =>
                        {
                            debrisResources.Append("'[[" + Item.Get(kvp.Key).DisplayName + "]]'");
                            if (tree.DebrisResources.Last().Key != kvp.Key)
                            {
                                debrisResources.Append(",");
                            }
                        });
                        EveryTree[treeName]["debrisResources"] = "{" + debrisResources + "}";

                        // The resources returned for chopping the trunk.
                        var trunkResources = new StringBuilder();
                        tree.TrunkResources.ForEach(kvp =>
                        {
                            var item = Item.Get(kvp.Key);
                            if (item != null)
                            {
                                debrisResources.Append("'[[" + item.DisplayName + "]]'");
                                if (tree.TrunkResources.Last().Key != kvp.Key)
                                {
                                    trunkResources.Append(",");
                                }
                            }
                        });
                        EveryTree[treeName]["trunkResources"] = "{" + trunkResources + "}";

                        #endregion

                        #region VISUALS

                        #endregion

                        #region WORLDLAYERS
                        EveryTree[treeName]["carbonRelease"] = "'" + tree.ReleasesCO2TonsPerDay.ToString("F4", CultureInfo.InvariantCulture) + "'"; 

                        EveryTree[treeName]["idealGrowthRate"] = "'" + tree.MaxGrowthRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        EveryTree[treeName]["idealDeathRate"] = "'" + tree.MaxDeathRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        EveryTree[treeName]["spreadRate"] = "'" + tree.SpreadRate.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        // The resource constraints that slow growth rate.
                        #region Resource Constraints
                        if (tree.ResourceConstraints != null)
                        {
                            foreach (ResourceConstraint r in tree.ResourceConstraints)
                            {
                                if (r.LayerName == "Nitrogen")
                                {
                                    EveryTree[treeName]["nitrogenHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryTree[treeName]["nitrogenContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "Phosphorus")
                                {
                                    EveryTree[treeName]["phosphorusHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryTree[treeName]["phosphorusContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "Potassium")
                                {
                                    EveryTree[treeName]["potassiumHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryTree[treeName]["potassiumContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                                if (r.LayerName == "SoilMoisture")
                                {
                                    EveryTree[treeName]["soilMoistureHalfSpeed"] = "'" + (r.HalfSpeedConcentration * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                    EveryTree[treeName]["soilMoistureContent"] = "'" + (r.MaxResourceContent * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";
                                }
                            }
                        }
                        #endregion

                        // The capacity constraints which slow growth.
                        #region Capacity Constraints
                        if (tree.CapacityConstraints != null)
                        {
                            foreach (CapacityConstraint c in tree.CapacityConstraints)
                            {
                                if (c.CapacityLayerName == "FertileGorund")
                                    EveryTree[treeName]["consumedFertileGround"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "CanopySpace")
                                    EveryTree[treeName]["consumedCanopySpace"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "UnderwaterFertileGorund")
                                    EveryTree[treeName]["consumedUnderwaterFertileGround"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                                if (c.CapacityLayerName == "ShrubSpace")
                                    EveryTree[treeName]["consumedShrubSpace"] = "'" + (c.ConsumedCapacityPerPop).ToString("F1", CultureInfo.InvariantCulture) + "'";
                            }
                        }
                        #endregion

                        // The environmental ranges this plant can tolerate.
                        #region Environment Ranges

                        // Temperature
                        EveryTree[treeName]["extremeTempMin"] = "'" + tree.TemperatureExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealTempMin"] = "'" + tree.IdealTemperatureRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealTempMax"] = "'" + tree.IdealTemperatureRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["extremeTempMax"] = "'" + tree.TemperatureExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        // Moisture
                        EveryTree[treeName]["extremeMoistureMin"] = "'" + tree.MoistureExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealMoistureMin"] = "'" + tree.IdealMoistureRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealMoistureMax"] = "'" + tree.IdealMoistureRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["extremeMoistureMax"] = "'" + tree.MoistureExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        // Salt Content
                        EveryTree[treeName]["extremeSaltMin"] = "'" + tree.WaterExtremes.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealSaltMin"] = "'" + tree.IdealWaterRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["idealSaltMax"] = "'" + tree.IdealWaterRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["extremeSaltMax"] = "'" + tree.WaterExtremes.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        #endregion

                        EveryTree[treeName]["maxPollutionDensity"] = "'" + tree.MaxPollutionDensity.ToString("F4", CultureInfo.InvariantCulture) + "'";
                        EveryTree[treeName]["pollutionTolerance"] = "'" + tree.PollutionDensityTolerance.ToString("F4", CultureInfo.InvariantCulture) + "'";

                        #endregion

                        #region UNCATEGORISED
                        #endregion

                        #region OBSOLETE
                        #endregion
                    }
                }
            }
            WriteDictionaryToFile("Wiki_Module_TreeData.txt", "trees", EveryTree);
        }

        public static void AnimalDetails()
        {
            // dictionary of animal properties
            Dictionary<string, string> animalDetails = new Dictionary<string, string>()
            {
                { "untranslated", "nil" },

                // LIFETIME               
                { "maturity", "nil" }, // Age for full maturity and reproduction.

                // MOVEMENT
                { "isSwimming", "nil" }, // Is the animal a swimming one.
                { "isFlying", "nil" }, // Is the animal a flying one.
                { "climbHeight", "nil" }, // What height in meters can this animal effectively climb.

                // BEHAVIOUR
                { "wanderingSpeed", "nil" }, // The animals speed when idle.
                { "speed", "nil" }, // The animals speed when active (hunting, fleeing etc).
                { "health", "nil" }, // The animals health.
                { "damage", "nil" }, // The damage the animal inflicts.
                { "chanceToAttack", "nil" }, //The chance the animal will attack.
                { "attackRange", "nil" }, // The distance at which animal needs to be from its prey to attack.
                { "detectRange", "nil" }, // This distance at which the animal can detect prey.
                { "flees", "nil" }, // Does the animal flee from players by default (not being attacked).
                { "fearFactor", "nil" }, // How quickly the animmal reaches the point where it wants to flee.
                { "headDistance", "nil" }, // The space the animal require around its head (used to figure out pack behaviour for sleeping and wandering etc.)

                { "minAttackDelay", "nil" }, // Minimum possible time before the animal is ready to attack again after making an attack.
                { "maxAttackDelay", "nil" }, // Maximum possible time before the animal is ready to attack again after making an attack.

                // FOOD
                { "calorieValue", "nil" }, // The base calories this species provides to it's consumers.

                // FOOD SOURCES
                { "foodSources", "nil" }, // The species sources this animal eats.

                // RESOURCES
                { "resourceItem", "nil" }, // The item you get from harvesting this animal.
                { "resourceMin", "nil" }, // The minimum number of items returned.
                { "resourceMax", "nil" }, // The maximum number of items returned.
                { "resourceBonus", "nil" }, // The bonus items returned for allowing it to grow.

                // WORLD LAYERS
                { "carbonRelease", "nil" } // The amount of carbon dioxide released by this species. (Animals are postive values)
            };

            IEnumerable<Species> species = EcoSim.AllSpecies;
            foreach (Species s in species)
            {
                if (s is AnimalSpecies)
                {
                    AnimalSpecies animal = s as AnimalSpecies;
                    //Console.WriteLine(animal.Name);
                    if (!EveryAnimal.ContainsKey(animal.DisplayName))
                    {
                        string animalName = animal.DisplayName;
                        EveryAnimal.Add(animalName, new Dictionary<string, string>(animalDetails));

                        EveryAnimal[animalName]["untranslated"] = $"'{animal.DisplayName.NotTranslated}'";

                        #region LIFETIME
                        EveryAnimal[animalName]["maturity"] = "'" + animal.MaturityAgeDays.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        #endregion

                        #region MOVEMENT
                        EveryAnimal[animalName]["isSwimming"] = animal.Swimming ? $"'{Localizer.DoStr("Swimming")}'" : "nil"; // Does the animal swin.
                        EveryAnimal[animalName]["isFlying"] = animal.Flying ? $"'{Localizer.DoStr("Flying")}'" : "nil"; // Does the animal fly.
                        EveryAnimal[animalName]["climbHeight"] = "'" + animal.ClimbHeight.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The height the animal can climb up
                        #endregion

                        #region BEHAVIOUR
                        EveryAnimal[animalName]["wanderingSpeed"] = "'" + animal.WanderingSpeed.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The general wandering speed of the animal.
                        EveryAnimal[animalName]["speed"] = "'" + animal.Speed.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The non-wandering speed of the animal.
                        EveryAnimal[animalName]["health"] = "'" + animal.Health.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The health of the animal.

                        EveryAnimal[animalName]["damage"] = "'" + animal.Damage.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The damage the animal inflicts.
                        EveryAnimal[animalName]["chanceToAttack"] = "'" + animal.ChanceToAttack.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The chance the animal will attack.
                        EveryAnimal[animalName]["attackRange"] = "'" + animal.AttackRange.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The range the animal attacks from.
                        EveryAnimal[animalName]["detectRange"] = "'" + animal.DetectRange.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The range the animal detects others from, default is 5X the attack range.

                        // Time between attacks
                        EveryAnimal[animalName]["minAttackDelay"] = "'" + animal.DelayBetweenAttacksRangeSec.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryAnimal[animalName]["maxAttackDelay"] = "'" + animal.DelayBetweenAttacksRangeSec.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";

                        EveryAnimal[animalName]["flees"] = animal.FleePlayers ? $"'{Localizer.DoStr("Flees")}'" : "nil"; // Will this animal flee players / predators.
                        EveryAnimal[animalName]["fearFactor"] = "'" + animal.FearFactor.ToString("F1", CultureInfo.InvariantCulture) + "'"; // How quickly will the animal flee.
                        EveryAnimal[animalName]["headDistance"] = "'" + animal.HeadDistance.ToString("F1", CultureInfo.InvariantCulture) + "'"; // The default distance from the animals head for calculating various behaviours
                        #endregion

                        #region FOOD
                        EveryAnimal[animalName]["calorieValue"] = "'" + animal.CalorieValue.ToString("F1", CultureInfo.InvariantCulture) + "'"; // Calorie value to consumers.
                        #endregion

                        #region FOOD SOURCES
                        if (animal.FoodSources != null && animal.FoodSources.Count > 0)
                        {
                            var sb = new StringBuilder();
                            int sourceCount = 0;
                            sb.Append("'");
                            foreach (Type meal in animal.FoodSources)
                            {
                                int count = 0;
                                string[] foodNameSplit = Regex.Split(meal.Name, @"(?<!^)(?=[A-Z])");
                                sb.Append("[[");
                                string foodName = "";
                                foreach (string str in foodNameSplit)
                                {       
                                    foodName += str;
                                    count++;
                                    if (count != foodNameSplit.Length)
                                        foodName += " ";
                                }
                                if (LocalizationPlugin.Config.Language == SupportedLanguage.English)
                                    sb.Append(Localizer.DoStr(foodName));
                                else
                                    sb.Append(Localizer.DoStr(meal.Name));
                                sb.Append("]]");
                                sourceCount++;
                                if (sourceCount != animal.FoodSources.Count)
                                    sb.Append(", ");

                            }
                            sb.Append("'");
                            EveryAnimal[animalName]["foodSources"] = sb.ToString();
                        }
                        #endregion

                        #region RESOURCES
                        // Resources returned.
                        EveryAnimal[animalName]["resourceMin"] = "'" + animal.ResourceRange.Min.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryAnimal[animalName]["resourceMax"] = "'" + animal.ResourceRange.Max.ToString("F1", CultureInfo.InvariantCulture) + "'";
                        EveryAnimal[animalName]["resourceBonus"] = "'" + (animal.ResourceBonusAtGrowth * 100).ToString("F0", CultureInfo.InvariantCulture) + "'";

                        if (animal.ResourceItemType != null)
                        {
                            string item = animal.ResourceItemType.Name.Substring(0, animal.ResourceItemType.Name.Length - 4);
                            string[] itemNameSplit = Regex.Split(item, @"(?<!^)(?=[A-Z])");
                            int count = 0;
                            var sb = new StringBuilder();
                            sb.Append("[[");
                            string itemName = "";
                            foreach (string str in itemNameSplit)
                            {
                                
                                itemName += str;
                                count++;
                                if (count != itemNameSplit.Length)
                                    itemName += " ";
                                else
                                    sb.Append(Localizer.DoStr(itemName));                              
                            }
                            sb.Append("]]");
                            EveryAnimal[animalName]["resourceItem"] = "'" + sb.ToString() + "'";
                        }
                        #endregion

                        #region WOLRD LAYERS
                        EveryAnimal[animalName]["carbonRelease"] = "'" + animal.ReleasesCO2TonsPerDay.ToString("F4", CultureInfo.InvariantCulture) + "'";
                        #endregion
                    }
                }
            }
            WriteDictionaryToFile("Wiki_Module_AnimalData.txt", "animals", EveryAnimal);
        }
    }
}