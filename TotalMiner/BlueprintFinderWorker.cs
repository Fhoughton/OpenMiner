// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BlueprintFinderWorker
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;

namespace StudioForge.TotalMiner
{
  internal class BlueprintFinderWorker : TimedThreadWorkItem
  {
    private MapTM map;
    private GameInstance instance;
    private BoundingFrustum frustum;

    public override string Name
    {
      get
      {
        return "BlueprintFinder";
      }
    }

    public BlueprintFinderWorker(GameInstance instance, MapTM map)
      : base(PriorityLevel.Normal, 1000)
    {
      this.map = map;
      this.instance = instance;
      this.frustum = new BoundingFrustum(Matrix.Identity);
    }

    protected override void UpdateCore()
    {
      if (!this.instance.IsMapActive)
        return;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
      {
        Player virtualPlayer = localEnabledPlayer.VirtualPlayer;
        this.SetClosestBlueprint(localEnabledPlayer, virtualPlayer);
      }
    }

    private void TryToPlaceRemainingBlueprints()
    {
      for (int index = 0; index < this.instance.BlueprintsToPlace.Count; ++index)
      {
        Blueprint bp = this.instance.BlueprintsToPlace[index];
        if (!bp.IsGenerated)
        {
          MapChunk chunkWithinDepth = this.map.GetChunkWithinDepth(bp.Depth);
          if (chunkWithinDepth != null && DigDeepBiome2.PlaceBlueprint(this.instance, this.map, bp, chunkWithinDepth, this.map.Random))
            break;
        }
        else
        {
          this.instance.BlueprintsToPlace.RemoveAt(index);
          break;
        }
      }
    }

    private void SetClosestBlueprint(Player player, Player virtualPlayer)
    {
      Blueprint blueprint1 = (Blueprint) null;
      Blueprint blueprint2 = (Blueprint) null;
      float num1 = float.MinValue;
      float num2 = float.MaxValue;
      float num3 = 900f;
      this.frustum.Matrix = virtualPlayer.ViewMatrix * player.ProjectionMatrix;
      foreach (Blueprint blueprint3 in Blueprints.BlueprintList)
      {
        if (blueprint3.IsGenerated && !blueprint3.IsEnabled && blueprint3.Point.Y > 0)
        {
          if ((double) blueprint3.Point.Y > (double) num1)
          {
            num1 = (float) blueprint3.Point.Y;
            blueprint1 = blueprint3;
          }
          float num4 = Vector3.DistanceSquared(virtualPlayer.EyePosition, this.map.GetBlockCenter(blueprint3.Point));
          if ((double) num4 < (double) num2 && (this.frustum.IsBlockBoxInFrustum(this.instance, blueprint3.Point) || (double) num4 < (double) num3))
          {
            num2 = num4;
            blueprint2 = blueprint3;
          }
        }
      }
      Blueprint blueprint4 = blueprint2 ?? blueprint1;
      this.instance.ClosestBlueprints[player.ScreenID] = blueprint4;
    }
  }
}
