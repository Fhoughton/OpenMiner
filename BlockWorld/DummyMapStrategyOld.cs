// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.DummyMapStrategyOld
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

namespace StudioForge.BlockWorld
{
  public class DummyMapStrategyOld : MapStrategyOld
  {
    public DummyMapStrategyOld(bool isRemote)
      : base(isRemote)
    {
    }

    public override void MapCleared(ushort blockData)
    {
    }

    public override void BlockChanged(
      Point3D p,
      ushort oldBlockData,
      ushort newBlockData,
      UpdateBlockMethod method,
      short playerID,
      bool transmit)
    {
    }

    public override void LightChanged(
      Point3D p,
      ushort blockData,
      byte light,
      UpdateBlockMethod method)
    {
    }

    public override void BeginFlood()
    {
    }

    public override void EndFlood(bool success)
    {
    }

    public override void Update()
    {
    }
  }
}
