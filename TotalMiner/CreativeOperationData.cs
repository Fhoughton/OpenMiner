// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeOperationData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class CreativeOperationData : IProgressBar
  {
    private float progressFactor = 1f;
    public CreativeCommand Command;
    public bool IsValid;
    public MapTM Map;
    public float Progress;
    public string Desc;
    public GlobalPoint3D Point;
    public GlobalPoint3D Min;
    public GlobalPoint3D Max;
    public GlobalPoint3D XMin;
    public GlobalPoint3D XMax;
    public byte BlockID;
    public byte BlockID1;
    public byte BlockID2;
    public byte Percent;
    public int Seed;
    public bool IsCustomSeed;
    public bool Abort;
    public bool ClearMarkers;
    public GamerID GamerID;
    public object Data;
    public Action<CreativeOperationData> OnCompletion;

    float IProgressBar.Progress
    {
      get
      {
        return this.Progress;
      }
    }

    float IProgressBar.Factor
    {
      get
      {
        return this.progressFactor;
      }
      set
      {
        this.progressFactor = value;
      }
    }

    object IProgressBar.Tag { get; set; }

    string IProgressBar.Text
    {
      get
      {
        return this.Desc;
      }
      set
      {
        this.Desc = value;
      }
    }

    void IProgressBar.AddProgress(float increment)
    {
      this.Progress += increment * this.progressFactor;
    }

    void IProgressBar.Reset()
    {
      this.Progress = 0.0f;
    }

    void IProgressBar.Reset(float value)
    {
      this.Progress = value;
    }

    public CreativeOperationData()
    {
    }

    public CreativeOperationData(CreativeOperationData copy)
    {
      this.Command = copy.Command;
      this.IsValid = copy.IsValid;
      this.Map = copy.Map;
      this.Progress = copy.Progress;
      this.Desc = copy.Desc;
      this.Point = copy.Point;
      this.Min = copy.Min;
      this.Max = copy.Max;
      this.XMin = copy.XMin;
      this.XMax = copy.XMax;
      this.BlockID = copy.BlockID;
      this.BlockID1 = copy.BlockID1;
      this.BlockID2 = copy.BlockID2;
      this.Percent = copy.Percent;
      this.IsCustomSeed = copy.IsCustomSeed;
      this.Seed = copy.Seed;
      this.Abort = copy.Abort;
      this.ClearMarkers = copy.ClearMarkers;
      this.GamerID = copy.GamerID;
      this.Data = copy.Data;
      this.OnCompletion = copy.OnCompletion;
    }

    public void ResetDefaults(CreativeOperationData data)
    {
      this.Desc = data.Desc;
      this.BlockID = data.BlockID;
      this.BlockID1 = data.BlockID1;
      this.BlockID2 = data.BlockID2;
      this.Percent = data.Percent;
      this.Seed = data.Seed;
      this.IsCustomSeed = data.Seed != 0;
      this.ClearMarkers = data.ClearMarkers;
    }
  }
}
