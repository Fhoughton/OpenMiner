// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ShopScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;

namespace StudioForge.TotalMiner.Screens
{
  internal class ShopScreen : ChestScreen
  {
    private int shopHeight;
    private bool isPlayerShop;
    private string shopTitle;
    private Block shopType;
    private Action<Item> onItemSelected;
    protected Texture2D checkboxOff;
    protected Rectangle triggerRect;

    protected override bool AllowRapidLiftSinglePressX
    {
      get
      {
        return true;
      }
    }

    private bool CanTradeCurrentItem
    {
      get
      {
        if (this.CursorItem.ItemID == Item.SkeletonKey && !this.isPlayerShop && this.currentSlotID >= 30)
          return this.player.IsAdmin;
        return true;
      }
    }

    public ShopScreen(GameInstance instance, Player player, Inventory inventory)
      : base(instance, player, inventory)
    {
    }

    public ShopScreen(GameInstance instance, Player player, Inventory inventory, Action onExit)
      : base(instance, player, inventory)
    {
      this.onExit = onExit;
    }

    public ShopScreen(GameInstance instance, Player player, Block shopType)
      : base(instance, player, shopType)
    {
      this.shopType = shopType;
    }

    public ShopScreen(
      GameInstance instance,
      Player player,
      Block shopType,
      Action<Item> onItemSelected)
      : base(instance, player, shopType)
    {
      this.shopType = shopType;
      this.onItemSelected = onItemSelected;
    }

    public ShopScreen(GameInstance instance, Player player, GlobalPoint3D p, Block shopType)
      : base(instance, player, p, shopType)
    {
      this.shopType = shopType;
    }

    public override void LoadContent()
    {
      if (this.instance.LocalPlayerCount == 1 || this.instance.LocalPlayerCount == 2 && Globals2.GameSettings.SplitScreenVertical)
      {
        this.pagesize = 50;
        this.shopHeight = 297;
      }
      else
      {
        this.pagesize = 30;
        this.shopHeight = 37 + this.pagesize / 10 * 46;
      }
      this.showItemCounts = this.isPlayerShop = this.shopType != Block.None && this.instance.Map.GetAuxData(this.chest.Point) == (byte) 1;
      if (this.isPlayerShop)
      {
        this.shopTitle = this.chest.Gamertag + "' Shop";
      }
      else
      {
        this.shopTitle = this.shopType == Block.None ? "Spawn Items" : "Game " + (this.shopType == Block.BlockShop ? "Block" : "Item") + " Shop";
        Inventory inventory = new Inventory((int) short.MaxValue);
        if (this.shopType == Block.ItemShop)
          this.AddItemsToShopInventory(inventory);
        else if (this.shopType == Block.BlockShop)
          this.AddBlocksToShopInventory(inventory);
        else if (this.shopType == Block.None)
        {
          this.AddItemsToShopInventory(inventory);
          this.AddBlocksToShopInventory(inventory);
        }
        this.chest.Inventory = new Inventory((short) (inventory.Count + 1), (short) 0, (short) 0, inventory);
      }
      base.LoadContent();
      this.triggerRect = new Rectangle(0, 0, 12, 24);
      this.checkboxOff = CoreGlobals.Content.Load<Texture2D>("Textures\\CheckboxOff");
    }

    protected override void OnScreenClosed()
    {
      if (!this.IsPlayerInventory)
        return;
      if (this.isPlayerShop)
      {
        if (this.instance == null)
          return;
        this.instance.CloseSpecialBlockScreen(this.player, (DataBlock) this.chest, false);
      }
      else
      {
        if (this.instance.MapStrategyTM == null)
          return;
        this.instance.MapStrategyTM.RemoveDataBlock((DataBlock) this.chest);
      }
    }

    public static void AddToShopInventory(
      Block shopType,
      GameInstance instance,
      Player player,
      Inventory inventory,
      bool isPlayerShop)
    {
      switch (shopType)
      {
        case Block.None:
          ShopScreen.AddItemsToShopInventory(instance, player, inventory, isPlayerShop);
          ShopScreen.AddBlocksToShopInventory(instance, player, inventory, isPlayerShop);
          break;
        case Block.ItemShop:
          ShopScreen.AddItemsToShopInventory(instance, player, inventory, isPlayerShop);
          break;
        case Block.BlockShop:
          ShopScreen.AddBlocksToShopInventory(instance, player, inventory, isPlayerShop);
          break;
      }
    }

    protected virtual void AddItemsToShopInventory(Inventory inventory)
    {
      ShopScreen.AddItemsToShopInventory(this.instance, this.player, inventory, this.isPlayerShop);
    }

    private static bool GetIsFiniteMode(GameInstance instance, Player player)
    {
      if (instance == null || !instance.IsFiniteResources)
        return false;
      if (player != null)
        return !player.IsAdmin;
      return true;
    }

