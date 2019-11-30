// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPluginArcade
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.API
{
  public interface ITMPluginArcade
  {
    /// <summary>
    /// Called by the game when a player swings at an arcade block face.
    /// </summary>
    /// <returns>An instance of a type that subclasses ArcadeMachine to provide an Arcade game.</returns>
    ArcadeMachine GetArcadeMachine(
      int gameID,
      ITMGame game,
      ITMMap map,
      ITMPlayer player,
      GlobalPoint3D p,
      BlockFace face);

    /// <summary>
    /// Called by the game when a player swings at an arcade block face.
    /// </summary>
    /// <returns>An instance of a type that implementes IArcadeMachineRenderer.</returns>
    IArcadeMachineRenderer GetArcadeMachineRenderer(int gameID);

    /// <summary>
    /// Called by the game. Return the name of the Arcade game.
    /// </summary>
    /// <returns>The name of the Arcade game.</returns>
    string GetArcadeMachineName(int gameID);
  }
}
