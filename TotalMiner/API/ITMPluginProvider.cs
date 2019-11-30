// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPluginProvider
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner.API
{
  public interface ITMPluginProvider
  {
    /// <summary>
    /// Called by the game. Implemented by the mod. All mods must implement this method.
    /// </summary>
    /// <returns>An ITMPlugin implementation.</returns>
    ITMPlugin GetPlugin();

    /// <summary>
    /// Called by the game. Implemented by mods that provide custom GUI screens.
    /// </summary>
    /// <returns>An ITMPluginGUI implementation.</returns>
    ITMPluginGUI GetPluginGUI();

    /// <summary>
    /// Called by the game. Implemented by mods that customize blocks.
    /// </summary>
    /// <returns>An ITMPluginBlocks implementation.</returns>
    ITMPluginBlocks GetPluginBlocks();

    /// <summary>
    /// Called by the game. Implemented by mods that supply arcade block games.
    /// </summary>
    /// <returns>An ITMPluginArcade implementation.</returns>
    ITMPluginArcade GetPluginArcade();

    /// <summary>
    /// Called by the game. Implemented by mods that supply a networking server.
    /// </summary>
    /// <returns>An ITMPluginNet implementation.</returns>
    ITMPluginNet GetPluginNet();
  }
}
