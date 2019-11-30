// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.Material
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class Material : IDisposable
  {
    public int IndexCount;
    public int VertexCount;
    public int PrimitiveCount;
    public BasicEffect Effect;
    public Texture2D Texture;
    public IndexBuffer IndexBuffer;
    public VertexBuffer VertexBuffer;

    public bool IsLoaded
    {
      get
      {
        if (this.PrimitiveCount > 0)
          return this.VertexBuffer != null;
        return false;
      }
    }

    public int GetStride()
    {
      if (this.VertexBuffer == null || this.VertexCount <= 0)
        return 0;
      return this.VertexBuffer.VertexDeclaration.VertexStride;
    }

    public void Dispose()
    {
      this.PrimitiveCount = this.VertexCount = 0;
      if (this.VertexBuffer != null)
        this.VertexBuffer.Dispose();
      if (this.Effect != null)
        this.Effect.Dispose();
      if (this.Texture == null)
        return;
      this.Texture.Dispose();
    }
  }
}
