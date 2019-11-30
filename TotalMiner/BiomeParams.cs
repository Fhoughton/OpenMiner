// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.BiomeParams
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using StudioForge.Engine.Core;
using System;
using System.IO;

namespace StudioForge.TotalMiner
{
  public class BiomeParams
  {
    public int DirtHeight = 30;
    public int BasaltHeight = 36;
    public int SnowLayerHeight = 45;
    public int SnowHeight = 50;
    public int MaxHeight;
    public int MaxSeaDepth;
    public int WaterSaturation;
    public float TreeFrequency;
    public int TreeDensityMin;
    public int TreeDensityMax;
    public int OreDensity;
    public float BigDetailNoise;
    public float MediumDetailNoise;
    public float FineDetailNoise;
    public float BigDetailMultiplier;
    public float MediumDetailMultiplier;
    public float FineDetailMultiplier;
    public float TotalNoiseDivisor;
    public bool GenerateCaves;
    public int OffsetX;
    public int OffsetZ;

    public BiomeParams Clone()
    {
      return new BiomeParams()
      {
        MaxHeight = this.MaxHeight,
        MaxSeaDepth = this.MaxSeaDepth,
        WaterSaturation = this.WaterSaturation,
        DirtHeight = this.DirtHeight,
        BasaltHeight = this.BasaltHeight,
        SnowLayerHeight = this.SnowLayerHeight,
        SnowHeight = this.SnowHeight,
        TreeFrequency = this.TreeFrequency,
        TreeDensityMin = this.TreeDensityMin,
        TreeDensityMax = this.TreeDensityMax,
        OreDensity = this.OreDensity,
        BigDetailNoise = this.BigDetailNoise,
        MediumDetailNoise = this.MediumDetailNoise,
        FineDetailNoise = this.FineDetailNoise,
        BigDetailMultiplier = this.BigDetailMultiplier,
        MediumDetailMultiplier = this.MediumDetailMultiplier,
        FineDetailMultiplier = this.FineDetailMultiplier,
        TotalNoiseDivisor = this.TotalNoiseDivisor,
        GenerateCaves = this.GenerateCaves,
        OffsetX = this.OffsetX,
        OffsetZ = this.OffsetZ
      };
    }

    public void Initialize(BiomeType biome, int version)
    {
      this.Initialize(biome, version, new int?());
    }

    public void Initialize(BiomeType biome, int version, int? seed)
    {
      switch (biome)
      {
        case BiomeType.Desert:
          this.InitForDesert(version, seed);
          break;
        case BiomeType.Grasslands:
          this.InitForGrasslands(version, seed);
          break;
        case BiomeType.SemiAlphine:
          this.InitForSemiAlpine(version, seed);
          break;
        case BiomeType.DigDeep:
          this.InitForDigDeep(version);
          break;
        default:
          this.InitForFlat(version);
          break;
      }
    }

    private void InitForFlat(int version)
    {
      this.MaxHeight = 0;
      this.MaxSeaDepth = 0;
      this.WaterSaturation = 0;
      this.DirtHeight = 0;
      this.BasaltHeight = 0;
      this.SnowLayerHeight = 0;
      this.SnowHeight = 0;
      this.BigDetailNoise = 0.0f;
      this.MediumDetailNoise = 0.0f;
      this.FineDetailNoise = 0.0f;
      this.BigDetailMultiplier = 0.0f;
      this.MediumDetailMultiplier = 0.0f;
      this.FineDetailMultiplier = 0.0f;
      this.TotalNoiseDivisor = 0.0f;
      this.TreeFrequency = 0.0f;
      this.TreeDensityMin = 0;
      this.TreeDensityMax = 0;
      this.OreDensity = 100;
      this.OffsetX = version <= 261 ? 0 : 1000000;
      this.OffsetZ = version <= 261 ? 0 : 1000000;
      this.GenerateCaves = false;
    }

    private void InitForDigDeep(int version)
    {
      this.BigDetailNoise = 400f;
      this.MediumDetailNoise = 45f;
      this.FineDetailNoise = 5f;
      this.BigDetailMultiplier = 10f;
      this.MediumDetailMultiplier = 1f;
      this.FineDetailMultiplier = 0.1f;
      this.TotalNoiseDivisor = 12.1f;
      this.MaxHeight = 94;
      this.MaxSeaDepth = 15;
      this.WaterSaturation = 55;
      this.DirtHeight = 30;
      this.BasaltHeight = 36;
      this.SnowLayerHeight = 45;
      this.SnowHeight = 50;
      this.TreeFrequency = 50f;
      this.TreeDensityMin = 10;
      this.TreeDensityMax = 70;
      this.OreDensity = 100;
      this.OffsetX = version <= 261 ? 0 : 1000000;
      this.OffsetZ = version <= 261 ? 0 : 1000000;
      this.GenerateCaves = true;
    }

