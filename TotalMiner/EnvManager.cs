// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.EnvManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.GamerServices;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using System;
using System.Collections.Generic;

namespace StudioForge.TotalMiner
{
  internal class EnvManager : ITMEnvManager
  {
    private List<EnvManager.EnvSection> rainSections = new List<EnvManager.EnvSection>();
    private List<EnvManager.EnvSection> tempRainSections = new List<EnvManager.EnvSection>();
    private List<EnvManager.HailSection> hailSections = new List<EnvManager.HailSection>();
    private List<EnvManager.HailSection> tempHailSections = new List<EnvManager.HailSection>();
    private List<EnvManager.FogSection> fogSections = new List<EnvManager.FogSection>();
    private List<EnvManager.FogSection> tempFogSections = new List<EnvManager.FogSection>();
    private List<float> tempRainIntensities = new List<float>();
    private List<float> tempHailIntensities = new List<float>();
    private List<EnvManager.FogSum> fogSumList = new List<EnvManager.FogSum>();
    private MapTM map;
    private GameInstance instance;
    private float totalRainIntensity;
    private float totalHailIntensity;
    private float totalFireIntensity;
    private float totalLavaIntensity;
    private float totalWaterIntensity;
    private float currentFireIntensity;
    private float currentLavaIntensity;
    private float currentWaterIntensity;
    private float interpolateFireIntensity;
    private float interpolateLavaIntensity;
    private float interpolateWaterIntensity;
    private EnvManager.EnvSound rainSound;
    private EnvManager.EnvSound hailSound;
    private EnvManager.EnvSound fireSound;
    private EnvManager.EnvSound lavaSound;
    private EnvManager.EnvSound waterSound;

    public int FogCount
    {
      get
      {
        return this.fogSections.Count;
      }
    }

    public int RainCount
    {
      get
      {
        return this.rainSections.Count;
      }
    }

    public int HailCount
    {
      get
      {
        return this.hailSections.Count;
      }
    }

    public EnvManager(GameInstance instance, MapTM map)
    {
      this.map = map;
      this.instance = instance;
    }

    public void LoadContent()
    {
      this.rainSound = new EnvManager.EnvSound()
      {
        CueName = "EnvRain"
      };
      this.hailSound = new EnvManager.EnvSound()
      {
        CueName = "EnvHail"
      };
      this.fireSound = new EnvManager.EnvSound()
      {
        CueName = "EnvFire"
      };
      this.lavaSound = new EnvManager.EnvSound()
      {
        CueName = "EnvLava"
      };
      this.waterSound = new EnvManager.EnvSound()
      {
        CueName = "EnvWater"
      };
    }

    public void UnloadContent()
    {
      if (this.rainSound.Cue != null)
        this.rainSound.Cue.Dispose();
      if (this.hailSound.Cue != null)
        this.hailSound.Cue.Dispose();
      if (this.fireSound.Cue != null)
        this.fireSound.Cue.Dispose();
      if (this.lavaSound.Cue != null)
        this.lavaSound.Cue.Dispose();
      if (this.waterSound.Cue == null)
        return;
      this.waterSound.Cue.Dispose();
    }

    public void SetSurroundings(float fire, float lava, float water)
    {
      this.totalFireIntensity = MathHelper.Clamp(fire, 0.0f, 1f);
      this.totalLavaIntensity = MathHelper.Clamp(lava, 0.0f, 1f);
      this.totalWaterIntensity = MathHelper.Clamp(water, 0.0f, 1f);
      this.interpolateFireIntensity = (double) this.totalFireIntensity < (double) this.currentFireIntensity ? -0.2f : ((double) this.totalFireIntensity > (double) this.currentFireIntensity ? 0.2f : 0.0f);
      this.interpolateLavaIntensity = (double) this.totalLavaIntensity < (double) this.currentLavaIntensity ? -0.2f : ((double) this.totalLavaIntensity > (double) this.currentLavaIntensity ? 0.2f : 0.0f);
      this.interpolateWaterIntensity = (double) this.totalWaterIntensity < (double) this.currentWaterIntensity ? -0.2f : ((double) this.totalWaterIntensity > (double) this.currentWaterIntensity ? 0.2f : 0.0f);
    }

