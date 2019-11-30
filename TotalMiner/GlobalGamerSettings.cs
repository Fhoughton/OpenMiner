// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.GlobalGamerSettings
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner
{
  internal class GlobalGamerSettings
  {
    public int Version = 294;
    public PlayerSettings PlayerSettings = new PlayerSettings();
    public GameSettings GameSettings = new GameSettings();
    public bool GlobalOverwrite;

    public void ReadState(BinaryReader reader, int version)
    {
      this.Version = version;
      if (version <= 153)
        return;
      this.GlobalOverwrite = reader.ReadBoolean();
      this.PlayerSettings.ReadState(reader, version);
      this.GameSettings.ReadState(reader, version);
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.GlobalOverwrite);
      this.PlayerSettings.WriteState(writer);
      this.GameSettings.WriteState(writer);
    }
  }
}
