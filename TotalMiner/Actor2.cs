// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Actor2
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Net;
using System;

namespace StudioForge.TotalMiner
{
  internal abstract class Actor2 : Actor
  {
    protected int changeDirectionTimer;
    protected int changeDirectionDelay;
    protected Vector2 movementInput;
    protected Vector2 viewInput;
    protected Vector2 viewAngle;
    private float releaseRopeTimer;
    private bool wasOnRope;
    private float flyAccel;

    public Vector2 ViewAngle
    {
      get
      {
        return this.viewAngle;
      }
    }

    protected Actor2(GameInstance instance, MapTM map, NetworkGamer gamer, ActorType mobType)
      : base(instance, map, gamer, mobType)
    {
    }

    protected override void UpdateControlPhysics(
      GlobalPoint3D underFootPoint,
      Block footBlockID,
      Block midBlockID,
      Block eyeBlockID,
      float speedModifier)
    {
      if (this.GetCameraType() == CameraType.Momentum)
        this.UpdateControlPhysicsFPSCinema(underFootPoint, footBlockID, midBlockID, eyeBlockID, speedModifier);
      else
        this.UpdateControlPhysicsFPS(underFootPoint, footBlockID, midBlockID, eyeBlockID, speedModifier);
    }

    private void UpdateControlPhysicsFPS(
      GlobalPoint3D underFootPoint,
      Block footBlockID,
      Block midBlockID,
      Block eyeBlockID,
      float speedModifier)
    {
      float num1 = this.movementInput.LengthSquared();
      Vector3 viewDirection = this.ViewDirection;
      viewDirection.Y *= 0.15f;
      viewDirection.Normalize();
      float num2 = this.Acceleration * speedModifier;
      float num3 = this.movementInput.Y * num2;
      float num4 = viewDirection.X * num3;
      float num5 = viewDirection.Z * num3;
      Matrix rotationY = Matrix.CreateRotationY(-1.570796f);
      Vector3 vector3 = Vector3.Transform(viewDirection, rotationY);
      float num6 = (float) ((double) this.movementInput.X * (double) num2 * 0.899999976158142);
      float num7 = num4 + vector3.X * num6;
      float num8 = num5 + vector3.Z * num6;
      if (this.FlyMode != FlyMode.None)
      {
        if ((double) num1 < 0.0399999991059303)
          this.flyAccel = 0.0f;
        this.flyAccel += num2 * 0.02f;
        if ((double) this.flyAccel > 5.0)
          this.flyAccel = 5f;
        float num9 = this.movementInput.Y * viewDirection.X;
        float num10 = this.movementInput.Y * viewDirection.Z;
        float num11 = this.movementInput.X * 0.9f;
        float num12 = num9 + vector3.X * num11;
        float num13 = num10 + vector3.Z * num11;
        float num14 = num12 * this.flyAccel;
        float num15 = num13 * this.flyAccel;
        this.Velocity.X = num14;
        this.Velocity.Z = num15;
        this.ClampVelocityHoriz(speedModifier);
        num7 = this.Velocity.X;
        num8 = this.Velocity.Z;
        this.Velocity.X = this.Velocity.Z = 0.0f;
      }
      float num16 = num7 + this.KnockForce.X;
      float num17 = num8 + this.KnockForce.Z;
      this.Velocity.X = num16;
      this.Velocity.Z = num17;
      if (!this.isOnLadder)
      {
        this.isOnLadder = this.StandingNextTo(Block.Scaffold);
        if (!this.isOnLadder && this.IsItemEquippedAndUsable(Item.SpiderRing))
        {
          Block oneUnpassableBlock = this.IsStandingNextToAtleastOneUnpassableBlock();
          this.isOnLadder = oneUnpassableBlock != Block.None && oneUnpassableBlock != Block.InvisibleBarrier;
        }
      }
      if ((double) this.releaseRopeTimer > 0.0)
      {
        this.releaseRopeTimer -= Services.ElapsedTime;
        if ((double) this.releaseRopeTimer < 0.0 || (double) this.Velocity.Y < -0.200000002980232)
          this.releaseRopeTimer = 0.0f;
      }
      if (this.FlyMode == FlyMode.None)
      {
        if (this.isOnLadder)
        {
          this.Velocity.Y = this.Acceleration * this.movementInput.Y;
          if ((double) this.ViewDirection.Y < 0.0)
            this.Velocity.Y *= -1f;
        }
        else if (this.isOnRope && (double) this.releaseRopeTimer == 0.0)
        {
          this.Velocity.Y = (float) ((double) this.Acceleration * (double) this.movementInput.Y * 3.0);
          if ((double) this.ViewDirection.Y < 0.0)
            this.Velocity.Y *= -1f;
          this.Velocity.X = this.Velocity.Z = 0.0f;
          Vector3 blockCenter = this.map.GetBlockCenter(underFootPoint);
          Vector2 vector2 = new Vector2(this.ViewDirection.X, this.ViewDirection.Z);
          if ((double) vector2.X == 0.0 && (double) vector2.Y == 0.0)
            vector2.Y = 1f;
          else
            vector2.Normalize();
          this.Position.X = blockCenter.X - vector2.X * 0.3f;
          this.Position.Z = blockCenter.Z - vector2.Y * 0.3f;
        }
      }
      if (this.FlyMode != FlyMode.None)
      {
        if (this.isFlightAscending)
          this.accVel.Y -= GameInstance.Gravity * 0.6f;
        else if (this.isFlightDescending)
          this.accVel.Y += GameInstance.Gravity * 0.6f;
        else if ((double) this.accVel.Y < 0.0)
        {
          this.accVel.Y += 0.06f;
          if ((double) this.accVel.Y > 0.0)
            this.accVel.Y = 0.0f;
        }
        else
        {
          this.accVel.Y -= 0.06f;
          if ((double) this.accVel.Y < 0.0)
            this.accVel.Y = 0.0f;
        }
        this.Velocity.Y = this.accVel.Y;
      }
      else
      {
        bool flag1 = footBlockID == Block.Water && midBlockID == Block.Water && eyeBlockID == Block.Water;
        if (this.jumpingInput && !this.isOnLadder)
        {
          if (!this.isOnRope || (double) num1 > 0.00039999998989515 && (double) this.releaseRopeTimer == 0.0)
          {
            float num9 = this.JumpSpeed * 0.5f;
            if ((double) this.Velocity.Y > -0.100000001490116)
            {
              bool flag2 = footBlockID == Block.Water || midBlockID == Block.Water || eyeBlockID == Block.Water;
              bool flag3 = flag2 && this.IsInLiquidButNextToLand;
              if (flag1 && !flag3)
              {
                num9 *= 0.025f;
                this.Velocity.Y += num9;
                this.Velocity.Y = MathHelper.Clamp(this.Velocity.Y, Math.Min(0.1f, this.Velocity.Y), 0.1f);
              }
              else if (flag2)
              {
                this.Velocity.Y += num9 * (flag3 ? 0.4f : 0.2f);
                float max = flag3 ? 0.15f : 0.05f;
                this.Velocity.Y = MathHelper.Clamp(this.Velocity.Y, -max, max);
              }
              else
                this.Velocity.Y = num9;
            }
            else if (flag1 && (double) this.Velocity.Y < -0.0599999986588955)
              this.Velocity.Y += (float) ((double) this.Velocity.Y * -0.100000001490116 + (double) num9 * 0.025000000372529);
            if (this.isOnRope)
            {
              this.releaseRopeTimer = 1.5f;
              this.Velocity.Y = num9 * 0.3f;
            }
          }
        }
        else if (!this.isOnLadder && flag1 && (double) this.Velocity.Y < -0.0599999986588955)
          this.Velocity.Y += this.Velocity.Y * -0.1f;
        if (eyeBlockID == Block.Water || eyeBlockID == Block.Lava)
        {
          Vector3 liquidFlowDirection = this.map.GetLiquidFlowDirection(this.map.GetPoint(this.EyePosition));
          this.Velocity.X += liquidFlowDirection.X * 0.1f;
          this.Velocity.Z += liquidFlowDirection.Z * 0.1f;
        }
        else if (footBlockID == Block.Water || footBlockID == Block.Lava)
        {
          Vector3 liquidFlowDirection = this.map.GetLiquidFlowDirection(this.map.GetPoint(this.Position));
          this.Velocity.X += liquidFlowDirection.X * 0.1f;
          this.Velocity.Z += liquidFlowDirection.Z * 0.1f;
        }
      }
      if (!this.isOnRope && this.wasOnRope && (double) this.Velocity.Y < 0.0)
        this.Velocity.Y = 0.0f;
      this.wasOnRope = this.isOnRope;
    }

