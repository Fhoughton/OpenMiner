// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.IArcadeMachineRenderer
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Integration;

namespace StudioForge.TotalMiner
{
  public interface IArcadeMachineRenderer : IHasContent
  {
    void LoadTexturePack();

    void Draw(ArcadeMachine machine);
  }
}