    private void InitForDesert(int version, int? seed)
    {
      this.BigDetailNoise = (float) byte.MaxValue;
      this.MediumDetailNoise = 83f;
      this.FineDetailNoise = 35f;
      this.BigDetailMultiplier = 10f;
      this.MediumDetailMultiplier = 1f;
      this.FineDetailMultiplier = 0.1f;
      this.TotalNoiseDivisor = 11.1f;
      this.MaxSeaDepth = 20;
      this.MaxHeight = 40;
      this.WaterSaturation = 20;
      this.TreeFrequency = 12.5f;
      this.TreeDensityMin = 1;
      this.TreeDensityMax = 4;
      this.OreDensity = 100;
      this.OffsetX = version <= 261 ? 0 : 1000000;
      this.OffsetZ = version <= 261 ? 0 : 1000000;
      this.GenerateCaves = true;
      if (!seed.HasValue)
        return;
      PcgRandom r = new PcgRandom(seed.Value);
      bool flag = r.Next(4) == 0;
      int max1 = flag ? 15 : 4;
      this.MaxSeaDepth += r.Next(max1);
      int max2 = flag ? 20 : 6;
      this.WaterSaturation += r.Next(max2);
      int max3 = flag ? 40 : 10;
      this.MaxHeight += r.Next(max3);
      this.BigDetailNoise = this.Tweak(r, this.BigDetailNoise, flag ? 0.5f : 0.1f, 0.2f);
      this.MediumDetailNoise = this.Tweak(r, this.MediumDetailNoise, flag ? 0.5f : 0.1f, 1f);
      this.FineDetailNoise = this.Tweak(r, this.FineDetailNoise, flag ? 0.6f : 0.2f, 1f);
      this.BigDetailMultiplier = this.Tweak(r, this.BigDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.MediumDetailMultiplier = this.Tweak(r, this.MediumDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.FineDetailMultiplier = this.Tweak(r, this.FineDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
    }

    private void InitForGrasslands(int version, int? seed)
    {
      this.BigDetailNoise = 315f;
      this.MediumDetailNoise = 32f;
      this.FineDetailNoise = 13f;
      this.BigDetailMultiplier = 10f;
      this.MediumDetailMultiplier = 1f;
      this.FineDetailMultiplier = 0.1f;
      this.TotalNoiseDivisor = 11.1f;
      this.MaxSeaDepth = 25;
      this.MaxHeight = 40;
      this.WaterSaturation = 25;
      this.TreeFrequency = 10f;
      this.TreeDensityMin = 1;
      this.TreeDensityMax = 4;
      this.OreDensity = 100;
      this.OffsetX = version <= 261 ? 0 : 1000000;
      this.OffsetZ = version <= 261 ? 0 : 1000000;
      this.GenerateCaves = true;
      if (!seed.HasValue)
        return;
      PcgRandom r = new PcgRandom(seed.Value);
      bool flag = r.Next(4) == 0;
      int max1 = flag ? 15 : 4;
      this.MaxSeaDepth += r.Next(max1);
      int max2 = flag ? 20 : 6;
      this.WaterSaturation += r.Next(max2);
      int max3 = flag ? 40 : 10;
      this.MaxHeight += r.Next(max3);
      this.BigDetailNoise = this.Tweak(r, this.BigDetailNoise, flag ? 0.5f : 0.1f, 0.5f);
      this.MediumDetailNoise = this.Tweak(r, this.MediumDetailNoise, flag ? 0.4f : 0.1f, 0.25f);
      this.FineDetailNoise = this.Tweak(r, this.FineDetailNoise, flag ? 0.3f : 0.1f, 0.8f);
      this.BigDetailMultiplier = this.Tweak(r, this.BigDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.MediumDetailMultiplier = this.Tweak(r, this.MediumDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.FineDetailMultiplier = this.Tweak(r, this.FineDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
    }

    private void InitForSemiAlpine(int version, int? seed)
    {
      this.BigDetailNoise = 710f;
      this.MediumDetailNoise = 45f;
      this.FineDetailNoise = 8f;
      this.BigDetailMultiplier = 10f;
      this.MediumDetailMultiplier = 1f;
      this.FineDetailMultiplier = 0.1f;
      this.TotalNoiseDivisor = 12.1f;
      this.MaxHeight = 104;
      this.MaxSeaDepth = 20;
      this.WaterSaturation = 65;
      this.DirtHeight = 30;
      this.BasaltHeight = 36;
      this.SnowLayerHeight = 45;
      this.SnowHeight = 50;
      this.TreeFrequency = 40f;
      this.TreeDensityMin = 5;
      this.TreeDensityMax = 20;
      this.OreDensity = 100;
      this.OffsetX = version <= 261 ? 0 : 1000000;
      this.OffsetZ = version <= 261 ? 0 : 1000000;
      this.GenerateCaves = true;
      if (!seed.HasValue)
        return;
      PcgRandom r = new PcgRandom(seed.Value);
      bool flag = r.Next(4) == 0;
      int max1 = flag ? 15 : 4;
      this.MaxSeaDepth += r.Next(max1);
      int max2 = flag ? 20 : 6;
      this.WaterSaturation += r.Next(max2);
      int max3 = flag ? 40 : 10;
      this.MaxHeight += (int) ((double) r.Next(max3) - (double) max3 * 0.25);
      this.BigDetailNoise = this.Tweak(r, this.BigDetailNoise, flag ? 0.5f : 0.1f, 0.5f);
      this.MediumDetailNoise = this.Tweak(r, this.MediumDetailNoise, flag ? 0.5f : 0.1f, 0.5f);
      this.FineDetailNoise = this.Tweak(r, this.FineDetailNoise, flag ? 0.5f : 0.2f, 0.2f);
      this.BigDetailMultiplier = this.Tweak(r, this.BigDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.MediumDetailMultiplier = this.Tweak(r, this.MediumDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
      this.FineDetailMultiplier = this.Tweak(r, this.FineDetailMultiplier, flag ? 0.1f : 0.02f, 0.5f);
    }

    private float Tweak(PcgRandom r, float p, float v, float s)
    {
      float num = p * v;
      return (float) Math.Round((double) p + r.NextDouble() * (double) num - (double) num * (double) s, 2);
    }

    public void ReadState(BinaryReader reader, int version)
    {
      this.MaxHeight = reader.ReadInt32();
      this.MaxSeaDepth = reader.ReadInt32();
      this.WaterSaturation = reader.ReadInt32();
      this.DirtHeight = reader.ReadInt32();
      this.BasaltHeight = reader.ReadInt32();
      this.SnowLayerHeight = reader.ReadInt32();
      this.SnowHeight = reader.ReadInt32();
      this.BigDetailNoise = reader.ReadSingle();
      this.MediumDetailNoise = reader.ReadSingle();
      this.FineDetailNoise = reader.ReadSingle();
      this.BigDetailMultiplier = reader.ReadSingle();
      this.MediumDetailMultiplier = reader.ReadSingle();
      this.FineDetailMultiplier = reader.ReadSingle();
      this.TotalNoiseDivisor = reader.ReadSingle();
      this.OffsetX = this.OffsetZ = 0;
      if (version <= 197)
        return;
      this.TreeFrequency = reader.ReadSingle();
      this.TreeDensityMin = (int) reader.ReadInt16();
      this.TreeDensityMax = (int) reader.ReadInt16();
      if (this.TreeDensityMax < this.TreeDensityMin)
        this.TreeDensityMax = this.TreeDensityMin;
      if (version > 292)
        this.OreDensity = (int) reader.ReadInt16();
      if (version <= 212)
        return;
      this.GenerateCaves = reader.ReadBoolean();
      if (version <= 261)
        return;
      this.OffsetX = reader.ReadInt32();
      this.OffsetZ = reader.ReadInt32();
    }

    public void WriteState(BinaryWriter writer)
    {
      writer.Write(this.MaxHeight);
      writer.Write(this.MaxSeaDepth);
      writer.Write(this.WaterSaturation);
      writer.Write(this.DirtHeight);
      writer.Write(this.BasaltHeight);
      writer.Write(this.SnowLayerHeight);
      writer.Write(this.SnowHeight);
      writer.Write(this.BigDetailNoise);
      writer.Write(this.MediumDetailNoise);
      writer.Write(this.FineDetailNoise);
      writer.Write(this.BigDetailMultiplier);
      writer.Write(this.MediumDetailMultiplier);
      writer.Write(this.FineDetailMultiplier);
      writer.Write(this.TotalNoiseDivisor);
      writer.Write(this.TreeFrequency);
      writer.Write((short) this.TreeDensityMin);
      writer.Write((short) this.TreeDensityMax);
      writer.Write((short) this.OreDensity);
      writer.Write(this.GenerateCaves);
      writer.Write(this.OffsetX);
      writer.Write(this.OffsetZ);
    }
  }
}
