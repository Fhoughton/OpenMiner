// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.TexturePackLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal class TexturePackLoader : IThreadWorkItem
  {
    private MapTM map;
    private string texpack;
    private Action<string, string> onCompletion;
    private bool copyPaintingData;
    private bool isReload;

    public string Name
    {
      get
      {
        return nameof (TexturePackLoader);
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

    public TexturePackLoader(
      MapTM map,
      string texpack,
      Action<string, string> onCompletion,
      bool copyPaintingData,
      bool isReload)
    {
      this.map = map;
      this.texpack = texpack;
      this.onCompletion = onCompletion;
      this.copyPaintingData = copyPaintingData;
      this.isReload = isReload;
    }

    public void Update()
    {
      try
      {
        GraphicStatics.LoadTexturePack(this.map, this.texpack, this.copyPaintingData, this.isReload);
        if (this.onCompletion == null)
          return;
        this.onCompletion(this.texpack, GraphicStatics.TexturePack.Name);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(110, ex);
      }
    }
  }
}
