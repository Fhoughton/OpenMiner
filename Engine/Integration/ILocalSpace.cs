// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.ILocalSpace
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Integration
{
  public interface ILocalSpace
  {
    Vector3 Side { get; set; }

    Vector3 Up { get; set; }

    Vector3 Forward { get; set; }

    Vector3 Position { get; set; }

    bool IsRightHanded { get; }

    void ResetLocalSpace();

    Vector3 LocalizeDirection(Vector3 globalDirection);

    Vector3 LocalizePosition(Vector3 globalPosition);

    Vector3 GlobalizeDirection(Vector3 localDirection);

    Vector3 GlobalizePosition(Vector3 localPosition);

    void SetUnitSideFromForwardAndUp();

    void RegenerateOrthonormalBasisUF(Vector3 newUnitForward);

    void RegenerateOrthonormalBasis(Vector3 newForward);

    void RegenerateOrthonormalBasis(Vector3 newForward, Vector3 newUp);

    Vector3 LocalRotateForwardToSide(Vector3 value);

    Vector3 GlobalRotateForwardToSide(Vector3 value);
  }
}
