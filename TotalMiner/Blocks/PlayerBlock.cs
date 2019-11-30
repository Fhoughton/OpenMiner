// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Blocks.PlayerBlock
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.TotalMiner.Storage;
using System.IO;

namespace StudioForge.TotalMiner.Blocks
{
  internal abstract class PlayerBlock : DataBlock
  {
    public string Gamertag;

    public bool IsOwner(Player player)
    {
      if (player != null && this.Gamertag != null && this.Gamertag.Length > 0)
        return player.Gamertag == this.Gamertag;
      return false;
    }

    public Player GetPlayer(GameInstance instance)
    {
      return instance?.GetPlayer(this.Gamertag);
    }

    public bool HasPlayer
    {
      get
      {
        if (this.Gamertag != null)
          return this.Gamertag.Length > 0;
        return false;
      }
    }

    public PlayerBlock()
    {
    }

    public PlayerBlock(GlobalPoint3D p)
      : base(p)
    {
    }

    public PlayerBlock(GlobalPoint3D p, Player player)
      : base(p)
    {
      if (player == null)
        return;
      this.Gamertag = player.Gamertag;
    }

    public override void CopyFrom(DataBlock from)
    {
      base.CopyFrom(from);
      this.Gamertag = (from as PlayerBlock).Gamertag;
      if (!(this.Gamertag == ""))
        return;
      this.Gamertag = (string) null;
    }

    public void LoadFromSaveData(SavePlayerBlockState state)
    {
      this.Point = state.Point;
      this.Gamertag = state.Gamertag;
      if (!(this.Gamertag == ""))
        return;
      this.Gamertag = (string) null;
    }

    protected override void ReadStateCore(BinaryReader reader, int version)
    {
      base.ReadStateCore(reader, version);
      this.Gamertag = reader.ReadString();
      if (!(this.Gamertag == ""))
        return;
      this.Gamertag = (string) null;
    }

    protected override void WriteStateCore(BinaryWriter writer)
    {
      base.WriteStateCore(writer);
      writer.Write(this.Gamertag == null ? "" : this.Gamertag);
    }
  }
}