    private void UpdateControlPhysicsFPSCinema(
      GlobalPoint3D underFootPoint,
      Block footBlockID,
      Block midBlockID,
      Block eyeBlockID,
      float speedModifier)
    {
      bool flag1 = this.FlyMode != FlyMode.None;
      Vector3 viewDirection = this.ViewDirection;
      viewDirection.Y *= 0.05f;
      viewDirection.Normalize();
      float num1 = this.Acceleration * speedModifier;
      float num2 = this.movementInput.Y * num1 * MathHelper.Lerp(0.1f, 1f, Math.Abs(this.movementInput.Y));
      float num3 = viewDirection.X * num2;
      float num4 = viewDirection.Z * num2;
      Matrix rotationY = Matrix.CreateRotationY(-1.570796f);
      Vector3 vector3 = Vector3.Transform(viewDirection, rotationY);
      float num5 = (float) ((double) this.movementInput.X * (double) num1 * 0.800000011920929) * MathHelper.Lerp(0.1f, 1f, Math.Abs(this.movementInput.X));
      float num6 = num3 + vector3.X * num5;
      float num7 = num4 + vector3.Z * num5;
      this.accVel.X += num6 + this.KnockForce.X;
      this.accVel.Z += num7 + this.KnockForce.Z;
      this.Velocity.X = this.accVel.X;
      this.Velocity.Z = this.accVel.Z;
      float num8 = this.map.BlockData[(int) this.map.GetBlockID(underFootPoint)].Friction;
      if (flag1)
      {
        if (this.FlyMode == FlyMode.Slow)
          num8 *= 2f;
        else if ((double) this.movementInput.LengthSquared() > 0.850000023841858)
          num8 *= 0.1f;
      }
      else if (this.isOnLadder)
        num8 = 1f;
      float num9 = this.accVel.X * num8;
      float num10 = this.accVel.Z * num8;
      this.accVel.X -= num9;
      this.accVel.Z -= num10;
      if ((double) num9 < 0.0)
      {
        if ((double) this.accVel.X > 0.0)
          this.accVel.X = 0.0f;
      }
      else if ((double) num9 > 0.0 && (double) this.accVel.X < 0.0)
        this.accVel.X = 0.0f;
      if ((double) num10 < 0.0)
      {
        if ((double) this.accVel.Z > 0.0)
          this.accVel.Z = 0.0f;
      }
      else if ((double) num10 > 0.0 && (double) this.accVel.Z < 0.0)
        this.accVel.Z = 0.0f;
      if (!this.isOnLadder)
      {
        this.isOnLadder = this.StandingNextTo(Block.Scaffold);
        if (!this.isOnLadder && this.IsItemEquippedAndUsable(Item.SpiderRing))
        {
          Block oneUnpassableBlock = this.IsStandingNextToAtleastOneUnpassableBlock();
          this.isOnLadder = oneUnpassableBlock != Block.None && oneUnpassableBlock != Block.InvisibleBarrier;
        }
      }
      if ((double) this.releaseRopeTimer > 0.0)
      {
        this.releaseRopeTimer -= Services.ElapsedTime;
        if ((double) this.releaseRopeTimer < 0.0 || (double) this.Velocity.Y < -0.200000002980232)
          this.releaseRopeTimer = 0.0f;
      }
      if (!flag1)
      {
        if (this.isOnLadder)
        {
          this.Velocity.Y = this.Acceleration * this.movementInput.Y;
          if ((double) this.ViewDirection.Y < 0.0)
            this.Velocity.Y *= -1f;
        }
        else if (this.isOnRope && (double) this.releaseRopeTimer == 0.0)
        {
          this.Velocity.Y = (float) ((double) this.Acceleration * (double) this.movementInput.Y * 3.0);
          if ((double) this.ViewDirection.Y < 0.0)
            this.Velocity.Y *= -1f;
          this.Velocity.X = this.Velocity.Z = 0.0f;
          Vector3 blockCenter = this.map.GetBlockCenter(underFootPoint);
          Vector2 vector2 = new Vector2(this.ViewDirection.X, this.ViewDirection.Z);
          if ((double) vector2.X == 0.0 && (double) vector2.Y == 0.0)
            vector2.Y = 1f;
          else
            vector2.Normalize();
          this.Position.X = blockCenter.X - vector2.X * 0.3f;
          this.Position.Z = blockCenter.Z - vector2.Y * 0.3f;
        }
        if (eyeBlockID == Block.Water || eyeBlockID == Block.Lava)
        {
          Vector3 liquidFlowDirection = this.map.GetLiquidFlowDirection(this.map.GetPoint(this.EyePosition));
          this.Velocity.X += liquidFlowDirection.X;
          this.Velocity.Z += liquidFlowDirection.Z;
        }
        else if (footBlockID == Block.Water || footBlockID == Block.Lava)
        {
          Vector3 liquidFlowDirection = this.map.GetLiquidFlowDirection(this.map.GetPoint(this.Position));
          this.Velocity.X += liquidFlowDirection.X;
          this.Velocity.Z += liquidFlowDirection.Z;
        }
      }
      if (flag1)
      {
        if (this.isFlightAscending)
          this.accVel.Y -= GameInstance.Gravity * 0.6f;
        else if (this.isFlightDescending)
          this.accVel.Y += GameInstance.Gravity * 0.6f;
        else if ((double) this.accVel.Y < 0.0)
        {
          this.accVel.Y += 0.06f;
          if ((double) this.accVel.Y > 0.0)
            this.accVel.Y = 0.0f;
        }
        else
        {
          this.accVel.Y -= 0.06f;
          if ((double) this.accVel.Y < 0.0)
            this.accVel.Y = 0.0f;
        }
        this.Velocity.Y = this.accVel.Y;
      }
      else
      {
        bool flag2 = footBlockID == Block.Water && midBlockID == Block.Water && eyeBlockID == Block.Water;
        if (this.jumpingInput && !this.isOnLadder)
        {
          float num11 = this.movementInput.LengthSquared();
          if (!this.isOnRope || (double) num11 > 0.00039999998989515 && (double) this.releaseRopeTimer == 0.0)
          {
            float num12 = this.JumpSpeed * 0.5f;
            if ((double) this.Velocity.Y > -0.100000001490116)
            {
              bool flag3 = footBlockID == Block.Water || midBlockID == Block.Water || eyeBlockID == Block.Water;
              bool flag4 = flag3 && this.IsInLiquidButNextToLand;
              if (flag2 && !flag4)
              {
                num12 *= 0.025f;
                this.Velocity.Y += num12;
                this.Velocity.Y = MathHelper.Clamp(this.Velocity.Y, Math.Min(0.1f, this.Velocity.Y), 0.1f);
              }
              else if (flag3)
              {
                this.Velocity.Y += num12 * (flag4 ? 0.4f : 0.2f);
                float max = flag4 ? 0.15f : 0.05f;
                this.Velocity.Y = MathHelper.Clamp(this.Velocity.Y, -max, max);
              }
              else
                this.Velocity.Y = num12;
            }
            else if (flag2 && (double) this.Velocity.Y < -0.0599999986588955)
              this.Velocity.Y += (float) ((double) this.Velocity.Y * -0.100000001490116 + (double) num12 * 0.025000000372529);
            if (this.isOnRope)
            {
              this.releaseRopeTimer = 1.5f;
              this.Velocity.Y = num12 * 0.3f;
            }
          }
        }
        else if (!this.isOnLadder && flag2 && (double) this.Velocity.Y < -0.0599999986588955)
          this.Velocity.Y += this.Velocity.Y * -0.1f;
      }
      if (!this.isOnRope && this.wasOnRope && (double) this.Velocity.Y < 0.0)
        this.Velocity.Y = 0.0f;
      this.wasOnRope = this.isOnRope;
    }

    protected virtual CameraType GetCameraType()
    {
      return CameraType.Original;
    }

    protected bool IsInLiquidButNextToLand
    {
      get
      {
        switch ((Block) this.map.GetBlockID(this.map.GetPoint(this.Position)))
        {
          case Block.Water:
          case Block.Lava:
            Vector3 viewDirection = this.ViewDirection;
            viewDirection.Y = 0.0f;
            if ((double) viewDirection.X != 0.0 || (double) viewDirection.Z != 0.0)
              viewDirection.Normalize();
            viewDirection.Y += 0.05f;
            return !this.IsLiquid(this.map.GetBlockID(this.Position + viewDirection));
          default:
            return false;
        }
      }
    }

    private bool IsLiquid(byte block)
    {
      Block block1 = (Block) block;
      if (block1 != Block.Water)
        return block1 == Block.Lava;
      return true;
    }

    protected virtual bool GetJump()
    {
      return false;
    }

    protected virtual bool GetHaltOnLadder()
    {
      return false;
    }

    protected virtual Vector2 GetMovementInput()
    {
      return Vector2.Zero;
    }

    protected virtual Vector2 GetLookAroundInput()
    {
      return Vector2.Zero;
    }
  }
}
