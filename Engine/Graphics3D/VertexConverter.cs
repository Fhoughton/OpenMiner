// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.VertexConverter
// Assembly: StudioForge.Engine.Graphics3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 23D4CDA5-24AA-4D34-B554-436CECC42F94
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Graphics3D
{
  public class VertexConverter
  {
    public VertexBuffer ConvertVertexBuffer(
      GraphicsDevice device,
      IModelPart part,
      VertexDeclaration to)
    {
      return this.ConvertVertexBuffer(device, part.VertexBuffer, to, part.VertexCount);
    }

    public VertexBuffer ConvertVertexBuffer(
      GraphicsDevice device,
      VertexBuffer vertexBuffer,
      VertexDeclaration to,
      int vertexCount)
    {
      return new VertexBuffer(device, to, vertexCount, BufferUsage.None);
    }

    private void CopyElementData(
      byte[] oldData,
      byte[] newData,
      VertexElement oldElement,
      VertexElement newElement,
      int vertexCount,
      int oldVertexStride,
      int newVertexStride)
    {
      for (int index1 = 0; index1 < vertexCount; ++index1)
      {
        int num1 = index1 * oldVertexStride + oldElement.Offset;
        int num2 = index1 * newVertexStride + newElement.Offset;
        for (int index2 = 0; index2 < this.GetElementSizeInBytes(oldElement); ++index2)
          newData[num2++] = oldData[num1++];
      }
    }

    private int FindMatchingElement(VertexElement elementToFind, VertexElement[] sourceDataFormat)
    {
      for (int index = 0; index < sourceDataFormat.Length; ++index)
      {
        if (sourceDataFormat[index].VertexElementUsage == elementToFind.VertexElementUsage)
          return index;
      }
      return -1;
    }

    private int GetElementSizeInBytes(VertexElement element)
    {
      switch (element.VertexElementFormat)
      {
        case VertexElementFormat.Single:
          return 4;
        case VertexElementFormat.Vector2:
          return Marshal.SizeOf(typeof (Vector2));
        case VertexElementFormat.Vector3:
          return Marshal.SizeOf(typeof (Vector3));
        case VertexElementFormat.Vector4:
          return Marshal.SizeOf(typeof (Vector4));
        case VertexElementFormat.Color:
          return 4;
        case VertexElementFormat.Byte4:
          return 4;
        case VertexElementFormat.Short2:
          return 4;
        case VertexElementFormat.Short4:
          return 8;
        case VertexElementFormat.NormalizedShort2:
          return 4;
        case VertexElementFormat.NormalizedShort4:
          return 8;
        default:
          return 0;
      }
    }
  }
}
