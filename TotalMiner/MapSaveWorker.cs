// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.MapSaveWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Storage;
using System;
using System.Threading;

namespace StudioForge.TotalMiner
{
  internal class MapSaveWorker : IThreadWorkItem
  {
    private bool isAutoSave;
    private GameInstance instance;
    private IProgressBar progressBar;
    private Action<bool, bool> callBack;

    public string Name
    {
      get
      {
        return "ThreadedMapSaver";
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public MapSaveWorker(GameInstance instance, bool isAutoSave, IProgressBar progressBar)
      : this(instance, isAutoSave, progressBar, (Action<bool, bool>) null)
    {
    }

    public MapSaveWorker(
      GameInstance instance,
      bool isAutoSave,
      IProgressBar progressBar,
      Action<bool, bool> callBack)
    {
      this.instance = instance;
      this.isAutoSave = isAutoSave;
      this.progressBar = progressBar;
      this.callBack = callBack;
    }

    public void Update()
    {
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        MapSaver.SaveMapToFile(this.instance, this.progressBar, this.isAutoSave);
        if (this.progressBar != null)
        {
          this.progressBar.Text = "Flushing Device";
          this.progressBar.Reset(1f);
        }
        Thread.Sleep(1000);
        flag1 = true;
      }
      catch (OtherDiskActivityInProgressException ex)
      {
        flag2 = true;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(2, ex);
      }
      finally
      {
        if (this.callBack != null)
          this.callBack(flag1, flag2);
      }
    }

    public static float GetNewAutoSaveTime()
    {
      switch (Globals2.GameSettings.AutoSave)
      {
        case AutoSaveSetting.Every5Minutes:
          return 300f;
        case AutoSaveSetting.Every15Minutes:
          return 900f;
        case AutoSaveSetting.Every30Minutes:
          return 1800f;
        default:
          return float.MaxValue;
      }
    }
  }
}