    public static void AddItemsToShopInventory(
      GameInstance instance,
      Player player,
      Inventory inventory,
      bool isPlayerShop)
    {
      if (instance.IsAvatarDesigner)
      {
        ShopScreen.AddToInventory(inventory, Item.SledgeHammer, player);
      }
      else
      {
        bool flag = true;
        if (!ShopScreen.GetIsFiniteMode(instance, player) || isPlayerShop)
          ShopScreen.AddToInventory(inventory, Item.GoldPieces, player);
        ShopScreen.AddToInventory(inventory, Item.Stick, player);
        ShopScreen.AddToInventory(inventory, Item.Torch, player);
        if (instance.IsCreativeMode || player.IsGod)
          ShopScreen.AddToInventory(inventory, Item.SledgeHammer, player);
        ShopScreen.AddToInventory(inventory, Item.WoodPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.WoodHatchet, player);
        ShopScreen.AddToInventory(inventory, Item.WoodShovel, player);
        ShopScreen.AddToInventory(inventory, Item.WoodSpear, player);
        ShopScreen.AddToInventory(inventory, Item.WoodSword, player);
        ShopScreen.AddToInventory(inventory, Item.CopperIngot, player);
        ShopScreen.AddToInventory(inventory, Item.TinIngot, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeIngot, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeHoe, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeScythe, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeSpear, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeSword, player);
        ShopScreen.AddToInventory(inventory, Item.IronIngot, player);
        ShopScreen.AddToInventory(inventory, Item.IronPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.IronHatchet, player);
        ShopScreen.AddToInventory(inventory, Item.IronShovel, player);
        ShopScreen.AddToInventory(inventory, Item.IronHoe, player);
        ShopScreen.AddToInventory(inventory, Item.IronScythe, player);
        ShopScreen.AddToInventory(inventory, Item.IronSpear, player);
        ShopScreen.AddToInventory(inventory, Item.IronSword, player);
        ShopScreen.AddToInventory(inventory, Item.IronBattleAxe, player);
        ShopScreen.AddToInventory(inventory, Item.SteelIngot, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.SteelHatchet, player);
        ShopScreen.AddToInventory(inventory, Item.SteelShovel, player);
        ShopScreen.AddToInventory(inventory, Item.SteelHoe, player);
        ShopScreen.AddToInventory(inventory, Item.SteelScythe, player);
        ShopScreen.AddToInventory(inventory, Item.SteelSpear, player);
        ShopScreen.AddToInventory(inventory, Item.SteelSword, player);
        ShopScreen.AddToInventory(inventory, Item.SteelScimitar, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPike, player);
        ShopScreen.AddToInventory(inventory, Item.SteelClaymore, player);
        ShopScreen.AddToInventory(inventory, Item.SteelKatana, player);
        ShopScreen.AddToInventory(inventory, Item.SteelBattleAxe, player);
        if (instance.IsCreativeMode || player.IsGod)
          ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldSledgeHammer, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldHatchet, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldShovel, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldSword, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldBattleAxe, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondHatchet, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondShovel, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondHoe, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondScythe, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondSpear, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondSword, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondBattleAxe, player);
        ShopScreen.AddToInventory(inventory, Item.PlatinumSword, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumSword, player);
        ShopScreen.AddToInventory(inventory, Item.RubyPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.RubySword, player);
        ShopScreen.AddToInventory(inventory, Item.RubyWarHammer, player);
        ShopScreen.AddToInventory(inventory, Item.RubyBattleAxe, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumPickaxe, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumSword, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumKatana, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumWarHammer, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumBattleAxe, player);
        ShopScreen.AddToInventory(inventory, Item.WoodBow, player);
        ShopScreen.AddToInventory(inventory, Item.GoldenBow, player);
        ShopScreen.AddToInventory(inventory, Item.SpiderBow, player);
        ShopScreen.AddToInventory(inventory, Item.TrollBow, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumBow, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.ElvenBow, player);
        ShopScreen.AddToInventory(inventory, Item.FlintArrow, player);
        ShopScreen.AddToInventory(inventory, Item.BronzeArrow, player);
        ShopScreen.AddToInventory(inventory, Item.IronArrow, player);
        ShopScreen.AddToInventory(inventory, Item.SteelArrow, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondArrow, player);
        ShopScreen.AddToInventory(inventory, Item.RubyArrow, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumArrow, player);
        ShopScreen.AddToInventory(inventory, Item.BoomArrow, player);
        ShopScreen.AddToInventory(inventory, Item.IceArrow, player);
        ShopScreen.AddToInventory(inventory, Item.FireArrow, player);
        ShopScreen.AddToInventory(inventory, Item.Wand, player);
        ShopScreen.AddToInventory(inventory, Item.NatureStaff, player);
        ShopScreen.AddToInventory(inventory, Item.LightStaff, player);
        ShopScreen.AddToInventory(inventory, Item.DarkStaff, player);
        ShopScreen.AddToInventory(inventory, Item.SpiderStaff, player);
        ShopScreen.AddToInventory(inventory, Item.OceanStaff, player);
        ShopScreen.AddToInventory(inventory, Item.NecromancerStaff, player);
        ShopScreen.AddToInventory(inventory, Item.Bullet, player);
        ShopScreen.AddToInventory(inventory, Item.Revolver, player);
        ShopScreen.AddToInventory(inventory, Item.SemiAutoHandGun, player);
        ShopScreen.AddToInventory(inventory, Item.EldarPistol, player);
        ShopScreen.AddToInventory(inventory, Item.Shotgun, player);
        ShopScreen.AddToInventory(inventory, Item.GoldenSMG, player);
        ShopScreen.AddToInventory(inventory, Item.SpiderSMG, player);
        ShopScreen.AddToInventory(inventory, Item.LaserBlaster, player);
        ShopScreen.AddToInventory(inventory, Item.PlasmaRifle, player);
        ShopScreen.AddToInventory(inventory, Item.AssaultRifle, player);
        ShopScreen.AddToInventory(inventory, Item.HeavyAssaultRifle, player);
        ShopScreen.AddToInventory(inventory, Item.ComboAssaultRifle, player);
        ShopScreen.AddToInventory(inventory, Item.MiniGun, player);
        ShopScreen.AddToInventory(inventory, Item.SniperRifle, player);
        ShopScreen.AddToInventory(inventory, Item.GrenadeLauncher, player);
        ShopScreen.AddToInventory(inventory, Item.Grenade, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.ShieldBadge, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.BattleAxe, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.WaterTalisman, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.AmuletOfFlight, player);
        if (flag)
          ShopScreen.AddToInventory(inventory, Item.TenLeagueBoots, player);
        ShopScreen.AddToInventory(inventory, Item.DebugTool, player);
        ShopScreen.AddToInventory(inventory, Item.DecalApplicator, player);
        ShopScreen.AddToInventory(inventory, Item.Lock, player);
        ShopScreen.AddToInventory(inventory, Item.Camera, player);
        ShopScreen.AddToInventory(inventory, Item.Binoculars, player);
        ShopScreen.AddToInventory(inventory, Item.Lighter, player);
        ShopScreen.AddToInventory(inventory, Item.Chisel, player);
        ShopScreen.AddToInventory(inventory, Item.FlintFlake, player);
        ShopScreen.AddToInventory(inventory, Item.Feather, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHide, player);
        ShopScreen.AddToInventory(inventory, Item.CowHide, player);
        ShopScreen.AddToInventory(inventory, Item.Leather, player);
        ShopScreen.AddToInventory(inventory, Item.Bucket, player);
        ShopScreen.AddToInventory(inventory, Item.BucketOfMilk, player);
        ShopScreen.AddToInventory(inventory, Item.BucketOfWater, player);
        ShopScreen.AddToInventory(inventory, Item.BucketOfLava, player);
        ShopScreen.AddToInventory(inventory, Item.Bottle, player);
        ShopScreen.AddToInventory(inventory, Item.BottleOfWater, player);
        ShopScreen.AddToInventory(inventory, Item.BottleOfMilk, player);
        ShopScreen.AddToInventory(inventory, Item.CookedBeef, player);
        ShopScreen.AddToInventory(inventory, Item.RawBeef, player);
        ShopScreen.AddToInventory(inventory, Item.CookedFish, player);
        ShopScreen.AddToInventory(inventory, Item.RawFish, player);
        ShopScreen.AddToInventory(inventory, Item.CookedDuckMeat, player);
        ShopScreen.AddToInventory(inventory, Item.RawDuckMeat, player);
        ShopScreen.AddToInventory(inventory, Item.CookedLambChops, player);
        ShopScreen.AddToInventory(inventory, Item.RawLambChops, player);
        ShopScreen.AddToInventory(inventory, Item.Egg, player);
        ShopScreen.AddToInventory(inventory, Item.Salt, player);
        ShopScreen.AddToInventory(inventory, Item.BoneMeal, player);
        ShopScreen.AddToInventory(inventory, Item.WheatSeed, player);
        ShopScreen.AddToInventory(inventory, Item.SugarCaneSeed, player);
        ShopScreen.AddToInventory(inventory, Item.TomatoSeed, player);
        ShopScreen.AddToInventory(inventory, Item.Potato, player);
        ShopScreen.AddToInventory(inventory, Item.Corn, player);
        ShopScreen.AddToInventory(inventory, Item.Wheat, player);
        ShopScreen.AddToInventory(inventory, Item.Sugar, player);
        ShopScreen.AddToInventory(inventory, Item.Tomato, player);
        ShopScreen.AddToInventory(inventory, Item.Flour, player);
        ShopScreen.AddToInventory(inventory, Item.Butter, player);
        ShopScreen.AddToInventory(inventory, Item.Cheese, player);
        ShopScreen.AddToInventory(inventory, Item.Dough, player);
        ShopScreen.AddToInventory(inventory, Item.Bread, player);
        ShopScreen.AddToInventory(inventory, Item.Cornbread, player);
        ShopScreen.AddToInventory(inventory, Item.Cake, player);
        ShopScreen.AddToInventory(inventory, Item.Pizza, player);
        ShopScreen.AddToInventory(inventory, Item.PotatoPie, player);
        ShopScreen.AddToInventory(inventory, Item.Apple, player);
        ShopScreen.AddToInventory(inventory, Item.Orange, player);
        ShopScreen.AddToInventory(inventory, Item.Plum, player);
        ShopScreen.AddToInventory(inventory, Item.Olives, player);
        ShopScreen.AddToInventory(inventory, Item.Cherries, player);
        ShopScreen.AddToInventory(inventory, Item.Strawberries, player);
        ShopScreen.AddToInventory(inventory, Item.Blueberries, player);
        ShopScreen.AddToInventory(inventory, Item.Banana, player);
        ShopScreen.AddToInventory(inventory, Item.Lemon, player);
        ShopScreen.AddToInventory(inventory, Item.Lime, player);
        ShopScreen.AddToInventory(inventory, Item.Grapes, player);
        ShopScreen.AddToInventory(inventory, Item.Raspberries, player);
        ShopScreen.AddToInventory(inventory, Item.Gooseberries, player);
        ShopScreen.AddToInventory(inventory, Item.Grapefruit, player);
        ShopScreen.AddToInventory(inventory, Item.Blackberries, player);
        ShopScreen.AddToInventory(inventory, Item.Rosemary, player);
        ShopScreen.AddToInventory(inventory, Item.Oregano, player);
        ShopScreen.AddToInventory(inventory, Item.Thyme, player);
        ShopScreen.AddToInventory(inventory, Item.Sage, player);
        ShopScreen.AddToInventory(inventory, Item.Tarragon, player);
        ShopScreen.AddToInventory(inventory, Item.Dill, player);
        ShopScreen.AddToInventory(inventory, Item.Basil, player);
        ShopScreen.AddToInventory(inventory, Item.BayLeaves, player);
        ShopScreen.AddToInventory(inventory, Item.Parsley, player);
        ShopScreen.AddToInventory(inventory, Item.Chives, player);
        ShopScreen.AddToInventory(inventory, Item.Mint, player);
        ShopScreen.AddToInventory(inventory, Item.Coriander, player);
        ShopScreen.AddToInventory(inventory, Item.Majoram, player);
        ShopScreen.AddToInventory(inventory, Item.Lavender, player);
        ShopScreen.AddToInventory(inventory, Item.Fennel, player);
        ShopScreen.AddToInventory(inventory, Item.YliasterPotion, player);
        ShopScreen.AddToInventory(inventory, Item.EitrPotion, player);
        ShopScreen.AddToInventory(inventory, Item.IchorPotion, player);
        ShopScreen.AddToInventory(inventory, Item.MagicPotion, player);
        ShopScreen.AddToInventory(inventory, Item.VaporPotion, player);
        ShopScreen.AddToInventory(inventory, Item.EssenciaPotion, player);
        ShopScreen.AddToInventory(inventory, Item.AstroPotion, player);
        ShopScreen.AddToInventory(inventory, Item.EarthPotion, player);
        ShopScreen.AddToInventory(inventory, Item.AestusFlask, player);
        ShopScreen.AddToInventory(inventory, Item.DwarvenPotion, player);
        ShopScreen.AddToInventory(inventory, Item.EctoplasmFlask, player);
        ShopScreen.AddToInventory(inventory, Item.VolatileConcoction, player);
        ShopScreen.AddToInventory(inventory, Item.SkoomaFlask, player);
        ShopScreen.AddToInventory(inventory, Item.ResplendentMixture, player);
        ShopScreen.AddToInventory(inventory, Item.KarmicPotion, player);
        ShopScreen.AddToInventory(inventory, Item.GoldBar, player);
        ShopScreen.AddToInventory(inventory, Item.SapphireGemStone, player);
        ShopScreen.AddToInventory(inventory, Item.RubyGemStone, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondGemStone, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumIngot, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumIngot, player);
        ShopScreen.AddToInventory(inventory, Item.ObsidianGemStone, player);
        ShopScreen.AddToInventory(inventory, Item.RingMould, player);
        ShopScreen.AddToInventory(inventory, Item.AmuletMould, player);
        ShopScreen.AddToInventory(inventory, Item.NecklaceMould, player);
        ShopScreen.AddToInventory(inventory, Item.GoldRing, player);
        ShopScreen.AddToInventory(inventory, Item.GoldNecklace, player);
        ShopScreen.AddToInventory(inventory, Item.GoldAmulet, player);
        ShopScreen.AddToInventory(inventory, Item.RingOfBob, player);
        ShopScreen.AddToInventory(inventory, Item.AmuletOfFury, player);
        ShopScreen.AddToInventory(inventory, Item.NecklaceOfKnowledge, player);
        ShopScreen.AddToInventory(inventory, Item.RingOfIce, player);
        ShopScreen.AddToInventory(inventory, Item.UnknownAmulet, player);
        ShopScreen.AddToInventory(inventory, Item.UnknownNecklace, player);
        ShopScreen.AddToInventory(inventory, Item.SpiderRing, player);
        ShopScreen.AddToInventory(inventory, Item.PredatorAmulet, player);
        ShopScreen.AddToInventory(inventory, Item.NecklaceOfHypocrisy, player);
        ShopScreen.AddToInventory(inventory, Item.RingOfExemption, player);
        ShopScreen.AddToInventory(inventory, Item.AmuletOfStarlight, player);
        ShopScreen.AddToInventory(inventory, Item.NecklaceOfFarsight, player);
        ShopScreen.AddToInventory(inventory, Item.Coif, player);
        ShopScreen.AddToInventory(inventory, Item.LeatherGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.LeatherHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.LeatherBoots, player);
        ShopScreen.AddToInventory(inventory, Item.LeatherLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.LeatherBody, player);
        ShopScreen.AddToInventory(inventory, Item.WoodShield, player);
        ShopScreen.AddToInventory(inventory, Item.IronShield, player);
        ShopScreen.AddToInventory(inventory, Item.IronGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.IronHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.IronBoots, player);
        ShopScreen.AddToInventory(inventory, Item.IronLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.IronBody, player);
        ShopScreen.AddToInventory(inventory, Item.SteelShield, player);
        ShopScreen.AddToInventory(inventory, Item.SteelGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.SteelHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.SteelBoots, player);
        ShopScreen.AddToInventory(inventory, Item.SteelLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.SteelBody, player);
        ShopScreen.AddToInventory(inventory, Item.GreenstoneGoldShield, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHideGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHideHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHideBoots, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHideLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.TrollHideBody, player);
        ShopScreen.AddToInventory(inventory, Item.DiamondShield, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumShield, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumBoots, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.DiamantiumBody, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumShield, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumGauntlets, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumHelmet, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumBoots, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumLeggings, player);
        ShopScreen.AddToInventory(inventory, Item.TitaniumBody, player);
        foreach (Item key in instance.KeyList)
          ShopScreen.AddToInventory(inventory, key, player);
        for (Item itemID = Item.SkillCombat; itemID < Item.Splinter1; ++itemID)
          ShopScreen.AddToInventory(inventory, itemID, player);
        for (int index = 595; index < Globals1.ItemData.Length; ++index)
        {
          ItemDataXML itemDataXml = Globals1.ItemData[index];
          if (itemDataXml.IsValid && !itemDataXml.HasItemProxy)
            ShopScreen.AddToInventory(inventory, (Item) index, player);
        }
        if (!player.IsGod)
          return;
        for (int index = 257; index < Globals1.ItemData.Length; ++index)
        {
          if (!inventory.HasItem((Item) index))
            ShopScreen.AddToInventory(inventory, (Item) index, player);
        }
      }
    }

