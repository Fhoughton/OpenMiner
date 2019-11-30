// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.TeapotPrimitive
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StudioForge.Engine.Graphics3D
{
  public class TeapotPrimitive : BezierPrimitive
  {
    private static TeapotPrimitive.TeapotPatch[] TeapotPatches = new TeapotPrimitive.TeapotPatch[10]
    {
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        102,
        103,
        104,
        105,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14,
        15
      }),
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        12,
        13,
        14,
        15,
        16,
        17,
        18,
        19,
        20,
        21,
        22,
        23,
        24,
        25,
        26,
        27
      }),
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        24,
        25,
        26,
        27,
        29,
        30,
        31,
        32,
        33,
        34,
        35,
        36,
        37,
        38,
        39,
        40
      }),
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        96,
        96,
        96,
        96,
        97,
        98,
        99,
        100,
        101,
        101,
        101,
        101,
        0,
        1,
        2,
        3
      }),
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        0,
        1,
        2,
        3,
        106,
        107,
        108,
        109,
        110,
        111,
        112,
        113,
        114,
        115,
        116,
        117
      }),
      new TeapotPrimitive.TeapotPatch(false, new int[16]
      {
        41,
        42,
        43,
        44,
        45,
        46,
        47,
        48,
        49,
        50,
        51,
        52,
        53,
        54,
        55,
        56
      }),
      new TeapotPrimitive.TeapotPatch(false, new int[16]
      {
        53,
        54,
        55,
        56,
        57,
        58,
        59,
        60,
        61,
        62,
        63,
        64,
        28,
        65,
        66,
        67
      }),
      new TeapotPrimitive.TeapotPatch(false, new int[16]
      {
        68,
        69,
        70,
        71,
        72,
        73,
        74,
        75,
        76,
        77,
        78,
        79,
        80,
        81,
        82,
        83
      }),
      new TeapotPrimitive.TeapotPatch(false, new int[16]
      {
        80,
        81,
        82,
        83,
        84,
        85,
        86,
        87,
        88,
        89,
        90,
        91,
        92,
        93,
        94,
        95
      }),
      new TeapotPrimitive.TeapotPatch(true, new int[16]
      {
        118,
        118,
        118,
        118,
        124,
        122,
        119,
        121,
        123,
        126,
        125,
        120,
        40,
        39,
        38,
        37
      })
    };
    private static Vector3[] TeapotControlPoints = new Vector3[(int) sbyte.MaxValue]
    {
      new Vector3(0.0f, 0.345f, -0.05f),
      new Vector3(-0.028f, 0.345f, -0.05f),
      new Vector3(-0.05f, 0.345f, -0.028f),
      new Vector3(-0.05f, 0.345f, -0.0f),
      new Vector3(0.0f, 0.3028125f, -0.334375f),
      new Vector3(-0.18725f, 0.3028125f, -0.334375f),
      new Vector3(-0.334375f, 0.3028125f, -0.18725f),
      new Vector3(-0.334375f, 0.3028125f, -0.0f),
      new Vector3(0.0f, 0.3028125f, -23f / 64f),
      new Vector3(-0.20125f, 0.3028125f, -23f / 64f),
      new Vector3(-23f / 64f, 0.3028125f, -0.20125f),
      new Vector3(-23f / 64f, 0.3028125f, -0.0f),
      new Vector3(0.0f, 0.27f, -0.375f),
      new Vector3(-0.21f, 0.27f, -0.375f),
      new Vector3(-0.375f, 0.27f, -0.21f),
      new Vector3(-0.375f, 0.27f, -0.0f),
      new Vector3(0.0f, 0.13875f, -7f / 16f),
      new Vector3(-0.245f, 0.13875f, -7f / 16f),
      new Vector3(-7f / 16f, 0.13875f, -0.245f),
      new Vector3(-7f / 16f, 0.13875f, -0.0f),
      new Vector3(0.0f, 0.007499993f, -0.5f),
      new Vector3(-0.28f, 0.007499993f, -0.5f),
      new Vector3(-0.5f, 0.007499993f, -0.28f),
      new Vector3(-0.5f, 0.007499993f, -0.0f),
      new Vector3(0.0f, -0.105f, -0.5f),
      new Vector3(-0.28f, -0.105f, -0.5f),
      new Vector3(-0.5f, -0.105f, -0.28f),
      new Vector3(-0.5f, -0.105f, -0.0f),
      new Vector3(0.0f, -0.105f, 0.5f),
      new Vector3(0.0f, -0.2175f, -0.5f),
      new Vector3(-0.28f, -0.2175f, -0.5f),
      new Vector3(-0.5f, -0.2175f, -0.28f),
      new Vector3(-0.5f, -0.2175f, -0.0f),
      new Vector3(0.0f, -0.27375f, -0.375f),
      new Vector3(-0.21f, -0.27375f, -0.375f),
      new Vector3(-0.375f, -0.27375f, -0.21f),
      new Vector3(-0.375f, -0.27375f, -0.0f),
      new Vector3(0.0f, -0.2925f, -0.375f),
      new Vector3(-0.21f, -0.2925f, -0.375f),
      new Vector3(-0.375f, -0.2925f, -0.21f),
      new Vector3(-0.375f, -0.2925f, -0.0f),
      new Vector3(0.0f, 0.17625f, 0.4f),
      new Vector3(-0.075f, 0.17625f, 0.4f),
      new Vector3(-0.075f, 0.2325f, 0.375f),
      new Vector3(0.0f, 0.2325f, 0.375f),
      new Vector3(0.0f, 0.17625f, 0.575f),
      new Vector3(-0.075f, 0.17625f, 0.575f),
      new Vector3(-0.075f, 0.2325f, 0.625f),
      new Vector3(0.0f, 0.2325f, 0.625f),
      new Vector3(0.0f, 0.17625f, 0.675f),
      new Vector3(-0.075f, 0.17625f, 0.675f),
      new Vector3(-0.075f, 0.2325f, 0.75f),
      new Vector3(0.0f, 0.2325f, 0.75f),
      new Vector3(0.0f, 0.12f, 0.675f),
      new Vector3(-0.075f, 0.12f, 0.675f),
      new Vector3(-0.075f, 0.12f, 0.75f),
      new Vector3(0.0f, 0.12f, 0.75f),
      new Vector3(0.0f, 0.06375f, 0.675f),
      new Vector3(-0.075f, 0.06375f, 0.675f),
      new Vector3(-0.075f, 0.007499993f, 0.75f),
      new Vector3(0.0f, 0.007499993f, 0.75f),
      new Vector3(0.0f, -0.04875001f, 0.625f),
      new Vector3(-0.075f, -0.04875001f, 0.625f),
      new Vector3(-0.075f, -0.09562501f, 0.6625f),
      new Vector3(0.0f, -0.09562501f, 0.6625f),
      new Vector3(-0.075f, -0.105f, 0.5f),
      new Vector3(-0.075f, -0.18f, 0.475f),
      new Vector3(0.0f, -0.18f, 0.475f),
      new Vector3(0.0f, 0.02624997f, -0.425f),
      new Vector3(-0.165f, 0.02624997f, -0.425f),
      new Vector3(-0.165f, -0.18f, -0.425f),
      new Vector3(0.0f, -0.18f, -0.425f),
      new Vector3(0.0f, 0.02624997f, -0.65f),
      new Vector3(-0.165f, 0.02624997f, -0.65f),
      new Vector3(-0.165f, -0.12375f, -0.775f),
      new Vector3(0.0f, -0.12375f, -0.775f),
      new Vector3(0.0f, 0.195f, -0.575f),
      new Vector3(-1f / 16f, 0.195f, -0.575f),
      new Vector3(-1f / 16f, 0.17625f, -0.6f),
      new Vector3(0.0f, 0.17625f, -0.6f),
      new Vector3(0.0f, 0.27f, -0.675f),
      new Vector3(-1f / 16f, 0.27f, -0.675f),
      new Vector3(-1f / 16f, 0.27f, -0.825f),
      new Vector3(0.0f, 0.27f, -0.825f),
      new Vector3(0.0f, 0.28875f, -0.7f),
      new Vector3(-1f / 16f, 0.28875f, -0.7f),
      new Vector3(-1f / 16f, 0.2934375f, -0.88125f),
      new Vector3(0.0f, 0.2934375f, -0.88125f),
      new Vector3(0.0f, 0.28875f, -0.725f),
      new Vector3(-0.0375f, 0.28875f, -0.725f),
      new Vector3(-0.0375f, 0.298125f, -0.8625f),
      new Vector3(0.0f, 0.298125f, -0.8625f),
      new Vector3(0.0f, 0.27f, -0.7f),
      new Vector3(-0.0375f, 0.27f, -0.7f),
      new Vector3(-0.0375f, 0.27f, -0.8f),
      new Vector3(0.0f, 0.27f, -0.8f),
      new Vector3(0.0f, 0.4575f, -0.0f),
      new Vector3(0.0f, 0.4575f, -0.2f),
      new Vector3(-0.1125f, 0.4575f, -0.2f),
      new Vector3(-0.2f, 0.4575f, -0.1125f),
      new Vector3(-0.2f, 0.4575f, -0.0f),
      new Vector3(0.0f, 0.3825f, -0.0f),
      new Vector3(0.0f, 0.27f, -0.35f),
      new Vector3(-0.196f, 0.27f, -0.35f),
      new Vector3(-0.35f, 0.27f, -0.196f),
      new Vector3(-0.35f, 0.27f, -0.0f),
      new Vector3(0.0f, 0.3075f, -0.1f),
      new Vector3(-0.056f, 0.3075f, -0.1f),
      new Vector3(-0.1f, 0.3075f, -0.056f),
      new Vector3(-0.1f, 0.3075f, -0.0f),
      new Vector3(0.0f, 0.3075f, -0.325f),
      new Vector3(-0.182f, 0.3075f, -0.325f),
      new Vector3(-0.325f, 0.3075f, -0.182f),
      new Vector3(-0.325f, 0.3075f, -0.0f),
      new Vector3(0.0f, 0.27f, -0.325f),
      new Vector3(-0.182f, 0.27f, -0.325f),
      new Vector3(-0.325f, 0.27f, -0.182f),
      new Vector3(-0.325f, 0.27f, -0.0f),
      new Vector3(0.0f, -0.33f, -0.0f),
      new Vector3(-0.1995f, -0.33f, -0.35625f),
      new Vector3(0.0f, -0.31125f, -0.375f),
      new Vector3(0.0f, -0.33f, -0.35625f),
      new Vector3(-0.35625f, -0.33f, -0.1995f),
      new Vector3(-0.375f, -0.31125f, -0.0f),
      new Vector3(-0.35625f, -0.33f, -0.0f),
      new Vector3(-0.21f, -0.31125f, -0.375f),
      new Vector3(-0.375f, -0.31125f, -0.21f)
    };

    public TeapotPrimitive(GraphicsDevice graphicsDevice)
      : this(graphicsDevice, 1f, 8)
    {
    }

    public TeapotPrimitive(GraphicsDevice graphicsDevice, float size, int tessellation)
    {
      if (tessellation < 1)
        throw new ArgumentOutOfRangeException(nameof (tessellation));
      foreach (TeapotPrimitive.TeapotPatch teapotPatch in TeapotPrimitive.TeapotPatches)
      {
        this.TessellatePatch(teapotPatch, tessellation, new Vector3(size, size, size));
        this.TessellatePatch(teapotPatch, tessellation, new Vector3(-size, size, size));
        if (teapotPatch.MirrorZ)
        {
          this.TessellatePatch(teapotPatch, tessellation, new Vector3(size, size, -size));
          this.TessellatePatch(teapotPatch, tessellation, new Vector3(-size, size, -size));
        }
      }
      this.InitializePrimitive(graphicsDevice);
    }

    private void TessellatePatch(
      TeapotPrimitive.TeapotPatch patch,
      int tessellation,
      Vector3 scale)
    {
      Vector3[] patch1 = new Vector3[16];
      for (int index1 = 0; index1 < 16; ++index1)
      {
        int index2 = patch.Indices[index1];
        patch1[index1] = TeapotPrimitive.TeapotControlPoints[index2] * scale;
      }
      bool isMirrored = Math.Sign(scale.X) != Math.Sign(scale.Z);
      this.CreatePatchIndices(tessellation, isMirrored);
      this.CreatePatchVertices(patch1, tessellation, isMirrored);
    }

    private class TeapotPatch
    {
      public readonly int[] Indices;
      public readonly bool MirrorZ;

      public TeapotPatch(bool mirrorZ, int[] indices)
      {
        this.Indices = indices;
        this.MirrorZ = mirrorZ;
      }
    }
  }
}
