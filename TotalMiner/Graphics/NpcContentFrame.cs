// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.NpcContentFrame
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
  internal class NpcContentFrame : IHasContent, IUnmanagedBuffer
  {
    public static object InstanceBufferSemaphore = new object();
    private List<NpcBase> sortedMobs = new List<NpcBase>(50);
    private static int id;
    private static Matrix[] tmpWorldsBuffer;
    public MapModel Model;
    public int InstancesToDrawCount;
    public bool IsContentLoaded;
    public int ContentID;
    private volatile DynamicVertexBuffer instanceBuffer;
    private DynamicVertexBuffer instanceBuffer1;
    private DynamicVertexBuffer instanceBuffer2;
    private float drawOffsetYaw;
    private float drawOffsetPitch;
    private Vector3 drawOffsetPos;

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

    public void LoadContent(GameInstance instance, ActorType actorType, string comName)
    {
      this.ContentID = NpcContentFrame.id++;
      GraphicsDevice graphicsDevice = CoreGlobals.GraphicsDevice;
      int vertexCount = 500;
      this.instanceBuffer1 = new DynamicVertexBuffer(graphicsDevice, typeof (VertexInstance), vertexCount, BufferUsage.WriteOnly);
      this.instanceBuffer2 = new DynamicVertexBuffer(graphicsDevice, typeof (VertexInstance), vertexCount, BufferUsage.WriteOnly);
      ActorTypeDataXML actorTypeDataXml = Globals1.NpcTypeData[(int) actorType];
      int comModId = actorTypeDataXml.ComModID;
      if (comModId == 0)
      {
        this.Model = instance.SystemVoxelModelManager.LoadComponent("System Avatars", comName, true);
      }
      else
      {
        this.Model = instance.VoxelModelManager.LoadComponent(comModId + 1000000, comName, true);
        if (this.Model == null)
          this.Model = instance.SystemVoxelModelManager.LoadComponent("System Avatars", comName, true);
      }
      this.Model.Flags |= ModelFlags.IsNPC;
      if (NpcContentFrame.tmpWorldsBuffer == null || NpcContentFrame.tmpWorldsBuffer.Length < vertexCount)
        NpcContentFrame.tmpWorldsBuffer = new Matrix[vertexCount];
      this.drawOffsetPos = new Vector3((float) -this.Model.ModelSize.X * 0.5f, -1f, (float) -this.Model.ModelSize.Z * 0.5f);
      this.drawOffsetYaw = actorTypeDataXml.ModelYRotation;
      this.IsContentLoaded = true;
    }

    public static float GetFullModelHeight(ActorType actorType)
    {
      return Globals1.NpcTypeData[(int) actorType].ModelHeight;
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

    public void Explode(GameInstance instance, Vector3 position, Vector2 scale, float ratio)
    {
      if (!this.IsContentLoaded || this.Model == null)
        return;
      this.Model.Explode(instance, position, scale, ratio, 2.5f);
    }

    public void PrepareForDraw(GameInstance instance, List<Actor> actors)
    {
      this.InstancesToDrawCount = 0;
      if (!this.IsContentLoaded)
        return;
      Vector3 up = Vector3.Up;
      int elementCount = 0;
      MapTM map = instance.Map;
      for (int index = 0; index < actors.Count && elementCount < NpcContentFrame.tmpWorldsBuffer.Length - 1; ++index)
      {
        Actor actor = actors[index];
        if (actor != null && !actor.IsDeadOrInactiveOrDisabled)
        {
          Vector3 position1 = actor.Position;
          position1.Y += 0.1f;
          MapChunk chunk = map.GetChunk(map.GetPoint(position1));
          if (chunk != null && chunk.IsChunkFlagSet(ChunkFlags.MeshLoaded | ChunkFlags.MeshLoading))
          {
            Matrix translation = Matrix.CreateTranslation(this.drawOffsetPos);
            if ((double) this.drawOffsetPitch != 0.0)
              translation *= Matrix.CreateRotationX(this.drawOffsetPitch);
            if ((double) this.drawOffsetYaw != 0.0)
              translation *= Matrix.CreateRotationY(this.drawOffsetYaw);
            Vector3 viewDirection = actor.ViewDirection;
            if (!actor.DrawUpDownViewDirection)
              viewDirection.Y = 0.0f;
            Matrix matrix = translation * Matrix.Invert(Matrix.CreateLookAt(Vector3.Zero, viewDirection, up));
            Vector3 position2 = actor.Position;
            position2.Y += actor.HoverHeight;
            if ((double) actor.DrawScale != 1.0)
              matrix *= Matrix.CreateScale(actor.DrawScale);
            Matrix world = matrix * Matrix.CreateTranslation(position2);
            Vector3 eyePosition = actor.EyePosition;
            eyePosition.Y += 0.1f;
            NpcManager.SetLightInWorldMatrix((Map) map, position1, eyePosition, ref world, (byte) ((double) actor.Alpha * (double) byte.MaxValue), true);
            NpcContentFrame.tmpWorldsBuffer[elementCount++] = world;
          }
        }
      }
      if (elementCount > 0)
      {
        if (this.instanceBuffer == this.instanceBuffer1 && this.instanceBuffer2 != null && !this.instanceBuffer2.IsDisposed)
        {
          this.instanceBuffer2.SetData<Matrix>(NpcContentFrame.tmpWorldsBuffer, 0, elementCount, SetDataOptions.Discard);
          this.instanceBuffer = this.instanceBuffer2;
        }
        else if (this.instanceBuffer1 != null && !this.instanceBuffer1.IsDisposed)
        {
          this.instanceBuffer1.SetData<Matrix>(NpcContentFrame.tmpWorldsBuffer, 0, elementCount, SetDataOptions.Discard);
          this.instanceBuffer = this.instanceBuffer1;
        }
      }
      this.InstancesToDrawCount = elementCount;
    }
  }
}
