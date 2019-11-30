// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.AI.MessagePacket
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.AI
{
  internal abstract class MessagePacket
  {
    public MessageType Type;
    public INPCBehaviour Sender;
    public List<ActorType> Recipients;
    public List<ActorType> Exclude;
    public Vector3 Position;
    public ushort Distance;
    public float Duration;
  }
}
