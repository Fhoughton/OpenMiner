// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ItemModelManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using System;

namespace StudioForge.TotalMiner.Graphics
{
  internal static class ItemModelManager
  {
    public static ItemModelCache[] Cache = new ItemModelCache[Globals1.ItemData.Length];
    private static Vector3[] normals = new Vector3[6]
    {
      Vector3.Left,
      Vector3.Forward,
      Vector3.Right,
      Vector3.Backward,
      Vector3.Up,
      Vector3.Down
    };

    public static void ResetCache()
    {
      if (ItemModelManager.Cache == null || ItemModelManager.Cache.Length >= Globals1.ItemData.Length)
        return;
      foreach (ItemModelCache itemModelCache in ItemModelManager.Cache)
        itemModelCache.Clear();
      ItemModelManager.Cache = new ItemModelCache[Globals1.ItemData.Length];
    }

    public static void ClearItemCache()
    {
      if (ItemModelManager.Cache == null)
        return;
      for (int index = 256; index < ItemModelManager.Cache.Length; ++index)
        ItemModelManager.Cache[index].Clear();
    }

    public static bool UseCube(Item itemID)
    {
      return VoxelMeshBuilder.UseCube(itemID);
    }

    public static void BuildModel(Item itemID)
    {
      ItemModelCache cache = ItemModelManager.Cache[(int) itemID];
      if (cache.VertexCount > 0)
        return;
      cache.VertexCount = 0;
      cache.Vertices = new CustomArray<VertexItemBlock>();
      cache.ItemBlockSize = (float) (0.0199999995529652 * (16.0 / (double) GraphicStatics.TexturePack.ItemTextureSize()));
      if (ItemModelManager.UseCube(itemID))
      {
        cache.Scale = 0.25f;
        ItemModelManager.BuildFromBlock(itemID, ref cache);
      }
      else
      {
        cache.Scale = 1f;
        ItemModelManager.BuildFromIcon(itemID, ref cache);
      }
      CoreGlobals.GraphicsDevice.SetVertexBuffer((VertexBuffer) null);
      cache.VertexBuffer = new VertexBuffer(CoreGlobals.GraphicsDevice, typeof (VertexItemBlock), cache.Vertices.Count, BufferUsage.WriteOnly);
      cache.VertexBuffer.SetData<VertexItemBlock>(cache.Vertices.Array, 0, cache.Vertices.Count);
      cache.VertexCount = cache.Vertices.Count;
      ItemModelManager.Cache[(int) itemID] = cache;
    }

    private static void BuildFromIcon(Item itemID, ref ItemModelCache cache)
    {
      Color[] itemColorData = GraphicStatics.TexturePack.GetItemColorData(itemID);
      if (itemColorData == null)
        return;
      int texSize = (int) Math.Sqrt((double) itemColorData.Length);
      float scale = texSize == 64 ? 0.5f : 1f;
      int depth = ItemModelManager.GetDepth(itemID, texSize);
      int num1 = texSize;
      int num2 = -1;
      int num3 = texSize;
      int num4 = -1;
      for (int z = 0; z < depth; ++z)
      {
        for (int index1 = 0; index1 < texSize; ++index1)
        {
          for (int x = 0; x < texSize; ++x)
          {
            int index2 = x + index1 * texSize;
            if (itemColorData[index2] != Color.Transparent)
            {
              bool forwardFace = z == 0;
              bool leftFace = x == 0 || itemColorData[index2 - 1].A < (byte) 10;
              bool backFace = z == depth - 1;
              bool rightFace = x == texSize - 1 || itemColorData[index2 + 1].A < (byte) 10;
              bool upFace = index1 == 0 || itemColorData[index2 - texSize].A < (byte) 10;
              bool downFace = index1 == texSize - 1 || itemColorData[index2 + texSize].A < (byte) 10;
              ItemModelManager.AddBlock(x, texSize - index1, z, itemColorData[index2], scale, leftFace, forwardFace, rightFace, backFace, upFace, downFace, ref cache);
              if (x < num1)
                num1 = x;
              if (x > num2)
                num2 = x;
              if (index1 < num3)
                num3 = index1;
              if (index1 > num4)
                num4 = index1;
            }
          }
        }
      }
      float num5 = (float) num1 + (float) (num2 - num1) * 0.5f;
      float num6 = (float) texSize - ((float) num3 + (float) (num4 - num3) * 0.5f);
      float itemBlockSize = cache.ItemBlockSize;
      cache.Center = new Vector3(num5 * itemBlockSize * scale, num6 * itemBlockSize * scale, (float) ((double) (depth - 1) * (double) itemBlockSize * 0.5) * scale);
    }

