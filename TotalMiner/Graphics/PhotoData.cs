// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.PhotoData
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Graphics
{
  internal class PhotoData
  {
    public Color[][] PhotoThumbnail64ColorData = new Color[16][];
    public Color[][] PhotoThumbnail32ColorData = new Color[16][];
    public Color[][] PhotoThumbnail16ColorData = new Color[16][];
    public List<int> PhotoIDsNotFound = new List<int>();
    private int lastPhotoIDLoaded;

    public void ClearPhotoThumbnailColorData()
    {
      this.PhotoThumbnail64ColorData = new Color[16][];
      this.PhotoThumbnail32ColorData = new Color[16][];
      this.PhotoThumbnail16ColorData = new Color[16][];
    }

    public void ClearPhotoThumbnailColorData(int index)
    {
      this.PhotoThumbnail64ColorData[index] = (Color[]) null;
      this.PhotoThumbnail32ColorData[index] = (Color[]) null;
      this.PhotoThumbnail16ColorData[index] = (Color[]) null;
    }

    public Texture2D LoadPhoto(int photoID, PhotoFileType type)
    {
      Texture2D texture2D = (Texture2D) null;
      if (photoID > 0 && type != PhotoFileType.PhotoInfo)
      {
        Color[] photoColorData = this.GetPhotoColorData(this.LoadPhotoRawData(photoID, type));
        if (photoColorData != null && photoColorData.Length >= 1)
        {
          int num = (int) Math.Sqrt((double) photoColorData.Length);
          if (num * num == photoColorData.Length)
          {
            texture2D = new Texture2D(CoreGlobals.GraphicsDevice, num, num);
            texture2D.SetData<Color>(photoColorData);
          }
        }
      }
      return texture2D;
    }

    private byte[] LoadPhotoRawData(int photoID, PhotoFileType type)
    {
      if (this.lastPhotoIDLoaded >= photoID)
      {
        lock (this.PhotoIDsNotFound)
        {
          if (this.PhotoIDsNotFound.Contains(photoID))
            return (byte[]) null;
        }
      }
      byte[] buffer = (byte[]) null;
      lock (Globals1.SaveSemaphore)
      {
        string photoFilename = Globals2.GetPhotoFilename(photoID, type);
        if (FileSystem.IsFileExist(photoFilename))
        {
          using (Stream input = FileSystem.OpenRead(photoFilename))
          {
            using (BinaryReader binaryReader = new BinaryReader(input))
            {
              binaryReader.ReadInt32();
              buffer = new byte[input.Length - input.Position];
              binaryReader.Read(buffer, 0, buffer.Length);
            }
          }
        }
        else
        {
          lock (this.PhotoIDsNotFound)
            this.PhotoIDsNotFound.Add(photoID);
        }
        if (this.lastPhotoIDLoaded < photoID)
          this.lastPhotoIDLoaded = photoID;
      }
      return buffer;
    }

    private Color[] GetPhotoColorData(byte[] data)
    {
      if (data == null || data.Length <= 0)
        return (Color[]) null;
      Color[] colorArray = new Color[data.Length / 3];
      Color color = new Color();
      color.A = byte.MaxValue;
      int num1 = 0;
      for (int index1 = 0; index1 < colorArray.Length; ++index1)
      {
        ref Color local1 = ref color;
        byte[] numArray1 = data;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        local1.R = (byte) num3;
        ref Color local2 = ref color;
        byte[] numArray2 = data;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        local2.G = (byte) num5;
        ref Color local3 = ref color;
        byte[] numArray3 = data;
        int index4 = num4;
        num1 = index4 + 1;
        int num6 = (int) numArray3[index4];
        local3.B = (byte) num6;
        colorArray[index1] = color;
      }
      return colorArray;
    }

    public void SavePhoto(int photoID, Color[] photo, PhotoFileType type)
    {
      if (photo.Length <= 0)
        return;
      byte[] buffer = new byte[photo.Length * 3];
      int index1 = 0;
      int num1 = 0;
      for (; index1 < photo.Length; ++index1)
      {
        Color color = photo[index1];
        byte[] numArray1 = buffer;
        int index2 = num1;
        int num2 = index2 + 1;
        int r = (int) color.R;
        numArray1[index2] = (byte) r;
        byte[] numArray2 = buffer;
        int index3 = num2;
        int num3 = index3 + 1;
        int g = (int) color.G;
        numArray2[index3] = (byte) g;
        byte[] numArray3 = buffer;
        int index4 = num3;
        num1 = index4 + 1;
        int b = (int) color.B;
        numArray3[index4] = (byte) b;
      }
      FileSystem.CreateDir(Globals2.GetPhotoFilePath(photoID));
      using (Stream output = FileSystem.OpenWrite(Globals2.GetPhotoFilename(photoID, type)))
      {
        using (BinaryWriter binaryWriter = new BinaryWriter(output))
        {
          binaryWriter.Write(294);
          binaryWriter.Write(buffer, 0, buffer.Length);
        }
      }
    }

    public PhotoInfo ReadPhotoInfo(int photoID)
    {
      PhotoInfo photoInfo = new PhotoInfo()
      {
        PhotoID = photoID,
        MapName = "Unknown",
        MapOwner = "Unknown",
        Photographer = "Unknown"
      };
      string photoFilename = Globals2.GetPhotoFilename(photoID, PhotoFileType.PhotoInfo);
      if (FileSystem.IsFileExist(photoFilename))
      {
        using (Stream input = FileSystem.OpenRead(photoFilename))
        {
          using (BinaryReader reader = new BinaryReader(input))
          {
            reader.ReadInt32();
            photoInfo.PhotoID = reader.ReadInt32();
            photoInfo.MapName = Globals2.ReadEncryptedString(reader);
            photoInfo.MapOwner = Globals2.ReadEncryptedString(reader);
            photoInfo.Photographer = Globals2.ReadEncryptedString(reader);
          }
        }
      }
      return photoInfo;
    }

    public void SavePhotoInfo(PhotoInfo info)
    {
      using (Stream output = FileSystem.OpenWrite(Globals2.GetPhotoFilename(info.PhotoID, PhotoFileType.PhotoInfo)))
      {
        using (BinaryWriter writer = new BinaryWriter(output))
        {
          writer.Write(294);
          writer.Write(info.PhotoID);
          Globals2.WriteEncryptedString(writer, info.MapName);
          Globals2.WriteEncryptedString(writer, info.MapOwner);
          Globals2.WriteEncryptedString(writer, info.Photographer);
        }
      }
    }

    public void LoadPhotoThumbnails(MapTM map)
    {
      for (byte index = 1; index < (byte) 16; ++index)
      {
        byte blockTextureId = (byte) map.GetBlockTextureID(Block.Painting, (int) index);
        if (blockTextureId > (byte) 0)
        {
          this.LoadPhotoThumbnail((int) blockTextureId, index, PhotoFileType.HDThumbnail);
          this.LoadPhotoThumbnail((int) blockTextureId, index, PhotoFileType.SDThumbnail);
        }
      }
    }

    public void LoadPhotoThumbnail(int photoID, byte index, PhotoFileType type)
    {
      Color[] photoColorData = this.GetPhotoColorData(this.LoadPhotoRawData(photoID, type));
      this.SetPhotoThumbnailColorData(index, photoColorData);
    }

    public void LoadPhotoThumbnail(byte index, Texture2D texture)
    {
      Color[] colorArray = new Color[texture.Width * texture.Height];
      texture.GetData<Color>(colorArray);
      this.SetPhotoThumbnailColorData(index, colorArray);
    }

    public void SetPhotoThumbnailColorData(byte index, Color[] colorData)
    {
      if (index <= (byte) 0 || colorData == null || colorData.Length <= 0)
        return;
      double num = Math.Sqrt((double) colorData.Length);
      if (num == 16.0)
        this.PhotoThumbnail16ColorData[(int) index] = colorData;
      else if (num == 32.0)
      {
        this.PhotoThumbnail32ColorData[(int) index] = colorData;
      }
      else
      {
        if (num != 64.0)
          return;
        this.PhotoThumbnail64ColorData[(int) index] = colorData;
      }
    }

    public void LoadPaintingsIntoTPImmediate(MapTM map, TexturePack texPack)
    {
      if (map == null || map.BlockTextures == null || (texPack == null || texPack.BlockTexture == null))
        return;
      int num = texPack.BlockTextureSize();
      for (byte index = 1; index < (byte) 16; ++index)
      {
        if (map.GetBlockTextureID(Block.Painting, (int) index) > Block.None)
        {
          Color[] colorData1;
          switch (num)
          {
            case 32:
              colorData1 = this.PhotoThumbnail32ColorData[(int) index];
              break;
            case 64:
              colorData1 = this.PhotoThumbnail64ColorData[(int) index];
              break;
            default:
              colorData1 = this.PhotoThumbnail16ColorData[(int) index];
              break;
          }
          this.CopyPhotoThumbnailIntoTexturePack(texPack, colorData1, index, false);
          if (num == 64)
          {
            Color[] colorData2 = this.PhotoThumbnail16ColorData[(int) index];
            this.CopyPhotoThumbnailIntoTexturePack(texPack, colorData2, index, true);
          }
        }
      }
    }

    public void CopyPhotoThumbnailIntoTexturePack(
      TexturePack texPack,
      Color[] colorData,
      byte index,
      bool isLOD)
    {
      if (index <= (byte) 0 || colorData == null || (colorData.Length <= 0 || texPack == null))
        return;
      if (isLOD)
      {
        if (texPack.BlockTextureLOD == null)
          return;
        if (texPack.BlockTextureLOD == texPack.BlockTexture)
          return;
      }
      try
      {
        Texture2D destTexture = isLOD ? texPack.BlockTextureLOD : texPack.BlockTexture;
        Rectangle photoBlockDestRect = this.GetPhotoBlockDestRect(texPack, destTexture, index);
        destTexture.SetData<Color>(0, new Rectangle?(photoBlockDestRect), colorData, 0, colorData.Length);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(117, ex);
      }
    }

    public bool IsPhotoThumbnailRequestSent(byte index)
    {
      return this.IsPhotoThumbnailColorDataLoaded(index);
    }

    public bool IsPhotoThumbnailColorDataLoaded(byte index)
    {
      if (index != (byte) 0)
        return this.PhotoThumbnail64ColorData[(int) index] != null;
      return true;
    }

    public Rectangle GetPhotoBlockDestRect(
      TexturePack texPack,
      Texture2D destTexture,
      byte index)
    {
      int index1 = 24 + (int) index % 8;
      if (index >= (byte) 8)
        index1 += 32;
      Rectangle blockSrcRect = texPack.BlockSrcRects[index1];
      int num = texPack.BlockTexture.Width / destTexture.Width;
      blockSrcRect.X /= num;
      blockSrcRect.Y /= num;
      blockSrcRect.Width /= num;
      blockSrcRect.Height /= num;
      blockSrcRect.Y += blockSrcRect.Height * 11;
      return blockSrcRect;
    }

    public PhotoFileType GetPhotoThumbnailType(bool isHD)
    {
      return !isHD ? PhotoFileType.SDThumbnail : PhotoFileType.HDThumbnail;
    }

    public PhotoFileType GetPhotoThumbnailType(Texture2D texPack)
    {
      return this.GetPhotoThumbnailType(TexturePack.BlockTextureSize(texPack) == 64);
    }
  }
}
