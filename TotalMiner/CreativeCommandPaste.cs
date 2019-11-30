// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeCommandPaste
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Graphics;

namespace StudioForge.TotalMiner
{
  internal class CreativeCommandPaste : CreativeCommandWorkItem
  {
    public CreativeCommandPaste(GameInstance instance)
      : base(instance)
    {
    }

    protected override void UpdateCore()
    {
      GamerID gamerId1 = this.Op.GamerID;
      GlobalPoint3D point = this.Op.Point;
      BlockFace blockId = (BlockFace) this.Op.BlockID;
      Map.CopyType blockId1 = (Map.CopyType) this.Op.BlockID1;
      bool flag = this.Op.BlockID2 > (byte) 0;
      Player player = this.instance.GetPlayer(gamerId1);
      MapModel data = this.Op.Data as MapModel;
      if (data == null)
        return;
      if (point.X < 0)
        point.X += 2;
      if (point.Z < 0)
        point.Z += 2;
      if (point.Y < 0)
        point.Y += 2;
      switch (blockId)
      {
        case BlockFace.Left:
          ++point.X;
          ++point.Z;
          break;
        case BlockFace.Forward:
          --point.X;
          --point.X;
          ++point.Z;
          break;
        case BlockFace.Right:
          --point.X;
          --point.X;
          --point.Z;
          --point.Z;
          break;
        case BlockFace.Backward:
          ++point.X;
          --point.Z;
          --point.Z;
          break;
      }
      ++point.Y;
      GlobalPoint3D one = GlobalPoint3D.One;
      GlobalPoint3D size = data.Map.MapSize - MapModel.EdgeBufferHalf;
      ((IProgressBar) this.Op).Reset();
      VoxelModelManager.MergeBlockTextureIndexes(this.instance, data);
      BoxInt? nullable = data.Map.CopyTo((Map) this.map, one, point, size, this.Op.XMin, this.Op.XMax, (int) blockId, UpdateBlockMethod.Paste, blockId1, Map.CopyAccess.Restricted, gamerId1, false, (IProgressBar) this.Op);
      if (player != null)
      {
        player.ChangeLog.LogPaste(this.instance, player, data.IsSystemModel ? 0 : data.DirNum, data.ComName, this.Op.Point, blockId, blockId1);
        GamerID gamerId2 = player.GamerID;
      }
      if (!nullable.HasValue)
        return;
      this.map.Commit();
      if (!flag)
        return;
      this.instance.NetworkManager.SendChunks(nullable.Value);
    }
  }
}