    private void AddBlocksToShopInventory(Inventory inventory)
    {
      ShopScreen.AddBlocksToShopInventory(this.instance, this.player, inventory, this.isPlayerShop);
    }

    public static void AddBlocksToShopInventory(
      GameInstance instance,
      Player player,
      Inventory inventory,
      bool isPlayerShop)
    {
      if (!instance.IsAvatarDesigner)
      {
        if (!ShopScreen.GetIsFiniteMode(instance, player) || isPlayerShop)
          ShopScreen.AddToInventory(inventory, Item.GoldPieces, player);
        ShopScreen.AddToInventory(inventory, Item.Marker, player);
        ShopScreen.AddToInventory(inventory, Item.ExcludeMarker, player);
        ShopScreen.AddToInventory(inventory, Item.CoverBlock, player);
        ShopScreen.AddToInventory(inventory, Item.Grass, player);
        ShopScreen.AddToInventory(inventory, Item.GrassShaded, player);
        ShopScreen.AddToInventory(inventory, Item.GrassyStone, player);
        ShopScreen.AddToInventory(inventory, Item.Dirt, player);
        ShopScreen.AddToInventory(inventory, Item.TilledEarth, player);
        ShopScreen.AddToInventory(inventory, Item.Sand, player);
        ShopScreen.AddToInventory(inventory, Item.Scoria, player);
        ShopScreen.AddToInventory(inventory, Item.Wood, player);
        ShopScreen.AddToInventory(inventory, Item.WoodPlank, player);
        ShopScreen.AddToInventory(inventory, Item.BirchWood, player);
        ShopScreen.AddToInventory(inventory, Item.BirchWoodPlank, player);
        ShopScreen.AddToInventory(inventory, Item.Leaves, player);
        ShopScreen.AddToInventory(inventory, Item.PineLeaves, player);
        ShopScreen.AddToInventory(inventory, Item.MapleLeaves, player);
        ShopScreen.AddToInventory(inventory, Item.WovenLeaves, player);
        ShopScreen.AddToInventory(inventory, Item.ClimbingIvy, player);
        ShopScreen.AddToInventory(inventory, Item.Clay, player);
        ShopScreen.AddToInventory(inventory, Item.Sandstone, player);
        ShopScreen.AddToInventory(inventory, Item.Limestone, player);
        ShopScreen.AddToInventory(inventory, Item.Basalt, player);
        ShopScreen.AddToInventory(inventory, Item.Andesite, player);
        ShopScreen.AddToInventory(inventory, Item.Dacite, player);
        ShopScreen.AddToInventory(inventory, Item.Diorite, player);
        ShopScreen.AddToInventory(inventory, Item.Tuff, player);
        ShopScreen.AddToInventory(inventory, Item.Serpentine, player);
        ShopScreen.AddToInventory(inventory, Item.Gabbro, player);
        ShopScreen.AddToInventory(inventory, Item.Granite, player);
        ShopScreen.AddToInventory(inventory, Item.Komatiite, player);
        ShopScreen.AddToInventory(inventory, Item.Marble, player);
        ShopScreen.AddToInventory(inventory, Item.Rhyolite, player);
        ShopScreen.AddToInventory(inventory, Item.Carbon, player);
        ShopScreen.AddToInventory(inventory, Item.Obsidian, player);
        ShopScreen.AddToInventory(inventory, Item.Coal, player);
        ShopScreen.AddToInventory(inventory, Item.Flint, player);
        ShopScreen.AddToInventory(inventory, Item.Copper, player);
        ShopScreen.AddToInventory(inventory, Item.Cassiterite, player);
        ShopScreen.AddToInventory(inventory, Item.Iron, player);
        ShopScreen.AddToInventory(inventory, Item.Gold, player);
        ShopScreen.AddToInventory(inventory, Item.Sapphire, player);
        ShopScreen.AddToInventory(inventory, Item.Diamond, player);
        ShopScreen.AddToInventory(inventory, Item.Ruby, player);
        ShopScreen.AddToInventory(inventory, Item.Platinum, player);
        ShopScreen.AddToInventory(inventory, Item.Opal, player);
        ShopScreen.AddToInventory(inventory, Item.Greenstone, player);
        ShopScreen.AddToInventory(inventory, Item.SaltBlock, player);
        ShopScreen.AddToInventory(inventory, Item.Sulphur, player);
        ShopScreen.AddToInventory(inventory, Item.Cyclonite, player);
        ShopScreen.AddToInventory(inventory, Item.Fluorite, player);
        ShopScreen.AddToInventory(inventory, Item.Titanium, player);
        ShopScreen.AddToInventory(inventory, Item.Uranium, player);
        ShopScreen.AddToInventory(inventory, Item.Workbench, player);
        ShopScreen.AddToInventory(inventory, Item.Furnace, player);
        ShopScreen.AddToInventory(inventory, Item.Crate, player);
        ShopScreen.AddToInventory(inventory, Item.Chest, player);
        ShopScreen.AddToInventory(inventory, Item.LockedChest, player);
        ShopScreen.AddToInventory(inventory, Item.Safe, player);
        ShopScreen.AddToInventory(inventory, Item.Ladder, player);
        ShopScreen.AddToInventory(inventory, Item.Torch, player);
        ShopScreen.AddToInventory(inventory, Item.WoodDoor, player);
        ShopScreen.AddToInventory(inventory, Item.SteelDoor, player);
        ShopScreen.AddToInventory(inventory, Item.LockedDoor, player);
        ShopScreen.AddToInventory(inventory, Item.TrapDoor, player);
        ShopScreen.AddToInventory(inventory, Item.StairsIcon, player);
        ShopScreen.AddToInventory(inventory, Item.Stairs2Icon, player);
        ShopScreen.AddToInventory(inventory, Item.HalfBlockIcon, player);
        ShopScreen.AddToInventory(inventory, Item.HalfBlock2Icon, player);
        ShopScreen.AddToInventory(inventory, Item.CornerBlock, player);
        ShopScreen.AddToInventory(inventory, Item.CornerBlock2, player);
        ShopScreen.AddToInventory(inventory, Item.RampIcon, player);
        ShopScreen.AddToInventory(inventory, Item.Ramp2Icon, player);
        ShopScreen.AddToInventory(inventory, Item.CylinderIcon, player);
        ShopScreen.AddToInventory(inventory, Item.Post, player);
        ShopScreen.AddToInventory(inventory, Item.Post2, player);
        ShopScreen.AddToInventory(inventory, Item.SidePost, player);
        ShopScreen.AddToInventory(inventory, Item.SidePost2, player);
        ShopScreen.AddToInventory(inventory, Item.Stack, player);
        ShopScreen.AddToInventory(inventory, Item.Stack2, player);
        ShopScreen.AddToInventory(inventory, Item.UpsideDownStack, player);
        ShopScreen.AddToInventory(inventory, Item.SignIcon, player);
        ShopScreen.AddToInventory(inventory, Item.FenceIcon, player);
        ShopScreen.AddToInventory(inventory, Item.TableIcon, player);
        ShopScreen.AddToInventory(inventory, Item.Bed, player);
        ShopScreen.AddToInventory(inventory, Item.RopeIcon, player);
        ShopScreen.AddToInventory(inventory, Item.BlockShop, player);
        ShopScreen.AddToInventory(inventory, Item.ItemShop, player);
        ShopScreen.AddToInventory(inventory, Item.Pane, player);
        ShopScreen.AddToInventory(inventory, Item.Glass, player);
        ShopScreen.AddToInventory(inventory, Item.StainedGlass, player);
        ShopScreen.AddToInventory(inventory, Item.StainedGlassPane, player);
        ShopScreen.AddToInventory(inventory, Item.OneWayGlass, player);
        ShopScreen.AddToInventory(inventory, Item.InvisibleBarrier, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPortcullis, player);
        ShopScreen.AddToInventory(inventory, Item.Scaffold, player);
        ShopScreen.AddToInventory(inventory, Item.MultiTextureBlock, player);
        ShopScreen.AddToInventory(inventory, Item.MultiTextureBlock2, player);
        ShopScreen.AddToInventory(inventory, Item.ScriptBlock, player);
        ShopScreen.AddToInventory(inventory, Item.AmbientSoundBlock, player);
        ShopScreen.AddToInventory(inventory, Item.PoweredLight, player);
        ShopScreen.AddToInventory(inventory, Item.SunBox, player);
        ShopScreen.AddToInventory(inventory, Item.PressurePlate, player);
        ShopScreen.AddToInventory(inventory, Item.SwitchIcon, player);
        ShopScreen.AddToInventory(inventory, Item.ButtonIcon, player);
        ShopScreen.AddToInventory(inventory, Item.WifiTransmitter, player);
        ShopScreen.AddToInventory(inventory, Item.WifiReceiver, player);
        ShopScreen.AddToInventory(inventory, Item.TNT, player);
        ShopScreen.AddToInventory(inventory, Item.C4, player);
        ShopScreen.AddToInventory(inventory, Item.SteelSpikes, player);
        ShopScreen.AddToInventory(inventory, Item.HealthBlock, player);
        ShopScreen.AddToInventory(inventory, Item.SentryTurret, player);
        ShopScreen.AddToInventory(inventory, Item.ProximityDetector, player);
        ShopScreen.AddToInventory(inventory, Item.ParticleEmitter, player);
        ShopScreen.AddToInventory(inventory, Item.ArcadeMachine, player);
        ShopScreen.AddToInventory(inventory, Item.NPCSpawn, player);
        ShopScreen.AddToInventory(inventory, Item.SpiderEgg, player);
        ShopScreen.AddToInventory(inventory, Item.Book, player);
        ShopScreen.AddToInventory(inventory, Item.Fire, player);
        ShopScreen.AddToInventory(inventory, Item.Bookcase, player);
        ShopScreen.AddToInventory(inventory, Item.Painting, player);
        ShopScreen.AddToInventory(inventory, Item.GoldBlock, player);
        ShopScreen.AddToInventory(inventory, Item.WoodVeneer, player);
        ShopScreen.AddToInventory(inventory, Item.GildedWoodPanel, player);
        ShopScreen.AddToInventory(inventory, Item.Teflon, player);
        ShopScreen.AddToInventory(inventory, Item.Ice, player);
        ShopScreen.AddToInventory(inventory, Item.Bricks, player);
        ShopScreen.AddToInventory(inventory, Item.SandBrick, player);
        ShopScreen.AddToInventory(inventory, Item.ConcreteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.StoneBrick, player);
        ShopScreen.AddToInventory(inventory, Item.StoneWall, player);
        ShopScreen.AddToInventory(inventory, Item.Cobblestone, player);
        ShopScreen.AddToInventory(inventory, Item.MossyCobblestone, player);
        ShopScreen.AddToInventory(inventory, Item.TerracottaTile, player);
        ShopScreen.AddToInventory(inventory, Item.Rasta, player);
        ShopScreen.AddToInventory(inventory, Item.WhiteTile, player);
        ShopScreen.AddToInventory(inventory, Item.Checkered, player);
        ShopScreen.AddToInventory(inventory, Item.Retro, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPlating, player);
        ShopScreen.AddToInventory(inventory, Item.CherryMetal, player);
        ShopScreen.AddToInventory(inventory, Item.Static, player);
        ShopScreen.AddToInventory(inventory, Item.Chess, player);
        ShopScreen.AddToInventory(inventory, Item.Turkish, player);
        ShopScreen.AddToInventory(inventory, Item.Camouflage, player);
        ShopScreen.AddToInventory(inventory, Item.BlueBox, player);
        ShopScreen.AddToInventory(inventory, Item.Console, player);
        ShopScreen.AddToInventory(inventory, Item.TechPanel, player);
        ShopScreen.AddToInventory(inventory, Item.TechLamp, player);
        ShopScreen.AddToInventory(inventory, Item.TechFurnace, player);
        ShopScreen.AddToInventory(inventory, Item.PlasmaConduit, player);
        ShopScreen.AddToInventory(inventory, Item.SteelVent, player);
        ShopScreen.AddToInventory(inventory, Item.Teleport, player);
        ShopScreen.AddToInventory(inventory, Item.Snow, player);
        ShopScreen.AddToInventory(inventory, Item.SnowLayer, player);
        ShopScreen.AddToInventory(inventory, Item.Cloud, player);
        ShopScreen.AddToInventory(inventory, Item.WhiteWool, player);
        ShopScreen.AddToInventory(inventory, Item.Bone, player);
        ShopScreen.AddToInventory(inventory, Item.Cactus, player);
        ShopScreen.AddToInventory(inventory, Item.BerryBush, player);
        ShopScreen.AddToInventory(inventory, Item.LongGrass, player);
        ShopScreen.AddToInventory(inventory, Item.WhiteFlowers, player);
        ShopScreen.AddToInventory(inventory, Item.YellowFlowers, player);
        ShopScreen.AddToInventory(inventory, Item.RedFlowers, player);
        ShopScreen.AddToInventory(inventory, Item.PurpleFlowers, player);
        ShopScreen.AddToInventory(inventory, Item.Sapling, player);
        ShopScreen.AddToInventory(inventory, Item.RedMushroom, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedBasalt, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedBasaltBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedAndesite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedAndesiteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedDacite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedDaciteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedDiorite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedDioriteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedTuff, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedTuffBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedSerpentine, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedSerpentineBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGabbro, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGabbroBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGranite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGraniteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedKomatiite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedKomatiiteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedMarble, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedMarbleBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedRhyolite, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedRhyoliteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGreenstone, player);
        ShopScreen.AddToInventory(inventory, Item.PolishedGreenstoneBrick, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBlack, player);
        ShopScreen.AddToInventory(inventory, Item.ColorWhite, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkGray, player);
        ShopScreen.AddToInventory(inventory, Item.ColorGray, player);
        ShopScreen.AddToInventory(inventory, Item.ColorSmoothGray, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorCyan, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLime, player);
        ShopScreen.AddToInventory(inventory, Item.ColorYellow, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightYellow, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkRed, player);
        ShopScreen.AddToInventory(inventory, Item.ColorRed, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBloodOrange, player);
        ShopScreen.AddToInventory(inventory, Item.ColorOrange, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightOrange, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLeather, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorTan, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightTan, player);
        ShopScreen.AddToInventory(inventory, Item.ColorCreme, player);
        ShopScreen.AddToInventory(inventory, Item.ColorPurple, player);
        ShopScreen.AddToInventory(inventory, Item.ColorPink, player);
        if (player == null || !player.IsGod)
          return;
        for (int index = 1; index < (int) byte.MaxValue; ++index)
        {
          if (!inventory.HasItem((Item) index))
            ShopScreen.AddToInventory(inventory, (Item) index, player);
        }
      }
      else
      {
        ShopScreen.AddToInventory(inventory, Item.Marker, player);
        ShopScreen.AddToInventory(inventory, Item.ExcludeMarker, player);
        ShopScreen.AddToInventory(inventory, Item.Grass, player);
        ShopScreen.AddToInventory(inventory, Item.GrassShaded, player);
        ShopScreen.AddToInventory(inventory, Item.GrassyStone, player);
        ShopScreen.AddToInventory(inventory, Item.Dirt, player);
        ShopScreen.AddToInventory(inventory, Item.TilledEarth, player);
        ShopScreen.AddToInventory(inventory, Item.Sand, player);
        ShopScreen.AddToInventory(inventory, Item.Scoria, player);
        ShopScreen.AddToInventory(inventory, Item.Wood, player);
        ShopScreen.AddToInventory(inventory, Item.WoodPlank, player);
        ShopScreen.AddToInventory(inventory, Item.BirchWood, player);
        ShopScreen.AddToInventory(inventory, Item.BirchWoodPlank, player);
        ShopScreen.AddToInventory(inventory, Item.WovenLeaves, player);
        ShopScreen.AddToInventory(inventory, Item.Clay, player);
        ShopScreen.AddToInventory(inventory, Item.Sandstone, player);
        ShopScreen.AddToInventory(inventory, Item.Limestone, player);
        ShopScreen.AddToInventory(inventory, Item.Basalt, player);
        ShopScreen.AddToInventory(inventory, Item.Andesite, player);
        ShopScreen.AddToInventory(inventory, Item.Dacite, player);
        ShopScreen.AddToInventory(inventory, Item.Diorite, player);
        ShopScreen.AddToInventory(inventory, Item.Tuff, player);
        ShopScreen.AddToInventory(inventory, Item.Serpentine, player);
        ShopScreen.AddToInventory(inventory, Item.Gabbro, player);
        ShopScreen.AddToInventory(inventory, Item.Granite, player);
        ShopScreen.AddToInventory(inventory, Item.Komatiite, player);
        ShopScreen.AddToInventory(inventory, Item.Marble, player);
        ShopScreen.AddToInventory(inventory, Item.Rhyolite, player);
        ShopScreen.AddToInventory(inventory, Item.Carbon, player);
        ShopScreen.AddToInventory(inventory, Item.Coal, player);
        ShopScreen.AddToInventory(inventory, Item.Flint, player);
        ShopScreen.AddToInventory(inventory, Item.Copper, player);
        ShopScreen.AddToInventory(inventory, Item.Cassiterite, player);
        ShopScreen.AddToInventory(inventory, Item.Iron, player);
        ShopScreen.AddToInventory(inventory, Item.Gold, player);
        ShopScreen.AddToInventory(inventory, Item.Sapphire, player);
        ShopScreen.AddToInventory(inventory, Item.Diamond, player);
        ShopScreen.AddToInventory(inventory, Item.Ruby, player);
        ShopScreen.AddToInventory(inventory, Item.Platinum, player);
        ShopScreen.AddToInventory(inventory, Item.Opal, player);
        ShopScreen.AddToInventory(inventory, Item.Greenstone, player);
        ShopScreen.AddToInventory(inventory, Item.SaltBlock, player);
        ShopScreen.AddToInventory(inventory, Item.Sulphur, player);
        ShopScreen.AddToInventory(inventory, Item.Cyclonite, player);
        ShopScreen.AddToInventory(inventory, Item.Fluorite, player);
        ShopScreen.AddToInventory(inventory, Item.Titanium, player);
        ShopScreen.AddToInventory(inventory, Item.Uranium, player);
        ShopScreen.AddToInventory(inventory, Item.Glass, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPortcullis, player);
        ShopScreen.AddToInventory(inventory, Item.Scaffold, player);
        ShopScreen.AddToInventory(inventory, Item.SunBox, player);
        ShopScreen.AddToInventory(inventory, Item.GoldBlock, player);
        ShopScreen.AddToInventory(inventory, Item.WoodVeneer, player);
        ShopScreen.AddToInventory(inventory, Item.GildedWoodPanel, player);
        ShopScreen.AddToInventory(inventory, Item.Ice, player);
        ShopScreen.AddToInventory(inventory, Item.Bricks, player);
        ShopScreen.AddToInventory(inventory, Item.SandBrick, player);
        ShopScreen.AddToInventory(inventory, Item.ConcreteBrick, player);
        ShopScreen.AddToInventory(inventory, Item.StoneBrick, player);
        ShopScreen.AddToInventory(inventory, Item.StoneWall, player);
        ShopScreen.AddToInventory(inventory, Item.Cobblestone, player);
        ShopScreen.AddToInventory(inventory, Item.MossyCobblestone, player);
        ShopScreen.AddToInventory(inventory, Item.TerracottaTile, player);
        ShopScreen.AddToInventory(inventory, Item.Rasta, player);
        ShopScreen.AddToInventory(inventory, Item.WhiteTile, player);
        ShopScreen.AddToInventory(inventory, Item.Checkered, player);
        ShopScreen.AddToInventory(inventory, Item.Retro, player);
        ShopScreen.AddToInventory(inventory, Item.SteelPlating, player);
        ShopScreen.AddToInventory(inventory, Item.CherryMetal, player);
        ShopScreen.AddToInventory(inventory, Item.Static, player);
        ShopScreen.AddToInventory(inventory, Item.Chess, player);
        ShopScreen.AddToInventory(inventory, Item.Turkish, player);
        ShopScreen.AddToInventory(inventory, Item.Camouflage, player);
        ShopScreen.AddToInventory(inventory, Item.BlueBox, player);
        ShopScreen.AddToInventory(inventory, Item.Snow, player);
        ShopScreen.AddToInventory(inventory, Item.WhiteWool, player);
        ShopScreen.AddToInventory(inventory, Item.Bone, player);
        ShopScreen.AddToInventory(inventory, Item.Cactus, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBlack, player);
        ShopScreen.AddToInventory(inventory, Item.ColorWhite, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkGray, player);
        ShopScreen.AddToInventory(inventory, Item.ColorGray, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightBlue, player);
        ShopScreen.AddToInventory(inventory, Item.ColorCyan, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightGreen, player);
        ShopScreen.AddToInventory(inventory, Item.ColorYellow, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightYellow, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkRed, player);
        ShopScreen.AddToInventory(inventory, Item.ColorRed, player);
        ShopScreen.AddToInventory(inventory, Item.ColorOrange, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightOrange, player);
        ShopScreen.AddToInventory(inventory, Item.ColorDarkBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorLightBrown, player);
        ShopScreen.AddToInventory(inventory, Item.ColorTan, player);
        ShopScreen.AddToInventory(inventory, Item.ColorCreme, player);
        ShopScreen.AddToInventory(inventory, Item.ColorPurple, player);
        ShopScreen.AddToInventory(inventory, Item.ColorPink, player);
      }
    }

