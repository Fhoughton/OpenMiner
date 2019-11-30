// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.AnimationPlayer
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class AnimationPlayer
  {
    private AnimationClip currentClipValue;
    private TimeSpan currentTimeValue;
    private int currentKeyframe;
    private Matrix[] boneTransforms;
    private Matrix[] worldTransforms;
    private Matrix[] skinTransforms;
    private SkinningData skinningDataValue;

    public AnimationPlayer(SkinningData skinningData)
    {
      if (skinningData == null)
        throw new ArgumentNullException(nameof (skinningData));
      this.skinningDataValue = skinningData;
      this.boneTransforms = new Matrix[skinningData.BindPose.Count];
      this.worldTransforms = new Matrix[skinningData.BindPose.Count];
      this.skinTransforms = new Matrix[skinningData.BindPose.Count];
    }

    public void StartClip(AnimationClip clip)
    {
      if (clip == null)
        throw new ArgumentNullException(nameof (clip));
      this.currentClipValue = clip;
      this.currentTimeValue = TimeSpan.Zero;
      this.currentKeyframe = 0;
      this.skinningDataValue.BindPose.CopyTo(this.boneTransforms, 0);
    }

    public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
    {
      this.UpdateBoneTransforms(time, relativeToCurrentTime);
      this.UpdateWorldTransforms(rootTransform, this.boneTransforms);
      this.UpdateSkinTransforms(Matrix.Identity);
    }

    public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
    {
      if (this.currentClipValue == null)
        throw new InvalidOperationException("AnimationPlayer.Update was called before StartClip");
      if (relativeToCurrentTime)
      {
        time += this.currentTimeValue;
        while (time >= this.currentClipValue.Duration)
          time -= this.currentClipValue.Duration;
      }
      if (time < TimeSpan.Zero || time >= this.currentClipValue.Duration)
        throw new ArgumentOutOfRangeException(nameof (time));
      if (time < this.currentTimeValue)
      {
        this.currentKeyframe = 0;
        this.skinningDataValue.BindPose.CopyTo(this.boneTransforms, 0);
      }
      this.currentTimeValue = time;
      for (IList<Keyframe> keyframes = (IList<Keyframe>) this.currentClipValue.Keyframes; this.currentKeyframe < keyframes.Count; ++this.currentKeyframe)
      {
        Keyframe keyframe = keyframes[this.currentKeyframe];
        if (keyframe.Time > this.currentTimeValue)
          break;
        this.boneTransforms[keyframe.Bone] = keyframe.Transform;
      }
    }

    public void UpdateWorldTransforms(Matrix rootTransform, Matrix[] boneTransforms)
    {
      this.worldTransforms[0] = boneTransforms[0] * rootTransform;
      for (int index1 = 1; index1 < this.worldTransforms.Length; ++index1)
      {
        int index2 = this.skinningDataValue.SkeletonHierarchy[index1];
        this.worldTransforms[index1] = boneTransforms[index1] * this.worldTransforms[index2];
      }
    }

    public void UpdateSkinTransforms(Matrix rootTransform)
    {
      for (int index = 0; index < this.skinTransforms.Length; ++index)
        this.skinTransforms[index] = this.skinningDataValue.InverseBindPose[index] * rootTransform * this.worldTransforms[index];
    }

    public Matrix[] GetBoneTransforms()
    {
      return this.boneTransforms;
    }

    public Matrix[] GetWorldTransforms()
    {
      return this.worldTransforms;
    }

    public Matrix[] GetSkinTransforms()
    {
      return this.skinTransforms;
    }

    public AnimationClip CurrentClip
    {
      get
      {
        return this.currentClipValue;
      }
    }

    public TimeSpan CurrentTime
    {
      get
      {
        return this.currentTimeValue;
      }
    }
  }
}
