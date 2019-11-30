// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.TakePhotoWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.Graphics;
using System;

namespace StudioForge.TotalMiner
{
  internal class TakePhotoWorker : IThreadWorkItem
  {
    private Player player;
    private Color[] photoFull;
    private Color[] photoThumbnail64x64;
    private Color[] photoThumbnail16x16;

    public string Name
    {
      get
      {
        return "PhotoTaker";
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

    public void Initialize(
      Player player,
      Color[] photoFull,
      Color[] photoThumbnail64x64,
      Color[] photoThumbnail16x16)
    {
      this.player = player;
      this.photoFull = photoFull;
      this.photoThumbnail64x64 = photoThumbnail64x64;
      this.photoThumbnail16x16 = photoThumbnail16x16;
    }

    public void Update()
    {
      try
      {
        int newPhotoNumber = Globals2.GetNewPhotoNumber();
        if (newPhotoNumber <= 0)
          return;
        PhotoInfo info = this.BuildPhotoInfo(this.player, newPhotoNumber);
        GraphicStatics.PhotoData.SavePhoto(newPhotoNumber, this.photoFull, PhotoFileType.PhotoImage);
        GraphicStatics.PhotoData.SavePhoto(newPhotoNumber, this.photoThumbnail64x64, PhotoFileType.HDThumbnail);
        GraphicStatics.PhotoData.SavePhoto(newPhotoNumber, this.photoThumbnail16x16, PhotoFileType.SDThumbnail);
        GraphicStatics.PhotoData.SavePhotoInfo(info);
        lock (GraphicStatics.PhotoData.PhotoIDsNotFound)
          GraphicStatics.PhotoData.PhotoIDsNotFound.Remove(newPhotoNumber);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(103, ex);
      }
      finally
      {
        this.player.IsAssemblingPhoto = false;
        this.photoFull = (Color[]) null;
        this.photoThumbnail64x64 = (Color[]) null;
        this.photoThumbnail16x16 = (Color[]) null;
      }
    }

    private PhotoInfo BuildPhotoInfo(Player player, int photoID)
    {
      return new PhotoInfo()
      {
        PhotoID = photoID,
        MapName = Globals2.GameProperties.SaveGame.Header.MapName,
        MapOwner = Globals2.GameProperties.SaveGame.Header.OwnerGamerTag,
        Photographer = player.Gamertag
      };
    }
  }
}