    private static void AddBlock(
      int x,
      int y,
      int z,
      Color color,
      float scale,
      bool leftFace,
      bool forwardFace,
      bool rightFace,
      bool backFace,
      bool upFace,
      bool downFace,
      ref ItemModelCache cache)
    {
      float itemBlockSize = cache.ItemBlockSize;
      float num = itemBlockSize * 0.5f * scale;
      Vector3 vector3_1 = new Vector3((float) x * itemBlockSize * scale, (float) y * itemBlockSize * scale, (float) z * itemBlockSize * scale);
      CustomArray<VertexItemBlock> vertices = cache.Vertices;
      VertexItemBlock t = new VertexItemBlock();
      t.Color = color;
      for (int index = 0; index < ItemModelManager.normals.Length; ++index)
      {
        if (index == 0 && leftFace || index == 1 && forwardFace || (index == 2 && rightFace || index == 3 && backFace) || (index == 4 && upFace || index == 5 && downFace))
        {
          Vector3 normal = ItemModelManager.normals[index];
          Vector3 vector2 = new Vector3(normal.Y, normal.Z, normal.X);
          Vector3 vector3_2 = Vector3.Cross(normal, vector2);
          Vector3 vector3_3 = (normal - vector2 - vector3_2) * num + vector3_1;
          t.Position = new HalfVector4(vector3_3.X, vector3_3.Y, vector3_3.Z, (float) index);
          vertices.Add(t);
          vector3_3 = (normal - vector2 + vector3_2) * num + vector3_1;
          t.Position = new HalfVector4(vector3_3.X, vector3_3.Y, vector3_3.Z, (float) index);
          vertices.Add(t);
          vector3_3 = (normal + vector2 + vector3_2) * num + vector3_1;
          t.Position = new HalfVector4(vector3_3.X, vector3_3.Y, vector3_3.Z, (float) index);
          vertices.Add(t);
          vector3_3 = (normal + vector2 - vector3_2) * num + vector3_1;
          t.Position = new HalfVector4(vector3_3.X, vector3_3.Y, vector3_3.Z, (float) index);
          vertices.Add(t);
        }
      }
    }

    private static int GetDepth(Item itemID, int texSize)
    {
      if (ItemData.IsSubTypeAny(itemID, ItemSubType.Bow | ItemSubType.Arrow))
        return 1;
      texSize = texSize != 32 ? 1 : 2;
      Item obj = itemID;
      if ((uint) obj <= 289U)
      {
        if ((uint) obj <= 256U)
        {
          if (obj != Item.Torch)
          {
            if (obj != Item.Hand)
              goto label_14;
          }
          else
            goto label_13;
        }
        else if (obj != Item.Camera)
        {
          if (obj == Item.GrenadeLauncher)
            return 4 * texSize;
          goto label_14;
        }
      }
      else if ((uint) obj <= 408U)
      {
        if (obj != Item.SledgeHammer && obj != Item.GreenstoneGoldSledgeHammer)
          goto label_14;
      }
      else
      {
        switch (obj)
        {
          case Item.RubyWarHammer:
          case Item.TitaniumWarHammer:
          case Item.GoldenSMG:
          case Item.LaserBlaster:
          case Item.Shotgun:
          case Item.MiniGun:
          case Item.PlasmaRifle:
            goto label_13;
          default:
            goto label_14;
        }
      }
      return 3 * texSize;
label_13:
      return 2 * texSize;
label_14:
      return texSize;
    }

