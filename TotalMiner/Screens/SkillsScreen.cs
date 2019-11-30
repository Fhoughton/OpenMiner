// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.SkillsScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class SkillsScreen : BlockMenuScreen
  {
    private bool unloadHighscores = true;
    private Player playerToView;
    private string gamertag;
    private int frameCount;
    private bool isServer;
    private CharacterSkillsData skillsData;
    private Thread loadHighScoreThread;

    private bool CanUpdateHighscores
    {
      get
      {
        if (this.player == null && this.frameCount > 5 && (Globals2.GamertagData.HighScoreData != null && this.loadHighScoreThread == null))
          return !NetworkManager.Instance.IsSessionOpen;
        return false;
      }
    }

    public bool IsServer
    {
      get
      {
        return this.isServer;
      }
    }

    public string Gamertag
    {
      get
      {
        return this.gamertag;
      }
    }

    public SkillsScreen(Player player, Player playerToView)
      : base("Skills List", player)
    {
      this.playerToView = playerToView;
      this.gamertag = playerToView.Gamertag;
      this.skillsData = playerToView.SkillsData;
      this.Initialize();
    }

    public SkillsScreen(CharacterSkillsData skillsData, string gamertag)
      : this(skillsData, gamertag, false)
    {
    }

    public SkillsScreen(CharacterSkillsData skillsData, string gamertag, bool isServer)
      : base("Skills List", (Player) null)
    {
      this.skillsData = skillsData;
      this.gamertag = gamertag;
      this.isServer = isServer;
      this.Initialize();
    }

    public void ChangeGamer(string gamertag)
    {
      if (!(this.gamertag != gamertag))
        return;
      this.gamertag = gamertag;
      this.skillsData = new CharacterSkillsData(Globals2.GamertagData.HighScoreData.HighScores[gamertag]);
      this.Initialize();
    }

    private void Initialize()
    {
      this.MenuEntries.Clear();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      if (this.gamertag != null)
      {
        blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, this.gamertag));
        blockMenuEntryList[0].Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectAnotherPlayer);
        if (this.isServer)
        {
          blockMenuEntryList[0].SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnRemovePlayerFromHighscores);
          blockMenuEntryList[0].ButtonTextX = "Ban Player";
          if (Globals2.GamertagData.HighScoreData.IsGamertagBanned(this.gamertag))
          {
            blockMenuEntryList[0].ColorOverride = Color.Red;
            blockMenuEntryList[0].OverrideColor = true;
          }
        }
      }
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "  Skill         Level                 XP    Next Level        Rank               Rank"));
      SkillMenuEntry skillMenuEntry1 = this.playerToView == null ? new SkillMenuEntry(this, this.skillsData, 0, "Combat", Globals2.GamertagData.HighScoreData) : new SkillMenuEntry(this, this.playerToView, 0, "Combat", Globals2.GamertagData.HighScoreData);
      skillMenuEntry1.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectedViewRank);
      blockMenuEntryList.Add((BlockMenuEntry) skillMenuEntry1);
      for (int skillDataIndex = 1; skillDataIndex < this.skillsData.SkillCount; ++skillDataIndex)
      {
        SkillMenuEntry skillMenuEntry2 = this.playerToView == null ? new SkillMenuEntry(this, this.skillsData, skillDataIndex, Globals2.GamertagData.HighScoreData) : new SkillMenuEntry(this, this.playerToView, skillDataIndex, Globals2.GamertagData.HighScoreData);
        skillMenuEntry2.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectedViewRank);
        if (this.player != null && this.player.IsGodOrTester)
        {
          skillMenuEntry2.SelectXButton += new EventHandler<PlayerIndexEventArgs>(this.OnEditLevel);
          skillMenuEntry2.SelectYButton += new EventHandler<PlayerIndexEventArgs>(this.OnEditXP);
        }
        blockMenuEntryList.Add((BlockMenuEntry) skillMenuEntry2);
      }
      SkillMenuEntry skillMenuEntry3 = (SkillMenuEntry) new TotalPlayerSkillMenuEntry(this, this.skillsData, Globals2.GamertagData.HighScoreData);
      skillMenuEntry3.Selected += new EventHandler<PlayerIndexEventArgs>(this.OnSelectedViewRank);
      blockMenuEntryList.Add((BlockMenuEntry) skillMenuEntry3);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 816;
      this.ItemHeight = 26;
      this.ItemGapY = 2;
      this.ItemTextScale = 0.55f;
      this.ItemsPerPage = this.MenuEntries.Count;
      this.DrawItemLines = this.DrawEntryLines = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (!this.unloadHighscores || this.isServer)
        return;
      if (this.loadHighScoreThread != null && this.loadHighScoreThread.IsAlive)
        this.loadHighScoreThread.Abort();
      if (!Globals2.GamertagData.IsHighScoresLoaded)
        return;
      Globals2.GamertagData.HighScoreData.Unload();
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (!input.IsNewButtonPress(Buttons.Y) || !this.CanUpdateHighscores || this.isServer)
        return base.HandleInput(input);
      this.UpdateHighscores();
      return true;
    }

    private void OnSelectAnotherPlayer(object sender, PlayerIndexEventArgs e)
    {
      if (this.player == null || this.player.GameInstance == null || this.player.GameInstance.NetworkManager.AllGamerCount <= 1)
        return;
      this.ScreenManager.AddScreen((GameScreen) new GamerListScreen(this.player, new Action<NetworkGamer, bool, string>(this.NewPlayerSelected), true, (string) null, false, false), this.ControllingPlayer);
    }

    private void NewPlayerSelected(NetworkGamer gamer, bool allGamers, string text)
    {
      Player tag = gamer.Tag as Player;
      if (tag == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new SkillsScreen(this.player, tag), this.ControllingPlayer);
      this.unloadHighscores = false;
      this.ExitScreen();
    }

    private void OnRemovePlayerFromHighscores(object sender, PlayerIndexEventArgs e)
    {
      MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Update " + this.gamertag + " on highscores?", "Add", "Remove", (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground));
      messageBoxScreen.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnAddPlayerConfirm);
      messageBoxScreen.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.OnRemovePlayerConfirm);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreen, this.ControllingPlayer);
    }

    private void OnAddPlayerConfirm(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GamertagData.HighScoreData.AddGamertagToHighscoresBanList(this.gamertag, false);
      Globals2.GamertagData.SaveGamertagData(true, false);
      this.MenuEntries[0].OverrideColor = false;
    }

    private void OnRemovePlayerConfirm(object sender, PlayerIndexEventArgs e)
    {
      Globals2.GamertagData.HighScoreData.AddGamertagToHighscoresBanList(this.gamertag, true);
      Globals2.GamertagData.SaveGamertagData(true, false);
      this.MenuEntries[0].OverrideColor = true;
    }

    private void OnSelectedViewRank(object sender, PlayerIndexEventArgs e)
    {
      SkillMenuEntry skillMenuEntry = sender as SkillMenuEntry;
      if (skillMenuEntry == null || !Globals2.GamertagData.IsHighScoresLoaded)
        return;
      if (!this.isServer && Globals2.GamertagData.HighScoreData.IsGamertagBanned(this.gamertag))
        this.PlayerRemovedFromHighscoreTableForCheatingMessageBox();
      else if (skillMenuEntry.TotalLevel >= 100 || this.isServer)
        this.ScreenManager.AddScreen((GameScreen) new SkillRankScreen(this, (SkillType) skillMenuEntry.SkillDataIndex, this.gamertag, skillMenuEntry is TotalPlayerSkillMenuEntry, skillMenuEntry.SkillDataIndex == 0), this.ControllingPlayer);
      else
        TotalMinerGame.Instance.ShowInvalidChoiceScreen("You must have a Total Level of 100 or higher\nbefore you can view the Ranking screen", this.ControllingPlayer.Value);
    }

    private void PlayerRemovedFromHighscoreTableForCheatingMessageBox()
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreen("This gamertag [" + this.gamertag + "] has been removed from global ranks for cheating.\n\nVisit our forums at www.totalminerforums.net for more information.", "Ok", (string) null, (string) null, (string) null, this.Font, 0.5f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground)), this.ControllingPlayer);
    }

    private void OnEditLevel(object sender, PlayerIndexEventArgs e)
    {
      SkillMenuEntry skillMenuEntry = sender as SkillMenuEntry;
      if (skillMenuEntry == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnLevelEntered), this.playerToView.SkillsData[skillMenuEntry.SkillDataIndex].Level, false), this.ControllingPlayer);
    }

    private void OnEditXP(object sender, PlayerIndexEventArgs e)
    {
      SkillMenuEntry skillMenuEntry = sender as SkillMenuEntry;
      if (skillMenuEntry == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new NumberEntryScreen(this.player, new NumberEntered(this.OnXPEntered), (int) this.playerToView.SkillsData[skillMenuEntry.SkillDataIndex].CurrentXP, false), this.ControllingPlayer);
    }

    private void OnLevelEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      SkillMenuEntry menuEntry = this.MenuEntries[this.selectedEntry] as SkillMenuEntry;
      if (menuEntry == null)
        return;
      SkillData skillData = this.playerToView.SkillsData[menuEntry.SkillDataIndex];
      skillData.SetCurrentXPRaw((double) (SkillData.GetXP((int) number) + 1L));
      this.playerToView.SkillsData[menuEntry.SkillDataIndex] = skillData;
      Globals2.GamertagData.SkipSkillMergeInternal = true;
      this.playerToView.OnSkillLevelled(skillData);
    }

    private void OnXPEntered(double number, bool isCancelled, object state)
    {
      if (isCancelled)
        return;
      SkillMenuEntry menuEntry = this.MenuEntries[this.selectedEntry] as SkillMenuEntry;
      if (menuEntry == null)
        return;
      SkillData skillData = this.playerToView.SkillsData[menuEntry.SkillDataIndex];
      skillData.SetCurrentXPRaw(number);
      this.playerToView.SkillsData[menuEntry.SkillDataIndex] = skillData;
      this.playerToView.OnSkillLevelled(skillData);
    }

    private void UpdateHighscores()
    {
    }

    private void OnHighscoresUpdated(bool success)
    {
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (++this.frameCount != 5 || this.loadHighScoreThread != null || this.isServer)
        return;
      this.LoadHighscoreList();
    }

    private void LoadHighscoreList()
    {
      this.loadHighScoreThread = new Thread(new ThreadStart(this.ThreadedHighScoreLoad));
      this.loadHighScoreThread.CurrentCulture = Globals1.CultureInfo;
      this.loadHighScoreThread.CurrentUICulture = Globals1.CultureInfo;
      this.loadHighScoreThread.Start();
    }

    private void ThreadedHighScoreLoad()
    {
      try
      {
        if (this.player == null || this.player.GameInstance != null && !this.player.GameInstance.IsLocalSkills)
          Globals2.GamertagData.LoadHighScoreDataGlobal();
        this.LoadRanks();
      }
      catch (Exception ex)
      {
      }
      this.loadHighScoreThread = (Thread) null;
    }

    private void LoadRanks()
    {
      bool isBanned = Globals2.GamertagData.HighScoreData.IsGamertagBanned(this.gamertag);
      for (int index = 2; index < this.MenuEntries.Count; ++index)
        (this.MenuEntries[index] as SkillMenuEntry)?.SetHighScores(Globals2.GamertagData.HighScoreData, isBanned);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 245, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      if (this.CanUpdateHighscores)
      {
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
        this.SpriteBatch.DrawString(this.Font, "Update Highscores", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
        destinationRectangle.X -= 300;
      }
      if (this.isServer && this.selectedEntry == 0)
      {
        this.SpriteBatch.Draw(CoreGlobals.ButtonTextureX, destinationRectangle, Color.White);
        this.SpriteBatch.DrawString(this.Font, "Remove Player", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      }
      if (this.selectedEntry <= 1)
        return;
      destinationRectangle.X = this.MenuRect.X + 48;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureA, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "View Rank", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 284, this.MenuRect.Y, 1, this.MenuRect.Height - this.ButtonBarHeight);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.White);
      destinationRectangle.X = this.MenuRect.X;
      destinationRectangle.Y = this.MenuRect.Y + this.ItemHeight + this.ItemGapY;
      destinationRectangle.Width = this.MenuRect.Width - 284;
      destinationRectangle.Height = 1;
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, destinationRectangle, Color.White);
      destinationRectangle.Y += (this.MenuEntries.Count - 2) * (this.ItemHeight + this.ItemGapY) + 3;
      destinationRectangle.Width = this.MenuRect.Width;
      this.SpriteBatch.DrawString(this.ItemFont, "Local              Global", new Vector2((float) (this.MenuRect.X + this.MenuRect.Width - 246), (float) (this.MenuRect.Y + 2)), this.MenuEntries[0].ColorSelected, 0.0f, Vector2.Zero, this.ItemTextScale, SpriteEffects.None, 0.0f);
    }
  }
}
