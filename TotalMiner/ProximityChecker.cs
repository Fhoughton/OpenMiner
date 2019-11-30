// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ProximityChecker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Blocks;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class ProximityChecker : TimedThreadWorkItem
  {
    private GameInstance instance;
    private List<Zone> zones;
    private Dictionary<GamerID, List<Zone>> lastPlayerZones;
    private Dictionary<GamerID, List<ProximityDetectorBlock>> lastPlayerProximities;

    public override string Name
    {
      get
      {
        return nameof (ProximityChecker);
      }
    }

    public ProximityChecker(GameInstance instance, PriorityLevel priority)
      : base(priority, 100)
    {
      this.instance = instance;
      this.zones = new List<Zone>();
      this.lastPlayerZones = new Dictionary<GamerID, List<Zone>>();
      this.lastPlayerProximities = new Dictionary<GamerID, List<ProximityDetectorBlock>>();
    }

    protected override void UpdateCore()
    {
      if (!this.instance.IsMapActive)
        return;
      this.UpdateZoneChecks();
      this.UpdateProximityDetectors();
    }

    private void UpdateZoneChecks()
    {
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
      {
        localEnabledPlayer.DisableFlyIfInNoFlyZone();
        this.CheckForZoneEntry(localEnabledPlayer);
      }
      foreach (Gamer allEnabledGamer in this.instance.NetworkManager.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null)
        {
          short combatLevelDifference;
          float speedModifier;
          float gravityModifier;
          this.instance.MapStrategyTM.GetZoneModifiers(tag.Box, out combatLevelDifference, out speedModifier, out gravityModifier);
          if (combatLevelDifference == (short) 0)
            combatLevelDifference = Globals2.GameProperties.SaveGame.Header.CombatLevelDifference;
          tag.CurrentZoneCombatLevelDifference = combatLevelDifference;
          tag.SpeedMultiplier = speedModifier;
          tag.GravityMultiplier = gravityModifier;
        }
      }
    }

    private void CheckForZoneEntry(Player player)
    {
      List<Zone> zoneList;
      lock (this.lastPlayerZones)
      {
        this.lastPlayerZones.TryGetValue(player.GamerID, out zoneList);
        if (zoneList == null)
        {
          zoneList = new List<Zone>();
          this.lastPlayerZones.Add(player.GamerID, zoneList);
        }
      }
      lock (this.zones)
      {
        this.zones.Clear();
        this.instance.MapStrategyTM.GetZones(player.Box, this.zones);
        ScriptExecuteData data = new ScriptExecuteData()
        {
          Actor = (Actor) player
        };
        foreach (Zone zone in this.zones)
        {
          if (!zoneList.Contains(zone))
          {
            zoneList.Add(zone);
            this.instance.ExecuteScript(zone.OnEntryScriptName, data, true);
          }
        }
        for (int index = zoneList.Count - 1; index >= 0; --index)
        {
          Zone zone = zoneList[index];
          if (!this.zones.Contains(zone))
          {
            zoneList.RemoveAt(index);
            this.instance.ExecuteScript(zone.OnExitScriptName, data, true);
          }
        }
      }
    }

    public void ZoneDeleted(Zone zone)
    {
      lock (this.lastPlayerZones)
      {
        lock (this.zones)
        {
          foreach (KeyValuePair<GamerID, List<Zone>> lastPlayerZone in this.lastPlayerZones)
            lastPlayerZone.Value.Remove(zone);
          this.zones.Remove(zone);
        }
      }
    }

    private List<ProximityDetectorBlock> GetProximityEntry(Actor character)
    {
      lock (this.lastPlayerProximities)
      {
        List<ProximityDetectorBlock> proximityDetectorBlockList;
        this.lastPlayerProximities.TryGetValue(character.GamerID, out proximityDetectorBlockList);
        if (proximityDetectorBlockList == null)
        {
          proximityDetectorBlockList = new List<ProximityDetectorBlock>();
          this.lastPlayerProximities.Add(character.GamerID, proximityDetectorBlockList);
        }
        return proximityDetectorBlockList;
      }
    }

    private void UpdateProximityDetectors()
    {
      List<ProximityDetectorBlock> proximityDetectors = this.instance.MapStrategyTM.ProximityDetectors;
      List<Actor> moveableCharacters = this.instance.AllMoveableCharacters;
      ScriptExecuteData data = new ScriptExecuteData();
      bool isHost = this.instance.IsHost;
      lock (proximityDetectors)
      {
        foreach (ProximityDetectorBlock proximityDetectorBlock in proximityDetectors)
        {
          if (proximityDetectorBlock.IsActive)
          {
            int num = ((int) proximityDetectorBlock.Range + 1) * ((int) proximityDetectorBlock.Range + 1);
            Vector3 blockCenter = this.instance.Map.GetBlockCenter(proximityDetectorBlock.Point);
            data.BlockOffset = new GlobalPoint3D?(proximityDetectorBlock.Point);
            Actor actor = (Actor) null;
            for (int index = moveableCharacters.Count - 1; index >= 0; --index)
            {
              Actor character = moveableCharacters[index];
              if (character != null && character.IsEnabledField && (isHost || character.IsPlayer))
              {
                bool flag = (double) Vector3.DistanceSquared(character.Position, blockCenter) < (double) num;
                if (!flag && character.IsPlayer)
                  flag = (double) Vector3.DistanceSquared(character.EyePosition, blockCenter) < (double) num;
                data.Actor = character;
                if (flag)
                {
                  if (character.IsPlayer)
                  {
                    if (proximityDetectorBlock.IsTargeting(character.IsAdmin ? BlockTargetTypes.Admins : BlockTargetTypes.Players))
                    {
                      List<ProximityDetectorBlock> proximityEntry = this.GetProximityEntry(character);
                      if (!proximityEntry.Contains(proximityDetectorBlock))
                      {
                        proximityEntry.Add(proximityDetectorBlock);
                        this.instance.ExecuteScript(proximityDetectorBlock.OnEntryScriptName, data, false);
                      }
                    }
                    else
                      continue;
                  }
                  else if (!proximityDetectorBlock.IsTargeting(BlockTargetTypes.Mobs))
                    continue;
                  actor = character;
                }
                else if (character.IsPlayer)
                {
                  List<ProximityDetectorBlock> proximityEntry = this.GetProximityEntry(character);
                  if (proximityEntry.Contains(proximityDetectorBlock))
                  {
                    proximityEntry.Remove(proximityDetectorBlock);
                    this.instance.ExecuteScript(proximityDetectorBlock.OnExitScriptName, data, false);
                  }
                }
              }
            }
            if (isHost)
            {
              bool flag = this.instance.MapStrategyTM.IsBlockDeliveringPower(proximityDetectorBlock.Point);
              if (actor != null)
              {
                if (!flag)
                  this.instance.DeliverPower(proximityDetectorBlock.Point, Block.ProximityDetector, BlockFace.ProxyDefault, true, UpdateBlockMethod.Strategy, GamerID.Sys1, true, false);
              }
              else if (flag)
                this.instance.DeliverPower(proximityDetectorBlock.Point, Block.ProximityDetector, BlockFace.ProxyDefault, false, UpdateBlockMethod.Strategy, GamerID.Sys1, true, false);
            }
          }
        }
      }
    }
  }
}
