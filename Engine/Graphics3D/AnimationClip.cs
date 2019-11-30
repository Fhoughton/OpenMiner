// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.AnimationClip
// Assembly: StudioForge.Engine.Graphics3D_Pipeline, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E459E66A-A239-4A08-BD37-25C4FD372E6D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D_Pipeline.dll

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace StudioForge.Engine.Graphics3D
{
  public class AnimationClip
  {
    public AnimationClip(TimeSpan duration, List<Keyframe> keyframes)
    {
      this.Duration = duration;
      this.Keyframes = keyframes;
    }

    private AnimationClip()
    {
    }

    [ContentSerializer]
    public TimeSpan Duration { get; private set; }

    [ContentSerializer]
    public List<Keyframe> Keyframes { get; private set; }

    public AnimationClip Clone(TimeSpan from, TimeSpan to)
    {
      AnimationClip clone = new AnimationClip(to - from, new List<Keyframe>());
      this.CloneFrames(clone, from, to);
      this.CloneAllBackwards(clone);
      return clone;
    }

    private void CloneFrames(AnimationClip clone, TimeSpan from, TimeSpan to)
    {
      foreach (Keyframe keyframe1 in this.Keyframes)
      {
        if (keyframe1.Time >= from)
        {
          if (!(keyframe1.Time <= to))
            break;
          Keyframe keyframe2 = new Keyframe(keyframe1.Bone, keyframe1.Time - from, keyframe1.Transform);
          clone.Keyframes.Add(keyframe2);
        }
      }
    }

    private void CloneAllBackwards(AnimationClip clone)
    {
      TimeSpan time = clone.Keyframes[clone.Keyframes.Count - 1].Time;
      for (int index = clone.Keyframes.Count - 1; index >= 0; --index)
      {
        Keyframe keyframe1 = clone.Keyframes[index];
        Keyframe keyframe2 = new Keyframe(keyframe1.Bone, time - keyframe1.Time, keyframe1.Transform);
        clone.Keyframes.Add(keyframe2);
        time = keyframe1.Time;
      }
    }
  }
}