    protected static void AddToInventory(Inventory inventory, Item itemID, Player player)
    {
      ShopScreen.AddToInventory(inventory, itemID, 1, player);
    }

    private static void AddToInventory(Inventory inventory, Item itemID, int count, Player player)
    {
      ItemDataXML itemDataXml = Globals1.ItemData[(int) itemID];
      if ((!itemDataXml.IsValid || !itemDataXml.IsEnabled && (player == null || !player.OverrideIsEnabledInShop)) && (player == null || !player.IsGod))
        return;
      InventoryItem inventoryItem1 = new InventoryItem(itemID, 0);
      int getFreeSlotForItem = inventory.FindOrGetFreeSlotForItem(inventoryItem1);
      if (getFreeSlotForItem < 0)
        return;
      InventoryItem inventoryItem2 = inventory[getFreeSlotForItem];
      inventoryItem2.ItemID = itemID;
      inventoryItem2.Count += (int) (ushort) count;
      inventoryItem2.Durability = ItemData.GetItemDurability(itemID);
      inventory[getFreeSlotForItem] = inventoryItem2;
    }

    protected override int PageCount
    {
      get
      {
        return (this.chest.Inventory.Count - 1) / this.pagesize + 1;
      }
    }

    protected override int CoWindowHeight
    {
      get
      {
        return this.pagesize / 10 * 46 + 3 + 60;
      }
    }

