// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPluginManager
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner.API
{
  public interface ITMPluginManager
  {
    EnumTypeOffsets Offsets { get; }

    void RegisterEnumCounts(EnumTypeOffsets offsets);
  }
}
