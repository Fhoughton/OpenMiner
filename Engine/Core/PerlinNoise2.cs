// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.PerlinNoise2
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class PerlinNoise2
  {
    private static int[][] grad3 = new int[12][]
    {
      new int[3]{ 1, 1, 0 },
      new int[3]{ -1, 1, 0 },
      new int[3]{ 1, -1, 0 },
      new int[3]{ -1, -1, 0 },
      new int[3]{ 1, 0, 1 },
      new int[3]{ -1, 0, 1 },
      new int[3]{ 1, 0, -1 },
      new int[3]{ -1, 0, -1 },
      new int[3]{ 0, 1, 1 },
      new int[3]{ 0, -1, 1 },
      new int[3]{ 0, 1, -1 },
      new int[3]{ 0, -1, -1 }
    };
    private static int[] p = new int[256]
    {
      151,
      160,
      137,
      91,
      90,
      15,
      131,
      13,
      201,
      95,
      96,
      53,
      194,
      233,
      7,
      225,
      140,
      36,
      103,
      30,
      69,
      142,
      8,
      99,
      37,
      240,
      21,
      10,
      23,
      190,
      6,
      148,
      247,
      120,
      234,
      75,
      0,
      26,
      197,
      62,
      94,
      252,
      219,
      203,
      117,
      35,
      11,
      32,
      57,
      177,
      33,
      88,
      237,
      149,
      56,
      87,
      174,
      20,
      125,
      136,
      171,
      168,
      68,
      175,
      74,
      165,
      71,
      134,
      139,
      48,
      27,
      166,
      77,
      146,
      158,
      231,
      83,
      111,
      229,
      122,
      60,
      211,
      133,
      230,
      220,
      105,
      92,
      41,
      55,
      46,
      245,
      40,
      244,
      102,
      143,
      54,
      65,
      25,
      63,
      161,
      1,
      216,
      80,
      73,
      209,
      76,
      132,
      187,
      208,
      89,
      18,
      169,
      200,
      196,
      135,
      130,
      116,
      188,
      159,
      86,
      164,
      100,
      109,
      198,
      173,
      186,
      3,
      64,
      52,
      217,
      226,
      250,
      124,
      123,
      5,
      202,
      38,
      147,
      118,
      126,
      (int) byte.MaxValue,
      82,
      85,
      212,
      207,
      206,
      59,
      227,
      47,
      16,
      58,
      17,
      182,
      189,
      28,
      42,
      223,
      183,
      170,
      213,
      119,
      248,
      152,
      2,
      44,
      154,
      163,
      70,
      221,
      153,
      101,
      155,
      167,
      43,
      172,
      9,
      129,
      22,
      39,
      253,
      19,
      98,
      108,
      110,
      79,
      113,
      224,
      232,
      178,
      185,
      112,
      104,
      218,
      246,
      97,
      228,
      251,
      34,
      242,
      193,
      238,
      210,
      144,
      12,
      191,
      179,
      162,
      241,
      81,
      51,
      145,
      235,
      249,
      14,
      239,
      107,
      49,
      192,
      214,
      31,
      181,
      199,
      106,
      157,
      184,
      84,
      204,
      176,
      115,
      121,
      50,
      45,
      (int) sbyte.MaxValue,
      4,
      150,
      254,
      138,
      236,
      205,
      93,
      222,
      114,
      67,
      29,
      24,
      72,
      243,
      141,
      128,
      195,
      78,
      66,
      215,
      61,
      156,
      180
    };
    private static int[] perm = new int[512];

    static PerlinNoise2()
    {
      for (int index = 0; index < 512; ++index)
        PerlinNoise2.perm[index] = PerlinNoise2.p[index & (int) byte.MaxValue];
    }

    private static int fastfloor(double x)
    {
      if (x <= 0.0)
        return (int) x - 1;
      return (int) x;
    }

    private static double dot(int[] g, double x, double y)
    {
      return (double) g[0] * x + (double) g[1] * y;
    }

    private static double dot(int[] g, double x, double y, double z)
    {
      return (double) g[0] * x + (double) g[1] * y + (double) g[2] * z;
    }

    private static double dot(int[] g, double x, double y, double z, double w)
    {
      return (double) g[0] * x + (double) g[1] * y + (double) g[2] * z + (double) g[3] * w;
    }

    public double noise(double xin, double yin)
    {
      double num1 = 0.5 * (Math.Sqrt(3.0) - 1.0);
      double num2 = (xin + yin) * num1;
      int num3 = PerlinNoise2.fastfloor(xin + num2);
      int num4 = PerlinNoise2.fastfloor(yin + num2);
      double num5 = (3.0 - Math.Sqrt(3.0)) / 6.0;
      double num6 = (double) (num3 + num4) * num5;
      double num7 = (double) num3 - num6;
      double num8 = (double) num4 - num6;
      double x1 = xin - num7;
      double y1 = yin - num8;
      int num9;
      int num10;
      if (x1 > y1)
      {
        num9 = 1;
        num10 = 0;
      }
      else
      {
        num9 = 0;
        num10 = 1;
      }
      double x2 = x1 - (double) num9 + num5;
      double y2 = y1 - (double) num10 + num5;
      double x3 = x1 - 1.0 + 2.0 * num5;
      double y3 = y1 - 1.0 + 2.0 * num5;
      int num11 = num3 & (int) byte.MaxValue;
      int index1 = num4 & (int) byte.MaxValue;
      int index2 = PerlinNoise2.perm[num11 + PerlinNoise2.perm[index1]] % 12;
      int index3 = PerlinNoise2.perm[num11 + num9 + PerlinNoise2.perm[index1 + num10]] % 12;
      int index4 = PerlinNoise2.perm[num11 + 1 + PerlinNoise2.perm[index1 + 1]] % 12;
      double num12 = 0.5 - x1 * x1 - y1 * y1;
      double num13;
      if (num12 < 0.0)
      {
        num13 = 0.0;
      }
      else
      {
        double num14 = num12 * num12;
        num13 = num14 * num14 * PerlinNoise2.dot(PerlinNoise2.grad3[index2], x1, y1);
      }
      double num15 = 0.5 - x2 * x2 - y2 * y2;
      double num16;
      if (num15 < 0.0)
      {
        num16 = 0.0;
      }
      else
      {
        double num14 = num15 * num15;
        num16 = num14 * num14 * PerlinNoise2.dot(PerlinNoise2.grad3[index3], x2, y2);
      }
      double num17 = 0.5 - x3 * x3 - y3 * y3;
      double num18;
      if (num17 < 0.0)
      {
        num18 = 0.0;
      }
      else
      {
        double num14 = num17 * num17;
        num18 = num14 * num14 * PerlinNoise2.dot(PerlinNoise2.grad3[index4], x3, y3);
      }
      return 70.0 * (num13 + num16 + num18);
    }

    public double noise(double xin, double yin, double zin)
    {
      double num1 = 1.0 / 3.0;
      double num2 = (xin + yin + zin) * num1;
      int num3 = PerlinNoise2.fastfloor(xin + num2);
      int num4 = PerlinNoise2.fastfloor(yin + num2);
      int num5 = PerlinNoise2.fastfloor(zin + num2);
      double num6 = 1.0 / 6.0;
      double num7 = (double) (num3 + num4 + num5) * num6;
      double num8 = (double) num3 - num7;
      double num9 = (double) num4 - num7;
      double num10 = (double) num5 - num7;
      double x1 = xin - num8;
      double y1 = yin - num9;
      double z1 = zin - num10;
      int num11;
      int num12;
      int num13;
      int num14;
      int num15;
      int num16;
      if (x1 >= y1)
      {
        if (y1 >= z1)
        {
          num11 = 1;
          num12 = 0;
          num13 = 0;
          num14 = 1;
          num15 = 1;
          num16 = 0;
        }
        else if (x1 >= z1)
        {
          num11 = 1;
          num12 = 0;
          num13 = 0;
          num14 = 1;
          num15 = 0;
          num16 = 1;
        }
        else
        {
          num11 = 0;
          num12 = 0;
          num13 = 1;
          num14 = 1;
          num15 = 0;
          num16 = 1;
        }
      }
      else if (y1 < z1)
      {
        num11 = 0;
        num12 = 0;
        num13 = 1;
        num14 = 0;
        num15 = 1;
        num16 = 1;
      }
      else if (x1 < z1)
      {
        num11 = 0;
        num12 = 1;
        num13 = 0;
        num14 = 0;
        num15 = 1;
        num16 = 1;
      }
      else
      {
        num11 = 0;
        num12 = 1;
        num13 = 0;
        num14 = 1;
        num15 = 1;
        num16 = 0;
      }
      double x2 = x1 - (double) num11 + num6;
      double y2 = y1 - (double) num12 + num6;
      double z2 = z1 - (double) num13 + num6;
      double x3 = x1 - (double) num14 + 2.0 * num6;
      double y3 = y1 - (double) num15 + 2.0 * num6;
      double z3 = z1 - (double) num16 + 2.0 * num6;
      double x4 = x1 - 1.0 + 3.0 * num6;
      double y4 = y1 - 1.0 + 3.0 * num6;
      double z4 = z1 - 1.0 + 3.0 * num6;
      int num17 = num3 & (int) byte.MaxValue;
      int num18 = num4 & (int) byte.MaxValue;
      int index1 = num5 & (int) byte.MaxValue;
      int index2 = PerlinNoise2.perm[num17 + PerlinNoise2.perm[num18 + PerlinNoise2.perm[index1]]] % 12;
      int index3 = PerlinNoise2.perm[num17 + num11 + PerlinNoise2.perm[num18 + num12 + PerlinNoise2.perm[index1 + num13]]] % 12;
      int index4 = PerlinNoise2.perm[num17 + num14 + PerlinNoise2.perm[num18 + num15 + PerlinNoise2.perm[index1 + num16]]] % 12;
      int index5 = PerlinNoise2.perm[num17 + 1 + PerlinNoise2.perm[num18 + 1 + PerlinNoise2.perm[index1 + 1]]] % 12;
      double num19 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
      double num20;
      if (num19 < 0.0)
      {
        num20 = 0.0;
      }
      else
      {
        double num21 = num19 * num19;
        num20 = num21 * num21 * PerlinNoise2.dot(PerlinNoise2.grad3[index2], x1, y1, z1);
      }
      double num22 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
      double num23;
      if (num22 < 0.0)
      {
        num23 = 0.0;
      }
      else
      {
        double num21 = num22 * num22;
        num23 = num21 * num21 * PerlinNoise2.dot(PerlinNoise2.grad3[index3], x2, y2, z2);
      }
      double num24 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
      double num25;
      if (num24 < 0.0)
      {
        num25 = 0.0;
      }
      else
      {
        double num21 = num24 * num24;
        num25 = num21 * num21 * PerlinNoise2.dot(PerlinNoise2.grad3[index4], x3, y3, z3);
      }
      double num26 = 0.6 - x4 * x4 - y4 * y4 - z4 * z4;
      double num27;
      if (num26 < 0.0)
      {
        num27 = 0.0;
      }
      else
      {
        double num21 = num26 * num26;
        num27 = num21 * num21 * PerlinNoise2.dot(PerlinNoise2.grad3[index5], x4, y4, z4);
      }
      return 32.0 * (num20 + num23 + num25 + num27);
    }
  }
}