    protected override bool CanEditInventory
    {
      get
      {
        return true;
      }
    }

    protected override void TransferItems()
    {
      if (this.chest.IsOwner(this.player) && this.currentSlotID > 29)
      {
        ShopBlock chest = this.chest as ShopBlock;
        PriceList priceList = chest == null || chest.PriceList == null ? this.player.DefaultPriceList : chest.PriceList;
        if (priceList == null)
          return;
        this.ScreenManager.AddScreen((GameScreen) new PriceScreen(this.instance, this.player, priceList, this.CursorItem.ItemID_Raw, chest, (Action<PriceList.Price>) null), this.ControllingPlayer);
      }
      else
      {
        if (!this.player.IsGod || !this.IsPlayerInventory)
          return;
        this.player.AddToInventory(Item.GoldPieces, 1000000);
      }
    }

    protected override void LiftAllButtonPressed()
    {
      this.Trade(1);
    }

    protected override void LiftSingleButtonPressed()
    {
      this.Trade(this.singlePressQuantity);
    }

    private bool IsItemForSale(Item itemID)
    {
      ShopBlock chest = this.chest as ShopBlock;
      PriceList priceList = !this.isPlayerShop || chest == null || chest.PriceList == null ? this.player.DefaultPriceList : chest.PriceList;
      if (!this.isPlayerShop || priceList == null)
        return true;
      return priceList.Prices[(int) itemID].ForSale;
    }

