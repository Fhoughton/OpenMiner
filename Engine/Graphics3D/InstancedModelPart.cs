// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.InstancedModelPart
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  internal class InstancedModelPart
  {
    private Matrix[] tempMatrices = new Matrix[45];
    private Vector4[] tempDiffuse = new Vector4[45];
    private const int MaxShaderMatrices = 45;
    private const int SizeOfVector4 = 16;
    private const int SizeOfMatrix = 64;
    private const string NoInstancingTechniqueName = "NoInstancing";
    private const string NoInstancingTexturedTechniqueName = "NoInstancingTextured";
    private const string VFetchInstancingTechniqueName = "VFetchInstancing";
    private const string VFetchInstancingTexturedTechniqueName = "VFetchInstancingTextured";
    private const string ShaderInstancingTechniqueName = "ShaderInstancing";
    private const string ShaderInstancingTexturedTechniqueName = "ShaderInstancingTextured";
    private const string HardwareInstancingTechniqueName = "HardwareInstancing";
    private const string HardwareInstancingTexturedTechniqueName = "HardwareInstancingTextured";
    private InstancedModel model;
    [ContentSerializer]
    private int indexCount;
    [ContentSerializer]
    private int vertexCount;
    [ContentSerializer]
    private VertexBuffer vertexBuffer;
    [ContentSerializer]
    private IndexBuffer indexBuffer;
    [ContentSerializer(SharedResource = true)]
    private Effect effect;
    private Texture2D texture;
    private bool techniqueChanged;
    private GraphicsDevice graphicsDevice;
    private int maxInstances;
    private DynamicVertexBuffer instanceDataStream;
    private DynamicVertexBuffer diffuseDataStream;
    private VertexElement[] originalVertexDeclaration;
    private bool vertexDataIsReplicated;
    private EffectParameter niTransformEffectParam;
    private EffectParameter niDiffuseEffectParam;
    private EffectParameter transformEffectParam;
    private EffectParameter diffuseEffectParam;
    private EffectParameter textureEffectParam;
    private EffectParameter eyeEffectParam;
    private EffectParameter viewEffectParam;
    private EffectParameter projectionEffectParam;

    public Effect Effect
    {
      get
      {
        return this.effect;
      }
    }

    private InstancedModelPart()
    {
    }

    internal void Initialize(GraphicsDevice device)
    {
      this.graphicsDevice = device;
      this.maxInstances = Math.Min((int) ushort.MaxValue / this.vertexCount, 45);
      this.niTransformEffectParam = this.effect.Parameters["NoInstancingTransform"];
      this.niDiffuseEffectParam = this.effect.Parameters["NoInstancingDiffuse"];
      this.transformEffectParam = this.effect.Parameters["InstanceTransforms"];
      this.diffuseEffectParam = this.effect.Parameters["InstanceDiffuse"];
      this.textureEffectParam = this.effect.Parameters["BasicTexture"];
      this.eyeEffectParam = this.effect.Parameters["EyePosition"];
      this.viewEffectParam = this.effect.Parameters["View"];
      this.projectionEffectParam = this.effect.Parameters["Projection"];
    }

    internal void SetInstancingTechnique(InstancingTechnique instancingTechnique)
    {
      switch (instancingTechnique)
      {
        case InstancingTechnique.HardwareInstancing:
          this.InitializeHardwareInstancing();
          break;
        case InstancingTechnique.ShaderInstancing:
          this.InitializeShaderInstancing();
          break;
      }
      this.techniqueChanged = true;
    }

    private void InitializeShaderInstancing()
    {
      if (!this.vertexDataIsReplicated)
      {
        this.ReplicateVertexData();
        this.ReplicateIndexData();
        this.vertexDataIsReplicated = true;
      }
      this.ExtendVertexDeclaration(new VertexElement[1]
      {
        new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
      });
    }

    private void InitializeHardwareInstancing()
    {
      VertexElement[] extraElements = new VertexElement[4];
      short num1 = 0;
      byte num2 = 1;
      for (int index = 0; index < extraElements.Length; ++index)
      {
        extraElements[index] = new VertexElement((int) num1, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, (int) num2);
        num1 += (short) 16;
        ++num2;
      }
      this.ExtendVertexDeclaration(extraElements);
    }

    private void ExtendVertexDeclaration(VertexElement[] extraElements)
    {
      VertexElement[] vertexElementArray = new VertexElement[this.originalVertexDeclaration.Length + extraElements.Length];
      this.originalVertexDeclaration.CopyTo((Array) vertexElementArray, 0);
      extraElements.CopyTo((Array) vertexElementArray, this.originalVertexDeclaration.Length);
    }

    private void ReplicateVertexData()
    {
    }

    private void ReplicateIndexData()
    {
      ushort[] data1 = new ushort[this.indexCount];
      this.indexBuffer.GetData<ushort>(data1);
      this.indexBuffer.Dispose();
      ushort[] data2 = new ushort[this.indexCount * this.maxInstances];
      int index1 = 0;
      for (int index2 = 0; index2 < this.maxInstances; ++index2)
      {
        int num = index2 * this.vertexCount;
        for (int index3 = 0; index3 < this.indexCount; ++index3)
        {
          data2[index1] = (ushort) ((uint) data1[index3] + (uint) num);
          ++index1;
        }
      }
      this.indexBuffer = new IndexBuffer(this.graphicsDevice, IndexElementSize.SixteenBits, this.indexCount, BufferUsage.None);
      this.indexBuffer.SetData<ushort>(data2);
    }

    public void SetTexture(Texture2D texture)
    {
      this.textureEffectParam.SetValue((Texture) texture);
      if (!this.techniqueChanged)
        this.techniqueChanged = this.texture == null && texture != null || this.texture != null && texture == null;
      this.texture = texture;
    }

    public void Draw(
      InstancedModel model,
      InstancingTechnique instancingTechnique,
      Matrix[] instanceTransforms,
      Vector4[] instanceDiffuse,
      Matrix view,
      Matrix projection,
      Vector3 eyePosition)
    {
      this.model = model;
      if (instancingTechnique == InstancingTechnique.NoInstancingOrStateBatching)
      {
        this.DrawNoInstancingOrStateBatching(instanceTransforms, instanceDiffuse, view, projection, eyePosition);
      }
      else
      {
        this.SetRenderStates(instancingTechnique, view, projection, eyePosition);
        int count = this.effect.CurrentTechnique.Passes.Count;
        for (int index = 0; index < count; ++index)
        {
          this.effect.CurrentTechnique.Passes[index].Apply();
          switch (instancingTechnique)
          {
            case InstancingTechnique.HardwareInstancing:
              this.DrawHardwareInstancing(instanceTransforms, instanceDiffuse);
              break;
            case InstancingTechnique.ShaderInstancing:
              this.DrawShaderInstancing(instanceTransforms, instanceDiffuse);
              break;
            case InstancingTechnique.NoInstancing:
              this.DrawNoInstancing(instanceTransforms, instanceDiffuse);
              break;
          }
        }
      }
    }

    private void SetRenderStates(
      InstancingTechnique instancingTechnique,
      Matrix view,
      Matrix projection,
      Vector3 eyePosition)
    {
      this.graphicsDevice.SetVertexBuffer(this.vertexBuffer);
      this.graphicsDevice.Indices = this.indexBuffer;
      int num = this.model.UseAlpha ? 1 : 0;
      if (this.techniqueChanged)
        this.SetTechnique(instancingTechnique);
      this.viewEffectParam.SetValue(view);
      this.eyeEffectParam.SetValue(eyePosition);
      this.projectionEffectParam.SetValue(projection);
    }

    private void SetTechnique(InstancingTechnique instancingTechnique)
    {
      switch (instancingTechnique)
      {
        case InstancingTechnique.HardwareInstancing:
          this.effect.CurrentTechnique = this.texture != null ? this.effect.Techniques["HardwareInstancingTextured"] : this.effect.Techniques["HardwareInstancing"];
          break;
        case InstancingTechnique.ShaderInstancing:
          this.effect.CurrentTechnique = this.texture != null ? this.effect.Techniques["ShaderInstancingTextured"] : this.effect.Techniques["ShaderInstancing"];
          break;
        default:
          this.effect.CurrentTechnique = this.texture != null ? this.effect.Techniques["NoInstancingTextured"] : this.effect.Techniques["NoInstancing"];
          break;
      }
      this.techniqueChanged = false;
    }

    private void DrawShaderInstancing(Matrix[] instanceTransforms, Vector4[] instanceDiffuse)
    {
      for (int sourceIndex = 0; sourceIndex < instanceTransforms.Length; sourceIndex += this.maxInstances)
      {
        int length = instanceTransforms.Length - sourceIndex;
        if (length > this.maxInstances)
          length = this.maxInstances;
        Array.Copy((Array) instanceTransforms, sourceIndex, (Array) this.tempMatrices, 0, length);
        Array.Copy((Array) instanceDiffuse, sourceIndex, (Array) this.tempDiffuse, 0, length);
        this.transformEffectParam.SetValue(this.tempMatrices);
        this.diffuseEffectParam.SetValue(this.tempDiffuse);
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, length * this.vertexCount, 0, length * this.indexCount / 3);
        ++CoreGlobals.FrameRateCounter.DrawCalls;
        CoreGlobals.FrameRateCounter.Primitives += length * this.indexCount / 3;
      }
    }

    private void DrawHardwareInstancing(Matrix[] instanceTransforms, Vector4[] instanceDiffuse)
    {
    }

    private void DrawNoInstancing(Matrix[] instanceTransforms, Vector4[] instanceDiffuse)
    {
      for (int index = 0; index < instanceTransforms.Length; ++index)
      {
        this.niTransformEffectParam.SetValue(instanceTransforms[index]);
        this.niDiffuseEffectParam.SetValue(instanceDiffuse[index]);
        this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertexCount, 0, this.indexCount / 3);
        ++CoreGlobals.FrameRateCounter.DrawCalls;
        CoreGlobals.FrameRateCounter.Primitives += this.indexCount / 3;
      }
    }

    private void DrawNoInstancingOrStateBatching(
      Matrix[] instanceTransforms,
      Vector4[] instanceDiffuse,
      Matrix view,
      Matrix projection,
      Vector3 eyePosition)
    {
      for (int index1 = 0; index1 < instanceTransforms.Length; ++index1)
      {
        this.niTransformEffectParam.SetValue(instanceTransforms[index1]);
        this.niDiffuseEffectParam.SetValue(instanceDiffuse[index1]);
        this.SetRenderStates(InstancingTechnique.NoInstancing, view, projection, eyePosition);
        int count = this.effect.CurrentTechnique.Passes.Count;
        for (int index2 = 0; index2 < count; ++index2)
        {
          this.effect.CurrentTechnique.Passes[index2].Apply();
          this.graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertexCount, 0, this.indexCount / 3);
          ++CoreGlobals.FrameRateCounter.DrawCalls;
          CoreGlobals.FrameRateCounter.Primitives += this.indexCount / 3;
        }
      }
    }
  }
}
