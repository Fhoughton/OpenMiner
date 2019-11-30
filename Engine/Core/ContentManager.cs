// Decompiled with JetBrains decompiler
// Type: StudioForge.Engine.Core.ContentManager
// Assembly: StudioForge.Engine.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FEA662EE-E9AD-40D5-B37E-9129B8970A33
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.Engine.Core.dll

using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace StudioForge.Engine.Core
{
  public class ContentManager : Microsoft.Xna.Framework.Content.ContentManager, IContentManager, IDisposable, IUnmanagedBuffer
  {
    private Dictionary<string, int> assetSize = new Dictionary<string, int>();
    private long bufferSize;

    public long BufferSize
    {
      get
      {
        return this.bufferSize;
      }
    }

    public ContentManager(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
      : base(serviceProvider, rootDirectory)
    {
    }

    public override T Load<T>(string assetName)
    {
      T obj = base.Load<T>(assetName);
      Texture2D t = (object) obj as Texture2D;
      if (t != null)
        this.CalcTextureSize(assetName, t);
      return obj;
    }

    protected override Stream OpenStream(string assetName)
    {
      Stream stream = base.OpenStream(assetName);
      if (!this.assetSize.ContainsKey(assetName))
      {
        this.assetSize.Add(assetName, (int) stream.Length);
        this.bufferSize += stream.Length;
      }
      return stream;
    }

    private void CalcTextureSize(string assetName, Texture2D t)
    {
      int num1 = 0;
      if (this.assetSize.TryGetValue(assetName, out num1))
        this.bufferSize -= (long) num1;
      else
        this.assetSize.Add(assetName, 0);
      if (t.Format == SurfaceFormat.Color)
        num1 = t.Width * t.Height * 4;
      int num2 = num1 * 2;
      this.assetSize[assetName] = num2;
      this.bufferSize += (long) num2;
    }

    public override void Unload()
    {
      base.Unload();
      this.assetSize.Clear();
      this.bufferSize = 0L;
    }

    [SpecialName]
    string get_RootDirectory()
    {
      return this.RootDirectory;
    }

    [SpecialName]
    void set_RootDirectory([In] string obj0)
    {
      this.RootDirectory = obj0;
    }

    [SpecialName]
    IServiceProvider get_ServiceProvider()
    {
      return this.ServiceProvider;
    }
  }
}
