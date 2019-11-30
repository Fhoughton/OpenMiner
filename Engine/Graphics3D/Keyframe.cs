// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.Keyframe
// Assembly: StudioForge.Engine.Graphics3D_Pipeline, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E459E66A-A239-4A08-BD37-25C4FD372E6D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D_Pipeline.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class Keyframe
  {
    public Keyframe(int bone, TimeSpan time, Matrix transform)
    {
      this.Bone = bone;
      this.Time = time;
      this.Transform = transform;
    }

    private Keyframe()
    {
    }

    [ContentSerializer]
    public int Bone { get; private set; }

    [ContentSerializer]
    public TimeSpan Time { get; private set; }

    [ContentSerializer]
    public Matrix Transform { get; private set; }
  }
}
