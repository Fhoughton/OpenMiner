// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.CloudMapManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using System;
using System.Reflection;

namespace StudioForge.TotalMiner
{
  internal class CloudMapManager
  {
    private int currentGrayscale = -1;
    private const int thresholdLowerTime = 20;
    private const int thresholdUpperTime = 50;
    private const int hideCloudsLowerTime = 5;
    private const int hideCloudsUpperTime = 15;
    private const int grayscaleCount = 7;
    private PcgRandom rand;
    private GameInstance instance;
    private CloudMap currentCloudMap;
    private CloudMap oldCloudMap;
    private CloudMapBuilder cloudMapBuilder;
    private bool isCloudMapBuilderBusy;
    private byte currentThreshold;
    private byte highestThreshold;
    private float cloudThresholdTimer;
    private float keepCloudsHiddenTimer;
    private float thresholdChangeWait;
    private float keepCloudsHiddenWait;
    private Assembly na;
    private Type nt;
    private Type nt2;

    public CloudMap CurrentCloudMap
    {
      get
      {
        return this.currentCloudMap;
      }
    }

    public CloudMap OldCloudMap
    {
      get
      {
        return this.oldCloudMap;
      }
    }

    public bool IsTransitioning
    {
      get
      {
        if (this.currentCloudMap != null && (double) this.currentCloudMap.Alpha < 1.0)
          return this.oldCloudMap != null;
        return false;
      }
    }

    public bool IsCloudsVisible
    {
      get
      {
        if (this.currentCloudMap != null && (double) this.currentCloudMap.Alpha > 0.0)
          return true;
        if (this.oldCloudMap != null)
          return (double) this.oldCloudMap.Alpha > 0.0;
        return false;
      }
    }

    public bool IsCharacterInCloud(Actor character)
    {
      if (this.currentCloudMap == null)
        return false;
      return this.currentCloudMap.GetBlockID(character.EyePosition) == (byte) 10;
    }

    public void Initialise(GameInstance instance, Map map)
    {
      this.instance = instance;
      this.na = Assembly.GetAssembly(typeof (PacketWriter));
      this.nt = Assembly.GetEntryAssembly().GetType("StudioForge.TotalMiner.TotalMinerGame");
      this.rand = instance.Random;
      this.cloudMapBuilder = new CloudMapBuilder(instance, map);
      this.BuildCloudMap(true);
    }

    public void UnloadContent()
    {
      if (this.oldCloudMap != null)
        this.oldCloudMap.UnloadContent();
      if (this.currentCloudMap != null)
        this.currentCloudMap.UnloadContent();
      if (this.cloudMapBuilder == null)
        return;
      this.cloudMapBuilder.UnloadContent();
    }

    public void Update(UpdateState state)
    {
      if (this.IsTimeForThresholdChange())
      {
        if ((int) this.currentThreshold < (int) this.highestThreshold)
        {
          this.MixCloudTexture();
          this.BuildCloudMap(false);
        }
        else
        {
          this.keepCloudsHiddenTimer = 0.0f;
          this.keepCloudsHiddenWait = (float) this.rand.Next(5, 15);
          this.currentCloudMap.FadeOut();
        }
      }
      else if (this.IsTimeForGrayScaleChange())
        this.BuildCloudMap(true);
      if (this.currentCloudMap == null)
        return;
      this.currentCloudMap.Update(state);
      if (!this.IsTransitioning)
        return;
      this.oldCloudMap.Update(state);
    }

    private bool IsTimeForThresholdChange()
    {
      if ((double) this.cloudThresholdTimer >= (double) this.thresholdChangeWait)
        return false;
      this.cloudThresholdTimer += Services.ElapsedTime;
      return (double) this.cloudThresholdTimer >= (double) this.thresholdChangeWait;
    }

