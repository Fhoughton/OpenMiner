// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.FileReceiveInfo
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;
using System.IO;

namespace StudioForge.TotalMiner.Net
{
  internal class FileReceiveInfo
  {
    public SessionType Type;
    public NetworkGamer Sender;
    public int DirNumber;
    public string Path;
    public string Filename;
    public Stream Stream;
    public FileShareReceiveProgress Progress;
    public Action<FileShareReceiveProgress> Callback;
  }
}
