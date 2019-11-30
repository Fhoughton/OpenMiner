// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ServerEntry
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using System.IO;

namespace StudioForge.TotalMiner
{
  internal struct ServerEntry
  {
    public int ID;
    public byte MyRating;
    public bool IsFavourite;
    public string MapName;
    public string Desc;

    public void ReadState(BinaryReader reader, int version)
    {
      this.ID = reader.ReadInt32();
      this.MyRating = reader.ReadByte();
      this.IsFavourite = reader.ReadBoolean();
      this.MapName = reader.ReadString();
      this.Desc = reader.ReadString();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.ID);
      writer.Write(this.MyRating);
      writer.Write(this.IsFavourite);
      writer.Write(this.MapName == null ? "" : this.MapName);
      writer.Write(this.Desc == null ? "" : this.Desc);
    }
  }
}
