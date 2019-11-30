// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.DummyMapStrategy
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;

namespace StudioForge.BlockWorld
{
  public class DummyMapStrategy : MapStrategy
  {
    public override void BlockChanged(
      GlobalPoint3D p,
      MapBlock oldBlock,
      MapBlock newBlock,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
    }

    public override void AuxChanged(
      GlobalPoint3D p,
      byte oldAuxData,
      byte newAuxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit)
    {
    }

    public override void AddLiquidFlow(GlobalPoint3D p, byte blockID, UpdateBlockMethod method)
    {
    }

    public override void Update(UpdateState state)
    {
    }
  }
}
