// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.ItemModelCache
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner.Graphics
{
  internal struct ItemModelCache
  {
    public int VertexCount;
    public VertexBuffer VertexBuffer;
    public CustomArray<VertexItemBlock> Vertices;
    public float Scale;
    public Vector3 Center;
    public float ItemBlockSize;

    public void Clear()
    {
      if (this.VertexBuffer != null)
        this.VertexBuffer.Dispose();
      if (this.Vertices != null)
        this.Vertices.Clear();
      this.VertexBuffer = (VertexBuffer) null;
      this.VertexCount = 0;
      this.Scale = 0.0f;
      this.ItemBlockSize = 0.0f;
    }
  }
}
