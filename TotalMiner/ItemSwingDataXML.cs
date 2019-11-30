// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ItemSwingDataXML
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;

namespace StudioForge.TotalMiner
{
  public struct ItemSwingDataXML
  {
    public ItemSwingType SwingType;
    public bool IsSwingable;
    public float SwingTime;
    public Vector3 RestPosition;
    public Vector3 RestRotation;
    public Vector3 ExtendedPosition;
    public Vector3 ExtendedPositionFPV;
    public Vector3 ExtendedRotation;
    public Vector3 ExtendedRotationFPV;
    public float CircularY;
    public float CircularZ;
    public float CircularYFPV;
  }
}
