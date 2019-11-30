// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.StaticBatchModel
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class StaticBatchModel : IDisposable
  {
    private GraphicsDevice graphicsDevice;
    private int indexChunkSize;
    private int vertexChunkSize;
    private int maxPartSize;
    private IndexElementSize indexElementSize;
    private List<StaticBatchModelPart> modelParts;

    public StaticBatchModel(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, 2048, 0, IndexElementSize.ThirtyTwoBits)
    {
    }

    public StaticBatchModel(
      GraphicsDevice graphicsDevice,
      int vertexChunkSize,
      int maxPartVertexCount,
      IndexElementSize indexElementSize)
    {
      this.graphicsDevice = graphicsDevice;
      this.vertexChunkSize = vertexChunkSize;
      this.maxPartSize = maxPartVertexCount;
      this.indexChunkSize = vertexChunkSize * 3;
      this.indexElementSize = indexElementSize;
      this.modelParts = new List<StaticBatchModelPart>();
    }

    public StaticBatchModelPart[] ModelParts
    {
      get
      {
        return this.modelParts.ToArray();
      }
    }

    public void Draw(StaticBatchModel.GlobalBasicEffectUpdate update)
    {
      if (this.modelParts.Count <= 0)
        return;
      ICamera camera = CoreGlobals.Camera;
      if (camera == null)
        return;
      foreach (StaticBatchModelPart modelPart in this.modelParts)
        modelPart.Draw(camera, update);
    }

    public StaticBatchModel.PartAdded[] AddModel(
      object model,
      Vector3 position,
      Vector3 scale)
    {
      if (model == null)
        return (StaticBatchModel.PartAdded[]) null;
      IModelPart[] modelPartArray = new ModelPartBuilder().BuildParts(model);
      StaticBatchModel.PartAdded[] partAddedArray = new StaticBatchModel.PartAdded[modelPartArray.Length];
      int num = 0;
      foreach (IModelPart modelPart in modelPartArray)
        partAddedArray[num++] = this.FindMatchingPart(modelPart).AddModelPart(modelPart, position, scale);
      return partAddedArray;
    }

    private StaticBatchModelPart FindMatchingPart(IModelPart newPart)
    {
      StaticBatchModelPart staticBatchModelPart = (StaticBatchModelPart) null;
      foreach (StaticBatchModelPart modelPart in this.modelParts)
      {
        if (modelPart.IsMatchAndHasSpace(newPart))
        {
          staticBatchModelPart = modelPart;
          break;
        }
      }
      if (staticBatchModelPart == null)
        this.modelParts.Add(staticBatchModelPart = new StaticBatchModelPart(this.graphicsDevice, this.indexChunkSize, this.vertexChunkSize, this.maxPartSize, this.indexElementSize));
      return staticBatchModelPart;
    }

    public void ClearParts()
    {
      foreach (StaticBatchModelPart modelPart in this.modelParts)
        modelPart.Dispose();
      this.modelParts.Clear();
    }

    public void ChangeTexture(StaticBatchModel.PartAdded[] partsToChange, Texture2D texture)
    {
      this.ChangeTexture(partsToChange, texture, 1f);
    }

    public void ChangeTexture(
      StaticBatchModel.PartAdded[] partsToChange,
      Texture2D texture,
      float texCoordFactorChange)
    {
      foreach (StaticBatchModel.PartAdded partAdded in partsToChange)
      {
        foreach (StaticBatchModelPart modelPart in this.modelParts)
        {
          if (partAdded.Part == modelPart)
          {
            modelPart.Texture = texture;
            if ((double) texCoordFactorChange != 1.0)
              this.ChangeTextureCoordFactor(partAdded, texCoordFactorChange);
          }
        }
      }
    }

    private void ChangeTextureCoordFactor(
      StaticBatchModel.PartAdded partAdded,
      float texCoordFactorChange)
    {
      Vector2 vector2 = new Vector2(texCoordFactorChange);
      VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[partAdded.Part.VertexCount];
      int vertexStride = partAdded.Part.VertexStride;
      int offsetInBytes = partAdded.VertexOffset * vertexStride;
      partAdded.Part.VertexBuffer.GetData<VertexPositionNormalTexture>(offsetInBytes, data, 0, data.Length, vertexStride);
      for (int index = 0; index < data.Length; ++index)
        data[index].TextureCoordinate *= vector2;
      partAdded.Part.VertexBuffer.SetData<VertexPositionNormalTexture>(offsetInBytes, data, 0, data.Length, vertexStride);
    }

    public void Dispose()
    {
      this.ClearParts();
    }

    public delegate void GlobalBasicEffectUpdate(BasicEffect effect);

    public struct PartAdded
    {
      public IModelPart Part;
      public int VertexOffset;
      public int IndexOffset;

      public PartAdded(IModelPart part, int vertexOffset, int indexOffset)
      {
        this.Part = part;
        this.VertexOffset = vertexOffset;
        this.IndexOffset = indexOffset;
      }
    }
  }
}
