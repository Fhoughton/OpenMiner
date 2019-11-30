// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PhotoLoader
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner
{
  internal class PhotoLoader : ThreadedWorkerBase
  {
    private int photoID;
    private bool backwards;
    private PhotoFileType type;
    private PhotoLoaded onPhotoLoaded;
    private Action onFinish;
    private ShouldLoadPhoto shouldLoad;

    public PhotoLoader()
      : base(0)
    {
    }

    public void Start(
      int photoID,
      bool backwards,
      PhotoFileType type,
      PhotoLoaded onPhotoLoaded,
      Action onFinish,
      ShouldLoadPhoto shouldLoad)
    {
      this.photoID = photoID;
      this.backwards = backwards;
      this.type = type;
      this.onPhotoLoaded = onPhotoLoaded;
      this.onFinish = onFinish;
      this.shouldLoad = shouldLoad;
      this.Start();
    }

    protected override void ThreadedUpdateCore()
    {
      if ((this.shouldLoad == null || this.shouldLoad(this.photoID)) && !this.onPhotoLoaded(this.photoID, GraphicStatics.PhotoData.LoadPhoto(this.photoID, this.type), this.backwards))
        this.EndThread();
      else if (this.backwards)
      {
        if (--this.photoID >= 0)
          return;
        this.EndThread();
      }
      else
      {
        if (++this.photoID <= (int) byte.MaxValue)
          return;
        this.EndThread();
      }
    }

    private void EndThread()
    {
      this.run = false;
      if (this.onFinish == null)
        return;
      this.onFinish();
    }
  }
}
