// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.GeneralModelPart
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Graphics3D
{
  public class GeneralModelPart : IModelPart, IUnmanagedBuffer
  {
    public GeneralModelPart()
    {
      this.DepthBufferEnable = true;
      this.AlphaBlendEnable = false;
      this.AlphaTestEnable = false;
      this.AddressU = TextureAddressMode.Wrap;
      this.AddressV = TextureAddressMode.Wrap;
    }

    public long BufferSize
    {
      get
      {
        long num = 0;
        if (this.VertexBuffer != null)
          num += this.VertexBuffer.BufferSize();
        if (this.IndexBuffer != null)
          num += this.IndexBuffer.BufferSize();
        return num;
      }
    }

    public Effect Effect { get; set; }

    public IndexBuffer IndexBuffer { get; set; }

    public int IndexCount { get; set; }

    public int IndexOffset { get; set; }

    public int PrimitiveCount { get; set; }

    public VertexBuffer VertexBuffer { get; set; }

    public int VertexCount { get; set; }

    public VertexDeclaration VertexDeclaration { get; set; }

    public int VertexOffset { get; set; }

    public int VertexStride { get; set; }

    public Texture2D Texture { get; set; }

    public bool DepthBufferEnable { get; set; }

    public bool AlphaBlendEnable { get; set; }

    public bool AlphaTestEnable { get; set; }

    public CullMode CullMode { get; set; }

    public TextureAddressMode AddressU { get; set; }

    public TextureAddressMode AddressV { get; set; }
  }
}
