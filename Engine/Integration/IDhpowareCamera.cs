// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Integration.IDhpowareCamera
// Assembly: StudioForge.Engine.Integration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 77444331-2B4F-47DB-B4ED-8A081283941E
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Integration.dll

using Microsoft.Xna.Framework;

namespace StudioForge.Engine.Integration
{
  public interface IDhpowareCamera : ICamera, IHasUpdate
  {
    void LookAt(Vector3 target);

    void LookAt(Vector3 eye, Vector3 target, Vector3 up);

    void Move(float dx, float dy, float dz);

    void Move(Vector3 direction, Vector3 distance);

    void Perspective(float fovx, float aspect, float znear, float zfar);

    void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees);

    void Zoom(float zoom, float minZoom, float maxZoom, float seconds);

    Quaternion Orientation { get; set; }

    Vector3 ViewDirection { get; }

    Matrix ViewProjectionMatrix { get; }

    Vector3 XAxis { get; }

    Vector3 YAxis { get; }

    Vector3 ZAxis { get; }

    Vector3 Target { get; }

    bool UserMoved { get; }

    Vector3 LastDisplacement { get; }
  }
}
