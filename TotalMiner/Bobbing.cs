// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Bobbing
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;

namespace StudioForge.TotalMiner
{
  internal class Bobbing
  {
    private float bobDir = 1f;
    private Vec3Interpolator positionInterpolator = new Vec3Interpolator();
    private FloatInterpolator dipInterpolator = new FloatInterpolator();
    public Vector3 Position;
    private InventoryHand hand;
    private Actor owner;
    private float size;
    private float bobValue;
    private bool dipStarted;
    private bool bobCenteredIsDirty;

    public event EventHandler BobCentered;

    public void Raise_BobCentered()
    {
      if (this.BobCentered == null)
        return;
      this.bobCenteredIsDirty = false;
      this.BobCentered((object) this, EventArgs.Empty);
    }

    public Bobbing(Actor owner, InventoryHand hand)
    {
      this.owner = owner;
      this.hand = hand;
    }

    public void Initialize(float size)
    {
      this.size = size;
    }

    public void Reset()
    {
      this.positionInterpolator.Reset();
    }

    public void Update(UpdateState state)
    {
      Vector3 velocity = this.owner.Velocity;
      if (!this.positionInterpolator.IsActive && this.owner.IsBobbing && ((double) Math.Abs(velocity.X) > 1.0 / 1000.0 || (double) Math.Abs(velocity.Z) > 1.0 / 1000.0))
      {
        if (!this.dipStarted)
        {
          this.dipInterpolator.Start((float) Math.Cos(0.0) * this.size, 0.0f, 0.800000011920929);
          this.dipStarted = true;
        }
        float num1 = 0.08f;
        float num2 = new Vector2(velocity.X, velocity.Z).Length() * 15f;
        this.bobValue += this.bobDir * num1 * num2;
        if ((double) this.bobValue > 1.57079637050629)
        {
          this.bobDir = -1f;
          this.bobValue = 1.570796f;
          this.bobCenteredIsDirty = true;
        }
        else if ((double) this.bobValue < -1.57079637050629)
        {
          this.bobDir = 1f;
          this.bobValue = -1.570796f;
          this.bobCenteredIsDirty = true;
        }
        float num3 = (float) Math.Cos((double) this.bobValue);
        this.Position.X = (float) Math.Sin((double) this.bobValue) * this.size;
        this.Position.Y = (float) -((double) num3 * (double) this.size);
        double num4 = (double) this.dipInterpolator.Update();
        this.Position.Y += this.dipInterpolator.CurrentValue;
        if ((double) this.bobDir > 0.0)
        {
          if (this.bobCenteredIsDirty && (double) this.bobValue > -(double) num1)
            this.Raise_BobCentered();
        }
        else if ((double) this.bobDir < 0.0 && this.bobCenteredIsDirty && (double) this.bobValue < (double) num1)
          this.Raise_BobCentered();
        this.positionInterpolator.Reset();
      }
      else
      {
        this.bobValue = 0.0f;
        this.bobDir = this.hand == InventoryHand.Left ? -1f : 1f;
        this.dipInterpolator.Reset(0.0f);
        this.dipStarted = false;
        if ((double) this.Position.X == 0.0 && (double) this.Position.Y == 0.0 && (double) this.Position.Z == 0.0)
          return;
        if (!this.positionInterpolator.IsActive)
          this.positionInterpolator.Start(this.Position, Vector3.Zero, 0.349999994039536);
        this.positionInterpolator.Update();
        this.Position = this.positionInterpolator.CurrentValue;
      }
    }
  }
}
