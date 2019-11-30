// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CaveIn
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.Net;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class CaveIn
  {
    private int caveInChance = 50000;
    private bool active;
    private Map map;
    private int size;
    private int seed;
    private float newPointTimer;
    private float caveInDuration;
    private PcgRandom random;
    private List<CaveIn.CaveInPoint> points;
    private GlobalPoint3D origin;
    private GameInstance instance;
    private Cue cue;

    public bool IsActive
    {
      get
      {
        return this.active;
      }
    }

    public CaveIn(GameInstance instance)
    {
      this.instance = instance;
      this.map = (Map) instance.Map;
      this.points = new List<CaveIn.CaveInPoint>();
    }

    public void UnloadContent()
    {
      if (this.cue == null || this.cue.IsDisposed)
        return;
      this.cue.Dispose();
      this.cue = (Cue) null;
    }

    public void StartNewCaveIn(GlobalPoint3D origin, int seed, bool transmit)
    {
      if (this.active)
        return;
      this.active = true;
      this.seed = seed;
      this.random = new PcgRandom(seed);
      this.caveInDuration = 0.0f;
      this.points.Clear();
      this.origin = origin;
      this.size = this.random.Next(8, 15);
      if (!this.instance.IsHost || !transmit)
        return;
      NetworkManager.Instance.SendCaveInStart(origin, seed);
    }

    public void Update()
    {
      if (!this.active)
      {
        if (!this.instance.IsDigDeepMode && !this.instance.IsSurvivalMode || (this.instance.IsPeacefulDifficulty || !this.instance.IsHost))
          return;
        Player playerIsInCaveInArea = this.GetPlayerIsInCaveInArea(this.instance.Random);
        if (playerIsInCaveInArea == null)
          return;
        this.StartNewCaveIn(this.map.GetPoint(playerIsInCaveInArea.EyePosition), this.instance.Random.Next(), true);
      }
      else
        this.UpdateCore();
    }

    private void UpdateCore()
    {
      if (this.random.Next(100 * this.size) == 0 && (double) this.caveInDuration > 10.0)
      {
        this.EndCaveIn();
      }
      else
      {
        this.caveInDuration += Services.ElapsedTime;
        float intensity = 0.0f;
        foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
        {
          float num1 = Math.Min(1f, this.caveInDuration * 0.1f);
          float num2 = Vector3.Distance(this.map.GetBlockCenter(this.origin), localEnabledPlayer.EyePosition);
          if ((double) num2 < (double) (this.size * 3))
          {
            float num3 = ((float) this.size * 3f - num2) / ((float) this.size * 3f);
            float val1 = num1 * num3;
            if ((double) val1 > (double) intensity)
              intensity = val1;
            localEnabledPlayer.SetNewRumble(1f, Math.Min(val1, 0.7f), Math.Min(val1, 0.7f));
            localEnabledPlayer.ShakeWorld(Math.Min(0.25f, val1 * 0.1f));
          }
        }
        this.UpdateSound(intensity);
        this.newPointTimer -= Services.ElapsedTime;
        if ((double) this.newPointTimer <= 0.0)
          this.FindNewPoint();
        this.UpdatePoints();
      }
    }

    private void UpdateSound(float intensity)
    {
      if ((double) intensity == 0.0)
      {
        if (this.cue == null)
          return;
        this.cue.Dispose();
        this.cue = (Cue) null;
      }
      else
      {
        if (this.cue == null || !this.cue.IsPlaying)
        {
          if (this.cue != null)
            this.cue.Dispose();
          CoreGlobals.AudioManager.PlaySound("EnvCaveIn", out this.cue);
        }
        this.cue.SetVariable("Intensity", intensity);
      }
    }

    private void EndCaveIn()
    {
      this.active = false;
      foreach (Player localEnabledPlayer in this.instance.NetworkManager.LocalEnabledPlayers)
        localEnabledPlayer.ClearWorldShake();
      this.UpdateSound(0.0f);
    }

    private void UpdatePoints()
    {
      float elapsedTime = Services.ElapsedTime;
      int count = this.points.Count;
      for (int index = this.points.Count - 1; index >= 0; --index)
      {
        CaveIn.CaveInPoint point = this.points[index];
        point.Timer += elapsedTime;
        if (this.random.RandomChance(0.1))
        {
          this.instance.AddMiningParticle(point.Point, point.BlockID, BlockFace.Down);
          if (this.random.RandomChance(0.2) && !this.map.HasChanged(point.Point) && !this.map.HasAttachment(point.Point))
          {
            this.instance.CreateSliderBlock(point.Point, GamerID.Sys1, UpdateBlockMethod.Strategy, false);
            this.points.RemoveAt(index);
          }
        }
      }
      if (this.points.Count == count)
        return;
      this.map.Commit();
    }

    private void FindNewPoint()
    {
      int num = 100;
      GlobalPoint3D p = new GlobalPoint3D();
      while (--num > 0)
      {
        if (this.random.Next(1, 2) == 0)
          this.random.Seed(this.seed);
        p.X = this.random.Next(this.size * 2) - this.size;
        p.Z = this.random.Next(this.size * 2) - this.size;
        p.X += this.origin.X;
        p.Y = this.origin.Y;
        p.Z += this.origin.Z;
        Block block;
        for (block = Block.None; block == Block.None && p.Y < (int) this.map.SeaLevel; block = (Block) this.map.GetBlockID(p))
        {
          ++p.Y;
          MapChunk chunk = this.map.GetChunk(p);
          if (chunk == null || !chunk.IsChunkDecorated(chunk))
            return;
        }
        if (block != Block.None && this.map.GetBlockID(p + GlobalPoint3D.Down) == (byte) 0)
        {
          this.points.Add(new CaveIn.CaveInPoint()
          {
            Point = p,
            BlockID = block,
            Timer = 0.0f
          });
          this.newPointTimer = (float) (this.random.NextDouble() * 0.5 + 0.100000001490116);
          break;
        }
      }
    }

    private Player GetPlayerIsInCaveInArea(PcgRandom random)
    {
      foreach (Gamer allEnabledGamer in NetworkManager.Instance.AllEnabledGamers)
      {
        Player tag = allEnabledGamer.Tag as Player;
        if (tag != null && (double) tag.Position.Y < (double) ((int) this.map.SeaLevel - 40) * (double) this.map.TileSize && random.Next(0, this.caveInChance) == 0)
          return tag;
      }
      return (Player) null;
    }

    private struct CaveInPoint
    {
      public GlobalPoint3D Point;
      public Block BlockID;
      public float Timer;
    }
  }
}
