// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.INPCExternalPacketStrategy
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner.Net
{
  internal interface INPCExternalPacketStrategy
  {
    void ReceivePacket(GameInstance instance, PacketReader reader);
  }
}
