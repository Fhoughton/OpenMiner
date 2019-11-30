// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ChunkLoaderPriority
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;

namespace StudioForge.TotalMiner
{
  internal class ChunkLoaderPriority : TimedThreadWorkItem
  {
    private SurroundingChunkEnumerator surroundingChunkEnumerator = new SurroundingChunkEnumerator();
    private Map map;
    private GameInstance instance;

    public override string Name
    {
      get
      {
        return "ChunkPriorityLoader";
      }
    }

    public ChunkLoaderPriority(GameInstance instance, Map map)
      : base(PriorityLevel.Urgent, 4)
    {
      this.map = map;
      this.instance = instance;
    }

    protected override void UpdateCore()
    {
      int y = this.map.ChunkSize.Y;
      this.LoadMeshes(0);
      this.LoadMeshes(-y);
      this.LoadMeshes(y);
    }

    public void LoadMeshes(int yoffset)
    {
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null)
          this.LoadMeshes(tag, yoffset);
      }
    }

    public void LoadMeshes(Player player, int yoffset)
    {
      GlobalPoint3D point = this.map.GetPoint(player.Position - new Vector3(0.0f, this.map.TileSize * 0.5f, 0.0f));
      point.Y += yoffset;
      this.surroundingChunkEnumerator.Reset(this.map, point, 9);
      while (this.surroundingChunkEnumerator.MoveNext())
      {
        MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumerator.Current);
        if (chunk != null && chunk.ShouldLoadMesh)
        {
          this.UpdateNeighboursFirst(chunk);
          chunk.LoadMesh(true, false);
        }
      }
      this.surroundingChunkEnumerator.Reset(this.map, point, 25);
      while (this.surroundingChunkEnumerator.MoveNext())
      {
        MapChunk chunk = this.map.GetChunk(this.surroundingChunkEnumerator.Current);
        if (chunk != null && chunk.ShouldLight)
          chunk.Light(true);
      }
    }

    private void UpdateNeighboursFirst(MapChunk chunk)
    {
      ChunkUpdateFlags updateFlags = chunk.UpdateFlags;
      if (updateFlags == ChunkUpdateFlags.None)
        return;
      if ((updateFlags & ChunkUpdateFlags.LeftChunkBorder) == ChunkUpdateFlags.LeftChunkBorder)
        this.LoadChunk(chunk.LeftNeighbour());
      if ((updateFlags & ChunkUpdateFlags.ForwardChunkBorder) == ChunkUpdateFlags.ForwardChunkBorder)
        this.LoadChunk(chunk.ForwardNeighbour());
      if ((updateFlags & ChunkUpdateFlags.RightChunkBorder) == ChunkUpdateFlags.RightChunkBorder)
        this.LoadChunk(chunk.RightNeighbour());
      if ((updateFlags & ChunkUpdateFlags.BackChunkBorder) == ChunkUpdateFlags.BackChunkBorder)
        this.LoadChunk(chunk.BackwardNeighbour());
      if ((updateFlags & ChunkUpdateFlags.DownChunkBorder) == ChunkUpdateFlags.DownChunkBorder)
        this.LoadChunk(chunk.DownNeighbour());
      if ((updateFlags & ChunkUpdateFlags.UpChunkBorder) != ChunkUpdateFlags.UpChunkBorder)
        return;
      this.LoadChunk(chunk.UpNeighbour());
    }

    private bool LoadChunk(MapChunk chunk)
    {
      if (chunk == null)
        return true;
      if (!chunk.ShouldLoadMesh)
        return false;
      chunk.LoadMesh(true, false);
      return true;
    }
  }
}
