// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BufferedChangeProcessor
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Net;
using System;

namespace StudioForge.TotalMiner
{
  internal class BufferedChangeProcessor : IThreadWorkItem
  {
    private GameInstance instance;
    private NetworkManager networkManager;
    private MapTM map;
    private bool needsCommit;

    public string Name
    {
      get
      {
        return nameof (BufferedChangeProcessor);
      }
    }

    public bool IsSleeping
    {
      get
      {
        return false;
      }
    }

    public bool CanWait
    {
      get
      {
        return true;
      }
    }

    public void Initialize(GameInstance instance, NetworkManager networkManager)
    {
      this.instance = instance;
      this.networkManager = networkManager;
      this.map = instance.Map;
    }

    public void Update()
    {
      this.needsCommit = false;
      do
        ;
      while (this.ProcessNextChange());
      if (!this.needsCommit)
        return;
      this.map.Commit();
    }

    private bool ProcessNextChange()
    {
      NetworkManager.BufferedChangeBase nextBufferedChange = this.networkManager.GetNextBufferedChange();
      if (nextBufferedChange == null)
        return false;
      switch (nextBufferedChange.Type)
      {
        case NetworkManager.BufferedChangeType.BlockChange:
          this.ProcessBufferedBlockChange(nextBufferedChange as NetworkManager.BufferedBlockChange);
          break;
        case NetworkManager.BufferedChangeType.Blast:
          this.ProcessBufferedBlast(nextBufferedChange as NetworkManager.BufferedBlast);
          break;
        case NetworkManager.BufferedChangeType.CreativeCommand:
          this.ProcessBufferedCreative(nextBufferedChange as NetworkManager.BufferedCreativeCommand);
          break;
        case NetworkManager.BufferedChangeType.Zone:
          this.ProcessBufferedZone(nextBufferedChange as NetworkManager.BufferedZone);
          break;
      }
      return true;
    }

    private void ProcessBufferedBlockChange(NetworkManager.BufferedBlockChange change)
    {
      GlobalPoint3D p = new GlobalPoint3D((int) change.X, (int) change.Y, (int) change.Z);
      if (!change.AuxChangeOnly)
      {
        MapBlock blockData = this.map.GetBlockData(p);
        change.BlockData.Chunk = change.OldBlockData.Chunk = blockData.Chunk;
        if ((int) blockData.BlockID != (int) change.BlockData.BlockID || ((int) blockData.AuxData & 247) != ((int) change.BlockData.AuxData & 247))
        {
          if (change.BlockData.BlockID == (byte) 0)
          {
            this.instance.ClearBlock(p, change.Method, change.GamerID, false);
            this.needsCommit = true;
            if (change.Method != UpdateBlockMethod.Player)
              return;
            Player player = this.instance.GetPlayer(change.GamerID);
            if (player == null)
              return;
            Sounds.PlaySound((Item) change.BlockData.BlockID, ItemSoundType.Use, p, (ITMActor) player);
            player.ChangeLog.LogSetBlock(this.instance, player, p, Item.None, (byte) 0);
            player.ActionLog.AddAction((Block) blockData.BlockID);
          }
          else
          {
            this.map.SetBlockData(p, change.BlockData.BlockID, change.BlockData.AuxData, change.Method, change.GamerID, false);
            this.needsCommit = true;
            if (change.Method != UpdateBlockMethod.Player)
              return;
            Player player = this.instance.GetPlayer(change.GamerID);
            if (player == null)
              return;
            Sounds.PlaySound((Item) change.BlockData.BlockID, ItemSoundType.Use, p, (ITMActor) player);
            player.ChangeLog.LogSetBlock(this.instance, player, p, (Item) change.BlockData.BlockID, change.BlockData.AuxData);
            player.ActionLog.AddAction((Item) change.BlockData.BlockID, ItemAction.Used);
          }
        }
        else
          this.map.MapStrategy.BlockChanged(p, change.OldBlockData, change.BlockData, change.Method, change.GamerID, false);
      }
      else
      {
        if (((int) this.map.GetAuxFullData(p) & 247) == ((int) change.BlockData.AuxData & 247))
          return;
        this.map.SetAuxData(p, change.OldBlockData.AuxData, change.BlockData.AuxData, change.Method, change.GamerID, false);
        this.needsCommit = true;
      }
    }

