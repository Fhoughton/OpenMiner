// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LobbyScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class LobbyScreen : BlockMenuScreen
  {
    private int createSessionTimeout = 300;
    private GameProperties gameProperties;
    private bool sessionInitializationStarted;
    private bool sessionInitialized;
    private bool sessionInitializationComplete;
    private bool isHost;
    private GameMode origMode;
    private GameDifficulty origDiff;
    private IAsyncResult mapSeedResult;
    private NetworkManager networkManager;
    private bool immediateStart;
    private int frameCount;
    private Thread initSessionThread;
    private int failedAttempts;

    public LobbyScreen(bool immediateStart)
      : base("Lobby", (Player) null)
    {
      TotalMinerGame.GameInstance = GameInstance.Instance = NetworkManager.Instance.GameInstance = (GameInstance) null;
      Sounds.Initialize((ITMGame) null);
      if (Globals2.GamertagData.IsHighScoresLoaded)
        Globals2.GamertagData.HighScoreData.Unload();
      this.immediateStart = immediateStart;
      this.networkManager = NetworkManager.Instance;
      this.gameProperties = Globals2.GameProperties;
      this.origMode = this.gameProperties.SaveGame.Header.GameMode;
      this.origDiff = this.gameProperties.SaveGame.Header.GameDifficulty;
      this.isHost = this.gameProperties.HostOrJoin == HostOrJoin.Host;
      if (this.gameProperties.IsNewMap && this.isHost)
      {
        this.gameProperties.SaveGame.Header.BiomeParams.Initialize(this.gameProperties.BiomeType, this.gameProperties.SaveGame.Header.MapSeed);
        this.ConvertGroundLevel();
        this.SetMapDimensions();
      }
      this.sessionInitializationStarted = this.sessionInitialized = !this.isHost;
      this.MenuEntries.Clear();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Network: " + NetworkManager.GetNetworkTypeDesc(this.gameProperties.NetworkSessionType)));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Map Name: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Map Seed: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Game Mode: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Attribute: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Difficulty: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Player Skills: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Terrain: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Texture Pack: "));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Active Mods: " + ModManager.ActiveMods.Count.ToString()));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, ""));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[2].Selected += new EventHandler<PlayerIndexEventArgs>(this.MapSeedSelectedEventHandler);
      blockMenuEntryList[3].Selected += new EventHandler<PlayerIndexEventArgs>(this.GameModeSelectedEventHandler);
      blockMenuEntryList[4].Selected += new EventHandler<PlayerIndexEventArgs>(this.MapAttributeSelectedEventHandler);
      blockMenuEntryList[5].Selected += new EventHandler<PlayerIndexEventArgs>(this.GameDifficultySelectedEventHandler);
      blockMenuEntryList[6].Selected += new EventHandler<PlayerIndexEventArgs>(this.PlayerSkillsSelectedEventHandler);
      blockMenuEntryList[7].Selected += new EventHandler<PlayerIndexEventArgs>(this.TerrainTypeSelectedEventHandler);
      blockMenuEntryList[8].Selected += new EventHandler<PlayerIndexEventArgs>(this.TexturePackSelectedEventHandler);
      blockMenuEntryList[9].Selected += new EventHandler<PlayerIndexEventArgs>(this.ModsSelectedEventHandler);
      blockMenuEntryList[10].Selected += new EventHandler<PlayerIndexEventArgs>(this.StartGameSelectedEventHandler);
      blockMenuEntryList[11].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.selectedEntry = 10;
      for (int index = 0; index <= this.selectedEntry; ++index)
        blockMenuEntryList[index].IsEnabled = false;
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 480;
      this.ItemsPerPage = 12;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      if (!this.sessionInitialized)
        return;
      this.InitBasic();
    }

    public override void UnloadContent()
    {
      this.UnhookSessionEvents();
      this.networkManager = (NetworkManager) null;
      base.UnloadContent();
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    private void HookSessionEvents()
    {
      if (this.sessionInitializationStarted && (!this.networkManager.IsSessionOpen || this.networkManager.Session.SessionState != NetworkSessionState.Lobby))
        return;
      this.networkManager.GameStarted += new EventHandler<GameEventArgs>(this.GameStartedEventHandler);
      if (this.isHost)
        return;
      this.networkManager.GamePropertiesReceived += new EventHandler<EventArgs>(this.GamePropertiesReceivedEventHandler);
      this.networkManager.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
    }

    private void UnhookSessionEvents()
    {
      if (this.networkManager == null)
        return;
      if (!this.isHost)
      {
        this.networkManager.GamerLeft -= new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
        this.networkManager.GamePropertiesReceived -= new EventHandler<EventArgs>(this.GamePropertiesReceivedEventHandler);
      }
      this.networkManager.GameStarted -= new EventHandler<GameEventArgs>(this.GameStartedEventHandler);
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      if (!e.Gamer.IsLocal)
        return;
      this.ExitLobbyScreen(sender, EventArgs.Empty);
    }

    private void ConvertGroundLevel()
    {
      if (!this.IsOldCreativeMap || this.gameProperties.SaveGame.Header.TerrainData.SeaLevel >= (ushort) 200)
        return;
      this.gameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 200;
    }

    private bool IsOldCreativeMap
    {
      get
      {
        SaveMapHead header = this.gameProperties.SaveGame.Header;
        int saveVersion = header.SaveVersion;
        if (header.GameMode == GameMode.Creative && saveVersion > 0)
          return saveVersion < 55;
        return false;
      }
    }

    private void ResetToggleItems()
    {
      this.ResetToggleItemsCore();
      if (this.networkManager == null || !this.networkManager.IsSessionOpen || !this.isHost)
        return;
      this.networkManager.SendGameProperties();
    }

    private void ResetToggleItemsCore()
    {
      this.MenuEntries[1].Text = "Map Name: " + this.gameProperties.SaveGame.Header.MapName;
      this.MenuEntries[2].Text = "Map Seed: " + (object) this.gameProperties.SaveGame.Header.MapSeed;
      this.MenuEntries[3].Text = "Game Mode: " + Utils.InsertSpacesBeforeCapitals(this.gameProperties.SaveGame.Header.GameMode.ToString());
      this.MenuEntries[4].Text = "Attribute: " + Utils.InsertSpacesBeforeCapitals(this.gameProperties.SaveGame.Header.Attribute.ToString());
      this.MenuEntries[5].Text = "Difficulty: " + this.gameProperties.SaveGame.Header.GameDifficulty.ToString();
      this.MenuEntries[6].Text = "Player Skills: " + (this.gameProperties.SaveGame.Header.SkillsEnabled ? (this.gameProperties.SaveGame.Header.SkillsLocal ? "Local" : "Global") : "Off");
      this.MenuEntries[7].Text = "Terrain: " + this.gameProperties.SaveGame.Header.TerrainData.Biome.ToString();
      int mapWidth = this.gameProperties.SaveGame.Header.MapWidth;
      string str1 = mapWidth.ToString();
      string str2 = str1 + " x " + str1;
      if (mapWidth >= 10000 && mapWidth < 1000000)
      {
        string str3 = str1.Substring(0, str1.Length - 3);
        string str4 = str3 + "k x " + str3 + "k";
      }
      string str5 = this.gameProperties.SaveGame.Header.TexturePack;
      int length = this.gameProperties.SaveGame.Header.TexturePack.IndexOf(" by ");
      if (length > 0)
        str5 = this.gameProperties.SaveGame.Header.TexturePack.Substring(0, length);
      this.MenuEntries[8].Text = "Texture Pack: " + str5;
      this.MenuEntries[9].Text = "Active Mods: " + ModManager.ActiveMods.Count.ToString();
      this.SetEnabled();
    }

    private void SetEnabled()
    {
      if (this.networkManager != null && this.networkManager.IsSessionOpen && (this.gameProperties != null && this.gameProperties.SaveGame != null) && this.gameProperties.SaveGame.Header != null)
      {
        bool isNewMap = this.gameProperties.IsNewMap;
        int sessionType = (int) this.networkManager.Session.SessionType;
        if (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.AvatarDesigner)
        {
          this.MenuEntries[2].IsEnabled = isNewMap && this.isHost;
          this.MenuEntries[3].IsEnabled = this.isHost && !isNewMap && this.origMode != GameMode.Creative;
          this.MenuEntries[4].IsEnabled = this.isHost;
          this.MenuEntries[5].IsEnabled = this.isHost && this.gameProperties.SaveGame.Header.GameMode != GameMode.Peaceful && (isNewMap || this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Legendary);
          this.MenuEntries[6].IsEnabled = this.isHost && (this.gameProperties.SaveGame.Header.SaveVersion < 100 || this.origMode == GameMode.Creative || this.gameProperties.IsNewMap);
          this.MenuEntries[7].IsEnabled = this.isHost && this.gameProperties.BiomeType != BiomeType.Infinite;
          this.MenuEntries[8].IsEnabled = this.isHost;
          this.MenuEntries[9].IsEnabled = this.isHost;
        }
        this.MenuEntries[10].IsEnabled = true;
      }
      else
      {
        for (int index = 0; index < this.MenuEntries.Count; ++index)
          this.MenuEntries[index].IsEnabled = false;
      }
    }

    private void ResetSessionProperties()
    {
      int num = this.networkManager.IsSessionOpen ? 1 : 0;
    }

    public override bool HandleInput(InputState input)
    {
      return base.HandleInput(input);
    }

    private void GroundBlockSelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new BlockSelectionScreen((GameInstance) null, (Player) null, new SelectItemCallBack(this.GroundBlockCallback), "Select the Ground block", BlockSelectMode.SelectingGround, Block.None, 0), new PlayerIndex?(e.PlayerIndex));
    }

    private bool GroundBlockCallback(Player player, Item itemID, int notUsed, object tagData)
    {
      if (itemID == Item.None)
        return false;
      this.gameProperties.SaveGame.Header.TerrainData.GroundBlock = itemID;
      this.SetMapDimensions();
      this.ResetToggleItems();
      return true;
    }

    public override void OnCancel(PlayerIndex playerIndex)
    {
      if (!this.sessionInitializationComplete)
        return;
      this.ExitLobbyScreen((object) null, EventArgs.Empty);
    }

    private void ExitLobbyScreen(object sender, EventArgs e)
    {
      this.UnhookSessionEvents();
      this.networkManager.EndSession();
      this.ExitScreen();
      Globals2.AutoStartMap = 0;
    }

    private void MapSeedSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.mapSeedResult = Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Enter the Map Seed (numbers only)", "The map seed is used to generate the world.", (string) null, new AsyncCallback(this.OnMapSeedEntered), (object) null, this.MenuEntries[this.selectedEntry], true);
    }

    private void GameModeSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      if (this.gameProperties.SaveGame.Header.GameMode == this.origMode)
      {
        this.gameProperties.SaveGame.Header.GameMode = GameMode.Creative;
      }
      else
      {
        this.gameProperties.SaveGame.Header.GameMode = this.origMode;
        this.gameProperties.SaveGame.Header.GameDifficulty = this.origDiff;
      }
      this.ResetToggleItems();
      this.ResetSessionProperties();
    }

    private void MapAttributeSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.gameProperties.SaveGame.Header.Attribute = this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Adventure ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Exploration ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Survival ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.RPG ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Construction ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Challenge ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Skilling ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Arena ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Deathmatch ? (this.gameProperties.SaveGame.Header.Attribute != MapAttribute.Component ? MapAttribute.Adventure : MapAttribute.WorkInProgress) : MapAttribute.Component) : MapAttribute.Deathmatch) : MapAttribute.Arena) : MapAttribute.Skilling) : MapAttribute.Challenge) : MapAttribute.Construction) : MapAttribute.RPG) : MapAttribute.Survival) : MapAttribute.Exploration;
      this.ResetToggleItems();
      this.ResetSessionProperties();
    }

    private void GameDifficultySelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      if (this.gameProperties.SaveGame.Header.GameMode == GameMode.Creative)
        this.gameProperties.SaveGame.Header.GameDifficulty = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Peaceful ? (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Easy ? GameDifficulty.Peaceful : GameDifficulty.Normal) : GameDifficulty.Easy;
      else if (this.gameProperties.SaveGame.Header.GameMode == GameMode.Survival)
      {
        if (this.gameProperties.IsNewMap)
          this.gameProperties.SaveGame.Header.GameDifficulty = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Easy ? (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Normal ? GameDifficulty.Easy : GameDifficulty.Legendary) : GameDifficulty.Normal;
        else if (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Legendary)
          this.gameProperties.SaveGame.Header.GameDifficulty = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Easy ? GameDifficulty.Easy : GameDifficulty.Normal;
      }
      else if (this.gameProperties.IsNewMap)
        this.gameProperties.SaveGame.Header.GameDifficulty = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Peaceful ? (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Easy ? (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Normal ? GameDifficulty.Peaceful : GameDifficulty.Legendary) : GameDifficulty.Normal) : GameDifficulty.Easy;
      else if (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Legendary)
        this.gameProperties.SaveGame.Header.GameDifficulty = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Peaceful ? (this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Easy ? GameDifficulty.Peaceful : GameDifficulty.Normal) : GameDifficulty.Easy;
      this.gameProperties.SaveGame.Header.CombatEnabled = this.gameProperties.SaveGame.Header.GameDifficulty != GameDifficulty.Peaceful;
      this.ResetToggleItems();
    }

    private void PlayerSkillsSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.gameProperties.SaveGame.Header.SkillsEnabled = !this.gameProperties.SaveGame.Header.SkillsEnabled;
      this.gameProperties.SaveGame.Header.SkillsLocal = true;
      this.ResetToggleItems();
      this.ResetSessionProperties();
    }

    private void PvPSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.gameProperties.SaveGame.Header.PvPCombat = !this.gameProperties.SaveGame.Header.PvPCombat;
      this.ResetToggleItems();
      this.ResetSessionProperties();
    }

    private void TerrainTypeSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new TerrainSetupScreen(this.gameProperties, new Action(this.ResetToggleItems)), this.ControllingPlayer);
    }

    private void TexturePackSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new TexturePackMenuScreen((GameInstance) null, (Player) null, new Action<string>(this.OnTexturePackSelected), true, false), this.ControllingPlayer);
    }

    private void ModsSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new ModListMenuScreen(new Action<bool>(this.OnModScreenExit)), this.ControllingPlayer);
    }

    private void SetMapDimensions()
    {
      SaveMapHead header = this.gameProperties.SaveGame.Header;
      int min = header.TerrainData.Biome == BiomeType.Flat ? 0 : 100;
      int max = header.MapHeight - 100 - header.MapHeight % 100;
      if (header.GameMode == GameMode.DigDeep || header.MapHeight > 512)
        min = max = (int) header.TerrainData.SeaLevel;
      switch (header.TerrainData.GroundBlock)
      {
        case Item.None:
        case Item.SkyWorld:
        case Item.SpaceWorld:
          min = max = 0;
          break;
        case Item.NaturalWorld:
          if (min < 50)
          {
            min = 50;
            break;
          }
          break;
      }
      header.TerrainData.SeaLevel = (ushort) MyMathHelper.Clamp((int) header.TerrainData.SeaLevel, min, max);
    }

    private void OnMapSeedEntered(IAsyncResult ar)
    {
      string s = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (s == null || s.Length <= 0)
        return;
      int result = 0;
      if (int.TryParse(s, out result))
      {
        if (Extensions.IsSpecialSeed(result))
        {
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This is a special seed\nSpecial seeds will be supported again in a future update", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        }
        else
        {
          this.gameProperties.SaveGame.Header.MapSeed = result;
          this.gameProperties.SaveGame.Header.BiomeParams.Initialize(this.gameProperties.BiomeType, result);
          this.ResetToggleItems();
        }
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("An invalid number was entered", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnTexturePackSelected(string texpack)
    {
      this.gameProperties.SaveGame.Header.TexturePack = texpack;
      this.ResetToggleItems();
      this.LoadTexturePack();
    }

    private void OnModScreenExit(bool changed)
    {
      this.ResetToggleItems();
      if (!changed)
        return;
      GraphicStatics.TexturePack.LoadTexturePack();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.frameCount++ < 3)
        return;
      if (!this.sessionInitializationStarted)
      {
        this.initSessionThread = new Thread(new ThreadStart(this.InitalizeSession));
        this.initSessionThread.CurrentCulture = Globals1.CultureInfo;
        this.initSessionThread.CurrentUICulture = Globals1.CultureInfo;
        this.initSessionThread.Start();
        this.sessionInitializationStarted = true;
      }
      else if (this.sessionInitialized && !this.sessionInitializationComplete)
      {
        this.MenuEntries[10].Text = !this.isHost ? (this.networkManager.Session.SessionState == NetworkSessionState.Playing ? "Join Game" : "Toggle Ready") : "Start Game";
        this.HookSessionEvents();
        if (this.isHost)
        {
          this.ResetSessionProperties();
          if (this.immediateStart)
            this.networkManager.StartGame();
        }
        this.sessionInitializationComplete = true;
      }
      if (this.networkManager == null || this.networkManager.IsSessionOpen || --this.createSessionTimeout >= 0)
        return;
      this.OnCancel(this.ControllingPlayer.Value);
    }

    private void InitBasic()
    {
      if (Globals2.GameProperties.SaveGame.Header.SkillsEnabled && !Globals2.GameProperties.SaveGame.Header.SkillsLocal)
        Globals2.GameProperties.SaveGame.Header.SkillsLocal = true;
      if (this.sessionInitialized)
      {
        this.networkManager.SetLocalGamersReady(true);
        if (!this.isHost)
          this.ResetToggleItemsCore();
      }
      if (Globals2.GameProperties.SaveGame.Header.Attribute != MapAttribute.AvatarDesigner)
      {
        GlobalGamerSettings globalGamerSettings = Globals2.GamertagData.GetGlobalGamerSettings(this.ControllingPlayer.Value);
        if ((globalGamerSettings.GlobalOverwrite || this.gameProperties.IsNewMap) && globalGamerSettings.GameSettings.TexturePack.IsNotEmpty())
          this.gameProperties.SaveGame.Header.TexturePack = globalGamerSettings.GameSettings.TexturePack;
        if (this.gameProperties.SaveGame.Header.ModNames != null && this.gameProperties.SaveGame.Header.ModNames.Count > 0)
        {
          ModManager.StartupActiveMods(this.gameProperties.SaveGame.Header.ModNames);
          GraphicStatics.LoadTexturePack((MapTM) null, GraphicStatics.TexturePack.Name, false, false);
        }
      }
      this.LoadTexturePack();
    }

    private void LoadTexturePack()
    {
    }

    private void LoadTexturePackThreaded()
    {
      GraphicStatics.LoadTexturePack((MapTM) null, this.gameProperties.SaveGame.Header.TexturePack, false, false);
      this.gameProperties.SaveGame.Header.TexturePack = GraphicStatics.TexturePack.Name;
    }

    private void InitalizeSession()
    {
      if (!this.gameProperties.IsNewMap)
      {
        try
        {
          this.gameProperties.SaveGame = this.gameProperties.SaveGame.DirNumber <= 0 ? Globals2.ParseGameFile(this.gameProperties.SaveGame.MapType, this.gameProperties.SaveGame.Filename, false) : Globals2.ParseGameFile(this.gameProperties.SaveGame.MapType, this.gameProperties.SaveGame.DirNumber, false, this.gameProperties.SaveGame.IsAutoSave);
        }
        catch (CorruptWorldFileException ex)
        {
          this.ExitScreen();
        }
      }
      this.InitBasic();
      this.CreateSession();
    }

    private void OnFileSizeCalced(int filesize)
    {
      if (NetworkManager.Instance == null)
        return;
      int num = NetworkManager.Instance.IsSessionOpen ? 1 : 0;
    }

    private void CreateSession()
    {
      SessionProperties properties = new SessionProperties()
      {
        SessionType = SessionType.Play,
        SessionState = NetworkSessionState.Lobby,
        ExeVersion = 27302,
        MapName = this.gameProperties.SaveGame.Header.MapName,
        OwnerName = this.gameProperties.SaveGame.Header.OwnerGamerTag,
        HostName = Globals2.GetSignedInGamer(this.ControllingPlayer).Gamertag,
        GameMode = this.gameProperties.SaveGame.Header.GameMode,
        Attribute = this.gameProperties.SaveGame.Header.Attribute,
        CurrentPlayerCount = 1,
        RatingAvgStars = this.gameProperties.SaveGame.Header.RatingStars,
        RatingsCount = this.gameProperties.SaveGame.Header.RatingCount,
        SkillsEnabled = this.gameProperties.SaveGame.Header.SkillsEnabled,
        SkillsLocal = this.gameProperties.SaveGame.Header.SkillsLocal,
        CombatEnabled = this.gameProperties.SaveGame.Header.CombatEnabled,
        DefaultPermission = Globals2.DefaultPermission,
        ModsEnabledCount = ModManager.ActiveMods.Count
      };
      if (this.networkManager == null)
        return;
      this.networkManager.CreateOnlinePlaySession(properties, new Action<bool>(this.OnSessionCreated), this.ControllingPlayer, Globals2.GameProperties.NetworkSessionType, "online", 4);
    }

    public void GamePropertiesReceivedEventHandler(object sender, EventArgs e)
    {
      this.ResetToggleItemsCore();
    }

    private void GameStartedEventHandler(object sender, GameEventArgs e)
    {
      if (Globals2.GameProperties == null)
        TotalMinerGame.Assert("Globals2.GameProperties null reference");
      else if (Globals2.GameProperties.SaveGame == null)
        TotalMinerGame.Assert("Globals2.GameProperties.SaveGame null reference");
      else if (Globals2.GameProperties.SaveGame.Header == null)
      {
        TotalMinerGame.Assert("Globals2.GameProperties.SaveGame.Header null reference");
      }
      else
      {
        Globals2.AutoStartMap = 0;
        this.ScreenManager.AddScreen((GameScreen) new LoadingScreen((Player) null), this.ControllingPlayer);
        this.ExitScreen();
      }
    }

    private void StartGameSelectedEventHandler(object sender, PlayerIndexEventArgs e)
    {
      if (this.gameProperties.SaveGame.Header.GameMode == GameMode.Creative && this.origMode != GameMode.Creative)
        this.ShowCreativeSwitchWarning();
      else if (this.gameProperties.SaveGame.Header.GameMode != GameMode.Creative && this.gameProperties.SaveGame.Header.SaveVersion < 100 && (this.gameProperties.SaveGame.Header.SaveVersion > 0 && !this.gameProperties.IsSystemMap) && this.networkManager.IsHost)
        this.ShowSkillOptionWarning();
      else
        this.StartGame();
    }

    private void StartGame()
    {
      if (this.networkManager == null || this.networkManager.Session == null)
        this.ExitScreen();
      else if (!this.isHost)
      {
        if (this.networkManager.Session.SessionState == NetworkSessionState.Lobby)
        {
          if (this.networkManager.LocalHost == null)
            return;
          this.networkManager.SetLocalGamersReady(!this.networkManager.LocalHost.IsReady);
        }
        else
          this.GameStartedEventHandler((object) this, new GameEventArgs());
      }
      else if (this.gameProperties != null && this.gameProperties.SaveGame != null && this.gameProperties.SaveGame.Header != null)
      {
        if (this.gameProperties.SaveGame.Header.GoodHash)
        {
          this.ValidateStartingData();
          this.networkManager.StartGame();
        }
        else
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This Game File has been tampered with\n\nTampered Game Files are not playable", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Invalid session, please restart the Lobby", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void ValidateStartingData()
    {
      if (this.gameProperties.SaveGame.Header.TerrainData.GroundBlock != Item.SpaceWorld && this.gameProperties.SaveGame.Header.TerrainData.GroundBlock != Item.SkyWorld)
        return;
      this.gameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 1;
    }

    private void OnSessionCreated(bool success)
    {
      if (!success)
      {
        if (++this.failedAttempts < 3)
        {
          this.MenuEntries[10].Text = "Creating Session. Please wait.";
          this.CreateSession();
        }
        else
        {
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Could not create session", "OK", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
          this.ExitScreen();
        }
      }
      else
      {
        int privateGamerCount = this.networkManager.PrivateGamerCount;
        if (this.gameProperties.SaveGame.Header.PrivateSlots < privateGamerCount)
          this.gameProperties.SaveGame.Header.PrivateSlots = privateGamerCount;
        this.ResetToggleItems();
        this.sessionInitialized = true;
      }
    }

    private void ShowCreativeSwitchWarning()
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("You have selected to convert this " + this.origMode.ToString() + " world to a\r\nCreative world. \r\n\r\nOnce accepted, this conversion cannot be reversed. \r\n\r\nIf you want to keep playing this world as a " + this.origMode.ToString() + " world, \r\nthen make a copy first, and convert the copy.", "Cancel", "Accept Conversion", (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.StartGameEventHandler);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void ShowSkillOptionWarning()
    {
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("This world was created before the 2.0 update. \r\nThe 2.0 update introduces the player skills system.\r\n\r\nFor Dig Deep, Survival and Peaceful worlds, once the world has been saved, \r\nthe player skills toggle cannot be changed. \r\n\r\nOnly save this world once you are sure you have made the choice you want \r\nregarding the player skills system for this world.", "Cancel", "Ok", (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.StartGameEventHandler);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void StartGameEventHandler(object sender, PlayerIndexEventArgs e)
    {
      this.StartGame();
    }

    protected override void DrawCore()
    {
      if (this.immediateStart)
        return;
      base.DrawCore();
    }

    protected override void DrawBottomBar()
    {
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - 30, 24, 24).Y - 8, this.MenuRect.Width, 1), Color.Gray);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
