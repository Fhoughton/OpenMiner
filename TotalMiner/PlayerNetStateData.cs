// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PlayerNetStateData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.TotalMiner.Net;

namespace StudioForge.TotalMiner
{
  internal struct PlayerNetStateData
  {
    public byte StateByte;
    public Vector3 Position;
    public Vector3 ViewDirection;
    public float SizeY;
    public float Health;
    public byte HotBarLeftID;
    public byte HotBarRightID;
    public PlayerStateDataToSend StateToSend;
  }
}