    protected override bool ExamineItemOverride(InventoryItem item)
    {
      if (item.ItemID == Item.Book && this.currentSlotID < 30)
        return false;
      this.ScreenManager.AddScreen((GameScreen) new InteractItemScreen(this.instance, this.player, item), this.ControllingPlayer);
      return true;
    }

    private void Trade(int qty)
    {
      if (!this.CursorItemLocked && this.CanTradeCurrentItem)
      {
        if (!this.isPlayerShop && (!this.IsFiniteMode || this.IsCreativeMode && this.player != null && this.player.IsAdmin) || this.player != null && this.player.IsGodOrTester)
          this.InfiniteTrade(qty);
        else
          this.FiniteTrade(qty);
      }
      else
        CoreGlobals.AudioManager.PlaySound(MenuScreen.DefaultMenuInvalidOperationSound);
    }

    private void InfiniteTrade(int qty)
    {
      if (this.onItemSelected != null)
      {
        this.onItemSelected(this.CursorItem.ItemID);
        this.ExitScreen();
      }
      else
      {
        if (this.isPlayerShop || this.currentSlotID < 30)
          qty = Math.Min(this.CursorItemCount, qty);
        if (qty <= 0)
          return;
        if (this.currentSlotID > 29)
          this.InfiniteBuy(qty);
        else
          this.ReturnToShop(qty);
      }
    }

