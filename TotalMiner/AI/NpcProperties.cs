// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.NpcProperties
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using System.IO;

namespace StudioForge.TotalMiner.AI
{
  public class NpcProperties : IPropertyEditorControl
  {
    public int? Reach;
    public bool? ShowSwingTarget;
    public bool? ShowNamePlate;
    public bool? CanPickup;
    public bool? CanFight;
    public bool? CanBeHealedByOther;
    public bool? EquipBody;
    public bool? DrawEquipedItems;
    public bool? DropInventoryOnDeath;
    public bool? DropRandomLootOnDeath;
    public StudioForge.TotalMiner.AI.MoveType? MoveType;
    public byte? MoveTypePercent;
    public float? GravityMultiplier;
    public float? LandingDamageMultiplier;

    string IPropertyEditorControl.ToString(string propertyName, object data)
    {
      if (data != null)
        return data.ToString();
      return "Default";
    }

    bool IPropertyEditorControl.IsPropertyEnabled(string name)
    {
      return true;
    }

    void IPropertyEditorControl.SetPropertyDefaults()
    {
    }

    void IPropertyEditorControl.SetPropertyEditorDefaults(
      string name,
      Window win)
    {
    }

    object IPropertyEditorControl.Validate(
      string propertyName,
      string input,
      out string adjustedInput)
    {
      if (propertyName == "Reach" && input.IsEmpty())
        input = "Default";
      adjustedInput = input;
      return (object) null;
    }

