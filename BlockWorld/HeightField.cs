// Decompiled with JetBrains decompiler
// Type: StudioForge.BlockWorld.HeightField
// Assembly: StudioForge.BlockWorld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 25A385FE-38C2-4B34-AF3F-1EF2EFA4B0A9
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.BlockWorld.dll

using Microsoft.Xna.Framework;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StudioForge.BlockWorld
{
  public class HeightField
  {
    private PcgRandom random = new PcgRandom(new Random().Next());
    private int sizeX;
    private int sizeZ;
    private float maxHeight;
    private float lastNormalizationMinVal;
    private float lastNormalizationScale;
    private float[] field;

    public HeightField(int sizeX, int sizeZ)
      : this(sizeX, sizeZ, float.MaxValue)
    {
    }

    public HeightField(int sizeX, int sizeZ, float maxHeight)
    {
      this.sizeX = sizeX;
      this.sizeZ = sizeZ;
      this.maxHeight = maxHeight;
      this.field = new float[sizeX * sizeZ];
    }

    public HeightField(int sizeX, int sizeZ, float[] field)
    {
      this.sizeX = sizeX;
      this.sizeZ = sizeZ;
      this.field = field;
    }

    public float[] Field
    {
      get
      {
        return this.field;
      }
    }

    public int SizeX
    {
      get
      {
        return this.sizeX;
      }
    }

    public int SizeZ
    {
      get
      {
        return this.sizeZ;
      }
    }

    public Vector2 MinMaxRange
    {
      get
      {
        float x = float.MaxValue;
        float y = float.MinValue;
        for (int index1 = 0; index1 < this.sizeZ; ++index1)
        {
          for (int index2 = 0; index2 < this.sizeX; ++index2)
          {
            float num = this.field[index2 + index1 * this.sizeX];
            if ((double) num < (double) x)
              x = num;
            if ((double) num > (double) y)
              y = num;
          }
        }
        return new Vector2(x, y);
      }
    }

    public int MostCommonHeightInt(int height)
    {
      int num1 = 0;
      int num2 = 0;
      int[] numArray = new int[height + 1];
      for (int index1 = 0; index1 < this.sizeZ; ++index1)
      {
        for (int index2 = 0; index2 < this.sizeX; ++index2)
        {
          int index3 = (int) ((double) this.field[index2 + index1 * this.sizeX] * (double) height);
          ++numArray[index3];
          if (numArray[index3] > num2)
          {
            num2 = numArray[index3];
            num1 = index3;
          }
        }
      }
      return num1;
    }

    public float GetHeight(int x, int z)
    {
      int index = x + z * this.sizeX;
      if (index < 0 || index >= this.field.Length)
        return 0.0f;
      return this.field[index];
    }

    public void SetHeight(int x, int z, float value)
    {
      int index = x + z * this.sizeX;
      if (index < 0 || index >= this.field.Length)
        return;
      this.field[index] = (double) value <= (double) this.maxHeight ? value : this.maxHeight;
    }

    public void ClearField()
    {
      for (int index = 0; index < this.field.Length; ++index)
        this.field[index] = 0.0f;
      this.lastNormalizationMinVal = 0.0f;
      this.lastNormalizationScale = 1f;
    }

    public short[] BuildHeightMapShort(float maxHeight, float upperBound)
    {
      short[] numArray = new short[this.field.Length];
      for (int index = 0; index < this.field.Length; ++index)
      {
        float num = this.field[index] * maxHeight;
        numArray[index] = (short) ((double) upperBound + ((double) maxHeight - (double) num));
      }
      return numArray;
    }

    public void FractalFaultFormation(
      int iterations,
      int maxDelta,
      int minDelta,
      int iterationsPerErosionFilter,
      float erosion,
      int seed,
      IProgressBar progressBar,
      bool denormalizeFirst)
    {
      this.random = new PcgRandom(seed);
      this.FractalFaultFormationCore(iterations, maxDelta, minDelta, iterationsPerErosionFilter, erosion, progressBar, denormalizeFirst);
    }

    public void FractalFaultFormationThreaded(
      int iterations,
      int maxDelta,
      int minDelta,
      int iterationsPerErosionFilter,
      float erosion,
      int seed,
      IProgressBar progressBar,
      bool denormalizeFirst)
    {
      this.random = new PcgRandom(seed);
      List<HeightField.ThreadData> threadDataList = new List<HeightField.ThreadData>();
      int num1 = 6;
      int val1 = iterations / 5;
      int num2 = 0;
      for (int index = 0; index < num1; ++index)
      {
        int num3 = Math.Min(val1, iterations - num2);
        if (num3 > 0)
        {
          HeightField.ThreadData threadData = new HeightField.ThreadData()
          {
            Iterations = num3,
            DenormalizeFirst = false,
            Erosion = erosion,
            IterationsPerErosionFilter = iterationsPerErosionFilter,
            MaxDelta = maxDelta,
            MinDelta = minDelta,
            ProgressBar = progressBar,
            Complete = false
          };
          threadDataList.Add(threadData);
          new Thread(new ParameterizedThreadStart(this.FractalFaultFormationThreadedCore)).Start((object) threadData);
        }
      }
      int num4 = 0;
      while (num4 < threadDataList.Count)
      {
        Thread.Sleep(500);
        num4 = 0;
        for (int index = 0; index < threadDataList.Count; ++index)
        {
          if (threadDataList[index].Complete)
            ++num4;
        }
      }
    }

    private void FractalFaultFormationThreadedCore(object data)
    {
      HeightField.ThreadData threadData = data as HeightField.ThreadData;
      this.FractalFaultFormationCore(threadData.Iterations, threadData.MaxDelta, threadData.MinDelta, threadData.IterationsPerErosionFilter, threadData.Erosion, threadData.ProgressBar, threadData.DenormalizeFirst);
      threadData.Complete = true;
    }

    private void FractalFaultFormationCore(
      int iterations,
      int maxDelta,
      int minDelta,
      int iterationsPerFilter,
      float filter,
      IProgressBar progressBar,
      bool denormalizeFirst)
    {
      if (denormalizeFirst)
        this.DenormalizeField();
      float increment = 1f / (float) iterations;
      for (int index1 = 0; index1 < iterations; ++index1)
      {
        progressBar.AddProgress(increment);
        int num1 = maxDelta - (maxDelta - minDelta) * index1 / iterations;
        int num2 = this.random.Next(0, this.sizeX);
        int num3 = this.random.Next(0, this.sizeZ);
        int num4;
        int num5;
        do
        {
          num4 = this.random.Next(0, this.sizeX);
          num5 = this.random.Next(0, this.sizeZ);
        }
        while (num4 == num2 && num5 == num3);
        int num6 = num4 - num2;
        int num7 = num5 - num3;
        for (int index2 = 0; index2 < this.sizeX; ++index2)
        {
          for (int index3 = 0; index3 < this.sizeZ; ++index3)
          {
            int num8 = index2 - num2;
            int num9 = index3 - num3;
            if (num8 * num7 - num6 * num9 > 0)
              this.field[index2 + this.sizeX * index3] += (float) num1;
          }
        }
        if ((double) filter > 0.0 && iterationsPerFilter != 0 && index1 % iterationsPerFilter == 0)
          this.FilterHeightField(filter);
      }
      this.NormalizeField();
    }

    public void ParticleDeposition(
      int jumps,
      int peakWalk,
      int minParticlesPerJump,
      int maxParticlesPerJump,
      float caldera,
      float particleHeight,
      int seed,
      IProgressBar progressBar,
      bool denormalizeFirst)
    {
      this.random = new PcgRandom(seed);
      this.ParticleDepositionCore(jumps, peakWalk, minParticlesPerJump, maxParticlesPerJump, caldera, particleHeight, progressBar, denormalizeFirst);
    }

    private void ParticleDepositionCore(
      int jumps,
      int peakWalk,
      int minParticlesPerJump,
      int maxParticlesPerJump,
      float caldera,
      float particleHeight,
      IProgressBar progressBar,
      bool denormalizeFirst)
    {
      if (denormalizeFirst)
        this.DenormalizeField();
      int[] numArray1 = new int[8]
      {
        0,
        1,
        0,
        this.sizeX - 1,
        1,
        1,
        this.sizeX - 1,
        this.sizeX - 1
      };
      int[] numArray2 = new int[8]
      {
        1,
        0,
        this.sizeZ - 1,
        0,
        this.sizeZ - 1,
        1,
        this.sizeZ - 1,
        1
      };
      int[] numArray3 = new int[this.sizeX * this.sizeZ];
      float increment = 1f / (float) jumps;
      if (minParticlesPerJump > maxParticlesPerJump)
      {
        int num = minParticlesPerJump;
        minParticlesPerJump = maxParticlesPerJump;
        maxParticlesPerJump = num;
      }
      for (int index1 = 0; index1 < jumps; ++index1)
      {
        progressBar.AddProgress(increment);
        int num1 = this.random.Next(0, this.sizeX);
        int num2 = this.random.Next(0, this.sizeZ);
        int num3 = num1;
        int num4 = num2;
        int num5 = this.random.Next(minParticlesPerJump, maxParticlesPerJump);
        for (int index2 = 0; index2 < num5; ++index2)
        {
          if (peakWalk != 0 && index2 % peakWalk == 0)
          {
            int index3 = this.random.Next(0, 8);
            num1 = (num1 + numArray1[index3] + this.sizeX) % this.sizeX;
            num2 = (num2 + numArray2[index3] + this.sizeZ) % this.sizeZ;
          }
          this.field[num1 + num2 * this.sizeX] += particleHeight;
          int num6 = num1;
          int num7 = num2;
          int num8 = 0;
          int num9 = 0;
          while (num8 == 0 && ++num9 < 1000)
          {
            num8 = 1;
            int num10 = this.random.Next();
            for (int index3 = 0; index3 < 8; ++index3)
            {
              int num11 = (num6 + numArray1[(index3 + num10) % 8]) % this.sizeX;
              int num12 = (num7 + numArray2[(index3 + num10) % 8]) % this.sizeZ;
              if ((double) this.field[num11 + num12 * this.sizeX] + (double) particleHeight < (double) this.field[num6 + num7 * this.sizeX])
              {
                this.field[num11 + num12 * this.sizeX] += particleHeight;
                this.field[num6 + num7 * this.sizeX] -= particleHeight;
                num6 = num11;
                num7 = num12;
                num8 = 0;
                break;
              }
            }
          }
          if ((double) this.field[num6 + this.sizeX * num7] > (double) this.field[num3 + this.sizeX * num4])
          {
            num3 = num6;
            num4 = num7;
          }
        }
        float num13 = this.field[num3 + this.sizeX * num4];
        float num14 = num13 * (particleHeight - caldera);
        int num15 = num3;
        int num16 = num3;
        int num17 = num4;
        int num18 = num4;
        numArray3[num3 + this.sizeX * num4] = 1;
        int num19 = 0;
        while (num19 == 0)
        {
          num19 = 1;
          int num6 = num15;
          int num7 = num17;
          int num8 = num16;
          int num9 = num18;
          for (int index2 = num6; index2 <= num8; ++index2)
          {
            for (int index3 = num7; index3 <= num9; ++index3)
            {
              int num10 = (index2 + this.sizeX) % this.sizeX;
              int num11 = (index3 + this.sizeZ) % this.sizeZ;
              if (numArray3[num10 + this.sizeX * num11] == 1)
              {
                numArray3[num10 + this.sizeX * num11] = 2;
                if ((double) this.field[num10 + this.sizeX * num11] > (double) num14 && (double) this.field[num10 + this.sizeX * num11] <= (double) num13)
                {
                  num19 = 0;
                  this.field[num10 + this.sizeX * num11] = 2f * num14 - this.field[num10 + this.sizeX * num11];
                  int num12 = (num10 + 1) % this.sizeX;
                  if (numArray3[num12 + this.sizeX * num11] == 0)
                  {
                    if (index2 + 1 > num16)
                      num16 = index2 + 1;
                    numArray3[num12 + this.sizeX * num11] = 1;
                  }
                  int num20 = (num12 + this.sizeX - 2) % this.sizeX;
                  if (numArray3[num20 + this.sizeX * num11] == 0)
                  {
                    if (index2 - 1 < num15)
                      num15 = index2 - 1;
                    numArray3[num20 + this.sizeX * num11] = 1;
                  }
                  int num21 = (index2 + this.sizeX) % this.sizeX;
                  int num22 = (num11 + 1) % this.sizeZ;
                  if (numArray3[num21 + this.sizeX * num22] == 0)
                  {
                    if (index3 + 1 > num18)
                      num18 = index3 + 1;
                    numArray3[num21 + this.sizeX * num22] = 1;
                  }
                  int num23 = (num22 + this.sizeZ - 2) % this.sizeZ;
                  if (numArray3[num21 + this.sizeX * num23] == 0)
                  {
                    if (index3 - 1 < num17)
                      num17 = index3 - 1;
                    numArray3[num21 + this.sizeX * num23] = 1;
                  }
                }
              }
            }
          }
        }
      }
      this.FilterHeightField(caldera);
      this.NormalizeField();
    }

    public void MidpointDisplacement(float rough, float seed)
    {
      this.random = new PcgRandom((int) seed);
      this.MidpointDisplacementCore(rough);
    }

    private void MidpointDisplacementCore(float rough)
    {
      int sizeX = this.sizeX;
      this.field[0] = 0.0f;
      for (int index1 = 1; index1 < 100; ++index1)
      {
        int num1 = sizeX;
        float num2 = (float) this.sizeX / 4f;
        float num3 = (float) Math.Pow(2.0, -1.0 * (double) rough);
        while (num1 > 0)
        {
          for (int index2 = 0; index2 < sizeX; index2 += num1)
          {
            for (int index3 = 0; index3 < sizeX; index3 += num1)
            {
              int num4 = (index2 + num1) % sizeX;
              int num5 = (index3 + num1) % sizeX;
              this.field[index2 + num1 / 2 + (index3 + num1 / 2) * sizeX] = (float) (((double) this.field[index2 + index3 * sizeX] + (double) this.field[num4 + index3 * sizeX] + (double) this.field[index2 + num5 * sizeX] + (double) this.field[num4 + num5 * sizeX]) / 4.0) + (float) this.random.Next((int) (-(double) num2 / 2.0), (int) ((double) num2 / 2.0));
            }
          }
          for (int index2 = 0; index2 < sizeX; index2 += num1)
          {
            for (int index3 = 0; index3 < sizeX; index3 += num1)
            {
              int num4 = (index2 + num1) % sizeX;
              int num5 = (index3 + num1) % sizeX;
              int num6 = index2 + num1 / 2;
              int num7 = index3 + num1 / 2;
              int num8 = (index2 - num1 / 2 + sizeX) % sizeX;
              int num9 = (index3 - num1 / 2 + sizeX) % sizeX;
              this.field[num6 + index3 * sizeX] = (float) (((double) this.field[index2 + index3 * sizeX] + (double) this.field[num4 + index3 * sizeX] + (double) this.field[num6 + num9 * sizeX] + (double) this.field[num6 + num7 * sizeX]) / 4.0) + (float) this.random.Next((int) (-(double) num2 / 2.0), (int) ((double) num2 / 2.0));
              this.field[index2 + num7 * sizeX] = (float) (((double) this.field[index2 + index3 * sizeX] + (double) this.field[index2 + num5 * sizeX] + (double) this.field[num8 + num7 * sizeX] + (double) this.field[num6 + num7 * sizeX]) / 4.0) + (float) this.random.Next((int) (-(double) num2 / 2.0), (int) ((double) num2 / 2.0));
            }
          }
          num1 /= 2;
          num2 *= num3;
        }
      }
      this.NormalizeField();
    }

    private void FilterHeightField(float filter)
    {
      for (int index = 0; index < this.sizeZ; ++index)
        this.FilterHeightBand(this.sizeX * index, 1, this.sizeX, filter);
      for (int index = 0; index < this.sizeZ; ++index)
        this.FilterHeightBand(this.sizeX * index + this.sizeX - 1, -1, this.sizeX, filter);
      for (int start = 0; start < this.sizeX; ++start)
        this.FilterHeightBand(start, this.sizeX, this.sizeZ, filter);
      for (int index = 0; index < this.sizeX; ++index)
        this.FilterHeightBand(this.sizeX * (this.sizeZ - 1) + index, -this.sizeX, this.sizeZ, filter);
    }

    private void FilterHeightBand(int start, int stride, int count, float filter)
    {
      int index1 = start + stride;
      float num1 = this.field[start];
      for (int index2 = 0; index2 < count - 1; ++index2)
      {
        float num2 = (float) ((double) filter * (double) num1 + (1.0 - (double) filter) * (double) this.field[index1]);
        this.field[index1] = num2;
        num1 = num2;
        index1 += stride;
      }
    }

    private void NormalizeField()
    {
      float num1 = this.field[0];
      float num2 = this.field[0];
      for (int index = 1; index < this.field.Length; ++index)
      {
        if ((double) this.field[index] > (double) num1)
          num1 = this.field[index];
        else if ((double) this.field[index] < (double) num2)
          num2 = this.field[index];
      }
      this.lastNormalizationMinVal = num2;
      if ((double) num1 <= (double) num2)
        return;
      this.lastNormalizationScale = num1 - num2;
      for (int index = 0; index < this.field.Length; ++index)
        this.field[index] = (this.field[index] - num2) / this.lastNormalizationScale;
    }

    private void DenormalizeField()
    {
      for (int index = 0; index < this.field.Length; ++index)
        this.field[index] = this.field[index] * this.lastNormalizationScale + this.lastNormalizationMinVal;
    }

    private class ThreadData
    {
      public int Iterations;
      public int MaxDelta;
      public int MinDelta;
      public int IterationsPerErosionFilter;
      public float Erosion;
      public IProgressBar ProgressBar;
      public bool DenormalizeFirst;
      public bool Complete;
    }
  }
}
