﻿// <copyright file="ChaosMixes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

    /// <summary>
    /// Initializer for chaos mixes.
    /// </summary>
    /// <remarks>
    /// Currently, there are no Talisman of Luck supported. If someone wants to add them, you'd have to do the following steps:
    ///   - The <see cref="ItemDefinition"/> for the TOL needs to be created somewhere.
    ///   - The created item definition needs to be added as <see cref="ItemCraftingRequiredItem"/>
    ///     to the corresponding <see cref="ItemCrafting"/>s with MinimumAmount of 0, Maximum of 1, AddPercentage of 25.
    /// </remarks>
    public class ChaosMixes : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosMixes"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosMixes(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            var chaosGoblin = this.GameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);
            chaosGoblin.ItemCraftings.Add(this.ChaosWeaponCrafting());
            chaosGoblin.ItemCraftings.Add(this.FruitCrafting());
            chaosGoblin.ItemCraftings.Add(this.DinorantCrafting());
            chaosGoblin.ItemCraftings.Add(this.PotionOfBlessCrafting());
            chaosGoblin.ItemCraftings.Add(this.PotionOfSoulCrafting());
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(3, 10, 1_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(4, 11, 2_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(22, 12, 4_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(23, 13, 8_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(49, 14, 10_000_000));
            chaosGoblin.ItemCraftings.Add(this.ItemLevelUpgradeCrafting(50, 15, 12_000_000));
            chaosGoblin.ItemCraftings.Add(this.BloodCastleTicketCrafting());
            chaosGoblin.ItemCraftings.Add(this.DevilSquareTicketCrafting());
            chaosGoblin.ItemCraftings.Add(this.IllusionTempleTicketCrafting());
            chaosGoblin.ItemCraftings.Add(this.LifeStoneCrafting());
            chaosGoblin.ItemCraftings.Add(this.SmallShieldPotionCrafting());
            chaosGoblin.ItemCraftings.Add(this.MediumShieldPotionCrafting());
            chaosGoblin.ItemCraftings.Add(this.LargeShieldPotionCrafting());
            chaosGoblin.ItemCraftings.Add(this.FenrirStage1Crafting());
            chaosGoblin.ItemCraftings.Add(this.FenrirStage2Crafting());
            chaosGoblin.ItemCraftings.Add(this.FenrirStage3Crafting());
            chaosGoblin.ItemCraftings.Add(this.FenrirUpgradeCrafting());
            chaosGoblin.ItemCraftings.Add(this.FirstWingsCrafting());
            chaosGoblin.ItemCraftings.Add(this.CapeCrafting());
            chaosGoblin.ItemCraftings.Add(this.SecondWingsCrafting());
            chaosGoblin.ItemCraftings.Add(this.Level380OptionCrafting());

            var elphis = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.ElphisRefinery);
            elphis.ItemCraftings.Add(this.GemstoneRefinery());

            var petTrainer = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.PetTrainer);
            petTrainer.ItemCraftings.Add(this.DarkHorseCrafting());
            petTrainer.ItemCraftings.Add(this.DarkRavenCrafting());

            var osbourne = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.RefineStoneMaking);
            osbourne.ItemCraftings.Add(this.RefineStoneCrafting());

            var jerridon = this.GameConfiguration.Monsters.Single(m => m.NpcWindow == NpcWindow.RemoveJohOption);
            jerridon.ItemCraftings.Add(this.RestoreItemCrafting());
        }

        private ItemCrafting ItemLevelUpgradeCrafting(byte craftingNumber, byte targetLevel, int money)
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = $"+{targetLevel} Item Combination";
            crafting.Number = craftingNumber;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = money;
            craftingSettings.SuccessPercent = (byte)(60 - (((targetLevel - 10) / 2) * 5));
            craftingSettings.SuccessPercentageAdditionForExcellentItem = -10;
            craftingSettings.SuccessPercentageAdditionForAncientItem = -15;
            craftingSettings.SuccessPercentageAdditionForSocketItem = -20;

            // Requirements:
            var item = this.Context.CreateNew<ItemCraftingRequiredItem>();
            item.Reference = 1;
            item.MinimumAmount = 1;
            item.MaximumAmount = 1;
            item.MinimumItemLevel = (byte)(targetLevel - 1);
            item.MaximumItemLevel = item.MinimumItemLevel;
            item.FailResult = MixResult.DowngradedTo0;
            item.SuccessResult = MixResult.StaysAsIs;
            craftingSettings.RequiredItems.Add(item);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = (byte)(targetLevel - 9);
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = (byte)(targetLevel - 9);
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            craftingSettings.RequiredItems.Add(soul);

            // Result:
            var upgrade = this.Context.CreateNew<ItemCraftingResultItem>();
            upgrade.Reference = 1;
            upgrade.AddLevel = 1;
            craftingSettings.ResultItems.Add(upgrade);
            return crafting;
        }

        private ItemCrafting ChaosWeaponCrafting()
        {
            var chaosWeapon = this.Context.CreateNew<ItemCrafting>();
            chaosWeapon.Name = "Chaos Weapon";
            chaosWeapon.Number = 1;
            var chaosWeaponSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            chaosWeapon.SimpleCraftingSettings = chaosWeaponSettings;
            chaosWeaponSettings.MoneyPerFinalSuccessPercentage = 10000;
            chaosWeaponSettings.SuccessPercent = 1;

            // Requirements:
            var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem.MinimumAmount = 1;
            randomItem.MinimumItemLevel = 4;
            randomItem.MaximumItemLevel = 15;
            randomItem.NpcPriceDivisor = 15000;
            randomItem.FailResult = MixResult.DowngradedRandom;
            randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            randomItem.SuccessResult = MixResult.Disappear;
            chaosWeaponSettings.RequiredItems.Add(randomItem);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.AddPercentage = 2;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaosWeaponSettings.RequiredItems.Add(chaos);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = 0;
            bless.AddPercentage = 5;
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            chaosWeaponSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = 0;
            soul.AddPercentage = 4;
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            chaosWeaponSettings.RequiredItems.Add(soul);

            // Result:
            chaosWeaponSettings.ResultItemSelect = ResultItemSelection.Any;
            chaosWeaponSettings.ResultItemLuckOptionChance = 10;
            chaosWeaponSettings.ResultItemSkillChance = 30;

            var chaosDragonAxe = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosDragonAxe.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Dragon Axe");
            chaosWeaponSettings.ResultItems.Add(chaosDragonAxe);

            var chaosNatureBow = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosNatureBow.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Nature Bow");
            chaosWeaponSettings.ResultItems.Add(chaosNatureBow);

            var chaosLightningStaff = this.Context.CreateNew<ItemCraftingResultItem>();
            chaosLightningStaff.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Chaos Lightning Staff");
            chaosWeaponSettings.ResultItems.Add(chaosLightningStaff);

            return chaosWeapon;
        }

        private ItemCrafting FirstWingsCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "1st Level Wings";
            crafting.Number = 11;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.FirstWingCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.MoneyPerFinalSuccessPercentage = 10000;
            craftingSettings.SuccessPercent = 1;

            // Requirements:
            var chaosWeapon = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaosWeapon.Reference = GameLogic.PlayerActions.Craftings.FirstWingCrafting.ChaosWeaponReference;
            chaosWeapon.MinimumAmount = 1;
            chaosWeapon.MaximumAmount = 1;
            chaosWeapon.MinimumItemLevel = 4;
            chaosWeapon.MaximumItemLevel = 15;
            chaosWeapon.NpcPriceDivisor = 15000;
            chaosWeapon.FailResult = MixResult.DowngradedRandom;
            chaosWeapon.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            chaosWeapon.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(chaosWeapon);

            var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem.MinimumAmount = 0;
            randomItem.MinimumItemLevel = 4;
            randomItem.MaximumItemLevel = 15;
            randomItem.NpcPriceDivisor = 15000;
            randomItem.FailResult = MixResult.DowngradedRandom;
            randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            randomItem.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(randomItem);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.AddPercentage = 2;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = 0;
            bless.AddPercentage = 5;
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = 0;
            soul.AddPercentage = 4;
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            craftingSettings.RequiredItems.Add(soul);

            // Result:
            craftingSettings.ResultItemSelect = ResultItemSelection.Any;
            craftingSettings.ResultItemLuckOptionChance = 10;

            var fairyWings = this.Context.CreateNew<ItemCraftingResultItem>();
            fairyWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 0);
            craftingSettings.ResultItems.Add(fairyWings);

            var heavenWings = this.Context.CreateNew<ItemCraftingResultItem>();
            heavenWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 1);
            craftingSettings.ResultItems.Add(heavenWings);

            var satanWings = this.Context.CreateNew<ItemCraftingResultItem>();
            satanWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 2);
            craftingSettings.ResultItems.Add(satanWings);

            var miseryWings = this.Context.CreateNew<ItemCraftingResultItem>();
            miseryWings.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 41);
            craftingSettings.ResultItems.Add(miseryWings);

            return crafting;
        }

        private ItemCrafting SecondWingsCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "2nd Level Wings";
            crafting.Number = 7;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.SecondWingCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 5_000_000;
            craftingSettings.MoneyPerFinalSuccessPercentage = 10000;
            craftingSettings.SuccessPercent = 0;

            // Requirements:
            var firstWing = this.Context.CreateNew<ItemCraftingRequiredItem>();
            firstWing.Reference = GameLogic.PlayerActions.Craftings.SecondWingCrafting.FirstWingReference;
            firstWing.MinimumAmount = 1;
            firstWing.MaximumAmount = 1;
            firstWing.MinimumItemLevel = 4;
            firstWing.MaximumItemLevel = 15;
            firstWing.NpcPriceDivisor = 4_000_000;
            firstWing.FailResult = MixResult.DowngradedRandom;
            firstWing.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(firstWing);

            var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomExcItem.MinimumAmount = 0;
            randomExcItem.MinimumItemLevel = 4;
            randomExcItem.MaximumItemLevel = 15;
            randomExcItem.NpcPriceDivisor = 40_000;
            randomExcItem.FailResult = MixResult.DowngradedRandom;
            randomExcItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Excellent));
            randomExcItem.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(randomExcItem);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.AddPercentage = 2;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var feather = this.Context.CreateNew<ItemCraftingRequiredItem>();
            feather.MinimumAmount = 1;
            feather.SuccessResult = MixResult.Disappear;
            feather.FailResult = MixResult.Disappear;
            feather.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Loch's Feather");
            craftingSettings.RequiredItems.Add(feather);

            // Result:
            craftingSettings.ResultItemSelect = ResultItemSelection.Any;
            craftingSettings.ResultItemLuckOptionChance = 10;
            craftingSettings.ResultItemExcellentOptionChance = 20;
            craftingSettings.ResultItemMaxExcOptionCount = 1;

            var wingsOfSpirit = this.Context.CreateNew<ItemCraftingResultItem>();
            wingsOfSpirit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 3);
            craftingSettings.ResultItems.Add(wingsOfSpirit);

            var wingsOfSoul = this.Context.CreateNew<ItemCraftingResultItem>();
            wingsOfSoul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 4);
            craftingSettings.ResultItems.Add(wingsOfSoul);

            var wingsOfDragon = this.Context.CreateNew<ItemCraftingResultItem>();
            wingsOfDragon.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 5);
            craftingSettings.ResultItems.Add(wingsOfDragon);

            var wingsOfDarkness = this.Context.CreateNew<ItemCraftingResultItem>();
            wingsOfDarkness.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 6);
            craftingSettings.ResultItems.Add(wingsOfDarkness);

            var wingsOfDespair = this.Context.CreateNew<ItemCraftingResultItem>();
            wingsOfDespair.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 42);
            craftingSettings.ResultItems.Add(wingsOfDespair);

            return crafting;
        }

        private ItemCrafting CapeCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Cape of Lord/Fighter";
            crafting.Number = 24;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.SecondWingCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 5_000_000;
            craftingSettings.MoneyPerFinalSuccessPercentage = 10000;
            craftingSettings.SuccessPercent = 0;

            // Requirements:
            var firstWing = this.Context.CreateNew<ItemCraftingRequiredItem>();
            firstWing.Reference = GameLogic.PlayerActions.Craftings.SecondWingCrafting.FirstWingReference;
            firstWing.MinimumAmount = 1;
            firstWing.MaximumAmount = 1;
            firstWing.MinimumItemLevel = 4;
            firstWing.MaximumItemLevel = 15;
            firstWing.NpcPriceDivisor = 4_000_000;
            firstWing.FailResult = MixResult.DowngradedRandom;
            firstWing.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(firstWing);

            var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomExcItem.MinimumAmount = 0;
            randomExcItem.MinimumItemLevel = 4;
            randomExcItem.MaximumItemLevel = 15;
            randomExcItem.NpcPriceDivisor = 40_000;
            randomExcItem.FailResult = MixResult.DowngradedRandom;
            randomExcItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Excellent));
            randomExcItem.SuccessResult = MixResult.Disappear;
            craftingSettings.RequiredItems.Add(randomExcItem);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.AddPercentage = 2;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var crest = this.Context.CreateNew<ItemCraftingRequiredItem>();
            crest.MinimumAmount = 1;
            crest.SuccessResult = MixResult.Disappear;
            crest.FailResult = MixResult.Disappear;

            // A monarch's crest is a Loch's Feather+1
            crest.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Loch's Feather");
            crest.MinimumItemLevel = 1;
            crest.MaximumItemLevel = 1;
            craftingSettings.RequiredItems.Add(crest);

            // Result:
            craftingSettings.ResultItemSelect = ResultItemSelection.Any;
            craftingSettings.ResultItemLuckOptionChance = 10;

            var capeOfLord = this.Context.CreateNew<ItemCraftingResultItem>();
            capeOfLord.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 13 && i.Number == 30);
            craftingSettings.ResultItems.Add(capeOfLord);

            var capeOfFighter = this.Context.CreateNew<ItemCraftingResultItem>();
            capeOfFighter.ItemDefinition = this.GameConfiguration.Items.First(i => i.Group == 13 && i.Number == 49);
            craftingSettings.ResultItems.Add(capeOfFighter);

            return crafting;
        }

        private ItemCrafting FruitCrafting()
        {
            var fruitCrafting = this.Context.CreateNew<ItemCrafting>();
            fruitCrafting.Name = "Fruits";
            fruitCrafting.Number = 6;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            fruitCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 3_000_000;
            craftingSettings.SuccessPercent = 90;

            // Requirements:
            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
            creation.MinimumAmount = 1;
            creation.MaximumAmount = 1;
            creation.SuccessResult = MixResult.Disappear;
            creation.FailResult = MixResult.Disappear;
            creation.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation");
            craftingSettings.RequiredItems.Add(creation);

            // Result:
            var fruit = this.Context.CreateNew<ItemCraftingResultItem>();
            fruit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Fruits");
            fruit.RandomMinimumLevel = 0;
            fruit.RandomMaximumLevel = 4;
            craftingSettings.ResultItems.Add(fruit);

            return fruitCrafting;
        }

        private ItemCrafting PotionOfBlessCrafting()
        {
            var potionCrafting = this.Context.CreateNew<ItemCrafting>();
            potionCrafting.Name = "Potion of Bless";
            potionCrafting.Number = 15;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            potionCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 100_000;
            craftingSettings.SuccessPercent = 100;
            craftingSettings.MultipleAllowed = true;

            // Requirements:
            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.MinimumAmount = 1;
            bless.Reference = 1;
            bless.SuccessResult = MixResult.Disappear;
            bless.FailResult = MixResult.Disappear;
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            craftingSettings.RequiredItems.Add(bless);

            // Result:
            var potion = this.Context.CreateNew<ItemCraftingResultItem>();
            potion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Siege Potion");
            potion.Durability = 10;
            craftingSettings.ResultItems.Add(potion);

            return potionCrafting;
        }

        private ItemCrafting PotionOfSoulCrafting()
        {
            var potionCrafting = this.Context.CreateNew<ItemCrafting>();
            potionCrafting.Name = "Potion of Soul";
            potionCrafting.Number = 16;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            potionCrafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 50_000;
            craftingSettings.SuccessPercent = 100;
            craftingSettings.MultipleAllowed = true;

            // Requirements:
            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.MinimumAmount = 1;
            soul.Reference = 1;
            soul.SuccessResult = MixResult.Disappear;
            soul.FailResult = MixResult.Disappear;
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            craftingSettings.RequiredItems.Add(soul);

            // Result:
            var potion = this.Context.CreateNew<ItemCraftingResultItem>();
            potion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Siege Potion");
            potion.Durability = 10;
            potion.RandomMinimumLevel = 1;
            potion.RandomMaximumLevel = 1;
            craftingSettings.ResultItems.Add(potion);

            return potionCrafting;
        }

        private ItemCrafting GemstoneRefinery()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Gemstone Refinery";
            crafting.Number = 33;
            crafting.SimpleCraftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            crafting.SimpleCraftingSettings.SuccessPercent = 80;

            // Requirements:
            var gemstone = this.Context.CreateNew<ItemCraftingRequiredItem>();
            gemstone.MinimumAmount = 1;
            gemstone.MaximumAmount = 1;
            gemstone.SuccessResult = MixResult.Disappear;
            gemstone.FailResult = MixResult.Disappear;
            gemstone.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Gemstone");
            crafting.SimpleCraftingSettings.RequiredItems.Add(gemstone);

            // Result:
            var harmony = this.Context.CreateNew<ItemCraftingResultItem>();
            harmony.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Harmony");
            crafting.SimpleCraftingSettings.ResultItems.Add(harmony);

            return crafting;
        }

        private ItemCrafting DinorantCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Dinorant";
            crafting.Number = 5;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();

            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 500_000;
            craftingSettings.SuccessPercent = 70;
            craftingSettings.ResultItemExcellentOptionChance = 10;
            craftingSettings.ResultItemSkillChance = 100;

            // Requirements:
            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            chaos.SuccessResult = MixResult.Disappear;
            chaos.FailResult = MixResult.Disappear;
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            craftingSettings.RequiredItems.Add(chaos);

            var horn = this.Context.CreateNew<ItemCraftingRequiredItem>();
            horn.MinimumAmount = 10;
            horn.MaximumAmount = 10;
            horn.SuccessResult = MixResult.Disappear;
            horn.FailResult = MixResult.Disappear;
            horn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Uniria");
            craftingSettings.RequiredItems.Add(horn);

            // Result:
            var dinorant = this.Context.CreateNew<ItemCraftingResultItem>();
            dinorant.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Dinorant");
            craftingSettings.ResultItems.Add(dinorant);

            return crafting;
        }

        private ItemCrafting BloodCastleTicketCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Blood Castle Ticket";
            crafting.Number = 8;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.BloodCastleTicketCrafting).FullName;
            return crafting;
        }

        private ItemCrafting DevilSquareTicketCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Devil's Square Ticket";
            crafting.Number = 2;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.DevilSquareTicketCrafting).FullName;
            return crafting;
        }

        private ItemCrafting IllusionTempleTicketCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Illusion Temple Ticket";
            crafting.Number = 37;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.IllusionTempleTicketCrafting).FullName;
            return crafting;
        }

        private ItemCrafting DarkHorseCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Dark Horse";
            crafting.Number = 13;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 5_000_000;
            craftingSettings.SuccessPercent = 60;
            crafting.SimpleCraftingSettings = craftingSettings;

            var spirit = this.Context.CreateNew<ItemCraftingRequiredItem>();
            spirit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Spirit");
            spirit.MinimumAmount = 1;
            spirit.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(spirit);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            bless.MinimumAmount = 5;
            bless.MaximumAmount = 5;
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            soul.MinimumAmount = 5;
            soul.MaximumAmount = 5;
            craftingSettings.RequiredItems.Add(soul);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
            creation.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation");
            creation.MinimumAmount = 1;
            creation.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(creation);

            var darkHorse = this.Context.CreateNew<ItemCraftingResultItem>();
            darkHorse.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Dark Horse");
            darkHorse.Durability = 255;
            craftingSettings.ResultItems.Add(darkHorse);
            craftingSettings.ResultItemSkillChance = 100;

            return crafting;
        }

        private ItemCrafting DarkRavenCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Dark Raven";
            crafting.Number = 14;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 1_000_000;
            craftingSettings.SuccessPercent = 60;
            crafting.SimpleCraftingSettings = craftingSettings;

            var spirit = this.Context.CreateNew<ItemCraftingRequiredItem>();
            spirit.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Spirit");
            spirit.MinimumAmount = 1;
            spirit.MaximumAmount = 1;
            spirit.MinimumItemLevel = 1;
            spirit.MaximumItemLevel = 1;
            craftingSettings.RequiredItems.Add(spirit);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            bless.MinimumAmount = 2;
            bless.MaximumAmount = 2;
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            soul.MinimumAmount = 2;
            soul.MaximumAmount = 2;
            craftingSettings.RequiredItems.Add(soul);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var creation = this.Context.CreateNew<ItemCraftingRequiredItem>();
            creation.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Creation");
            creation.MinimumAmount = 1;
            creation.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(creation);

            var darkRaven = this.Context.CreateNew<ItemCraftingResultItem>();
            darkRaven.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Dark Raven");
            darkRaven.Durability = 255;
            craftingSettings.ResultItems.Add(darkRaven);
            craftingSettings.ResultItemSkillChance = 100;

            return crafting;
        }

        private ItemCrafting SmallShieldPotionCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Small Shield Potion";
            crafting.Number = 30;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 100_000;
            craftingSettings.SuccessPercent = 50;
            crafting.SimpleCraftingSettings = craftingSettings;

            var healthPotion = this.Context.CreateNew<ItemCraftingRequiredItem>();
            healthPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Large Healing Potion");
            healthPotion.MinimumAmount = 3;
            healthPotion.MaximumAmount = 3;
            healthPotion.MaximumItemLevel = 15;
            craftingSettings.RequiredItems.Add(healthPotion);

            var shieldPotion = this.Context.CreateNew<ItemCraftingResultItem>();
            shieldPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Small Shield Potion");
            shieldPotion.Durability = 3;
            craftingSettings.ResultItems.Add(shieldPotion);

            return crafting;
        }

        private ItemCrafting MediumShieldPotionCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Medium Shield Potion";
            crafting.Number = 31;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 500_000;
            craftingSettings.SuccessPercent = 30;
            crafting.SimpleCraftingSettings = craftingSettings;

            var complexPotion = this.Context.CreateNew<ItemCraftingRequiredItem>();
            complexPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Small Complex Potion");
            complexPotion.MinimumAmount = 3;
            complexPotion.MaximumAmount = 3;
            complexPotion.MaximumItemLevel = 1;
            craftingSettings.RequiredItems.Add(complexPotion);

            var shieldPotion = this.Context.CreateNew<ItemCraftingResultItem>();
            shieldPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Medium Shield Potion");
            shieldPotion.Durability = 3;
            craftingSettings.ResultItems.Add(shieldPotion);

            return crafting;
        }

        private ItemCrafting LargeShieldPotionCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Large Shield Potion";
            crafting.Number = 32;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 1_000_000;
            craftingSettings.SuccessPercent = 30;
            crafting.SimpleCraftingSettings = craftingSettings;

            var complexPotion = this.Context.CreateNew<ItemCraftingRequiredItem>();
            complexPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Medium Complex Potion");
            complexPotion.MinimumAmount = 3;
            complexPotion.MaximumAmount = 3;
            complexPotion.MaximumItemLevel = 1;
            craftingSettings.RequiredItems.Add(complexPotion);

            var shieldPotion = this.Context.CreateNew<ItemCraftingResultItem>();
            shieldPotion.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Large Shield Potion");
            shieldPotion.Durability = 3;
            craftingSettings.ResultItems.Add(shieldPotion);

            return crafting;
        }

        private ItemCrafting LifeStoneCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Life Stone";
            crafting.Number = 17;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 5_000_000;
            craftingSettings.SuccessPercent = 100;
            crafting.SimpleCraftingSettings = craftingSettings;

            var guardian = this.Context.CreateNew<ItemCraftingRequiredItem>();
            guardian.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Guardian");
            guardian.MinimumAmount = 1;
            guardian.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(guardian);

            var bless = this.Context.CreateNew<ItemCraftingRequiredItem>();
            bless.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Bless");
            bless.MinimumAmount = 5;
            bless.MaximumAmount = 5;
            craftingSettings.RequiredItems.Add(bless);

            var soul = this.Context.CreateNew<ItemCraftingRequiredItem>();
            soul.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Soul");
            soul.MinimumAmount = 5;
            soul.MaximumAmount = 5;
            craftingSettings.RequiredItems.Add(soul);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var lifeStone = this.Context.CreateNew<ItemCraftingResultItem>();
            lifeStone.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Life Stone");
            lifeStone.Durability = 1;
            craftingSettings.ResultItems.Add(lifeStone);

            return crafting;
        }

        private ItemCrafting FenrirStage1Crafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Fenrir Stage 1";
            crafting.Number = 25;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 0;
            craftingSettings.SuccessPercent = 70;
            crafting.SimpleCraftingSettings = craftingSettings;

            var guardian = this.Context.CreateNew<ItemCraftingRequiredItem>();
            guardian.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Bless of Guardian");
            guardian.MinimumAmount = 20;
            guardian.MaximumAmount = 20;
            craftingSettings.RequiredItems.Add(guardian);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var splinter = this.Context.CreateNew<ItemCraftingRequiredItem>();
            splinter.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Splinter of Armor");
            splinter.MinimumAmount = 20;
            splinter.MaximumAmount = 20;
            craftingSettings.RequiredItems.Add(splinter);

            var fragmentOfHorn = this.Context.CreateNew<ItemCraftingResultItem>();
            fragmentOfHorn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Fragment of Horn");
            fragmentOfHorn.Durability = 1;
            craftingSettings.ResultItems.Add(fragmentOfHorn);

            return crafting;
        }

        private ItemCrafting FenrirStage2Crafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Fenrir Stage 2";
            crafting.Number = 26;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 0;
            craftingSettings.SuccessPercent = 50;
            crafting.SimpleCraftingSettings = craftingSettings;

            var fragmentOfHorn = this.Context.CreateNew<ItemCraftingRequiredItem>();
            fragmentOfHorn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Fragment of Horn");
            fragmentOfHorn.MinimumAmount = 5;
            fragmentOfHorn.MaximumAmount = 5;
            craftingSettings.RequiredItems.Add(fragmentOfHorn);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var clawOfBeast = this.Context.CreateNew<ItemCraftingRequiredItem>();
            clawOfBeast.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Claw of Beast");
            clawOfBeast.MinimumAmount = 10;
            clawOfBeast.MaximumAmount = 10;
            craftingSettings.RequiredItems.Add(clawOfBeast);

            var brokenHorn = this.Context.CreateNew<ItemCraftingResultItem>();
            brokenHorn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Broken Horn");
            brokenHorn.Durability = 1;
            craftingSettings.ResultItems.Add(brokenHorn);

            return crafting;
        }

        private ItemCrafting FenrirStage3Crafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Fenrir Stage 3";
            crafting.Number = 27;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            craftingSettings.Money = 10_000_000;
            craftingSettings.SuccessPercent = 30;
            crafting.SimpleCraftingSettings = craftingSettings;

            var brokenHorn = this.Context.CreateNew<ItemCraftingRequiredItem>();
            brokenHorn.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Broken Horn");
            brokenHorn.MinimumAmount = 1;
            brokenHorn.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(brokenHorn);

            var chaos = this.Context.CreateNew<ItemCraftingRequiredItem>();
            chaos.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Chaos");
            chaos.MinimumAmount = 1;
            chaos.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(chaos);

            var life = this.Context.CreateNew<ItemCraftingRequiredItem>();
            life.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Life");
            life.MinimumAmount = 3;
            life.MaximumAmount = 3;
            craftingSettings.RequiredItems.Add(life);

            var fenrir = this.Context.CreateNew<ItemCraftingResultItem>();
            fenrir.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Horn of Fenrir");
            fenrir.Durability = 255;
            craftingSettings.ResultItemSkillChance = 100;
            craftingSettings.ResultItems.Add(fenrir);

            return crafting;
        }

        private ItemCrafting FenrirUpgradeCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Fenrir Upgrade (Stage 4)";
            crafting.Number = 28;
            crafting.ItemCraftingHandlerClassName = typeof(GameLogic.PlayerActions.Craftings.FenrirUpgradeCrafting).FullName;
            return crafting;
        }

        private ItemCrafting RefineStoneCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Refine Stone";
            crafting.Number = 34;
            crafting.ItemCraftingHandlerClassName = typeof(RefineStoneCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.SuccessPercent = 100; // It will be handled when creating the stones.

            var randomExcItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomExcItem.MinimumAmount = 0;
            randomExcItem.MaximumAmount = 32;
            randomExcItem.MaximumItemLevel = 15;
            randomExcItem.Reference = GameLogic.PlayerActions.Craftings.RefineStoneCrafting.HigherRefineStoneReference;
            randomExcItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Excellent));
            craftingSettings.RequiredItems.Add(randomExcItem);

            var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem.MinimumAmount = 0;
            randomItem.MaximumAmount = 32;
            randomItem.MaximumItemLevel = 15;
            randomItem.Reference = GameLogic.PlayerActions.Craftings.RefineStoneCrafting.LowerRefineStoneReference;
            craftingSettings.RequiredItems.Add(randomItem);

            return crafting;
        }

        private ItemCrafting RestoreItemCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Restore Item (Remove JOH Option)";
            crafting.Number = 35;
            crafting.ItemCraftingHandlerClassName = typeof(RestoreItemCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.SuccessPercent = 100;

            var randomItem = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem.MinimumAmount = 1;
            randomItem.MinimumAmount = 1;
            randomItem.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.HarmonyOption));
            randomItem.FailResult = MixResult.StaysAsIs; // It's never failing, but we set it just in case.
            randomItem.SuccessResult = MixResult.StaysAsIs;
            craftingSettings.RequiredItems.Add(randomItem);

            return crafting;
        }

        private ItemCrafting Level380OptionCrafting()
        {
            var crafting = this.Context.CreateNew<ItemCrafting>();
            crafting.Name = "Guardian Option (Level 380)";
            crafting.Number = 36;
            crafting.ItemCraftingHandlerClassName = typeof(GuardianOptionCrafting).FullName;
            var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
            crafting.SimpleCraftingSettings = craftingSettings;
            craftingSettings.Money = 10_000_000;

            var randomItem4To6 = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem4To6.MaximumAmount = 1;
            randomItem4To6.MinimumItemLevel = 4;
            randomItem4To6.MaximumItemLevel = 6;
            randomItem4To6.AddPercentage = 50;
            randomItem4To6.FailResult = MixResult.StaysAsIs;
            randomItem4To6.SuccessResult = MixResult.StaysAsIs;
            randomItem4To6.Reference = GuardianOptionCrafting.ItemReference;
            randomItem4To6.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            craftingSettings.RequiredItems.Add(randomItem4To6);

            var randomItem7To9 = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem7To9.MaximumAmount = 1;
            randomItem7To9.MinimumItemLevel = 7;
            randomItem7To9.MaximumItemLevel = 9;
            randomItem7To9.AddPercentage = 60;
            randomItem7To9.FailResult = MixResult.StaysAsIs;
            randomItem7To9.SuccessResult = MixResult.StaysAsIs;
            randomItem7To9.Reference = GuardianOptionCrafting.ItemReference;
            randomItem7To9.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            craftingSettings.RequiredItems.Add(randomItem7To9);

            var randomItem10To13 = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem10To13.MaximumAmount = 1;
            randomItem10To13.MinimumItemLevel = 10;
            randomItem10To13.MaximumItemLevel = 13;
            randomItem10To13.AddPercentage = 70;
            randomItem10To13.FailResult = MixResult.StaysAsIs;
            randomItem10To13.SuccessResult = MixResult.StaysAsIs;
            randomItem10To13.Reference = GuardianOptionCrafting.ItemReference;
            randomItem10To13.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            craftingSettings.RequiredItems.Add(randomItem10To13);

            var randomItem14To15 = this.Context.CreateNew<ItemCraftingRequiredItem>();
            randomItem14To15.MaximumAmount = 1;
            randomItem14To15.MinimumItemLevel = 14;
            randomItem14To15.MaximumItemLevel = 15;
            randomItem14To15.AddPercentage = 80;
            randomItem14To15.FailResult = MixResult.StaysAsIs;
            randomItem14To15.SuccessResult = MixResult.StaysAsIs;
            randomItem14To15.Reference = GuardianOptionCrafting.ItemReference;
            randomItem14To15.RequiredItemOptions.Add(this.GameConfiguration.ItemOptionTypes.First(o => o == ItemOptionTypes.Option));
            craftingSettings.RequiredItems.Add(randomItem14To15);

            var harmony = this.Context.CreateNew<ItemCraftingRequiredItem>();
            harmony.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Harmony");
            harmony.MinimumAmount = 1;
            harmony.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(harmony);

            var guardian = this.Context.CreateNew<ItemCraftingRequiredItem>();
            guardian.ItemDefinition = this.GameConfiguration.Items.First(i => i.Name == "Jewel of Guardian");
            guardian.MinimumAmount = 1;
            guardian.MaximumAmount = 1;
            craftingSettings.RequiredItems.Add(guardian);

            return crafting;
        }
    }
}
