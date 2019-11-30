// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ActorAnim
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine;
using System;

namespace StudioForge.TotalMiner
{
  internal class ActorAnim
  {
    public int CurrentFrame;
    private float walkAnimTimer;
    private bool backWalkFrames;

    public void Update(
      float elapsed,
      int frameCount,
      Vector3 velocity,
      Vector2 maxVelocity,
      float frameInterval)
    {
      if (frameCount <= 0)
        return;
      if ((double) velocity.X == 0.0 && (double) velocity.Z == 0.0)
      {
        this.CurrentFrame = frameCount / 2;
      }
      else
      {
        this.walkAnimTimer += Services.ElapsedTime * Math.Max(Math.Abs(velocity.X) / maxVelocity.X, Math.Abs(velocity.Z) / maxVelocity.X);
        if ((double) this.walkAnimTimer <= (double) frameInterval)
          return;
        if (this.backWalkFrames)
        {
          --this.CurrentFrame;
          if (this.CurrentFrame < 0)
          {
            this.CurrentFrame = 1;
            this.backWalkFrames = false;
          }
        }
        else
        {
          ++this.CurrentFrame;
          if (this.CurrentFrame > frameCount)
          {
            this.CurrentFrame = frameCount - 1;
            this.backWalkFrames = true;
          }
        }
        this.walkAnimTimer = 0.0f;
      }
    }
  }
}
