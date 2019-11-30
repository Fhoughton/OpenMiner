// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.InstanceRenderer
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class InstanceRenderer : IHasContent
  {
    protected GraphicsDevice graphicsDevice;
    protected Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> cache;
    protected Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> transparentCache;
    protected List<InstanceRenderer.BatchData> lastRegisteredBatchList;

    public virtual void LoadContent(InitState state)
    {
      this.graphicsDevice = CoreGlobals.GraphicsDevice;
      this.cache = new Dictionary<InstancedModel, List<InstanceRenderer.BatchData>>();
      this.transparentCache = new Dictionary<InstancedModel, List<InstanceRenderer.BatchData>>();
    }

    public virtual void UnloadContent()
    {
    }

    public InstanceRenderer.BatchData Register(
      InstancedModel model,
      Texture2D texture,
      int instanceCount,
      bool transparent)
    {
      model.UseAlpha = transparent;
      this.GetBatch(transparent ? this.transparentCache : this.cache, model);
      InstanceRenderer.BatchData batchData = new InstanceRenderer.BatchData(texture, instanceCount);
      this.lastRegisteredBatchList.Add(batchData);
      return batchData;
    }

    private void GetBatch(
      Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> cache,
      InstancedModel model)
    {
      if (cache.TryGetValue(model, out this.lastRegisteredBatchList))
        return;
      this.lastRegisteredBatchList = new List<InstanceRenderer.BatchData>();
      cache.Add(model, this.lastRegisteredBatchList);
    }

    public void Unregister(InstancedModel model, InstanceRenderer.BatchData batch)
    {
      if (this.Unregister(this.cache, model, batch))
        return;
      this.Unregister(this.transparentCache, model, batch);
    }

    private bool Unregister(
      Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> cache,
      InstancedModel model,
      InstanceRenderer.BatchData batch)
    {
      List<InstanceRenderer.BatchData> batchDataList;
      if (!cache.TryGetValue(model, out batchDataList) || !batchDataList.Contains(batch))
        return false;
      batchDataList.Remove(batch);
      return true;
    }

    public void Draw(ICamera camera)
    {
      this.DrawCache(camera, this.cache);
    }

    public void Draw(Matrix view, Matrix proj, Vector3 eye)
    {
      this.DrawCache(view, proj, eye, this.cache);
    }

    public void DrawTransparents(ICamera camera)
    {
      this.DrawCache(camera, this.transparentCache);
    }

    private void DrawCache(
      ICamera camera,
      Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> cache)
    {
      this.DrawCache(camera.ViewMatrix, camera.ProjectionMatrix, camera.Position, cache);
    }

    private void DrawCache(
      Matrix view,
      Matrix proj,
      Vector3 eye,
      Dictionary<InstancedModel, List<InstanceRenderer.BatchData>> cache)
    {
      foreach (InstancedModel key in cache.Keys)
      {
        foreach (InstanceRenderer.BatchData batchData in cache[key])
          ;
      }
    }

    public class BatchData
    {
      public Texture2D Texture;
      public Matrix[] InstanceTransforms;
      public Vector4[] InstanceDiffuse;

      public BatchData(Texture2D texture, int instanceCount)
      {
        this.Texture = texture;
        this.InstanceTransforms = new Matrix[instanceCount];
        this.InstanceDiffuse = new Vector4[instanceCount];
      }
    }
  }
}
