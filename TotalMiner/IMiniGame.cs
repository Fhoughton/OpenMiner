// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.IMiniGame
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;

namespace StudioForge.TotalMiner
{
  internal interface IMiniGame
  {
    float Elapsed { get; }

    float EndTime { get; }

    Player Leader { get; }

    MiniGameType GameType { get; }

    bool IsEatingAllowed { get; }

    void Start(GameInstance instance, Player startedBy);

    void End();

    void Abort();

    void Update();

    void UpdateTimerFromHost(float elapsed);

    void RegisterKill(Player killer, Player killed);

    void ReadPacket(PacketReader reader, byte dataType);

    void EquipOnDeath(Player player);

    void RespawnOnDeath(Player player);
  }
}
