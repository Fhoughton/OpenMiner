// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.FileShareReceiveProgress
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Net
{
  internal class FileShareReceiveProgress
  {
    public SessionType ShareType;
    public FileShareStatus Status;
    public bool FirstPacket;
    public string ShareName;
    public string ErrorMessage;
    public int DirNumber;
    public int TotalBytesToShare;
    public int BytesShared;
  }
}