    public void Update()
    {
      if (this.instance == null || this.instance.NetworkManager == null)
        return;
      this.LoadActiveFog();
      this.LoadActiveRain();
      this.LoadActiveHail();
      foreach (Gamer localGamer in this.instance.NetworkManager.LocalGamers)
      {
        Player tag = localGamer.Tag as Player;
        if (tag != null && tag.IsEnabledField)
        {
          this.UpdateFog(tag);
          this.UpdateHail(tag);
          this.UpdateRain(tag);
        }
      }
      this.UpdateSurroundingEnvironmentIntensities();
      this.UpdateSound(this.rainSound, MathHelper.Clamp(this.totalRainIntensity, 0.0f, 1f) * CoreGlobals.AudioManager.SoundVolume);
      this.UpdateSound(this.hailSound, MathHelper.Clamp(this.totalHailIntensity, 0.0f, 1f) * CoreGlobals.AudioManager.SoundVolume);
      this.UpdateSound(this.fireSound, MathHelper.Clamp(this.currentFireIntensity, 0.0f, 1f) * CoreGlobals.AudioManager.SoundVolume);
      this.UpdateSound(this.lavaSound, MathHelper.Clamp(this.currentLavaIntensity, 0.0f, 1f) * CoreGlobals.AudioManager.SoundVolume);
      this.UpdateSound(this.waterSound, MathHelper.Clamp(this.currentWaterIntensity, 0.0f, 1f) * CoreGlobals.AudioManager.SoundVolume);
    }

    private void UpdateSurroundingEnvironmentIntensities()
    {
      this.currentFireIntensity += this.interpolateFireIntensity * Services.ElapsedTime;
      if ((double) Math.Abs(this.totalFireIntensity - this.currentFireIntensity) <= (double) Math.Abs(this.interpolateFireIntensity) * (double) Services.ElapsedTime)
      {
        this.currentFireIntensity = this.totalFireIntensity;
        this.interpolateFireIntensity = 0.0f;
      }
      this.currentLavaIntensity += this.interpolateLavaIntensity * Services.ElapsedTime;
      if ((double) Math.Abs(this.totalLavaIntensity - this.currentLavaIntensity) <= (double) Math.Abs(this.interpolateLavaIntensity) * (double) Services.ElapsedTime)
      {
        this.currentLavaIntensity = this.totalLavaIntensity;
        this.interpolateLavaIntensity = 0.0f;
      }
      this.currentWaterIntensity += this.interpolateWaterIntensity * Services.ElapsedTime;
      if ((double) Math.Abs(this.totalWaterIntensity - this.currentWaterIntensity) > (double) Math.Abs(this.interpolateWaterIntensity) * (double) Services.ElapsedTime)
        return;
      this.currentWaterIntensity = this.totalWaterIntensity;
      this.interpolateWaterIntensity = 0.0f;
    }

    private void UpdateSound(EnvManager.EnvSound sound, float intensity)
    {
      if ((double) intensity == 0.0)
      {
        if (sound.Cue == null)
          return;
        sound.Cue.Dispose();
        sound.Cue = (Cue) null;
      }
      else
      {
        if (sound.Cue == null || !sound.Cue.IsPlaying)
        {
          if (sound.Cue != null)
            sound.Cue.Dispose();
          CoreGlobals.AudioManager.PlaySound(sound.CueName, out sound.Cue);
        }
        if (sound.Cue == null)
          return;
        sound.Cue.SetVariable("Intensity", intensity);
      }
    }

    public void RemoveAllWeather()
    {
      lock (this.fogSections)
      {
        foreach (EnvManager.FogSection fogSection in this.fogSections)
          fogSection.Age = Math.Max(fogSection.Age, fogSection.Duration - 6f);
      }
      lock (this.rainSections)
      {
        foreach (EnvManager.EnvSection rainSection in this.rainSections)
          rainSection.Age = Math.Max(rainSection.Age, rainSection.Duration - 6f);
      }
      lock (this.hailSections)
      {
        foreach (EnvManager.HailSection hailSection in this.hailSections)
          hailSection.Age = Math.Max(hailSection.Age, hailSection.Duration - 6f);
      }
    }

    public void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit)
    {
      this.AddFog(center, radius, duration, intensity, 50, transmit);
    }

