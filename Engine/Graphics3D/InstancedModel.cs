// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.InstancedModel
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class InstancedModel
  {
    [ContentSerializer]
    private List<InstancedModelPart> modelParts;
    private GraphicsDevice graphicsDevice;
    private Vector3 emissiveColor;
    private Vector3 specularColor;
    private float specularPower;
    private Vector3 ambientLightColor;
    private Vector3 dirLight0Direction;
    private Vector3 dirLight0DiffuseColor;
    private Vector3 dirLight0SpecularColor;
    private Vector3 dirLight1Direction;
    private Vector3 dirLight1DiffuseColor;
    private Vector3 dirLight1SpecularColor;
    private Vector3 dirLight2Direction;
    private Vector3 dirLight2DiffuseColor;
    private Vector3 dirLight2SpecularColor;

    public void Initialize(GraphicsDevice device)
    {
      this.graphicsDevice = device;
      foreach (InstancedModelPart modelPart in this.modelParts)
        modelPart.Initialize(device);
      InstancingTechnique technique = InstancingTechnique.HardwareInstancing;
      while (!this.IsTechniqueSupported(technique))
        ++technique;
      this.SetInstancingTechnique(technique);
    }

    public InstancingTechnique InstancingTechnique { get; private set; }

    public void SetInstancingTechnique(InstancingTechnique technique)
    {
      this.InstancingTechnique = technique;
      foreach (InstancedModelPart modelPart in this.modelParts)
        modelPart.SetInstancingTechnique(technique);
    }

    public bool IsTechniqueSupported(InstancingTechnique technique)
    {
      return true;
    }

    public Effect GetEffect(int partID)
    {
      return this.modelParts[partID].Effect;
    }

    public void SetTexture(int partID, Texture2D texture)
    {
      this.modelParts[partID].SetTexture(texture);
    }

    [ContentSerializerIgnore]
    public bool UseAlpha { get; set; }

    [ContentSerializerIgnore]
    public Vector3 EmissiveColor
    {
      get
      {
        return this.emissiveColor;
      }
      set
      {
        this.emissiveColor = value;
        this.UpdateModelPartsV3(nameof (EmissiveColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 SpecularColor
    {
      get
      {
        return this.specularColor;
      }
      set
      {
        this.specularColor = value;
        this.UpdateModelPartsV3(nameof (SpecularColor), value);
      }
    }

    [ContentSerializerIgnore]
    public float SpecularPower
    {
      get
      {
        return this.specularPower;
      }
      set
      {
        this.specularPower = value;
        this.UpdateModelPartsFloat(nameof (SpecularPower), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 AmbientLightColor
    {
      get
      {
        return this.ambientLightColor;
      }
      set
      {
        this.ambientLightColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (AmbientLightColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight0Direction
    {
      get
      {
        return this.dirLight0Direction;
      }
      set
      {
        this.dirLight0Direction = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight0Direction), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight0DiffuseColor
    {
      get
      {
        return this.dirLight0DiffuseColor;
      }
      set
      {
        this.dirLight0DiffuseColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight0DiffuseColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight0SpecularColor
    {
      get
      {
        return this.dirLight0SpecularColor;
      }
      set
      {
        this.dirLight0SpecularColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight0SpecularColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight1Direction
    {
      get
      {
        return this.dirLight1Direction;
      }
      set
      {
        this.dirLight1Direction = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight1Direction), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight1DiffuseColor
    {
      get
      {
        return this.dirLight1DiffuseColor;
      }
      set
      {
        this.dirLight1DiffuseColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight1DiffuseColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight1SpecularColor
    {
      get
      {
        return this.dirLight1SpecularColor;
      }
      set
      {
        this.dirLight1SpecularColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight1SpecularColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight2Direction
    {
      get
      {
        return this.dirLight2Direction;
      }
      set
      {
        this.dirLight2Direction = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight2Direction), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight2DiffuseColor
    {
      get
      {
        return this.dirLight2DiffuseColor;
      }
      set
      {
        this.dirLight2DiffuseColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight2DiffuseColor), value);
      }
    }

    [ContentSerializerIgnore]
    public Vector3 DirLight2SpecularColor
    {
      get
      {
        return this.dirLight2SpecularColor;
      }
      set
      {
        this.dirLight2SpecularColor = value;
        foreach (InstancedModelPart modelPart in this.modelParts)
          this.UpdateModelPartsV3(nameof (DirLight2SpecularColor), value);
      }
    }

    private void UpdateModelPartsV3(string name, Vector3 value)
    {
      foreach (InstancedModelPart modelPart in this.modelParts)
        modelPart.Effect.Parameters[name].SetValue(value);
    }

    private void UpdateModelPartsFloat(string name, float value)
    {
      foreach (InstancedModelPart modelPart in this.modelParts)
        modelPart.Effect.Parameters[name].SetValue(value);
    }

    public void DrawInstances(
      Matrix[] instanceTransforms,
      Vector4[] instanceDiffuse,
      Matrix view,
      Matrix projection,
      Vector3 eye)
    {
      if (instanceTransforms.Length <= 0)
        return;
      foreach (InstancedModelPart modelPart in this.modelParts)
        modelPart.Draw(this, this.InstancingTechnique, instanceTransforms, instanceDiffuse, view, projection, eye);
    }
  }
}
