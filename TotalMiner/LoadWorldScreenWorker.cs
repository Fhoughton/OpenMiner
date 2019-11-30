// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.LoadWorldScreenWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.TotalMiner.Screens;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class LoadWorldScreenWorker : ThreadedWorkerBase
  {
    private LoadWorldsMenuScreen menuScreen;
    private List<MenuEntry> items;
    private string[] dirList;
    private int currentDir;
    private Action onFinish;
    private MapType mapType;

    public LoadWorldScreenWorker()
      : base(0)
    {
    }

    public void Start(
      LoadWorldsMenuScreen menuScreen,
      List<MenuEntry> items,
      Action onFinish,
      MapType mapType)
    {
      this.menuScreen = menuScreen;
      this.items = items;
      this.onFinish = onFinish;
      this.mapType = mapType;
      this.dirList = (string[]) null;
      this.Start();
    }

    protected override void ThreadedUpdateCore()
    {
      if (this.dirList == null)
      {
        if (this.mapType == MapType.Avatar)
        {
          try
          {
            this.GetWorldFiles();
          }
          catch (IOException ex)
          {
            this.Finished();
          }
        }
        else if (this.mapType == MapType.System)
        {
          try
          {
            this.GetWorldFiles();
          }
          catch (IOException ex)
          {
            this.Finished();
          }
        }
        else
        {
          try
          {
            this.GetWorldFiles();
          }
          catch (IOException ex)
          {
            this.Finished();
          }
        }
      }
      else
        this.LoadNextWorld();
    }

    private void Finished()
    {
      this.run = false;
      if (this.onFinish == null)
        return;
      this.onFinish();
    }

    private void GetWorldFiles()
    {
      lock (Globals1.SaveSemaphore)
      {
        string mapTypeDirName = Globals2.GetMapTypeDirName(this.mapType);
        FileSystem.CreateDir(mapTypeDirName);
        this.dirList = FileSystem.GetDirs(mapTypeDirName + "\\");
        this.currentDir = this.dirList.Length;
      }
    }

    private void LoadNextWorld()
    {
      if (this.currentDir == 0)
      {
        this.Finished();
      }
      else
      {
        int dirnum = 0;
        try
        {
          string dir = this.dirList[--this.currentDir];
          bool isAutoSave = dir.EndsWith("_auto");
          dirnum = Globals2.ParseDirNumber(dir);
          if (dirnum <= 0)
            return;
          SaveGameFileInfo gameFile = Globals2.ParseGameFile(this.mapType, dirnum, false, isAutoSave);
          gameFile.DirNumber = dirnum;
          gameFile.Header.MapName = Globals2.StripBadChars(gameFile.Header.MapName);
          gameFile.Header.IsAutoSave = isAutoSave;
          GameFileMenuEntry gameFileMenuEntry = new GameFileMenuEntry((BlockMenuScreen) this.menuScreen, gameFile);
          gameFileMenuEntry.Tag = (object) gameFile;
          gameFileMenuEntry.LoadContent();
          lock (this.items)
            this.items.Add((MenuEntry) gameFileMenuEntry);
        }
        catch (CorruptWorldFileException ex)
        {
        }
        catch (ThreadAbortException ex)
        {
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(40, ex);
          TotalMinerGame.Instance.AddNotification(string.Format("Game {0} is corrupted", (object) dirnum), true);
        }
      }
    }
  }
}
