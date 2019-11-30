// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.FileShareInfo
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using StudioForge.Engine.Net;
using System;

namespace StudioForge.TotalMiner.Net
{
  internal class FileShareInfo
  {
    public SessionType Type;
    public int DirNum;
    public string ShareName;
    public string Filename;
    public NetworkGamer Recipient;
    public FileShareSendProgress Progress;
    public Action<NetworkGamer, FileShareSendProgress> Callback;
    public FileShareInfoInternals Internals;
  }
}
