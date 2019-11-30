// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPlugin
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner.API
{
  public interface ITMPlugin
  {
    /// <summary>
    /// Initialize the Mod. This method is called by the game when the mod is first activated/loaded.
    /// </summary>
    /// <param name="mgr">ITMPluginManager object.</param>
    /// <param name="path">The path of your Mods folder on disk relative to FileSystem.RootPath. Store and use this path if you require to read/write files from your Mods folder.</param>
    void Initialize(ITMPluginManager mgr, string path);

    /// <summary>
    /// This method is called by the game when the player has loaded a world.
    /// </summary>
    /// <param name="game">ITMGame object.</param>
    void InitializeGame(ITMGame game);

    /// <summary>Called if the mod is unloaded.</summary>
    void UnloadMod();

    /// <summary>
    /// This method is called by the game once every frame for every local player. Use this method to process user input.
    /// </summary>
    /// <param name="player"></param>
    /// <returns>True if an input was processed.</returns>
    bool HandleInput(ITMPlayer player);

    /// <summary>
    /// This method is called by the game once every frame. Use this method to add frame based logic that is not dependant on a particular player.
    /// </summary>
    void Update();

    /// <summary>
    /// This method is called by the game once every frame for every local and remote player in the game. Use this method to add player dependant frame based logic.
    /// </summary>
    /// <param name="player"></param>
    void Update(ITMPlayer player);

    /// <summary>
    /// This method is called by the game once every frame, after all the main rendering has been done. Use this method to add your own rendering.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="virtualPlayer"></param>
    void Draw(ITMPlayer player, ITMPlayer virtualPlayer);

    /// <summary>
    /// This method is called by the game after the world has been saved to disk.
    /// </summary>
    /// <param name="version">The version number of the save file.</param>
    void WorldSaved(int version);

    /// <summary>
    /// This method is called once for each player that joins the game.
    /// </summary>
    /// <param name="player"></param>
    void PlayerJoined(ITMPlayer player);

    /// <summary>
    /// This method is called once for each player that leaves the game.
    /// </summary>
    /// <param name="player"></param>
    void PlayerLeft(ITMPlayer player);
  }
}
