// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CreativeModeHelper
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Generators;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Net;
using StudioForge.TotalMiner.Screens;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CreativeModeHelper
  {
    public static int MaxRegionBlocks = 500000;
    private MapTM map;
    private GameInstance instance;
    private GlobalPoint3D min;
    private GlobalPoint3D max;
    private GlobalPoint3D xmin;
    private GlobalPoint3D xmax;
    private List<StudioForge.TotalMiner.Blocks.MarkerBlock> MarkerBlocks;

    private ScreenManager ScreenManager
    {
      get
      {
        return TotalMinerGame.Instance.ScreenManager;
      }
    }

    public CreativeModeHelper(GameInstance instance, MapTM map)
    {
      this.map = map;
      this.instance = instance;
      this.MarkerBlocks = ((MapStrategyTM) map.MapStrategy).MarkerBlocks;
    }

    public void RunOperation(CreativeOperationData op, bool threaded)
    {
      this.instance.CreativeCommandQueue.Execute(op, threaded);
    }

    public void RemoveMarkers(GamerID gamerID, bool transmit)
    {
      if (!gamerID.IsGamer)
        return;
      lock (this.MarkerBlocks)
      {
        for (int index = this.MarkerBlocks.Count - 1; index >= 0; --index)
        {
          if (this.MarkerBlocks[index].GamerID == gamerID)
          {
            int num = (int) this.map.ClearBlock(this.MarkerBlocks[index].Point, UpdateBlockMethod.Strategy, gamerID, transmit);
          }
        }
      }
      this.map.Commit();
    }

    public void RunClearFill(
      GamerID gamerID,
      Block blockID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      byte percent,
      int seed,
      bool clearMarkers,
      string desc,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = blockID == Block.None ? CreativeCommand.Clear : CreativeCommand.Fill,
        Map = this.map,
        Progress = 0.0f,
        Desc = desc,
        BlockID = (byte) blockID,
        Min = min,
        Max = max,
        XMin = xmin,
        XMax = xmax,
        Percent = percent,
        Seed = seed,
        ClearMarkers = clearMarkers,
        GamerID = gamerID
      }, threaded);
    }

    public CreativeOperationData GetClearCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeClearDefaults());
      op.Map = this.map;
      op.Progress = 0.0f;
      this.GetMinMax(ref op);
      return op;
    }

    public CreativeOperationData GetFillCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeFillDefaults());
      op.Map = this.map;
      op.Progress = 0.0f;
      this.GetMinMax(ref op);
      return op;
    }

    public void RunMove(
      GamerID gamerID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D to,
      string desc,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Move,
        Map = this.map,
        Progress = 0.0f,
        Desc = desc,
        Point = to,
        Min = min,
        Max = max,
        GamerID = gamerID
      }, threaded);
    }

    public void RunCopy(
      GamerID gamerID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D to,
      string desc,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Copy,
        Map = this.map,
        Progress = 0.0f,
        Desc = desc,
        Point = to,
        Min = min,
        Max = max,
        GamerID = gamerID
      }, threaded);
    }

    public void RunGenerateLine(
      MapTM map,
      GamerID gamerID,
      Block blockID1,
      byte width,
      byte height,
      bool clearMarkers,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Line,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        BlockID = (byte) blockID1,
        BlockID1 = width,
        BlockID2 = height,
        ClearMarkers = clearMarkers,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    public CreativeOperationData GetLineCommandData(Player player)
    {
      return new CreativeOperationData(player.GetCreativeLineDefaults())
      {
        Map = this.map,
        Progress = 0.0f
      };
    }

    public void RunSetSphere(
      MapTM map,
      GamerID gamerID,
      GlobalPoint3D p,
      Block blockID1,
      byte radius,
      byte percent,
      int seed,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Sphere,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        Point = p,
        BlockID = (byte) blockID1,
        BlockID1 = radius,
        Percent = percent,
        Seed = seed,
        ClearMarkers = false,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    public CreativeOperationData GetSphereCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeSphereDefaults());
      op.Map = this.map;
      op.Progress = 0.0f;
      this.GetMinMax(ref op);
      return op;
    }

    public void RunGenerateWall(
      MapTM map,
      GamerID gamerID,
      Block blockID1,
      byte width,
      byte height,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Wall,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        BlockID = (byte) blockID1,
        BlockID1 = width,
        BlockID2 = height,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    public CreativeOperationData GetPathCommandData(Player player)
    {
      return new CreativeOperationData(player.GetCreativePathDefaults())
      {
        Map = this.map,
        Progress = 0.0f,
        ClearMarkers = true
      };
    }

    public CreativeOperationData GetWallCommandData(Player player)
    {
      return new CreativeOperationData(player.GetCreativeWallDefaults())
      {
        Map = this.map,
        Progress = 0.0f,
        ClearMarkers = true
      };
    }

    public void RunGenerateTrees(
      MapTM map,
      GamerID gamerID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      int seed,
      bool clearMarkers,
      string desc,
      CreativeGenerateTreeData data,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      data.TreeModels = CreativeModeHelper.GetTreeModelsToUseForGenerateTrees(data.CompsSelected);
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Trees,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        Min = min,
        Max = max,
        XMin = xmin,
        XMax = xmax,
        Seed = seed,
        Data = (object) data,
        ClearMarkers = clearMarkers,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    public static VegetationGenerator.FloraModelSetup[] GetTreeModelsToUseForGenerateTrees(
      bool[] comps)
    {
      List<string> stringList = new List<string>();
      if (comps[0])
      {
        stringList.Add("Trees_Acacia_Medium");
        stringList.Add("Trees_Acacia_Medium1");
        stringList.Add("Trees_Acacia_Small");
      }
      if (comps[1])
      {
        stringList.Add("Trees_Jungle_Bush");
        stringList.Add("Trees_Jungle_Bush1");
        stringList.Add("Trees_Jungle_Bush2");
        stringList.Add("Trees_Jungle_Large");
        stringList.Add("Trees_Jungle_Large1");
        stringList.Add("Trees_Jungle_Medium");
        stringList.Add("Trees_Jungle_Small");
        stringList.Add("Trees_Jungle_Small1");
      }
      if (comps[2])
      {
        stringList.Add("Trees_Maple_Big");
        stringList.Add("Trees_Maple_Large");
        stringList.Add("Trees_Maple_Large2");
        stringList.Add("Trees_Maple_Medium");
        stringList.Add("Trees_Maple_Medium2");
        stringList.Add("Trees_Maple_Small");
        stringList.Add("Trees_Maple_Small1");
        stringList.Add("Trees_Maple_Small2");
        stringList.Add("Trees_Maple_Small3");
        stringList.Add("Trees_Maple_Small4");
        stringList.Add("Trees_Maple_Tiny");
        stringList.Add("Trees_Maple_Tiny2");
        stringList.Add("Trees_Maple_Tiny3");
        stringList.Add("Trees_Maple_Tiny4");
      }
      if (comps[3])
      {
        stringList.Add("Trees_Oak_Medium");
        stringList.Add("Trees_Oak_Medium1");
        stringList.Add("Trees_Oak_Small");
        stringList.Add("Trees_Oak_Small2");
        stringList.Add("Trees_Oak_Small3");
      }
      if (comps[4])
      {
        stringList.Add("Trees_Original_Fir");
        stringList.Add("Trees_Original_Birch");
        stringList.Add("Trees_Original_Oak Small");
        stringList.Add("Trees_Original_Oak Med");
        stringList.Add("Trees_Original_Oak Bent");
        stringList.Add("Trees_Original_Oak Big");
        stringList.Add("Trees_Original_Willow");
        stringList.Add("Trees_Original_Pine");
        stringList.Add("Trees_Original_Pine Big");
        stringList.Add("Trees_Original_Maple");
        stringList.Add("Trees_Original_Maple Small");
        stringList.Add("Trees_Original_Maple Small2");
        stringList.Add("Trees_Original_Maple Med");
        stringList.Add("Trees_Original_Maple Med2");
        stringList.Add("Trees_Original_Maple Big");
        stringList.Add("Trees_Original_Maple Big2");
        stringList.Add("Trees_Original_Maple Large");
      }
      if (comps[5])
      {
        stringList.Add("Trees_Pine_Big");
        stringList.Add("Trees_Pine_Medium");
        stringList.Add("Trees_Pine_Small");
        stringList.Add("Trees_Pine_Small1");
        stringList.Add("Trees_Pine_Small2");
        stringList.Add("Trees_Pine_Small3");
      }
      VegetationGenerator.FloraModelSetup[] floraModelSetupArray = new VegetationGenerator.FloraModelSetup[stringList.Count];
      for (int index = 0; index < stringList.Count; ++index)
        floraModelSetupArray[index] = new VegetationGenerator.FloraModelSetup()
        {
          ComPack = "System",
          ComName = stringList[index]
        };
      return floraModelSetupArray;
    }

    public CreativeOperationData GetTreesCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeTreesDefaults())
      {
        Map = this.map,
        Progress = 0.0f
      };
      op.Data = (object) new CreativeGenerateTreeData(op.Data as CreativeGenerateTreeData);
      this.GetMinMax(ref op);
      return op;
    }

    public void RunGenerateRiver(
      MapTM map,
      GamerID gamerID,
      byte width,
      byte height,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.River,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        BlockID1 = width,
        BlockID2 = height,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    public void CopyToClipboard(GamerID gamerID, Map.CopyAccess copyAccess)
    {
      if (this.MarkerBlockCount(gamerID) < 2)
      {
        this.ShowInvalidMarkerCountError(gamerID);
      }
      else
      {
        this.SetMinMax(gamerID);
        Player player = this.instance.GetPlayer(gamerID);
        if (player != null && !player.Inventory.HasFreeSlot)
          this.ShowInventoryFullError(player);
        else if ((this.max.X - this.min.X + 1) * (this.max.Z - this.min.Z + 1) * (this.max.Y - this.min.Y + 1) > CreativeModeHelper.MaxRegionBlocks && (player == null || !player.IsGod))
        {
          this.ShowRegionExceedsCapacityError(gamerID);
        }
        else
        {
          this.RemoveMarkers(gamerID, true);
          this.RunOperation(new CreativeOperationData()
          {
            Command = CreativeCommand.CopyToClipboard,
            Map = this.map,
            Progress = 0.0f,
            Desc = "Copy",
            Min = this.min,
            Max = this.max,
            XMin = this.xmin,
            XMax = this.xmax,
            GamerID = gamerID
          }, true);
        }
      }
    }

    public static MapModel CreateClipboardModel(
      GameInstance instance,
      MapTM srcMap,
      GlobalPoint3D pos,
      GlobalPoint3D destOffset,
      GlobalPoint3D copySize,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      GlobalPoint3D edgeBufferSize,
      int facing,
      bool buildMesh,
      GamerID playerID,
      IProgressBar progress)
    {
      GlobalPoint3D size = copySize;
      BoxInt boxInt = new BoxInt();
      boxInt.Max = size + edgeBufferSize;
      Point3D regionSize = new Point3D(boxInt.Max);
      Point3D chunkSize = regionSize;
      MapTM map = new MapTM(instance, "Clipboard", srcMap.TileSize, false, boxInt, boxInt, regionSize, chunkSize, Globals1.BlockData, (int) srcMap.MaxLight, 0, (ushort) 1, 3, (MapStrategy) new MapStrategyTM(instance), true, false);
      map.PregenerateRegions(true, true, (IProgressBar) null);
      map.CopyFrom((Map) srcMap, pos, destOffset, size, xmin, xmax, facing, UpdateBlockMethod.Copy, Map.CopyAccess.Restricted, playerID, progress);
      map.BlockTextures = srcMap.BlockTextures;
      MapModel mapModel = new MapModel(instance, map);
      mapModel.Initialize((InitState) null);
      mapModel.LoadContent(buildMesh);
      return mapModel;
    }

    public void Paste(Player player, Map.CopyType copyType)
    {
      this.Paste(player.GamerID, player.ClipboardModel, this.map.GetPoint(player.ClipboardModel.World), (BlockFace) player.ClipboardModelViewFacing, copyType, true, true);
    }

    public void Paste(
      GamerID gamerID,
      MapModel model,
      GlobalPoint3D p,
      BlockFace facing,
      Map.CopyType copyType,
      bool threaded,
      bool transmit)
    {
      if (model == null)
        return;
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Paste,
        Map = this.map,
        Progress = 0.0f,
        Desc = nameof (Paste),
        GamerID = gamerID,
        Point = p,
        BlockID = (byte) facing,
        BlockID1 = (byte) copyType,
        BlockID2 = transmit ? (byte) 1 : (byte) 0,
        Data = (object) model
      }, threaded);
    }

    public void Undo(Player player)
    {
      player?.UndoLastPaste();
    }

    public void AbortFlood(Player player)
    {
      player?.AbortFloods();
    }

    public void RunFlood(Player player, Block block)
    {
      lock (this.MarkerBlocks)
      {
        int index1 = this.MarkerBlocks.Count - 1;
        for (int index2 = 0; index1 >= 0 && index2 < 20; ++index2)
        {
          StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock = this.MarkerBlocks[index1];
          if (!markerBlock.Exclude && markerBlock.GamerID == player.Gamer.ID)
            this.instance.FloodPhysics(markerBlock.Point, block, markerBlock.GamerID, true);
          --index1;
        }
        this.RemoveMarkers(player.GamerID, false);
      }
    }

    public CreativeOperationData GetFloodCommandData(Player player)
    {
      return new CreativeOperationData(player.GetCreativeFloodDefaults())
      {
        Map = this.map,
        Progress = 0.0f
      };
    }

    public void RunReplace(
      MapTM map,
      GamerID gamerID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      Block blockID1,
      Block blockID2,
      byte percent,
      int seed,
      bool clearMarkers,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.Replace,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        BlockID = (byte) blockID1,
        BlockID1 = (byte) blockID2,
        Percent = percent,
        Seed = seed,
        Min = min,
        Max = max,
        XMin = xmin,
        XMax = xmax,
        ClearMarkers = clearMarkers,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    private void OnClipboardReplaceComplete(CreativeOperationData op)
    {
      op.Map.Regions[0].Chunks[0].LoadMesh(true, true);
    }

    public CreativeOperationData GetReplaceCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeReplaceDefaults());
      op.Map = this.map;
      op.Progress = 0.0f;
      this.GetMinMax(ref op);
      return op;
    }

    public CreativeOperationData GetReplaceClipboardCommandData(Player player)
    {
      return new CreativeOperationData(player.GetCreativeReplaceClipboardDefaults());
    }

    public void RunReplaceTexture(
      MapTM map,
      GamerID gamerID,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      Block blockID,
      Block blockID1,
      Block blockID2,
      byte percent,
      int seed,
      bool clearMarkers,
      string desc,
      Action<CreativeOperationData> onComplete,
      bool threaded)
    {
      this.RunOperation(new CreativeOperationData()
      {
        Command = CreativeCommand.ReplaceTexture,
        Map = map,
        Progress = 0.0f,
        Desc = desc,
        BlockID = (byte) blockID,
        BlockID1 = (byte) blockID1,
        BlockID2 = (byte) blockID2,
        Percent = percent,
        Seed = seed,
        Min = min,
        Max = max,
        XMin = xmin,
        XMax = xmax,
        ClearMarkers = clearMarkers,
        GamerID = gamerID,
        OnCompletion = onComplete
      }, threaded);
    }

    private void OnClipboardReplaceTextureComplete(CreativeOperationData op)
    {
      op.Map.Regions[0].Chunks[0].LoadMesh(true, true);
    }

    public CreativeOperationData GetReplaceTextureCommandData(Player player)
    {
      CreativeOperationData op = new CreativeOperationData(player.GetCreativeReplaceTextureDefaults());
      op.Map = this.map;
      op.Progress = 0.0f;
      this.GetMinMax(ref op);
      return op;
    }

    private void GenerateOresCore(Player player)
    {
      GenerateOptions options = new GenerateOptions()
      {
        Density = 0.5f,
        AreaSize = this.max - this.min + GlobalPoint3D.One
      };
      this.instance.AddScreen((GameScreen) new GenerateOreOptionsScreen(this.instance, player, options, new Action<GamerID, GenerateOptions>(this.GenerateOresCore)), player);
    }

    private void GenerateOresCore(GamerID gamerID, GenerateOptions options)
    {
      try
      {
        CreativeOperationData creativeOperationData = new CreativeOperationData()
        {
          Map = this.map,
          Progress = 0.0f,
          Desc = "Ores",
          Min = this.min,
          Max = this.max,
          GamerID = gamerID,
          Data = (object) options
        };
      }
      catch (OutOfMemoryException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(53, (Exception) ex);
      }
    }

    public void Measure(Player player)
    {
      if (this.MarkerBlockCount(player.GamerID) < 2)
      {
        this.ShowInvalidMarkerCountError(player.GamerID);
      }
      else
      {
        this.SetMinMax(player.Gamer.ID);
        MessageBoxScreenTM messageBoxScreenTm = new MessageBoxScreenTM(string.Format("Measure: {0} x {1} x {2} - {3} blocks\n\nMin: {4}, {5}, {6}\nMax: {7}, {8}, {9}\nCenter: {10}, {11}, {12}", (object) (this.max.X - this.min.X + 1), (object) (this.max.Y - this.min.Y + 1), (object) (this.max.Z - this.min.Z + 1), (object) ((this.max.X - this.min.X + 1) * (this.max.Z - this.min.Z + 1) * (this.max.Y - this.min.Y + 1)), (object) this.min.X, (object) this.min.Y, (object) this.min.Z, (object) this.max.X, (object) this.max.Y, (object) this.max.Z, (object) (float) ((double) (this.max.X - this.min.X) * 0.5 + (double) this.min.X), (object) (float) ((double) (this.max.Y - this.min.Y) * 0.5 + (double) this.min.Y), (object) (float) ((double) (this.max.Z - this.min.Z) * 0.5 + (double) this.min.Z)), "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player);
        messageBoxScreenTm.IsPopup = false;
        this.ScreenManager.AddScreen((GameScreen) messageBoxScreenTm, new PlayerIndex?(player.PlayerIndex));
      }
    }

    private bool CheckBlock(Player player, Block block)
    {
      if (player != null && player.IsGod)
        return true;
      return Globals1.ItemData[(int) block].IsEnabled;
    }

    private bool NoSetupErrors(GamerID gamerID, byte blockID, bool checkInside)
    {
      return this.NoSetupErrors(gamerID, blockID, true, checkInside);
    }

    private bool NoSetupErrors(
      GamerID gamerID,
      byte blockID,
      bool checkRegionSize,
      bool checkInside)
    {
      if (this.MarkerBlockCount(gamerID) < 2)
      {
        this.ShowInvalidMarkerCountError(gamerID);
        return false;
      }
      if (checkRegionSize && (this.max.X - this.min.X + 1) * (this.max.Z - this.min.Z + 1) * (this.max.Y - this.min.Y + 1) > CreativeModeHelper.MaxRegionBlocks)
      {
        this.ShowRegionExceedsCapacityError(gamerID);
        return false;
      }
      BoundingBox box = Globals2.GetBox(this.min, this.max, 0.01f);
      MapStrategyTM mapStrategy = this.map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
      {
        for (int index = 0; index < mapStrategy.Zones.Count; ++index)
        {
          Zone zone = mapStrategy.Zones[index];
          if (zone.HasZoneType(ZoneType.NoEdit) && Globals2.GetBox(zone.Min, zone.Max, 0.01f).Intersects(box))
          {
            Player player = this.instance.GetPlayer(gamerID);
            if (zone.HasZoneType(ZoneType.Spawn) || player != null && !player.IsAdmin && (zone.BuilderType == ZoneBuilderType.Player && player.Gamertag != zone.Builder || zone.BuilderType == ZoneBuilderType.Clan && player.ClanName != zone.Builder))
            {
              this.ShowNonEditZoneError(player);
              return false;
            }
          }
        }
      }
      if (checkInside && !this.map.IsBlockPassable(blockID))
      {
        bool flag = false;
        foreach (Gamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
        {
          Player tag = allEnabledGamer.Tag as Player;
          if (tag != null)
          {
            GlobalPoint3D point1 = this.map.GetPoint(tag.Position);
            GlobalPoint3D point2 = this.map.GetPoint(tag.EyePosition);
            if (this.IsInsideFillArea(point1) || this.IsInsideFillArea(point2))
            {
              this.ShowStandingInsideError(tag);
              flag = true;
            }
          }
        }
        if (flag)
          return false;
      }
      return true;
    }

    private bool IsInsideFillArea(GlobalPoint3D p)
    {
      if (p.X >= this.min.X && p.Y >= this.min.Y && (p.Z >= this.min.Z && p.X <= this.max.X) && p.Y <= this.max.Y)
        return p.Z <= this.max.Z;
      return false;
    }

    public void ShowInvalidMarkerCountError(GamerID gamerID)
    {
      Player player = this.instance.GetPlayer(gamerID);
      if (player == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You must place at least two Marker blocks to define an area", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowInvalidBlockError(Player player)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("The selected block is not valid for this operation", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowStandingInsideError(Player player)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("A player is standing inside the fill area.\nMove outside the fill area and try again.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowNonEditZoneError(Player player)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("This operation would cause changes to a Non Edit Zone.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowRegionExceedsCapacityError(GamerID gamerID)
    {
      Player player = this.instance.GetPlayer(gamerID);
      if (player == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM(string.Format("The marked region is too big.\n\nThe current maximum size is {0} blocks.\nYour region size is {1} blocks.\n\nReduce the region size and try again.", (object) CreativeModeHelper.MaxRegionBlocks, (object) ((this.max.X - this.min.X + 1) * (this.max.Z - this.min.Z + 1) * (this.max.Y - this.min.Y + 1))), "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowInventoryFullError(Player player)
    {
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("You need at least one free inventory slot to equip a clipboard", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    private void ShowOperationInProgress(GamerID gamerID)
    {
      Player player = this.instance.GetPlayer(gamerID);
      if (player == null)
        return;
      this.ScreenManager.AddScreen((GameScreen) new MessageBoxScreenTM("There is already an operation in progress.\nPlease wait for it to finish.", "Ok", (string) null, (string) null, (string) null, CoreGlobals.GameFont, 0.8f, new MessageBoxScreen.DrawMessageBoxBackgroundHandler(GraphicStatics.DrawMessageBoxBackground), player), new PlayerIndex?(player.PlayerIndex));
    }

    public int MarkerBlockCount(GamerID gamerID)
    {
      int excludeCount;
      return this.MarkerBlockCount(gamerID, out excludeCount);
    }

    public int MarkerBlockCount(GamerID gamerID, out int excludeCount)
    {
      int num = 0;
      excludeCount = 0;
      lock (this.MarkerBlocks)
      {
        foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
        {
          if (markerBlock.GamerID == gamerID)
          {
            if (!markerBlock.Exclude)
              ++num;
            else
              ++excludeCount;
          }
        }
      }
      return num;
    }

    public void GetMinMax(GamerID gamerID, out GlobalPoint3D min, out GlobalPoint3D max)
    {
      lock (this.MarkerBlocks)
      {
        min = new GlobalPoint3D();
        max = new GlobalPoint3D();
        min.X = this.GetMinMarkerX(gamerID, false);
        min.Y = this.GetMinMarkerY(gamerID, false);
        min.Z = this.GetMinMarkerZ(gamerID, false);
        max.X = this.GetMaxMarkerX(gamerID, false);
        max.Y = this.GetMaxMarkerY(gamerID, false);
        max.Z = this.GetMaxMarkerZ(gamerID, false);
      }
    }

    public void GetMinMax(
      GamerID gamerID,
      out GlobalPoint3D min,
      out GlobalPoint3D max,
      out GlobalPoint3D xmin,
      out GlobalPoint3D xmax)
    {
      lock (this.MarkerBlocks)
      {
        this.GetMinMax(gamerID, out min, out max);
        xmin = new GlobalPoint3D();
        xmax = new GlobalPoint3D();
        xmin.X = this.GetMinMarkerX(gamerID, true);
        xmin.Y = this.GetMinMarkerY(gamerID, true);
        xmin.Z = this.GetMinMarkerZ(gamerID, true);
        xmax.X = this.GetMaxMarkerX(gamerID, true);
        xmax.Y = this.GetMaxMarkerY(gamerID, true);
        xmax.Z = this.GetMaxMarkerZ(gamerID, true);
      }
    }

    public void GetMinMax(ref CreativeOperationData op)
    {
      lock (this.MarkerBlocks)
      {
        op.Min.X = this.GetMinMarkerX(op.GamerID, false);
        op.Min.Y = this.GetMinMarkerY(op.GamerID, false);
        op.Min.Z = this.GetMinMarkerZ(op.GamerID, false);
        op.Max.X = this.GetMaxMarkerX(op.GamerID, false);
        op.Max.Y = this.GetMaxMarkerY(op.GamerID, false);
        op.Max.Z = this.GetMaxMarkerZ(op.GamerID, false);
        op.XMin.X = this.GetMinMarkerX(op.GamerID, true);
        op.XMin.Y = this.GetMinMarkerY(op.GamerID, true);
        op.XMin.Z = this.GetMinMarkerZ(op.GamerID, true);
        op.XMax.X = this.GetMaxMarkerX(op.GamerID, true);
        op.XMax.Y = this.GetMaxMarkerY(op.GamerID, true);
        op.XMax.Z = this.GetMaxMarkerZ(op.GamerID, true);
      }
    }

    private void SetMinMax(GamerID gamerID)
    {
      lock (this.MarkerBlocks)
      {
        this.min.X = this.GetMinMarkerX(gamerID, false);
        this.min.Y = this.GetMinMarkerY(gamerID, false);
        this.min.Z = this.GetMinMarkerZ(gamerID, false);
        this.max.X = this.GetMaxMarkerX(gamerID, false);
        this.max.Y = this.GetMaxMarkerY(gamerID, false);
        this.max.Z = this.GetMaxMarkerZ(gamerID, false);
        this.xmin.X = this.GetMinMarkerX(gamerID, true);
        this.xmin.Y = this.GetMinMarkerY(gamerID, true);
        this.xmin.Z = this.GetMinMarkerZ(gamerID, true);
        this.xmax.X = this.GetMaxMarkerX(gamerID, true);
        this.xmax.Y = this.GetMaxMarkerY(gamerID, true);
        this.xmax.Z = this.GetMaxMarkerZ(gamerID, true);
      }
    }

    private int GetMinMarkerX(GamerID gamerID, bool exclude)
    {
      int num = int.MaxValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.X < num)
          num = markerBlock.Point.X;
      }
      return num;
    }

    private int GetMinMarkerY(GamerID gamerID, bool exclude)
    {
      int num = int.MaxValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.Y < num)
          num = markerBlock.Point.Y;
      }
      return num;
    }

    private int GetMinMarkerZ(GamerID gamerID, bool exclude)
    {
      int num = int.MaxValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.Z < num)
          num = markerBlock.Point.Z;
      }
      return num;
    }

    private int GetMaxMarkerX(GamerID gamerID, bool exclude)
    {
      int num = int.MinValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.X > num)
          num = markerBlock.Point.X;
      }
      return num;
    }

    private int GetMaxMarkerY(GamerID gamerID, bool exclude)
    {
      int num = int.MinValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.Y > num)
          num = markerBlock.Point.Y;
      }
      return num;
    }

    private int GetMaxMarkerZ(GamerID gamerID, bool exclude)
    {
      int num = int.MinValue;
      foreach (StudioForge.TotalMiner.Blocks.MarkerBlock markerBlock in this.MarkerBlocks)
      {
        if (markerBlock.Exclude == exclude && markerBlock.GamerID == gamerID && markerBlock.Point.Z > num)
          num = markerBlock.Point.Z;
      }
      return num;
    }
  }
}
