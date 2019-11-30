// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Storage.SaveZoneState
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.Storage
{
  internal class SaveZoneState
  {
    public float SpeedMultiplier = 1f;
    public float GravityMultiplier = 1f;
    public string Name;
    public ZoneType Type;
    public GlobalPoint3D Min;
    public GlobalPoint3D Max;
    public string Builder;
    public ZoneBuilderType BuilderType;
    public string OnEntryScript;
    public string OnExitScript;
    public short CombatLevelDifference;
  }
}
