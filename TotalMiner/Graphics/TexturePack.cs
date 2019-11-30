// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.TexturePack
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.IO;

namespace StudioForge.TotalMiner.Graphics
{
  internal class TexturePack : ITMTexturePack
  {
    private Dictionary<Block, Color[]> leafColorData = new Dictionary<Block, Color[]>();
    public string Name;
    public Texture2D BlockTexture;
    public Texture2D BlockTextureLOD;
    public Texture2D ItemTexture;
    public Texture2D LightMapTexture;
    public Texture2D NightLightMapTexture;
    public Rectangle[] BlockSrcRects;
    public Rectangle[] BlockSrcRectsSD;
    public Rectangle[] ItemSrcRects;
    public Vector2[] TexCoords1;
    public Vector2[] TexCoords2;
    public Vector2[] TexCoords3;
    public Vector2[] TexCoords4;
    public int[,] TexOffsets;
    public ContentManager Content;
    public Color SkyColor;
    public Color LowSkyColor;
    public Color SeaColor;
    public Color WaterColor;
    public Color LavaColor;
    public Color CloudColor;
    public Color TeleportColor;
    public Vector3 LanturnColor;
    public int TorchSpriteWidth;
    public int TorchSpriteHeight;
    public bool NeedLightMap;
    private Color[] singleItemColorData;
    private Color[] singleBlockColorData;
    private Color[] singleLastColorData;
    private Item lastColorDataItemID;

    string ITMTexturePack.Name
    {
      get
      {
        return this.Name;
      }
    }

    Texture2D ITMTexturePack.BlockTexture
    {
      get
      {
        return this.BlockTexture;
      }
    }

    Texture2D ITMTexturePack.BlockTextureLOD
    {
      get
      {
        return this.BlockTextureLOD;
      }
    }

    Texture2D ITMTexturePack.ItemTexture
    {
      get
      {
        return this.ItemTexture;
      }
    }

    Texture2D ITMTexturePack.LightMapTexture
    {
      get
      {
        return this.LightMapTexture;
      }
    }

    Texture2D ITMTexturePack.NightLightMapTexture
    {
      get
      {
        return this.NightLightMapTexture;
      }
    }

    public int BlockTextureSize()
    {
      if (this.BlockTexture == null)
        return 16;
      return this.BlockTexture.Width / 32;
    }

    public static int BlockTextureSize(Texture2D tex)
    {
      if (tex == null)
        return 16;
      return tex.Width / 32;
    }

    public int ItemTextureSize()
    {
      return this.BlockTextureSize() != 16 ? 32 : 16;
    }

    public static int BlockTexturesPerRow()
    {
      return 32;
    }

    public static int BlockTexturesPerRow(Texture2D texPack)
    {
      return 32;
    }

    public Texture2D GetTexureForItem(Item itemID)
    {
      if (itemID > Item.zLastBlockID)
        return this.ItemTexture;
      return this.BlockTexture;
    }

    public byte GetBedRockID()
    {
      switch (Globals2.GameProperties == null || Globals2.GameProperties.SaveGame == null ? Item.Bedrock : Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock)
      {
        case Item.SkyWorld:
        case Item.SpaceWorld:
          return 125;
        default:
          return 29;
      }
    }

    public void LoadTexturePack()
    {
      this.BlockSrcRects = this.BuildBlockSrcRects(this.BlockTexture);
      this.BlockSrcRectsSD = this.BlockTextureLOD == null || this.BlockTextureLOD == this.BlockTexture ? this.BlockSrcRects : this.BuildBlockSrcRects(this.BlockTextureLOD);
      this.ItemSrcRects = this.BuildItemSrcRects();
      this.LoadColorData();
      this.CalcTorchSpriteWidth();
      this.InitTexCoordsAndOffsets();
      this.LoadModTextures();
      this.NeedLightMap = true;
    }

    protected Rectangle[] BuildBlockSrcRects(Texture2D texture)
    {
      int num1 = TexturePack.BlockTextureSize(texture);
      int num2 = texture != null ? texture.Width : num1 * 32;
      Rectangle[] rectangleArray = new Rectangle[256];
      Rectangle rectangle = new Rectangle(-num1, 0, num1, num1);
      for (int index = 0; index < 256; ++index)
      {
        rectangle.X += num1;
        if (rectangle.X >= num2)
        {
          rectangle.X = 0;
          rectangle.Y += num1;
        }
        rectangleArray[index] = rectangle;
      }
      return rectangleArray;
    }

