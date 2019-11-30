// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.DeltaClampVec3
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class DeltaClampVec3
  {
    public Vector3 CurrentValue;
    private readonly Queue<Vector3> history;
    private readonly float maxChange;
    private readonly int historyCapacity;
    private Vector3 currentChange;

    public DeltaClampVec3(float historyLen, float maxChange)
    {
      this.maxChange = maxChange;
      this.historyCapacity = (int) ((double) historyLen * 60.0);
      this.history = new Queue<Vector3>(this.historyCapacity);
      for (int index = 0; index < this.historyCapacity; ++index)
        this.history.Enqueue(Vector3.Zero);
    }

    public Vector3 Update(Vector3 newValue)
    {
      Vector3 vector3_1 = new Vector3();
      vector3_1.X = Math.Abs(newValue.X - this.CurrentValue.X);
      vector3_1.Y = Math.Abs(newValue.Y - this.CurrentValue.Y);
      vector3_1.Z = Math.Abs(newValue.Z - this.CurrentValue.Z);
      Vector3 vector3_2 = this.history.Dequeue();
      this.currentChange.X -= vector3_2.X;
      this.currentChange.Y -= vector3_2.Y;
      this.currentChange.Z -= vector3_2.Z;
      if ((double) this.currentChange.X + (double) vector3_1.X > (double) this.maxChange)
        vector3_1.X = this.maxChange - this.currentChange.X;
      if ((double) this.currentChange.Y + (double) vector3_1.Y > (double) this.maxChange)
        vector3_1.Y = this.maxChange - this.currentChange.Y;
      if ((double) this.currentChange.Z + (double) vector3_1.Z > (double) this.maxChange)
        vector3_1.Z = this.maxChange - this.currentChange.Z;
      this.history.Enqueue(vector3_1);
      if ((double) vector3_1.X != 0.0)
      {
        this.currentChange.X += vector3_1.X;
        this.CurrentValue.X += (double) newValue.X > (double) this.CurrentValue.X ? vector3_1.X : -vector3_1.X;
      }
      if ((double) vector3_1.Y != 0.0)
      {
        this.currentChange.Y += vector3_1.Y;
        this.CurrentValue.Y += (double) newValue.Y > (double) this.CurrentValue.Y ? vector3_1.Y : -vector3_1.Y;
      }
      if ((double) vector3_1.Z != 0.0)
      {
        this.currentChange.Z += vector3_1.Z;
        this.CurrentValue.Z += (double) newValue.Z > (double) this.CurrentValue.Z ? vector3_1.Z : -vector3_1.Z;
      }
      return this.CurrentValue;
    }
  }
}
