// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.MetaExecuteFactory
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Net
{
  internal static class MetaExecuteFactory
  {
    public static MetaExecuteBase Create(MetaExecuteType type)
    {
      if (type == MetaExecuteType.Script)
        return (MetaExecuteBase) new MetaExecuteScript();
      return (MetaExecuteBase) null;
    }
  }
}
