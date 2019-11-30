// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMMeshBuilder
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.BlockWorld;

namespace StudioForge.TotalMiner.API
{
  public interface ITMMeshBuilder
  {
    Vector2[] TexCoords1 { get; }

    Vector2[] TexCoords2 { get; }

    Vector2[] TexCoords3 { get; }

    Vector2[] TexCoords4 { get; }

    void AddVertex(
      float x,
      float y,
      float z,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p);

    void AddVertex(
      Vector3 pos,
      int face,
      float tx,
      float ty,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p);

    void AddVertex(
      float x,
      float y,
      float z,
      int face,
      NormalizedShort2 tc,
      byte blockID,
      byte aux,
      ref GlobalPoint3D p);

    void AddVertex(ref AVParams data);

    bool IsClear(GlobalPoint3D p, byte blockID, int aux, int face);

    void RotateTexCoords(
      ref GlobalPoint3D p,
      byte face,
      ref Vector2 tc1,
      ref Vector2 tc2,
      ref Vector2 tc3,
      ref Vector2 tc4);
  }
}
