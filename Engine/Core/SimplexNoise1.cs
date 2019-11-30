// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.SimplexNoise1
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using System;

namespace StudioForge.Engine.Core
{
  public class SimplexNoise1
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
    private static int[][] grad4 = new int[32][]
    {
      new int[4]{ 0, 1, 1, 1 },
      new int[4]{ 0, 1, 1, -1 },
      new int[4]{ 0, 1, -1, 1 },
      new int[4]{ 0, 1, -1, -1 },
      new int[4]{ 0, -1, 1, 1 },
      new int[4]{ 0, -1, 1, -1 },
      new int[4]{ 0, -1, -1, 1 },
      new int[4]{ 0, -1, -1, -1 },
      new int[4]{ 1, 0, 1, 1 },
      new int[4]{ 1, 0, 1, -1 },
      new int[4]{ 1, 0, -1, 1 },
      new int[4]{ 1, 0, -1, -1 },
      new int[4]{ -1, 0, 1, 1 },
      new int[4]{ -1, 0, 1, -1 },
      new int[4]{ -1, 0, -1, 1 },
      new int[4]{ -1, 0, -1, -1 },
      new int[4]{ 1, 1, 0, 1 },
      new int[4]{ 1, 1, 0, -1 },
      new int[4]{ 1, -1, 0, 1 },
      new int[4]{ 1, -1, 0, -1 },
      new int[4]{ -1, 1, 0, 1 },
      new int[4]{ -1, 1, 0, -1 },
      new int[4]{ -1, -1, 0, 1 },
      new int[4]{ -1, -1, 0, -1 },
      new int[4]{ 1, 1, 1, 0 },
      new int[4]{ 1, 1, -1, 0 },
      new int[4]{ 1, -1, 1, 0 },
      new int[4]{ 1, -1, -1, 0 },
      new int[4]{ -1, 1, 1, 0 },
      new int[4]{ -1, 1, -1, 0 },
      new int[4]{ -1, -1, 1, 0 },
      new int[4]{ -1, -1, -1, 0 }
    };
    private static int[][] simplex = new int[64][]
    {
      new int[4]{ 0, 1, 2, 3 },
      new int[4]{ 0, 1, 3, 2 },
      new int[4],
      new int[4]{ 0, 2, 3, 1 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 1, 2, 3, 0 },
      new int[4]{ 0, 2, 1, 3 },
      new int[4],
      new int[4]{ 0, 3, 1, 2 },
      new int[4]{ 0, 3, 2, 1 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 1, 3, 2, 0 },
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 1, 2, 0, 3 },
      new int[4],
      new int[4]{ 1, 3, 0, 2 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 2, 3, 0, 1 },
      new int[4]{ 2, 3, 1, 0 },
      new int[4]{ 1, 0, 2, 3 },
      new int[4]{ 1, 0, 3, 2 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 2, 0, 3, 1 },
      new int[4],
      new int[4]{ 2, 1, 3, 0 },
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 2, 0, 1, 3 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 3, 0, 1, 2 },
      new int[4]{ 3, 0, 2, 1 },
      new int[4],
      new int[4]{ 3, 1, 2, 0 },
      new int[4]{ 2, 1, 0, 3 },
      new int[4],
      new int[4],
      new int[4],
      new int[4]{ 3, 1, 0, 2 },
      new int[4],
      new int[4]{ 3, 2, 0, 1 },
      new int[4]{ 3, 2, 1, 0 }
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
    private static double sqrt3 = Math.Sqrt(3.0);

    public static int[] GetSimplexNoisePermTable(int seed)
    {
      int[] numArray = new int[512];
      BWRandom bwRandom = new BWRandom(seed, true);
      for (int index = 0; index < 512; ++index)
        numArray[index] = (int) (byte) (SimplexNoise1.p[index & (int) byte.MaxValue] * bwRandom.Next());
      return numArray;
    }

    private static int fastfloor(float x)
    {
      if ((double) x <= 0.0)
        return (int) x - 1;
      return (int) x;
    }

    private static float dot(int[] g, float x, float y)
    {
      return (float) ((double) g[0] * (double) x + (double) g[1] * (double) y);
    }

    private static float dot(int[] g, float x, float y, float z)
    {
      return (float) ((double) g[0] * (double) x + (double) g[1] * (double) y + (double) g[2] * (double) z);
    }

    private static float dot(int[] g, float x, float y, float z, float w)
    {
      return (float) ((double) g[0] * (double) x + (double) g[1] * (double) y + (double) g[2] * (double) z + (double) g[3] * (double) w);
    }

    public static float noise(float xin, float yin, float zin, int[] perm)
    {
      float num1 = 0.3333333f;
      float num2 = (xin + yin + zin) * num1;
      int num3 = SimplexNoise1.fastfloor(xin + num2);
      int num4 = SimplexNoise1.fastfloor(yin + num2);
      int num5 = SimplexNoise1.fastfloor(zin + num2);
      float num6 = 0.1666667f;
      float num7 = (float) (num3 + num4 + num5) * num6;
      float num8 = (float) num3 - num7;
      float num9 = (float) num4 - num7;
      float num10 = (float) num5 - num7;
      float x1 = xin - num8;
      float y1 = yin - num9;
      float z1 = zin - num10;
      int num11;
      int num12;
      int num13;
      int num14;
      int num15;
      int num16;
      if ((double) x1 >= (double) y1)
      {
        if ((double) y1 >= (double) z1)
        {
          num11 = 1;
          num12 = 0;
          num13 = 0;
          num14 = 1;
          num15 = 1;
          num16 = 0;
        }
        else if ((double) x1 >= (double) z1)
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
      else if ((double) y1 < (double) z1)
      {
        num11 = 0;
        num12 = 0;
        num13 = 1;
        num14 = 0;
        num15 = 1;
        num16 = 1;
      }
      else if ((double) x1 < (double) z1)
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
      float x2 = x1 - (float) num11 + num6;
      float y2 = y1 - (float) num12 + num6;
      float z2 = z1 - (float) num13 + num6;
      float x3 = (float) ((double) x1 - (double) num14 + 2.0 * (double) num6);
      float y3 = (float) ((double) y1 - (double) num15 + 2.0 * (double) num6);
      float z3 = (float) ((double) z1 - (double) num16 + 2.0 * (double) num6);
      float x4 = (float) ((double) x1 - 1.0 + 3.0 * (double) num6);
      float y4 = (float) ((double) y1 - 1.0 + 3.0 * (double) num6);
      float z4 = (float) ((double) z1 - 1.0 + 3.0 * (double) num6);
      int num17 = num3 & (int) byte.MaxValue;
      int num18 = num4 & (int) byte.MaxValue;
      int index1 = num5 & (int) byte.MaxValue;
      int index2 = perm[num17 + perm[num18 + perm[index1]]] % 12;
      int index3 = perm[num17 + num11 + perm[num18 + num12 + perm[index1 + num13]]] % 12;
      int index4 = perm[num17 + num14 + perm[num18 + num15 + perm[index1 + num16]]] % 12;
      int index5 = perm[num17 + 1 + perm[num18 + 1 + perm[index1 + 1]]] % 12;
      float num19 = (float) (0.600000023841858 - (double) x1 * (double) x1 - (double) y1 * (double) y1 - (double) z1 * (double) z1);
      float num20;
      if ((double) num19 < 0.0)
      {
        num20 = 0.0f;
      }
      else
      {
        float num21 = num19 * num19;
        num20 = num21 * num21 * SimplexNoise1.dot(SimplexNoise1.grad3[index2], x1, y1, z1);
      }
      float num22 = (float) (0.600000023841858 - (double) x2 * (double) x2 - (double) y2 * (double) y2 - (double) z2 * (double) z2);
      float num23;
      if ((double) num22 < 0.0)
      {
        num23 = 0.0f;
      }
      else
      {
        float num21 = num22 * num22;
        num23 = num21 * num21 * SimplexNoise1.dot(SimplexNoise1.grad3[index3], x2, y2, z2);
      }
      float num24 = (float) (0.600000023841858 - (double) x3 * (double) x3 - (double) y3 * (double) y3 - (double) z3 * (double) z3);
      float num25;
      if ((double) num24 < 0.0)
      {
        num25 = 0.0f;
      }
      else
      {
        float num21 = num24 * num24;
        num25 = num21 * num21 * SimplexNoise1.dot(SimplexNoise1.grad3[index4], x3, y3, z3);
      }
      float num26 = (float) (0.600000023841858 - (double) x4 * (double) x4 - (double) y4 * (double) y4 - (double) z4 * (double) z4);
      float num27;
      if ((double) num26 < 0.0)
      {
        num27 = 0.0f;
      }
      else
      {
        float num21 = num26 * num26;
        num27 = num21 * num21 * SimplexNoise1.dot(SimplexNoise1.grad3[index5], x4, y4, z4);
      }
      return (float) (32.0 * ((double) num20 + (double) num23 + (double) num25 + (double) num27));
    }

    public static float noise(float xin, float yin, int[] perm)
    {
      float num1 = (float) (0.5 * (SimplexNoise1.sqrt3 - 1.0));
      float num2 = (xin + yin) * num1;
      int num3 = SimplexNoise1.fastfloor(xin + num2);
      int num4 = SimplexNoise1.fastfloor(yin + num2);
      float num5 = (float) ((3.0 - SimplexNoise1.sqrt3) / 6.0);
      float num6 = (float) (num3 + num4) * num5;
      float num7 = (float) num3 - num6;
      float num8 = (float) num4 - num6;
      float x1 = xin - num7;
      float y1 = yin - num8;
      int num9;
      int num10;
      if ((double) x1 > (double) y1)
      {
        num9 = 1;
        num10 = 0;
      }
      else
      {
        num9 = 0;
        num10 = 1;
      }
      float x2 = x1 - (float) num9 + num5;
      float y2 = y1 - (float) num10 + num5;
      float x3 = (float) ((double) x1 - 1.0 + 2.0 * (double) num5);
      float y3 = (float) ((double) y1 - 1.0 + 2.0 * (double) num5);
      int num11 = num3 & (int) byte.MaxValue;
      int index1 = num4 & (int) byte.MaxValue;
      int index2 = perm[num11 + perm[index1]] % 12;
      int index3 = perm[num11 + num9 + perm[index1 + num10]] % 12;
      int index4 = perm[num11 + 1 + perm[index1 + 1]] % 12;
      float num12 = (float) (0.5 - (double) x1 * (double) x1 - (double) y1 * (double) y1);
      float num13;
      if ((double) num12 < 0.0)
      {
        num13 = 0.0f;
      }
      else
      {
        float num14 = num12 * num12;
        num13 = num14 * num14 * SimplexNoise1.dot(SimplexNoise1.grad3[index2], x1, y1);
      }
      float num15 = (float) (0.5 - (double) x2 * (double) x2 - (double) y2 * (double) y2);
      float num16;
      if ((double) num15 < 0.0)
      {
        num16 = 0.0f;
      }
      else
      {
        float num14 = num15 * num15;
        num16 = num14 * num14 * SimplexNoise1.dot(SimplexNoise1.grad3[index3], x2, y2);
      }
      float num17 = (float) (0.5 - (double) x3 * (double) x3 - (double) y3 * (double) y3);
      float num18;
      if ((double) num17 < 0.0)
      {
        num18 = 0.0f;
      }
      else
      {
        float num14 = num17 * num17;
        num18 = num14 * num14 * SimplexNoise1.dot(SimplexNoise1.grad3[index4], x3, y3);
      }
      return (float) ((70.0 * ((double) num13 + (double) num16 + (double) num18) + 1.0) * 0.5);
    }
  }
}
