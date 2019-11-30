// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.LocalSpace
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Integration;

namespace StudioForge.Engine.Core
{
  public class LocalSpace : ILocalSpace
  {
    private Vector3 side;
    private Vector3 up;
    private Vector3 forward;
    private Vector3 position;

    public Vector3 Side
    {
      get
      {
        return this.side;
      }
      set
      {
        this.side = value;
      }
    }

    public Vector3 Up
    {
      get
      {
        return this.up;
      }
      set
      {
        this.up = value;
      }
    }

    public Vector3 Forward
    {
      get
      {
        return this.forward;
      }
      set
      {
        this.CheckForwardSet(value);
        this.forward = value;
      }
    }

    protected virtual void CheckForwardSet(Vector3 value)
    {
    }

    public Vector3 Position
    {
      get
      {
        return this.position;
      }
      set
      {
        this.CheckPositionSet(value);
        this.position = value;
      }
    }

    protected virtual void CheckPositionSet(Vector3 value)
    {
      if (float.IsNaN(value.X))
        throw new BadValueException("Position (NaN)", (object) value);
    }

    public bool IsRightHanded
    {
      get
      {
        return true;
      }
    }

    public LocalSpace()
    {
      this.ResetLocalSpace();
    }

    public LocalSpace(Vector3 Side, Vector3 Up, Vector3 Forward, Vector3 Position)
    {
      this.side = Side;
      this.up = Up;
      this.forward = Forward;
      this.Position = Position;
    }

    public LocalSpace(Vector3 Up, Vector3 Forward, Vector3 Position)
    {
      this.up = Up;
      this.forward = Forward;
      this.Position = Position;
      this.SetUnitSideFromForwardAndUp();
    }

    public void ResetLocalSpace()
    {
      this.ResetLocalSpace(Vector3.Zero, Vector3.Backward);
    }

    public void ResetLocalSpace(Vector3 pos, Vector3 forward)
    {
      this.forward = forward;
      this.side = this.LocalRotateForwardToSide(forward);
      this.up = Vector3.Up;
      this.Position = pos;
    }

    public Vector3 LocalizeDirection(Vector3 globalDirection)
    {
      return new Vector3(Vector3.Dot(globalDirection, this.side), Vector3.Dot(globalDirection, this.up), Vector3.Dot(globalDirection, this.forward));
    }

    public Vector3 LocalizePosition(Vector3 globalPosition)
    {
      return this.LocalizeDirection(globalPosition - this.Position);
    }

    public Vector3 GlobalizePosition(Vector3 localPosition)
    {
      return this.Position + this.GlobalizeDirection(localPosition);
    }

    public Vector3 GlobalizeDirection(Vector3 localDirection)
    {
      return this.side * localDirection.X + this.up * localDirection.Y + this.forward * localDirection.Z;
    }

    public void SetUnitSideFromForwardAndUp()
    {
      this.side = !this.IsRightHanded ? Vector3.Cross(this.up, this.forward) : Vector3.Cross(this.forward, this.up);
      this.side.Normalize();
    }

    public void RegenerateOrthonormalBasisUF(Vector3 newUnitForward)
    {
      this.forward = newUnitForward;
      this.SetUnitSideFromForwardAndUp();
      if (this.IsRightHanded)
        this.up = Vector3.Cross(this.side, this.forward);
      else
        this.up = Vector3.Cross(this.forward, this.side);
    }

    public void RegenerateOrthonormalBasis(Vector3 newForward)
    {
      newForward.Normalize();
      this.RegenerateOrthonormalBasisUF(newForward);
    }

    public void RegenerateOrthonormalBasis(Vector3 newForward, Vector3 newUp)
    {
      this.up = newUp;
      newForward.Normalize();
      this.RegenerateOrthonormalBasis(newForward);
    }

    public Vector3 LocalRotateForwardToSide(Vector3 value)
    {
      return new Vector3(this.IsRightHanded ? -value.Z : value.Z, value.Y, value.X);
    }

    public Vector3 GlobalRotateForwardToSide(Vector3 value)
    {
      return this.GlobalizeDirection(this.LocalRotateForwardToSide(this.LocalizeDirection(value)));
    }
  }
}
