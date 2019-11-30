// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IModelPart
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.Engine.Integration
{
  public interface IModelPart : IUnmanagedBuffer
  {
    int IndexCount { get; }

    int IndexOffset { get; }

    int VertexCount { get; }

    int VertexStride { get; }

    int VertexOffset { get; }

    int PrimitiveCount { get; }

    Effect Effect { get; }

    IndexBuffer IndexBuffer { get; }

    VertexBuffer VertexBuffer { get; }

    VertexDeclaration VertexDeclaration { get; }

    Texture2D Texture { get; }

    bool DepthBufferEnable { get; }

    bool AlphaBlendEnable { get; }

    bool AlphaTestEnable { get; }

    CullMode CullMode { get; }

    TextureAddressMode AddressU { get; }

    TextureAddressMode AddressV { get; }
  }
}
