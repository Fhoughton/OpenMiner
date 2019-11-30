// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Renderers.ModelRenderer
// Assembly: StudioForge.Engine.Renderers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A5B8FBA8-9BCB-4F81-AE3F-9C2CDA9150FB
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Renderers.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Renderers
{
  public static class ModelRenderer
  {
    public static void InitModel(Model model, ModelRenderer.GlobalBasicEffectUpdate update)
    {
      Matrix[] destinationBoneTransforms = new Matrix[model.Bones.Count];
      model.CopyAbsoluteBoneTransformsTo(destinationBoneTransforms);
      if (update == null)
        return;
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (Effect effect1 in mesh.Effects)
        {
          BasicEffect effect2 = effect1 as BasicEffect;
          if (effect2 != null)
            update(effect2);
        }
      }
    }

    public static void Draw(Model model, ICamera camera, Matrix world)
    {
      ModelRenderer.Draw(model, camera, world, false);
    }

    public static void Draw(Model model, ICamera camera, Matrix world, bool dontTouchRenderStates)
    {
      ModelRenderer.Draw(model, camera, world, dontTouchRenderStates, (ModelRenderer.GlobalBasicEffectUpdate) null);
    }

    public static void Draw(
      Model model,
      ICamera camera,
      Matrix world,
      bool dontTouchRenderStates,
      ModelRenderer.GlobalBasicEffectUpdate update)
    {
      if (model == null || camera == null || model.Meshes.Count < 1)
        return;
      int num = dontTouchRenderStates ? 1 : 0;
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (Effect effect1 in mesh.Effects)
        {
          BasicEffect effect2 = effect1 as BasicEffect;
          if (effect2 != null)
          {
            effect2.View = camera.ViewMatrix;
            effect2.Projection = camera.ProjectionMatrix;
            effect2.World = world;
            if (update != null)
              update(effect2);
            if (effect2.FogEnabled = camera.FogEnabled)
            {
              effect2.FogColor = camera.LenseColor.ToVector3();
              effect2.FogStart = camera.FogStart;
              effect2.FogEnd = camera.FarClip;
            }
          }
        }
        mesh.Draw();
        ++CoreGlobals.FrameRateCounter.DrawCalls;
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
          CoreGlobals.FrameRateCounter.Primitives += meshPart.PrimitiveCount;
      }
    }

    public static void Draw(
      Model model,
      Matrix world,
      Matrix view,
      Matrix projection,
      ModelRenderer.GlobalBasicEffectUpdate update)
    {
      if (model == null || model.Meshes.Count < 1)
        return;
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (Effect effect1 in mesh.Effects)
        {
          BasicEffect effect2 = effect1 as BasicEffect;
          if (effect2 != null)
          {
            effect2.View = view;
            effect2.Projection = projection;
            effect2.World = world;
            if (update != null)
              update(effect2);
          }
        }
        mesh.Draw();
        ++CoreGlobals.FrameRateCounter.DrawCalls;
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
          CoreGlobals.FrameRateCounter.Primitives += meshPart.PrimitiveCount;
      }
    }

    public delegate void GlobalBasicEffectUpdate(BasicEffect effect);
  }
}
