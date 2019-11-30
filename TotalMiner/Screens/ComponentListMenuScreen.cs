// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.ComponentListMenuScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace StudioForge.TotalMiner.Screens
{
  internal class ComponentListMenuScreen : FolderListMenuScreen
  {
    private static Dictionary<int, List<string>> comNameCache = new Dictionary<int, List<string>>();
    private static Dictionary<int, List<string>> sysNameCache = new Dictionary<int, List<string>>();
    private int comDir;
    private bool saveMode;
    private bool systemComponent;
    private string comToDelete;
    private GameScreen waitScreen;

    private VoxelModelManager VoxelManager
    {
      get
      {
        if (!this.systemComponent)
          return this.instance.VoxelModelManager;
        return this.instance.SystemVoxelModelManager;
      }
    }

    private Dictionary<int, List<string>> CompNameCache
    {
      get
      {
        if (!this.systemComponent)
          return ComponentListMenuScreen.comNameCache;
        return ComponentListMenuScreen.sysNameCache;
      }
    }

    public ComponentListMenuScreen(
      GameInstance instance,
      Player player,
      int comDir,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool systemComponent)
      : this(instance, player, comDir, path, onSelected, systemComponent, false, (GameScreen) null)
    {
    }

    public ComponentListMenuScreen(
      GameInstance instance,
      Player player,
      int comDir,
      string path,
      ListBoxScreen.OnMenuItemSelected onSelected,
      bool systemComponent,
      bool saveMode,
      GameScreen waitScreen)
      : base(instance, player)
    {
      this.comDir = comDir;
      this.systemComponent = systemComponent;
      this.saveMode = saveMode;
      this.waitScreen = waitScreen;
      this.ItemFolderIcon = Item.FolderIcon;
      this.ItemFileIcon = Item.Clipboard;
      this.Initialize(path, new FolderListMenuScreen.LoadFolderItems(this.LoadComponents), onSelected != null ? onSelected : new ListBoxScreen.OnMenuItemSelected(this.OnComponentSelected), systemComponent ? (EventHandler<PlayerIndexEventArgs>) null : new EventHandler<PlayerIndexEventArgs>(this.OnSelectXButton), systemComponent ? (string) null : (saveMode ? "Save" : "Rename"), systemComponent || saveMode ? (EventHandler<PlayerIndexEventArgs>) null : new EventHandler<PlayerIndexEventArgs>(this.OnSelectYButton), systemComponent || saveMode ? (string) null : "Delete", false);
    }

    protected override void OnScreenRemovedCore()
    {
      base.OnScreenRemovedCore();
      if (this.waitScreen == null)
        return;
      this.waitScreen.ExitScreen();
    }

    protected override GameScreen RestartScreenCore()
    {
      return (GameScreen) new ComponentListMenuScreen(this.instance, this.player, this.comDir, this.currentPath, this.onItemSelected, this.systemComponent, this.saveMode, this.waitScreen);
    }

    protected override bool ShouldRemoveButtonXFromFolders
    {
      get
      {
        return !this.saveMode;
      }
    }

    private string[] LoadComponents(string path)
    {
      List<string> stringList1 = this.GetComNames(this.comDir) ?? new List<string>();
      if (stringList1.Count > 0)
      {
        if (path == null)
          path = "";
        List<string> stringList2 = new List<string>(stringList1.Count);
        for (int index = 0; index < stringList1.Count; ++index)
        {
          string str1 = stringList1[index];
          if (str1.StartsWith(path))
          {
            string str2 = str1.Substring(path.Length);
            int num = str2.IndexOf('\\');
            if (num >= 0)
            {
              string str3 = str2.Substring(0, num + 1);
              if (!stringList2.Contains(str3))
                stringList2.Add(str3);
            }
            else
              stringList2.Add(str2);
          }
        }
        stringList2.Sort(new Comparison<string>(Globals2.SortNamesWithFoldersAtTop));
        stringList1 = stringList2;
      }
      if (this.saveMode)
        stringList1.Insert(0, "New");
      return stringList1.ToArray();
    }

    private List<string> GetComNames(int comDir)
    {
      List<string> stringList;
      if (!this.CompNameCache.TryGetValue(comDir, out stringList))
      {
        stringList = new List<string>();
        string path = Globals2.ComponentPath(this.VoxelManager.RootDir, comDir);
        string[] fileNames = this.GetFileNames(path, "*.com");
        if (fileNames != null)
        {
          foreach (string str1 in fileNames)
          {
            if (!this.systemComponent || !str1.Contains("(SYS)"))
            {
              string str2 = str1.Substring(path.Length, str1.Length - path.Length - 4);
              stringList.Add(str2.Replace('_', '\\'));
            }
          }
        }
        this.CompNameCache.Add(comDir, stringList);
      }
      return stringList;
    }

    private string[] GetFileNames(string path, string searchPattern)
    {
      try
      {
        return this.systemComponent ? TitleFileSystem.GetFiles(path, searchPattern) : FileSystem.GetFiles(path, searchPattern);
      }
      catch (IOException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(48, (Exception) ex);
        return (string[]) null;
      }
    }

    private bool OnComponentSelected(MenuEntry item)
    {
      if (this.saveMode)
        this.SelectNewComponentName((BlockMenuEntry) item);
      else if (this.IsOkToLoadComponent)
        this.LoadComponentThreaded((string) item.Tag + item.Text);
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Cannot load a component while another component is loading.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
      return true;
    }

    private bool IsOkToLoadComponent
    {
      get
      {
        foreach (Gamer localGamer in NetworkManager.Instance.LocalGamers)
        {
          Player tag = localGamer.Tag as Player;
          if (tag != null && tag.HasActionRequest("Loading Component:"))
            return false;
        }
        return true;
      }
    }

    private void LoadComponentThreaded(string comName)
    {
      new Thread(new ParameterizedThreadStart(this.LoadComponentThreadedCore))
      {
        CurrentCulture = Globals1.CultureInfo,
        CurrentUICulture = Globals1.CultureInfo
      }.Start((object) comName);
    }

    private void LoadComponentThreadedCore(object state)
    {
      string str = (string) state;
      int index = this.player.AddActionRequest(string.Format("Loading Component: {0}: ", (object) str), Color.Cyan, 3.0);
      string componentName = str.Replace('\\', '_');
      this.ScreenManager.ExitAllPlayerScreens();
      Player.ActionRequest actionRequest = this.player.GetActionRequest(index);
      string errorDesc;
      MapModel mapModel = this.VoxelManager.LoadComponent(this.comDir, componentName, true, out errorDesc, new Action<bool, object>(this.player.OnComponentLoaded), (object) actionRequest);
      if (mapModel == null)
      {
        if (errorDesc == null)
          errorDesc = "Inventory is full";
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(errorDesc, "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
        this.player.CloseActionRequest(index);
      }
      else
        mapModel.IsSystemModel = this.systemComponent;
    }

    private void OnSelectXButton(object sender, PlayerIndexEventArgs e)
    {
      BlockMenuEntry blockMenuEntry = (BlockMenuEntry) sender;
      if (this.saveMode)
        this.SelectNewComponentName(blockMenuEntry);
      else
        this.RenameComponent(blockMenuEntry);
    }

    private void SelectNewComponentName(BlockMenuEntry item)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "New Component", "Enter a name for the new component. Maximum 36 characters.", this.currentPath, new AsyncCallback(this.EndShowKeyboardForNewComponent), (object) null);
    }

    private void EndShowKeyboardForNewComponent(IAsyncResult ar)
    {
      string name = Guide.EndShowKeyboardInput(ar);
      ar.AsyncWaitHandle.Close();
      bool flag = false;
      if (name != null && name.ToLower() != "new")
      {
        string str = Globals2.StripFolderName(name);
        if (str.Length > 0 && (flag = this.SaveComponent(str.Replace('\\', '_'))))
          this.ScreenManager.ExitAllPlayerScreens(new PlayerIndex?(this.ControllingPlayer.Value));
      }
      TotalMinerGame.Instance.AddNotification("Component " + (flag ? "" : "not ") + "Saved", true);
    }

    private bool SaveComponent(string comName)
    {
      MapModel clipboardModel = this.player.ClipboardModel;
      try
      {
        clipboardModel.DirNum = this.comDir;
        clipboardModel.ComName = comName;
        VoxelModelManager.SaveComponent(clipboardModel);
        this.CompNameCache.Remove(this.comDir);
        return true;
      }
      catch (Exception ex)
      {
        TotalMinerGame.Instance.ShowExceptionMessageBox("Could not save component:\n\n" + ex.Message, this.ControllingPlayer);
        return false;
      }
    }

    private void RenameComponent(BlockMenuEntry item)
    {
      Guide.BeginShowKeyboardInput(this.ScreenManager, this.ControllingPlayer.Value, "Rename Component", "Enter a new name for the component. Maximum 36 characters.", this.currentPath + item.Text, new AsyncCallback(this.EndShowKeyboardForRenameComponent), (object) item);
    }

    private void EndShowKeyboardForRenameComponent(IAsyncResult ar)
    {
      string name = Guide.EndShowKeyboardInput(ar);
      BlockMenuEntry asyncState = (BlockMenuEntry) ar.AsyncState;
      ar.AsyncWaitHandle.Close();
      if (name == null || !(name.ToLower() != "new"))
        return;
      string str1 = Globals2.StripFolderName(name);
      if (str1.Length <= 0)
        return;
      string str2 = str1.Replace('\\', '_');
      string str3 = (this.currentPath + asyncState.Text).Replace('\\', '_');
      string str4 = Globals2.ComponentPath(this.comDir);
      string str5 = Globals2.RenameFile(str4 + str3 + ".com", str4 + str2 + ".com");
      if (str5 == null)
      {
        this.CompNameCache.Remove(this.comDir);
        this.ScreenManager.ExitAllPlayerScreens();
        TotalMinerGame.Instance.AddNotification("Component Renamed", true);
      }
      else
        this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("Rename Error: " + str5, "Ok", (string) null, (string) null, (string) null, this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player), this.ControllingPlayer);
    }

    private void OnSelectYButton(object sender, PlayerIndexEventArgs e)
    {
      this.comToDelete = ((MenuEntry) sender).Text;
      MessageBoxScreenTM messageBoxScreenTm1 = new MessageBoxScreenTM("Confirm deletion of Component: " + this.comToDelete, "Yes Delete it", (string) null, (string) null, "No don't delete it", this.Font, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), this.player);
      messageBoxScreenTm1.TransitionOnTime = TimeSpan.FromSeconds(0.5);
      MessageBoxScreenTM messageBoxScreenTm2 = messageBoxScreenTm1;
      messageBoxScreenTm2.ButtonA += new EventHandler<PlayerIndexEventArgs>(this.OnDeleteComponent);
      this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm2, new PlayerIndex?(e.PlayerIndex));
    }

    private void OnDeleteComponent(object sender, PlayerIndexEventArgs e)
    {
      try
      {
        FileSystem.DeleteFile(Globals2.ComponentPath(this.comDir) + (this.currentPath + this.comToDelete + ".com").Replace('\\', '_'));
        this.CompNameCache.Remove(this.comDir);
        TotalMinerGame.Instance.AddNotification("Component deleted", true);
        if (this.MenuEntries.Count < 2)
          this.OnCancel(this.ControllingPlayer.Value);
        else
          this.RestartScreen();
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(39, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("Delete Error", ex, this.ControllingPlayer);
      }
    }
  }
}