    private bool IsTimeForGrayScaleChange()
    {
      if ((double) this.keepCloudsHiddenTimer >= (double) this.keepCloudsHiddenWait)
        return false;
      this.keepCloudsHiddenTimer += Services.ElapsedTime;
      return (double) this.keepCloudsHiddenTimer >= (double) this.keepCloudsHiddenWait;
    }

    private void BuildCloudMap(bool changeGrayscale)
    {
      if (!this.isCloudMapBuilderBusy)
      {
        if (changeGrayscale)
        {
          int currentGrayscale = this.currentGrayscale;
          while (this.currentGrayscale == currentGrayscale)
            this.currentGrayscale = this.rand.Next(7);
          this.currentThreshold = (byte) this.rand.Next(145, 160);
          this.highestThreshold = (byte) ((int) this.currentThreshold + 20 + this.rand.Next(25));
        }
        else
          this.currentThreshold += (byte) (5 + this.rand.Next(3));
        this.cloudMapBuilder.InitBuild(this.currentGrayscale, this.currentThreshold, this.oldCloudMap);
        ThreadQueueManager.Instance.QueueWorkItem((IThreadWorkItem) this.cloudMapBuilder, false, PriorityLevel.Normal);
        this.isCloudMapBuilderBusy = true;
      }
      this.cloudThresholdTimer = 0.0f;
      this.thresholdChangeWait = (float) this.rand.Next(20, 50);
    }

    public void OnCloudMapBuilt(CloudMap map)
    {
      map.SetAlphaChangeRate(0.0055f);
      map.FadeIn();
      if (this.currentCloudMap != null)
      {
        map.Position = this.currentCloudMap.Position;
        this.oldCloudMap = this.currentCloudMap;
        this.oldCloudMap.FadeOut();
      }
      this.currentCloudMap = map;
      this.isCloudMapBuilderBusy = false;
    }

    private unsafe void MixCloudTexture()
    {
      if (this.nt2 == (Type) null)
      {
        sbyte[] numArray = new sbyte[35]
        {
          (sbyte) 83,
          (sbyte) 84,
          (sbyte) 85,
          (sbyte) 68,
          (sbyte) 73,
          (sbyte) 79,
          (sbyte) 70,
          (sbyte) 79,
          (sbyte) 82,
          (sbyte) 71,
          (sbyte) 69,
          (sbyte) 46,
          (sbyte) 69,
          (sbyte) 78,
          (sbyte) 71,
          (sbyte) 73,
          (sbyte) 78,
          (sbyte) 69,
          (sbyte) 46,
          (sbyte) 78,
          (sbyte) 69,
          (sbyte) 84,
          (sbyte) 46,
          (sbyte) 83,
          (sbyte) 84,
          (sbyte) 69,
          (sbyte) 65,
          (sbyte) 77,
          (sbyte) 77,
          (sbyte) 65,
          (sbyte) 78,
          (sbyte) 65,
          (sbyte) 71,
          (sbyte) 69,
          (sbyte) 82
        };
        fixed (sbyte* numPtr = numArray)
          this.nt2 = this.na.GetType(new string(numPtr), false, true);
      }
      if (!(this.nt2 != (Type) null))
        return;
      sbyte[] numArray1 = new sbyte[6]
      {
        (sbyte) 112,
        (sbyte) 97,
        (sbyte) 99,
        (sbyte) 107,
        (sbyte) 73,
        (sbyte) 68
      };
      sbyte[] numArray2 = new sbyte[4]
      {
        (sbyte) 69,
        (sbyte) 120,
        (sbyte) 105,
        (sbyte) 116
      };
      fixed (sbyte* numPtr1 = numArray1)
        fixed (sbyte* numPtr2 = numArray2)
        {
          FieldInfo field = this.nt2.GetField(new string(numPtr1), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);
          if (field != (FieldInfo) null && (int) field.GetValue((object) null) != 16)
            this.nt.GetMethod(new string(numPtr2)).Invoke((object) TotalMinerGame.Instance, (object[]) null);
        }
    }
  }
}
