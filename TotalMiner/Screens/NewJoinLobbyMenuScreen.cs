// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.NewJoinLobbyMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class NewJoinLobbyMenuScreen : BlockMenuScreen
  {
    private object findSessionsLock = new object();
    public static NetworkSessionProperties Filter;
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private bool refreshSessions;
    private bool findSessionsBegun;
    private bool drawCalledAtleastOnce;
    private bool sessionsLoaded;
    private bool screenUnloaded;
    private int column;
    private bool ascending;
    private GameScreen joiningProgressScreen;
    private List<IAvailableNetworkSession> sessions;

    private bool CanSort
    {
      get
      {
        return this.MenuEntries.Count > 3;
      }
    }

    public NewJoinLobbyMenuScreen()
      : base("Join Lobby", (Player) null)
    {
      if (NewJoinLobbyMenuScreen.Filter == null)
        NewJoinLobbyMenuScreen.Filter = new NetworkSessionProperties();
      this.MenuEntries.Clear();
      List<BlockMenuEntry> blockMenuEntryList = new List<BlockMenuEntry>();
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Finding Sessions..."));
      blockMenuEntryList.Add(new BlockMenuEntry((BlockMenuScreen) this, "Back"));
      blockMenuEntryList[1].Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      this.MenuEntries.AddRange((IEnumerable<MenuEntry>) blockMenuEntryList.ToArray());
      this.column = 1;
      this.ascending = false;
    }

    public override void LoadContent()
    {
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 959;
      this.ItemHeight = 40;
      this.ItemGapY = 8;
      this.ItemTextScale = 0.7f;
      this.ItemsPerPage = 10;
      this.DrawEntryLines = true;
      this.drawCalledAtleastOnce = false;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 42;
      }
    }

    private void RefreshSessions()
    {
      this.refreshSessions = true;
    }

    public override void UnloadContent()
    {
      lock (this.findSessionsLock)
      {
        this.screenUnloaded = true;
        base.UnloadContent();
      }
    }

    public override bool HandleInput(InputState input)
    {
      if (!this.ControllingPlayer.HasValue)
        return false;
      GamePadState currentGamePadState = input.CurrentGamePadStates[(int) this.ControllingPlayer.Value];
      GamePadState lastGamePadState = input.LastGamePadStates[(int) this.ControllingPlayer.Value];
      if (this.sessionsLoaded)
      {
        if (this.refreshSessions || currentGamePadState.Buttons.Y == ButtonState.Pressed && lastGamePadState.Buttons.Y == ButtonState.Released)
        {
          this.ScreenManager.AddScreen((GameScreen) new NewJoinLobbyMenuScreen(), this.ControllingPlayer);
          this.ExitScreen();
          return true;
        }
        if (currentGamePadState.Buttons.X == ButtonState.Pressed && lastGamePadState.Buttons.X == ButtonState.Released)
        {
          this.ScreenManager.AddScreen((GameScreen) new JoinLobbyFilterMenuScreen(new Action(this.RefreshSessions)), this.ControllingPlayer);
          return true;
        }
        if (this.CanSort)
        {
          if (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && lastGamePadState.Buttons.LeftShoulder == ButtonState.Released)
          {
            if (--this.column == 0)
              this.column = 6;
            this.SortItems();
            return true;
          }
          if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed && lastGamePadState.Buttons.RightShoulder == ButtonState.Released)
          {
            if (++this.column == 7)
              this.column = 1;
            this.SortItems();
            return true;
          }
          if (currentGamePadState.Buttons.Start == ButtonState.Pressed && lastGamePadState.Buttons.Start == ButtonState.Released)
          {
            this.ascending = !this.ascending;
            this.SortItems();
            return true;
          }
        }
      }
      return base.HandleInput(input);
    }

    private SessionProperties GetSessProperties(IAvailableNetworkSession session)
    {
      return session.SessionProperties as SessionProperties;
    }

    private void JoinSessionEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      SessionMenuEntry sessionMenuEntry = sender as SessionMenuEntry;
      if (sessionMenuEntry == null)
        return;
      if (this.GetSessProperties(sessionMenuEntry.Session).ExeVersion != 27302)
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreen("Error: Your game version is different from the session host\n", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground)), this.ControllingPlayer);
      else if (this.GetSessProperties(sessionMenuEntry.Session).SessionType == SessionType.Play)
      {
        string str = NetworkManager.Instance.JoinOnlineSession(sessionMenuEntry.Session, Globals2.LocalGamer, new Action<string>(this.OnSessionJoined));
        if (str == null)
        {
          this.joiningProgressScreen = (GameScreen) new SpinProgressScreen("Connecting to online session.\n\nPlease wait...", 0.7f);
          this.ScreenManager.AddScreen(this.joiningProgressScreen, this.ControllingPlayer);
        }
        else
        {
          this.refreshSessions = true;
          this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreen("Error: " + str, "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground)), this.ControllingPlayer);
        }
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreen("Error: This is not a play session.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground)), this.ControllingPlayer);
    }

    private void OnSessionJoined(string result)
    {
      if (this.joiningProgressScreen != null)
      {
        this.joiningProgressScreen.ExitScreen(true);
        this.joiningProgressScreen = (GameScreen) null;
      }
      if (result != null)
        TotalMinerGame.Instance.ShowExceptionMessageBox(result, this.ControllingPlayer);
      else if (NetworkManager.Instance.IsSessionOpen)
      {
        NetworkManager.Instance.GamePropertiesReceived += new EventHandler<EventArgs>(this.GamePropertiesReceived);
        NetworkManager.Instance.SendGamePropertiesRequest();
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Could not join session.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void GamePropertiesReceived(object sender, EventArgs e)
    {
      if (this.joiningProgressScreen != null)
      {
        this.joiningProgressScreen.ExitScreen(true);
        this.joiningProgressScreen = (GameScreen) null;
      }
      NetworkManager.Instance.GamePropertiesReceived -= new EventHandler<EventArgs>(this.GamePropertiesReceived);
      this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), this.ControllingPlayer);
      this.ExitScreen();
    }

    private void FindSessions()
    {
      this.findSessionsBegun = true;
      List<IAvailableNetworkSession> availableNetworkSessionList = (List<IAvailableNetworkSession>) null;
      if (ModManager.NetMod != null)
        availableNetworkSessionList = ModManager.NetMod.NetworkManager.FindSessions(new SessionMatching()
        {
          ExeVersion = 27302
        });
      lock (this.findSessionsLock)
      {
        if (!this.screenUnloaded)
        {
          if (availableNetworkSessionList != null)
          {
            foreach (IAvailableNetworkSession session in availableNetworkSessionList)
              this.AddNewSessionMenuItem(session);
          }
          if (this.MenuEntries.Count == 2)
          {
            this.MenuEntries[0].Text = "No sessions found";
            this.MenuEntries[0].ColorOverride = Color.Orange;
          }
          else if (this.MenuEntries.Count > 2)
            this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortSessions));
        }
        this.sessionsLoaded = true;
      }
    }

    private void AddNewSessionMenuItem(IAvailableNetworkSession session)
    {
      if (Globals2.KickedBy.Contains(((SessionProperties) session.SessionProperties).HostName))
        return;
      SessionMenuEntry sessionMenuEntry = new SessionMenuEntry((BlockMenuScreen) this, session);
      sessionMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.JoinSessionEntrySelected);
      sessionMenuEntry.LoadContent();
      if (this.MenuEntries[0].Text == "Finding Sessions...")
      {
        this.MenuEntries[0].Text = "Gamertag            Rating        Mode     Type        Plyrs  Skills  Filesize";
        this.selectedEntry = 1;
      }
      this.MenuEntries.Insert(this.MenuEntries.Count - 1, (MenuEntry) sessionMenuEntry);
      this.ItemsPerPage = Math.Min(10, this.MenuEntries.Count);
      this.ResetMenuRect();
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (this.otherScreenHasFocus || coveredByOtherScreen || !this.ShouldTryToFindSessions)
        return;
      this.FindSessions();
    }

    private bool ShouldTryToFindSessions
    {
      get
      {
        if (this.sessions == null && !this.findSessionsBegun)
          return this.drawCalledAtleastOnce;
        return false;
      }
    }

    private void SortItems()
    {
      if (!this.CanSort)
        return;
      switch (this.column)
      {
        case 1:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortByRating));
          break;
        case 2:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortByMode));
          break;
        case 3:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortByType));
          break;
        case 4:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortByPlayers));
          break;
        case 5:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortBySkills));
          break;
        case 6:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortByFilesize));
          break;
        default:
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortSessions));
          break;
      }
      this.MenuEntries.Insert(0, this.MenuEntries[this.MenuEntries.Count - 1]);
      this.MenuEntries.RemoveAt(this.MenuEntries.Count - 1);
    }

    private int SortSessions(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && e1.Text == "Back")
        return 1;
      if (sessionMenuEntry2 == null && e2.Text == "Back")
        return -1;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e2.Text.CompareTo(e1.Text);
      if (sessionMenuEntry1 == null)
        return -1;
      if (sessionMenuEntry2 == null)
        return 1;
      if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
        return -1;
      if (sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite)
        return 1;
      return sessionMenuEntry2.Rating.CompareTo(sessionMenuEntry1.Rating);
    }

    private int SortByRating(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if ((double) sessionMenuEntry1.Rating == (double) sessionMenuEntry2.Rating)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        if (sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite)
          return 1;
      }
      if (!this.ascending)
        return sessionMenuEntry2.Rating.CompareTo(sessionMenuEntry1.Rating);
      return sessionMenuEntry1.Rating.CompareTo(sessionMenuEntry2.Rating);
    }

    private int SortByMode(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if (sessionMenuEntry1.GameModeText == sessionMenuEntry2.GameModeText)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        return sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite ? 1 : 0;
      }
      if (!this.ascending)
        return sessionMenuEntry2.GameModeText.CompareTo(sessionMenuEntry1.GameModeText);
      return sessionMenuEntry1.GameModeText.CompareTo(sessionMenuEntry2.GameModeText);
    }

    private int SortByType(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if (sessionMenuEntry1.AttributeText == sessionMenuEntry2.AttributeText)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        return sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite ? 1 : 0;
      }
      if (!this.ascending)
        return sessionMenuEntry2.AttributeText.CompareTo(sessionMenuEntry1.AttributeText);
      return sessionMenuEntry1.AttributeText.CompareTo(sessionMenuEntry2.AttributeText);
    }

    private int SortByPlayers(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if (sessionMenuEntry1.PlayerCount == sessionMenuEntry2.PlayerCount)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        return sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite ? 1 : 0;
      }
      if (!this.ascending)
        return sessionMenuEntry2.PlayerCount.CompareTo(sessionMenuEntry1.PlayerCount);
      return sessionMenuEntry1.PlayerCount.CompareTo(sessionMenuEntry2.PlayerCount);
    }

    private int SortBySkills(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if (sessionMenuEntry1.SkillsEnabled == sessionMenuEntry2.SkillsEnabled)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        return sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite ? 1 : 0;
      }
      if (!this.ascending)
        return sessionMenuEntry2.SkillsEnabled.CompareTo(sessionMenuEntry1.SkillsEnabled);
      return sessionMenuEntry1.SkillsEnabled.CompareTo(sessionMenuEntry2.SkillsEnabled);
    }

    private int SortByFilesize(MenuEntry e1, MenuEntry e2)
    {
      SessionMenuEntry sessionMenuEntry1 = e1 as SessionMenuEntry;
      SessionMenuEntry sessionMenuEntry2 = e2 as SessionMenuEntry;
      if (sessionMenuEntry1 == null && sessionMenuEntry2 == null)
        return e1.Text.CompareTo(e2.Text);
      if (sessionMenuEntry1 == null)
        return 1;
      if (sessionMenuEntry2 == null)
        return -1;
      if (sessionMenuEntry1.FileSize == sessionMenuEntry2.FileSize)
      {
        if (sessionMenuEntry1.ServerEntry.IsFavourite && !sessionMenuEntry2.ServerEntry.IsFavourite)
          return -1;
        return sessionMenuEntry2.ServerEntry.IsFavourite && !sessionMenuEntry1.ServerEntry.IsFavourite ? 1 : 0;
      }
      if (!this.ascending)
        return sessionMenuEntry2.FileSize.CompareTo(sessionMenuEntry1.FileSize);
      return sessionMenuEntry1.FileSize.CompareTo(sessionMenuEntry2.FileSize);
    }

    private bool IsFindFilterActive
    {
      get
      {
        if (!NewJoinLobbyMenuScreen.Filter[1].HasValue && !NewJoinLobbyMenuScreen.Filter[2].HasValue)
          return NewJoinLobbyMenuScreen.Filter[5].HasValue;
        return true;
      }
    }

    protected override void DrawBackground()
    {
      base.DrawBackground();
      this.drawCalledAtleastOnce = true;
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 64;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - 80;
      this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, Color.White);
    }

    protected override void DrawMenuExtra()
    {
      base.DrawMenuExtra();
      if (!this.sessionsLoaded || this.itemAtTopOfPage != 0 || !this.CanSort)
        return;
      Rectangle columnHighlightRect = this.GetColumnHighlightRect();
      this.ScreenManager.SpriteBatch.Draw(CoreGlobals.BlankTexture, columnHighlightRect, new Color(160, 160, 160, (int) byte.MaxValue));
      ++columnHighlightRect.Y;
      columnHighlightRect.X += 2;
      columnHighlightRect.Width -= 4;
      this.ScreenManager.SpriteBatch.Draw(CoreGlobals.BlankTexture, columnHighlightRect, new Color(100, 100, 100, (int) byte.MaxValue));
    }

    private Rectangle GetColumnHighlightRect()
    {
      switch (this.column)
      {
        case 2:
          return new Rectangle(this.MenuRect.X + 428, this.MenuRect.Y + 40, 65, 1);
        case 3:
          return new Rectangle(this.MenuRect.X + 545, this.MenuRect.Y + 40, 61, 1);
        case 4:
          return new Rectangle(this.MenuRect.X + 688, this.MenuRect.Y + 40, 63, 1);
        case 5:
          return new Rectangle(this.MenuRect.X + 772, this.MenuRect.Y + 40, 57, 1);
        case 6:
          return new Rectangle(this.MenuRect.X + 849, this.MenuRect.Y + 40, 84, 1);
        default:
          return new Rectangle(this.MenuRect.X + 273, this.MenuRect.Y + 40, 72, 1);
      }
    }

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight + 8, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y - 8, this.MenuRect.Width, 1), Color.Gray);
      if (!this.sessionsLoaded)
        return;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Refresh", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      destinationRectangle.X -= 210;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureX, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Filter " + (this.IsFindFilterActive ? "(Active)" : "(Empty)"), new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
      if (!this.CanSort)
        return;
      destinationRectangle.X -= 172;
      --destinationRectangle.Y;
      this.SpriteBatch.Draw(GraphicStatics.ButtonTexture(Buttons.Start), destinationRectangle, Color.White);
      ++destinationRectangle.Y;
      this.SpriteBatch.DrawString(this.Font, this.ascending ? "Descending" : "Ascending", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