    protected Rectangle[] BuildItemSrcRects()
    {
      int num1 = this.ItemTextureSize();
      int num2 = this.ItemTexture != null ? this.ItemTexture.Width : num1 * 32;
      int length = Globals1.ItemData.Length - 256;
      Rectangle[] rectangleArray = new Rectangle[length];
      Rectangle rectangle1 = new Rectangle(0, 0, num1, num1);
      if (length > 338)
      {
        int num3 = num2 / num1;
        if (length / num3 * num1 > 4096)
        {
          Texture2D texture2D = new Texture2D(CoreGlobals.GraphicsDevice, 4096, (length * num1 / 4096 + 1) * num1, false, SurfaceFormat.Color);
          Color[] data = new Color[num1 * num1];
          Rectangle rectangle2 = new Rectangle(0, 0, num1, num1);
          for (rectangle1.Y = 0; rectangle1.Y < this.ItemTexture.Height; rectangle1.Y += num1)
          {
            for (rectangle1.X = 0; rectangle1.X < this.ItemTexture.Width; rectangle1.X += num1)
            {
              this.ItemTexture.GetData<Color>(0, new Rectangle?(rectangle1), data, 0, data.Length);
              texture2D.SetData<Color>(0, new Rectangle?(rectangle2), data, 0, data.Length);
              rectangle2.X += num1;
              if (rectangle2.X >= texture2D.Width)
              {
                rectangle2.X = 0;
                rectangle2.Y += num1;
              }
            }
          }
          num2 = 4096;
          Texture2D itemTexture = this.ItemTexture;
          this.ItemTexture = texture2D;
          itemTexture.Dispose();
        }
      }
      rectangle1.X = -num1;
      rectangle1.Y = 0;
      for (int index = 0; index < length; ++index)
      {
        rectangle1.X += num1;
        if (rectangle1.X >= num2)
        {
          rectangle1.X = 0;
          rectangle1.Y += num1;
        }
        rectangleArray[index] = rectangle1;
      }
      return rectangleArray;
    }

    public Rectangle ItemSrcRect(Item itemID)
    {
      int index1 = (int) itemID;
      if (index1 < 256)
        return this.BlockSrcRects[index1];
      int index2 = index1 - 256;
      if (index2 < this.ItemSrcRects.Length)
        return this.ItemSrcRects[index2];
      return new Rectangle();
    }

    public Vector4 ConvertTexPackRectToVector4(Texture2D texPack, Rectangle texRect)
    {
      return new Vector4()
      {
        X = (float) texRect.X / (float) texPack.Width,
        Y = (float) texRect.Y / (float) texPack.Height,
        W = (float) (texRect.X + texRect.Width) / (float) texPack.Width,
        Z = (float) (texRect.Y + texRect.Height) / (float) texPack.Height
      };
    }

    private void LoadColorData()
    {
      this.lastColorDataItemID = Item.None;
      this.singleItemColorData = new Color[this.ItemTextureSize() * this.ItemTextureSize()];
      this.singleBlockColorData = new Color[this.BlockTextureSize() * this.BlockTextureSize()];
      Rectangle rectangle = new Rectangle();
      int num = this.BlockSrcRects[(int) byte.MaxValue].Y + this.BlockSrcRects[(int) byte.MaxValue].Height;
      Color[] data = new Color[1];
      rectangle.Width = rectangle.Height = 1;
      rectangle.X = this.BlockTextureSize() * 19 + 1;
      rectangle.Y = num + 1;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.WaterColor = data[0];
      rectangle.X = this.BlockTextureSize() * 20 + 1;
      rectangle.Y = num + 1;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.LavaColor = data[0];
      rectangle.X = this.BlockSrcRects[10].X + 5;
      rectangle.Y = this.BlockSrcRects[10].Y + 5;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.CloudColor = data[0];
      rectangle.X = this.BlockSrcRects[53].X + 5;
      rectangle.Y = this.BlockSrcRects[53].Y + 5;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.TeleportColor = data[0];
      rectangle.X = this.BlockSrcRects[0].X + 1;
      rectangle.Y = this.BlockSrcRects[0].Y;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.SkyColor = data[0];
      rectangle.Y = this.BlockSrcRects[0].Y + (int) ((double) this.BlockTextureSize() * 0.400000005960464);
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.LowSkyColor = data[0];
      rectangle.Y = this.BlockSrcRects[0].Y + this.BlockTextureSize() - 1;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.SeaColor = data[0];
      rectangle.X = this.BlockSrcRects[0].X;
      rectangle.Y = this.BlockSrcRects[(int) byte.MaxValue].Y + this.BlockTextureSize() * 2 - 1;
      this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, 1);
      this.LanturnColor = data[0].ToVector3();
      this.leafColorData.Clear();
      this.AddLeafColorData(Block.Leaves);
      this.AddLeafColorData(Block.PineLeaves);
      this.AddLeafColorData(Block.MapleLeaves);
    }

