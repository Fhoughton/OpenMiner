// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.EntityContentFrame
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Graphics3D;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner.Graphics
{
  internal class EntityContentFrame : IHasContent, IUnmanagedBuffer
  {
    public static object InstanceBufferSemaphore = new object();
    private static int id;
    private static Matrix[] tmpWorldsBuffer;
    public MapModel Model;
    public int InstancesToDrawCount;
    public bool IsContentLoaded;
    public int ContentID;
    private volatile DynamicVertexBuffer instanceBuffer;
    private DynamicVertexBuffer instanceBuffer1;
    private DynamicVertexBuffer instanceBuffer2;

    public DynamicVertexBuffer InstanceBuffer
    {
      get
      {
        return this.instanceBuffer;
      }
    }

    public long BufferSize
    {
      get
      {
        long num = this.Model != null ? this.Model.BufferSize : 0L;
        if (this.instanceBuffer1 != null)
          num += this.instanceBuffer1.BufferSize();
        if (this.instanceBuffer2 != null)
          num += this.instanceBuffer2.BufferSize();
        return num;
      }
    }

    public void LoadContent(InitState state)
    {
      throw new Exception("Do not use");
    }

    public void LoadContent(GameInstance instance, int comDirNum, string comName)
    {
      this.ContentID = EntityContentFrame.id++;
      GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
      int vertexCount = 100;
      this.instanceBuffer1 = new DynamicVertexBuffer(graphicsDevice, typeof (VertexInstance), vertexCount, BufferUsage.WriteOnly);
      this.instanceBuffer2 = new DynamicVertexBuffer(graphicsDevice, typeof (VertexInstance), vertexCount, BufferUsage.WriteOnly);
      this.Model = instance.VoxelModelManager.LoadComponent(comDirNum, comName, true);
      if (EntityContentFrame.tmpWorldsBuffer == null || EntityContentFrame.tmpWorldsBuffer.Length < vertexCount)
        EntityContentFrame.tmpWorldsBuffer = new Matrix[vertexCount];
      this.IsContentLoaded = true;
    }

    public void UnloadContent()
    {
      if (this.Model != null)
        this.Model.UnloadContent();
      if (this.instanceBuffer1 != null)
        this.instanceBuffer1.Dispose();
      if (this.instanceBuffer2 != null)
        this.instanceBuffer1.Dispose();
      this.instanceBuffer = (DynamicVertexBuffer) null;
      this.instanceBuffer1 = (DynamicVertexBuffer) null;
      this.instanceBuffer2 = (DynamicVertexBuffer) null;
      this.IsContentLoaded = false;
    }

    public void PrepareForDraw(
      GameInstance instance,
      List<Entity> entities,
      BoundingFrustum frustum)
    {
      this.InstancesToDrawCount = 0;
      if (!this.IsContentLoaded)
        return;
      Vector3 up = Vector3.Up;
      int elementCount = 0;
      MapTM map = instance.Map;
      BoundingSphere sphere = new BoundingSphere();
      for (int index = 0; index < entities.Count && elementCount < EntityContentFrame.tmpWorldsBuffer.Length - 1; ++index)
      {
        Entity entity = entities[index];
        Vector3 position1 = entity.Position;
        if (entity.FrustumCull)
        {
          sphere.Center = position1;
          sphere.Center.Y += entity.CenterOffY;
          sphere.Radius = entity.Radius;
          ContainmentType result;
          frustum.Contains(ref sphere, out result);
          if (result == ContainmentType.Disjoint)
            continue;
        }
        position1.Y += 0.1f;
        MapChunk chunk = map.GetChunk(map.GetPoint(position1));
        if (chunk != null && chunk.IsChunkFlagSet(ChunkFlags.MeshLoaded))
        {
          float scale = entity.Scale;
          Matrix translation = Matrix.CreateTranslation(new Vector3((float) -this.Model.ModelSize.X * 0.5f, entity.DrawOffY - 1f, (float) -this.Model.ModelSize.Z * 0.5f));
          if ((double) entity.DrawRotY != 0.0)
            translation *= Matrix.CreateRotationY(entity.DrawRotY);
          translation *= Matrix.Invert(Matrix.CreateLookAt(Vector3.Zero, entity.ViewDirection, up));
          Vector3 position2 = entity.Position;
          if ((double) scale != 1.0)
            translation *= Matrix.CreateScale(scale);
          translation *= Matrix.CreateTranslation(position2);
          NpcManager.SetLightInWorldMatrix((Map) map, position1, position1, ref translation, byte.MaxValue, true);
          EntityContentFrame.tmpWorldsBuffer[elementCount++] = translation;
        }
      }
      if (elementCount > 0)
      {
        if (this.instanceBuffer == this.instanceBuffer1 && this.instanceBuffer2 != null && !this.instanceBuffer2.IsDisposed)
        {
          this.instanceBuffer2.SetData<Matrix>(EntityContentFrame.tmpWorldsBuffer, 0, elementCount, SetDataOptions.Discard);
          this.instanceBuffer = this.instanceBuffer2;
        }
        else if (this.instanceBuffer1 != null && !this.instanceBuffer1.IsDisposed)
        {
          this.instanceBuffer1.SetData<Matrix>(EntityContentFrame.tmpWorldsBuffer, 0, elementCount, SetDataOptions.Discard);
          this.instanceBuffer = this.instanceBuffer1;
        }
      }
      this.InstancesToDrawCount = elementCount;
    }
  }
}
