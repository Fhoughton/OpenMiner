// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NewGameMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NewGameMenuScreen : BlockMenuScreen
  {
    public NewGameMenuScreen()
      : base("New World", (Player) null)
    {
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Dig Deep"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.DigDeepGameSelected);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Survival"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.SurvivalGameSelected);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Creative"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(this.CreativeGameSelected);
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[blockMenuEntryList.Count - 1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public string OwnerGamerTag()
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(this.ControllingPlayer.Value);
      if (signedInGamer != null)
        return signedInGamer.Gamertag;
      return "LocalGamer" + this.ControllingPlayer.Value.ToString();
    }

    public override void LoadContent()
    {
      Globals2.GameProperties.IsNewMap = true;
      Globals2.GameProperties.SaveGame.Header.MapName = "New";
      Globals2.GameProperties.SaveGame.Header.MapSeed = new PcgRandom((int) ((double) Services.TotalTime * 1000.0)).Next();
      Globals2.GameProperties.SaveGame.Header.OwnerGamerTag = this.OwnerGamerTag();
      Globals2.GameProperties.SaveGame.Header.DefaultPermission = Permissions.Adventure | Permissions.Map | Permissions.SystemShops | Permissions.TextChat;
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 191;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
      if (Globals2.AutoStartMap >= 0)
        return;
      this.CreativeGameSelected((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    private void TrialMapSelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.IsNewMap = false;
      string filename = Globals2.GetMapFilePath(MapType.System, Globals2.Contents.TrialMapDirNum) + "header.dat";
      Globals2.GameProperties.SaveGame = Globals2.ParseGameFile(MapType.System, filename, false);
      Globals2.GameProperties.SaveGame.DirNumber = Globals2.Contents.TrialMapDirNum;
      Globals2.GameProperties.BiomeType = Globals2.GameProperties.SaveGame.Header.TerrainData.Biome;
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void DigDeepGameSelected(object sender, PlayerIndexEventArgs e)
    {
      TerrainData terrainData = Globals2.GameProperties.SaveGame.Header.TerrainData;
      terrainData.Biome = BiomeType.DigDeep;
      terrainData.Iterations = 100;
      terrainData.MaxParticles = 100;
      terrainData.SeaLevel = (ushort) 2968;
      terrainData.GroundBlock = Item.Grass;
      Globals2.GameProperties.BiomeType = terrainData.Biome;
      Globals2.GameProperties.SaveGame.Header.GameMode = GameMode.DigDeep;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = StudioForge.TotalMiner.GameDifficulty.Normal;
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = true;
      Globals2.GameProperties.SaveGame.Header.PvPCombat = true;
      Globals2.GameProperties.SaveGame.Header.Attribute = MapAttribute.Exploration;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = true;
      Globals2.GameProperties.SaveGame.Header.PassiveMobs = true;
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = true;
      Globals2.GameProperties.SaveGame.Header.ResetMapBounds();
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void SurvivalGameSelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.SaveGame.Header.TerrainData.Biome = BiomeType.SemiAlphine;
      Globals2.GameProperties.SaveGame.Header.GameMode = GameMode.Survival;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = StudioForge.TotalMiner.GameDifficulty.Normal;
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = true;
      Globals2.GameProperties.SaveGame.Header.PvPCombat = true;
      Globals2.GameProperties.SaveGame.Header.Attribute = MapAttribute.Survival;
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = true;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = true;
      Globals2.GameProperties.SaveGame.Header.ResetMapBounds();
      Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock = Item.Grass;
      Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 200;
      this.ScreenManager.AddScreen((GameScreen) new NewGameBiomeSelectMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void PeacefulGameSelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.SaveGame.Header.TerrainData.Biome = BiomeType.SemiAlphine;
      Globals2.GameProperties.SaveGame.Header.GameMode = GameMode.Peaceful;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = StudioForge.TotalMiner.GameDifficulty.Peaceful;
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = false;
      Globals2.GameProperties.SaveGame.Header.PvPCombat = false;
      Globals2.GameProperties.SaveGame.Header.Attribute = MapAttribute.Adventure;
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = false;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = true;
      Globals2.GameProperties.SaveGame.Header.ResetMapBounds();
      Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock = Item.Grass;
      Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 200;
      this.ScreenManager.AddScreen((GameScreen) new NewGameBiomeSelectMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void CreativeGameSelected(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GameProperties.SaveGame.Header.GameMode = GameMode.Creative;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = StudioForge.TotalMiner.GameDifficulty.Peaceful;
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = false;
      Globals2.GameProperties.SaveGame.Header.Attribute = MapAttribute.WorkInProgress;
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = false;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = false;
      Globals2.GameProperties.SaveGame.Header.ResetMapBounds();
      Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 200;
      this.ScreenManager.AddScreen((GameScreen) new NewGameBiomeSelectMenuScreen(), this.ControllingPlayer);
      this.ExitScreen();
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