    public Color GetLeafColor(GameInstance instance, Block blockID)
    {
      Color[] colorArray;
      if (!this.leafColorData.TryGetValue(blockID, out colorArray))
        return Color.Green;
      Color color;
      do
      {
        color = colorArray[instance.Random.Next(colorArray.Length)];
      }
      while (color.A != byte.MaxValue);
      return color;
    }

    private void AddLeafColorData(Block blockID)
    {
      Rectangle blockSrcRect = this.BlockSrcRects[(int) blockID];
      if (blockSrcRect.Width > 16)
      {
        blockSrcRect.X += 24;
        blockSrcRect.Y += 24;
        blockSrcRect.Width = blockSrcRect.Height = 16;
      }
      Color[] data = new Color[256];
      this.BlockTexture.GetData<Color>(0, new Rectangle?(blockSrcRect), data, 0, 256);
      this.leafColorData.Add(blockID, data);
    }

    private void CalcTorchSpriteWidth()
    {
      Color[] itemColorData = this.GetItemColorData(Item.Torch);
      int num1 = this.BlockTextureSize();
      int num2 = num1;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        int num3 = -1;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          int index3 = index2 + index1 * num1;
          if (itemColorData[index3].A == (byte) 0)
          {
            if (num3 >= 0)
            {
              int num4 = index2 - num3;
              if (num4 < num2)
                num2 = num4;
            }
          }
          else if (num3 == -1)
            num3 = index2;
        }
      }
      this.TorchSpriteWidth = num2;
      for (int index1 = num1 - 1; index1 > 0; --index1)
      {
        this.TorchSpriteHeight = index1 + 1;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          if (itemColorData[index2 + index1 * num1].A != (byte) 0)
            return;
        }
      }
    }

    public Color[] GetBlockColorData(Block blockID)
    {
      Rectangle rectangle = this.BlockTextureSize() > 16 ? this.BlockSrcRects[(int) blockID] : this.BlockSrcRectsSD[(int) blockID];
      Color[] data = new Color[rectangle.Width * rectangle.Height];
      try
      {
        this.BlockTexture.GetData<Color>(0, new Rectangle?(rectangle), data, 0, rectangle.Width * rectangle.Height);
      }
      catch (OutOfMemoryException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(115, (Exception) ex);
        return (Color[]) null;
      }
      return data;
    }

    public Color[] GetItemColorData(Item itemID)
    {
      if ((itemID != this.lastColorDataItemID || this.singleLastColorData == null) && itemID < (Item) Globals1.ItemData.Length)
      {
        Rectangle rectangle = this.ItemSrcRect(itemID);
        this.singleLastColorData = itemID < Item.Hand ? this.singleBlockColorData : this.singleItemColorData;
        try
        {
          this.GetTexureForItem(itemID).GetData<Color>(0, new Rectangle?(rectangle), this.singleLastColorData, 0, rectangle.Width * rectangle.Height);
          this.lastColorDataItemID = itemID;
        }
        catch (OutOfMemoryException ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(115, (Exception) ex);
          return (Color[]) null;
        }
      }
      return this.singleLastColorData;
    }

    public void LoadLightMap()
    {
      this.NeedLightMap = false;
      int y = this.BlockSrcRects[(int) byte.MaxValue].Y + this.BlockSrcRects[(int) byte.MaxValue].Height;
      int width1 = 128;
      int height1 = 128;
      int width2 = 132;
      int height2 = 132;
      this.LightMapTexture = new Texture2D(CoreGlobals.GraphicsDevice, width1, height1);
      this.NightLightMapTexture = new Texture2D(CoreGlobals.GraphicsDevice, width1, height1);
      Rectangle rectangle = new Rectangle(0, y, this.BlockTextureSize(), this.BlockTextureSize());
      ++rectangle.Y;
      --rectangle.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width2, height2);
      SpriteBatchSafe spriteBatch = CoreGlobals.SpriteBatch;
      RenderTarget2D renderTarget = TotalMinerGame.Instance.CreateRenderTarget(width2, height2, false);
      CoreGlobals.GraphicsDevice.SetRenderTarget(renderTarget);
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
      spriteBatch.Draw(this.BlockTexture, destinationRectangle, new Rectangle?(rectangle), Color.White);
      spriteBatch.End();
      CoreGlobals.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      Color[] data = new Color[width1 * height1];
      renderTarget.GetData<Color>(0, new Rectangle?(new Rectangle(0, 0, width1, height1)), data, 0, width1 * height1);
      this.LightMapTexture.SetData<Color>(data);
      CoreGlobals.GraphicsDevice.SetRenderTarget(renderTarget);
      rectangle.X += this.BlockTextureSize() + 1;
      --rectangle.Width;
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
      spriteBatch.Draw(this.BlockTexture, destinationRectangle, new Rectangle?(rectangle), Color.White);
      spriteBatch.End();
      CoreGlobals.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      renderTarget.GetData<Color>(0, new Rectangle?(new Rectangle(0, 0, width1, height1)), data, 0, width1 * height1);
      this.NightLightMapTexture.SetData<Color>(data);
      renderTarget.Dispose();
      GraphicStatics.SetLightMaps(this.LightMapTexture, this.NightLightMapTexture);
    }

    private void InitTexCoordsAndOffsets()
    {
      if (this.BlockTexture == null)
        return;
      int num1 = this.BlockTextureSize();
      int num2 = 256 + TexturePack.BlockTexturesPerRow() * 5;
      int num3 = 16;
      int length = num2 + num3;
      this.TexCoords1 = new Vector2[length];
      this.TexCoords2 = new Vector2[length];
      this.TexCoords3 = new Vector2[length];
      this.TexCoords4 = new Vector2[length];
      float num4 = (float) num1 / (float) this.BlockTexture.Width;
      float num5 = (float) num1 / (float) this.BlockTexture.Height;
      float num6 = (float) ((double) num4 / 64.0 * 0.150000005960464);
      float num7 = (float) ((double) num5 / 64.0 * 0.150000005960464);
      float num8 = num6;
      float num9 = num7;
      float num10 = 0.0f;
      float num11 = 0.0f;
      int num12 = 0;
      for (int index1 = 0; index1 < length - num3; ++index1)
      {
        this.TexCoords1[index1] = new Vector2(num10 + num8, num11 + num9);
        this.TexCoords2[index1] = new Vector2(num10 + num4 - num6, num11 + num9);
        this.TexCoords3[index1] = new Vector2(num10 + num8, num11 + num5 - num7);
        this.TexCoords4[index1] = new Vector2(num10 + num4 - num6, num11 + num5 - num7);
        if (index1 >= 304 && index1 < 320)
        {
          int index2 = length - num3 + (index1 - 304);
          this.TexCoords1[index2] = new Vector2(num10 + num4 * 0.25f, num11 + num5 * 0.25f);
          this.TexCoords2[index2] = new Vector2(num10 + num4 * 0.75f, num11 + num5 * 0.25f);
          this.TexCoords3[index2] = new Vector2(num10 + num4 * 0.25f, num11 + num5 * 0.75f);
          this.TexCoords4[index2] = new Vector2(num10 + num4 * 0.75f, num11 + num5 * 0.75f);
        }
        num12 += num1;
        num10 += num4;
        if (num12 == this.BlockTexture.Width)
        {
          num12 = 0;
          num10 = 0.0f;
          num11 += num5;
        }
      }
      this.TexOffsets = new int[256, 7];
      int num13 = 256;
      int num14 = TexturePack.BlockTexturesPerRow();
      int num15 = num13 + num14;
      int num16 = num15 + num14 + num14;
      for (int index1 = 0; index1 < 256; ++index1)
      {
        for (int index2 = 0; index2 < 7; ++index2)
          this.TexOffsets[index1, index2] = index1;
      }
      this.TexOffsets[0, 4] = num13 + 3;
      this.TexOffsets[1, 0] = num13 + 4;
      this.TexOffsets[1, 1] = num13 + 4;
      this.TexOffsets[1, 2] = num13 + 4;
      this.TexOffsets[1, 3] = num13 + 4;
      this.TexOffsets[1, 5] = 2;
      this.TexOffsets[161, 0] = num13 + 4;
      this.TexOffsets[161, 1] = num13 + 4;
      this.TexOffsets[161, 2] = num13 + 4;
      this.TexOffsets[161, 3] = num13 + 4;
      this.TexOffsets[161, 5] = 2;
      this.TexOffsets[79, 0] = num13 + 4;
      this.TexOffsets[79, 1] = num13 + 4;
      this.TexOffsets[79, 2] = num13 + 4;
      this.TexOffsets[79, 3] = num13 + 4;
      this.TexOffsets[79, 5] = 2;
      this.TexOffsets[126, 4] = 1;
      this.TexOffsets[126, 5] = 0;
      this.TexOffsets[5, 4] = num13 + 5;
      this.TexOffsets[5, 5] = num13 + 5;
      this.TexOffsets[179, 4] = num15 + 10;
      this.TexOffsets[179, 5] = num15 + 10;
      this.TexOffsets[11, 4] = num16 + 14;
      this.TexOffsets[11, 5] = num16 + 18;
      this.TexOffsets[11, 0] = num16 + 19;
      int index3 = num16 + 19;
      Vector2 vector2_1 = this.TexCoords1[index3];
      Vector2 vector2_2 = this.TexCoords4[index3];
      float num17 = (float) (((double) vector2_2.X - (double) vector2_1.X) * 0.5);
      float num18 = (float) (((double) vector2_2.Y - (double) vector2_1.Y) * 0.5);
      this.TexCoords1[index3].X += num17;
      this.TexCoords1[index3].Y += num18;
      this.TexCoords2[index3].X += num17;
      this.TexCoords2[index3].Y += num18;
      this.TexCoords3[index3].X += num17;
      this.TexCoords3[index3].Y += num18;
      this.TexCoords4[index3].X += num17;
      this.TexCoords4[index3].Y += num18;
      int index4 = index3 - 1;
      this.TexCoords1[index4].Y += num18;
      this.TexCoords2[index4].Y += num18;
      this.TexCoords3[index4].Y += num18;
      this.TexCoords4[index4].Y += num18;
      this.TexOffsets[11, 1] = num13 + 19;
      this.TexOffsets[11, 2] = num13 + 19;
      this.TexOffsets[11, 3] = num13 + 19;
      this.TexOffsets[13, 4] = num16 + 16;
      this.TexOffsets[13, 5] = num16 + 21;
      this.TexOffsets[13, 0] = num16 + 22;
      int index5 = num16 + 22;
      Vector2 vector2_3 = this.TexCoords1[index5];
      Vector2 vector2_4 = this.TexCoords4[index5];
      float num19 = (float) (((double) vector2_4.X - (double) vector2_3.X) * 0.5);
      float num20 = (float) (((double) vector2_4.Y - (double) vector2_3.Y) * 0.5);
      this.TexCoords1[index5].X += num19;
      this.TexCoords1[index5].Y += num20;
      this.TexCoords2[index5].X += num19;
      this.TexCoords2[index5].Y += num20;
      this.TexCoords3[index5].X += num19;
      this.TexCoords3[index5].Y += num20;
      this.TexCoords4[index5].X += num19;
      this.TexCoords4[index5].Y += num20;
      int index6 = index5 - 1;
      this.TexCoords1[index6].Y += num20;
      this.TexCoords2[index6].Y += num20;
      this.TexCoords3[index6].Y += num20;
      this.TexCoords4[index6].Y += num20;
      this.TexOffsets[13, 1] = num13 + 20;
      this.TexOffsets[13, 2] = num13 + 20;
      this.TexOffsets[13, 3] = num13 + 20;
      this.TexOffsets[54, 4] = num13 + 6;
      this.TexOffsets[54, 5] = num13 + 6;
      this.TexOffsets[55, 4] = num13 + 7;
      this.TexOffsets[55, 5] = num13 + 7;
      this.TexOffsets[48, 1] = num13 + 9;
      this.TexOffsets[48, 2] = num13 + 10;
      this.TexOffsets[48, 3] = num13 + 11;
      this.TexOffsets[48, 4] = num13 + 8;
      this.TexOffsets[48, 5] = num13 + 8;
      this.TexOffsets[48, 6] = num13 + 8;
      this.TexOffsets[50, 4] = num13 + 12;
      this.TexOffsets[50, 5] = num13 + 12;
      this.TexOffsets[50, 6] = num13 + 12;
      this.TexOffsets[49, 4] = num13 + 13;
      this.TexOffsets[49, 5] = num13 + 13;
      this.TexOffsets[49, 6] = num13 + 13;
      this.TexOffsets[133, 4] = num13 + 13;
      this.TexOffsets[133, 5] = num13 + 13;
      this.TexOffsets[65, 1] = num13 + 16;
      this.TexOffsets[65, 3] = num13 + 16;
      this.TexOffsets[65, 4] = num13 + 14;
      this.TexOffsets[65, 5] = num13 + 14;
      this.TexOffsets[64, 1] = num13 + 16;
      this.TexOffsets[64, 3] = num13 + 16;
      this.TexOffsets[64, 4] = num13 + 15;
      this.TexOffsets[64, 5] = num13 + 15;
      this.TexOffsets[69, 4] = num13 + 17;
      this.TexOffsets[69, 5] = num13 + 17;
      this.TexOffsets[45, 4] = 7;
      this.TexOffsets[45, 5] = 7;
      this.TexOffsets[107, 4] = num13 + 18;
      this.TexOffsets[107, 5] = num13 + 18;
      this.TexOffsets[132, 4] = num13 + 21;
      this.TexOffsets[132, 5] = num13 + 21;
      this.TexOffsets[132, 6] = num13 + 21;
      this.TexOffsets[135, 0] = num13 + 29;
      this.TexOffsets[135, 1] = num13 + 30;
      this.TexOffsets[135, 3] = num13 + 30;
      this.TexOffsets[136, 1] = num13 + 31;
      this.TexOffsets[136, 2] = num15;
      this.TexOffsets[136, 3] = num13 + 31;
      this.TexOffsets[153, 1] = 9;
      this.TexOffsets[153, 2] = 9;
      this.TexOffsets[153, 3] = 9;
      this.TexOffsets[153, 4] = 9;
      this.TexOffsets[153, 5] = 9;
      this.TexOffsets[160, 1] = 7;
      this.TexOffsets[160, 2] = 7;
      this.TexOffsets[160, 3] = 7;
      this.TexOffsets[160, 4] = 7;
      this.TexOffsets[160, 5] = 7;
      this.TexOffsets[121, 1] = num15 + 7;
      this.TexOffsets[121, 3] = num15 + 7;
      this.TexOffsets[114, 4] = 238;
      this.TexOffsets[114, 5] = 238;
      this.TexOffsets[167, 4] = num15 + 8;
      this.TexOffsets[167, 5] = num15 + 8;
      this.TexOffsets[167, 6] = num15 + 8;
      this.TexOffsets[169, 2] = num15 + 9;
      this.TexOffsets[173, 0] = 2;
      this.TexOffsets[173, 1] = 2;
      this.TexOffsets[173, 2] = 2;
      this.TexOffsets[173, 3] = 2;
      this.TexOffsets[173, 5] = 2;
    }

    private void LoadModTextures()
    {
      Rectangle itemSrcRect = this.ItemSrcRects[this.ItemSrcRects.Length - 1];
      if (itemSrcRect.Y + itemSrcRect.Height > this.ItemTexture.Height)
      {
        Texture2D texture2D = new Texture2D(CoreGlobals.GraphicsDevice, this.ItemTexture.Width, itemSrcRect.Y + itemSrcRect.Height, false, this.ItemTexture.Format);
        Color[] data = new Color[this.ItemTexture.Width * this.ItemTexture.Height];
        this.ItemTexture.GetData<Color>(data);
        texture2D.SetData<Color>(0, new Rectangle?(new Rectangle(0, 0, this.ItemTexture.Width, this.ItemTexture.Height)), data, 0, data.Length);
        this.ItemTexture.Dispose();
        this.ItemTexture = texture2D;
      }
      int num = this.ItemTextureSize();
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        this.LoadModBlockTextures(activeMod, activeMod.LoadTexturePackBlocks(num == 32 ? 64 : num), num);
        this.LoadModItemTextures(activeMod, activeMod.LoadTexturePackItems(num), num);
      }
    }

    private void LoadModBlockTextures(Mod mod, Item[] items, int texSize)
    {
      if (items == null || items.Length <= 0)
        return;
      bool flag = false;
      foreach (Item obj in items)
      {
        if (obj != Item.None && obj < Item.Hand)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      int num = texSize == 32 ? 64 : texSize;
      string path = ModManager.ModsPath + mod.Name + "\\tp_" + num.ToString() + ".png";
      if (!FileSystem.IsFileExist(path))
        return;
      using (Stream stream = FileSystem.OpenRead(path))
      {
        Texture2D srcTex = Texture2D.FromStream(CoreGlobals.GraphicsDevice, stream);
        Rectangle rect = new Rectangle(0, 0, num, num);
        this.LoadModTextures(srcTex, this.BlockTexture, items, ref rect, true, false);
        if (num != 64 || srcTex.Height <= rect.Y || this.BlockTextureLOD == null)
          return;
        rect.Width = rect.Height = 16;
        this.LoadModTextures(srcTex, this.BlockTextureLOD, items, ref rect, true, true);
      }
    }

    private void LoadModItemTextures(Mod mod, Item[] items, int texSize)
    {
      if (items == null || items.Length <= 0)
        return;
      bool flag = false;
      foreach (Item obj in items)
      {
        if (obj >= Item.Hand)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      int num = texSize;
      string path = ModManager.ModsPath + mod.Name + "\\tpi_" + num.ToString() + ".png";
      if (!FileSystem.IsFileExist(path))
        return;
      using (Stream stream = FileSystem.OpenRead(path))
      {
        Texture2D srcTex = Texture2D.FromStream(CoreGlobals.GraphicsDevice, stream);
        Rectangle rect = new Rectangle(0, 0, num, num);
        this.LoadModTextures(srcTex, this.ItemTexture, items, ref rect, false, false);
      }
    }

    private void LoadModTextures(
      Texture2D srcTex,
      Texture2D destTex,
      Item[] items,
      ref Rectangle rect,
      bool blocks,
      bool isLOD)
    {
      Color[] data = new Color[rect.Width * rect.Height];
      int width = srcTex.Width;
      if (isLOD)
        width /= 4;
      for (int index = 0; index < items.Length; ++index)
      {
        try
        {
          Item itemID = items[index];
          if (itemID != Item.None)
          {
            if ((blocks ? (itemID < Item.Hand ? 1 : 0) : (itemID >= Item.Hand ? 1 : 0)) != 0)
            {
              srcTex.GetData<Color>(0, new Rectangle?(rect), data, 0, data.Length);
              Rectangle rectangle = this.ItemSrcRect(itemID);
              if (isLOD)
              {
                rectangle.X /= 4;
                rectangle.Y /= 4;
                rectangle.Width /= 4;
                rectangle.Height /= 4;
              }
              destTex.SetData<Color>(0, new Rectangle?(rectangle), data, 0, data.Length);
            }
          }
        }
        catch (Exception ex)
        {
        }
        rect.X += rect.Width;
        if (rect.X >= width)
        {
          rect.X = 0;
          rect.Y += rect.Height;
        }
      }
    }

    private enum TexurePackType
    {
      None,
      SD,
      HD,
    }
  }
}
