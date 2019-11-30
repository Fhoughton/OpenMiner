// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Entity
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  public class Entity
  {
    public Vector3 Position;
    public float Scale;
    public float Radius;
    public bool FrustumCull;
    public float CenterOffY;
    public float DrawRotY;
    public float DrawOffY;
    public Vector3 ViewDirection;
    public int ContentID;

    public virtual void Update()
    {
    }
  }
}
