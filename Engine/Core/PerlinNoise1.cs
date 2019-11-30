// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PerlinNoise1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class PerlinNoise1
  {
    private readonly double[] _gradients = new double[768];
    private readonly byte[] _perm = new byte[256]
    {
      (byte) 225,
      (byte) 155,
      (byte) 210,
      (byte) 108,
      (byte) 175,
      (byte) 199,
      (byte) 221,
      (byte) 144,
      (byte) 203,
      (byte) 116,
      (byte) 70,
      (byte) 213,
      (byte) 69,
      (byte) 158,
      (byte) 33,
      (byte) 252,
      (byte) 5,
      (byte) 82,
      (byte) 173,
      (byte) 133,
      (byte) 222,
      (byte) 139,
      (byte) 174,
      (byte) 27,
      (byte) 9,
      (byte) 71,
      (byte) 90,
      (byte) 246,
      (byte) 75,
      (byte) 130,
      (byte) 91,
      (byte) 191,
      (byte) 169,
      (byte) 138,
      (byte) 2,
      (byte) 151,
      (byte) 194,
      (byte) 235,
      (byte) 81,
      (byte) 7,
      (byte) 25,
      (byte) 113,
      (byte) 228,
      (byte) 159,
      (byte) 205,
      (byte) 253,
      (byte) 134,
      (byte) 142,
      (byte) 248,
      (byte) 65,
      (byte) 224,
      (byte) 217,
      (byte) 22,
      (byte) 121,
      (byte) 229,
      (byte) 63,
      (byte) 89,
      (byte) 103,
      (byte) 96,
      (byte) 104,
      (byte) 156,
      (byte) 17,
      (byte) 201,
      (byte) 129,
      (byte) 36,
      (byte) 8,
      (byte) 165,
      (byte) 110,
      (byte) 237,
      (byte) 117,
      (byte) 231,
      (byte) 56,
      (byte) 132,
      (byte) 211,
      (byte) 152,
      (byte) 20,
      (byte) 181,
      (byte) 111,
      (byte) 239,
      (byte) 218,
      (byte) 170,
      (byte) 163,
      (byte) 51,
      (byte) 172,
      (byte) 157,
      (byte) 47,
      (byte) 80,
      (byte) 212,
      (byte) 176,
      (byte) 250,
      (byte) 87,
      (byte) 49,
      (byte) 99,
      (byte) 242,
      (byte) 136,
      (byte) 189,
      (byte) 162,
      (byte) 115,
      (byte) 44,
      (byte) 43,
      (byte) 124,
      (byte) 94,
      (byte) 150,
      (byte) 16,
      (byte) 141,
      (byte) 247,
      (byte) 32,
      (byte) 10,
      (byte) 198,
      (byte) 223,
      byte.MaxValue,
      (byte) 72,
      (byte) 53,
      (byte) 131,
      (byte) 84,
      (byte) 57,
      (byte) 220,
      (byte) 197,
      (byte) 58,
      (byte) 50,
      (byte) 208,
      (byte) 11,
      (byte) 241,
      (byte) 28,
      (byte) 3,
      (byte) 192,
      (byte) 62,
      (byte) 202,
      (byte) 18,
      (byte) 215,
      (byte) 153,
      (byte) 24,
      (byte) 76,
      (byte) 41,
      (byte) 15,
      (byte) 179,
      (byte) 39,
      (byte) 46,
      (byte) 55,
      (byte) 6,
      (byte) 128,
      (byte) 167,
      (byte) 23,
      (byte) 188,
      (byte) 106,
      (byte) 34,
      (byte) 187,
      (byte) 140,
      (byte) 164,
      (byte) 73,
      (byte) 112,
      (byte) 182,
      (byte) 244,
      (byte) 195,
      (byte) 227,
      (byte) 13,
      (byte) 35,
      (byte) 77,
      (byte) 196,
      (byte) 185,
      (byte) 26,
      (byte) 200,
      (byte) 226,
      (byte) 119,
      (byte) 31,
      (byte) 123,
      (byte) 168,
      (byte) 125,
      (byte) 249,
      (byte) 68,
      (byte) 183,
      (byte) 230,
      (byte) 177,
      (byte) 135,
      (byte) 160,
      (byte) 180,
      (byte) 12,
      (byte) 1,
      (byte) 243,
      (byte) 148,
      (byte) 102,
      (byte) 166,
      (byte) 38,
      (byte) 238,
      (byte) 251,
      (byte) 37,
      (byte) 240,
      (byte) 126,
      (byte) 64,
      (byte) 74,
      (byte) 161,
      (byte) 40,
      (byte) 184,
      (byte) 149,
      (byte) 171,
      (byte) 178,
      (byte) 101,
      (byte) 66,
      (byte) 29,
      (byte) 59,
      (byte) 146,
      (byte) 61,
      (byte) 254,
      (byte) 107,
      (byte) 42,
      (byte) 86,
      (byte) 154,
      (byte) 4,
      (byte) 236,
      (byte) 232,
      (byte) 120,
      (byte) 21,
      (byte) 233,
      (byte) 209,
      (byte) 45,
      (byte) 98,
      (byte) 193,
      (byte) 114,
      (byte) 78,
      (byte) 19,
      (byte) 206,
      (byte) 14,
      (byte) 118,
      (byte) 127,
      (byte) 48,
      (byte) 79,
      (byte) 147,
      (byte) 85,
      (byte) 30,
      (byte) 207,
      (byte) 219,
      (byte) 54,
      (byte) 88,
      (byte) 234,
      (byte) 190,
      (byte) 122,
      (byte) 95,
      (byte) 67,
      (byte) 143,
      (byte) 109,
      (byte) 137,
      (byte) 214,
      (byte) 145,
      (byte) 93,
      (byte) 92,
      (byte) 100,
      (byte) 245,
      (byte) 0,
      (byte) 216,
      (byte) 186,
      (byte) 60,
      (byte) 83,
      (byte) 105,
      (byte) 97,
      (byte) 204,
      (byte) 52
    };
    private const int GradientSizeTable = 256;
    private readonly Random _random;

    public PerlinNoise1(int seed)
    {
      this._random = new Random(seed);
      this.InitGradients();
    }

    public double Noise(double x, double y, double z)
    {
      int ix = (int) Math.Floor(x);
      double num1 = x - (double) ix;
      double fx = num1 - 1.0;
      double t1 = this.Smooth(num1);
      int iy = (int) Math.Floor(y);
      double num2 = y - (double) iy;
      double fy = num2 - 1.0;
      double t2 = this.Smooth(num2);
      int iz = (int) Math.Floor(z);
      double num3 = z - (double) iz;
      double fz = num3 - 1.0;
      double t3 = this.Smooth(num3);
      double num4 = this.Lattice(ix, iy, iz, num1, num2, num3);
      double num5 = this.Lattice(ix + 1, iy, iz, fx, num2, num3);
      double num6 = this.Lerp(t1, num4, num5);
      double num7 = this.Lattice(ix, iy + 1, iz, num1, fy, num3);
      double num8 = this.Lattice(ix + 1, iy + 1, iz, fx, fy, num3);
      double num9 = this.Lerp(t1, num7, num8);
      double num10 = this.Lerp(t2, num6, num9);
      double num11 = this.Lattice(ix, iy, iz + 1, num1, num2, fz);
      double num12 = this.Lattice(ix + 1, iy, iz + 1, fx, num2, fz);
      double num13 = this.Lerp(t1, num11, num12);
      double num14 = this.Lattice(ix, iy + 1, iz + 1, num1, fy, fz);
      double num15 = this.Lattice(ix + 1, iy + 1, iz + 1, fx, fy, fz);
      double num16 = this.Lerp(t1, num14, num15);
      double num17 = this.Lerp(t2, num13, num16);
      return this.Lerp(t3, num10, num17);
    }

    private void InitGradients()
    {
      for (int index = 0; index < 256; ++index)
      {
        double num1 = 1.0 - 2.0 * this._random.NextDouble();
        double num2 = Math.Sqrt(1.0 - num1 * num1);
        double num3 = 2.0 * Math.PI * this._random.NextDouble();
        this._gradients[index * 3] = num2 * Math.Cos(num3);
        this._gradients[index * 3 + 1] = num2 * Math.Sin(num3);
        this._gradients[index * 3 + 2] = num1;
      }
    }

    private int Permutate(int x)
    {
      return (int) this._perm[x & (int) byte.MaxValue];
    }

    private int Index(int ix, int iy, int iz)
    {
      return this.Permutate(ix + this.Permutate(iy + this.Permutate(iz)));
    }

    private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
    {
      int index = this.Index(ix, iy, iz) * 3;
      return this._gradients[index] * fx + this._gradients[index + 1] * fy + this._gradients[index + 2] * fz;
    }

    private double Lerp(double t, double value0, double value1)
    {
      return value0 + t * (value1 - value0);
    }

    private double Smooth(double x)
    {
      return x * x * (3.0 - 2.0 * x);
    }
  }
}
