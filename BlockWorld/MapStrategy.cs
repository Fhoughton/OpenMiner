// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.MapStrategy
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;

namespace StudioForge.BlockWorld
{
  public abstract class MapStrategy
  {
    protected Map map;

    public virtual void Initialize(Map map)
    {
      this.map = map;
    }

    public void UnloadContent()
    {
      this.UnloadContentCore();
    }

    public virtual void UnloadContentCore()
    {
    }

    public virtual void Begin(Map map)
    {
      this.map = map;
      this.BeginCore();
    }

    public virtual ClearBlockResult GetClearBlockResult(
      GlobalPoint3D p,
      byte blockID,
      UpdateBlockMethod method,
      GamerID playerID,
      bool isRelatedClear)
    {
      return ClearBlockResult.Success;
    }

    protected virtual void BeginCore()
    {
    }

    public abstract void BlockChanged(
      GlobalPoint3D p,
      MapBlock oldBlock,
      MapBlock newBlock,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit);

    public abstract void AuxChanged(
      GlobalPoint3D p,
      byte oldAuxData,
      byte newAuxData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool transmit);

    public abstract void AddLiquidFlow(GlobalPoint3D p, byte blockID, UpdateBlockMethod method);

    public abstract void Update(UpdateState state);
  }
}
