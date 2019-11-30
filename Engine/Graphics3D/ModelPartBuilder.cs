// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.ModelPartBuilder
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Graphics3D
{
  public class ModelPartBuilder
  {
    public IModelPart[] BuildParts(object model)
    {
      Model model1 = model as Model;
      if (model1 != null)
        return this.BuildParts(model1);
      StaticBatchModel model2 = model as StaticBatchModel;
      if (model2 != null)
        return this.BuildParts(model2);
      IModelPart[] modelPartArray = model as IModelPart[];
      if (modelPartArray != null)
        return modelPartArray;
      IModelPart modelPart = model as IModelPart;
      if (modelPart == null)
        return new IModelPart[0];
      return new IModelPart[1]{ modelPart };
    }

    public IModelPart[] BuildParts(StaticBatchModel model)
    {
      return (IModelPart[]) model.ModelParts;
    }

    public IModelPart[] BuildParts(Model model)
    {
      IModelPart[] modelPartArray = new IModelPart[this.GetPartCount(model)];
      int num = 0;
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
          modelPartArray[num++] = (IModelPart) this.BuildPart(model, mesh, meshPart);
      }
      return modelPartArray;
    }

    public GeneralModelPart BuildPart(
      Model model,
      ModelMesh mesh,
      ModelMeshPart meshPart)
    {
      return new GeneralModelPart()
      {
        Effect = meshPart.Effect
      };
    }

    private int GetPartCount(Model model)
    {
      int num = 0;
      foreach (ModelMesh mesh in model.Meshes)
        num += mesh.MeshParts.Count;
      return num;
    }

    private BoundingBox CalculateBoundingBoxes(Model model)
    {
      return new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
    }
  }
}
