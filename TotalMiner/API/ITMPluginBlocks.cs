// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.API.ITMPluginBlocks
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;

namespace StudioForge.TotalMiner.API
{
  public interface ITMPluginBlocks
  {
    void InitializeGame(ITMGame game);

    DataBlock NewDataBlock(GlobalPoint3D p, Block blockID, GamerID playerID);

    bool IsCustomMesh(byte blockID);

    byte GetMeshBlockID(byte blockID);

    BoundingBox GetBlockBox(GlobalPoint3D p, Block blockID);

    byte GetAuxForPlacement(
      Vector3 viewDirection,
      GlobalPoint3D swingTarget,
      BlockFace swingFace,
      int facePos,
      Block blockID);

    void BuildCustomMesh(ITMMeshBuilder meshBuilder, ITMMap map, GlobalPoint3D p, byte blockID);
  }
}
