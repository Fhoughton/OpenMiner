// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.MetaExecuteBase
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner.Net
{
  internal abstract class MetaExecuteBase
  {
    public abstract MetaExecuteType Type { get; }

    public abstract void ReadState(BinaryReader reader);

    public abstract void WriteState(BinaryWriter writer);
  }
}
