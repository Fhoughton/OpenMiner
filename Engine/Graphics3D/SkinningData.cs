// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.SkinningData
// Assembly: StudioForge.Engine.Graphics3D_Pipeline, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E459E66A-A239-4A08-BD37-25C4FD372E6D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D_Pipeline.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class SkinningData
  {
    public SkinningData(
      Dictionary<string, AnimationClip> animationClips,
      List<Matrix> bindPose,
      List<Matrix> inverseBindPose,
      List<int> skeletonHierarchy,
      Dictionary<string, int> boneIndices)
    {
      this.AnimationClips = animationClips;
      this.BindPose = bindPose;
      this.InverseBindPose = inverseBindPose;
      this.SkeletonHierarchy = skeletonHierarchy;
      this.BoneIndices = boneIndices;
    }

    private SkinningData()
    {
    }

    [ContentSerializer]
    public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

    [ContentSerializer]
    public List<Matrix> BindPose { get; private set; }

    [ContentSerializer]
    public List<Matrix> InverseBindPose { get; private set; }

    [ContentSerializer]
    public List<int> SkeletonHierarchy { get; private set; }

    [ContentSerializer]
    public Dictionary<string, int> BoneIndices { get; private set; }
  }
}
