// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.SentryTurretBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal class SentryTurretBlock : ChestBlock
  {
    public const int TurretRange = 30;
    public float Cooldown;
    public BlockTargetTypes TargetTypes;
    public bool RequiresPower;

    public override DataBlockType ClassType
    {
      get
      {
        return DataBlockType.SentryTurret;
      }
    }

    public bool IsActive
    {
      get
      {
        return this.TargetTypes != BlockTargetTypes.None;
      }
    }

    public bool IsTargeting(BlockTargetTypes target)
    {
      return (this.TargetTypes & target) == target;
    }

    public void ToggleTargetType(BlockTargetTypes target)
    {
      if (this.IsTargeting(target))
        this.TargetTypes &= ~target;
      else
        this.TargetTypes |= target;
    }

    public void ToggleTargetChoice()
    {
      if (this.IsTargeting(BlockTargetTypes.Strongest))
      {
        this.ToggleTargetType(BlockTargetTypes.Strongest);
        this.TargetTypes |= BlockTargetTypes.Weakest;
      }
      else if (this.IsTargeting(BlockTargetTypes.Weakest))
        this.TargetTypes &= ~BlockTargetTypes.Weakest;
      else
        this.TargetTypes |= BlockTargetTypes.Strongest;
    }

    public Item Weapon
    {
      get
      {
        if (this.Inventory.HasItem(Item.GrenadeLauncher) && this.Inventory.HasItem(Item.Grenade))
          return Item.GrenadeLauncher;
        if (!this.Inventory.HasItem(ItemSubType.Bow, false))
          return Item.None;
        if (this.Inventory.HasItem(Item.ElvenBow))
          return Item.ElvenBow;
        if (this.Inventory.HasItem(Item.TitaniumBow))
          return Item.TitaniumBow;
        if (this.Inventory.HasItem(Item.TrollBow))
          return Item.TrollBow;
        if (this.Inventory.HasItem(Item.SpiderBow))
          return Item.SpiderBow;
        if (this.Inventory.HasItem(Item.GoldenBow))
          return Item.GoldenBow;
        return this.Inventory.HasItem(Item.WoodBow) ? Item.WoodBow : Item.None;
      }
    }

    public InventoryItem Ammunition
    {
      get
      {
        Item weapon = this.Weapon;
        if (weapon == Item.GrenadeLauncher)
        {
          int index;
          if ((index = this.Inventory.FindItem(Item.Grenade)) >= 0)
            return this.Inventory[index];
        }
        else if (ItemData.IsSubType(weapon, ItemSubType.Bow) && this.Inventory.HasItem(ItemSubType.Arrow, false))
        {
          int index1;
          if ((index1 = this.Inventory.FindItem(Item.TitaniumArrow)) >= 0)
            return this.Inventory[index1];
          int index2;
          if ((index2 = this.Inventory.FindItem(Item.RubyArrow)) >= 0)
            return this.Inventory[index2];
          int index3;
          if ((index3 = this.Inventory.FindItem(Item.DiamondArrow)) >= 0)
            return this.Inventory[index3];
          int index4;
          if ((index4 = this.Inventory.FindItem(Item.BoomArrow)) >= 0)
            return this.Inventory[index4];
          int index5;
          if ((index5 = this.Inventory.FindItem(Item.FireArrow)) >= 0)
            return this.Inventory[index5];
          int index6;
          if ((index6 = this.Inventory.FindItem(Item.IceArrow)) >= 0)
            return this.Inventory[index6];
          int index7;
          if ((index7 = this.Inventory.FindItem(Item.SteelArrow)) >= 0)
            return this.Inventory[index7];
          int index8;
          if ((index8 = this.Inventory.FindItem(Item.IronArrow)) >= 0)
            return this.Inventory[index8];
          int index9;
          if ((index9 = this.Inventory.FindItem(Item.BronzeArrow)) >= 0)
            return this.Inventory[index9];
          int index10;
          if ((index10 = this.Inventory.FindItem(Item.FlintArrow)) >= 0)
            return this.Inventory[index10];
        }
        return Inventory.EmptyItem;
      }
    }

    private float GetCooldown()
    {
      ItemSwingTimeDataXML swingTimeDataXml = Globals1.ItemSwingTimeData[(int) this.Weapon];
      return (float) (((double) swingTimeDataXml.Time + (double) swingTimeDataXml.Pause) * 1.5);
    }

    private bool IsPowered(MapTM map)
    {
      return map.MapStrategyTM.IsBlockReceivingPower(this.Point);
    }

    public SentryTurretBlock()
    {
    }

    public SentryTurretBlock(GlobalPoint3D p)
      : this(p, (Player) null)
    {
    }

    public SentryTurretBlock(GlobalPoint3D p, Player player)
      : base(p, 10, player)
    {
      this.TargetTypes = BlockTargetTypes.Players | BlockTargetTypes.Mobs;
    }

    public void Update(MapTM map, float elapsed)
    {
      GameInstance instance = map.Instance;
      if (instance == null || !instance.IsCombatEnabled || !this.IsActive || this.RequiresPower && !this.IsPowered(map))
        return;
      this.Cooldown -= elapsed;
      if ((double) this.Cooldown > 0.0)
        return;
      this.FireWeapon(instance);
      this.Cooldown = this.GetCooldown();
    }

    private void FireWeapon(GameInstance instance)
    {
      InventoryItem ammunition = this.Ammunition;
      if (ammunition.ItemID == Item.None)
        return;
      Vector3 blockCenter = instance.Map.GetBlockCenter(this.Point);
      Player player = instance.GetPlayer(this.Gamertag);
      Actor closestCharacter = instance.GetClosestCharacter(blockCenter, player, 4, 30f, this.TargetTypes);
      if (closestCharacter == null)
        return;
      Vector3 eyePosition = closestCharacter.EyePosition;
      float num = (eyePosition - blockCenter).Length();
      if ((double) num >= 30.0)
        return;
      Vector3 vector3 = eyePosition + closestCharacter.VisualVelocity * num * 2f - blockCenter;
      vector3.Normalize();
      Vector3 position = blockCenter + vector3 * instance.Map.TileSize;
      if (!instance.Map.IsPassable(instance.Map.GetPoint(position)))
        return;
      Vector3 velocity = vector3 * 30f;
      velocity.Y -= (float) ((double) num * (double) GameInstance.Gravity * 12.0);
      HitTest hitTest = instance.CalcBlockTarget(position, Vector3.Normalize(velocity), 15f);
      if (hitTest.IsValid && (double) hitTest.Distance <= (double) num)
        return;
      if (instance.IsFiniteResources)
        this.OnWeaponUsed();
      instance.AddProjectile(ammunition.ItemID, position, velocity, GamerID.Sys1, false, true);
    }

    private void OnWeaponUsed()
    {
      Item weapon = this.Weapon;
      if (ItemData.GetItemDurability(weapon) > (ushort) 0)
      {
        int slotID = this.Inventory.FindItem(weapon);
        if (slotID >= 0 && this.Inventory.DecrementItemDurability(slotID, (ushort) 1) == (ushort) 0)
          this.Inventory.DecrementItem(slotID);
      }
      if (weapon == Item.ElvenBow)
        return;
      this.Inventory.DecrementItem(this.Ammunition.ItemID, 1);
    }

    public override void BlockClosed()
    {
      base.BlockClosed();
      if ((double) this.Cooldown <= 1000.0)
        return;
      this.Cooldown = 0.0f;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      SentryTurretBlock sentryTurretBlock = from as SentryTurretBlock;
      this.TargetTypes = sentryTurretBlock.TargetTypes;
      this.Cooldown = sentryTurretBlock.Cooldown;
      this.RequiresPower = sentryTurretBlock.RequiresPower;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Cooldown = reader.ReadSingle();
      this.TargetTypes = (BlockTargetTypes) reader.ReadByte();
      this.RequiresPower = reader.ReadBoolean();
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Cooldown);
      writer.Write((byte) this.TargetTypes);
      writer.Write(this.RequiresPower);
    }

    public void LoadFromSaveData(SaveSentryTurretState state)
    {
      this.LoadFromSaveData((SaveChestState) state);
      this.Cooldown = state.Cooldown;
      this.TargetTypes = state.TargetTypes;
      this.RequiresPower = state.RequiresPower;
    }
  }
}