    private void ProcessBufferedBlast(NetworkManager.BufferedBlast blast)
    {
      this.instance.CreateRemoteBlast(new GlobalPoint3D((int) blast.X, (int) blast.Y, (int) blast.Z), blast.ItemID, blast.Strength, (int) blast.Radius, blast.GamerID, blast.Seed);
    }

    private void ProcessBufferedZone(NetworkManager.BufferedZone zone)
    {
      this.instance.EditZone(zone.Name, zone.GamerID, zone.Action, zone.ZoneType, new GlobalPoint3D?(zone.Min), new GlobalPoint3D?(zone.Max), zone.BuilderType, zone.Builder, zone.OnEntryScript, zone.OnExitScript, zone.CombatLevelDifference, zone.SpeedMultiplier, zone.GravityMultiplier);
    }

    private void ProcessBufferedCreative(NetworkManager.BufferedCreativeCommand cmd)
    {
      switch (cmd.Command)
      {
        case CreativeCommand.Clear:
        case CreativeCommand.Fill:
          this.instance.CreativeModeHelper.RunClearFill(cmd.GamerID, cmd.BlockID, cmd.Min, cmd.Max, cmd.XMin, cmd.XMax, cmd.Percent, cmd.Seed, cmd.ClearMarkers, cmd.Desc, true);
          break;
        case CreativeCommand.Replace:
          this.instance.CreativeModeHelper.RunReplace(this.map, cmd.GamerID, cmd.Min, cmd.Max, cmd.XMin, cmd.XMax, cmd.BlockID, cmd.BlockID1, cmd.Percent, cmd.Seed, cmd.ClearMarkers, cmd.Desc, (Action<CreativeOperationData>) null, true);
          break;
        case CreativeCommand.ReplaceTexture:
          this.instance.CreativeModeHelper.RunReplaceTexture(this.map, cmd.GamerID, cmd.Min, cmd.Max, cmd.XMin, cmd.XMax, cmd.BlockID, cmd.BlockID1, cmd.BlockID2, cmd.Percent, cmd.Seed, cmd.ClearMarkers, cmd.Desc, (Action<CreativeOperationData>) null, true);
          break;
        case CreativeCommand.Flood:
          this.instance.FloodPhysics(cmd.Point, cmd.BlockID, cmd.GamerID, false);
          break;
        case CreativeCommand.Line:
          this.instance.CreativeModeHelper.RunGenerateLine(this.map, cmd.GamerID, cmd.BlockID, (byte) cmd.BlockID1, (byte) cmd.BlockID2, cmd.ClearMarkers, cmd.Desc, (Action<CreativeOperationData>) null, true);
          break;
        case CreativeCommand.Sphere:
          this.instance.CreativeModeHelper.RunSetSphere(this.map, cmd.GamerID, cmd.Point, cmd.BlockID, (byte) cmd.BlockID1, cmd.Percent, cmd.Seed, cmd.Desc, (Action<CreativeOperationData>) null, true);
          break;
        case CreativeCommand.Path:
        case CreativeCommand.Wall:
          this.instance.CreativeModeHelper.RunGenerateWall(this.map, cmd.GamerID, cmd.BlockID, (byte) cmd.BlockID1, (byte) cmd.BlockID2, cmd.Desc, (Action<CreativeOperationData>) null, true);
          break;
        case CreativeCommand.Trees:
          this.instance.CreativeModeHelper.RunGenerateTrees(this.map, cmd.GamerID, cmd.Min, cmd.Max, cmd.XMin, cmd.XMax, cmd.Seed, cmd.ClearMarkers, cmd.Desc, (CreativeGenerateTreeData) cmd.Data, (Action<CreativeOperationData>) null, true);
          break;
      }
    }
  }
}
