// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CombatStats
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using System.IO;

namespace StudioForge.TotalMiner
{
  public struct CombatStats
  {
    public int HealthLevel;
    public int AttackLevel;
    public int StrengthLevel;
    public int DefenceLevel;
    public int RangedLevel;

    public void MergeNotZero(CombatStats stats)
    {
      if (stats.HealthLevel > 0)
        this.HealthLevel = stats.HealthLevel;
      if (stats.AttackLevel > 0)
        this.AttackLevel = stats.AttackLevel;
      if (stats.StrengthLevel > 0)
        this.StrengthLevel = stats.StrengthLevel;
      if (stats.DefenceLevel > 0)
        this.DefenceLevel = stats.DefenceLevel;
      if (stats.RangedLevel <= 0)
        return;
      this.RangedLevel = stats.RangedLevel;
    }

    public void SetFromXML(ActorLevelDataXML data)
    {
      this.HealthLevel = data.HealthLevel;
      this.AttackLevel = data.AttackLevel;
      this.StrengthLevel = data.StrengthLevel;
      this.DefenceLevel = data.DefenceLevel;
      this.RangedLevel = data.RangedLevel;
    }

    public bool IsEqual(ActorLevelDataXML data)
    {
      if (this.HealthLevel == data.HealthLevel && this.AttackLevel == data.AttackLevel && (this.StrengthLevel == data.StrengthLevel && this.DefenceLevel == data.DefenceLevel))
        return this.RangedLevel == data.RangedLevel;
      return false;
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.HealthLevel = (int) reader.ReadUInt16();
      this.StrengthLevel = (int) reader.ReadUInt16();
      this.AttackLevel = (int) reader.ReadUInt16();
      this.DefenceLevel = (int) reader.ReadUInt16();
      this.RangedLevel = (int) reader.ReadUInt16();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write((ushort) this.HealthLevel);
      writer.Write((ushort) this.StrengthLevel);
      writer.Write((ushort) this.AttackLevel);
      writer.Write((ushort) this.DefenceLevel);
      writer.Write((ushort) this.RangedLevel);
    }
  }
}
