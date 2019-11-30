// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LoadComponentPackScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Screens
{
  internal class LoadComponentPackScreen : BlockMenuScreen
  {
    public bool AllowDelete;
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private bool drawCalled;
    private float drawTimer;
    private bool saveMode;
    private bool componentsLoaded;
    private int comPackToDelete;
    private GameInstance instance;
    private Action<int> componentSelected;
    private bool includeSystemPacks;

    public LoadComponentPackScreen(
      bool saveMode,
      bool includeSystemPacks,
      Action<int> componentSelected)
      : this((GameInstance) null, (Player) null, saveMode, includeSystemPacks)
    {
      this.componentSelected = componentSelected;
    }

    public LoadComponentPackScreen(
      GameInstance instance,
      Player player,
      bool saveMode,
      bool includeSystemPacks)
      : base("Load Component Pack", player)
    {
      this.instance = instance;
      this.saveMode = saveMode;
      if (saveMode)
        includeSystemPacks = false;
      this.includeSystemPacks = includeSystemPacks;
      this.AllowDelete = true;
      this.MenuEntries.Add((MenuEntry) new BlockMenuEntry((BlockMenuScreen) this, "Loading Component Packs..."));
    }

    private int SortItems(MenuEntry e1, MenuEntry e2)
    {
      return e1.Text.CompareTo(e2.Text);
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 479;
      this.ItemsPerPage = 15;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 32, 0, this.arrowTexture.Width, this.arrowTexture.Height);
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 38;
      }
    }

    private void NewComponentPackMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "New Component Pack", "Enter a max of 30 characters for the new component pack name.", "", new AsyncCallback(this.EndShowKeyboardForNewComponentPack), (object) null);
    }

    private void EndShowKeyboardForNewComponentPack(IAsyncResult ar)
    {
      string name = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      if (name == null)
        return;
      string newPackName = Globals2.StripBadChars(name);
      if (newPackName.Length <= 0)
        return;
      this.ExitScreen();
      PleaseWaitScreen pleaseWaitScreen = new PleaseWaitScreen("Please wait..", "Creating new Component Pack", (Action) null, false);
      this.ScreenManager.AddScreen((GameScreen) pleaseWaitScreen, new PlayerIndex?(this.ControllingPlayer.Value));
      if (newPackName.Length > 30)
        newPackName = newPackName.Substring(0, 30);
      int newComPack = Globals2.CreateNewComPack(this.instance, newPackName);
      if (newComPack >= 0)
        this.ScreenManager.AddScreen((GameScreen) new ComponentListMenuScreen(this.instance, this.player, newComPack, (string) null, (ListBoxScreen.OnMenuItemSelected) null, false, true, (GameScreen) pleaseWaitScreen), this.ControllingPlayer);
      else
        pleaseWaitScreen.ExitScreen();
    }

    private void OpenComponentPackMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      int tag = (int) this.MenuEntries[this.selectedEntry].Tag;
      if (this.componentSelected != null)
      {
        this.componentSelected(tag);
        this.ExitScreen();
      }
      else
      {
        string text = this.MenuEntries[this.selectedEntry].Text;
        bool systemComponent = false;
        foreach (Globals2.ComPackData systemComPackName in Globals2.PublicSystemComPackNames)
        {
          if (systemComPackName.PackName == text)
          {
            systemComponent = true;
            break;
          }
        }
        this.ScreenManager.AddScreen((GameScreen) new ComponentListMenuScreen(this.instance, this.player, tag, (string) null, (ListBoxScreen.OnMenuItemSelected) null, systemComponent, this.saveMode, (GameScreen) null), this.ControllingPlayer);
      }
    }

    private void DeleteMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.comPackToDelete = (int) this.MenuEntries[this.selectedEntry].Tag;
      MessageBoxScreenTM messageBoxScreenTm1 = new MessageBoxScreenTM("Confirm deletion of " + Globals2.StripBadChars(Globals2.ParseComPack(this.comPackToDelete)), "Yes Delete it", (string) null, (string) null, "No don't delete it", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm1.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      MessageBoxScreenTM messageBoxScreenTm2 = messageBoxScreenTm1;
      messageBoxScreenTm2.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnDeleteComponent);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm2, new PlayerIndex?(e.PlayerIndex));
    }

    private void OnDeleteComponent(object sender, PlayerIndexEventArgs e)
    {
      ((MessageBoxScreen) sender).ButtonA -= new EventHandler<PlayerIndexEventArgs>(this.OnDeleteComponent);
      this.DeleteComponentPack(this.comPackToDelete);
      this.ScreenManager.AddScreen((GameScreen) new LoadComponentPackScreen(this.instance, this.player, this.saveMode, this.includeSystemPacks), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void DeleteComponentPack(int comPack)
    {
      try
      {
        FileSystem.DeleteDir(Globals2.ComponentPath(comPack));
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(39, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Delete Error", ex, this.ControllingPlayer);
      }
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.drawCalled || this.componentsLoaded)
        return;
      this.LoadComponentPacks();
      this.componentsLoaded = true;
    }

    private void LoadComponentPacks()
    {
      List<MenuEntry> items = new List<MenuEntry>();
      try
      {
        this.LoadComponentPackFiles(items);
      }
      catch (InvalidOperationException ex)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Error: Not all Component Packs could be loaded", "OK", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      finally
      {
        items.Sort(new Comparison<MenuEntry>(this.SortItems));
        this.MenuEntries.Clear();
        if (this.saveMode)
        {
          BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, "New Component Pack");
          blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.NewComponentPackMenuEntrySelected);
          blockMenuEntry.LoadContent();
          items.Insert(0, (MenuEntry) blockMenuEntry);
        }
        else if (this.includeSystemPacks)
        {
          for (int index = 0; index < Globals2.PublicSystemComPackNames.Length; ++index)
          {
            if (Globals2.PublicSystemComPackNames[index].DirNum != 9)
            {
              BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, Globals2.PublicSystemComPackNames[index].PackName);
              blockMenuEntry.Tag = (object) Globals2.PublicSystemComPackNames[index].DirNum;
              blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OpenComponentPackMenuEntrySelected);
              blockMenuEntry.LoadContent();
              items.Add((MenuEntry) blockMenuEntry);
            }
          }
        }
        this.MenuEntries.AddRange((IEnumerable<MenuEntry>) items);
        BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "Back");
        blockMenuEntry1.Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
        blockMenuEntry1.LoadContent();
        this.MenuEntries.Add((MenuEntry) blockMenuEntry1);
        this.ItemsPerPage = Math.Min(15, this.MenuEntries.Count);
        this.ResetMenuRect();
      }
    }

    private void LoadComponentPackFiles(List<MenuEntry> items)
    {
      FileSystem.CreateDir("Com");
      bool flag = false;
      foreach (string dir in FileSystem.GetDirs("Com\\"))
      {
        if (FileSystem.IsFileExist(dir + "\\header.dat"))
        {
          int num = int.Parse(dir.Substring(dir.Length - 6, 6));
          string text = (string) null;
          try
          {
            using (Stream input = FileSystem.OpenRead(dir + "\\header.dat"))
            {
              using (BinaryReader binaryReader = new BinaryReader(input))
                text = Globals2.StripBadChars(binaryReader.ReadString());
            }
          }
          catch (EndOfStreamException ex)
          {
            text = (string) null;
            FileSystem.DeleteDir(dir);
          }
          if (text != null && text != "System Temp")
          {
            BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, text);
            blockMenuEntry.Tag = (object) num;
            blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.OpenComponentPackMenuEntrySelected);
            if (this.AllowDelete)
              blockMenuEntry.SelectYButton += new EventHandler<PlayerIndexEventArgs>(this.DeleteMenuEntrySelected);
            blockMenuEntry.LoadContent();
            items.Add((MenuEntry) blockMenuEntry);
            flag = true;
          }
        }
      }
      if (flag || !this.saveMode || this.player.SaveState.NewComPackMessageShown)
        return;
      MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM("There are no Component Packs created yet.\r\nComponents are saved in Component Packs.\r\n\r\nAs no Component Pack has yet been created, you must \r\ncreate one now before you save the Component.\r\n\r\nTo create a Component Pack, select 'New Component Pack' and\r\nenter the name of the Pack.\r\n\r\nThen save the Component.", "OK", "Don't show me this message again", (string) null, (string) null, CoreGlobals.GameFont, 0.7f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm.ButtonX += new EventHandler<PlayerIndexEventArgs>(this.DontShowNewComPackMessageAgain);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, this.ControllingPlayer);
    }

    private void DontShowNewComPackMessageAgain(object sender, EventArgs e)
    {
      this.player.SaveState.NewComPackMessageShown = true;
    }

    protected override void DrawBackground()
    {
      this.drawCalled = true;
      this.drawTimer += Services.ElapsedTime;
      base.DrawBackground();
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

    protected override void DrawBottomBar()
    {
      Rectangle destinationRectangle = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 120, this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight, 24, 24);
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, destinationRectangle.Y, this.MenuRect.Width, 1), Color.Gray);
      destinationRectangle.Y += 7;
      this.SpriteBatch.Draw(CoreGlobals.ButtonTextureY, destinationRectangle, Color.White);
      this.SpriteBatch.DrawString(this.Font, "Delete", new Vector2((float) (destinationRectangle.X + 32), (float) (destinationRectangle.Y + 4)) + TMFont.yVec, Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.0f);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
