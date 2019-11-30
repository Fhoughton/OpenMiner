// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LoadWorldsMenuScreen
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
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Screens
{
  internal class LoadWorldsMenuScreen : BlockMenuScreen
  {
    private Texture2D arrowTexture;
    private Rectangle arrowRect;
    private LoadGameCallback callback;
    private bool drawCalled;
    private PleaseWaitScreen pleaseWaitScreen;
    private FilesizeBuilder filesizeWorker;
    private LoadWorldScreenWorker loadWorker;
    private List<MenuEntry> itemsAddedByWorker;
    private MenuEntry itemToCopyRenameDelete;
    private SaveGameFileInfo fileToCopyRenameDelete;
    private MapType mapType;
    private bool includeOptions;

    public LoadWorldsMenuScreen()
      : this((LoadGameCallback) null, MapType.Map, true)
    {
    }

    public LoadWorldsMenuScreen(LoadGameCallback callback, MapType mapType, bool includeOptions)
      : base("Load Game", (Player) null)
    {
      this.callback = callback;
      this.mapType = mapType;
      this.includeOptions = includeOptions;
    }

    public override void LoadContent()
    {
      this.DrawLeftMarginLine = this.DrawPanel = false;
      this.DrawItemTextures = this.DrawLastLine = false;
      this.DrawTitleStrip = false;
      this.HighlightRect.Width = 768;
      this.ItemsPerPage = 15;
      this.Font = this.ItemFont = CoreGlobals.GameFont;
      base.LoadContent();
      this.arrowTexture = CoreGlobals.Content.Load<Texture2D>("Textures\\MenuArrow");
      this.arrowRect = new Rectangle(this.MenuRect.X + this.MenuRect.Width - 22, 0, this.arrowTexture.Width, this.arrowTexture.Height);
      this.itemsAddedByWorker = new List<MenuEntry>();
    }

    protected override void OnScreenAddedCore()
    {
      base.OnScreenAddedCore();
      if (Globals2.AutoStartMap >= 0)
        return;
      this.NewGameMenuEntrySelected((object) null, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
    }

    protected override int ButtonBarHeight
    {
      get
      {
        return 42;
      }
    }

    private void EndThreadWorkers()
    {
      if (this.loadWorker != null)
        this.loadWorker.End(0);
      if (this.filesizeWorker == null)
        return;
      this.filesizeWorker.End(0);
    }

    protected override void OnScreenRemovedCore()
    {
      this.EndThreadWorkers();
      base.OnScreenRemovedCore();
    }

    public override bool HandleInput(InputState input)
    {
      if (this.ControllingPlayer.HasValue && this.selectedEntry >= 0 && this.selectedEntry < this.MenuEntries.Count && input.CurrentGamePadStates[(int) this.ControllingPlayer.Value].Buttons.RightShoulder == ButtonState.Pressed && input.LastGamePadStates[(int) this.ControllingPlayer.Value].Buttons.RightShoulder == ButtonState.Released)
        return true;
      return base.HandleInput(input);
    }

    private void NewGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.callback != null)
      {
        this.callback((SaveGameFileInfo) null);
        this.ExitScreen();
      }
      else
      {
        if (this.mapType != MapType.Avatar)
        {
          this.ScreenManager.AddScreen((GameScreen) new NewGameMenuScreen(), new PlayerIndex?(e.PlayerIndex));
        }
        else
        {
          Globals2.GameProperties.SaveGame.Header.MapSeed = 0;
          Globals2.GameProperties.SaveGame.Header.GameMode = GameMode.Creative;
          Globals2.GameProperties.SaveGame.Header.GameDifficulty = GameDifficulty.Peaceful;
          Globals2.GameProperties.SaveGame.Header.CombatEnabled = false;
          Globals2.GameProperties.SaveGame.Header.Attribute = MapAttribute.AvatarDesigner;
          Globals2.GameProperties.SaveGame.Header.EnemyMobs = false;
          Globals2.GameProperties.SaveGame.Header.FiniteMode = false;
          Globals2.GameProperties.SaveGame.Header.ResetMapBounds();
          Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel = (ushort) 0;
          Globals2.GameProperties.SaveGame.Header.PassiveMobs = false;
          Globals2.GameProperties.SaveGame.Header.RegionSize = new Point3D(256, 256, 256);
          Globals2.GameProperties.SaveGame.Header.TotalMapBound = new BoxInt()
          {
            Min = new GlobalPoint3D(0, 0, 0),
            Max = new GlobalPoint3D(32, 64, 32)
          };
          Globals2.GameProperties.SaveGame.Header.CurrentMapBound = new BoxInt()
          {
            Min = new GlobalPoint3D(0, 0, 0),
            Max = new GlobalPoint3D(32, 64, 32)
          };
          Globals2.GameProperties.SaveGame.Header.TexturePack = "AvatarPalette";
          Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock = Item.None;
          Globals2.GameProperties.BiomeType = Globals2.GameProperties.SaveGame.Header.TerrainData.Biome = BiomeType.Flat;
          Globals2.GameSettings.ViewClouds = false;
          Globals2.GameSettings.FloraAnimation = false;
          this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), this.ControllingPlayer);
        }
        this.ExitScreen();
      }
    }

    private void SystemWorldsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen((LoadGameCallback) null, MapType.System, false), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void AvatarsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen((LoadGameCallback) null, MapType.Avatar, true), new PlayerIndex?(e.PlayerIndex));
      this.ExitScreen();
    }

    private void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.EndThreadWorkers();
      SaveGameFileInfo tag = ((MenuEntry) sender).Tag as SaveGameFileInfo;
      if (this.callback != null)
      {
        this.callback(tag);
        this.ExitScreen();
      }
      else
      {
        Globals2.GameProperties.IsNewMap = false;
        Globals2.GameProperties.SaveGame = tag;
        Globals2.GameProperties.UseOldGenerator = tag.Header.CreatedVersion == 0;
        Globals2.LastMapPlayed = tag.DirNumber;
        this.ScreenManager.AddScreen((GameScreen) new LobbyScreen(false), new PlayerIndex?(e.PlayerIndex));
        this.ExitScreen();
      }
    }

    private void CopyGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (this.mapType != MapType.System)
      {
        if (!this.CanCopyGameSave(this.itemToCopyRenameDelete = (MenuEntry) sender))
          return;
        this.fileToCopyRenameDelete = this.itemToCopyRenameDelete.Tag as SaveGameFileInfo;
        Guide.BeginShowKeyboardInput(this.ScreenManager, e.PlayerIndex, "Copy World", "Enter a max of 14 characters for the new name.", this.fileToCopyRenameDelete.Header.MapName, new AsyncCallback(this.EndShowKeyboardForCopyWorld), (object) null);
        this.pleaseWaitScreen = new PleaseWaitScreen("Copying. Please wait...", "Do not quit to dashboard");
        this.ScreenManager.AddScreen((GameScreen) this.pleaseWaitScreen, this.ControllingPlayer);
      }
      else
        this.CopySystemMap(((MenuEntry) sender).Tag as SaveGameFileInfo);
    }

    private void EndShowKeyboardForCopyWorld(IAsyncResult ar)
    {
      string newMapName = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (newMapName.Length > 0)
      {
        if (newMapName.Length > 14)
          newMapName = newMapName.Substring(0, 14);
        if (this.fileToCopyRenameDelete.Header.MapName != newMapName)
          this.CopyGame(newMapName);
      }
      this.pleaseWaitScreen.ExitScreen();
    }

    private void CopyGame(string newMapName)
    {
      try
      {
        this.EndThreadWorkers();
        SaveGameFileInfo saveGameFileInfo = new SaveGameFileInfo(this.mapType);
        saveGameFileInfo.DirNumber = Globals2.GetNewMapDirNumber(this.mapType);
        SaveDataResult data = new SaveDataResult()
        {
          SaveData = MapLoader.LoadMapDataExternal(this.fileToCopyRenameDelete.MapFilePath)
        };
        data.SaveData.Header.MapName = newMapName;
        data.SaveData.Header.IsAutoSave = false;
        saveGameFileInfo.Header = data.SaveData.Header;
        saveGameFileInfo.Header.MapName = newMapName;
        MapSaver.SaveMapData(saveGameFileInfo.MapFilePath, data, (IProgressBar) null, false, true);
        MapSaver.CopyMapFiles(this.fileToCopyRenameDelete.MapFilePath, saveGameFileInfo.MapFilePath, true);
        this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen(this.callback, this.mapType, this.includeOptions), this.ControllingPlayer);
        this.ExitScreen();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(37, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Copy Error", ex, this.ControllingPlayer);
      }
    }

    private bool CanCopyGameSave(MenuEntry entry)
    {
      SaveGameFileInfo tag = entry.Tag as SaveGameFileInfo;
      if (tag == null)
        return false;
      if (!tag.Header.GoodHash)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This Game File has been tampered with\n\nTampered Game Files cannot be copied.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        return false;
      }
      if (tag.Header.SaveVersion >= 294)
        return true;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This world was saved on a version prior to the current version.\nThe game can only copy worlds that are saved with the current version.\nTo do that, load this world, save it, quit and then copy.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      return false;
    }

    private void CopySystemMap(SaveGameFileInfo info)
    {
      if (info == null)
        return;
      try
      {
        int newMapDirNumber = Globals2.GetNewMapDirNumber(MapType.Map);
        string mapFilePath1 = Globals2.GetMapFilePath(MapType.System, info.DirNumber);
        string mapFilePath2 = Globals2.GetMapFilePath(MapType.Map, newMapDirNumber);
        FileSystem.CreateDir(mapFilePath2);
        MapSaver.CopyMapFiles(mapFilePath1, mapFilePath2, false);
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Copy Successful\nYou will find the copy on the regular world list.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(37, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Copy Error", ex, this.ControllingPlayer);
      }
    }

    private void RenameGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      if (!this.CanRenameGameSave((MenuEntry) sender))
        return;
      this.itemToCopyRenameDelete = (MenuEntry) sender;
      this.fileToCopyRenameDelete = this.itemToCopyRenameDelete.Tag as SaveGameFileInfo;
      Guide.BeginShowKeyboardInput(this.ScreenManager, e.PlayerIndex, "Rename World", "Enter a max of 14 characters for the new name.", this.fileToCopyRenameDelete.Header.MapName, new AsyncCallback(this.EndShowKeyboardForRename), (object) null);
      this.pleaseWaitScreen = new PleaseWaitScreen("Renaming. Please wait...", "Do not quit to dashboard");
      this.ScreenManager.AddScreen((GameScreen) this.pleaseWaitScreen, this.ControllingPlayer);
    }

    private void EndShowKeyboardForRename(IAsyncResult ar)
    {
      string newMapName = Globals2.StripBadChars(Guide.EndShowKeyboardInput(ar));
      ar.AsyncWaitHandle.Close();
      if (newMapName.Length > 0)
      {
        if (newMapName.Length > 14)
          newMapName = newMapName.Substring(0, 14);
        if (this.fileToCopyRenameDelete.Header.MapName != newMapName)
          this.RenameGame(newMapName);
      }
      this.pleaseWaitScreen.ExitScreen();
    }

    private void RenameGame(string newMapName)
    {
      try
      {
        SaveDataResult data = new SaveDataResult()
        {
          SaveData = MapLoader.LoadMapDataExternal(this.fileToCopyRenameDelete.MapFilePath)
        };
        data.SaveData.Header.MapName = newMapName;
        MapSaver.SaveMapData(this.fileToCopyRenameDelete.MapFilePath, data, (IProgressBar) null, false, true);
        this.fileToCopyRenameDelete.Header.MapName = newMapName;
        this.itemToCopyRenameDelete.Text = newMapName;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(38, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Rename Error", ex, this.ControllingPlayer);
      }
    }

    private bool CanRenameGameSave(MenuEntry entry)
    {
      SaveGameFileInfo tag = entry.Tag as SaveGameFileInfo;
      if (tag == null || tag.MapType != MapType.Map)
        return false;
      if (!tag.Header.GoodHash)
      {
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This Game File has been tampered with\n\nTampered Game Files cannot be renamed.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        return false;
      }
      if (tag.Header.SaveVersion >= 294)
        return true;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This world was saved on a version prior to the current version.\nThe game can only rename worlds that are saved with the current version.\nTo do that, load this world, save it, quit and then rename.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.6f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      return false;
    }

    private void DeleteGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
    {
      this.itemToCopyRenameDelete = (MenuEntry) sender;
      this.fileToCopyRenameDelete = this.itemToCopyRenameDelete.Tag as SaveGameFileInfo;
      if (this.fileToCopyRenameDelete == null || this.fileToCopyRenameDelete.MapType != MapType.Map || (this.fileToCopyRenameDelete.Header.MapName == null || this.fileToCopyRenameDelete.DirNumber <= 0))
        return;
      MessageBoxScreenTM messageBoxScreenTm1 = new MessageBoxScreenTM("Confirm deletion of " + this.fileToCopyRenameDelete.Header.MapName, "Yes Delete it", (string) null, (string) null, "No don't delete it", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm1.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      MessageBoxScreenTM messageBoxScreenTm2 = messageBoxScreenTm1;
      messageBoxScreenTm2.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnDeleteGame);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm2, new PlayerIndex?(e.PlayerIndex));
    }

    private void OnDeleteGame(object sender, PlayerIndexEventArgs e)
    {
      ((MessageBoxScreen) sender).ButtonA -= new EventHandler<PlayerIndexEventArgs>(this.OnDeleteGame);
      this.pleaseWaitScreen = new PleaseWaitScreen("Deleting. Please wait...", "Do not quit to dashboard", new Action(this.DeleteGame));
      this.ScreenManager.AddScreen((GameScreen) this.pleaseWaitScreen, this.ControllingPlayer);
    }

    private void DeleteGame()
    {
      try
      {
        this.EndThreadWorkers();
        FileSystem.DeleteDir(Globals2.GetMapFilePath(this.mapType, this.fileToCopyRenameDelete.DirNumber, this.fileToCopyRenameDelete.Header.IsAutoSave));
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(66, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Delete Error", ex, this.ControllingPlayer);
      }
      if (this.pleaseWaitScreen != null)
      {
        this.pleaseWaitScreen.ExitScreen();
        this.pleaseWaitScreen = (PleaseWaitScreen) null;
      }
      this.ExitScreen();
      this.ScreenManager.AddScreen((GameScreen) new LoadWorldsMenuScreen(this.callback, this.mapType, this.includeOptions), this.ControllingPlayer);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      if (!this.drawCalled)
        return;
      if (this.loadWorker == null)
        this.LoadWorlds();
      else
        this.CheckWorkerLoad();
    }

    private void CheckWorkerLoad()
    {
      lock (this.itemsAddedByWorker)
      {
        if (this.itemsAddedByWorker.Count > 0)
        {
          MenuEntry menuEntry1 = (MenuEntry) null;
          foreach (MenuEntry menuEntry2 in this.itemsAddedByWorker)
          {
            if (menuEntry2 != null)
            {
              menuEntry2.Selected += new EventHandler<PlayerIndexEventArgs>(this.LoadGameMenuEntrySelected);
              this.MenuEntries.Insert(this.MenuEntries.Count - 1, menuEntry2);
              if (Globals2.AutoStartMap > 0)
              {
                SaveGameFileInfo tag = menuEntry2.Tag as SaveGameFileInfo;
                if (tag.DirNumber == Globals2.AutoStartMap && !tag.IsAutoSave)
                  menuEntry1 = menuEntry2;
              }
            }
          }
          this.itemsAddedByWorker.Clear();
          this.MenuEntries.Sort(new Comparison<MenuEntry>(this.SortWorldEntries));
          this.ItemsPerPage = Math.Min(15, this.MenuEntries.Count);
          this.ResetMenuRect();
          if (menuEntry1 == null)
            return;
          this.LoadGameMenuEntrySelected((object) menuEntry1, new PlayerIndexEventArgs(this.ControllingPlayer.Value));
        }
        else
        {
          if (this.loadWorker.IsStarted || this.filesizeWorker != null)
            return;
          this.filesizeWorker = new FilesizeBuilder();
          this.filesizeWorker.Start(this.MenuEntries.ToArray());
        }
      }
    }

    private int SortWorldEntries(MenuEntry e1, MenuEntry e2)
    {
      int num1 = this.MenuEntries.IndexOf(e1);
      int num2 = this.MenuEntries.IndexOf(e2);
      int num3 = this.MenuEntries.Count - 1;
      if (num1 == num3 || num2 == num3 || this.includeOptions && (num1 < 3 || num2 < 3))
        return num1.CompareTo(num2);
      int num4 = e1.Text.CompareTo(e2.Text);
      if (num4 != 0)
        return num4;
      string text1 = e1.Text;
      string text2 = e2.Text;
      SaveGameFileInfo tag1 = e1.Tag as SaveGameFileInfo;
      SaveGameFileInfo tag2 = e2.Tag as SaveGameFileInfo;
      if (tag1 != null && tag1.Header.IsAutoSave)
        text1 += "Z";
      if (tag2 != null && tag2.Header.IsAutoSave)
        text2 += "Z";
      return text1.CompareTo(text2);
    }

    private void LoadWorlds()
    {
      if (this.includeOptions && this.mapType != MapType.System)
      {
        BlockMenuEntry blockMenuEntry = new BlockMenuEntry((BlockMenuScreen) this, this.mapType == MapType.Avatar ? "New Avatar" : "New World");
        blockMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(this.NewGameMenuEntrySelected);
        blockMenuEntry.LoadContent();
        this.MenuEntries.Add((MenuEntry) blockMenuEntry);
      }
      if (this.includeOptions && this.mapType != MapType.System)
      {
        BlockMenuEntry blockMenuEntry1 = new BlockMenuEntry((BlockMenuScreen) this, "System Worlds");
        blockMenuEntry1.Selected += new EventHandler<PlayerIndexEventArgs>(this.SystemWorldsMenuEntrySelected);
        blockMenuEntry1.LoadContent();
        this.MenuEntries.Add((MenuEntry) blockMenuEntry1);
        BlockMenuEntry blockMenuEntry2 = new BlockMenuEntry((BlockMenuScreen) this, "Loading Worlds. Please wait...");
        blockMenuEntry2.LoadContent();
        this.MenuEntries.Add((MenuEntry) blockMenuEntry2);
        blockMenuEntry2.ColorUnselected = blockMenuEntry2.ColorSelected = Color.Orange;
      }
      BlockMenuEntry blockMenuEntry3 = new BlockMenuEntry((BlockMenuScreen) this, "Back");
      blockMenuEntry3.Selected += new EventHandler<PlayerIndexEventArgs>(((MenuScreen) this).OnCancel);
      blockMenuEntry3.LoadContent();
      this.MenuEntries.Add((MenuEntry) blockMenuEntry3);
      this.ItemsPerPage = Math.Min(15, this.MenuEntries.Count);
      this.ResetMenuRect();
      this.loadWorker = new LoadWorldScreenWorker();
      this.loadWorker.Start(this, this.itemsAddedByWorker, new Action(this.OnWorldsAllLoaded), this.mapType);
    }

    private void OnWorldsAllLoaded()
    {
      if (this.mapType != MapType.Map || !this.includeOptions)
        return;
      this.MenuEntries.RemoveAt(2);
    }

    protected override void DrawBackground()
    {
      this.drawCalled = true;
      base.DrawBackground();
      this.SpriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(this.MenuRect.X, new Rectangle(this.MenuRect.X + this.MenuRect.Width - 220, this.MenuRect.Y + this.MenuRect.Height - 30, 24, 24).Y - 8, this.MenuRect.Width, 1), Color.Gray);
      if (this.itemAtTopOfPage > 0)
      {
        this.arrowRect.Y = this.MenuRect.Y + 8;
        this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
      }
      if (this.itemAtTopOfPage + this.ItemsPerPage >= this.MenuEntries.Count)
        return;
      this.arrowRect.Y = this.MenuRect.Y + this.MenuRect.Height - this.ButtonBarHeight - 26;
      this.SpriteBatch.Draw(this.arrowTexture, this.arrowRect, Color.White);
    }

    protected override void DrawTitle()
    {
    }

    protected override void DrawButtons(int x)
    {
    }
  }
}