    private void InfiniteBuy(int qty)
    {
      bool flag = false;
      if (this.CursorItem.MaxDurability == (ushort) 0 || qty == 1)
        flag = !this.IsPlayerInventory ? this.inventory.AddToInventory(this.CursorItem.ItemID, qty) > 0 : this.player.AddToInventory(this.CursorItem.ItemID, qty) > 0;
      if (flag)
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private void ReturnToShop(int qty)
    {
      bool flag = false;
      if (this.chest.Inventory.HasItem(this.CursorItem.ItemID))
      {
        this.CursorItemCount -= qty;
        flag = true;
      }
      if (flag)
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private void FiniteTrade(int qty)
    {
      if (this.isPlayerShop || this.currentSlotID < 30)
      {
        qty = Math.Min(this.CursorItemCount, qty);
        if (this.isPlayerShop && this.currentSlotID < 30)
        {
          ushort maxDurability = this.CursorItem.MaxDurability;
          if (maxDurability > (ushort) 0 && (int) this.CursorItem.Durability < (int) maxDurability)
          {
            this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Cannot stock a damaged item", "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
            return;
          }
        }
      }
      if (qty <= 0)
        return;
      int price = this.GetPrice();
      if (price >= 0)
      {
        if (this.currentSlotID > 29)
        {
          this.Buy(qty, price);
        }
        else
        {
          bool flag = this.isPlayerShop && this.chest.IsOwner(this.player);
          if (price > 0 || flag && this.CursorItem.ItemID == Item.GoldPieces)
            this.Sell(qty, price);
          else
            Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
        }
      }
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private void Buy(int qty, int price)
    {
      bool flag1 = false;
      if (this.CursorItem.MaxDurability == (ushort) 0 || qty == 1)
      {
        bool flag2 = this.isPlayerShop && this.chest.IsOwner(this.player);
        if (flag2 || (long) this.player.GoldCoinsOnPerson >= (long) price * (long) qty)
        {
          qty = !this.IsPlayerInventory ? this.inventory.AddToInventory(this.CursorItem.ItemID, qty) : this.player.AddToInventory(this.CursorItem.ItemID, qty);
          if (qty > 0)
          {
            if (this.isPlayerShop)
            {
              this.chest.Inventory.DecrementItem(this.CursorItem.ItemID, qty);
              if (!flag2)
                this.chest.Inventory.IncrementItem(Item.GoldPieces, price * qty);
            }
            if (!flag2)
            {
              this.inventory.DecrementItem(Item.GoldPieces, price * qty);
              if (this.IsPlayerInventory)
                this.player.Raise_ItemTraded(this.CursorItem.ItemID, qty, price * qty, false);
            }
            flag1 = true;
          }
        }
      }
      if (flag1)
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    private void Sell(int qty, int price)
    {
      bool flag1 = false;
      if (this.chest.Inventory.HasItem(this.CursorItem.ItemID))
      {
        bool flag2 = this.isPlayerShop && this.chest.IsOwner(this.player);
        while (qty > 0)
        {
          bool flag3 = !this.isPlayerShop || this.chest.Inventory.ItemCount(Item.GoldPieces) >= price;
          if (flag2 || flag3 && (this.IsPlayerInventory ? this.player.AddToInventory(Item.GoldPieces, price) : this.inventory.AddToInventory(Item.GoldPieces, price)) > 0)
          {
            if (this.isPlayerShop)
            {
              if (!flag2)
                this.chest.Inventory.DecrementItem(Item.GoldPieces, price);
              this.chest.Inventory.IncrementItem(this.CursorItem.ItemID, 1);
            }
            --qty;
            --this.CursorItemCount;
            flag1 = true;
            if (!flag2 && this.IsPlayerInventory)
              this.player.Raise_ItemTraded(this.CursorItem.ItemID, 1, price, true);
          }
          else
            break;
        }
      }
      if (flag1)
        Sounds.PlaySound(Item.GoldPieces, ItemSoundType.Use);
      else
        Sounds.PlaySound(ItemSoundGroup.GuiInvalid);
    }

    protected override void CheckForClear()
    {
    }

    protected override void DrawCoWindow()
    {
      int height = this.pagesize / 10;
      int num1 = 171;
      this.DrawGrid(height, num1 + 36, this.chest.Inventory, 30, this.page * this.pagesize, true, this.isPlayerShop);
      int num2 = 6;
      this.spriteBatch.DrawString(this.Font, this.shopTitle, new Vector2((float) (this.screenRect.X + 14), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (this.PageCount > 1)
      {
        this.spriteBatch.DrawString(this.Font, "Page " + this.pageString, new Vector2((float) (this.screenRect.X + this.screenRect.Width - 160), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.spriteBatch.DrawString(this.Font, "Flip", new Vector2((float) (this.screenRect.X + this.screenRect.Width - 50), (float) (this.screenRect.Y + num2)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        this.triggerRect.X = this.screenRect.X + this.screenRect.Width - 70;
        this.triggerRect.Y = this.screenRect.Y + num2 - 1;
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.PrevTab, this.triggerRect);
      }
      bool flag1 = this.IsFiniteMode && (!this.IsCreativeMode || this.player == null || !this.player.IsAdmin);
      bool flag2 = this.isPlayerShop && this.chest.IsOwner(this.player);
      bool flag3 = this.CursorItem.Count > 0 && (this.CursorItem.ItemID != Item.GoldPieces || flag2 || !flag1 && !this.isPlayerShop);
      string str = this.currentSlotID > 29 ? (flag2 ? "Take" : (flag1 || this.isPlayerShop ? "Buy" : "Take")) : (flag2 ? "Stock" : (flag1 || this.isPlayerShop ? "Sell" : "Return"));
      Rectangle screenRect = this.screenRect;
      screenRect.X += 15;
      screenRect.Y = this.screenRect.Y + this.screenRect.Height - num1 - 46;
      screenRect.Width = 25;
      screenRect.Height = 25;
      if (flag3)
      {
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.SelectItem, screenRect);
        this.spriteBatch.DrawString(this.Font, str + " 1", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      if (flag3 && this.CursorItem.MaxDurability == (ushort) 0)
      {
        screenRect.X += 146;
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.LiftItemSingle, screenRect);
        this.spriteBatch.DrawString(this.Font, str + " 100", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        screenRect.X -= 146;
      }
      screenRect.X += 324;
      bool flag4 = this.chest.IsOwner(this.player) && this.currentSlotID > 30;
      if (flag4)
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.TransferItem, screenRect);
      else
        GraphicStatics.DrawInputIcon(this.spriteBatch, GuiInput.ExitScreen, screenRect);
      this.spriteBatch.DrawString(this.Font, flag4 ? "Set Price" : "Exit Shop", new Vector2((float) (screenRect.X + 35), (float) (screenRect.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override Rectangle GetSlotRect(int slotID)
    {
      return base.GetSlotRect(slotID);
    }

    protected override float GetDrawItemColorAlpha(int slotID, InventoryItem item)
    {
      ChestBlock chest = this.chest;
      if (slotID <= 29 || this.IsItemForSale(item.ItemID_Raw) && (!this.isPlayerShop || item.Count != 0 || item.ItemID_Raw == Item.GoldPieces))
        return base.GetDrawItemColorAlpha(slotID, item);
      return !this.isPlayerShop || item.Count != 0 ? 0.6f : 0.2f;
    }

    protected override void DrawItemExtra(Rectangle slotRect, int slotID, InventoryItem item)
    {
      if (slotID <= 29 || this.IsItemForSale(item.ItemID_Raw))
        return;
      slotRect.X += 15;
      slotRect.Y += 14;
      slotRect.Width = slotRect.Height = 20;
      this.spriteBatchPoint.Draw(this.checkboxOff, slotRect, Color.White);
    }

    protected override bool ShowDurabilityBar(int slotID, InventoryItem item)
    {
      if (slotID >= 30)
        return false;
      return base.ShowDurabilityBar(slotID, item);
    }

    protected override string ItemDescriptionPanelText
    {
      get
      {
        if (!this.isPlayerShop && (!this.IsFiniteMode || this.IsCreativeMode && this.player != null && this.player.IsAdmin))
          return base.ItemDescriptionPanelText;
        if (this.CursorItemLocked)
          return "Locked Item: Not Tradeable.";
        if (this.currentSlotID < 29 && this.CursorItemCount == 0)
          return "";
        int price = this.GetPrice();
        bool flag = this.currentSlotID > 29;
        string str = flag ? "Buy" : "Sell";
        if (price < 0)
          return string.Format("{0}: Not Tradeable.", (object) base.ItemDescriptionPanelText);
        if (!this.chest.Inventory.HasItem(this.CursorItem.ItemID_Raw))
          return string.Format("{0}: Not Tradeable at this shop.", (object) base.ItemDescriptionPanelText);
        if (price != 0)
        {
          if (!flag || this.CursorItemCount != 0)
            return string.Format("{0}: {1} gold ea to {2}.", (object) base.ItemDescriptionPanelText, (object) price, (object) str);
          return string.Format("{0}: No Stock.", (object) base.ItemDescriptionPanelText);
        }
        if (!flag)
          return string.Format("{0}: Not sellable.", (object) base.ItemDescriptionPanelText);
        return string.Format("{0}: Free.", (object) base.ItemDescriptionPanelText);
      }
    }

    private int GetPrice()
    {
      if (this.isPlayerShop && this.chest.IsOwner(this.player) && this.CursorItem.ItemID == Item.GoldPieces)
        return 0;
      int itemIdRaw = (int) this.CursorItem.ItemID_Raw;
      ShopBlock chest = this.chest as ShopBlock;
      Player player = chest != null ? this.instance.GetPlayer(chest.Gamertag) : (Player) null;
      PriceList priceList = !this.isPlayerShop || chest == null || chest.PriceList == null ? player?.DefaultPriceList : chest.PriceList;
      int num;
      if (this.currentSlotID > 29)
      {
        num = priceList != null ? (priceList.Prices[itemIdRaw].ForSale ? priceList.Prices[itemIdRaw].Sell : -1) : ItemData.GetMinCustBuyPrice(this.CursorItem.ItemID_Raw);
      }
      else
      {
        num = priceList != null ? (priceList.Prices[itemIdRaw].ForSale ? priceList.Prices[itemIdRaw].FinalBuy : -1) : ItemData.GetMinCustSellPrice(this.CursorItem.ItemID_Raw);
        if (this.CursorItem.MaxDurability > (ushort) 0 && this.CursorItem.ItemID != Item.Book)
          num = (int) ((double) num * ((double) this.CursorItem.Durability / (double) this.CursorItem.MaxDurability));
      }
      return num;
    }

    protected override bool ShouldDrawQuantity(int slotID)
    {
      if (slotID >= 30 && !this.isPlayerShop)
        return this.IsFiniteMode;
      return true;
    }
  }
}
