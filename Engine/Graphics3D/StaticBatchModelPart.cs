// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.StaticBatchModelPart
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class StaticBatchModelPart : IModelPart, IUnmanagedBuffer, IDisposable
  {
    private int indexCount;
    private int indexChunkSize;
    private int vertexCount;
    private int vertexStride;
    private int vertexChunkSize;
    private int maxVertices;
    private int currentVertexBufferSize;
    private Texture2D texture;
    private BasicEffect effect;
    private IndexBuffer indexBuffer;
    private VertexBuffer vertexBuffer;
    private VertexDeclaration vertexDeclaration;
    private IndexElementSize indexElementSize;
    private GraphicsDevice graphicsDevice;
    private bool depthBufferEnable;
    private bool alphaBlendEnable;
    private bool alphaTestEnable;
    private CullMode cullMode;
    private TextureAddressMode addressU;
    private TextureAddressMode addressV;

    public StaticBatchModelPart(
      GraphicsDevice graphicsDevice,
      int indexChunkSize,
      int vertexChunkSize,
      int maxVertices,
      IndexElementSize indexElementSize)
    {
      this.graphicsDevice = graphicsDevice;
      this.indexChunkSize = indexChunkSize;
      this.vertexChunkSize = vertexChunkSize;
      this.maxVertices = maxVertices;
      this.indexElementSize = indexElementSize;
      this.cullMode = CullMode.CullCounterClockwiseFace;
    }

    public bool IsMatchAndHasSpace(IModelPart part)
    {
      if (this.HasSpaceFor(part.VertexCount) && (this.vertexStride == 0 || part.VertexStride == this.vertexStride) && (this.depthBufferEnable == part.DepthBufferEnable && this.alphaBlendEnable == part.AlphaBlendEnable && this.alphaTestEnable == part.AlphaTestEnable && this.cullMode == part.CullMode))
        return object.ReferenceEquals((object) this.texture, (object) part.Texture) && object.ReferenceEquals((object) this.effect, (object) part.Effect) && object.ReferenceEquals((object) this.vertexDeclaration, (object) part.VertexDeclaration);
      return false;
    }

    public bool HasSpaceFor(int vertices)
    {
      if (this.maxVertices != 0)
        return vertices + this.vertexCount <= this.maxVertices;
      return true;
    }

    public void Draw(ICamera camera, StaticBatchModel.GlobalBasicEffectUpdate update)
    {
      if (this.vertexCount <= 0)
        return;
      this.SetEffectStates(camera);
      if (update != null)
        update(this.effect);
      for (int index = 0; index < this.effect.CurrentTechnique.Passes.Count; ++index)
      {
        this.effect.CurrentTechnique.Passes[index].Apply();
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertexCount, 0, this.indexCount / 3);
      }
      CoreGlobals.FrameRateCounter.DrawCalls += this.effect.CurrentTechnique.Passes.Count;
      CoreGlobals.FrameRateCounter.Primitives += this.indexCount / 3 * this.effect.CurrentTechnique.Passes.Count;
    }

    private void SetEffectStates(ICamera camera)
    {
      this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);
      this.graphicsDevice.Indices = this.indexBuffer;
      this.graphicsDevice.SamplerStates[0].AddressU = this.addressU;
      this.graphicsDevice.SamplerStates[0].AddressV = this.addressV;
      this.effect.View = camera.ViewMatrix;
      this.effect.Projection = camera.ProjectionMatrix;
      this.effect.World = Matrix.Identity;
      this.effect.TextureEnabled = (this.effect.Texture = this.texture) != null;
      if (!(this.effect.FogEnabled = camera.FogEnabled))
        return;
      this.effect.FogColor = camera.LenseColor.ToVector3();
      this.effect.FogStart = camera.FogStart;
      this.effect.FogEnd = camera.FarClip;
    }

    public StaticBatchModel.PartAdded AddModelPart(
      IModelPart part,
      Vector3 position,
      Vector3 scale)
    {
      if (!this.HasSpaceFor(part.VertexCount))
        throw new ArgumentOutOfRangeException("Vertex Buffer size limit reached");
      StaticBatchModel.PartAdded partAdded = new StaticBatchModel.PartAdded((IModelPart) this, this.vertexCount, this.indexCount);
      this.AddPartToIndexBuffer(part);
      this.AddPartToVertexBuffer(part, position, scale);
      this.SetRenderStates(part);
      return partAdded;
    }

    private void AddPartToIndexBuffer(IModelPart part)
    {
      int[] currentIndices = this.GetCurrentIndices(part);
      this.InsertAndOffsetNewIndices(currentIndices, part);
      if (this.indexElementSize == IndexElementSize.ThirtyTwoBits)
        this.indexBuffer.SetData<int>(currentIndices);
      else
        this.indexBuffer.SetData<short>(this.SixteenBit(currentIndices));
      this.indexCount += part.IndexCount;
    }

    private void InsertAndOffsetNewIndices(int[] indices, IModelPart part)
    {
      this.GetExistingIndexData(indices, part);
      for (int indexCount = this.indexCount; indexCount < indices.Length; ++indexCount)
        indices[indexCount] += part.IndexOffset + this.vertexCount;
    }

    private int[] GetCurrentIndices(IModelPart part)
    {
      int length = this.indexCount + part.IndexCount;
      int[] indexData = new int[length];
      if (this.indexBuffer != null)
      {
        this.GetExistingIndexData(indexData, 0, this.indexCount);
        int indexCount = this.indexBuffer.IndexCount;
        if (length > indexCount)
        {
          this.indexBuffer.Dispose();
          this.indexBuffer = (IndexBuffer) null;
        }
      }
      if (this.indexBuffer == null)
        this.indexBuffer = new IndexBuffer(this.graphicsDevice, this.indexElementSize, length + this.indexChunkSize, BufferUsage.None);
      return indexData;
    }

    private void GetExistingIndexData(int[] indexData, int index, int indexCount)
    {
      if (this.indexElementSize == IndexElementSize.ThirtyTwoBits)
      {
        this.indexBuffer.GetData<int>(indexData, index, indexCount);
      }
      else
      {
        short[] numArray = new short[indexData.Length];
        this.indexBuffer.GetData<short>(numArray, index, indexCount);
        this.ThirtyTwoBit(numArray, indexData);
      }
    }

    private void GetExistingIndexData(int[] indices, IModelPart part)
    {
      if (this.indexElementSize == IndexElementSize.ThirtyTwoBits)
      {
        part.IndexBuffer.GetData<int>(part.IndexOffset, indices, this.indexCount, part.IndexCount);
      }
      else
      {
        short[] numArray = new short[indices.Length];
        part.IndexBuffer.GetData<short>(part.IndexOffset, numArray, this.indexCount, part.IndexCount);
        this.ThirtyTwoBit(numArray, indices);
      }
    }

    private short[] SixteenBit(int[] indices)
    {
      short[] numArray = new short[indices.Length];
      for (int index = 0; index < indices.Length; ++index)
        numArray[index] = (short) indices[index];
      return numArray;
    }

    private int[] ThirtyTwoBit(short[] indices)
    {
      int[] numArray = new int[indices.Length];
      for (int index = 0; index < indices.Length; ++index)
        numArray[index] = (int) indices[index];
      return numArray;
    }

    private void ThirtyTwoBit(short[] indices, int[] result)
    {
      for (int index = 0; index < indices.Length; ++index)
        result[index] = (int) indices[index];
    }

    private void AddPartToVertexBuffer(IModelPart part, Vector3 position, Vector3 scale)
    {
      this.CreateOrExpandVertexBufferIfNeeded(part);
      VertexPositionNormalTexture[] convertedVertexData = this.GetConvertedVertexData(part);
      this.TransformNewVertexData(convertedVertexData, position, scale);
      this.InsertNewVertexDataIntoBuffer(part, convertedVertexData);
    }

    private void CreateOrExpandVertexBufferIfNeeded(IModelPart part)
    {
      if (this.vertexBuffer == null)
      {
        this.CreateVertexBuffer(part);
      }
      else
      {
        if (this.vertexCount + part.VertexCount <= this.currentVertexBufferSize)
          return;
        this.ExpandVertexBuffer(this.vertexCount + part.VertexCount + this.vertexChunkSize);
      }
    }

    private void CreateVertexBuffer(IModelPart part)
    {
      this.vertexDeclaration = part.VertexDeclaration;
      this.vertexStride = this.vertexDeclaration.VertexStride;
      this.currentVertexBufferSize = part.VertexCount + this.vertexChunkSize;
      if (this.maxVertices > 0 && this.currentVertexBufferSize > this.maxVertices)
        this.currentVertexBufferSize = this.maxVertices;
      this.vertexBuffer = new VertexBuffer(this.graphicsDevice, this.vertexDeclaration, this.currentVertexBufferSize, BufferUsage.None);
    }

    private void ExpandVertexBuffer(int newVertexCount)
    {
      if (newVertexCount > this.maxVertices)
        newVertexCount = this.maxVertices;
      VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[this.vertexCount];
      this.vertexBuffer.GetData<VertexPositionNormalTexture>(data);
      this.vertexBuffer.Dispose();
      this.vertexBuffer = new VertexBuffer(this.graphicsDevice, this.vertexDeclaration, newVertexCount, BufferUsage.None);
      this.vertexBuffer.SetData<VertexPositionNormalTexture>(data);
      this.currentVertexBufferSize = newVertexCount;
    }

    private VertexPositionNormalTexture[] GetConvertedVertexData(
      IModelPart part)
    {
      VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[part.VertexCount];
      if (part.VertexDeclaration == this.vertexDeclaration)
      {
        part.VertexBuffer.GetData<VertexPositionNormalTexture>(data, part.VertexOffset, part.VertexCount);
      }
      else
      {
        VertexBuffer vertexBuffer = new VertexConverter().ConvertVertexBuffer(this.graphicsDevice, part, this.vertexDeclaration);
        vertexBuffer.GetData<VertexPositionNormalTexture>(data);
        vertexBuffer.Dispose();
      }
      return data;
    }

    private void TransformNewVertexData(
      VertexPositionNormalTexture[] vertexData,
      Vector3 position,
      Vector3 scale)
    {
      Matrix matrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
      for (int index = 0; index < vertexData.Length; ++index)
        vertexData[index].Position = Vector3.Transform(vertexData[index].Position, matrix);
    }

    private void InsertNewVertexDataIntoBuffer(
      IModelPart part,
      VertexPositionNormalTexture[] newVertexData)
    {
      this.vertexBuffer.SetData<VertexPositionNormalTexture>(this.vertexCount * this.vertexStride, newVertexData, 0, part.VertexCount, this.vertexStride);
      this.vertexCount += part.VertexCount;
    }

    private void SetRenderStates(IModelPart part)
    {
      this.effect = part.Effect as BasicEffect;
      if (this.effect != null)
        this.effect.World = Matrix.Identity;
      this.texture = part.Texture;
      this.depthBufferEnable = part.DepthBufferEnable;
      this.alphaBlendEnable = part.AlphaBlendEnable;
      this.alphaTestEnable = part.AlphaTestEnable;
      this.addressU = part.AddressU;
      this.addressV = part.AddressV;
    }

    IndexBuffer IModelPart.IndexBuffer
    {
      get
      {
        return this.indexBuffer;
      }
    }

    int IModelPart.IndexCount
    {
      get
      {
        return this.indexCount;
      }
    }

    int IModelPart.IndexOffset
    {
      get
      {
        return 0;
      }
    }

    int IModelPart.PrimitiveCount
    {
      get
      {
        return this.indexCount * 3;
      }
    }

    VertexBuffer IModelPart.VertexBuffer
    {
      get
      {
        return this.vertexBuffer;
      }
    }

    int IModelPart.VertexCount
    {
      get
      {
        return this.vertexCount;
      }
    }

    VertexDeclaration IModelPart.VertexDeclaration
    {
      get
      {
        return this.vertexDeclaration;
      }
    }

    int IModelPart.VertexOffset
    {
      get
      {
        return 0;
      }
    }

    int IModelPart.VertexStride
    {
      get
      {
        return this.vertexStride;
      }
    }

    Effect IModelPart.Effect
    {
      get
      {
        return (Effect) this.effect;
      }
    }

    public Texture2D Texture
    {
      get
      {
        return this.texture;
      }
      set
      {
        this.texture = value;
      }
    }

    bool IModelPart.DepthBufferEnable
    {
      get
      {
        return this.depthBufferEnable;
      }
    }

    bool IModelPart.AlphaBlendEnable
    {
      get
      {
        return this.alphaBlendEnable;
      }
    }

    bool IModelPart.AlphaTestEnable
    {
      get
      {
        return this.alphaTestEnable;
      }
    }

    CullMode IModelPart.CullMode
    {
      get
      {
        return this.cullMode;
      }
    }

    TextureAddressMode IModelPart.AddressU
    {
      get
      {
        return this.addressU;
      }
    }

    TextureAddressMode IModelPart.AddressV
    {
      get
      {
        return this.addressV;
      }
    }

    public void Dispose()
    {
      if (this.indexBuffer != null)
      {
        this.indexBuffer.Dispose();
        this.indexBuffer = (IndexBuffer) null;
      }
      if (this.vertexBuffer != null)
      {
        this.vertexBuffer.Dispose();
        this.vertexBuffer = (VertexBuffer) null;
      }
      this.vertexDeclaration = (VertexDeclaration) null;
      this.effect = (BasicEffect) null;
    }

    public override string ToString()
    {
      return string.Format("Indices: Chunk size: {0}  Current Count: {1}  Buffer Size: {2}\n", (object) this.indexChunkSize, (object) this.indexCount, (object) (this.indexBuffer != null ? this.indexBuffer.IndexCount : 0)) + string.Format("Vertices: Chunk size: {0}  Current Count: {1}  Buffer Size: {2}\n", (object) this.vertexChunkSize, (object) this.vertexCount, (object) (this.vertexBuffer != null ? this.vertexBuffer.VertexCount : 0));
    }

    public long BufferSize
    {
      get
      {
        long num = 0;
        if (this.indexBuffer != null)
          num += this.indexBuffer.BufferSize();
        if (this.vertexBuffer != null)
          num += this.vertexBuffer.BufferSize();
        return num;
      }
    }
  }
}