    public void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      int visibility,
      bool transmit)
    {
      float num = (float) (0.449999988079071 + this.map.Random.NextDouble() * 0.200000002980232 - 0.100000001490116);
      this.AddFog(center, radius, duration, 5f, intensity, new Color(num, num, num, 1f), visibility, transmit);
    }

    public void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      Color color,
      bool transmit)
    {
      this.AddFog(center, radius, duration, 5f, intensity, color, 50, transmit);
    }

    public void AddFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      int visibility,
      bool transmit)
    {
      if ((double) intensity <= 0.0 || !this.map.IsValidPoint(center) || this.fogSections.Count >= 20)
        return;
      transitDuration = Math.Max(1f, transitDuration);
      EnvManager.FogSection fogSection = new EnvManager.FogSection(this.instance, this.map, center, radius, duration, transitDuration, MathHelper.Clamp(intensity, 0.0f, 1f), (int) MathHelper.Clamp((float) visibility, 2f, 100f), color);
      lock (this.fogSections)
        this.fogSections.Add(fogSection);
      if (!transmit)
        return;
      this.instance.NetworkManager.SendFog(center, radius, duration, transitDuration, intensity, color, (ushort) visibility);
    }

    public void RemoveFog(GlobalPoint3D center, bool transmit)
    {
      lock (this.fogSections)
      {
        foreach (EnvManager.FogSection fogSection in this.fogSections)
        {
          if (fogSection.Center.X == center.X && fogSection.Center.Z == center.Z)
            fogSection.Age = Math.Max(fogSection.Age, fogSection.Duration - 6f);
        }
      }
    }

    private void LoadActiveFog()
    {
      lock (this.fogSections)
      {
        this.tempFogSections.Clear();
        for (int index = this.fogSections.Count - 1; index >= 0; --index)
        {
          EnvManager.FogSection fogSection = this.fogSections[index];
          fogSection.Age += Services.ElapsedTime;
          if ((double) fogSection.Age <= (double) fogSection.Duration)
            this.tempFogSections.Add(fogSection);
          else
            this.fogSections.RemoveAt(index);
        }
      }
    }

    private void UpdateFog(Player player)
    {
      float num = 0.0f;
      Player virtualPlayer = player.VirtualPlayer;
      virtualPlayer.FogColor = Vector3.Zero;
      virtualPlayer.FogVisibility = (int) player.FarClip;
      foreach (EnvManager.FogSection tempFogSection in this.tempFogSections)
      {
        float relativeIntensity = tempFogSection.GetRelativeIntensity(this.map, virtualPlayer);
        num += relativeIntensity;
        this.fogSumList.Add(new EnvManager.FogSum()
        {
          Color = tempFogSection.Color.ToVector3(),
          Intensity = relativeIntensity
        });
        if (tempFogSection.Visibility < virtualPlayer.FogVisibility)
          virtualPlayer.FogVisibility = tempFogSection.Visibility;
      }
      foreach (EnvManager.FogSum fogSum in this.fogSumList)
        virtualPlayer.FogColor += fogSum.Color * fogSum.Intensity / num;
      virtualPlayer.FogIntensity = MathHelper.Clamp(num, 0.0f, 1f);
      this.fogSumList.Clear();
    }

    public void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit)
    {
      this.AddHail(center, radius, duration, intensity, Color.LightGray * 0.8f, transmit);
    }

    public void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      Color color,
      bool transmit)
    {
      this.AddHail(center, radius, duration, 5f, intensity, color, 0.04f, 0.08f, transmit);
    }

    public void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      float minSize,
      float maxSize,
      bool transmit)
    {
      this.AddHail(center, radius, duration, 5f, intensity, Color.White, minSize, maxSize, transmit);
    }

    public void AddHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      float minSize,
      float maxSize,
      bool transmit)
    {
      if (!this.map.IsValidPoint(center) || this.hailSections.Count >= 15)
        return;
      EnvManager.HailSection hailSection = new EnvManager.HailSection(this.instance, this.map, center, radius, duration, transitDuration, MathHelper.Clamp(intensity, 0.1f, 1f), minSize, maxSize, color);
      lock (this.hailSections)
        this.hailSections.Add(hailSection);
      if (!transmit)
        return;
      this.instance.NetworkManager.SendHail(center, radius, duration, transitDuration, intensity, color, minSize, maxSize);
    }

    public void RemoveHail(GlobalPoint3D center, bool transmit)
    {
      lock (this.hailSections)
      {
        foreach (EnvManager.HailSection hailSection in this.hailSections)
        {
          if (hailSection.Center.X == center.X && hailSection.Center.Z == center.Z)
            hailSection.Age = Math.Max(hailSection.Age, hailSection.Duration - 6f);
        }
      }
    }

    private void LoadActiveHail()
    {
      lock (this.hailSections)
      {
        this.tempHailSections.Clear();
        for (int index = this.hailSections.Count - 1; index >= 0; --index)
        {
          EnvManager.HailSection hailSection = this.hailSections[index];
          hailSection.Age += Services.ElapsedTime;
          if ((double) hailSection.Age <= (double) hailSection.Duration)
            this.tempHailSections.Add(hailSection);
          else
            this.hailSections.RemoveAt(index);
        }
      }
    }

    private void UpdateHail(Player player)
    {
      Player virtualPlayer = player.VirtualPlayer;
      float tileSize = this.map.TileSize;
      float num1 = this.instance.CloudHeight + 8f;
      bool flag = false;
      this.totalHailIntensity = 0.0f;
      this.tempHailIntensities.Clear();
      HailParticleSystem hailParticleSystem = player.HailParticleSystem;
      foreach (EnvManager.EnvSection tempHailSection in this.tempHailSections)
      {
        float relativeIntensity = tempHailSection.GetRelativeIntensity(this.map, virtualPlayer);
        this.tempHailIntensities.Add(relativeIntensity);
        this.totalHailIntensity += relativeIntensity;
      }
      float num2 = (double) this.totalHailIntensity > 1.0 ? 1f / this.totalHailIntensity : 1f;
      double num3 = (double) MathHelper.Clamp(this.totalHailIntensity, 0.0f, 1f);
      Vector3 position = new Vector3();
      float num4 = hailParticleSystem.ParticleDuration * 0.5f;
      Vector3 vector3 = virtualPlayer.Position + virtualPlayer.Velocity * num4;
      float y1 = virtualPlayer.Box.Max.Y;
      float y2 = virtualPlayer.Box.Min.Y;
      float num5 = virtualPlayer.Size.X * 0.5f;
      float num6 = hailParticleSystem.MaxDistance * 2f / hailParticleSystem.ParticleDuration;
      for (int index1 = 0; index1 < this.tempHailSections.Count; ++index1)
      {
        EnvManager.HailSection tempHailSection = this.tempHailSections[index1];
        float tempHailIntensity = this.tempHailIntensities[index1];
        if ((double) tempHailIntensity > 0.0)
        {
          int num7 = (int) Math.Max(1f, num2 * ((float) hailParticleSystem.MaxParticles / 60f / hailParticleSystem.ParticleDuration * tempHailIntensity));
          for (int index2 = 0; index2 < num7; ++index2)
          {
            float num8 = vector3.X + ((float) (this.map.Random.NextDouble() * ((double) hailParticleSystem.MaxDistance * 2.0)) - hailParticleSystem.MaxDistance);
            float num9 = vector3.Z + ((float) (this.map.Random.NextDouble() * ((double) hailParticleSystem.MaxDistance * 2.0)) - hailParticleSystem.MaxDistance);
            float velocity = (float) (-(double) num6 - this.map.Random.NextDouble() * 2.0);
            float num10 = vector3.Y - velocity * num4;
            if ((double) num10 > (double) num1)
              num10 = num1;
            position.X = num8;
            position.Y = num10;
            position.Z = num9;
            GlobalPoint3D point = this.map.GetPoint(position);
            if (this.map.IsValidPoint(point))
            {
              point.Y = (int) this.map.GetHeight(point);
              float num11 = (float) point.Y * tileSize;
              if ((double) num11 < (double) position.Y)
              {
                float size = (float) this.map.Random.NextDouble() * (tempHailSection.MaxSize - tempHailSection.MinSize) + tempHailSection.MinSize;
                if (player == virtualPlayer)
                {
                  if ((double) num11 < (double) y1 && (double) position.Y > (double) y2 && ((double) position.X > (double) vector3.X - (double) num5 && (double) position.X < (double) vector3.X + (double) num5) && ((double) position.Z > (double) vector3.Z - (double) num5 && (double) position.Z < (double) vector3.Z + (double) num5))
                  {
                    double damageAndDisplay = (double) player.TakeDamageAndDisplay(DamageType.Hail, size * 20f, Vector3.Zero);
                  }
                  else
                  {
                    switch ((Block) this.map.GetBlockID(point))
                    {
                      case Block.Glass:
                      case Block.StainedGlass:
                        if (this.SmashGlass(point, size))
                        {
                          flag = true;
                          break;
                        }
                        break;
                      case Block.Fire:
                        if (this.PutOutFire(point))
                        {
                          flag = true;
                          break;
                        }
                        break;
                    }
                  }
                }
                ++point.Y;
                float endY = num11 + tileSize;
                if (!hailParticleSystem.AddParticle(position, velocity, endY, size, tempHailSection.Color))
                  return;
              }
            }
          }
        }
      }
      if (!flag)
        return;
      this.map.Commit();
    }

    public void AddRain(
      GlobalPoint3D center,
      float radius,
      float duration,
      float intensity,
      bool transmit)
    {
      float r = (float) (0.600000023841858 + this.map.Random.NextDouble() * 0.0199999995529652);
      float g = (float) (0.600000023841858 + this.map.Random.NextDouble() * 0.0199999995529652);
      float b = (float) (0.699999988079071 + this.map.Random.NextDouble() * 0.200000002980232);
      this.AddRain(center, radius, duration, 5f, intensity, new Color(r, g, b, 1f), transmit);
    }

    public void AddRain(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      bool transmit)
    {
      if (!this.map.IsValidPoint(center) || this.rainSections.Count >= 20)
        return;
      EnvManager.EnvSection envSection = new EnvManager.EnvSection(this.instance, this.map, center, radius, duration, transitDuration, MathHelper.Clamp(intensity, 0.1f, 1f), color);
      lock (this.rainSections)
        this.rainSections.Add(envSection);
      if (!transmit)
        return;
      this.instance.NetworkManager.SendRain(center, radius, duration, transitDuration, intensity, color);
    }

    public void RemoveRain(GlobalPoint3D center, bool transmit)
    {
      lock (this.rainSections)
      {
        foreach (EnvManager.EnvSection rainSection in this.rainSections)
        {
          if (rainSection.Center.X == center.X && rainSection.Center.Z == center.Z)
            rainSection.Age = Math.Max(rainSection.Age, rainSection.Duration - 6f);
        }
      }
    }

    private void LoadActiveRain()
    {
      lock (this.rainSections)
      {
        this.tempRainSections.Clear();
        for (int index = this.rainSections.Count - 1; index >= 0; --index)
        {
          EnvManager.EnvSection rainSection = this.rainSections[index];
          rainSection.Age += Services.ElapsedTime;
          if ((double) rainSection.Age <= (double) rainSection.Duration)
            this.tempRainSections.Add(rainSection);
          else
            this.rainSections.RemoveAt(index);
        }
      }
    }

    private void UpdateRain(Player player)
    {
      Player virtualPlayer = player.VirtualPlayer;
      float tileSize = this.map.TileSize;
      float num1 = this.instance.CloudHeight + 8f;
      bool flag = false;
      this.totalRainIntensity = 0.0f;
      this.tempRainIntensities.Clear();
      RainParticleSystem rainParticleSystem = player.RainParticleSystem;
      foreach (EnvManager.EnvSection tempRainSection in this.tempRainSections)
      {
        float relativeIntensity = tempRainSection.GetRelativeIntensity(this.map, virtualPlayer);
        this.tempRainIntensities.Add(relativeIntensity);
        this.totalRainIntensity += relativeIntensity;
      }
      float num2 = (double) this.totalRainIntensity > 1.0 ? 1f / this.totalRainIntensity : 1f;
      double num3 = (double) MathHelper.Clamp(this.totalRainIntensity, 0.0f, 1f);
      Vector3 position = new Vector3();
      float num4 = rainParticleSystem.ParticleDuration * 0.5f;
      Vector3 vector3 = virtualPlayer.Position + virtualPlayer.Velocity * num4;
      float num5 = rainParticleSystem.MaxDistance * 2f / rainParticleSystem.ParticleDuration;
      for (int index1 = 0; index1 < this.tempRainSections.Count; ++index1)
      {
        EnvManager.EnvSection tempRainSection = this.tempRainSections[index1];
        float tempRainIntensity = this.tempRainIntensities[index1];
        if ((double) tempRainIntensity > 0.0)
        {
          int num6 = (int) Math.Max(1f, num2 * ((float) rainParticleSystem.MaxParticles / 60f / rainParticleSystem.ParticleDuration * tempRainIntensity));
          for (int index2 = 0; index2 < num6; ++index2)
          {
            float num7 = vector3.X + ((float) (this.map.Random.NextDouble() * ((double) rainParticleSystem.MaxDistance * 2.0)) - rainParticleSystem.MaxDistance);
            float num8 = vector3.Z + ((float) (this.map.Random.NextDouble() * ((double) rainParticleSystem.MaxDistance * 2.0)) - rainParticleSystem.MaxDistance);
            float velocity = (float) (-(double) num5 - this.map.Random.NextDouble() * 2.0);
            float num9 = vector3.Y - velocity * num4;
            if ((double) num9 > (double) num1)
              num9 = num1;
            position.X = num7;
            position.Y = num9;
            position.Z = num8;
            GlobalPoint3D point = this.map.GetPoint(position);
            if (this.map.IsValidPoint(point))
            {
              point.Y = (int) this.map.GetHeight(point);
              if ((double) point.Y * (double) tileSize < (double) position.Y)
              {
                if (player != virtualPlayer && this.map.GetBlockID(point) == (byte) 118 && this.PutOutFire(point))
                  flag = true;
                ++point.Y;
                float endY = (float) point.Y * tileSize;
                if (!rainParticleSystem.AddParticle(position, velocity, endY, tempRainSection.Color))
                  return;
              }
            }
          }
        }
      }
      if (!flag)
        return;
      this.map.Commit();
    }

    private bool PutOutFire(GlobalPoint3D p)
    {
      if (this.instance.MapStrategyTM.GetDataBlock(p) is FireBlock)
        return this.map.SetBlockData(p, (byte) 0, (byte) 0, UpdateBlockMethod.Strategy, GamerID.Sys1, true) != null;
      return false;
    }

    private bool SmashGlass(GlobalPoint3D p, float size)
    {
      if ((double) size <= 0.100000001490116 || !this.instance.IsFiniteResources || (!this.map.Random.RandomChance(0.2) || !this.instance.ClearBlock(p, UpdateBlockMethod.Strategy, GamerID.Sys1, true)))
        return false;
      Sounds.PlaySound(Item.Glass, ItemSoundType.Mine, p, (ITMActor) null);
      return true;
    }

    private class EnvSection
    {
      public GameInstance Instance;
      public MapTM Map;
      public GlobalPoint3D Center;
      public Color Color;
      public float Radius;
      public float Duration;
      public float TransitDuration;
      public float Intensity;
      public float Age;

      public EnvSection(
        GameInstance instance,
        MapTM map,
        GlobalPoint3D center,
        float radius,
        float duration,
        float transitDuration,
        float intensity,
        Color color)
      {
        this.Instance = instance;
        this.Map = map;
        this.Center = center;
        this.Radius = radius;
        this.TransitDuration = Math.Max(1f, transitDuration);
        this.Duration = Math.Max(duration, this.TransitDuration * 2f);
        this.Intensity = intensity;
        this.Color = color;
      }

      public virtual float GetRelativeIntensity(MapTM map, Player player)
      {
        Vector2 vector2_1 = new Vector2(player.Position.X, player.Position.Z);
        Vector3 position = map.GetPosition(this.Center);
        Vector2 vector2_2 = new Vector2(position.X, position.Z);
        float num1 = Vector2.Distance(vector2_1, vector2_2);
        if ((double) num1 > (double) this.Radius)
          return 0.0f;
        float num2 = this.Radius * 0.75f;
        float num3 = this.Intensity * ((double) num1 <= (double) num2 ? 1f : (float) (((double) this.Radius - (double) num1) / ((double) this.Radius * 0.25)));
        float num4 = 1f;
        if ((double) this.Age < (double) this.TransitDuration)
          num4 = this.Age / this.TransitDuration;
        else if ((double) this.Age > (double) this.Duration - (double) this.TransitDuration)
          num4 = (this.Duration - this.Age) / this.TransitDuration;
        return num3 * num4;
      }
    }

    private class FogSection : EnvManager.EnvSection
    {
      public int Visibility;

      public FogSection(
        GameInstance instance,
        MapTM map,
        GlobalPoint3D center,
        float radius,
        float duration,
        float transitDuration,
        float intensity,
        int visibility,
        Color color)
        : base(instance, map, center, radius, duration, transitDuration, intensity, color)
      {
        this.Visibility = visibility;
      }
    }

    private class HailSection : EnvManager.EnvSection
    {
      public float MinSize;
      public float MaxSize;

      public HailSection(
        GameInstance instance,
        MapTM map,
        GlobalPoint3D center,
        float radius,
        float duration,
        float transitDuration,
        float intensity,
        float minSize,
        float maxSize,
        Color color)
        : base(instance, map, center, radius, duration, transitDuration, intensity, color)
      {
        this.MinSize = minSize;
        this.MaxSize = maxSize;
      }
    }

    private class EnvSound
    {
      public string CueName;
      public Cue Cue;
    }

    private struct FogSum
    {
      public Vector3 Color;
      public float Intensity;
    }
  }
}
