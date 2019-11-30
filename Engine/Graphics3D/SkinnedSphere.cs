// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Graphics3D.SkinnedSphere
// Assembly: StudioForge.Engine.Graphics3D_Pipeline, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E459E66A-A239-4A08-BD37-25C4FD372E6D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Graphics3D_Pipeline.dll

using Microsoft.Xna.Framework.Content;

namespace StudioForge.Engine.Graphics3D
{
  public class SkinnedSphere
  {
    public string BoneName;
    public float Radius;
    [ContentSerializer(Optional = true)]
    public float Offset;
  }
}