    public void SetFrom(NpcProperties other)
    {
      if (other.Reach.HasValue)
        this.Reach = other.Reach;
      if (other.ShowSwingTarget.HasValue)
        this.ShowSwingTarget = other.ShowSwingTarget;
      if (other.ShowNamePlate.HasValue)
        this.ShowNamePlate = other.ShowNamePlate;
      if (other.CanPickup.HasValue)
        this.CanPickup = other.CanPickup;
      if (other.CanFight.HasValue)
        this.CanFight = other.CanFight;
      if (other.CanBeHealedByOther.HasValue)
        this.CanBeHealedByOther = other.CanBeHealedByOther;
      if (other.EquipBody.HasValue)
        this.EquipBody = other.EquipBody;
      if (other.DrawEquipedItems.HasValue)
        this.DrawEquipedItems = other.DrawEquipedItems;
      if (other.DropInventoryOnDeath.HasValue)
        this.DropInventoryOnDeath = other.DropInventoryOnDeath;
      if (other.DropRandomLootOnDeath.HasValue)
        this.DropRandomLootOnDeath = other.DropRandomLootOnDeath;
      if (other.MoveType.HasValue)
        this.MoveType = other.MoveType;
      if (other.MoveTypePercent.HasValue)
        this.MoveTypePercent = other.MoveTypePercent;
      if (other.GravityMultiplier.HasValue)
        this.GravityMultiplier = other.GravityMultiplier;
      if (!other.LandingDamageMultiplier.HasValue)
        return;
      this.LandingDamageMultiplier = other.LandingDamageMultiplier;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.Reach = new int?();
      this.ShowSwingTarget = new bool?();
      this.ShowNamePlate = new bool?();
      this.CanPickup = new bool?();
      this.CanFight = new bool?();
      this.CanBeHealedByOther = new bool?();
      this.EquipBody = new bool?();
      this.DrawEquipedItems = new bool?();
      this.DropInventoryOnDeath = new bool?();
      this.DropRandomLootOnDeath = new bool?();
      this.MoveType = new StudioForge.TotalMiner.AI.MoveType?();
      this.MoveTypePercent = new byte?();
      this.GravityMultiplier = new float?();
      this.LandingDamageMultiplier = new float?();
      if (version > 268)
      {
        if (reader.ReadBoolean())
          this.Reach = new int?(reader.ReadInt32());
        if (reader.ReadBoolean())
          this.ShowSwingTarget = new bool?(reader.ReadBoolean());
        if (reader.ReadBoolean())
          this.ShowNamePlate = new bool?(reader.ReadBoolean());
        if (reader.ReadBoolean())
          this.CanPickup = new bool?(reader.ReadBoolean());
        if (reader.ReadBoolean())
          this.CanFight = new bool?(reader.ReadBoolean());
        if (version < 284 && reader.ReadBoolean())
          reader.ReadBoolean();
        if (version > 279 && reader.ReadBoolean())
          this.CanBeHealedByOther = new bool?(reader.ReadBoolean());
        if (reader.ReadBoolean())
          this.EquipBody = new bool?(reader.ReadBoolean());
        if (version > 289 && reader.ReadBoolean())
          this.DrawEquipedItems = new bool?(reader.ReadBoolean());
        if (version > 273 && reader.ReadBoolean())
          this.DropInventoryOnDeath = new bool?(reader.ReadBoolean());
        if (version > 288 && reader.ReadBoolean())
          this.DropRandomLootOnDeath = new bool?(reader.ReadBoolean());
        if (version > 278 && reader.ReadBoolean())
          this.MoveType = new StudioForge.TotalMiner.AI.MoveType?((StudioForge.TotalMiner.AI.MoveType) reader.ReadByte());
        if (version > 278 && reader.ReadBoolean())
          this.MoveTypePercent = new byte?(reader.ReadByte());
        if (version > 280 && reader.ReadBoolean())
          this.GravityMultiplier = new float?(reader.ReadSingle());
        if (version <= 280 || !reader.ReadBoolean())
          return;
        this.LandingDamageMultiplier = new float?(reader.ReadSingle());
      }
      else
        reader.ReadBytes(12);
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.Reach.HasValue);
      if (this.Reach.HasValue)
        writer.Write(this.Reach.Value);
      writer.Write(this.ShowSwingTarget.HasValue);
      if (this.ShowSwingTarget.HasValue)
        writer.Write(this.ShowSwingTarget.Value);
      writer.Write(this.ShowNamePlate.HasValue);
      if (this.ShowNamePlate.HasValue)
        writer.Write(this.ShowNamePlate.Value);
      writer.Write(this.CanPickup.HasValue);
      if (this.CanPickup.HasValue)
        writer.Write(this.CanPickup.Value);
      writer.Write(this.CanFight.HasValue);
      if (this.CanFight.HasValue)
        writer.Write(this.CanFight.Value);
      writer.Write(this.CanBeHealedByOther.HasValue);
      if (this.CanBeHealedByOther.HasValue)
        writer.Write(this.CanBeHealedByOther.Value);
      writer.Write(this.EquipBody.HasValue);
      if (this.EquipBody.HasValue)
        writer.Write(this.EquipBody.Value);
      writer.Write(this.DrawEquipedItems.HasValue);
      if (this.DrawEquipedItems.HasValue)
        writer.Write(this.DrawEquipedItems.Value);
      writer.Write(this.DropInventoryOnDeath.HasValue);
      if (this.DropInventoryOnDeath.HasValue)
        writer.Write(this.DropInventoryOnDeath.Value);
      writer.Write(this.DropRandomLootOnDeath.HasValue);
      if (this.DropRandomLootOnDeath.HasValue)
        writer.Write(this.DropRandomLootOnDeath.Value);
      writer.Write(this.MoveType.HasValue);
      if (this.MoveType.HasValue)
        writer.Write((byte) this.MoveType.Value);
      writer.Write(this.MoveTypePercent.HasValue);
      if (this.MoveTypePercent.HasValue)
        writer.Write(this.MoveTypePercent.Value);
      writer.Write(this.GravityMultiplier.HasValue);
      if (this.GravityMultiplier.HasValue)
        writer.Write(this.GravityMultiplier.Value);
      writer.Write(this.LandingDamageMultiplier.HasValue);
      if (!this.LandingDamageMultiplier.HasValue)
        return;
      writer.Write(this.LandingDamageMultiplier.Value);
    }
  }
}