    private static void BuildFromBlock(Item itemID, ref ItemModelCache cache)
    {
      VertexItemBlock t = new VertexItemBlock();
      float scale = cache.Scale;
      float num1 = scale * 0.5f;
      Block block = (Block) itemID;
      int index1 = (int) block;
      CustomArray<VertexItemBlock> vertices = cache.Vertices;
      int index2 = 0;
      Vector2 vector2_1 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index2]];
      Vector2 vector2_2 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index2]];
      Vector3 vector3 = new Vector3(-num1, -num1, -num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index2);
      t.TexCoord = new NormalizedShort2(vector2_1.X, vector2_2.Y);
      vertices.Add(t);
      vector3.Y += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index2);
      t.TexCoord = new NormalizedShort2(vector2_1.X, vector2_1.Y);
      vertices.Add(t);
      vector3.Z += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index2);
      t.TexCoord = new NormalizedShort2(vector2_2.X, vector2_1.Y);
      vertices.Add(t);
      vector3.Y -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index2);
      t.TexCoord = new NormalizedShort2(vector2_2.X, vector2_2.Y);
      vertices.Add(t);
      int index3 = 1;
      Vector2 vector2_3 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index3]];
      Vector2 vector2_4 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index3]];
      vector3 = new Vector3(num1, -num1, -num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index3);
      t.TexCoord = new NormalizedShort2(vector2_3.X, vector2_4.Y);
      vertices.Add(t);
      vector3.Y += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index3);
      t.TexCoord = new NormalizedShort2(vector2_3.X, vector2_3.Y);
      vertices.Add(t);
      vector3.X -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index3);
      t.TexCoord = new NormalizedShort2(vector2_4.X, vector2_3.Y);
      vertices.Add(t);
      vector3.Y -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index3);
      t.TexCoord = new NormalizedShort2(vector2_4.X, vector2_4.Y);
      vertices.Add(t);
      int index4 = 2;
      Vector2 vector2_5 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index4]];
      Vector2 vector2_6 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index4]];
      vector3 = new Vector3(num1, -num1, num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index4);
      t.TexCoord = new NormalizedShort2(vector2_5.X, vector2_6.Y);
      vertices.Add(t);
      vector3.Y += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index4);
      t.TexCoord = new NormalizedShort2(vector2_5.X, vector2_5.Y);
      vertices.Add(t);
      vector3.Z -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index4);
      t.TexCoord = new NormalizedShort2(vector2_6.X, vector2_5.Y);
      vertices.Add(t);
      vector3.Y -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index4);
      t.TexCoord = new NormalizedShort2(vector2_6.X, vector2_6.Y);
      vertices.Add(t);
      int index5 = 3;
      Vector2 vector2_7 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index5]];
      Vector2 vector2_8 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index5]];
      vector3 = new Vector3(-num1, -num1, num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index5);
      t.TexCoord = new NormalizedShort2(vector2_7.X, vector2_8.Y);
      vertices.Add(t);
      vector3.Y += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index5);
      t.TexCoord = new NormalizedShort2(vector2_7.X, vector2_7.Y);
      vertices.Add(t);
      vector3.X += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index5);
      t.TexCoord = new NormalizedShort2(vector2_8.X, vector2_7.Y);
      vertices.Add(t);
      vector3.Y -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index5);
      t.TexCoord = new NormalizedShort2(vector2_8.X, vector2_8.Y);
      vertices.Add(t);
      int num2 = 4;
      int index6 = num2;
      switch (block)
      {
        case Block.OneWayGlass:
        case Block.Painting:
          index6 = 6;
          break;
      }
      Vector2 vector2_9 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index6]];
      Vector2 vector2_10 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index6]];
      vector3 = new Vector3(-num1, num1, num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) num2);
      t.TexCoord = new NormalizedShort2(vector2_9.X, vector2_9.Y);
      vertices.Add(t);
      vector3.Z -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) num2);
      t.TexCoord = new NormalizedShort2(vector2_10.X, vector2_9.Y);
      vertices.Add(t);
      vector3.X += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) num2);
      t.TexCoord = new NormalizedShort2(vector2_10.X, vector2_10.Y);
      vertices.Add(t);
      vector3.Z += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) num2);
      t.TexCoord = new NormalizedShort2(vector2_9.X, vector2_10.Y);
      vertices.Add(t);
      int index7 = 5;
      vector2_9 = MapChunkContent.TexCoords1[MapChunkContent.TexOffsets[index1, index7]];
      vector2_10 = MapChunkContent.TexCoords4[MapChunkContent.TexOffsets[index1, index7]];
      vector3 = new Vector3(-num1, -num1, -num1);
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index7);
      t.TexCoord = new NormalizedShort2(vector2_9.X, vector2_10.Y);
      vertices.Add(t);
      vector3.Z += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index7);
      t.TexCoord = new NormalizedShort2(vector2_9.X, vector2_9.Y);
      vertices.Add(t);
      vector3.X += scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index7);
      t.TexCoord = new NormalizedShort2(vector2_10.X, vector2_9.Y);
      vertices.Add(t);
      vector3.Z -= scale;
      t.Position = new HalfVector4(vector3.X, vector3.Y, vector3.Z, (float) index7);
      t.TexCoord = new NormalizedShort2(vector2_10.X, vector2_10.Y);
      vertices.Add(t);
      cache.Center = new Vector3(0.0f, 0.0f, 0.0f);
    }
  }
}
