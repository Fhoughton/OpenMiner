// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Graphics.CubePrimitive
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StudioForge.TotalMiner.Graphics
{
  internal class CubePrimitive
  {
    private VertexPositionColor temp = new VertexPositionColor();
    public short[] Indices;
    public VertexPositionColor[] Vertices;
    private int index;

    public CubePrimitive()
    {
      this.Indices = new short[54]
      {
        (short) 0,
        (short) 1,
        (short) 2,
        (short) 0,
        (short) 2,
        (short) 3,
        (short) 4,
        (short) 5,
        (short) 6,
        (short) 4,
        (short) 6,
        (short) 7,
        (short) 8,
        (short) 9,
        (short) 10,
        (short) 8,
        (short) 10,
        (short) 11,
        (short) 12,
        (short) 13,
        (short) 14,
        (short) 12,
        (short) 14,
        (short) 15,
        (short) 16,
        (short) 17,
        (short) 18,
        (short) 16,
        (short) 18,
        (short) 19,
        (short) 20,
        (short) 21,
        (short) 22,
        (short) 20,
        (short) 22,
        (short) 23,
        (short) 24,
        (short) 25,
        (short) 26,
        (short) 24,
        (short) 26,
        (short) 27,
        (short) 28,
        (short) 29,
        (short) 30,
        (short) 28,
        (short) 30,
        (short) 31,
        (short) 32,
        (short) 33,
        (short) 34,
        (short) 32,
        (short) 34,
        (short) 35
      };
      this.Vertices = new VertexPositionColor[24];
    }

    public void Build(Vector3 viewPoint, Vector3 min, Vector3 max, Color color)
    {
      float num1 = max.X - min.X;
      float num2 = max.Z - min.Z;
      float num3 = max.Y - min.Y;
      this.index = 0;
      this.AddVertex(min, color);
      min.X += num1;
      this.AddVertex(min, color);
      min.Y += num3;
      this.AddVertex(min, color);
      min.X -= num1;
      this.AddVertex(min, color);
      min.Y -= num3;
      min.Z += num2;
      this.AddVertex(min, color);
      min.Z -= num2;
      this.AddVertex(min, color);
      min.Y += num3;
      this.AddVertex(min, color);
      min.Z += num2;
      this.AddVertex(min, color);
      min.Y -= num3;
      min.X += num1;
      this.AddVertex(min, color);
      min.X -= num1;
      this.AddVertex(min, color);
      min.Y += num3;
      this.AddVertex(min, color);
      min.X += num1;
      this.AddVertex(min, color);
      min.Y -= num3;
      min.Z -= num2;
      this.AddVertex(min, color);
      min.Z += num2;
      this.AddVertex(min, color);
      min.Y += num3;
      this.AddVertex(min, color);
      min.Z -= num2;
      this.AddVertex(min, color);
      min.X -= num1;
      this.AddVertex(min, color);
      min.X += num1;
      this.AddVertex(min, color);
      min.Z += num2;
      this.AddVertex(min, color);
      min.X -= num1;
      this.AddVertex(min, color);
      min.Y -= num3;
      this.AddVertex(min, color);
      min.X += num1;
      this.AddVertex(min, color);
      min.Z -= num2;
      this.AddVertex(min, color);
      min.X -= num1;
      this.AddVertex(min, color);
    }

    private void AddVertex(Vector3 pos, Color color)
    {
      this.temp.Position = pos;
      this.temp.Color = color;
      this.Vertices[this.index++] = this.temp;
    }
  }
}
