// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Net.NetworkManager
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Integration;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;
using StudioForge.TotalMiner.Graphics;
using StudioForge.TotalMiner.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace StudioForge.TotalMiner.Net
{
  internal class NetworkManager : GameObjectBase
  {
    public object BufferSemaphore = new object();
    private List<NetworkManager.BufferedBlockChange> blockChangesToSend = new List<NetworkManager.BufferedBlockChange>();
    private List<NetworkManager.PickupRequest> pickupRequests = new List<NetworkManager.PickupRequest>();
    private List<long> chunkRequestsToSend = new List<long>();
    private List<long> readChunkList = new List<long>();
    private Queue<NetworkManager.ChunkRequestsSent> chunksRequestedNotReceived = new Queue<NetworkManager.ChunkRequestsSent>();
    private Dictionary<long, MapChunk> sendChunkList = new Dictionary<long, MapChunk>();
    private Dictionary<long, MapChunk> tempChunkHashList = new Dictionary<long, MapChunk>();
    private Queue<NetworkManager.BufferedChangeBase> bufferedChanges = new Queue<NetworkManager.BufferedChangeBase>();
    private List<NetworkManager.GameDataSendReceive> gameDataToSend = new List<NetworkManager.GameDataSendReceive>();
    private List<NetworkManager.MachineData> tempRemoteMachineList = new List<NetworkManager.MachineData>();
    private List<NetworkGamer> tempAllGamerList = new List<NetworkGamer>();
    private List<NetworkGamer> tempAllEnabledGamerList = new List<NetworkGamer>();
    private List<NetworkGamer> tempLocalGamerList = new List<NetworkGamer>();
    private List<NetworkGamer> tempRemoteGamerList = new List<NetworkGamer>();
    private List<NetworkGamer> tempRemoteEnabledGamerList = new List<NetworkGamer>();
    private List<StudioForge.TotalMiner.Player> tempAllPlayerList = new List<StudioForge.TotalMiner.Player>();
    private List<StudioForge.TotalMiner.Player> tempLocalPlayerList = new List<StudioForge.TotalMiner.Player>(4);
    private List<StudioForge.TotalMiner.Player> tempLocalEnabledPlayerList = new List<StudioForge.TotalMiner.Player>(4);
    private object gamerListSemaphore = new object();
    private List<NpcBase> mobsToSend = new List<NpcBase>(10);
    private List<short> validMobList = new List<short>(10);
    private Dictionary<string, NetworkGamer> componentAsTempRequestConfirmations = new Dictionary<string, NetworkGamer>();
    private PacketReader packetReader = new PacketReader();
    private PacketWriter packetWriterNone = new PacketWriter();
    private PacketWriter packetWriterInOrder = new PacketWriter();
    private PacketWriter packetWriterReliable = new PacketWriter();
    private PacketWriter packetWriterReliableInOrder = new PacketWriter();
    private List<FileShareInfo> currentFileShares = new List<FileShareInfo>();
    private int fileShareBufferLen = 50000;
    private List<MapChunk> neighbours = new List<MapChunk>(26);
    private object chunkRequestLock = new object();
    private List<int> blockIndexes = new List<int>(100);
    private List<DataBlock> dataBlockTempList = new List<DataBlock>(100);
    private List<NetworkManager.DataBlockChange> dataBlockChangeList = new List<NetworkManager.DataBlockChange>(100);
    private Stack<PacketType> packetStack = new Stack<PacketType>();
    private List<StudioForge.TotalMiner.Player> playersToSend = new List<StudioForge.TotalMiner.Player>(4);
    private object updateLock = new object();
    private List<short> tempInvChangedSlotIDList = new List<short>();
    private List<InventoryItem> tempInvChangedItemList = new List<InventoryItem>();
    private List<string> scriptCommandChangeList = new List<string>(5);
    private const int maxMobInstancesToSend = 10;
    private const int ticksPerMobSend = 10;
    private const int ticksPerMobFullValidate = 60;
    public static NetworkManager Instance;
    public PlayerIndex? ControllingPlayer;
    public int TotalBytesSentLastFrame;
    public bool HostIsReady;
    private GameInstance gameInstance;
    private int maxGamers;
    private int maxLocalGamers;
    private string sessionDesc;
    private INetworkSession session;
    private Thread createSessionThread;
    private Action sessioEndedCallback;
    private Action<bool> sessionCreatedCallback;
    private NetworkGamer localHost;
    private SessionType sessionType;
    private SessionProperties sessionProperties;
    private NetworkSessionType networkSessionType;
    private Action<NetworkGamer, PacketReader, PacketType> onServerPacketReceived;
    private NetworkManager.GameDataSendReceive gameDataToReceive;
    private List<NetworkGamer> workAllGamerList;
    private List<NetworkGamer> workAllEnabledGamerList;
    private List<NetworkGamer> workLocalGamerList;
    private List<NetworkGamer> workRemoteGamerList;
    private List<NetworkGamer> workRemoteEnabledGamerList;
    private List<StudioForge.TotalMiner.Player> workAllPlayerList;
    private List<StudioForge.TotalMiner.Player> workLocalPlayerList;
    private List<StudioForge.TotalMiner.Player> workLocalEnabledPlayerList;
    private NetworkGamer workLocalHost;
    private Dictionary<int, NetworkManager.MachineData> machineData;
    private int allGamerCount;
    private int allGamerEnabledCount;
    private int localGamerCount;
    private int localPlayerCount;
    private int remoteGamerCount;
    private bool sessionEnded;
    private int sessionEndTimer;
    private int onlineSendTimer;
    private int onlineMobSendTimer;
    private int mobFullValidateTimer;
    private float timeSinceLastGameStateSend;
    private int totalBytesSentLastFrame;
    private Action<FileShareReceiveProgress> fileShareReceiveCallback;
    private FileReceiveInfo fileReceiveInfo;
    private byte[] fileShareBuffer;
    private NetworkManager.ReadInPacket[] readMethods;
    private bool resendAllGameInstances;
    private bool resendAllMobInstances;
    private bool isHostOverrideForTest;
    private float lastSendSunRotation;
    private int lastSendSeed;
    private Action<string> endOnlineJoinCallback;
    private IAsyncResult joinResult;
    private ITMNetworkManager modNetMgr;

    public event EventHandler<GamerEventArgs> GamerJoined
    {
      add
      {
        if (!this.IsSessionOpen)
          return;
        this.session.GamerJoined += value;
      }
      remove
      {
        if (this.session == null)
          return;
        this.session.GamerJoined -= value;
      }
    }

    public event EventHandler<GamerEventArgs> GamerLeft
    {
      add
      {
        if (!this.IsSessionOpen)
          return;
        this.session.GamerLeft += value;
      }
      remove
      {
        if (this.session == null)
          return;
        this.session.GamerLeft -= value;
      }
    }

    public event EventHandler<GameEventArgs> GameStarted
    {
      add
      {
        if (!this.IsSessionOpen)
          return;
        this.session.GameStarted += value;
      }
      remove
      {
        if (this.session == null)
          return;
        this.session.GameStarted -= value;
      }
    }

    public event EventHandler<GameEventArgs> GameEnded
    {
      add
      {
        if (!this.IsSessionOpen)
          return;
        this.session.GameEnded += value;
      }
      remove
      {
        if (this.session == null)
          return;
        this.session.GameEnded -= value;
      }
    }

    public event EventHandler<EventArgs> GamePropertiesReceived;

    public event EventHandler<EventArgs> GameDataReceived;

    public event EventHandler<EventArgs> HostLoadedConfirmed;

    public event IntEventHandler PlayerStatsReceived;

    public event BlockEventHandler BlockTextureChangedReceived;

    private void RaiseGamePropertiesReceived(object sender, EventArgs e)
    {
      if (this.GamePropertiesReceived == null)
        return;
      this.GamePropertiesReceived(sender, e);
    }

    private void RaiseGameDataReceived(object sender, EventArgs e)
    {
      if (this.GameDataReceived == null)
        return;
      this.GameDataReceived(sender, e);
    }

    private void RaiseHostLoadedConfirmed(object sender, EventArgs e)
    {
      if (this.HostLoadedConfirmed == null)
        return;
      this.HostLoadedConfirmed(sender, e);
    }

    private void RaisePlayerStatsReceived(object sender, int index)
    {
      if (this.PlayerStatsReceived == null)
        return;
      this.PlayerStatsReceived(sender, new IntEventArgs(index));
    }

    private void RaiseBlockTextureChangedReceived(object sender, GlobalPoint3D p, Block blockID)
    {
      if (this.BlockTextureChangedReceived == null)
        return;
      this.BlockTextureChangedReceived(sender, new BlockEventArgs(p, blockID));
    }

    public int ChunksRequestedNotReceivedCount
    {
      get
      {
        return this.chunksRequestedNotReceived.Count;
      }
    }

    private NetworkGamer FindGamerById(GamerID gamerID)
    {
      if (this.session == null)
        return (NetworkGamer) null;
      return this.session.FindGamerById(gamerID);
    }

    public GameInstance GameInstance
    {
      get
      {
        return this.gameInstance;
      }
      set
      {
        this.gameInstance = value;
      }
    }

    public INetworkSession Session
    {
      get
      {
        return this.session;
      }
    }

    public List<NetworkGamer> AllGamers
    {
      get
      {
        return this.tempAllGamerList;
      }
    }

    public List<NetworkGamer> AllEnabledGamers
    {
      get
      {
        return this.tempAllEnabledGamerList;
      }
    }

    public List<NetworkGamer> LocalGamers
    {
      get
      {
        return this.tempLocalGamerList;
      }
    }

    public List<StudioForge.TotalMiner.Player> LocalEnabledPlayers
    {
      get
      {
        return this.tempLocalEnabledPlayerList;
      }
    }

    public List<NetworkGamer> RemoteGamers
    {
      get
      {
        return this.tempRemoteGamerList;
      }
    }

    public List<NetworkGamer> RemoteEnabledGamers
    {
      get
      {
        return this.tempRemoteEnabledGamerList;
      }
    }

    public bool IsHost
    {
      get
      {
        if (this.session == null)
          return this.isHostOverrideForTest;
        return this.session.IsHost;
      }
    }

    public bool IsRemote
    {
      get
      {
        if (this.session == null)
          return !this.isHostOverrideForTest;
        return !this.session.IsHost;
      }
    }

    public bool IsSessionOpen
    {
      get
      {
        if (this.session != null)
          return !this.session.IsDisposed;
        return false;
      }
    }

    public SessionType SessionType
    {
      get
      {
        return this.sessionType;
      }
    }

    public bool IsSessionOpenAndNotLocal
    {
      get
      {
        if (this.session != null && this.session.SessionState != NetworkSessionState.Ended && (!this.session.IsDisposed && this.session.SessionType != NetworkSessionType.Local))
          return this.session.RemoteGamers.Count > 0;
        return false;
      }
    }

    public bool IsSessionOpenAndNotLocalAndRemotes
    {
      get
      {
        if (this.IsSessionOpenAndNotLocal)
          return this.remoteGamerCount > 0;
        return false;
      }
    }

    public NetworkGamer LocalHost
    {
      get
      {
        if (this.session == null)
          return (NetworkGamer) null;
        if (this.localHost != null)
          return this.localHost;
        if (this.session.LocalGamers.Count < 1)
          return (NetworkGamer) null;
        return this.localHost = this.session.LocalGamers[0];
      }
    }

    public bool HasJoinedSession(NetworkGamer gamer)
    {
      if (this.IsSessionOpen)
      {
        foreach (NetworkGamer tempAllGamer in this.tempAllGamerList)
        {
          if (tempAllGamer == gamer)
            return true;
        }
      }
      return false;
    }

    public bool HasJoinedSession(string gamertag)
    {
      if (this.IsSessionOpen)
      {
        foreach (Gamer tempAllGamer in this.tempAllGamerList)
        {
          if (tempAllGamer.Gamertag == gamertag)
            return true;
        }
      }
      return false;
    }

    public static string GetNetworkTypeDesc(NetworkSessionType type)
    {
      switch (type)
      {
        case NetworkSessionType.Local:
          return "Single Player";
        case NetworkSessionType.SystemLink:
          return "LAN";
        case NetworkSessionType.PlayerMatch:
          return "Online";
        default:
          return "Unknown";
      }
    }

    public int AllGamerCount
    {
      get
      {
        return this.allGamerCount;
      }
    }

    public int AllGamerEnabledCount
    {
      get
      {
        return this.allGamerEnabledCount;
      }
    }

    public int LocalGamerCount
    {
      get
      {
        return this.localGamerCount;
      }
    }

    public int LocalPlayerCount
    {
      get
      {
        return this.localPlayerCount;
      }
    }

    public int RemoteGamerCount
    {
      get
      {
        return this.remoteGamerCount;
      }
    }

    public int MapID
    {
      get
      {
        return this.IsSessionOpen ? 12312 : -1;
      }
    }

    public bool IsShareSession
    {
      get
      {
        if (this.sessionType != SessionType.ShareMap)
          return this.sessionType == SessionType.ShareComPack;
        return true;
      }
    }

    private long GetXZHash(MapChunk chunk)
    {
      MapRegion region = chunk.Region;
      Map map = region.Map;
      long x1 = (long) region.Offset.X;
      long z1 = (long) region.Offset.Z;
      long x2 = (long) chunk.Offset.X;
      long z2 = (long) chunk.Offset.Z;
      long num1 = x1 - (long) map.MapBound.Min.X;
      long num2 = z1 - (long) map.MapBound.Min.Z;
      return (num1 / (long) map.RegionSize.X << 48) + (num2 / (long) map.RegionSize.Z << 32) + (x2 / (long) map.ChunkSize.X << 16) + z2 / (long) map.ChunkSize.Z;
    }

    public NetworkManager.MachineData GetMachineData(short gamerID)
    {
      if (gamerID >= (short) 0)
      {
        lock (this.machineData)
        {
          foreach (NetworkManager.MachineData machineData in this.machineData.Values)
          {
            foreach (Gamer gamer in machineData.Machine.Gamers)
            {
              if (gamer.ID == gamerID)
                return machineData;
            }
          }
        }
      }
      return (NetworkManager.MachineData) null;
    }

    public NetworkManager.MachineData GetMachineData(int machineHash)
    {
      NetworkManager.MachineData machineData = (NetworkManager.MachineData) null;
      lock (this.machineData)
        this.machineData.TryGetValue(machineHash, out machineData);
      return machineData;
    }

    public NetworkManager()
    {
      this.InitReadMethods();
    }

    public void InitializeForTest(bool isHost)
    {
      this.isHostOverrideForTest = isHost;
    }

    private void InitReadMethods()
    {
      this.readMethods = new NetworkManager.ReadInPacket[111];
      this.readMethods[2] = new NetworkManager.ReadInPacket(this.ReadModPacket);
      this.readMethods[3] = new NetworkManager.ReadInPacket(this.ReadServerUncaughtExceptionReport);
      this.readMethods[4] = new NetworkManager.ReadInPacket(this.ReadServerData);
      this.readMethods[5] = new NetworkManager.ReadInPacket(this.ReadServerHighscoreData);
      this.readMethods[6] = new NetworkManager.ReadInPacket(this.ReadServerHighscoreDataRequest);
      this.readMethods[7] = new NetworkManager.ReadInPacket(this.ReadServerCaughtExceptionReport);
      this.readMethods[8] = new NetworkManager.ReadInPacket(this.ReadServerConfirmReceiptRequest);
      this.readMethods[9] = new NetworkManager.ReadInPacket(this.ReadServerReceiptConfirmed);
      this.readMethods[10] = new NetworkManager.ReadInPacket(this.ReadServerDataRequest);
      this.readMethods[18] = new NetworkManager.ReadInPacket(this.ReadGameInstanceData);
      this.readMethods[20] = new NetworkManager.ReadInPacket(this.ReadPlayerSpatial);
      this.readMethods[78] = new NetworkManager.ReadInPacket(this.ReadMobInstanceData);
      this.readMethods[63] = new NetworkManager.ReadInPacket(this.ReadProjectile);
      this.readMethods[70] = new NetworkManager.ReadInPacket(this.ReadDoorChangeConfirm);
      this.readMethods[71] = new NetworkManager.ReadInPacket(this.ReadTrapDoorChangeConfirm);
      this.readMethods[68] = new NetworkManager.ReadInPacket(this.ReadDamage);
      this.readMethods[69] = new NetworkManager.ReadInPacket(this.ReadKillConfirm);
      this.readMethods[89] = new NetworkManager.ReadInPacket(this.ReadWifiTransmitterFrequency);
      this.readMethods[40] = new NetworkManager.ReadInPacket(this.ReadPickupCreate);
      this.readMethods[41] = new NetworkManager.ReadInPacket(this.ReadPickupRequest);
      this.readMethods[42] = new NetworkManager.ReadInPacket(this.ReadPickupConfirm);
      this.readMethods[81] = new NetworkManager.ReadInPacket(this.ReadSlider);
      this.readMethods[72] = new NetworkManager.ReadInPacket(this.ReadPowerDeliver);
      this.readMethods[31] = new NetworkManager.ReadInPacket(this.ReadChunkRequests);
      this.readMethods[24] = new NetworkManager.ReadInPacket(this.ReadBlockChanges);
      this.readMethods[29] = new NetworkManager.ReadInPacket(this.ReadDataBlockChange);
      this.readMethods[30] = new NetworkManager.ReadInPacket(this.ReadDataBlockRemove);
      this.readMethods[35] = new NetworkManager.ReadInPacket(this.ReadChunkData);
      this.readMethods[32] = new NetworkManager.ReadInPacket(this.ReadUneditedChunks);
      this.readMethods[27] = new NetworkManager.ReadInPacket(this.ReadDataBlockInfoRequest);
      this.readMethods[28] = new NetworkManager.ReadInPacket(this.ReadDataBlockInfo);
      this.readMethods[64] = new NetworkManager.ReadInPacket(this.ReadHeal);
      this.readMethods[53] = new NetworkManager.ReadInPacket(this.ReadInventory);
      this.readMethods[54] = new NetworkManager.ReadInPacket(this.ReadInventoryChanged);
      this.readMethods[52] = new NetworkManager.ReadInPacket(this.ReadBlast);
      this.readMethods[25] = new NetworkManager.ReadInPacket(this.ReadBlockTextureChange);
      this.readMethods[26] = new NetworkManager.ReadInPacket(this.ReadBlockTextureRemoved);
      this.readMethods[65] = new NetworkManager.ReadInPacket(this.ReadActionLog);
      this.readMethods[66] = new NetworkManager.ReadInPacket(this.ReadHistoryItem);
      this.readMethods[67] = new NetworkManager.ReadInPacket(this.ReadHistoryTable);
      this.readMethods[37] = new NetworkManager.ReadInPacket(this.ReadOpenBlockRequest);
      this.readMethods[38] = new NetworkManager.ReadInPacket(this.ReadOpenBlockConfirm);
      this.readMethods[39] = new NetworkManager.ReadInPacket(this.ReadCloseBlock);
      this.readMethods[97] = new NetworkManager.ReadInPacket(this.ReadScriptExecute);
      this.readMethods[80] = new NetworkManager.ReadInPacket(this.ReadMobSpawnDataRequest);
      this.readMethods[79] = new NetworkManager.ReadInPacket(this.ReadMobSpawnData);
      this.readMethods[90] = new NetworkManager.ReadInPacket(this.ReadBookIDRequest);
      this.readMethods[91] = new NetworkManager.ReadInPacket(this.ReadBookIDConfirm);
      this.readMethods[75] = new NetworkManager.ReadInPacket(this.ReadFloodAbort);
      this.readMethods[51] = new NetworkManager.ReadInPacket(this.ReadCreativeCommand);
      this.readMethods[11] = new NetworkManager.ReadInPacket(this.ReadCommand);
      this.readMethods[84] = new NetworkManager.ReadInPacket(this.ReadWeather);
      this.readMethods[60] = new NetworkManager.ReadInPacket(this.ReadPlayerSettings);
      this.readMethods[58] = new NetworkManager.ReadInPacket(this.ReadPlayerSkills);
      this.readMethods[57] = new NetworkManager.ReadInPacket(this.ReadPlayerSkill);
      this.readMethods[55] = new NetworkManager.ReadInPacket(this.ReadPlayerStatistics);
      this.readMethods[56] = new NetworkManager.ReadInPacket(this.ReadPlayerStatisticsRequest);
      this.readMethods[85] = new NetworkManager.ReadInPacket(this.ReadPriceChange);
      this.readMethods[86] = new NetworkManager.ReadInPacket(this.ReadPriceList);
      this.readMethods[14] = new NetworkManager.ReadInPacket(this.ReadGameProperties);
      this.readMethods[15] = new NetworkManager.ReadInPacket(this.ReadGamePropertiesNonVital);
      this.readMethods[16] = new NetworkManager.ReadInPacket(this.ReadGamePropertiesRequest);
      this.readMethods[17] = new NetworkManager.ReadInPacket(this.ReadGlobalItemData);
      this.readMethods[83] = new NetworkManager.ReadInPacket(this.ReadCaveInStart);
      this.readMethods[74] = new NetworkManager.ReadInPacket(this.ReadSignText);
      this.readMethods[50] = new NetworkManager.ReadInPacket(this.ReadPermissions);
      this.readMethods[43] = new NetworkManager.ReadInPacket(this.ReadKickGamer);
      this.readMethods[44] = new NetworkManager.ReadInPacket(this.ReadRatingVote);
      this.readMethods[45] = new NetworkManager.ReadInPacket(this.ReadWorldFavorited);
      this.readMethods[76] = new NetworkManager.ReadInPacket(this.ReadNotification);
      this.readMethods[21] = new NetworkManager.ReadInPacket(this.ReadGameState);
      this.readMethods[109] = new NetworkManager.ReadInPacket(this.ReadTextMessage);
      this.readMethods[82] = new NetworkManager.ReadInPacket(this.ReadZone);
      this.readMethods[95] = new NetworkManager.ReadInPacket(this.ReadTopMapMarkerUpdate);
      this.readMethods[96] = new NetworkManager.ReadInPacket(this.ReadTopMapMarkerRemove);
      this.readMethods[92] = new NetworkManager.ReadInPacket(this.ReadBookUpdate);
      this.readMethods[94] = new NetworkManager.ReadInPacket(this.ReadSleepState);
      this.readMethods[61] = new NetworkManager.ReadInPacket(this.ReadPlayerSettingsRequest);
      this.readMethods[62] = new NetworkManager.ReadInPacket(this.ReadPlayerLoaded);
      this.readMethods[46] = new NetworkManager.ReadInPacket(this.ReadLockedInfoRequest);
      this.readMethods[47] = new NetworkManager.ReadInPacket(this.ReadLockedInfo);
      this.readMethods[93] = new NetworkManager.ReadInPacket(this.ReadItemUnlocked);
      this.readMethods[48] = new NetworkManager.ReadInPacket(this.ReadCustomDataRequest);
      this.readMethods[49] = new NetworkManager.ReadInPacket(this.ReadCustomData);
      this.readMethods[12] = new NetworkManager.ReadInPacket(this.ReadGameData);
      this.readMethods[98] = new NetworkManager.ReadInPacket(this.ReadScriptEdited);
      this.readMethods[99] = new NetworkManager.ReadInPacket(this.ReadScriptDeleted);
      this.readMethods[100] = new NetworkManager.ReadInPacket(this.ReadScriptCancelled);
      this.readMethods[101] = new NetworkManager.ReadInPacket(this.ReadScriptInputResult);
      this.readMethods[102] = new NetworkManager.ReadInPacket(this.ReadScriptIntersectResult);
      this.readMethods[103] = new NetworkManager.ReadInPacket(this.ReadAdventureScript);
      this.readMethods[104] = new NetworkManager.ReadInPacket(this.ReadEventScript);
      this.readMethods[19] = new NetworkManager.ReadInPacket(this.ReadGameInstanceDataRequest);
      this.readMethods[105] = new NetworkManager.ReadInPacket(this.ReadComponentAsTempRequest);
      this.readMethods[106] = new NetworkManager.ReadInPacket(this.ReadComponentAsTempRequestConfirm);
      this.readMethods[107] = new NetworkManager.ReadInPacket(this.ReadComponentAsTempRequestData);
      this.readMethods[108] = new NetworkManager.ReadInPacket(this.ReadComponentAsTemp);
      this.readMethods[13] = new NetworkManager.ReadInPacket(this.ReadGameDataRequest);
      this.readMethods[22] = new NetworkManager.ReadInPacket(this.ReadFileShare);
      this.readMethods[23] = new NetworkManager.ReadInPacket(this.ReadFileShareAck);
      this.readMethods[87] = new NetworkManager.ReadInPacket(this.ReadPhotoThumbnailRequest);
      this.readMethods[88] = new NetworkManager.ReadInPacket(this.ReadPhotoThumbnail);
      this.readMethods[110] = new NetworkManager.ReadInPacket(this.ReadArcadeState);
    }

    protected override void UpdateCore(UpdateState state)
    {
      this.TotalBytesSentLastFrame = this.totalBytesSentLastFrame;
      this.totalBytesSentLastFrame = 0;
      this.ValidateSession();
      if (this.IsSessionOpen)
      {
        try
        {
          lock (this.updateLock)
          {
            this.BuildTempLists();
            this.SendData();
            this.session.Update();
            this.ReceiveData();
          }
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(107, ex);
        }
      }
      if (!this.sessionEnded || --this.sessionEndTimer > 0)
        return;
      this.EndSessionCore();
    }

    private void SendAllPackets()
    {
      this.SendPacket(this.packetWriterNone, SendDataOptions.None, (NetworkGamer) null);
      this.SendPacket(this.packetWriterInOrder, SendDataOptions.InOrder, (NetworkGamer) null);
      this.SendPacket(this.packetWriterReliable, SendDataOptions.Reliable, (NetworkGamer) null);
      this.SendPacket(this.packetWriterReliableInOrder, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      foreach (NetworkManager.MachineData tempRemoteMachine in this.tempRemoteMachineList)
      {
        if (tempRemoteMachine.Machine.Gamers.Count > 0)
        {
          NetworkGamer gamer = tempRemoteMachine.Machine.Gamers[0];
          if (gamer != null)
          {
            this.SendPacket(tempRemoteMachine.PacketWriterNone, SendDataOptions.None, gamer);
            this.SendPacket(tempRemoteMachine.PacketWriterInOrder, SendDataOptions.InOrder, gamer);
            this.SendPacket(tempRemoteMachine.PacketWriterReliable, SendDataOptions.Reliable, gamer);
            this.SendPacket(tempRemoteMachine.PacketWriterReliableInOrder, SendDataOptions.ReliableInOrder, gamer);
          }
        }
      }
    }

    private void SendPacket(
      PacketWriter packetWriter,
      SendDataOptions options,
      NetworkGamer recipient)
    {
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        if (packetWriter.Length <= 0)
          return;
        packetWriter.Write((byte) 1);
        this.totalBytesSentLastFrame += packetWriter.Length + 52;
        if (this.LocalHost != null && this.modNetMgr != null)
          this.modNetMgr.SendData(packetWriter, options, recipient);
        packetWriter.BaseStream.SetLength(0L);
      }
    }

    private int GetCurrentPacketSize()
    {
      int num = this.packetWriterNone.Length + this.packetWriterInOrder.Length + this.packetWriterReliable.Length + this.packetWriterReliableInOrder.Length;
      foreach (NetworkManager.MachineData tempRemoteMachine in this.tempRemoteMachineList)
      {
        num += tempRemoteMachine.PacketWriterNone.Length;
        num += tempRemoteMachine.PacketWriterInOrder.Length;
        num += tempRemoteMachine.PacketWriterReliable.Length;
        num += tempRemoteMachine.PacketWriterReliableInOrder.Length;
      }
      return num;
    }

    private void BuildTempLists()
    {
      if (this.session == null || this.machineData == null)
        return;
      lock (this.machineData)
      {
        this.tempRemoteMachineList.Clear();
        foreach (NetworkManager.MachineData machineData in this.machineData.Values)
        {
          if (!machineData.IsLocalMachine(this.session))
            this.tempRemoteMachineList.Add(machineData);
        }
      }
    }

    public void BuildGamerList()
    {
      int num = 3;
label_1:
      lock (this.gamerListSemaphore)
      {
        int capacity = 24;
        this.workAllGamerList = new List<NetworkGamer>(capacity);
        this.workAllEnabledGamerList = new List<NetworkGamer>(capacity);
        this.workLocalGamerList = new List<NetworkGamer>(4);
        this.workRemoteGamerList = new List<NetworkGamer>(capacity);
        this.workRemoteEnabledGamerList = new List<NetworkGamer>(capacity);
        this.workAllPlayerList = new List<StudioForge.TotalMiner.Player>(capacity);
        this.workLocalPlayerList = new List<StudioForge.TotalMiner.Player>(4);
        this.workLocalEnabledPlayerList = new List<StudioForge.TotalMiner.Player>(4);
        this.workLocalHost = (NetworkGamer) null;
        try
        {
          for (int index = 0; index < this.session.AllGamers.Count; ++index)
            this.AddGamer(this.session.AllGamers[index]);
        }
        catch (Exception ex)
        {
          if (--num > 0)
            goto label_1;
        }
        this.tempAllGamerList = this.workAllGamerList;
        this.allGamerCount = this.tempAllGamerList.Count;
        this.tempAllEnabledGamerList = this.workAllEnabledGamerList;
        this.allGamerEnabledCount = this.tempAllEnabledGamerList.Count;
        this.tempAllPlayerList = this.workAllPlayerList;
        this.tempLocalGamerList = this.workLocalGamerList;
        this.localGamerCount = this.tempLocalGamerList.Count;
        this.tempLocalPlayerList = this.workLocalPlayerList;
        this.tempLocalEnabledPlayerList = this.workLocalEnabledPlayerList;
        this.localPlayerCount = this.tempLocalPlayerList.Count;
        this.tempRemoteEnabledGamerList = this.workRemoteEnabledGamerList;
        this.tempRemoteGamerList = this.workRemoteGamerList;
        this.remoteGamerCount = this.tempRemoteGamerList.Count;
        this.localHost = this.workLocalHost;
      }
    }

    private void AddGamer(NetworkGamer gamer)
    {
      this.workAllGamerList.Add(gamer);
      if (gamer.IsLocal)
      {
        NetworkGamer networkGamer = gamer;
        this.workLocalGamerList.Add(networkGamer);
        if (networkGamer.IsHost || this.workLocalHost == null)
          this.workLocalHost = networkGamer;
      }
      else
        this.workRemoteGamerList.Add(gamer);
      StudioForge.TotalMiner.Player tag = gamer.Tag as StudioForge.TotalMiner.Player;
      if (tag == null)
        return;
      if (gamer.IsLocal)
        this.workLocalPlayerList.Add(tag);
      if (!tag.IsEnabledField)
        return;
      this.workAllEnabledGamerList.Add(gamer);
      if (gamer.IsLocal)
        this.workLocalEnabledPlayerList.Add(tag);
      else
        this.workRemoteEnabledGamerList.Add(gamer);
    }

    private void SendData()
    {
      if (this.session.SessionType == NetworkSessionType.Local)
        return;
      if (this.gameInstance != null)
      {
        bool isHost = this.IsHost;
        this.SendGameData();
        if (isHost)
          this.SendGameState(false);
        this.SendGameInstanceData();
        if (isHost)
          this.SendMobInstanceData();
        this.SendBlockChanges();
        this.SendDataBlockChanges();
        this.SendPickupRequests();
        if (!isHost)
          this.SendChunkRequests();
        this.SendChunksToRemotes();
      }
      this.SendFileShare();
      this.SendAllPackets();
    }

    private void ReceiveData()
    {
      if (this.modNetMgr == null)
        return;
      this.packetStack.Clear();
      NetworkGamer sender;
      while (this.modNetMgr.ReadData(this.packetReader, out sender))
      {
        if (this.packetReader.Length > 1)
        {
          while (this.ReceiveDataCore(sender, (NetworkGamer) null))
            ;
        }
      }
    }

    private bool ReceiveDataCore(NetworkGamer sender, NetworkGamer receiver)
    {
      PacketType type = (PacketType) this.packetReader.ReadByte();
      this.FlagPacketRecv(type);
      if (type != PacketType.EndOfPacket)
      {
        PacketType packetType = (PacketType) this.packetReader.ReadByte();
        if (packetType == type)
        {
          if (this.readMethods[(int) type] != null)
          {
            try
            {
              this.readMethods[(int) type](sender);
              goto label_6;
            }
            catch (Exception ex)
            {
              this.ReportReceiveException(ex);
              return false;
            }
          }
        }
        this.ReportReceiveException((Exception) new CoreException(string.Format("Bad Packet:{0} Confirm:{1}", (object) type, (object) packetType)));
        return false;
      }
label_6:
      if (type != PacketType.EndOfPacket)
        return this.packetReader.Position < this.packetReader.Length;
      return false;
    }

    private void ReportReceiveException(Exception e)
    {
      StringBuilder stringBuilder = new StringBuilder("Packet stack: ");
      if (this.packetStack.Count > 0)
      {
        while (this.packetStack.Count > 0)
        {
          stringBuilder.Append((object) this.packetStack.Pop());
          stringBuilder.Append(", ");
        }
      }
      else
        stringBuilder.Append("Empty, ");
      stringBuilder.Append(e.Message);
      Exception e1 = new Exception(stringBuilder.ToString(), e);
      Services.ExceptionReporter.ReportExceptionCaught(14, e1);
    }

    private void FlagPacketRecv(PacketType type)
    {
      this.packetStack.Push(type);
    }

    private void ReadModPacket(NetworkGamer sender)
    {
      if (this.modNetMgr != null && this.modNetMgr.ParseCustomPacket(this.packetReader, sender))
        return;
      foreach (Mod activeMod in ModManager.ActiveMods)
      {
        if (activeMod.NetworkManager != null && activeMod.NetworkManager != this.modNetMgr && activeMod.NetworkManager.ParseCustomPacket(this.packetReader, sender))
          break;
      }
    }

    public void RequeueOldChunkRequests()
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      lock (this.chunksRequestedNotReceived)
      {
        if (this.chunksRequestedNotReceived.Count <= 0)
          return;
        for (NetworkManager.ChunkRequestsSent chunkRequestsSent = this.chunksRequestedNotReceived.Peek(); (double) (chunkRequestsSent.Time + 60) < Globals1.ElapsedWatch.Elapsed.TotalSeconds; chunkRequestsSent = this.chunksRequestedNotReceived.Peek())
        {
          this.chunksRequestedNotReceived.Dequeue();
          this.EnqueuChunkRequestCore(chunkRequestsSent.Hash);
        }
      }
    }

    public void EnqueueChunkRequest(MapChunkTM chunk)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      lock (this.chunksRequestedNotReceived)
      {
        lock (this.chunkRequestsToSend)
          this.EnqueuChunkRequestCore(chunk.GetGlobalHashCode());
      }
    }

    public void EnqueueChunkRequests(List<long> hashList)
    {
      if (hashList.Count <= 0 || this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      lock (this.chunksRequestedNotReceived)
      {
        lock (this.chunkRequestsToSend)
        {
          foreach (long hash in hashList)
            this.EnqueuChunkRequestCore(hash);
        }
      }
    }

    private void EnqueuChunkRequestCore(long hash)
    {
      this.chunkRequestsToSend.Add(hash);
      this.chunksRequestedNotReceived.Enqueue(new NetworkManager.ChunkRequestsSent()
      {
        Hash = hash,
        Time = (int) Globals1.ElapsedWatch.Elapsed.TotalSeconds
      });
    }

    private void SendChunkRequests()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      lock (this.chunkRequestsToSend)
      {
        if (this.chunkRequestsToSend.Count <= 0)
          return;
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
        if (packetWriter == null)
          return;
        lock (packetWriter)
        {
          packetWriter.Write((byte) 31);
          packetWriter.Write((byte) 31);
          this.WriteLongList(packetWriter, this.chunkRequestsToSend);
          this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
        }
        this.chunkRequestsToSend.Clear();
      }
    }

    private void ReadChunkRequests(NetworkGamer sender)
    {
      lock (this.readChunkList)
      {
        this.ReadLongList(this.readChunkList);
        if (this.readChunkList.Count <= 0 || !this.IsHost || (!this.IsSessionOpenAndNotLocal || this.gameInstance == null))
          return;
        lock (this.chunkRequestLock)
        {
          NetworkManager.MachineData machineData = this.GetMachineData(sender.Machine.GetHashCode());
          if (machineData == null)
            return;
          foreach (long readChunk in this.readChunkList)
            this.AddChunkReqeust(machineData, sender, this.gameInstance.Map.GetChunk(readChunk), readChunk);
        }
      }
    }

    private void AddChunkReqeust(
      NetworkManager.MachineData machine,
      NetworkGamer sender,
      MapChunk chunk,
      long hash)
    {
      List<long> longList = machine.ChunkOutsideMapBound;
      if (chunk != null)
      {
        bool isDecorated = chunk.IsDecorated;
        bool flag = chunk.IsChunkFlagSet(ChunkFlags.UserEdited | ChunkFlags.HasSpecialBlocks);
        longList = !isDecorated ? machine.HostNeedsToGenerateChunks : (flag ? machine.EditedChunkRequests : machine.UneditedChunkRequests);
      }
      if (longList.Contains(hash))
        return;
      longList.Add(hash);
    }

    public void ChunkIsDecorated(MapChunk chunk, ChunkFlags atomicFlagsToSet)
    {
      if (!this.IsHost || chunk == null)
        return;
      lock (this.chunkRequestLock)
      {
        chunk.SetChunkFlag(atomicFlagsToSet);
        if (this.tempRemoteMachineList.Count <= 0)
          return;
        long globalHashCode = chunk.GetGlobalHashCode();
        lock (this.machineData)
        {
          foreach (NetworkManager.MachineData tempRemoteMachine in this.tempRemoteMachineList)
          {
            if (tempRemoteMachine.HostChunkGenerationQueued.ContainsKey(globalHashCode))
            {
              this.AddChunkReqeust(tempRemoteMachine, (NetworkGamer) null, chunk, globalHashCode);
              tempRemoteMachine.HostChunkGenerationQueued.Remove(globalHashCode);
            }
          }
        }
      }
    }

    public void SendChunks(BoxInt bound)
    {
      if (!this.IsSessionOpenAndNotLocal || this.gameInstance == null)
        return;
      lock (this.updateLock)
      {
        lock (this.sendChunkList)
        {
          MapTM map = this.gameInstance.Map;
          bound.Min -= GlobalPoint3D.One;
          bound.Max += GlobalPoint3D.One;
          map.GetChunks(bound, this.sendChunkList);
          if (this.sendChunkList.Count <= 0)
            return;
          for (int index = 0; index < this.tempRemoteMachineList.Count; ++index)
          {
            NetworkManager.MachineData tempRemoteMachine = this.tempRemoteMachineList[index];
            if (!tempRemoteMachine.IsLocalMachine(this.session))
            {
              lock (this.chunkRequestLock)
              {
                foreach (KeyValuePair<long, MapChunk> sendChunk in this.sendChunkList)
                  this.AddChunkReqeust(tempRemoteMachine, tempRemoteMachine.Host, sendChunk.Value, sendChunk.Key);
              }
            }
          }
          this.sendChunkList.Clear();
        }
      }
    }

    private void SendChunksToRemotes()
    {
      if (!this.IsSessionOpenAndNotLocal || this.gameInstance == null)
        return;
      for (int index = 0; index < this.tempRemoteMachineList.Count; ++index)
      {
        NetworkManager.MachineData tempRemoteMachine = this.tempRemoteMachineList[index];
        this.SendChunks(tempRemoteMachine);
        this.SendUneditedChunks(tempRemoteMachine);
        this.QueueChunksForGeneration(tempRemoteMachine);
      }
    }

    private void SendChunks(NetworkManager.MachineData machine)
    {
      lock (this.chunkRequestLock)
        this.SendChunks(machine, machine.EditedChunkRequests);
    }

    private int SendChunks(NetworkManager.MachineData machine, List<long> chunks)
    {
      int num1 = 0;
      if (chunks.Count > 0 && !machine.IsLocalMachine(this.session))
      {
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, machine.Host);
        lock (packetWriter)
        {
          packetWriter.Write((byte) 35);
          packetWriter.Write((byte) 35);
          int num2 = 0;
          int position = packetWriter.Position;
          packetWriter.Write(num2);
          foreach (long chunk1 in chunks)
          {
            MapChunk chunk2 = this.gameInstance.Map.GetChunk(chunk1);
            this.WriteChunkData(packetWriter, chunk2);
            ++num2;
            if (packetWriter.Length > 5000)
              break;
          }
          packetWriter.Position = position;
          packetWriter.Write(num2);
          packetWriter.Position = packetWriter.Length;
          for (int index = num2 - 1; index >= 0; --index)
            chunks.RemoveAt(index);
          num1 = packetWriter.Length;
          this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, machine.Host);
        }
      }
      return num1;
    }

    private void WriteChunkData(PacketWriter packetWriter, MapChunk chunk)
    {
      if (chunk == null)
        return;
      packetWriter.Write(chunk.GetGlobalHashCode());
      packetWriter.Write((uint) chunk.GetChunkFlagMask(ChunkFlags.UserEdited | ChunkFlags.HasSpecialBlocks));
      this.WriteRleData(packetWriter, chunk, chunk.BlockData);
      this.WriteLightData(packetWriter, chunk);
      this.WriteRleData(packetWriter, chunk, chunk.AuxData);
    }

    private void WriteRleData(PacketWriter packetWriter, MapChunk chunk, RLEStreamByte rle)
    {
      rle.UpdateStream(chunk);
      packetWriter.Write(rle.StreamSize);
      if (rle.StreamSize <= 0)
        return;
      packetWriter.Write(Map.RLEStreamBufferManager.Stream[(int) rle.StreamID], rle.StreamIndex, rle.StreamSize);
    }

    private void WriteLightData(PacketWriter packetWriter, MapChunk chunk)
    {
      if (!chunk.IsLightDirty)
      {
        RLEStreamByte lightData = chunk.LightData;
        lightData.UpdateStream(chunk);
        if (lightData.StreamSize < 40)
        {
          packetWriter.Write(lightData.StreamSize);
          if (lightData.StreamSize <= 0)
            return;
          packetWriter.Write(Map.RLEStreamBufferManager.Stream[(int) lightData.StreamID], lightData.StreamIndex, lightData.StreamSize);
          return;
        }
      }
      packetWriter.Write(0);
    }

    private void SendUneditedChunks(NetworkManager.MachineData machine)
    {
      lock (this.chunkRequestLock)
      {
        if (machine.UneditedChunkRequests.Count <= 0)
          return;
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, machine.Host);
        lock (packetWriter)
        {
          packetWriter.Write((byte) 32);
          packetWriter.Write((byte) 32);
          this.WriteLongList(packetWriter, machine.UneditedChunkRequests);
          this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, machine.Host);
          machine.UneditedChunkRequests.Clear();
        }
      }
    }

    private void QueueChunksForGeneration(NetworkManager.MachineData machine)
    {
      lock (this.chunkRequestLock)
      {
        if (machine.HostNeedsToGenerateChunks.Count <= 0)
          return;
        this.gameInstance.QueueChunksForRemoteGenerating(machine.HostNeedsToGenerateChunks);
        foreach (long needsToGenerateChunk in machine.HostNeedsToGenerateChunks)
        {
          if (!machine.HostChunkGenerationQueued.ContainsKey(needsToGenerateChunk))
          {
            MapChunk chunk = this.gameInstance.Map.GetChunk(needsToGenerateChunk);
            if (!chunk.IsDecorated)
              machine.HostChunkGenerationQueued.Add(needsToGenerateChunk, needsToGenerateChunk);
            else
              this.AddChunkReqeust(machine, (NetworkGamer) null, chunk, needsToGenerateChunk);
          }
        }
        machine.HostNeedsToGenerateChunks.Clear();
      }
    }

    private void ReadChunkData(NetworkGamer sender)
    {
      int num = this.packetReader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        MapChunkPendingData data = new MapChunkPendingData();
        data.Sender = sender;
        long hash = this.packetReader.ReadInt64();
        data.Flags = (ChunkFlags) this.packetReader.ReadUInt32();
        data.BlockData = this.ReadChunkStreamData(this.packetReader);
        data.LightData = this.ReadChunkStreamData(this.packetReader);
        data.AuxData = this.ReadChunkStreamData(this.packetReader);
        this.RegisterChunkReceived(hash);
        if (this.gameInstance != null)
        {
          MapChunk chunk = this.gameInstance.Map.GetChunk(hash);
          if (chunk != null)
            this.gameInstance.Map.AddChunkPendingData(chunk, data);
        }
      }
    }

    private MapChunkPendingStream ReadChunkStreamData(PacketReader reader)
    {
      MapChunkPendingStream chunkPendingStream = new MapChunkPendingStream();
      lock (BuffLock.StreamLock)
      {
        chunkPendingStream.StreamSize = reader.ReadInt32();
        if (chunkPendingStream.StreamSize > 0)
        {
          Map.RLEStreamBufferManager.Allocate(chunkPendingStream.StreamSize, out chunkPendingStream.StreamID, out chunkPendingStream.StreamIndex);
          reader.Read(Map.RLEStreamBufferManager.Stream[(int) chunkPendingStream.StreamID], chunkPendingStream.StreamIndex, chunkPendingStream.StreamSize);
        }
      }
      return chunkPendingStream;
    }

    private void ReadUneditedChunks(NetworkGamer sender)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      lock (this.readChunkList)
      {
        this.ReadLongList(this.readChunkList);
        if (this.readChunkList.Count <= 0)
          return;
        MapTM mapTm = this.IsHost || this.gameInstance == null ? (MapTM) null : this.gameInstance.Map;
        foreach (long readChunk in this.readChunkList)
        {
          this.RegisterChunkReceived(readChunk);
          if (mapTm != null)
            (mapTm.GetChunk(readChunk) as MapChunkTM)?.SetChunkFlag(ChunkFlags.ReceivedFromHost);
        }
      }
    }

    private void RegisterChunkReceived(long hash)
    {
      lock (this.chunksRequestedNotReceived)
      {
        int count = this.chunksRequestedNotReceived.Count;
        for (int index = 0; index < count; ++index)
        {
          NetworkManager.ChunkRequestsSent chunkRequestsSent = this.chunksRequestedNotReceived.Dequeue();
          if (chunkRequestsSent.Hash != hash)
            this.chunksRequestedNotReceived.Enqueue(chunkRequestsSent);
        }
      }
    }

    public void SendFileShare(
      SessionType type,
      string shareName,
      int dirNum,
      string filename,
      NetworkGamer recipient,
      Action<NetworkGamer, FileShareSendProgress> callback)
    {
      FileShareInfo fileShareInfo = new FileShareInfo() { Type = type, ShareName = shareName, DirNum = dirNum, Filename = filename, Recipient = recipient, Callback = callback };
      lock (this.currentFileShares)
        this.currentFileShares.Add(fileShareInfo);
    }

    private void RemoveAnyCurrentSharesForGamer(NetworkGamer gamer)
    {
      lock (this.currentFileShares)
      {
        for (int index = this.currentFileShares.Count - 1; index >= 0; --index)
        {
          FileShareInfo currentFileShare = this.currentFileShares[index];
          if (currentFileShare != null && currentFileShare.Recipient.ID == gamer.ID)
          {
            currentFileShare.Progress.Status = FileShareStatus.RecipientLeftSession;
            currentFileShare.Callback(gamer, currentFileShare.Progress);
            this.currentFileShares.RemoveAt(index);
          }
        }
      }
    }

    private FileShareInfo GetCurrentShare()
    {
      if (this.currentFileShares.Count > 0)
        return this.currentFileShares[0];
      return (FileShareInfo) null;
    }

    private void SendFileShare()
    {
      lock (this.currentFileShares)
      {
        FileShareInfo currentShare = this.GetCurrentShare();
        if (currentShare == null)
          return;
        lock (currentShare)
        {
          this.InitFileShare(currentShare);
          this.SendFileShareCore(currentShare);
          this.EndFileShare(currentShare);
        }
      }
    }

    private void InitFileShare(FileShareInfo share)
    {
      if (share.Internals == null)
      {
        share.Internals = new FileShareInfoInternals()
        {
          FirstPacket = true
        };
        share.Progress = new FileShareSendProgress()
        {
          Info = share
        };
        if (share.Filename != null)
        {
          share.Internals.FileList = new string[1]
          {
            share.Filename
          };
        }
        else
        {
          List<string> stringList = new List<string>((IEnumerable<string>) FileSystem.GetFiles(Globals2.GetFilePath(MapType.Map, share.Type, share.DirNum), "*.*"));
          for (int index = stringList.Count - 1; index >= 0; --index)
          {
            if (stringList[index].EndsWith(".scr"))
              stringList.RemoveAt(index);
          }
          share.Internals.FileList = stringList.ToArray();
        }
        new Thread(new ParameterizedThreadStart(this.InitFileShareThreaded))
        {
          CurrentCulture = Globals1.CultureInfo,
          CurrentUICulture = Globals1.CultureInfo
        }.Start((object) share);
      }
      this.EnsureFileShareBufferIsInitialized();
    }

    private void InitFileShareThreaded(object s)
    {
      FileShareInfo fileShareInfo = s as FileShareInfo;
      fileShareInfo.Internals.TotalBytesToShare = Globals2.GetTotalBytes(fileShareInfo.Internals.FileList);
      fileShareInfo.Progress.TotalBytesToShare = fileShareInfo.Internals.TotalBytesToShare;
      lock (fileShareInfo)
        fileShareInfo.Internals.Stream = FileSystem.OpenRead(fileShareInfo.Internals.FileList[0]);
    }

    private void EnsureFileShareBufferIsInitialized()
    {
      if (this.fileShareBuffer != null)
        return;
      this.fileShareBuffer = new byte[this.fileShareBufferLen];
    }

    private void SendFileShareCore(FileShareInfo share)
    {
      FileShareInfoInternals internals = share.Internals;
      if (internals.Stream == null)
        return;
      int count = Math.Min(this.fileShareBufferLen, (int) (internals.Stream.Length - internals.Stream.Position));
      internals.Stream.Read(this.fileShareBuffer, 0, count);
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, share.Recipient);
      if (packetWriter != null)
      {
        lock (packetWriter)
        {
          packetWriter.Write((byte) 22);
          packetWriter.Write((byte) 22);
          packetWriter.Write((byte) 3);
          packetWriter.Write((byte) share.Type);
          packetWriter.Write(internals.FirstPacket);
          if (internals.FirstPacket)
            packetWriter.Write(share.ShareName);
          packetWriter.Write(Globals2.ExtractFileFromPath(internals.FileList[internals.CurrentFile]));
          packetWriter.Write(internals.TotalBytesToShare);
          packetWriter.Write(count);
          packetWriter.Write(this.fileShareBuffer, 0, count);
          this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, share.Recipient);
        }
      }
      share.Progress.Status = FileShareStatus.TransferInProgress;
      share.Progress.BytesShared += count;
      share.Callback(share.Recipient, share.Progress);
      internals.FirstPacket = false;
    }

    private void EndFileShare(FileShareInfo share)
    {
      FileShareInfoInternals internals = share.Internals;
      if (internals.Stream == null || internals.Stream.Position != internals.Stream.Length)
        return;
      internals.Stream.Close();
      if (internals.CurrentFile == internals.FileList.Length - 1)
      {
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, share.Recipient);
        if (packetWriter != null)
        {
          lock (packetWriter)
          {
            packetWriter.Write((byte) 22);
            packetWriter.Write((byte) 22);
            packetWriter.Write((byte) 4);
            this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, share.Recipient);
          }
        }
        internals.Stream = (Stream) null;
      }
      else
        internals.Stream = FileSystem.OpenRead(internals.FileList[++internals.CurrentFile]);
    }

    private void ReadFileShare(NetworkGamer sender)
    {
      switch (this.packetReader.ReadByte())
      {
        case 2:
          this.ReadFileShareWaiting();
          break;
        case 3:
          this.ReadFileShareInProgress(sender);
          break;
        case 4:
          this.ReadFileShareComplete();
          break;
      }
    }

    private void ReadFileShareInProgress(NetworkGamer sender)
    {
      SessionType shareType = (SessionType) this.packetReader.ReadByte();
      bool flag = this.packetReader.ReadBoolean();
      string str = (string) null;
      if (flag)
        str = this.packetReader.ReadString();
      string filename = this.packetReader.ReadString();
      int num1 = this.packetReader.ReadInt32();
      this.EnsureShareReceiveInitialized(shareType, filename, sender);
      int num2 = this.packetReader.ReadInt32();
      this.EnsureFileShareBufferIsInitialized();
      this.packetReader.Read(this.fileShareBuffer, 0, num2);
      this.fileReceiveInfo.Stream.Write(this.fileShareBuffer, 0, num2);
      if (this.fileReceiveInfo.Progress.FirstPacket = flag)
        this.fileReceiveInfo.Progress.ShareName = str;
      this.fileReceiveInfo.Progress.Status = FileShareStatus.TransferInProgress;
      this.fileReceiveInfo.Progress.TotalBytesToShare = num1;
      this.fileReceiveInfo.Progress.BytesShared += num2;
      this.fileReceiveInfo.Callback(this.fileReceiveInfo.Progress);
      this.SendFileShareAck(num2, false, sender);
    }

    private void ReadFileShareWaiting()
    {
    }

    private void ReadFileShareComplete()
    {
      if (this.fileReceiveInfo.Stream != null)
        this.fileReceiveInfo.Stream.Close();
      this.SendFileShareAck(0, true, this.fileReceiveInfo.Sender);
      this.fileReceiveInfo.Progress.Status = FileShareStatus.TransferComplete;
      this.fileReceiveInfo.Callback(this.fileReceiveInfo.Progress);
      this.fileReceiveInfo = (FileReceiveInfo) null;
    }

    private void EnsureShareReceiveInitialized(
      SessionType shareType,
      string filename,
      NetworkGamer sender)
    {
      if (this.fileReceiveInfo == null)
      {
        this.fileReceiveInfo = new FileReceiveInfo()
        {
          Type = shareType,
          DirNumber = Globals2.GetNewDirNumber(shareType),
          Callback = this.fileShareReceiveCallback,
          Sender = sender
        };
        this.fileReceiveInfo.Path = Globals2.GetFilePath(MapType.Map, shareType, this.fileReceiveInfo.DirNumber);
        FileSystem.CreateDir(this.fileReceiveInfo.Path);
        FileShareReceiveProgress shareReceiveProgress = new FileShareReceiveProgress();
        shareReceiveProgress.Status = FileShareStatus.TransferInProgress;
        shareReceiveProgress.DirNumber = this.fileReceiveInfo.DirNumber;
        this.fileReceiveInfo.Progress = shareReceiveProgress;
        this.fileReceiveInfo.Callback(shareReceiveProgress);
      }
      if (!(filename != this.fileReceiveInfo.Filename))
        return;
      if (this.fileReceiveInfo.Stream != null)
        this.fileReceiveInfo.Stream.Close();
      this.fileReceiveInfo.Filename = filename;
      this.fileReceiveInfo.Stream = FileSystem.CreateFile(this.fileReceiveInfo.Path + "\\" + filename);
    }

    public void SendFileShareAck(
      int bytesReceived,
      bool completionConfirmed,
      NetworkGamer recipient)
    {
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 23);
        packetWriter.Write((byte) 23);
        packetWriter.Write(completionConfirmed);
        packetWriter.Write(bytesReceived);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, recipient);
      }
    }

    private void ReadFileShareAck(NetworkGamer sender)
    {
      bool flag = this.packetReader.ReadBoolean();
      int num = this.packetReader.ReadInt32();
      lock (this.currentFileShares)
      {
        FileShareInfo currentShare = this.GetCurrentShare();
        if (currentShare == null)
          return;
        currentShare.Progress.BytesReceived += num;
        currentShare.Callback(currentShare.Recipient, currentShare.Progress);
        if (!flag)
          return;
        this.currentFileShares.Remove(currentShare);
        currentShare.Progress.Status = FileShareStatus.TransferComplete;
        currentShare.Callback(currentShare.Recipient, currentShare.Progress);
      }
    }

    public void SendServerCommand(PacketType packetType, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || this.sessionType != SessionType.Server)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) packetType);
        packetWriter.Write((byte) packetType);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, this.session.Host);
      }
    }

    public void SendHighscoreDataRequest(int timestamp, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || this.sessionType != SessionType.Server)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 6);
        packetWriter.Write((byte) 6);
        packetWriter.Write(timestamp);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, recipient);
      }
    }

    public void SendServerData(
      NetworkGamer recipient,
      List<string> messageList,
      List<BadBoyData> badboyList)
    {
      if (!this.IsHost || !this.IsSessionOpenAndNotLocal || (recipient == null || this.sessionType != SessionType.Server))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 4);
        packetWriter.Write((byte) 4);
        this.WriteStringArray(packetWriter, messageList.ToArray());
        this.WriteBadboyData(packetWriter, badboyList);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, recipient);
      }
    }

    public void SendErrorsToErrorServer()
    {
      if (TotalMinerGame.Instance.CaughtExceptions.Count <= 0 || this.IsHost || (!this.IsSessionOpenAndNotLocal || this.sessionType != SessionType.Server))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 7);
        packetWriter.Write((byte) 7);
        this.WriteExceptionDataToServer(packetWriter);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, this.session.Host);
      }
      TotalMinerGame.Instance.CaughtExceptions.Clear();
    }

    public void SendHighScores(
      int serverTimestamp,
      int userTimestamp,
      HighScoreData highScores,
      NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || this.sessionType != SessionType.Server || highScores == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 5);
        packetWriter.Write((byte) 5);
        this.WriteHighScoreData(packetWriter, serverTimestamp, userTimestamp, serverTimestamp == 0 ? (string) null : recipient.Gamertag, highScores);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, recipient);
      }
    }

    private void WriteExceptionDataToServer(PacketWriter writer)
    {
      writer.Write(TotalMinerGame.Instance.CaughtExceptions.Count);
      foreach (string key in TotalMinerGame.Instance.CaughtExceptions.Keys)
      {
        writer.Write(key);
        writer.Write(TotalMinerGame.Instance.CaughtExceptions[key]);
      }
    }

    private void WriteBadboyData(PacketWriter writer, List<BadBoyData> badboyList)
    {
      writer.Write((ushort) badboyList.Count);
      foreach (BadBoyData badboy in badboyList)
      {
        writer.Write((byte) badboy.Type);
        writer.Write(badboy.Gamertag != null ? badboy.Gamertag : "");
      }
    }

    private void WriteHighScoreData(
      PacketWriter writer,
      int serverTimestamp,
      int userTimestamp,
      string userGamertag,
      HighScoreData highScores)
    {
      writer.Write(serverTimestamp);
      int position = writer.Position;
      writer.Write(0);
      if (highScores != null && highScores.HighScores.Count > 0)
      {
        int num = 0;
        foreach (KeyValuePair<string, HighScoreItem> highScore in highScores.HighScores)
        {
          if (userTimestamp == 0 || highScore.Value.Ticks > userTimestamp || highScore.Key == userGamertag)
          {
            ++num;
            writer.Write(highScore.Key);
            for (int index = 0; index < 15; ++index)
              writer.Write(highScore.Value.XPList[index]);
          }
        }
        writer.Position = position;
        writer.Write(num);
        writer.Position = writer.Length;
      }
      highScores.WriteStateBanned((BinaryWriter) writer);
    }

    private void ReadServerDataRequest(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerDataRequest);
    }

    private void ReadServerUncaughtExceptionReport(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerUncaughtExceptionReport);
    }

    private void ReadServerCaughtExceptionReport(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerCaughtExceptionReport);
    }

    private void ReadServerHighscoreData(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerHighscoreData);
    }

    private void ReadServerHighscoreDataRequest(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerHighscoreDataRequest);
    }

    private void ReadServerConfirmReceiptRequest(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerConfirmReceiptRequest);
    }

    private void ReadServerReceiptConfirmed(NetworkGamer sender)
    {
      this.ReadServerReport(sender, PacketType.ServerReceiptConfirmed);
    }

    private void ReadServerReport(NetworkGamer sender, PacketType packetType)
    {
      if (this.onServerPacketReceived == null || this.sessionType != SessionType.Server)
        return;
      this.onServerPacketReceived(sender, this.packetReader, packetType);
    }

    private void ReadServerData(NetworkGamer sender)
    {
      if (this.sessionType != SessionType.Server)
        return;
      Globals2.SaveAllDataThreaded(Globals2.UpdateBannerList(this.ReadStringArray()), Globals2.GamertagData.UpdateBadBoys(this.ReadBadBoyData()));
      if (this.onServerPacketReceived == null)
        return;
      this.onServerPacketReceived(this.session.Host, this.packetReader, PacketType.ServerData);
    }

    public bool ReadHighScoreData(
      PacketReader packetReader,
      HighScoreData highScores,
      out int serverTimestamp,
      out int updates)
    {
      bool flag = false;
      serverTimestamp = packetReader.ReadInt32();
      updates = packetReader.ReadInt32();
      if (updates > 0)
      {
        for (int index1 = 0; index1 < updates; ++index1)
        {
          HighScoreItem data = new HighScoreItem();
          data.Ticks = serverTimestamp;
          string gamertag = packetReader.ReadString();
          data.XPList = new int[15];
          for (int index2 = 0; index2 < data.XPList.Length; ++index2)
            data.XPList[index2] = packetReader.ReadInt32();
          flag |= Globals2.GamertagData.AddHighScoreEntry(highScores, gamertag, data);
        }
      }
      Globals2.GamertagData.HighScoreData.ReadStateBanned((BinaryReader) packetReader, 294);
      return flag;
    }

    private List<BadBoyData> ReadBadBoyData()
    {
      List<BadBoyData> badBoyDataList = (List<BadBoyData>) null;
      ushort num = this.packetReader.ReadUInt16();
      if (num > (ushort) 0)
      {
        badBoyDataList = new List<BadBoyData>((int) num);
        for (int index = 0; index < (int) num; ++index)
          badBoyDataList.Add(new BadBoyData()
          {
            Type = (BadBoyType) this.packetReader.ReadByte(),
            Gamertag = this.packetReader.ReadString()
          });
      }
      return badBoyDataList;
    }

    public bool HasBufferedChangesToProcess
    {
      get
      {
        if (this.gameInstance == null)
          return false;
        lock (this.BufferSemaphore)
          return this.bufferedChanges.Count > 0;
      }
    }

    public NetworkManager.BufferedChangeBase GetNextBufferedChange()
    {
      if (this.gameInstance != null)
      {
        lock (this.BufferSemaphore)
        {
          if (this.bufferedChanges.Count > 0)
          {
            NetworkManager.BufferedChangeBase change = this.bufferedChanges.Dequeue();
            if (this.gameInstance.AreChunksDecorated(change))
              return change;
            this.bufferedChanges.Enqueue(change);
          }
        }
      }
      return (NetworkManager.BufferedChangeBase) null;
    }

    private void EnqueueBufferedChange(NetworkManager.BufferedChangeBase change)
    {
      if (change.ChunksList != null && change.ChunksList.Count != 0 && change.ChunksList.Count == 1)
      {
        change.ChunkHash = new long?(change.ChunksList[0]);
        change.ChunksList = (List<long>) null;
      }
      lock (this.BufferSemaphore)
        this.bufferedChanges.Enqueue(change);
    }

    private void WriteMetaExecute(PacketWriter packetWriter, MetaExecuteBase meta)
    {
      packetWriter.Write((byte) meta.Type);
      meta.WriteState((BinaryWriter) packetWriter);
    }

    private MetaExecuteBase ReadMetaExecute(PacketReader packetReader)
    {
      MetaExecuteBase metaExecuteBase = MetaExecuteFactory.Create((MetaExecuteType) packetReader.ReadByte());
      metaExecuteBase.ReadState((BinaryReader) packetReader);
      return metaExecuteBase;
    }

    private void ProcessMetaExecute(MetaExecuteBase meta)
    {
      if (meta.Type != MetaExecuteType.Script)
        return;
      MetaExecuteScript metaExecuteScript = (MetaExecuteScript) meta;
      ScriptExecuteData data = new ScriptExecuteData() { ScriptOffset = metaExecuteScript.ScriptOffset, BlockOffset = metaExecuteScript.BlockOffset };
      this.gameInstance.ExecuteScript(metaExecuteScript.ScriptName, data, false);
    }

    public void SendCommand(NetworkCommand command)
    {
      this.SendCommand(command, (NetworkGamer) null, SendDataOptions.Reliable);
    }

    public void SendCommand(
      NetworkCommand command,
      NetworkGamer recipient,
      SendDataOptions sendOptions)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(sendOptions, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 11);
        packetWriter.Write((byte) 11);
        packetWriter.Write((byte) command);
        this.EndPacketWriter(packetWriter, sendOptions, recipient);
      }
    }

    private void ReadCommand(NetworkGamer sender)
    {
      switch (this.packetReader.ReadByte())
      {
        case 1:
          if (this.gameInstance == null)
            break;
          this.gameInstance.ClearAllParticles(false);
          break;
        case 2:
          this.HostIsReady = true;
          break;
        case 3:
          if (this.gameInstance == null)
            break;
          this.gameInstance.RemoteIsLoaded();
          break;
        case 4:
          sender.Machine.RemoveFromSession();
          break;
      }
    }

    public void SendBlockAuxChange(
      GlobalPoint3D p,
      byte oldAuxData,
      byte auxData,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      MapBlock oldBlockData = new MapBlock() { AuxData = oldAuxData };
      MapBlock blockData = new MapBlock() { AuxData = auxData };
      this.SendBlockChange(p, oldBlockData, blockData, method, playerID, true);
    }

    public void SendBlockChange(
      GlobalPoint3D p,
      MapBlock oldBlockData,
      MapBlock blockData,
      UpdateBlockMethod method,
      GamerID playerID,
      bool auxOnly)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      NetworkManager.BufferedBlockChange bufferedBlockChange = new NetworkManager.BufferedBlockChange();
      bufferedBlockChange.GamerID = playerID;
      bufferedBlockChange.X = (short) p.X;
      bufferedBlockChange.Y = (short) p.Y;
      bufferedBlockChange.Z = (short) p.Z;
      bufferedBlockChange.OldBlockData = oldBlockData;
      bufferedBlockChange.BlockData = blockData;
      bufferedBlockChange.Method = method;
      bufferedBlockChange.AuxChangeOnly = auxOnly;
      lock (this.blockChangesToSend)
        this.blockChangesToSend.Add(bufferedBlockChange);
    }

    private void SendBlockChanges()
    {
      lock (this.blockChangesToSend)
      {
        if (this.blockChangesToSend.Count <= 0)
          return;
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
        if (packetWriter != null)
        {
          lock (packetWriter)
          {
            packetWriter.Write((byte) 24);
            packetWriter.Write((byte) 24);
            packetWriter.Write((ushort) this.blockChangesToSend.Count);
            foreach (NetworkManager.BufferedBlockChange bufferedBlockChange in this.blockChangesToSend)
            {
              packetWriter.WriteGamerID(bufferedBlockChange.GamerID);
              packetWriter.Write(bufferedBlockChange.X);
              packetWriter.Write(bufferedBlockChange.Y);
              packetWriter.Write(bufferedBlockChange.Z);
              packetWriter.Write(bufferedBlockChange.AuxChangeOnly);
              if (!bufferedBlockChange.AuxChangeOnly)
              {
                packetWriter.Write(bufferedBlockChange.OldBlockData.BlockID);
                packetWriter.Write(bufferedBlockChange.OldBlockData.Light.ToByte());
                packetWriter.Write(bufferedBlockChange.OldBlockData.AuxData);
                packetWriter.Write(bufferedBlockChange.BlockData.BlockID);
                packetWriter.Write(bufferedBlockChange.BlockData.Light.ToByte());
                packetWriter.Write(bufferedBlockChange.BlockData.AuxData);
              }
              else
              {
                packetWriter.Write(bufferedBlockChange.OldBlockData.AuxData);
                packetWriter.Write(bufferedBlockChange.BlockData.AuxData);
              }
              packetWriter.Write((byte) bufferedBlockChange.Method);
            }
            this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
          }
        }
        this.blockChangesToSend.Clear();
      }
    }

    private void ReadBlockChanges(NetworkGamer sender)
    {
      int num = (int) this.packetReader.ReadUInt16();
      for (int index = 0; index < num; ++index)
      {
        NetworkManager.BufferedBlockChange bufferedBlockChange = new NetworkManager.BufferedBlockChange();
        bufferedBlockChange.Type = NetworkManager.BufferedChangeType.BlockChange;
        bufferedBlockChange.GamerID = this.packetReader.ReadGamerID();
        bufferedBlockChange.X = this.packetReader.ReadInt16();
        bufferedBlockChange.Y = this.packetReader.ReadInt16();
        bufferedBlockChange.Z = this.packetReader.ReadInt16();
        bufferedBlockChange.AuxChangeOnly = this.packetReader.ReadBoolean();
        if (!bufferedBlockChange.AuxChangeOnly)
        {
          bufferedBlockChange.OldBlockData.BlockID = this.packetReader.ReadByte();
          bufferedBlockChange.OldBlockData.Light = MapLight.FromByte(this.packetReader.ReadByte());
          bufferedBlockChange.OldBlockData.AuxData = this.packetReader.ReadByte();
          bufferedBlockChange.BlockData.BlockID = this.packetReader.ReadByte();
          bufferedBlockChange.BlockData.Light = MapLight.FromByte(this.packetReader.ReadByte());
          bufferedBlockChange.BlockData.AuxData = this.packetReader.ReadByte();
        }
        else
        {
          bufferedBlockChange.OldBlockData.AuxData = this.packetReader.ReadByte();
          bufferedBlockChange.BlockData.AuxData = this.packetReader.ReadByte();
        }
        bufferedBlockChange.Method = (UpdateBlockMethod) this.packetReader.ReadByte();
        if (this.gameInstance != null)
        {
          MapChunk chunk = this.gameInstance.Map.GetChunk(new GlobalPoint3D((int) bufferedBlockChange.X, (int) bufferedBlockChange.Y, (int) bufferedBlockChange.Z));
          if (chunk != null)
          {
            bufferedBlockChange.ChunkHash = new long?(chunk.GetGlobalHashCode());
            this.EnqueueBufferedChange((NetworkManager.BufferedChangeBase) bufferedBlockChange);
          }
        }
      }
    }

    public void SendBlockTextureChange(
      StudioForge.TotalMiner.Player player,
      GlobalPoint3D point,
      Block blockID,
      Block textureID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      NetworkGamer recipient = this.IsHost ? (NetworkGamer) null : this.session.Host;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 25);
        packetWriter.Write((byte) 25);
        packetWriter.Write((ushort) point.X);
        packetWriter.Write((ushort) point.Y);
        packetWriter.Write((ushort) point.Z);
        packetWriter.WriteGamerID(player != null ? player.GamerID : GamerID.Sys1);
        packetWriter.Write((byte) blockID);
        packetWriter.Write((byte) textureID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadBlockTextureChange(NetworkGamer sender)
    {
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      globalPoint3D.X = (int) this.packetReader.ReadUInt16();
      globalPoint3D.Y = (int) this.packetReader.ReadUInt16();
      globalPoint3D.Z = (int) this.packetReader.ReadUInt16();
      GamerID gamerID = this.packetReader.ReadGamerID();
      Block blockID = (Block) this.packetReader.ReadByte();
      Block block = (Block) this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      StudioForge.TotalMiner.Player player = this.gameInstance.GetPlayer(gamerID);
      if (this.gameInstance.Map.ChangeBlockTextureFromHost(player, globalPoint3D, blockID, block) == MapTM.BlockTextureChangeResult.ExistingTextureUsed)
        this.SendPhotoThumbnailRequest(this.gameInstance.Map.GetAuxHighData(globalPoint3D));
      if (this.IsHost)
        this.SendBlockTextureChange(player, globalPoint3D, blockID, block);
      this.RaiseBlockTextureChangedReceived((object) this, globalPoint3D, block);
    }

    public void SendBlockTextureRemoved(Block block, int index)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      NetworkGamer recipient = this.IsHost ? (NetworkGamer) null : this.session.Host;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 26);
        packetWriter.Write((byte) 26);
        packetWriter.Write((byte) block);
        packetWriter.Write(index);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadBlockTextureRemoved(NetworkGamer sender)
    {
      Block block = (Block) this.packetReader.ReadByte();
      int index = this.packetReader.ReadInt32();
      if (this.gameInstance == null)
        return;
      this.gameInstance.Map.SetBlockTexture(block, index, Block.None);
      if (!this.IsHost)
        return;
      this.SendBlockTextureRemoved(block, index);
    }

    public void SendGameInstanceDataRequest()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 19);
        packetWriter.Write((byte) 19);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadGameInstanceDataRequest(NetworkGamer sender)
    {
      this.resendAllGameInstances = this.resendAllMobInstances = true;
    }

    private void SendGameInstanceData()
    {
      ++this.onlineSendTimer;
      if (this.onlineSendTimer < 8 || !this.IsSessionOpenAndNotLocalAndRemotes)
        return;
      this.SendPlayerPositions();
      this.onlineSendTimer = 0;
      if (!this.IsInstanceDataToSend())
        return;
      this.playersToSend.Clear();
      for (int index = 0; index < this.tempLocalPlayerList.Count; ++index)
      {
        StudioForge.TotalMiner.Player tempLocalPlayer = this.tempLocalPlayerList[index];
        if (tempLocalPlayer.IsEnabledField)
        {
          PlayerNetStateData lastStateSent = tempLocalPlayer.LastStateSent;
          PlayerStateDataToSend playerStateDataToSend = tempLocalPlayer.StateDataToSend & (PlayerStateDataToSend.RefreshItemModels | PlayerStateDataToSend.FootSound);
          if (tempLocalPlayer.IsFlying)
            playerStateDataToSend |= PlayerStateDataToSend.IsFlying;
          if (tempLocalPlayer.IceEffectActive)
            playerStateDataToSend |= PlayerStateDataToSend.IceEffectActive;
          if (tempLocalPlayer.PositionReset)
          {
            playerStateDataToSend |= PlayerStateDataToSend.PositionReset;
            tempLocalPlayer.PositionReset = false;
          }
          if (tempLocalPlayer.Inventory.HotBarLeftSlotID != (int) lastStateSent.HotBarLeftID || tempLocalPlayer.Inventory.HotBarRightSlotID != (int) lastStateSent.HotBarRightID)
          {
            playerStateDataToSend |= PlayerStateDataToSend.HotBar;
            lastStateSent.StateToSend &= ~PlayerStateDataToSend.HotBar;
          }
          tempLocalPlayer.StateDataToSend = playerStateDataToSend;
          if (this.resendAllGameInstances || this.NeedToSendData(tempLocalPlayer))
            this.playersToSend.Add(tempLocalPlayer);
        }
      }
      if (this.playersToSend.Count > 0)
      {
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, (NetworkGamer) null);
        if (packetWriter != null)
        {
          lock (packetWriter)
          {
            int length = packetWriter.Length;
            try
            {
              packetWriter.Write((byte) 18);
              packetWriter.Write((byte) 18);
              packetWriter.Write((byte) this.playersToSend.Count);
              foreach (StudioForge.TotalMiner.Player player in this.playersToSend)
                this.WritePlayerState(packetWriter, player);
            }
            catch (Exception ex)
            {
              Services.ExceptionReporter.ReportExceptionCaught(68, ex);
              packetWriter.BaseStream.SetLength((long) length);
            }
            this.EndPacketWriter(packetWriter, SendDataOptions.None, (NetworkGamer) null);
          }
        }
      }
      this.resendAllGameInstances = false;
    }

    private void SendPlayerPositions()
    {
      if (this.tempLocalEnabledPlayerList.Count <= 0)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        int length = packetWriter.Length;
        try
        {
          packetWriter.Write((byte) 20);
          packetWriter.Write((byte) 20);
          this.WritePlayerSpatial(packetWriter, this.tempLocalEnabledPlayerList[0]);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(68, ex);
          packetWriter.BaseStream.SetLength((long) length);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.None, (NetworkGamer) null);
      }
    }

    private bool NeedToSendData(StudioForge.TotalMiner.Player player)
    {
      PlayerNetStateData lastStateSent = player.LastStateSent;
      if (player.StateDataToSend == lastStateSent.StateToSend && player.LeftSwingCountNet == 0 && (player.RightSwingCountNet == 0 && (double) player.Size.Y == (double) lastStateSent.SizeY) && (double) player.Health == (double) lastStateSent.Health)
        return (int) (byte) (player.State & (ActorState) 15) != (int) lastStateSent.StateByte;
      return true;
    }

    private void WritePlayerState(PacketWriter packetWriter, StudioForge.TotalMiner.Player player)
    {
      packetWriter.WriteGamerID(player.Gamer.ID);
      PlayerStateDataToSend stateDataToSend = player.StateDataToSend;
      packetWriter.Write((byte) stateDataToSend);
      byte num1 = (byte) ((uint) (byte) player.State & 15U);
      player.LastStateSent.StateByte = num1;
      int num2 = (player.LeftSwingCountNet & 3) << 2;
      int num3 = player.RightSwingCountNet & 3;
      byte num4 = (byte) ((uint) num1 + (uint) (byte) (num2 + num3 << 4));
      packetWriter.Write(num4);
      player.LeftSwingCountNet = 0;
      player.RightSwingCountNet = 0;
      packetWriter.Write(new HalfSingle(player.Size.Y).PackedValue);
      player.LastStateSent.SizeY = player.Size.Y;
      packetWriter.Write(new HalfSingle(player.Health).PackedValue);
      player.LastStateSent.Health = player.Health;
      if ((stateDataToSend & PlayerStateDataToSend.HotBar) == PlayerStateDataToSend.HotBar)
      {
        packetWriter.Write((byte) player.Inventory.HotBarLeftSlotID);
        player.LastStateSent.HotBarLeftID = (byte) player.Inventory.HotBarLeftSlotID;
        packetWriter.Write((byte) player.Inventory.HotBarRightSlotID);
        player.LastStateSent.HotBarRightID = (byte) player.Inventory.HotBarRightSlotID;
      }
      if ((stateDataToSend & PlayerStateDataToSend.FootSound) == PlayerStateDataToSend.FootSound)
        packetWriter.Write((byte) player.FootStepBlockForSound);
      player.LastStateSent.StateToSend = stateDataToSend & ~PlayerStateDataToSend.HotBar;
      player.StateDataToSend = PlayerStateDataToSend.None;
    }

    private bool IsInstanceDataToSend()
    {
      return this.tempLocalPlayerList.Count > 0;
    }

    private void ReadGameInstanceData(NetworkGamer sender)
    {
      int num = (int) this.packetReader.ReadByte();
      for (int index = 0; index < num; ++index)
        this.ReadPlayerState();
    }

    private void ReadPlayerState()
    {
      HalfSingle halfSingle = new HalfSingle();
      GamerID gamerID = this.packetReader.ReadGamerID();
      PlayerStateDataToSend playerStateDataToSend = (PlayerStateDataToSend) this.packetReader.ReadByte();
      bool flag1 = (playerStateDataToSend & PlayerStateDataToSend.HotBar) == PlayerStateDataToSend.HotBar;
      bool jetPackActive = (playerStateDataToSend & PlayerStateDataToSend.IsFlying) == PlayerStateDataToSend.IsFlying;
      bool positionReset = (playerStateDataToSend & PlayerStateDataToSend.PositionReset) == PlayerStateDataToSend.PositionReset;
      bool iceEffectActive = (playerStateDataToSend & PlayerStateDataToSend.IceEffectActive) == PlayerStateDataToSend.IceEffectActive;
      bool flag2 = (playerStateDataToSend & PlayerStateDataToSend.FootSound) == PlayerStateDataToSend.FootSound;
      byte num1 = this.packetReader.ReadByte();
      ActorState state = (ActorState) ((int) num1 & 15);
      int num2 = (int) num1 >> 4;
      byte leftHandSwing = (byte) (num2 >> 2);
      byte rightHandSwing = (byte) (num2 & 3);
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      float single1 = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      float single2 = halfSingle.ToSingle();
      int slotID1 = -1;
      int slotID2 = -1;
      if (flag1)
      {
        slotID1 = (int) this.packetReader.ReadByte();
        slotID2 = (int) this.packetReader.ReadByte();
      }
      Block FootSoundBlock = Block.None;
      if (flag2)
        FootSoundBlock = (Block) this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      StudioForge.TotalMiner.Player remotePlayer = this.gameInstance.GetRemotePlayer(gamerID);
      if (remotePlayer == null)
        return;
      if (!remotePlayer.IsEnabledField)
      {
        remotePlayer.IsEnabled = true;
        this.BuildGamerList();
      }
      if (flag1)
      {
        remotePlayer.SetLeftHotBarSlot(slotID1);
        remotePlayer.SetRightHotBarSlot(slotID2);
      }
      remotePlayer.UpdateFromNetworkData(single1, state, jetPackActive, leftHandSwing, rightHandSwing, positionReset, FootSoundBlock, iceEffectActive, single2, Globals1.ElapsedWatch.ElapsedMilliseconds);
    }

    private void WritePlayerSpatial(PacketWriter packetWriter, StudioForge.TotalMiner.Player player)
    {
      packetWriter.WriteGamerID(player.Gamer.ID);
      packetWriter.Write(player.Position.X);
      packetWriter.Write(player.Position.Y);
      packetWriter.Write(player.Position.Z);
      packetWriter.Write(player.ViewDirection.X);
      packetWriter.Write(player.ViewDirection.Z);
    }

    private void ReadPlayerSpatial(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      Vector3 pos;
      pos.X = this.packetReader.ReadSingle();
      pos.Y = this.packetReader.ReadSingle();
      pos.Z = this.packetReader.ReadSingle();
      Vector2 vd;
      vd.X = this.packetReader.ReadSingle();
      vd.Y = this.packetReader.ReadSingle();
      if (this.gameInstance == null)
        return;
      StudioForge.TotalMiner.Player remotePlayer = this.gameInstance.GetRemotePlayer(gamerID);
      if (remotePlayer == null || !remotePlayer.IsEnabledField)
        return;
      remotePlayer.UpdateFromNetworkData(pos, vd);
    }

    private void SendMobInstanceData()
    {
      if (!this.IsSessionOpenAndNotLocalAndRemotes || this.gameInstance == null || this.gameInstance.NpcManager == null)
        return;
      ++this.mobFullValidateTimer;
      if (++this.onlineMobSendTimer < 10)
        return;
      this.mobsToSend.Clear();
      this.gameInstance.NpcManager.GetNpcsToSend(this.mobsToSend, 10, this.resendAllMobInstances);
      this.onlineMobSendTimer = 0;
      this.resendAllMobInstances = false;
      if (this.mobFullValidateTimer >= 60)
      {
        this.validMobList.Clear();
        this.gameInstance.NpcManager.GetActiveNpcIDs(this.validMobList);
      }
      if (this.mobsToSend.Count <= 0 && this.validMobList.Count <= 0)
        return;
      this.SendMobInstanceDataCore();
    }

    private void SendMobInstanceDataCore()
    {
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.InOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        int length = packetWriter.Length;
        try
        {
          packetWriter.Write((byte) 78);
          packetWriter.Write((byte) 78);
          packetWriter.Write((byte) this.mobsToSend.Count);
          foreach (NpcBase mob in this.mobsToSend)
            this.WriteMobInstanceData(packetWriter, mob);
          if (this.mobFullValidateTimer >= 60)
          {
            this.WriteActiveMobIDs(packetWriter);
            this.mobFullValidateTimer = 0;
          }
          else
            packetWriter.Write((byte) 0);
        }
        catch (Exception ex)
        {
          Services.ExceptionReporter.ReportExceptionCaught(69, ex);
          packetWriter.BaseStream.SetLength((long) length);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.InOrder, (NetworkGamer) null);
      }
    }

    private void WriteMobInstanceData(PacketWriter packetWriter, NpcBase mob)
    {
      ActorState state = mob.State;
      mob.LastSendState = state;
      mob.LastSendHealth = mob.Health;
      mob.LastSendPosition = mob.Position;
      mob.LastSendViewDir.X = mob.ViewDirection.X;
      mob.LastSendViewDir.Z = mob.ViewDirection.Z;
      packetWriter.Write((byte) mob.ActorType);
      packetWriter.WriteGamerID(mob.GamerID);
      byte num = (byte) state;
      if ((double) mob.FreezeTimer > 0.0)
        num += (byte) 128;
      packetWriter.Write(num);
      if (state == ActorState.InActive)
        return;
      packetWriter.Write(new HalfSingle(mob.LastSendHealth).PackedValue);
      packetWriter.Write(mob.LastSendPosition.X);
      packetWriter.Write(mob.LastSendPosition.Y);
      packetWriter.Write(mob.LastSendPosition.Z);
      packetWriter.Write(new HalfSingle(mob.Velocity.Y).PackedValue);
      packetWriter.Write(new HalfSingle(mob.LastSendViewDir.X).PackedValue);
      packetWriter.Write(new HalfSingle(mob.LastSendViewDir.Z).PackedValue);
    }

    private void WriteActiveMobIDs(PacketWriter packetWriter)
    {
      packetWriter.Write((byte) this.validMobList.Count);
      for (int index = 0; index < this.validMobList.Count; ++index)
        packetWriter.Write(this.validMobList[index]);
      this.validMobList.Clear();
    }

    private void ReadMobInstanceData(NetworkGamer sender)
    {
      int num1 = (int) this.packetReader.ReadByte();
      for (int index = 0; index < num1; ++index)
      {
        StudioForge.TotalMiner.ActorType actorType = (StudioForge.TotalMiner.ActorType) this.packetReader.ReadByte();
        GamerID npcID = this.packetReader.ReadGamerID();
        byte stateBits = this.packetReader.ReadByte();
        if (((int) stateBits & (int) sbyte.MaxValue) == 0)
        {
          if (this.gameInstance != null)
            this.gameInstance.NpcManager.DeactivateNpc(npcID);
        }
        else
        {
          HalfSingle halfSingle = new HalfSingle();
          halfSingle.PackedValue = this.packetReader.ReadUInt16();
          float single1 = halfSingle.ToSingle();
          Vector3 zero = Vector3.Zero;
          zero.X = this.packetReader.ReadSingle();
          zero.Y = this.packetReader.ReadSingle();
          zero.Z = this.packetReader.ReadSingle();
          halfSingle.PackedValue = this.packetReader.ReadUInt16();
          float single2 = halfSingle.ToSingle();
          halfSingle.PackedValue = this.packetReader.ReadUInt16();
          float single3 = halfSingle.ToSingle();
          float single4 = halfSingle.ToSingle();
          if (!this.IsHost && this.gameInstance != null)
            this.gameInstance.NpcManager.GetOrAddNpcUsingServerID(actorType, zero, npcID)?.UpdateFromNetworkData(stateBits, single1, zero, single2, single3, single4);
        }
      }
      byte num2 = this.packetReader.ReadByte();
      if (num2 <= (byte) 0)
        return;
      this.validMobList.Clear();
      for (int index = 0; index < (int) num2; ++index)
        this.validMobList.Add(this.packetReader.ReadInt16());
      if (this.gameInstance != null)
        this.gameInstance.NpcManager.ValidateFullNpcList(this.validMobList);
      this.validMobList.Clear();
    }

    public void SendMobSpawnDataRequest(GamerID mobID)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 80);
        packetWriter.Write((byte) 80);
        packetWriter.WriteGamerID(mobID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadMobSpawnDataRequest(NetworkGamer sender)
    {
      this.SendMobSpawnData(this.packetReader.ReadGamerID(), sender);
    }

    private void SendMobSpawnData(GamerID mobID, NetworkGamer recipient)
    {
      if (!this.IsHost || !this.IsSessionOpenAndNotLocal || (this.gameInstance == null || this.gameInstance.NpcManager == null))
        return;
      NpcBase npcUsingServerId = this.gameInstance.NpcManager.GetNpcUsingServerID(mobID);
      if (npcUsingServerId == null || npcUsingServerId.IsInactive)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 79);
        packetWriter.Write((byte) 79);
        packetWriter.WriteGamerID(mobID);
        this.WriteMobSpawnData(packetWriter, mobID, npcUsingServerId);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void WriteMobSpawnData(PacketWriter packetWriter, GamerID mobID, NpcBase mob)
    {
      if (mob.SpawnBlock != null)
      {
        packetWriter.Write(true);
        packetWriter.Write(this.gameInstance.Map.GetGlobalHashCode(mob.SpawnBlock.Point));
      }
      else
        packetWriter.Write(false);
      if (mob.IsCustomMob)
      {
        packetWriter.Write(true);
        CombatStats combatStats = mob.CombatStats;
        packetWriter.Write((ushort) combatStats.HealthLevel);
        packetWriter.Write((ushort) combatStats.AttackLevel);
        packetWriter.Write((ushort) combatStats.StrengthLevel);
        packetWriter.Write((ushort) combatStats.DefenceLevel);
        packetWriter.Write((ushort) combatStats.RangedLevel);
      }
      else
        packetWriter.Write(false);
    }

    private void ReadMobSpawnData(NetworkGamer sender)
    {
      GamerID npcID = this.packetReader.ReadGamerID();
      long? spawnBlockHash = new long?();
      CombatStats? stats = new CombatStats?();
      if (this.packetReader.ReadBoolean())
        spawnBlockHash = new long?(this.packetReader.ReadInt64());
      if (this.packetReader.ReadBoolean())
        stats = new CombatStats?(new CombatStats()
        {
          HealthLevel = (int) this.packetReader.ReadUInt16(),
          AttackLevel = (int) this.packetReader.ReadUInt16(),
          StrengthLevel = (int) this.packetReader.ReadUInt16(),
          DefenceLevel = (int) this.packetReader.ReadUInt16(),
          RangedLevel = (int) this.packetReader.ReadUInt16()
        });
      if (this.gameInstance == null || this.gameInstance.NpcManager == null)
        return;
      this.gameInstance.NpcManager.UpdateNpcSpawnData(npcID, spawnBlockHash, stats);
    }

    public void SendGameState(bool forceSend)
    {
      if (this.gameInstance == null || this.gameInstance.SunMoon == null || (!this.IsSessionOpenAndNotLocalAndRemotes || this.gameInstance.IsSleeping))
        return;
      this.timeSinceLastGameStateSend += Services.ElapsedTime;
      if (!forceSend && (double) this.timeSinceLastGameStateSend <= 10.0)
        return;
      this.timeSinceLastGameStateSend = 0.0f;
      if ((double) this.gameInstance.SunMoon.Rotation == (double) this.lastSendSunRotation && Globals2.GameProperties.IsRandomSeed == this.lastSendSeed)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      this.lastSendSunRotation = this.gameInstance.SunMoon.Rotation;
      this.lastSendSeed = Globals2.GameProperties.IsRandomSeed;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 21);
        packetWriter.Write((byte) 21);
        packetWriter.Write(this.lastSendSunRotation);
        packetWriter.Write(this.lastSendSeed);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, (NetworkGamer) null);
      }
    }

    private void ReadGameState(NetworkGamer sender)
    {
      float num1 = this.packetReader.ReadSingle();
      int num2 = this.packetReader.ReadInt32();
      if (this.gameInstance == null)
        return;
      this.gameInstance.SunMoon.Rotation = num1;
      Globals2.GameProperties.IsRandomSeed = num2;
    }

    public void SendGameData(byte[] gameData, NetworkGamer recipient)
    {
      if (!this.IsHost || recipient == null || this.session.RemoteGamers.Count == 0)
        return;
      NetworkManager.GameDataSendReceive gameDataSendReceive = new NetworkManager.GameDataSendReceive() { Recipient = recipient, GameData = gameData };
      lock (this.gameDataToSend)
        this.gameDataToSend.Add(gameDataSendReceive);
    }

    private void SendGameData()
    {
      lock (this.gameDataToSend)
      {
        if (this.gameDataToSend.Count <= 0)
          return;
        foreach (NetworkManager.GameDataSendReceive gameDataSendReceive in this.gameDataToSend)
        {
          PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, gameDataSendReceive.Recipient);
          if (packetWriter != null)
          {
            lock (packetWriter)
            {
              packetWriter.Write((byte) 12);
              packetWriter.Write((byte) 12);
              packetWriter.Write(gameDataSendReceive.GameData.Length);
              packetWriter.Write(gameDataSendReceive.GameData);
              packetWriter.Write((byte) 50);
              packetWriter.Write((byte) 50);
              this.SendPermissions(gameDataSendReceive.Recipient, packetWriter);
              this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, gameDataSendReceive.Recipient);
            }
          }
        }
        this.gameDataToSend.Clear();
      }
    }

    private void ReadGameData(NetworkGamer sender)
    {
      int count = this.packetReader.ReadInt32();
      this.gameDataToReceive.GameData = new byte[count];
      this.packetReader.Read(this.gameDataToReceive.GameData, 0, count);
      this.RaiseGameDataReceived((object) this.gameDataToReceive.GameData, EventArgs.Empty);
    }

    public void SendGameDataRequest()
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 13);
        packetWriter.Write((byte) 13);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadGameDataRequest(NetworkGamer sender)
    {
      if (this.gameInstance == null)
        return;
      this.gameInstance.QueueGameDataRequest(sender);
    }

    public void SendGameProperties()
    {
      this.SendGameProperties((NetworkGamer) null);
    }

    public void SendGameProperties(NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost || !Globals2.IsValidGameHeader)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 14);
        packetWriter.Write((byte) 14);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.MapType);
        packetWriter.Write(Globals2.GameProperties.IsNewMap);
        packetWriter.Write(Globals2.GameProperties.UseOldGenerator);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.MaxPlayers);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.PrivateSlots);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.SaveVersion);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.GameMode);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.Attribute);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.MapName);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.MapSeed);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.OwnerGamerTag);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.TexturePack);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.GameDifficulty);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.PvPCombat);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.CombatEnabled);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.FiniteMode);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.PassiveMobs);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.EnemyMobs);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.DayNightActive);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.WeatherActive);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.WindFactor);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.CombatLevelDifference);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.ClanProtection);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.SkillsEnabled);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.SkillsLocal);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.XPMultiplier);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.Z);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.Z);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.Z);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.Z);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.RegionSize.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.RegionSize.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.RegionSize.Z);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.ChunkSize.X);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.ChunkSize.Y);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.ChunkSize.Z);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.TerrainData.Biome);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TerrainData.MaxParticles);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TerrainData.Iterations);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.TerrainData.SeaLevel);
        packetWriter.Write((ushort) Globals2.GameProperties.SaveGame.Header.TerrainData.GroundBlock);
        Globals2.GameProperties.SaveGame.Header.BiomeParams.WriteState((BinaryWriter) packetWriter);
        packetWriter.Write(Globals2.GameProperties.IsRandomSeed);
        packetWriter.Write((byte) Globals2.MaxConcurrentPlayers);
        packetWriter.Write((ushort) Globals2.GameProperties.ShopPoint.X);
        packetWriter.Write((ushort) Math.Abs(Globals2.GameProperties.ShopPoint.Y));
        packetWriter.Write((ushort) Globals2.GameProperties.ShopPoint.Z);
        this.WriteGlobalItemData(packetWriter);
        packetWriter.Write((byte) 50);
        packetWriter.Write((byte) 50);
        this.SendPermissions(recipient, packetWriter);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadGameProperties(NetworkGamer sender)
    {
      GameProperties gp = new GameProperties((MapType) this.packetReader.ReadByte());
      gp.IsNewMap = this.packetReader.ReadBoolean();
      gp.UseOldGenerator = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.MaxPlayers = (int) this.packetReader.ReadByte();
      gp.SaveGame.Header.PrivateSlots = (int) this.packetReader.ReadByte();
      gp.SaveGame.Header.SaveVersion = this.packetReader.ReadInt32();
      byte gameMode = this.packetReader.ReadByte();
      byte attribute = this.packetReader.ReadByte();
      gp.SaveGame.Header.MapName = this.packetReader.ReadString();
      gp.SaveGame.Header.MapSeed = this.packetReader.ReadInt32();
      gp.SaveGame.Header.OwnerGamerTag = this.packetReader.ReadString();
      gp.SaveGame.Header.TexturePack = this.packetReader.ReadString();
      byte difficulty = this.packetReader.ReadByte();
      gp.SaveGame.Header.PvPCombat = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.CombatEnabled = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.FiniteMode = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.PassiveMobs = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.EnemyMobs = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.KeepItemsOnDeath = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.DayNightActive = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.WeatherActive = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.WindFactor = this.packetReader.ReadSingle();
      gp.SaveGame.Header.CombatLevelDifference = this.packetReader.ReadInt16();
      gp.SaveGame.Header.ClanProtection = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.SkillsEnabled = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.SkillsLocal = this.packetReader.ReadBoolean();
      gp.SaveGame.Header.XPMultiplier = this.packetReader.ReadSingle();
      gp.SaveGame.Header.TotalMapBound.Min.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TotalMapBound.Min.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TotalMapBound.Min.Z = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TotalMapBound.Max.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TotalMapBound.Max.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TotalMapBound.Max.Z = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Min.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Min.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Min.Z = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Max.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Max.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.CurrentMapBound.Max.Z = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.RegionSize.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.RegionSize.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.RegionSize.Z = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.ChunkSize.X = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.ChunkSize.Y = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.ChunkSize.Z = (int) this.packetReader.ReadUInt16();
      byte biome = this.packetReader.ReadByte();
      gp.SaveGame.Header.TerrainData.MaxParticles = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TerrainData.Iterations = (int) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TerrainData.SeaLevel = this.packetReader.ReadUInt16();
      gp.SaveGame.Header.TerrainData.GroundBlock = (Item) this.packetReader.ReadUInt16();
      gp.SaveGame.Header.BiomeParams.ReadState((BinaryReader) this.packetReader, 294);
      gp.IsRandomSeed = this.packetReader.ReadInt32();
      byte maxConcurrentPlayers = this.packetReader.ReadByte();
      gp.ShopPoint.X = (int) this.packetReader.ReadUInt16();
      gp.ShopPoint.Y = (int) this.packetReader.ReadUInt16();
      gp.ShopPoint.Z = (int) this.packetReader.ReadUInt16();
      bool[] flagArray = this.ReadBoolArray();
      if (this.IsHost || !this.IsValidGamePropertyData(gp, gameMode, attribute, difficulty, biome, maxConcurrentPlayers))
        return;
      Globals2.GameProperties.IsNewMap = gp.IsNewMap;
      Globals2.GameProperties.UseOldGenerator = gp.UseOldGenerator;
      Globals2.GameProperties.SaveGame.Header.MaxPlayers = gp.SaveGame.Header.MaxPlayers;
      Globals2.GameProperties.SaveGame.Header.PrivateSlots = gp.SaveGame.Header.PrivateSlots;
      Globals2.GameProperties.SaveGame.Header.SaveVersion = gp.SaveGame.Header.SaveVersion;
      Globals2.GameProperties.SaveGame.Header.GameMode = (GameMode) gameMode;
      Globals2.GameProperties.SaveGame.Header.Attribute = (MapAttribute) attribute;
      Globals2.GameProperties.SaveGame.Header.MapName = gp.SaveGame.Header.MapName;
      Globals2.GameProperties.SaveGame.Header.MapSeed = gp.SaveGame.Header.MapSeed;
      Globals2.GameProperties.SaveGame.Header.OwnerGamerTag = gp.SaveGame.Header.OwnerGamerTag;
      Globals2.GameProperties.SaveGame.Header.TexturePack = gp.SaveGame.Header.TexturePack;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = (StudioForge.TotalMiner.GameDifficulty) difficulty;
      Globals2.GameProperties.SaveGame.Header.PvPCombat = gp.SaveGame.Header.PvPCombat;
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = gp.SaveGame.Header.CombatEnabled;
      Globals2.GameProperties.SaveGame.Header.FiniteMode = gp.SaveGame.Header.FiniteMode;
      Globals2.GameProperties.SaveGame.Header.PassiveMobs = gp.SaveGame.Header.PassiveMobs;
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = gp.SaveGame.Header.EnemyMobs;
      Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath = gp.SaveGame.Header.KeepItemsOnDeath;
      Globals2.GameProperties.SaveGame.Header.DayNightActive = gp.SaveGame.Header.DayNightActive;
      Globals2.GameProperties.SaveGame.Header.WeatherActive = gp.SaveGame.Header.WeatherActive;
      Globals2.GameProperties.SaveGame.Header.WindFactor = gp.SaveGame.Header.WindFactor;
      Globals2.GameProperties.SaveGame.Header.SkillsEnabled = gp.SaveGame.Header.SkillsEnabled;
      Globals2.GameProperties.SaveGame.Header.SkillsLocal = gp.SaveGame.Header.SkillsLocal;
      Globals2.GameProperties.SaveGame.Header.XPMultiplier = gp.SaveGame.Header.XPMultiplier;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.X = gp.SaveGame.Header.TotalMapBound.Min.X;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.Y = gp.SaveGame.Header.TotalMapBound.Min.Y;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Min.Z = gp.SaveGame.Header.TotalMapBound.Min.Z;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.X = gp.SaveGame.Header.TotalMapBound.Max.X;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.Y = gp.SaveGame.Header.TotalMapBound.Max.Y;
      Globals2.GameProperties.SaveGame.Header.TotalMapBound.Max.Z = gp.SaveGame.Header.TotalMapBound.Max.Z;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.X = gp.SaveGame.Header.CurrentMapBound.Min.X;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.Y = gp.SaveGame.Header.CurrentMapBound.Min.Y;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Min.Z = gp.SaveGame.Header.CurrentMapBound.Min.Z;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.X = gp.SaveGame.Header.CurrentMapBound.Max.X;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.Y = gp.SaveGame.Header.CurrentMapBound.Max.Y;
      Globals2.GameProperties.SaveGame.Header.CurrentMapBound.Max.Z = gp.SaveGame.Header.CurrentMapBound.Max.Z;
      Globals2.GameProperties.SaveGame.Header.RegionSize.X = gp.SaveGame.Header.RegionSize.X;
      Globals2.GameProperties.SaveGame.Header.RegionSize.Y = gp.SaveGame.Header.RegionSize.Y;
      Globals2.GameProperties.SaveGame.Header.RegionSize.Z = gp.SaveGame.Header.RegionSize.Z;
      Globals2.GameProperties.SaveGame.Header.ChunkSize.X = gp.SaveGame.Header.ChunkSize.X;
      Globals2.GameProperties.SaveGame.Header.ChunkSize.Y = gp.SaveGame.Header.ChunkSize.Y;
      Globals2.GameProperties.SaveGame.Header.ChunkSize.Z = gp.SaveGame.Header.ChunkSize.Z;
      gp.SaveGame.Header.TerrainData.Biome = (BiomeType) biome;
      Globals2.GameProperties.SaveGame.Header.TerrainData = gp.SaveGame.Header.TerrainData.Clone();
      Globals2.GameProperties.SaveGame.Header.BiomeParams = gp.SaveGame.Header.BiomeParams.Clone();
      Globals2.GameProperties.IsRandomSeed = gp.IsRandomSeed;
      Globals2.MaxConcurrentPlayers = (int) maxConcurrentPlayers;
      Globals2.GameProperties.ShopPoint.X = gp.ShopPoint.X;
      Globals2.GameProperties.ShopPoint.Y = gp.ShopPoint.Y;
      Globals2.GameProperties.ShopPoint.Z = gp.ShopPoint.Z;
      Globals2.GameProperties.ItemIsValid = flagArray;
      this.RaiseGamePropertiesReceived((object) sender, EventArgs.Empty);
      foreach (NetworkGamer tempLocalGamer in this.tempLocalGamerList)
        Globals2.GamertagData.AddServerEntry((Gamer) tempLocalGamer, this.MapID, Globals2.GameProperties.SaveGame.Header.MapName);
    }

    private bool IsValidGamePropertyData(
      GameProperties gp,
      byte gameMode,
      byte attribute,
      byte difficulty,
      byte biome,
      byte maxConcurrentPlayers)
    {
      return gameMode <= (byte) 4 && attribute < (byte) 11 && (difficulty <= (byte) 3 && biome < (byte) 9) && (maxConcurrentPlayers <= (byte) 24 && gp.SaveGame.Header.TotalMapBound.Min.X == 0 && (gp.SaveGame.Header.TotalMapBound.Min.Y == 0 && gp.SaveGame.Header.TotalMapBound.Min.Z == 0)) && (gp.SaveGame.Header.CurrentMapBound.Min.X == 0 && gp.SaveGame.Header.CurrentMapBound.Min.Y == 0 && gp.SaveGame.Header.CurrentMapBound.Min.Z == 0 && ((gp.SaveGame.Header.RegionSize.X == 512 || gp.SaveGame.Header.RegionSize.X == 256) && (gp.SaveGame.Header.RegionSize.Y == 512 || gp.SaveGame.Header.RegionSize.Y == 256))) && ((gp.SaveGame.Header.RegionSize.Z == 512 || gp.SaveGame.Header.RegionSize.Z == 256) && (gp.SaveGame.Header.ChunkSize.X == 32 && gp.SaveGame.Header.ChunkSize.Y == 32) && (gp.SaveGame.Header.ChunkSize.Z == 32 && gp.SaveGame.Header.TerrainData.SeaLevel >= (ushort) 0 && (int) gp.SaveGame.Header.TerrainData.SeaLevel <= gp.SaveGame.Header.MapHeight));
    }

    public void SendGamePropertiesRequest()
    {
      if (!this.IsSessionOpenAndNotLocal || this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 16);
        packetWriter.Write((byte) 16);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadGamePropertiesRequest(NetworkGamer sender)
    {
      if (!this.IsHost || sender == null || sender.IsHost)
        return;
      this.SendGameProperties(sender);
    }

    public void SendGamePropertiesNonVital()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 15);
        packetWriter.Write((byte) 15);
        packetWriter.Write((byte) Globals2.GameProperties.SaveGame.Header.GameDifficulty);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.CombatEnabled);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.FiniteMode);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.SkillsEnabled);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.XPMultiplier);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.PassiveMobs);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.EnemyMobs);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.DayNightActive);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.WeatherActive);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.WindFactor);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.CombatLevelDifference);
        packetWriter.Write(Globals2.GameProperties.SaveGame.Header.ClanProtection);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadGamePropertiesNonVital(NetworkGamer sender)
    {
      bool oldSkillSetting = Globals2.GameProperties.SaveGame.Header.SkillsEnabled && Globals2.GameProperties.SaveGame.Header.FiniteMode;
      Globals2.GameProperties.SaveGame.Header.GameDifficulty = (StudioForge.TotalMiner.GameDifficulty) this.packetReader.ReadByte();
      Globals2.GameProperties.SaveGame.Header.CombatEnabled = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.FiniteMode = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.SkillsEnabled = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.XPMultiplier = this.packetReader.ReadSingle();
      Globals2.GameProperties.SaveGame.Header.PassiveMobs = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.EnemyMobs = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.KeepItemsOnDeath = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.DayNightActive = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.WeatherActive = this.packetReader.ReadBoolean();
      Globals2.GameProperties.SaveGame.Header.WindFactor = this.packetReader.ReadSingle();
      Globals2.GameProperties.SaveGame.Header.CombatLevelDifference = this.packetReader.ReadInt16();
      Globals2.GameProperties.SaveGame.Header.ClanProtection = this.packetReader.ReadBoolean();
      if (this.gameInstance == null)
        return;
      this.gameInstance.SkillSystemChanged(oldSkillSetting);
    }

    public void SendGlobalItemData()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 17);
        packetWriter.Write((byte) 17);
        this.WriteGlobalItemData(packetWriter);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteGlobalItemData(PacketWriter packetWriter)
    {
      packetWriter.Write(Globals1.ItemData.Length);
      foreach (ItemDataXML itemDataXml in Globals1.ItemData)
        packetWriter.Write(itemDataXml.IsEnabled);
    }

    private void ReadGlobalItemData(NetworkGamer sender)
    {
      bool[] flagArray = this.ReadBoolArray();
      for (int index = 0; index < flagArray.Length && index < Globals1.ItemData.Length; ++index)
        Globals1.ItemData[index].IsEnabled = flagArray[index];
    }

    public void SendSlider(GlobalPoint3D p, GamerID playerID, UpdateBlockMethod method)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 81);
        packetWriter.Write((byte) 81);
        packetWriter.Write((short) p.X);
        packetWriter.Write((short) p.Y);
        packetWriter.Write((short) p.Z);
        packetWriter.WriteGamerID(playerID);
        packetWriter.Write((byte) method);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, (NetworkGamer) null);
      }
    }

    private void ReadSlider(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadInt16();
      p.Y = (int) this.packetReader.ReadInt16();
      p.Z = (int) this.packetReader.ReadInt16();
      GamerID playerID = this.packetReader.ReadGamerID();
      UpdateBlockMethod method = (UpdateBlockMethod) this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      this.gameInstance.CreateSliderBlock(p, playerID, method, false);
    }

    public void SendHeal(StudioForge.TotalMiner.Actor healer, StudioForge.TotalMiner.Actor targetPlayer, byte health)
    {
      if (!this.IsSessionOpenAndNotLocal || healer == null || (targetPlayer == null || healer == targetPlayer))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 64);
        packetWriter.Write((byte) 64);
        packetWriter.WriteGamerID(healer.GamerID);
        packetWriter.WriteGamerID(targetPlayer.GamerID);
        packetWriter.Write(health);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadHeal(NetworkGamer sender)
    {
      GamerID gamerID1 = this.packetReader.ReadGamerID();
      GamerID gamerID2 = this.packetReader.ReadGamerID();
      byte num = this.packetReader.ReadByte();
      NetworkGamer gamerById1 = this.FindGamerById(gamerID2);
      StudioForge.TotalMiner.Player player = gamerById1 != null ? gamerById1.Tag as StudioForge.TotalMiner.Player : (StudioForge.TotalMiner.Player) null;
      NetworkGamer gamerById2 = this.FindGamerById(gamerID1);
      StudioForge.TotalMiner.Player healer = gamerById2 != null ? gamerById2.Tag as StudioForge.TotalMiner.Player : (StudioForge.TotalMiner.Player) null;
      player?.HealFromNetwork(healer, (int) num);
    }

    public void SendProjectile(
      Vector3 position,
      Vector3 velocity,
      Item itemID,
      GamerID playerID,
      bool cameFromRemote)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 63);
        packetWriter.Write((byte) 63);
        packetWriter.Write(cameFromRemote);
        packetWriter.WriteGamerID(playerID);
        packetWriter.Write((ushort) itemID);
        packetWriter.Write(new HalfSingle(position.X).PackedValue);
        packetWriter.Write(new HalfSingle(position.Y).PackedValue);
        packetWriter.Write(new HalfSingle(position.Z).PackedValue);
        packetWriter.Write(new HalfSingle(velocity.X).PackedValue);
        packetWriter.Write(new HalfSingle(velocity.Y).PackedValue);
        packetWriter.Write(new HalfSingle(velocity.Z).PackedValue);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadProjectile(NetworkGamer sender)
    {
      bool cameFromRemote = this.packetReader.ReadBoolean();
      GamerID playerID = this.packetReader.ReadGamerID();
      Item itemID = (Item) this.packetReader.ReadUInt16();
      HalfSingle halfSingle = new HalfSingle();
      Vector3 zero1 = Vector3.Zero;
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero1.X = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero1.Y = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero1.Z = halfSingle.ToSingle();
      Vector3 zero2 = Vector3.Zero;
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.X = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.Y = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.Z = halfSingle.ToSingle();
      if (this.gameInstance == null)
        return;
      this.gameInstance.AddProjectile(itemID, zero1, zero2, playerID, cameFromRemote, false);
    }

    public void SendPickupCreateToClients(
      StudioForge.TotalMiner.ParticleType type,
      float age,
      Vector3 position,
      Vector3 velocity,
      float radius,
      InventoryItem item,
      float minPickupAge,
      int particleID,
      GamerID playerID)
    {
      if (!this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 40);
        packetWriter.Write((byte) 40);
        this.WritePickupCreate(packetWriter, type, age, position, velocity, radius, item, minPickupAge, playerID);
        packetWriter.Write(particleID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendPickupCreateToHost(
      StudioForge.TotalMiner.ParticleType type,
      float age,
      Vector3 position,
      Vector3 velocity,
      float radius,
      InventoryItem item,
      float minPickupAge,
      GamerID playerID)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 40);
        packetWriter.Write((byte) 40);
        this.WritePickupCreate(packetWriter, type, age, position, velocity, radius, item, minPickupAge, playerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void WritePickupCreate(
      PacketWriter packetWriter,
      StudioForge.TotalMiner.ParticleType type,
      float age,
      Vector3 position,
      Vector3 velocity,
      float radius,
      InventoryItem item,
      float minPickupAge,
      GamerID playerID)
    {
      packetWriter.Write((byte) type);
      packetWriter.Write(position.X);
      packetWriter.Write(position.Y);
      packetWriter.Write(position.Z);
      packetWriter.Write(new HalfSingle(velocity.X).PackedValue);
      packetWriter.Write(new HalfSingle(velocity.Y).PackedValue);
      packetWriter.Write(new HalfSingle(velocity.Z).PackedValue);
      packetWriter.Write(new HalfSingle(age).PackedValue);
      packetWriter.Write(new HalfSingle(radius).PackedValue);
      packetWriter.Write(new HalfSingle(minPickupAge).PackedValue);
      packetWriter.Write((ushort) item.ItemID);
      packetWriter.Write(item.Durability);
      packetWriter.Write(item.Count);
      packetWriter.WriteGamerID(playerID);
    }

    private void ReadPickupCreate(NetworkGamer sender)
    {
      StudioForge.TotalMiner.ParticleType type = (StudioForge.TotalMiner.ParticleType) this.packetReader.ReadByte();
      Vector3 zero1 = Vector3.Zero;
      zero1.X = this.packetReader.ReadSingle();
      zero1.Y = this.packetReader.ReadSingle();
      zero1.Z = this.packetReader.ReadSingle();
      Vector3 zero2 = Vector3.Zero;
      HalfSingle halfSingle = new HalfSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.X = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.Y = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      zero2.Z = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      float single1 = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      float single2 = halfSingle.ToSingle();
      halfSingle.PackedValue = this.packetReader.ReadUInt16();
      float single3 = halfSingle.ToSingle();
      Item itemID = (Item) this.packetReader.ReadUInt16();
      ushort durability = this.packetReader.ReadUInt16();
      int count = this.packetReader.ReadInt32();
      InventoryItem inventoryItem = new InventoryItem(itemID, count, durability);
      GamerID playerID = this.packetReader.ReadGamerID();
      if (this.IsHost)
      {
        if (this.gameInstance == null)
          return;
        this.gameInstance.AddPickupFromClient(type, single1, zero1, zero2, single2, inventoryItem, single3, playerID, true);
      }
      else
      {
        int particleID = this.packetReader.ReadInt32();
        if (this.gameInstance == null)
          return;
        this.gameInstance.AddPickupFromHost(type, single1, zero1, zero2, single2, inventoryItem, single3, particleID, playerID);
      }
    }

    public void SendPickupRequest(StudioForge.TotalMiner.Player player, int particleID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      lock (this.pickupRequests)
        this.pickupRequests.Add(new NetworkManager.PickupRequest()
        {
          GamerId = player.Gamer.ID,
          ParticleID = particleID
        });
    }

    private void SendPickupRequests()
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      lock (this.pickupRequests)
      {
        if (this.pickupRequests.Count <= 0)
          return;
        PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, this.session.Host);
        if (packetWriter != null)
        {
          lock (packetWriter)
          {
            packetWriter.Write((byte) 41);
            packetWriter.Write((byte) 41);
            packetWriter.Write((ushort) this.pickupRequests.Count);
            foreach (NetworkManager.PickupRequest pickupRequest in this.pickupRequests)
            {
              packetWriter.WriteGamerID(pickupRequest.GamerId);
              packetWriter.Write(pickupRequest.ParticleID);
            }
            this.EndPacketWriter(packetWriter, SendDataOptions.None, this.session.Host);
          }
        }
        this.pickupRequests.Clear();
      }
    }

    private void ReadPickupRequest(NetworkGamer sender)
    {
      int num = (int) this.packetReader.ReadUInt16();
      for (int index = 0; index < num; ++index)
      {
        GamerID gamerID = this.packetReader.ReadGamerID();
        int particleID = this.packetReader.ReadInt32();
        if (this.IsHost && this.gameInstance != null && this.gameInstance.ConfirmPickup(particleID))
        {
          this.SendPickupConfirm(gamerID, particleID);
          this.gameInstance.FinalizePickup(gamerID, particleID);
        }
      }
    }

    public void SendPickupConfirm(GamerID gamerID, int particleID)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 42);
        packetWriter.Write((byte) 42);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write(particleID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadPickupConfirm(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      int particleID = this.packetReader.ReadInt32();
      if (this.IsHost || this.gameInstance == null)
        return;
      this.gameInstance.FinalizePickup(gamerID, particleID);
    }

    public void SendOpenBlockRequest(GlobalPoint3D p, Block blockID, GamerID gamerID)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 37);
        packetWriter.Write((byte) 37);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.Write((byte) blockID);
        packetWriter.WriteGamerID(gamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, this.session.Host);
      }
    }

    public void SendOpenBlockConfirm(
      GlobalPoint3D p,
      Block blockID,
      GamerID gamerID,
      bool success)
    {
      if (!this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 38);
        packetWriter.Write((byte) 38);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.Write((byte) blockID);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write(success);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, gamerById);
      }
    }

    private void ReadOpenBlockRequest(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      Block blockID = (Block) this.packetReader.ReadByte();
      GamerID gamerID = this.packetReader.ReadGamerID();
      if (!this.IsHost || this.gameInstance == null)
        return;
      bool success = false;
      if (this.gameInstance.IsBlockOpen(p) == gamerID)
        success = this.gameInstance.FlagBlockIsOpen(p, blockID, gamerID);
      this.SendOpenBlockConfirm(p, blockID, gamerID, success);
    }

    private void ReadOpenBlockConfirm(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      Block blockID = (Block) this.packetReader.ReadByte();
      GamerID gamerID = this.packetReader.ReadGamerID();
      bool success = this.packetReader.ReadBoolean();
      if (this.IsHost || this.gameInstance == null)
        return;
      StudioForge.TotalMiner.Player player = this.GetPlayer(gamerID);
      if (player == null)
        return;
      this.gameInstance.OpenBlockConfirmation(player, p, blockID, success);
    }

    public void SendCloseBlock(GamerID gamerID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 39);
        packetWriter.Write((byte) 39);
        packetWriter.WriteGamerID(gamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadCloseBlock(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      this.gameInstance.FlagBlockIsClosed(gamerID, false);
    }

    public void SendDataBlockInfoRequest(
      MapChunk chunk,
      List<int> blockIndexes,
      NetworkGamer target,
      bool resultToAll)
    {
      if (target == null || blockIndexes.Count <= 0 || (!this.IsSessionOpenAndNotLocal || target.Machine.Gamers.Contains(this.session.LocalGamers[0])))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, target);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 27);
        packetWriter.Write((byte) 27);
        packetWriter.Write(resultToAll);
        packetWriter.Write(chunk.GetGlobalHashCode());
        packetWriter.Write((ushort) blockIndexes.Count);
        foreach (int blockIndex in blockIndexes)
          packetWriter.Write((ushort) blockIndex);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, target);
      }
    }

    private void ReadDataBlockInfoRequest(NetworkGamer sender)
    {
      bool flag = this.packetReader.ReadBoolean();
      long hash = this.packetReader.ReadInt64();
      int num = (int) this.packetReader.ReadUInt16();
      if (num <= 0)
        return;
      this.blockIndexes.Clear();
      for (int index = 0; index < num; ++index)
        this.blockIndexes.Add((int) this.packetReader.ReadUInt16());
      this.SendDataBlockInfo(this.gameInstance.Map.GetChunk(hash), this.blockIndexes, flag ? (NetworkGamer) null : sender);
      this.blockIndexes.Clear();
    }

    public void SendDataBlockInfo(MapChunk chunk, List<int> blockIndexes, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || chunk == null || (blockIndexes.Count <= 0 || this.gameInstance == null) || this.gameInstance.Map == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      this.dataBlockTempList.Clear();
      this.GetDataBlocksToSend(chunk, blockIndexes, this.dataBlockTempList);
      lock (packetWriter)
      {
        packetWriter.Write((byte) 28);
        packetWriter.Write((byte) 28);
        packetWriter.Write(chunk.GetGlobalHashCode());
        packetWriter.Write((ushort) this.dataBlockTempList.Count);
        foreach (DataBlock dataBlockTemp in this.dataBlockTempList)
        {
          packetWriter.Write((ushort) chunk.GetMapIndex(dataBlockTemp.Point));
          packetWriter.Write((byte) dataBlockTemp.ClassType);
          dataBlockTemp.WriteState((BinaryWriter) packetWriter);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
      this.dataBlockTempList.Clear();
    }

    private void GetDataBlocksToSend(
      MapChunk chunk,
      List<int> blockIndexes,
      List<DataBlock> result)
    {
      MapStrategyTM mapStrategyTm = this.gameInstance.MapStrategyTM;
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      GlobalPoint3D p = new GlobalPoint3D();
      Point3D point3D = new Point3D();
      result.Clear();
      foreach (int blockIndex in blockIndexes)
      {
        Point3D point = chunk.GetPoint(blockIndex);
        p.X = globalOffset.X + point.X;
        p.Y = globalOffset.Y + point.Y;
        p.Z = globalOffset.Z + point.Z;
        DataBlock dataBlock = mapStrategyTm.GetDataBlock(p) ?? (DataBlock) new DummyDataBlock();
        result.Add(dataBlock);
      }
    }

    private void ReadDataBlockInfo(NetworkGamer sender)
    {
      this.blockIndexes.Clear();
      this.dataBlockTempList.Clear();
      long hash = this.packetReader.ReadInt64();
      int num1 = (int) this.packetReader.ReadUInt16();
      if (num1 <= 0)
        return;
      MapStrategyTM mapStrategyTm = this.gameInstance.MapStrategyTM;
      MapTM map = this.gameInstance.Map;
      MapChunk chunk = map.GetChunk(hash);
      GlobalPoint3D globalOffset = chunk.GlobalOffset;
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      for (int index = 0; index < num1; ++index)
      {
        ushort num2 = this.packetReader.ReadUInt16();
        DataBlockType type = (DataBlockType) this.packetReader.ReadByte();
        GlobalPoint3D globalPoint = chunk.GetGlobalPoint(chunk.GetPoint((int) num2));
        if (type == DataBlockType.None)
        {
          mapStrategyTm.RemoveDataBlock(map.GetGlobalHashCode(globalPoint));
        }
        else
        {
          bool flag = false;
          DataBlock dataBlock = mapStrategyTm.GetDataBlock(globalPoint);
          if (dataBlock == null)
          {
            flag = true;
            dataBlock = MapSerializer.CreateDataBlock(type);
          }
          dataBlock.ReadState((BinaryReader) this.packetReader, 294);
          if (flag)
            mapStrategyTm.AddDataBlock(dataBlock, UpdateBlockMethod.Strategy, true);
          switch (type)
          {
            case DataBlockType.Sign:
              this.gameInstance.MapRenderer.SignsChanged(true);
              break;
            case DataBlockType.WifiReceiver:
            case DataBlockType.WifiTransmitter:
              if (!this.IsHost && !sender.IsHost)
              {
                this.blockIndexes.Add((int) num2);
                break;
              }
              if (this.IsHost)
              {
                this.dataBlockTempList.Add(dataBlock);
                break;
              }
              break;
            case DataBlockType.Blueprint:
              this.gameInstance.BlueprintLoaded(dataBlock as BlueprintBlock);
              break;
            case DataBlockType.WisdomScroll:
              this.gameInstance.WisdomScrollLoaded(dataBlock as WisdomScrollBlock);
              break;
          }
        }
        if (this.blockIndexes.Count > 0)
          this.SendDataBlockInfoRequest(chunk, this.blockIndexes, this.session.Host, true);
      }
      if (this.IsHost)
        map.UpdateWifiFrequencies(this.dataBlockTempList);
      this.dataBlockTempList.Clear();
    }

    public void SendDataBlockChange(DataBlock block, bool isClosed, UpdateBlockMethod method)
    {
      NetworkManager.DataBlockChange dataBlockChange = new NetworkManager.DataBlockChange() { DataBlock = block, IsClosed = isClosed, Method = method };
      lock (this.dataBlockChangeList)
        this.dataBlockChangeList.Add(dataBlockChange);
    }

    private void SendDataBlockChanges()
    {
      lock (this.dataBlockChangeList)
      {
        this.SendDataBlockChanges(this.dataBlockChangeList);
        this.dataBlockChangeList.Clear();
      }
    }

    public void SendDataBlockChanges(List<NetworkManager.DataBlockChange> blocks)
    {
      if (!this.IsSessionOpenAndNotLocal || blocks == null || (blocks.Count <= 0 || this.gameInstance == null) || this.gameInstance.Map == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 29);
        packetWriter.Write((byte) 29);
        packetWriter.Write(blocks.Count);
        foreach (NetworkManager.DataBlockChange block in blocks)
        {
          packetWriter.Write(block.IsClosed);
          packetWriter.Write((byte) block.Method);
          packetWriter.Write((byte) block.DataBlock.ClassType);
          packetWriter.Write(this.gameInstance.Map.GetGlobalHashCode(block.DataBlock.Point));
          block.DataBlock.WriteState((BinaryWriter) packetWriter);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadDataBlockChange(NetworkGamer sender)
    {
      int num = this.packetReader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        bool flag1 = this.packetReader.ReadBoolean();
        UpdateBlockMethod method = (UpdateBlockMethod) this.packetReader.ReadByte();
        DataBlockType type = (DataBlockType) this.packetReader.ReadByte();
        long hash = this.packetReader.ReadInt64();
        MapStrategyTM mapStrategyTm = this.gameInstance != null ? this.gameInstance.MapStrategyTM : (MapStrategyTM) null;
        if (mapStrategyTm != null)
        {
          GlobalPoint3D pointFromGlobalHash = this.gameInstance.Map.GetPointFromGlobalHash(hash);
          DataBlock dataBlock = mapStrategyTm.GetDataBlock(pointFromGlobalHash);
          bool flag2 = dataBlock == null || dataBlock.ClassType != type;
          if (flag2)
            dataBlock = MapSerializer.CreateDataBlock(type);
          dataBlock.ReadState((BinaryReader) this.packetReader, 294);
          if (flag2)
            mapStrategyTm.AddDataBlock(dataBlock, method);
          if (flag1)
            this.gameInstance.FlagBlockIsClosed(hash);
          this.ProcessDataBlockSpecificChange(dataBlock);
        }
        else
          MapSerializer.CreateDataBlock(type).ReadState((BinaryReader) this.packetReader, 294);
      }
    }

    private void ProcessDataBlockSpecificChange(DataBlock block)
    {
      if (block == null || this.gameInstance == null)
        return;
      switch (block.ClassType)
      {
        case DataBlockType.Furnace:
          FurnaceBlock furnaceBlock = block as FurnaceBlock;
          furnaceBlock.Map = (Map) this.gameInstance.Map;
          furnaceBlock.GetProduct();
          if (!furnaceBlock.HasFuel)
            break;
          furnaceBlock.Raise_FurnaceBurnStarted();
          break;
        case DataBlockType.AmbientSound:
          this.gameInstance.AmbientSoundManager.SetBlock(block as AmbientSoundBlock);
          break;
      }
    }

    public void SendDataBlockRemove(DataBlock block)
    {
      if (!this.IsSessionOpenAndNotLocal || this.gameInstance == null || this.gameInstance.Map == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 30);
        packetWriter.Write((byte) 30);
        packetWriter.Write(this.gameInstance.Map.GetGlobalHashCode(block.Point));
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadDataBlockRemove(NetworkGamer sender)
    {
      long hash = this.packetReader.ReadInt64();
      MapStrategyTM mapStrategyTm = this.gameInstance != null ? this.gameInstance.MapStrategyTM : (MapStrategyTM) null;
      if (mapStrategyTm == null)
        return;
      mapStrategyTm.RemoveDataBlock(hash);
      this.gameInstance.FlagBlockIsClosed(hash);
    }

    public void SendDoorChangeConfirm(GlobalPoint3D p1, GlobalPoint3D p2, byte auxData)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 70);
        packetWriter.Write((byte) 70);
        this.WriteDoorChange(packetWriter, p1, p2, auxData);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendTrapDoorChangeConfirm(GlobalPoint3D p, byte auxData)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 71);
        packetWriter.Write((byte) 71);
        this.WriteTrapDoorChange(packetWriter, p, auxData);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteDoorChange(
      PacketWriter packetWriter,
      GlobalPoint3D p1,
      GlobalPoint3D p2,
      byte auxData)
    {
      packetWriter.Write((ushort) p1.X);
      packetWriter.Write((ushort) p1.Y);
      packetWriter.Write((ushort) p1.Z);
      packetWriter.Write((ushort) p2.X);
      packetWriter.Write((ushort) p2.Y);
      packetWriter.Write((ushort) p2.Z);
      packetWriter.Write(auxData);
    }

    private void WriteTrapDoorChange(PacketWriter packetWriter, GlobalPoint3D p, byte auxData)
    {
      packetWriter.Write((ushort) p.X);
      packetWriter.Write((ushort) p.Y);
      packetWriter.Write((ushort) p.Z);
      packetWriter.Write(auxData);
    }

    private void ReadDoorChangeConfirm(NetworkGamer sender)
    {
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D();
      globalPoint3D1.X = (int) this.packetReader.ReadUInt16();
      globalPoint3D1.Y = (int) this.packetReader.ReadUInt16();
      globalPoint3D1.Z = (int) this.packetReader.ReadUInt16();
      GlobalPoint3D globalPoint3D2 = new GlobalPoint3D();
      globalPoint3D2.X = (int) this.packetReader.ReadUInt16();
      globalPoint3D2.Y = (int) this.packetReader.ReadUInt16();
      globalPoint3D2.Z = (int) this.packetReader.ReadUInt16();
      byte aux = this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      this.gameInstance.HitDoorCore(globalPoint3D1, globalPoint3D2, aux, (StudioForge.TotalMiner.Actor) null);
      this.gameInstance.MapStrategyTM.TogglePowerReceipt(globalPoint3D1);
      this.gameInstance.MapStrategyTM.TogglePowerReceipt(globalPoint3D2);
      this.gameInstance.Map.Commit();
    }

    private void ReadTrapDoorChangeConfirm(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      byte aux = this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      this.gameInstance.HitDoorCore(p, aux, (StudioForge.TotalMiner.Actor) null);
      this.gameInstance.Map.Commit();
    }

    public void SendPowerDeliver(
      GlobalPoint3D p,
      Block blockID,
      BlockFace face,
      bool power,
      UpdateBlockMethod method,
      GamerID playerID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.IsHost ? (NetworkGamer) null : this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 72);
        packetWriter.Write((byte) 72);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.Write(power);
        packetWriter.Write((byte) blockID);
        packetWriter.Write((byte) face);
        packetWriter.Write((byte) method);
        packetWriter.WriteGamerID(playerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadPowerDeliver(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      bool power = this.packetReader.ReadBoolean();
      Block blockID = (Block) this.packetReader.ReadByte();
      BlockFace face = (BlockFace) this.packetReader.ReadByte();
      UpdateBlockMethod method = (UpdateBlockMethod) this.packetReader.ReadByte();
      GamerID playerID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      this.gameInstance.DeliverPower(p, blockID, face, power, method, playerID, this.IsHost, false);
    }

    public void SendTransmitterFrequency(GlobalPoint3D p, GamerID playerID, ushort frequency)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 89);
        packetWriter.Write((byte) 89);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.WriteGamerID(playerID);
        packetWriter.Write(frequency);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadWifiTransmitterFrequency(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      GamerID playerID = this.packetReader.ReadGamerID();
      ushort frequency = this.packetReader.ReadUInt16();
      if (this.gameInstance == null || this.gameInstance.MapStrategyTM == null)
        return;
      this.gameInstance.MapStrategyTM.UpdateWifiTransmitterFrequency(p, playerID, frequency);
    }

    public void SendSignText(GlobalPoint3D p, string text)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 74);
        packetWriter.Write((byte) 74);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.Write(text);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadSignText(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      string text = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.AddSignFromRemote(p, text);
    }

    public void SendTopMapMarkerUpdate(GlobalPoint3D p, string text, MapMarkerType type)
    {
      if (!this.IsSessionOpenAndNotLocal || text == null || text.Length <= 0)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 95);
        packetWriter.Write((byte) 95);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        packetWriter.Write((byte) type);
        packetWriter.Write(text);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadTopMapMarkerUpdate(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      MapMarkerType type = (MapMarkerType) this.packetReader.ReadByte();
      string text = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.AddMapMarker(p, text, type, false);
    }

    public void SendTopMapMarkerRemove(GlobalPoint3D p)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 96);
        packetWriter.Write((byte) 96);
        packetWriter.Write((ushort) p.X);
        packetWriter.Write((ushort) p.Y);
        packetWriter.Write((ushort) p.Z);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadTopMapMarkerRemove(NetworkGamer sender)
    {
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      if (this.gameInstance == null)
        return;
      this.gameInstance.RemoveMapMarker(p, false);
    }

    public void SendInventory(StudioForge.TotalMiner.Player player)
    {
      if (!this.IsSessionOpenAndNotLocal || this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 53);
        packetWriter.Write((byte) 53);
        packetWriter.WriteGamerID((Gamer) player.Gamer);
        this.WriteInventoryCore(packetWriter, (Inventory) player.Inventory);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadInventory(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      int num = 1;
      if (this.gameInstance != null)
      {
        StudioForge.TotalMiner.Player player = this.gameInstance.GetPlayer(gamerID);
        if (player != null)
        {
          this.ReadInventoryCore(this.packetReader, (Inventory) player.Inventory);
          --num;
          player.OnRemoteInventoryUpdated();
        }
      }
      for (int index = 0; index < num; ++index)
        this.ReadInventoryCoreDummy(this.packetReader);
    }

    private void WriteInventoryCore(PacketWriter writer, Inventory inventory)
    {
      byte num = 0;
      for (int index = 0; index < inventory.Count; ++index)
      {
        if (inventory[index].ItemID != Item.None || inventory.AllowZeroCountItems && inventory[index].ItemID_Raw != Item.None)
          ++num;
      }
      writer.Write(inventory.AllowZeroCountItems);
      writer.Write(num);
      for (int index = 0; index < inventory.Count; ++index)
      {
        InventoryItem inventoryItem = inventory[index];
        if (inventoryItem.ItemID != Item.None || inventory.AllowZeroCountItems && inventoryItem.ItemID_Raw != Item.None)
          this.WriteInventoryItemCore(writer, (byte) index, inventoryItem);
      }
    }

    private void WriteInventoryItemCore(PacketWriter writer, byte slotID, InventoryItem item)
    {
      writer.Write(slotID);
      writer.Write((ushort) item.ItemID_Raw);
      writer.Write(item.Count);
      writer.Write(item.Durability);
    }

    private void ReadInventoryCore(PacketReader reader, Inventory inventory)
    {
      inventory.AllowZeroCountItems = reader.ReadBoolean();
      int num1 = (int) reader.ReadByte();
      inventory.ClearItems();
      InventoryItem inventoryItem = new InventoryItem();
      for (int index = 0; index < num1; ++index)
      {
        byte num2 = this.ReadInventoryItemCore(reader, out inventoryItem);
        inventory[(int) num2] = inventoryItem;
      }
    }

    private byte ReadInventoryItemCore(PacketReader reader, out InventoryItem item)
    {
      byte num = reader.ReadByte();
      item = new InventoryItem()
      {
        ItemID = (Item) reader.ReadUInt16(),
        Count = reader.ReadInt32(),
        Durability = reader.ReadUInt16()
      };
      return num;
    }

    private void ReadInventoryCoreDummy(PacketReader reader)
    {
      reader.ReadBoolean();
      int num1 = (int) reader.ReadByte();
      InventoryItem inventoryItem = new InventoryItem();
      for (int index = 0; index < num1; ++index)
      {
        int num2 = (int) this.ReadInventoryItemCore(reader, out inventoryItem);
      }
    }

    public void SendInventoryChanged(StudioForge.TotalMiner.Player player, Inventory inventory)
    {
      if (!this.IsSessionOpenAndNotLocal || player == null || (inventory == null || !inventory.HasItemsChanged) || inventory.ItemsChanged.Count <= 0)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 54);
        packetWriter.Write((byte) 54);
        packetWriter.WriteGamerID(player.GamerID);
        this.WriteInventoryChangedCore(packetWriter, inventory);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteInventoryChangedCore(PacketWriter writer, Inventory inventory)
    {
      writer.Write((ushort) inventory.ItemsChanged.Count);
      foreach (short num in inventory.ItemsChanged)
        this.WriteInventoryItemCore(writer, (byte) num, inventory[(int) num]);
    }

    private void ReadInventoryChanged(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      this.tempInvChangedItemList.Clear();
      this.tempInvChangedSlotIDList.Clear();
      int num = (int) this.packetReader.ReadUInt16();
      for (int index = 0; index < num; ++index)
      {
        InventoryItem inventoryItem;
        this.tempInvChangedSlotIDList.Add((short) this.ReadInventoryItemCore(this.packetReader, out inventoryItem));
        this.tempInvChangedItemList.Add(inventoryItem);
      }
      if (this.gameInstance == null)
        return;
      StudioForge.TotalMiner.Player player = this.gameInstance.GetPlayer(gamerID);
      if (player == null || player.Inventory == null || player.IsLocalGamer)
        return;
      player.Inventory.UpdateChanges(this.tempInvChangedSlotIDList, this.tempInvChangedItemList);
      player.OnRemoteInventoryUpdated();
    }

    public void SendPriceChange(
      StudioForge.TotalMiner.Player player,
      ShopBlock shopBlock,
      PriceList.PriceListType type,
      Item itemID,
      PriceList.Price oldPrice,
      PriceList.Price newPrice)
    {
      if (newPrice.Equals(oldPrice) || (type == PriceList.PriceListType.PlayerShop || type == PriceList.PriceListType.SystemShop) && shopBlock == null || type == PriceList.PriceListType.PlayerDefault && player == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 85);
        packetWriter.Write((byte) 85);
        packetWriter.Write((byte) type);
        switch (type)
        {
          case PriceList.PriceListType.PlayerDefault:
            packetWriter.WriteGamerID(player.GamerID);
            break;
          case PriceList.PriceListType.PlayerShop:
          case PriceList.PriceListType.SystemShop:
            packetWriter.Write((ushort) shopBlock.Point.X);
            packetWriter.Write((ushort) shopBlock.Point.Y);
            packetWriter.Write((ushort) shopBlock.Point.Z);
            break;
        }
        packetWriter.Write((ushort) itemID);
        this.WritePrice(packetWriter, newPrice);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WritePrice(PacketWriter packetWriter, PriceList.Price price)
    {
      packetWriter.Write(price.Buy);
      packetWriter.Write(price.Sell);
      packetWriter.Write((byte) price.Perc);
      packetWriter.Write(price.UsePerc);
      packetWriter.Write(price.ForSale);
    }

    private void ReadPriceChange(NetworkGamer sender)
    {
      PriceList.PriceListType type = (PriceList.PriceListType) this.packetReader.ReadByte();
      GlobalPoint3D p = new GlobalPoint3D();
      GamerID gamerID = GamerID.Sys1;
      switch (type)
      {
        case PriceList.PriceListType.PlayerDefault:
          gamerID = this.packetReader.ReadGamerID();
          break;
        case PriceList.PriceListType.PlayerShop:
        case PriceList.PriceListType.SystemShop:
          p.X = (int) this.packetReader.ReadUInt16();
          p.Y = (int) this.packetReader.ReadUInt16();
          p.Z = (int) this.packetReader.ReadUInt16();
          break;
      }
      int index = (int) this.packetReader.ReadUInt16();
      PriceList.Price price = this.ReadPrice(this.packetReader);
      StudioForge.TotalMiner.Player player = this.GetPlayer(gamerID);
      PriceList priceList = this.GetPriceList(type, player, p);
      if (priceList == null || priceList.Prices == null || (index < 0 || index >= priceList.Prices.Length))
        return;
      priceList.Prices[index] = price;
    }

    private PriceList.Price ReadPrice(PacketReader packetReader)
    {
      return new PriceList.Price() { Buy = packetReader.ReadInt32(), Sell = packetReader.ReadInt32(), Perc = (int) packetReader.ReadByte(), UsePerc = packetReader.ReadBoolean(), ForSale = packetReader.ReadBoolean() };
    }

    private PriceList GetPriceList(
      PriceList.PriceListType type,
      StudioForge.TotalMiner.Player player,
      GlobalPoint3D p)
    {
      PriceList priceList = (PriceList) null;
      if (type == PriceList.PriceListType.PlayerDefault)
      {
        if (player != null)
        {
          if (player.DefaultPriceList == null)
            player.DefaultPriceList = new PriceList(PriceList.PriceListType.PlayerDefault);
          priceList = player.DefaultPriceList;
        }
      }
      else
      {
        ShopBlock dataBlock = this.gameInstance.MapStrategyTM.GetDataBlock(p) as ShopBlock;
        if (dataBlock != null)
          priceList = dataBlock.PriceList;
      }
      return priceList;
    }

    public void SendPriceList(
      NetworkManager.PriceListChangeType changeType,
      StudioForge.TotalMiner.Player player,
      ShopBlock shopBlock)
    {
      if (player == null || shopBlock == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 86);
        packetWriter.Write((byte) 86);
        packetWriter.Write((byte) changeType);
        packetWriter.WriteGamerID(player.GamerID);
        packetWriter.Write((ushort) shopBlock.Point.X);
        packetWriter.Write((ushort) shopBlock.Point.Y);
        packetWriter.Write((ushort) shopBlock.Point.Z);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadPriceList(NetworkGamer sender)
    {
      NetworkManager.PriceListChangeType priceListChangeType = (NetworkManager.PriceListChangeType) this.packetReader.ReadByte();
      GamerID gamerID = this.packetReader.ReadGamerID();
      GlobalPoint3D p = new GlobalPoint3D();
      p.X = (int) this.packetReader.ReadUInt16();
      p.Y = (int) this.packetReader.ReadUInt16();
      p.Z = (int) this.packetReader.ReadUInt16();
      StudioForge.TotalMiner.Player player = this.GetPlayer(gamerID);
      ShopBlock shopBlock = (ShopBlock) null;
      MapStrategyTM mapStrategy = this.gameInstance.Map.MapStrategy as MapStrategyTM;
      if (mapStrategy != null)
        shopBlock = mapStrategy.GetDataBlock(p) as ShopBlock;
      if (player == null || shopBlock == null)
        return;
      switch (priceListChangeType)
      {
        case NetworkManager.PriceListChangeType.ShopUsesDefault:
          shopBlock.PriceList = (PriceList) null;
          break;
        case NetworkManager.PriceListChangeType.ShopCopyOfDefault:
          shopBlock.PriceList = new PriceList(PriceList.PriceListType.PlayerShop, player.DefaultPriceList);
          break;
        case NetworkManager.PriceListChangeType.DefaultCopyOfShop:
          player.DefaultPriceList = new PriceList(PriceList.PriceListType.PlayerDefault, shopBlock.PriceList);
          break;
      }
    }

    public void SendPlayerSettings(StudioForge.TotalMiner.Player player, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || player == null || player.Settings == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 60);
        packetWriter.Write((byte) 60);
        packetWriter.WriteGamerID(player.GamerID);
        this.WritePlayerSettingsCore(packetWriter, player.Settings);
        packetWriter.Write((byte) player.ActorType);
        packetWriter.Write(player.IsGod);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    public void SendPlayerSettingsRequest()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 61);
        packetWriter.Write((byte) 61);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadPlayerSettingsRequest(NetworkGamer sender)
    {
      foreach (Gamer tempLocalGamer in this.tempLocalGamerList)
        this.SendPlayerSettings(tempLocalGamer.Tag as StudioForge.TotalMiner.Player, sender);
    }

    private void ReadPlayerSettings(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      bool flag = false;
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById != null)
      {
        StudioForge.TotalMiner.Player tag = gamerById.Tag as StudioForge.TotalMiner.Player;
        if (tag != null)
        {
          PlayerSettings settings = tag.Settings;
          this.ReadPlayerSettingsCore(this.packetReader, tag.Settings);
          tag.SetAvatar(tag, (StudioForge.TotalMiner.ActorType) this.packetReader.ReadByte());
          tag.IsGod = this.packetReader.ReadBoolean();
          if (tag.WieldType != settings.WieldType)
            tag.OnWieldTypeChanged(settings.WieldType);
          flag = true;
        }
      }
      if (flag)
        return;
      this.ReadPlayerSettingsCore(this.packetReader, new PlayerSettings());
      int num = (int) this.packetReader.ReadByte();
      this.packetReader.ReadBoolean();
    }

    private void WritePlayerSettingsCore(PacketWriter writer, PlayerSettings settings)
    {
      writer.Write(settings.BlueprintFinderVisible);
      writer.Write(settings.GamePadSensitivity);
      writer.Write(settings.MouseSensitivity);
      writer.Write(settings.FOVNormalized);
      writer.Write(settings.HudVisible);
      writer.Write(settings.InvertY);
      writer.Write(settings.MapVisible);
      writer.Write((byte) settings.Nameplates);
      writer.Write(settings.DisplayXPGains);
      writer.Write(settings.RumbleOn);
      writer.Write(settings.MobNameplates);
      writer.Write(settings.AutoplaceTime);
      writer.Write(settings.HotBarToTransparentTime);
      writer.Write((byte) settings.WieldType);
      writer.Write((byte) settings.CameraType);
      writer.Write((byte) settings.UserControlSetting);
    }

    private void ReadPlayerSettingsCore(PacketReader reader, PlayerSettings settings)
    {
      settings.BlueprintFinderVisible = reader.ReadBoolean();
      settings.GamePadSensitivity = reader.ReadSingle();
      settings.MouseSensitivity = reader.ReadSingle();
      settings.FOVNormalized = reader.ReadSingle();
      settings.HudVisible = reader.ReadBoolean();
      settings.InvertY = reader.ReadBoolean();
      settings.MapVisible = reader.ReadBoolean();
      settings.Nameplates = (NamePlateSetting) reader.ReadByte();
      settings.DisplayXPGains = reader.ReadBoolean();
      settings.RumbleOn = reader.ReadBoolean();
      settings.MobNameplates = reader.ReadBoolean();
      settings.AutoplaceTime = reader.ReadSingle();
      settings.HotBarToTransparentTime = reader.ReadByte();
      settings.WieldType = (WieldType) reader.ReadByte();
      settings.CameraType = (CameraType) reader.ReadByte();
      settings.UserControlSetting = (UserControlSetting) reader.ReadByte();
    }

    public void SendPlayerStatisticsRequest(int index)
    {
      if (index < 0 || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 56);
        packetWriter.Write((byte) 56);
        packetWriter.Write(index);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, this.session.Host);
      }
    }

    private void ReadPlayerStatisticsRequest(NetworkGamer sender)
    {
      int index = this.packetReader.ReadInt32();
      if (!this.IsHost || index < 0 || index >= this.gameInstance.PlayerSaves.Count)
        return;
      SavePlayerState playerSave = this.gameInstance.PlayerSaves[index];
      this.SendPlayerStatistics(index, playerSave.Gamertag, playerSave.Statistics, sender);
    }

    public void SendPlayerStatistics(
      int index,
      string gamertag,
      PlayerStats stats,
      NetworkGamer recipient)
    {
      if (index < 0 || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 55);
        packetWriter.Write((byte) 55);
        packetWriter.Write(index);
        this.WritePlayerStatsCore(packetWriter, gamertag, stats);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, recipient);
      }
    }

    private void ReadPlayerStatistics(NetworkGamer sender)
    {
      int index = this.packetReader.ReadInt32();
      if (this.gameInstance != null && index < this.gameInstance.PlayerSaves.Count && this.gameInstance.PlayerSaves[index] != null)
      {
        string gamertag = this.ReadPlayerStatsCore(this.packetReader, this.GameInstance.PlayerSaves[index].Statistics);
        this.gameInstance.PlayerSaves[index].Gamertag = gamertag;
        StudioForge.TotalMiner.Player player = this.gameInstance.GetPlayer(gamertag);
        if (player != null && !player.IsLocalGamer)
          player.Statistics.Merge(this.GameInstance.PlayerSaves[index].Statistics);
        this.RaisePlayerStatsReceived((object) this, index);
      }
      else
        this.ReadPlayerStatsCore(this.packetReader, new PlayerStats());
    }

    private void WritePlayerStatsCore(PacketWriter writer, string gamertag, PlayerStats stats)
    {
      writer.Write(gamertag);
      writer.Write(stats.SecondsPlayed);
      writer.Write(stats.DistanceWalked);
      writer.Write(stats.DistanceFlown);
      writer.Write(stats.BlocksCleared);
      writer.Write(stats.BlocksPlaced);
      writer.Write(stats.BlocksPickedUp);
      writer.Write(stats.ItemsPickedUp);
      writer.Write(stats.DamageDealt);
      writer.Write(stats.DamageTaken);
      writer.Write(stats.TotalDeaths);
      writer.Write(stats.PlayerKills);
      writer.Write(stats.NPCKills);
      writer.Write(stats.LootValue);
      writer.Write(stats.GrenadesLaunched);
    }

    private string ReadPlayerStatsCore(PacketReader reader, PlayerStats stats)
    {
      string str = reader.ReadString();
      if (stats != null)
      {
        stats.SecondsPlayed = reader.ReadDouble();
        stats.DistanceWalked = reader.ReadSingle();
        stats.DistanceFlown = reader.ReadSingle();
        stats.BlocksCleared = reader.ReadInt32();
        stats.BlocksPlaced = reader.ReadInt32();
        stats.BlocksPickedUp = reader.ReadInt32();
        stats.ItemsPickedUp = reader.ReadInt32();
        stats.DamageDealt = reader.ReadSingle();
        stats.DamageTaken = reader.ReadSingle();
        stats.TotalDeaths = reader.ReadInt32();
        stats.PlayerKills = reader.ReadInt32();
        stats.NPCKills = reader.ReadInt32();
        stats.TotalKills = stats.PlayerKills + stats.NPCKills;
        stats.LootValue = reader.ReadInt32();
        stats.GrenadesLaunched = reader.ReadInt32();
      }
      return str;
    }

    public void SendPlayerSkills(
      StudioForge.TotalMiner.Player player,
      NetworkGamer recipient,
      bool requestRemoteSkills,
      bool sendLocalSkillsData)
    {
      if (player == null || !this.IsSessionOpenAndNotLocal)
        return;
      CharacterSkillsData skillData = sendLocalSkillsData ? player.LocalSkillsData : Globals2.GamertagData.GetPlayerSkillData(Globals2.GetSignedInGamer(player.PlayerIndex));
      if (skillData == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 58);
        packetWriter.Write((byte) 58);
        packetWriter.WriteGamerID(player.GamerID);
        packetWriter.Write(sendLocalSkillsData);
        packetWriter.Write(requestRemoteSkills);
        this.WritePlayerSkillsCore(packetWriter, skillData);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    public void SendPlayerSkill(StudioForge.TotalMiner.Player player, SkillData skillData)
    {
      if (!this.IsSessionOpenAndNotLocal || !Globals2.GameProperties.SaveGame.Header.SkillsEnabled)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 57);
        packetWriter.Write((byte) 57);
        packetWriter.WriteGamerID(player.GamerID);
        packetWriter.Write((byte) skillData.SkillType);
        packetWriter.Write(skillData.CurrentXP);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadPlayerSkills(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      bool sendLocalSkillsData = this.packetReader.ReadBoolean();
      bool flag = this.packetReader.ReadBoolean();
      CharacterSkillsData characterSkillsData = new CharacterSkillsData();
      this.ReadPlayerSkillsCore(this.packetReader, characterSkillsData);
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById != null && !gamerById.IsLocal)
      {
        StudioForge.TotalMiner.Player tag = gamerById.Tag as StudioForge.TotalMiner.Player;
        if (tag != null && this.gameInstance != null)
        {
          if (sendLocalSkillsData)
          {
            if (this.gameInstance.IsLocalSkills)
              tag.SkillsData = tag.LocalSkillsData = characterSkillsData;
          }
          else
          {
            if (!this.gameInstance.IsLocalSkills)
              tag.SkillsData = characterSkillsData;
            Globals2.GamertagData.AddHighScoreCacheEntry(tag.Gamertag, characterSkillsData);
          }
        }
      }
      if (!flag)
        return;
      foreach (Gamer localGamer in this.LocalGamers)
        this.SendPlayerSkills(localGamer.Tag as StudioForge.TotalMiner.Player, gamerById, false, sendLocalSkillsData);
    }

    private void ReadPlayerSkill(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      byte num = this.packetReader.ReadByte();
      double xp = this.packetReader.ReadDouble();
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById == null)
        return;
      StudioForge.TotalMiner.Player tag = gamerById.Tag as StudioForge.TotalMiner.Player;
      if (tag == null || !Globals2.GameProperties.SaveGame.Header.SkillsEnabled)
        return;
      SkillData skillData = tag.SkillsData[(int) num];
      skillData.SetCurrentXPRaw(xp);
      tag.SkillsData[(int) num] = skillData;
    }

    private void WritePlayerSkillsCore(PacketWriter writer, CharacterSkillsData skillData)
    {
      writer.Write(skillData.SkillCount);
      for (int index = 0; index < skillData.SkillCount; ++index)
        writer.Write(skillData[index].CurrentXP);
    }

    private void ReadPlayerSkillsCore(PacketReader reader, CharacterSkillsData skillData)
    {
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        double xp = reader.ReadDouble();
        if (index < skillData.SkillCount)
        {
          SkillData skillData1 = skillData[index];
          skillData1.SetCurrentXPRaw(xp);
          skillData[index] = skillData1;
        }
      }
    }

    public void SendPlayerLoaded(StudioForge.TotalMiner.Player player, SavePlayerState playerData)
    {
      if (player == null || !this.IsSessionOpenAndNotLocal || this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 62);
        packetWriter.Write((byte) 62);
        packetWriter.WriteGamerID(player.GamerID);
        packetWriter.Write(playerData.IsNewPlayer);
        packetWriter.Write((byte) playerData.ClanBannerID);
        packetWriter.Write(playerData.ClanName != null ? playerData.ClanName : "");
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadPlayerLoaded(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      this.packetReader.ReadBoolean();
      int num = (int) this.packetReader.ReadByte();
      string str = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById == null)
        return;
      SavePlayerState playerStateData = this.gameInstance.GetPlayerStateData(gamerById.Gamertag);
      if (playerStateData != null)
      {
        playerStateData.IsNewPlayer = false;
        playerStateData.ClanBannerID = num;
        playerStateData.ClanName = str;
      }
      StudioForge.TotalMiner.Player player = this.GetPlayer(gamerID);
      if (player == null)
        return;
      player.ClanBannerID = num;
      player.ClanName = str;
    }

    public void SendDamage(
      DamageType damageType,
      float damage,
      StudioForge.TotalMiner.Actor victum,
      StudioForge.TotalMiner.Actor attacker,
      Item weaponID,
      GlobalPoint3D? healthBlock)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 68);
        packetWriter.Write((byte) 68);
        packetWriter.Write((byte) damageType);
        packetWriter.Write(damage);
        packetWriter.WriteGamerID(attacker != null ? attacker.GamerID : GamerID.Sys1);
        packetWriter.Write((ushort) weaponID);
        packetWriter.Write(healthBlock.HasValue);
        if (healthBlock.HasValue)
        {
          packetWriter.Write(healthBlock.Value.X);
          packetWriter.Write(healthBlock.Value.Y);
          packetWriter.Write(healthBlock.Value.Z);
        }
        else
          packetWriter.WriteGamerID(victum.GamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, (NetworkGamer) null);
      }
    }

    private void ReadDamage(NetworkGamer sender)
    {
      DamageType damageType = (DamageType) this.packetReader.ReadByte();
      float damage = this.packetReader.ReadSingle();
      GamerID attackerID = this.packetReader.ReadGamerID();
      Item weaponID = (Item) this.packetReader.ReadUInt16();
      GamerID victumID = GamerID.Sys1;
      GlobalPoint3D? healthBlock = new GlobalPoint3D?();
      if (this.packetReader.ReadBoolean())
        healthBlock = new GlobalPoint3D?(new GlobalPoint3D(this.packetReader.ReadInt32(), this.packetReader.ReadInt32(), this.packetReader.ReadInt32()));
      else
        victumID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      this.gameInstance.RegisterDamage(damageType, damage, victumID, attackerID, weaponID, healthBlock);
    }

    public void SendActionLog(GamerID gamerID, Item itemID, ItemAction action)
    {
      if (!this.IsSessionOpenAndNotLocal || !gamerID.IsGamer)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 65);
        packetWriter.Write((byte) 65);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write((ushort) itemID);
        packetWriter.Write((byte) action);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadActionLog(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      ushort num1 = this.packetReader.ReadUInt16();
      byte num2 = this.packetReader.ReadByte();
      if (this.gameInstance == null)
        return;
      this.gameInstance.AddActionLog(gamerID, (Item) num1, (ItemAction) num2);
    }

    public void SendHistoryTable(StudioForge.TotalMiner.Player player, History history)
    {
      if (!this.IsSessionOpenAndNotLocal || history == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 67);
        packetWriter.Write((byte) 67);
        packetWriter.WriteGamerID(player != null ? player.Gamer.ID : GamerID.Sys1);
        history.WriteState((BinaryWriter) packetWriter);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendHistoryItem(string key, StudioForge.TotalMiner.Player player)
    {
      if (!this.IsSessionOpenAndNotLocal || key == null || key.Length <= 0)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 66);
        packetWriter.Write((byte) 66);
        packetWriter.WriteGamerID(player != null ? player.Gamer.ID : GamerID.Sys1);
        packetWriter.Write(key);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadHistoryTable(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      History history = (History) null;
      if (this.gameInstance != null)
        history = this.gameInstance.GetHistory(gamerID);
      if (history == null)
        history = new History();
      history.ReadState((BinaryReader) this.packetReader, 294);
    }

    private void ReadHistoryItem(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      string key = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.AddHistory(key, gamerID);
    }

    public void SendKillConfirm(
      DamageType damageType,
      StudioForge.TotalMiner.Actor victum,
      StudioForge.TotalMiner.Actor victumAttackTarget,
      StudioForge.TotalMiner.Actor attacker,
      Item weaponID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.None, attacker.Gamer);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 69);
        packetWriter.Write((byte) 69);
        packetWriter.Write((byte) damageType);
        packetWriter.WriteGamerID(victum.GamerID);
        packetWriter.WriteGamerID(victumAttackTarget != null ? victumAttackTarget.GamerID : GamerID.Sys1);
        packetWriter.WriteGamerID(attacker.GamerID);
        packetWriter.Write((ushort) weaponID);
        this.EndPacketWriter(packetWriter, SendDataOptions.None, attacker.Gamer);
      }
    }

    private void ReadKillConfirm(NetworkGamer sender)
    {
      DamageType damageType = (DamageType) this.packetReader.ReadByte();
      GamerID victumID = this.packetReader.ReadGamerID();
      GamerID victumAttackTargetID = this.packetReader.ReadGamerID();
      GamerID attackerID = this.packetReader.ReadGamerID();
      Item weaponID = (Item) this.packetReader.ReadUInt16();
      if (this.gameInstance == null)
        return;
      this.gameInstance.KillConfirm(damageType, victumID, victumAttackTargetID, attackerID, weaponID);
    }

    public void KickGamer(NetworkGamer gamer, bool isLobby)
    {
      if (!this.IsSessionOpenAndNotLocal || gamer == null || gamer.IsLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 43);
        packetWriter.Write((byte) 43);
        packetWriter.WriteGamerID(gamer.ID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
      TotalMinerGame.Instance.AddNotification(gamer.Gamertag + " has been kicked!", true);
    }

    private void ReadKickGamer(NetworkGamer sender)
    {
      NetworkGamer gamerById = this.session.FindGamerById(this.packetReader.ReadGamerID());
      if (gamerById == null)
        return;
      TotalMinerGame.Instance.AddNotification(gamerById.Gamertag + " has been kicked!", true);
      if (!gamerById.IsLocal)
        return;
      Globals2.KickedBy.Add(sender.Gamertag);
      this.EndSession(true);
    }

    public void SendGamerToJail(NetworkGamer gamer, bool isLobby)
    {
      if (!this.IsSessionOpenAndNotLocal || gamer.IsHost || gamer.IsLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 43);
        packetWriter.Write((byte) 43);
        packetWriter.WriteGamerID((Gamer) gamer);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadJailGamer(NetworkGamer sender)
    {
      int num = (int) this.packetReader.ReadInt16();
    }

    public void SendPermissions(NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 50);
        packetWriter.Write((byte) 50);
        this.SendPermissions(recipient, packetWriter);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void SendPermissions(NetworkGamer recipient, PacketWriter packetWriter)
    {
      packetWriter.Write((byte) (this.allGamerCount + 1));
      this.WritePermission(packetWriter, GamerID.Sys1, Globals2.GameProperties.SaveGame.Header.DefaultPermission);
      foreach (NetworkGamer tempAllGamer in this.tempAllGamerList)
      {
        Permissions permission = Globals2.GameProperties.SaveGame.Header.DefaultPermission;
        StudioForge.TotalMiner.Player tag = tempAllGamer.Tag as StudioForge.TotalMiner.Player;
        if (tag != null)
          permission = tag.Permission;
        this.WritePermission(packetWriter, tempAllGamer.ID, permission);
      }
    }

    public void SendPermissions(GamerID gamerID, Permissions permission, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 50);
        packetWriter.Write((byte) 50);
        packetWriter.Write((byte) 1);
        this.WritePermission(packetWriter, gamerID, permission);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void WritePermission(
      PacketWriter packetWriter,
      GamerID gamerID,
      Permissions permission)
    {
      packetWriter.WriteGamerID(gamerID);
      packetWriter.Write((ushort) permission);
    }

    private void ReadPermissions(NetworkGamer sender)
    {
      int num = (int) this.packetReader.ReadByte();
      for (int index = 0; index < num; ++index)
      {
        GamerID gamerID = this.packetReader.ReadGamerID();
        Permissions permissions = (Permissions) this.packetReader.ReadUInt16();
        if (gamerID == (short) -1)
        {
          if (Globals2.GameProperties != null)
            Globals2.GameProperties.SaveGame.Header.DefaultPermission = permissions;
        }
        else
        {
          NetworkGamer gamerById = this.FindGamerById(gamerID);
          if (gamerById != null && gamerById.Tag != null)
          {
            StudioForge.TotalMiner.Player tag = gamerById.Tag as StudioForge.TotalMiner.Player;
            if (tag != null)
              tag.Permission = permissions;
          }
        }
      }
    }

    public void SendBlast(
      GlobalPoint3D p,
      Item itemID,
      float strength,
      int radius,
      GamerID playerID,
      ushort seed)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 52);
        packetWriter.Write((byte) 52);
        packetWriter.Write((short) p.X);
        packetWriter.Write((short) p.Y);
        packetWriter.Write((short) p.Z);
        packetWriter.Write((ushort) itemID);
        packetWriter.Write(new HalfSingle(strength).PackedValue);
        packetWriter.Write((byte) radius);
        packetWriter.WriteGamerID(playerID);
        packetWriter.Write(seed);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadBlast(NetworkGamer sender)
    {
      NetworkManager.BufferedBlast bufferedBlast = new NetworkManager.BufferedBlast();
      bufferedBlast.Type = NetworkManager.BufferedChangeType.Blast;
      bufferedBlast.X = this.packetReader.ReadInt16();
      bufferedBlast.Y = this.packetReader.ReadInt16();
      bufferedBlast.Z = this.packetReader.ReadInt16();
      bufferedBlast.ItemID = (Item) this.packetReader.ReadUInt16();
      bufferedBlast.Strength = new HalfSingle()
      {
        PackedValue = this.packetReader.ReadUInt16()
      }.ToSingle();
      bufferedBlast.Radius = this.packetReader.ReadByte();
      bufferedBlast.GamerID = this.packetReader.ReadGamerID();
      bufferedBlast.Seed = this.packetReader.ReadUInt16();
      if (this.gameInstance == null)
        return;
      GlobalPoint3D globalPoint3D1 = new GlobalPoint3D((int) bufferedBlast.X, (int) bufferedBlast.Y, (int) bufferedBlast.Z);
      GlobalPoint3D globalPoint3D2 = globalPoint3D1;
      GlobalPoint3D min = globalPoint3D1 - (int) bufferedBlast.Radius;
      GlobalPoint3D max = globalPoint3D2 + (int) bufferedBlast.Radius;
      lock (this.tempChunkHashList)
      {
        this.gameInstance.Map.GetChunks(min, max, this.tempChunkHashList);
        bufferedBlast.ChunksList = new List<long>();
        foreach (KeyValuePair<long, MapChunk> tempChunkHash in this.tempChunkHashList)
          bufferedBlast.ChunksList.Add(tempChunkHash.Key);
        this.tempChunkHashList.Clear();
      }
      this.EnqueueBufferedChange((NetworkManager.BufferedChangeBase) bufferedBlast);
    }

    public void SendZone(Zone zone)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 82);
        packetWriter.Write((byte) 82);
        packetWriter.Write((byte) 0);
        this.WriteZone(packetWriter, zone);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendZoneDelete(Zone zone)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 82);
        packetWriter.Write((byte) 82);
        packetWriter.Write((byte) 1);
        this.WriteZone(packetWriter, zone);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteZone(PacketWriter packetWriter, Zone zone)
    {
      packetWriter.Write(zone.Name);
      packetWriter.WriteGamerID(zone.GamerID);
      packetWriter.Write((byte) zone.ZoneType);
      packetWriter.Write((short) zone.Min.X);
      packetWriter.Write((short) zone.Min.Y);
      packetWriter.Write((short) zone.Min.Z);
      packetWriter.Write((short) zone.Max.X);
      packetWriter.Write((short) zone.Max.Y);
      packetWriter.Write((short) zone.Max.Z);
      packetWriter.Write((byte) zone.BuilderType);
      if (zone.BuilderType != ZoneBuilderType.None)
        packetWriter.Write(zone.Builder != null ? zone.Builder : "");
      packetWriter.Write(zone.OnEntryScriptName != null ? zone.OnEntryScriptName : "");
      packetWriter.Write(zone.OnExitScriptName != null ? zone.OnExitScriptName : "");
      packetWriter.Write(zone.CombatLevelDifference);
      packetWriter.Write(zone.SpeedMultiplier);
      packetWriter.Write(zone.GravityMultiplier);
    }

    private void ReadZone(NetworkGamer sender)
    {
      NetworkManager.BufferedZone bufferedZone = new NetworkManager.BufferedZone();
      bufferedZone.Type = NetworkManager.BufferedChangeType.Zone;
      bufferedZone.Action = (ZoneEditType) this.packetReader.ReadByte();
      bufferedZone.Name = this.packetReader.ReadString();
      bufferedZone.GamerID = this.packetReader.ReadGamerID();
      bufferedZone.ZoneType = (ZoneType) this.packetReader.ReadByte();
      bufferedZone.Min.X = (int) this.packetReader.ReadInt16();
      bufferedZone.Min.Y = (int) this.packetReader.ReadInt16();
      bufferedZone.Min.Z = (int) this.packetReader.ReadInt16();
      bufferedZone.Max.X = (int) this.packetReader.ReadInt16();
      bufferedZone.Max.Y = (int) this.packetReader.ReadInt16();
      bufferedZone.Max.Z = (int) this.packetReader.ReadInt16();
      bufferedZone.BuilderType = (ZoneBuilderType) this.packetReader.ReadByte();
      if (bufferedZone.BuilderType != ZoneBuilderType.None)
        bufferedZone.Builder = this.packetReader.ReadString();
      bufferedZone.OnEntryScript = this.packetReader.ReadString();
      bufferedZone.OnExitScript = this.packetReader.ReadString();
      bufferedZone.CombatLevelDifference = this.packetReader.ReadInt16();
      bufferedZone.SpeedMultiplier = this.packetReader.ReadSingle();
      bufferedZone.GravityMultiplier = this.packetReader.ReadSingle();
      if (this.gameInstance == null)
        return;
      this.EnqueueBufferedChange((NetworkManager.BufferedChangeBase) bufferedZone);
    }

    public void SendSleepState(float hours)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 94);
        packetWriter.Write((byte) 94);
        packetWriter.Write(hours);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadSleepState(NetworkGamer sender)
    {
      float hours = this.packetReader.ReadSingle();
      if (this.IsHost || this.gameInstance == null)
        return;
      if ((double) hours == 0.0)
        this.gameInstance.ClearSleepState();
      else
        this.gameInstance.StartSleep(hours);
    }

    public void SendBookIDRequest(BookData book, StudioForge.TotalMiner.Player player, short slotID)
    {
      if (!this.IsSessionOpenAndNotLocal || this.IsHost || (player == null || book == null))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 90);
        packetWriter.Write((byte) 90);
        packetWriter.WriteGamerID(player.GamerID);
        packetWriter.Write(slotID);
        this.WriteBookData(packetWriter, book);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    public void SendBookIDConfirm(BookData book, GamerID gamerID, short slotID)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost || book == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 91);
        packetWriter.Write((byte) 91);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write(slotID);
        packetWriter.Write(book.ID);
        this.WriteBookData(packetWriter, book);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendBookUpdate(BookData book)
    {
      if (!this.IsSessionOpenAndNotLocal || book == null || book.ID <= (ushort) 1)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 92);
        packetWriter.Write((byte) 92);
        packetWriter.Write(book.ID);
        this.WriteBookData(packetWriter, book);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteBookData(PacketWriter packetWriter, BookData book)
    {
      packetWriter.Write(book.Title != null ? book.Title : "");
      this.WriteStringArray(packetWriter, book.Text);
    }

    private void ReadBookIDRequest(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      short slotID = this.packetReader.ReadInt16();
      string title;
      string[] text;
      this.ReadBookData(this.packetReader, out title, out text);
      if (!this.IsHost || this.gameInstance == null)
        return;
      this.gameInstance.AllocateAndConfirmNewBookID(new BookData()
      {
        Title = title,
        Text = text
      }, gamerID, slotID);
    }

    private void ReadBookIDConfirm(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      short num1 = this.packetReader.ReadInt16();
      ushort num2 = this.packetReader.ReadUInt16();
      string title;
      string[] text;
      this.ReadBookData(this.packetReader, out title, out text);
      if (this.gameInstance == null)
        return;
      this.gameInstance.OnBookIDConfirmed(new BookData()
      {
        ID = num2,
        Title = title,
        Text = text
      }, gamerID, (int) num1);
    }

    private void ReadBookUpdate(NetworkGamer sender)
    {
      ushort bookID = this.packetReader.ReadUInt16();
      string title;
      string[] text;
      this.ReadBookData(this.packetReader, out title, out text);
      if (this.gameInstance == null)
        return;
      this.gameInstance.UpdateBookDetails(bookID, title, text);
    }

    private void ReadBookData(PacketReader packetReader, out string title, out string[] text)
    {
      title = packetReader.ReadString();
      text = this.ReadStringArray();
    }

    public void SendNotification(
      string message,
      NotifyRecipient recType,
      string clanName,
      NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 76);
        packetWriter.Write((byte) 76);
        packetWriter.Write((byte) recType);
        packetWriter.Write(message);
        if ((recType & NotifyRecipient.Clan) > NotifyRecipient.None)
          packetWriter.Write(clanName != null ? clanName : "");
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadNotification(NetworkGamer sender)
    {
      NotifyRecipient recType = (NotifyRecipient) this.packetReader.ReadByte();
      string str = this.packetReader.ReadString();
      string clanName = (recType & NotifyRecipient.Clan) > NotifyRecipient.None ? this.packetReader.ReadString() : (string) null;
      if (this.gameInstance == null || this.LocalGamerCount > 0 && !this.gameInstance.GetLocalPlayerByScreenID(0).Settings.HudVisible)
        return;
      bool flag = false;
      if ((recType & (NotifyRecipient.Admin | NotifyRecipient.Clan)) == NotifyRecipient.None)
      {
        flag = true;
      }
      else
      {
        foreach (StudioForge.TotalMiner.Player localEnabledPlayer in this.LocalEnabledPlayers)
        {
          if ((recType & NotifyRecipient.Admin) > NotifyRecipient.None && localEnabledPlayer.IsAdmin || (recType & NotifyRecipient.Clan) > NotifyRecipient.None && clanName != null && localEnabledPlayer.ClanName == clanName)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        return;
      this.gameInstance.AddMessageToChatLog(recType, clanName, str);
      TotalMinerGame.Instance.AddNotification(str, false);
    }

    public void SendTextMessage(TextMessage msg, NetworkGamer sender, NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 109);
        packetWriter.Write((byte) 109);
        packetWriter.Write((byte) msg.Target);
        packetWriter.WriteGamerID(sender.ID);
        packetWriter.Write(msg.Message);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, recipient);
      }
    }

    private void ReadTextMessage(NetworkGamer sender)
    {
      TextMsgTarget target = (TextMsgTarget) this.packetReader.ReadByte();
      GamerID id = this.packetReader.ReadGamerID();
      string message = this.packetReader.ReadString();
      if (this.gameInstance == null || this.LocalGamerCount <= 0)
        return;
      this.gameInstance.ReceiveTextMessage(target, this.GetGamer(id), message);
    }

    public void SendArcadeState()
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.IsHost ? (NetworkGamer) null : this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 110);
        packetWriter.Write((byte) 110);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScore);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScore);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreGamer);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreGamer);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders.HighScoreVersion);
        packetWriter.Write(StudioForge.TotalMiner.Arcade.TotalRush.TotalRush.HighScoreVersion);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.IsHost ? (NetworkGamer) null : this.session.Host);
      }
    }

    private void ReadArcadeState(NetworkGamer sender)
    {
      int highscore1 = this.packetReader.ReadInt32();
      int highscore2 = this.packetReader.ReadInt32();
      string highscoreGamer1 = this.packetReader.ReadString();
      string highscoreGamer2 = this.packetReader.ReadString();
      string highscoreVersion1 = this.packetReader.ReadString();
      string highscoreVersion2 = this.packetReader.ReadString();
      if (this.gameInstance != null)
      {
        foreach (ArcadeMachine arcadeMachine in this.gameInstance.ArcadeMachines)
        {
          if (arcadeMachine is StudioForge.TotalMiner.Arcade.TotalInvaders.TotalInvaders)
            arcadeMachine.UpdateState(highscore1, highscoreGamer1, highscoreVersion1);
          if (arcadeMachine is StudioForge.TotalMiner.Arcade.TotalRush.TotalRush)
            arcadeMachine.UpdateState(highscore2, highscoreGamer2, highscoreVersion2);
        }
      }
      if (!this.IsHost || sender.IsHost)
        return;
      this.SendArcadeState();
    }

    public void SendFlood(GlobalPoint3D p, Block blockID, GamerID gamerID)
    {
      this.SendCreativeCommand(CreativeCommand.Flood, blockID, Block.None, Block.None, (byte) 100, 0, true, "Flood", p, GlobalPoint3D.Zero, GlobalPoint3D.Zero, GlobalPoint3D.MaxValue, GlobalPoint3D.MaxValue, gamerID, (object) null);
    }

    public void SendFloodAbort(GamerID gamerID)
    {
      if (!this.IsSessionOpenAndNotLocal || !gamerID.IsGamer)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 75);
        packetWriter.Write((byte) 75);
        packetWriter.WriteGamerID(gamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendCreativeCommand(
      CreativeCommand cmd,
      Block blockID,
      Block blockID1,
      Block blockID2,
      byte percent,
      int seed,
      bool clearMarkers,
      string desc,
      GlobalPoint3D p,
      GlobalPoint3D min,
      GlobalPoint3D max,
      GlobalPoint3D xmin,
      GlobalPoint3D xmax,
      GamerID gamerID,
      object data)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 51);
        packetWriter.Write((byte) 51);
        packetWriter.Write((byte) cmd);
        packetWriter.Write((byte) blockID);
        packetWriter.Write((byte) blockID1);
        packetWriter.Write((byte) blockID2);
        packetWriter.Write(percent);
        packetWriter.Write(seed);
        packetWriter.Write(clearMarkers);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write(p.X);
        packetWriter.Write(p.Y);
        packetWriter.Write(p.Z);
        packetWriter.Write(min.X);
        packetWriter.Write(min.Y);
        packetWriter.Write(min.Z);
        packetWriter.Write(max.X);
        packetWriter.Write(max.Y);
        packetWriter.Write(max.Z);
        packetWriter.Write(xmin.X);
        packetWriter.Write(xmin.Y);
        packetWriter.Write(xmin.Z);
        packetWriter.Write(xmax.X);
        packetWriter.Write(xmax.Y);
        packetWriter.Write(xmax.Z);
        packetWriter.Write(desc);
        if (cmd == CreativeCommand.Trees)
        {
          CreativeGenerateTreeData generateTreeData = (CreativeGenerateTreeData) data;
          packetWriter.Write(generateTreeData.TreeCount);
          this.WriteBoolArray(packetWriter, generateTreeData.CompsSelected);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadCreativeCommand(NetworkGamer sender)
    {
      NetworkManager.BufferedCreativeCommand bufferedCreativeCommand = new NetworkManager.BufferedCreativeCommand();
      bufferedCreativeCommand.Type = NetworkManager.BufferedChangeType.CreativeCommand;
      bufferedCreativeCommand.Command = (CreativeCommand) this.packetReader.ReadByte();
      bufferedCreativeCommand.BlockID = (Block) this.packetReader.ReadByte();
      bufferedCreativeCommand.BlockID1 = (Block) this.packetReader.ReadByte();
      bufferedCreativeCommand.BlockID2 = (Block) this.packetReader.ReadByte();
      bufferedCreativeCommand.Percent = this.packetReader.ReadByte();
      bufferedCreativeCommand.Seed = this.packetReader.ReadInt32();
      bufferedCreativeCommand.ClearMarkers = this.packetReader.ReadBoolean();
      bufferedCreativeCommand.GamerID = this.packetReader.ReadGamerID();
      bufferedCreativeCommand.Point.X = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Point.Y = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Point.Z = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Min.X = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Min.Y = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Min.Z = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Max.X = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Max.Y = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Max.Z = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMin.X = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMin.Y = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMin.Z = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMax.X = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMax.Y = this.packetReader.ReadInt32();
      bufferedCreativeCommand.XMax.Z = this.packetReader.ReadInt32();
      bufferedCreativeCommand.Desc = this.packetReader.ReadString();
      if (bufferedCreativeCommand.Command == CreativeCommand.Trees)
        bufferedCreativeCommand.Data = (object) new CreativeGenerateTreeData()
        {
          TreeCount = this.packetReader.ReadInt32(),
          CompsSelected = this.ReadBoolArray()
        };
      if (this.gameInstance == null)
        return;
      GlobalPoint3D min = bufferedCreativeCommand.Min;
      GlobalPoint3D max = bufferedCreativeCommand.Max;
      int command = (int) bufferedCreativeCommand.Command;
      this.EnqueueBufferedChange((NetworkManager.BufferedChangeBase) bufferedCreativeCommand);
    }

    private void ReadFloodAbort(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      this.gameInstance.GetPlayer(gamerID)?.AbortFloods();
    }

    public void SendDeathmatchStart(
      DeathmatchWinType winType,
      bool eating,
      GamerID startedByGamerID)
    {
    }

    public void SendMiniGameAbort()
    {
    }

    public void SendMiniGameTimer(float elapsed)
    {
    }

    public void SendRatingVote(byte rating, GamerID gamerID)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 44);
        packetWriter.Write((byte) 44);
        packetWriter.Write(rating);
        packetWriter.WriteGamerID(gamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadRatingVote(NetworkGamer sender)
    {
      byte stars = this.packetReader.ReadByte();
      GamerID gamerID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById == null)
        return;
      this.gameInstance.WorldRateReceived(stars, gamerById);
    }

    public void UpdateLiveRating(int delta, bool newRating)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      SaveMapHead header = Globals2.GameProperties.SaveGame.Header;
      float num = header.RatingStars * (float) header.RatingCount + (float) delta;
      if (newRating)
        ++header.RatingCount;
      header.RatingStars = num / (float) header.RatingCount;
    }

    public void SendWorldFavorited(GamerID gamerID)
    {
      if (this.IsHost || !this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 45);
        packetWriter.Write((byte) 45);
        packetWriter.WriteGamerID(gamerID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadWorldFavorited(NetworkGamer sender)
    {
      GamerID gamerID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      NetworkGamer gamerById = this.FindGamerById(gamerID);
      if (gamerById == null)
        return;
      this.gameInstance.WorldFavoriteReceived(gamerById);
    }

    public void SendLockedInfoRequest()
    {
      if (!this.IsSessionOpenAndNotLocal || this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 46);
        packetWriter.Write((byte) 46);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadLockedInfoRequest(NetworkGamer sender)
    {
      this.SendLockedInfo(sender);
    }

    private void SendLockedInfo(NetworkGamer recipient)
    {
      if (!this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 47);
        packetWriter.Write((byte) 47);
        packetWriter.Write(Blueprints.BlueprintList.Length);
        foreach (Blueprint blueprint in Blueprints.BlueprintList)
          packetWriter.Write(blueprint.IsEnabled);
        packetWriter.Write(Wisdom.WisdomList.Length);
        foreach (WisdomItem wisdom in Wisdom.WisdomList)
          packetWriter.Write(wisdom.IsEnabled);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadLockedInfo(NetworkGamer sender)
    {
      bool isHost = this.IsHost;
      int num1 = this.packetReader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        bool flag = this.packetReader.ReadBoolean();
        if (!isHost && index < Blueprints.BlueprintList.Length && flag)
        {
          Blueprints.BlueprintList[index].IsValid = true;
          Blueprints.BlueprintList[index].IsEnabled = true;
          Blueprints.BlueprintList[index].IsUnearthed = true;
        }
      }
      int num2 = this.packetReader.ReadInt32();
      for (int index = 0; index < num2; ++index)
      {
        bool flag = this.packetReader.ReadBoolean();
        if (!isHost && index < Wisdom.WisdomList.Length && flag)
          Wisdom.WisdomList[index].IsEnabled = flag;
      }
    }

    public void SendItemUnlocked(Item itemID)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 93);
        packetWriter.Write((byte) 93);
        packetWriter.Write((ushort) itemID);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadItemUnlocked(NetworkGamer sender)
    {
      Item itemID = (Item) this.packetReader.ReadUInt16();
      if (this.gameInstance == null)
        return;
      this.gameInstance.UnlockItem((StudioForge.TotalMiner.Player) null, itemID, false);
    }

    public void SendCustomDataRequest()
    {
      if (!this.IsSessionOpenAndNotLocal || this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 48);
        packetWriter.Write((byte) 48);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadCustomDataRequest(NetworkGamer sender)
    {
      this.SendCustomData(sender);
    }

    private void SendCustomData(NetworkGamer recipient)
    {
      if (!this.IsHost || this.gameInstance == null || this.gameInstance.ScriptCatchupCommands.Count <= 0)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 49);
        packetWriter.Write((byte) 49);
        Globals1.WriteStringList((BinaryWriter) packetWriter, this.gameInstance.ScriptCatchupCommands);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadCustomData(NetworkGamer sender)
    {
      List<string> stringList = Globals1.ReadStringList((BinaryReader) this.packetReader);
      if (this.gameInstance == null)
        return;
      Script script = new Script("");
      script.Commands = stringList;
      ScriptExecuteData data = new ScriptExecuteData();
      this.gameInstance.ExecuteScript(script, data, false);
    }

    public void SendScriptExecute(Script script, ScriptExecuteData data)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 97);
        packetWriter.Write((byte) 97);
        packetWriter.Write(script.Name);
        packetWriter.WriteGamerID(data.Actor != null ? data.Actor.GamerID : GamerID.Sys1);
        packetWriter.Write(data.Seed);
        packetWriter.Write(data.ScriptOffset.HasValue);
        if (data.ScriptOffset.HasValue)
        {
          packetWriter.Write(data.ScriptOffset.Value.X);
          packetWriter.Write(data.ScriptOffset.Value.Y);
          packetWriter.Write(data.ScriptOffset.Value.Z);
        }
        packetWriter.Write(data.BlockOffset.HasValue);
        if (data.BlockOffset.HasValue)
        {
          packetWriter.Write(data.BlockOffset.Value.X);
          packetWriter.Write(data.BlockOffset.Value.Y);
          packetWriter.Write(data.BlockOffset.Value.Z);
        }
        packetWriter.Write(data.TempScript);
        if (data.TempScript)
        {
          packetWriter.Write(script.Commands.Count);
          for (int index = 0; index < script.Commands.Count; ++index)
            packetWriter.Write(script.Commands[index]);
        }
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadScriptExecute(NetworkGamer sender)
    {
      ScriptExecuteData data = new ScriptExecuteData();
      string str = this.packetReader.ReadString();
      GamerID gamerID = this.packetReader.ReadGamerID();
      data.Seed = this.packetReader.ReadInt32();
      if (this.packetReader.ReadBoolean())
        data.ScriptOffset = new GlobalPoint3D?(new GlobalPoint3D(this.packetReader.ReadInt32(), this.packetReader.ReadInt32(), this.packetReader.ReadInt32()));
      if (this.packetReader.ReadBoolean())
        data.BlockOffset = new GlobalPoint3D?(new GlobalPoint3D(this.packetReader.ReadInt32(), this.packetReader.ReadInt32(), this.packetReader.ReadInt32()));
      if (this.packetReader.ReadBoolean())
      {
        int commandCount = this.packetReader.ReadInt32();
        Script script = new Script(str, commandCount);
        for (int index = 0; index < commandCount; ++index)
          script.Commands.Add(this.packetReader.ReadString());
        if (this.gameInstance == null)
          return;
        data.Actor = (StudioForge.TotalMiner.Actor) this.gameInstance.GetPlayer(gamerID);
        this.gameInstance.ExecuteScript(script, data, false);
      }
      else
      {
        if (this.gameInstance == null)
          return;
        data.Actor = (StudioForge.TotalMiner.Actor) this.gameInstance.GetPlayer(gamerID);
        this.gameInstance.ExecuteScript(str, data, false);
      }
    }

    public void SendScriptEdited(string oldName, Script script)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 98);
        packetWriter.Write((byte) 98);
        packetWriter.Write(oldName);
        packetWriter.Write(script.Name);
        packetWriter.Write(script.Alias);
        packetWriter.Write(script.Commands.Count);
        for (int index = 0; index < script.Commands.Count; ++index)
          packetWriter.Write(script.Commands[index]);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadScriptEdited(NetworkGamer sender)
    {
      string origName = this.packetReader.ReadString();
      string name = this.packetReader.ReadString();
      string str = this.packetReader.ReadString();
      int num = this.packetReader.ReadInt32();
      this.scriptCommandChangeList.Clear();
      for (int index = 0; index < num; ++index)
        this.scriptCommandChangeList.Add(this.packetReader.ReadString());
      if (this.gameInstance != null)
      {
        Script script = new Script(name, this.scriptCommandChangeList.Count);
        script.Alias = str;
        script.Commands.AddRange((IEnumerable<string>) this.scriptCommandChangeList);
        this.gameInstance.AddOrOverwriteScript(origName, script, false);
      }
      this.scriptCommandChangeList.Clear();
    }

    public void SendScriptDeleted(string scriptName)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 99);
        packetWriter.Write((byte) 99);
        packetWriter.Write(scriptName);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadScriptDeleted(NetworkGamer sender)
    {
      string scriptName = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.DeleteScript(scriptName);
    }

    public void SendScriptCancelled(string scriptName)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 100);
        packetWriter.Write((byte) 100);
        packetWriter.Write(scriptName);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadScriptCancelled(NetworkGamer sender)
    {
      string scriptName = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.CancelScript(scriptName, (StudioForge.TotalMiner.Actor) null, false);
    }

    public void SendScriptInputResult(string scriptName, GamerID gamerID, double? val)
    {
      if (!this.IsSessionOpenAndNotLocalAndRemotes)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 101);
        packetWriter.Write((byte) 101);
        packetWriter.Write(scriptName);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.Write(val.HasValue);
        if (val.HasValue)
          packetWriter.Write(val.Value);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadScriptInputResult(NetworkGamer sender)
    {
      string name = this.packetReader.ReadString();
      GamerID gamerID = this.packetReader.ReadGamerID();
      double? val = new double?();
      if (this.packetReader.ReadBoolean())
        val = new double?(this.packetReader.ReadDouble());
      if (this.gameInstance == null)
        return;
      this.gameInstance.ReceiveScriptInputResult(name, gamerID, val);
    }

    public void SendScriptIntersectResult(string scriptName, GamerID gamerID, GamerID targetID)
    {
      if (!this.IsSessionOpenAndNotLocalAndRemotes)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 102);
        packetWriter.Write((byte) 102);
        packetWriter.Write(scriptName);
        packetWriter.WriteGamerID(gamerID);
        packetWriter.WriteGamerID(targetID);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadScriptIntersectResult(NetworkGamer sender)
    {
      string name = this.packetReader.ReadString();
      GamerID gamerID = this.packetReader.ReadGamerID();
      GamerID targetID = this.packetReader.ReadGamerID();
      if (this.gameInstance == null)
        return;
      this.gameInstance.ReceiveScriptIntersectResult(name, gamerID, targetID);
    }

    public void SendAdventureScript(string scriptName, bool remove)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 103);
        packetWriter.Write((byte) 103);
        packetWriter.Write(scriptName);
        packetWriter.Write(remove);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadAdventureScript(NetworkGamer sender)
    {
      string scriptName = this.packetReader.ReadString();
      bool flag = this.packetReader.ReadBoolean();
      if (this.gameInstance == null)
        return;
      Script script = this.gameInstance.GetScript(scriptName);
      if (script == null)
        return;
      if (flag)
        this.gameInstance.RemoveAdventureScript(script);
      else
        this.gameInstance.AddAdventureScript(script);
    }

    public void SendEventScript(string scriptName, ScriptEvent e)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 104);
        packetWriter.Write((byte) 104);
        packetWriter.Write((byte) e);
        packetWriter.Write(scriptName == null ? "" : scriptName);
        this.EndPacketWriter(packetWriter, SendDataOptions.ReliableInOrder, (NetworkGamer) null);
      }
    }

    private void ReadEventScript(NetworkGamer sender)
    {
      ScriptEvent e = (ScriptEvent) this.packetReader.ReadByte();
      string scriptName = this.packetReader.ReadString();
      if (this.gameInstance == null)
        return;
      this.gameInstance.SetEventScript(e, this.gameInstance.GetScript(scriptName));
    }

    public void SendComponentAsTempRequest(string comPack, string comName, MetaExecuteBase meta)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 105);
        packetWriter.Write((byte) 105);
        packetWriter.Write(comPack);
        packetWriter.Write(comName);
        this.WriteMetaExecute(packetWriter, meta);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadComponentAsTempRequest(NetworkGamer sender)
    {
      string str1 = this.packetReader.ReadString();
      string str2 = this.packetReader.ReadString();
      MetaExecuteBase meta = this.ReadMetaExecute(this.packetReader);
      if (this.gameInstance == null || !this.gameInstance.VoxelModelManager.HasComponent(str1, str2, true))
        return;
      this.SendComponentAsTempRequestConfirm(str1, str2, meta, sender);
    }

    public void SendComponentAsTempRequestConfirm(
      string comPack,
      string comName,
      MetaExecuteBase meta,
      NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 106);
        packetWriter.Write((byte) 106);
        packetWriter.Write(comPack);
        packetWriter.Write(comName);
        this.WriteMetaExecute(packetWriter, meta);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadComponentAsTempRequestConfirm(NetworkGamer sender)
    {
      string comPack = this.packetReader.ReadString();
      string comName = this.packetReader.ReadString();
      MetaExecuteBase meta = this.ReadMetaExecute(this.packetReader);
      if (this.gameInstance == null)
        return;
      string key = comPack + "_" + comName;
      bool flag = false;
      lock (this.componentAsTempRequestConfirmations)
      {
        if (!this.componentAsTempRequestConfirmations.ContainsKey(key))
        {
          this.componentAsTempRequestConfirmations.Add(key, sender);
          flag = true;
        }
      }
      if (!flag)
        return;
      this.SendComponentAsTempRequestData(comPack, comName, meta, sender);
    }

    public void SendComponentAsTempRequestData(
      string comPack,
      string comName,
      MetaExecuteBase meta,
      NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 107);
        packetWriter.Write((byte) 107);
        packetWriter.Write(comPack);
        packetWriter.Write(comName);
        this.WriteMetaExecute(packetWriter, meta);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadComponentAsTempRequestData(NetworkGamer sender)
    {
      string comPack = this.packetReader.ReadString();
      string comName = this.packetReader.ReadString();
      MetaExecuteBase meta = this.ReadMetaExecute(this.packetReader);
      if (this.gameInstance == null)
        return;
      this.SendComponentAsTemp(comPack, comName, meta, sender);
    }

    public void SendComponentAsTemp(
      string comPack,
      string comName,
      MetaExecuteBase meta,
      NetworkGamer recipient)
    {
      if (!this.IsSessionOpenAndNotLocal || this.gameInstance == null || this.gameInstance.VoxelModelManager == null)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 108);
        packetWriter.Write((byte) 108);
        packetWriter.Write(comPack);
        packetWriter.Write(comName);
        this.WriteMetaExecute(packetWriter, meta);
        this.WriteComponentRawData(packetWriter, comPack, comName);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void WriteComponentRawData(PacketWriter packetWriter, string comPack, string comName)
    {
      byte[] buffer = this.gameInstance.VoxelModelManager.LoadComponentRawData(comPack, comName, true);
      if (buffer != null)
      {
        packetWriter.Write(buffer.Length);
        packetWriter.Write(buffer);
      }
      else
        packetWriter.Write(0);
    }

    private void ReadComponentAsTemp(NetworkGamer sender)
    {
      string str1 = this.packetReader.ReadString();
      string str2 = this.packetReader.ReadString();
      MetaExecuteBase meta = this.ReadMetaExecute(this.packetReader);
      int count = this.packetReader.ReadInt32();
      if (count <= 0)
        return;
      byte[] numArray = new byte[count];
      this.packetReader.Read(numArray, 0, count);
      int packIfDoesNotExist = Globals2.CreateNewComPackIfDoesNotExist(this.gameInstance, "System Temp");
      if (packIfDoesNotExist < 0)
        return;
      VoxelModelManager.SaveComponentNoHash(packIfDoesNotExist, str1 + "_" + str2, numArray);
      this.ProcessMetaExecute(meta);
    }

    public void SendCaveInStart(GlobalPoint3D origin, int seed)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 83);
        packetWriter.Write((byte) 83);
        packetWriter.Write((short) origin.X);
        packetWriter.Write((short) origin.Y);
        packetWriter.Write((short) origin.Z);
        packetWriter.Write(seed);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void ReadCaveInStart(NetworkGamer sender)
    {
      GlobalPoint3D zero = GlobalPoint3D.Zero;
      zero.X = (int) this.packetReader.ReadInt16();
      zero.Y = (int) this.packetReader.ReadInt16();
      zero.Z = (int) this.packetReader.ReadInt16();
      int seed = this.packetReader.ReadInt32();
      if (this.IsHost || this.gameInstance == null)
        return;
      this.gameInstance.StartCaveIn(zero, seed, false);
    }

    public void SendFog(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      ushort visibility)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 84);
        packetWriter.Write((byte) 84);
        this.WriteEnvBase(packetWriter, NetworkManager.WeatherType.Fog, center, radius, duration, transitDuration, intensity, color);
        packetWriter.Write(visibility);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendRain(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 84);
        packetWriter.Write((byte) 84);
        this.WriteEnvBase(packetWriter, NetworkManager.WeatherType.Rain, center, radius, duration, transitDuration, intensity, color);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    public void SendHail(
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color,
      float minSize,
      float maxSize)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsHost)
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, (NetworkGamer) null);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 84);
        packetWriter.Write((byte) 84);
        this.WriteEnvBase(packetWriter, NetworkManager.WeatherType.Hail, center, radius, duration, transitDuration, intensity, color);
        packetWriter.Write(minSize);
        packetWriter.Write(maxSize);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, (NetworkGamer) null);
      }
    }

    private void WriteEnvBase(
      PacketWriter packetWriter,
      NetworkManager.WeatherType type,
      GlobalPoint3D center,
      float radius,
      float duration,
      float transitDuration,
      float intensity,
      Color color)
    {
      packetWriter.Write((byte) type);
      packetWriter.Write((short) center.X);
      packetWriter.Write((short) center.Y);
      packetWriter.Write((short) center.Z);
      packetWriter.Write(radius);
      packetWriter.Write(duration);
      packetWriter.Write(transitDuration);
      packetWriter.Write(intensity);
      packetWriter.Write(color.PackedValue);
    }

    private void ReadWeather(NetworkGamer sender)
    {
      NetworkManager.WeatherType weatherType = (NetworkManager.WeatherType) this.packetReader.ReadByte();
      GlobalPoint3D center;
      float radius;
      float duration;
      float transitDuration;
      float intensity;
      Color color;
      this.ReadEnvBase(this.packetReader, out center, out radius, out duration, out transitDuration, out intensity, out color);
      switch (weatherType)
      {
        case NetworkManager.WeatherType.Fog:
          ushort num = this.packetReader.ReadUInt16();
          if (this.GameInstance == null || this.IsHost)
            break;
          this.GameInstance.MapStrategyTM.EnvManager.AddFog(center, radius, duration, transitDuration, intensity, color, (int) num, false);
          break;
        case NetworkManager.WeatherType.Rain:
          if (this.GameInstance == null || this.IsHost)
            break;
          this.GameInstance.MapStrategyTM.EnvManager.AddRain(center, radius, duration, transitDuration, intensity, color, false);
          break;
        case NetworkManager.WeatherType.Hail:
          float minSize = this.packetReader.ReadSingle();
          float maxSize = this.packetReader.ReadSingle();
          if (this.GameInstance == null || this.IsHost)
            break;
          this.GameInstance.MapStrategyTM.EnvManager.AddHail(center, radius, duration, transitDuration, intensity, color, minSize, maxSize, false);
          break;
      }
    }

    private void ReadEnvBase(
      PacketReader reader,
      out GlobalPoint3D center,
      out float radius,
      out float duration,
      out float transitDuration,
      out float intensity,
      out Color color)
    {
      center = new GlobalPoint3D();
      center.X = (int) this.packetReader.ReadInt16();
      center.Y = (int) this.packetReader.ReadInt16();
      center.Z = (int) this.packetReader.ReadInt16();
      radius = this.packetReader.ReadSingle();
      duration = this.packetReader.ReadSingle();
      transitDuration = this.packetReader.ReadSingle();
      intensity = this.packetReader.ReadSingle();
      color = new Color();
      color.PackedValue = this.packetReader.ReadUInt32();
    }

    public void SendPhotoThumbnailRequest(byte index)
    {
      if (!this.IsSessionOpenAndNotLocal || !this.IsRemote || GraphicStatics.PhotoData.IsPhotoThumbnailRequestSent(index))
        return;
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, this.session.Host);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 87);
        packetWriter.Write((byte) 87);
        packetWriter.Write(index);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, this.session.Host);
      }
    }

    private void ReadPhotoThumbnailRequest(NetworkGamer sender)
    {
      byte index = this.packetReader.ReadByte();
      this.SendPhotoThumbnail(sender, index);
    }

    public void SendPhotoThumbnail(NetworkGamer recipient, byte index)
    {
      PacketWriter packetWriter = this.GetPacketWriter(SendDataOptions.Reliable, recipient);
      if (packetWriter == null)
        return;
      lock (packetWriter)
      {
        packetWriter.Write((byte) 88);
        packetWriter.Write((byte) 88);
        packetWriter.Write(index);
        MapSaver.WriteColorArray((BinaryWriter) packetWriter, GraphicStatics.PhotoData.PhotoThumbnail16ColorData[(int) index]);
        MapSaver.WriteColorArray((BinaryWriter) packetWriter, GraphicStatics.PhotoData.PhotoThumbnail64ColorData[(int) index]);
        this.EndPacketWriter(packetWriter, SendDataOptions.Reliable, recipient);
      }
    }

    private void ReadPhotoThumbnail(NetworkGamer sender)
    {
      byte num = this.packetReader.ReadByte();
      this.ReadPhotoThumbnail((int) num, GraphicStatics.PhotoData.PhotoThumbnail16ColorData);
      this.ReadPhotoThumbnail((int) num, GraphicStatics.PhotoData.PhotoThumbnail64ColorData);
      this.GameInstance.TexturePackNeedsReload();
    }

    private void ReadPhotoThumbnail(int index, Color[][] colorData)
    {
      int count = this.packetReader.ReadInt32();
      if (count <= 0)
        return;
      Color[] colorData1 = colorData[index];
      if (colorData1 == null || colorData1.Length != count)
        colorData[index] = colorData1 = new Color[count];
      MapLoader.ReadColorArray((BinaryReader) this.packetReader, colorData1, count, 294);
    }

    private void WriteGlobalPoint3DList(PacketWriter writer, List<GlobalPoint3D> list)
    {
      writer.Write(list.Count);
      foreach (GlobalPoint3D globalPoint3D in list)
      {
        writer.Write((ushort) globalPoint3D.X);
        writer.Write((ushort) globalPoint3D.Y);
        writer.Write((ushort) globalPoint3D.Z);
      }
    }

    private List<GlobalPoint3D> ReadGlobalPoint3DList()
    {
      int capacity = this.packetReader.ReadInt32();
      List<GlobalPoint3D> globalPoint3DList = new List<GlobalPoint3D>(capacity);
      GlobalPoint3D globalPoint3D = new GlobalPoint3D();
      for (int index = 0; index < capacity; ++index)
      {
        globalPoint3D.X = (int) this.packetReader.ReadUInt16();
        globalPoint3D.Y = (int) this.packetReader.ReadUInt16();
        globalPoint3D.Z = (int) this.packetReader.ReadUInt16();
        globalPoint3DList.Add(globalPoint3D);
      }
      return globalPoint3DList;
    }

    private void WriteLongList(PacketWriter writer, List<long> list)
    {
      writer.Write(list.Count);
      foreach (long num in list)
        writer.Write(num);
    }

    private void ReadLongList(List<long> result)
    {
      int num = this.packetReader.ReadInt32();
      result.Clear();
      for (int index = 0; index < num; ++index)
        result.Add(this.packetReader.ReadInt64());
    }

    private void WriteByteList(PacketWriter writer, List<byte> list)
    {
      writer.Write(list.Count);
      foreach (byte num in list)
        writer.Write(num);
    }

    private void WriteBoolList(PacketWriter writer, List<bool> list)
    {
      writer.Write(list.Count);
      foreach (bool flag in list)
        writer.Write(flag);
    }

    private void WriteBoolArray(PacketWriter writer, bool[] list)
    {
      writer.Write(list.Length);
      foreach (bool flag in list)
        writer.Write(flag);
    }

    private bool[] ReadBoolArray()
    {
      int length = this.packetReader.ReadInt32();
      bool[] flagArray = new bool[length];
      for (int index = 0; index < length; ++index)
        flagArray[index] = this.packetReader.ReadBoolean();
      return flagArray;
    }

    private List<bool> ReadBoolList()
    {
      int capacity = this.packetReader.ReadInt32();
      List<bool> boolList = new List<bool>(capacity);
      for (int index = 0; index < capacity; ++index)
        boolList.Add(this.packetReader.ReadBoolean());
      return boolList;
    }

    private byte[] ReadByteArray()
    {
      int count = this.packetReader.ReadInt32();
      byte[] buffer = new byte[count];
      this.packetReader.BaseStream.Read(buffer, 0, count);
      return buffer;
    }

    private List<byte> ReadByteList()
    {
      int capacity = this.packetReader.ReadInt32();
      List<byte> byteList = new List<byte>(capacity);
      for (int index = 0; index < capacity; ++index)
        byteList.Add(this.packetReader.ReadByte());
      return byteList;
    }

    private void WriteStringArray(PacketWriter writer, string[] list)
    {
      int num = list != null ? list.Length : 0;
      writer.Write(num);
      if (num <= 0)
        return;
      foreach (string str in list)
        writer.Write(str ?? "");
    }

    private string[] ReadStringArray()
    {
      string[] strArray = (string[]) null;
      int length = this.packetReader.ReadInt32();
      if (length > 0)
      {
        strArray = new string[length];
        for (int index = 0; index < length; ++index)
        {
          strArray[index] = this.packetReader.ReadString();
          if (strArray[index].Length < 1)
            strArray[index] = (string) null;
        }
      }
      return strArray;
    }

    private void EndPacketWriter(
      PacketWriter packetWriter,
      SendDataOptions options,
      NetworkGamer recipient)
    {
    }

    private PacketWriter GetPacketWriter(
      SendDataOptions options,
      NetworkGamer recipient)
    {
      if (recipient == null)
      {
        switch (options)
        {
          case SendDataOptions.None:
            return this.packetWriterNone;
          case SendDataOptions.Reliable:
            return this.packetWriterReliable;
          case SendDataOptions.InOrder:
            return this.packetWriterInOrder;
          case SendDataOptions.ReliableInOrder:
            return this.packetWriterReliableInOrder;
          default:
            return (PacketWriter) null;
        }
      }
      else
      {
        NetworkManager.MachineData machineData = this.GetMachineData(recipient.Machine.GetHashCode());
        if (machineData != null)
        {
          switch (options)
          {
            case SendDataOptions.None:
              return machineData.PacketWriterNone;
            case SendDataOptions.Reliable:
              return machineData.PacketWriterReliable;
            case SendDataOptions.InOrder:
              return machineData.PacketWriterInOrder;
            case SendDataOptions.ReliableInOrder:
              return machineData.PacketWriterReliableInOrder;
          }
        }
        return (PacketWriter) null;
      }
    }

    public void CreateOnlinePlaySession(
      SessionProperties properties,
      Action<bool> onSessionCreated,
      PlayerIndex? controllingPlayer,
      NetworkSessionType sessionType,
      string sessionDesc,
      int maxLocalGamers)
    {
      this.gameDataToReceive = new NetworkManager.GameDataSendReceive();
      this.CreateSession(properties, onSessionCreated, controllingPlayer, sessionType, sessionDesc, maxLocalGamers, Globals2.GameProperties.SaveGame.Header.MaxPlayers);
    }

    public void CreateShareSession(
      SessionProperties properties,
      SessionType sessionType,
      Action<bool> onSessionCreated,
      PlayerIndex? controllingPlayer,
      string sessionDesc,
      int maxLocalGamers)
    {
      this.CreateSession(properties, onSessionCreated, controllingPlayer, NetworkSessionType.PlayerMatch, sessionDesc, maxLocalGamers, 2);
    }

    public void SetServerDataReceiveCallback(
      Action<NetworkGamer, PacketReader, PacketType> onServerPacketReceived)
    {
      this.onServerPacketReceived = onServerPacketReceived;
    }

    private void CreateSession(
      SessionProperties properties,
      Action<bool> sessionCreatedCallback,
      PlayerIndex? controllingPlayer,
      NetworkSessionType type,
      string sessionDesc,
      int maxLocalGamers,
      int maxGamers)
    {
      if (this.IsSessionOpen)
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        while (this.IsSessionOpen && stopwatch.ElapsedMilliseconds < 3000L)
          this.UpdateCore((UpdateState) null);
        if (this.IsSessionOpen)
          this.EndSession();
      }
      this.sessionProperties = properties;
      this.sessionCreatedCallback = sessionCreatedCallback;
      this.ControllingPlayer = controllingPlayer;
      this.networkSessionType = type;
      this.sessionDesc = sessionDesc;
      this.maxLocalGamers = maxLocalGamers;
      this.maxGamers = maxGamers;
      this.sessionType = properties.SessionType;
      this.createSessionThread = new Thread(new ThreadStart(this.CreateSessionCore));
      this.createSessionThread.CurrentCulture = Globals1.CultureInfo;
      this.createSessionThread.CurrentUICulture = Globals1.CultureInfo;
      this.createSessionThread.Start();
    }

    private void CreateSessionCore()
    {
      try
      {
        this.ResetSessionData();
        this.machineData = new Dictionary<int, NetworkManager.MachineData>();
        if (this.maxGamers < 1)
          this.maxGamers = 1;
        this.session = this.modNetMgr == null ? (INetworkSession) NetworkSession.Create(this.networkSessionType, Globals2.GetSignedInGamer(this.ControllingPlayer), this.maxGamers, 0, (object) this.sessionProperties) : this.modNetMgr.CreateSession(this.networkSessionType, Globals2.GetSignedInGamer(this.ControllingPlayer), this.sessionProperties);
        if (this.session != null)
        {
          this.session.GamerJoined += new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
          this.session.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
          this.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(TotalMinerGame.Instance.SessionEndedEventHandler);
          this.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(this.SessionEndedEventHandler);
          this.HostIsReady = this.session.IsHost;
        }
        if (this.sessionCreatedCallback == null)
          return;
        this.sessionCreatedCallback(this.session != null);
      }
      catch (Exception ex)
      {
        if (this.sessionCreatedCallback == null)
        {
          Services.ExceptionReporter.ReportExceptionCaught(16, ex);
          if (this.sessionCreatedCallback != null)
            this.sessionCreatedCallback(false);
          TotalMinerGame.Instance.ShowExceptionMessageBox("Error creating " + this.sessionDesc + " session:\n\n", ex, this.ControllingPlayer);
        }
        else
          this.sessionCreatedCallback(false);
      }
    }

    public void SetSessionProperty6(NetworkSessionProperties properties, int filesize)
    {
      int num = Math.Min(filesize, 64000000);
      if (Globals2.HasDefaultPermission(Permissions.Adventure))
        num |= 67108864;
      if (Globals2.HasDefaultPermission(Permissions.Edit))
        num |= 134217728;
      if (Globals2.HasDefaultPermission(Permissions.Creative))
        num |= 268435456;
      if (Globals2.HasDefaultPermission(Permissions.Fly))
        num |= 536870912;
      if (Globals2.HasDefaultPermission(Permissions.Map))
        num |= 1073741824;
      properties[6] = new int?(num);
    }

    public void BeginFind(
      NetworkSessionType type,
      int maxGamers,
      IEnumerable<Gamer> localGamers,
      NetworkSessionProperties properties,
      SessionType sessionType,
      AsyncCallback endFindCallback,
      object state)
    {
      if (this.IsSessionOpen)
      {
        Stopwatch stopwatch = new Stopwatch();
        while (this.IsSessionOpen && stopwatch.ElapsedMilliseconds < 3000L)
          this.UpdateCore((UpdateState) null);
        if (this.IsSessionOpen)
          this.EndSession(true);
      }
      properties[0] = new int?((int) (436832 + sessionType));
    }

    public List<IAvailableNetworkSession> EndFind(IAsyncResult ar)
    {
      try
      {
        return (List<IAvailableNetworkSession>) null;
      }
      catch (InvalidOperationException ex)
      {
        return (List<IAvailableNetworkSession>) null;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(17, ex);
        TotalMinerGame.Instance.ShowExceptionMessageBox("", ex, this.ControllingPlayer);
        return (List<IAvailableNetworkSession>) null;
      }
    }

    public string JoinOnlineSession(
      IAvailableNetworkSession availableSession,
      Gamer gamer,
      Action<string> endJoinCallback)
    {
      if (this.IsSessionOpen)
        return "A session is already open";
      this.session = (INetworkSession) null;
      try
      {
        this.ResetSessionData();
        this.HostIsReady = false;
        this.gameDataToReceive = new NetworkManager.GameDataSendReceive();
        this.endOnlineJoinCallback = endJoinCallback;
        if (this.modNetMgr != null)
        {
          this.session = this.modNetMgr.JoinSession(availableSession, gamer);
          this.machineData = new Dictionary<int, NetworkManager.MachineData>();
          this.sessionProperties = (SessionProperties) this.session.SessionProperties;
          this.sessionType = this.sessionProperties.SessionType;
          this.session.GamerJoined += new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
          this.session.GamerLeft += new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
          this.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(TotalMinerGame.Instance.SessionEndedEventHandler);
          this.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(this.SessionEndedEventHandler);
          if (this.endOnlineJoinCallback != null)
            this.endOnlineJoinCallback((string) null);
        }
        else
          this.joinResult = NetworkSession.BeginJoin(availableSession, new AsyncCallback(this.EndOnlineJoin), (object) null);
        return (string) null;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(18, ex);
        return ex.Message;
      }
    }

    public string JoinShareSession(
      AvailableNetworkSession availableSession,
      Action<FileShareReceiveProgress> receiveCallback)
    {
      if (this.IsSessionOpen)
        return "A session is already open";
      this.session = (INetworkSession) null;
      try
      {
        this.machineData = new Dictionary<int, NetworkManager.MachineData>();
        this.fileShareReceiveCallback = receiveCallback;
        this.joinResult = NetworkSession.BeginJoin((IAvailableNetworkSession) availableSession, new AsyncCallback(this.EndShareJoin), (object) null);
        return (string) null;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(21, ex);
        return ex.Message;
      }
    }

    public void JoinServerSession(Action onJoinCallback, Action onServerSessionFindBegin)
    {
      if (!this.IsSessionOpen)
      {
        new NetworkSessionProperties()[0] = new int?(436836);
        this.ResetSessionData();
        if (this.modNetMgr == null)
          return;
        this.modNetMgr.FindSessions(new SessionMatching()
        {
          ExeVersion = 27302
        });
        if (onServerSessionFindBegin == null)
          return;
        onServerSessionFindBegin();
      }
      else
      {
        if (onJoinCallback == null)
          return;
        onJoinCallback();
      }
    }

    private void EndFindSystemSession(IAsyncResult ar)
    {
      Action asyncState = (Action) ar.AsyncState;
      try
      {
      }
      finally
      {
        ar.AsyncWaitHandle.Close();
        if (asyncState != null)
          asyncState();
      }
    }

    public bool JoinCurrentPlaySession(Gamer gamer)
    {
      try
      {
        return true;
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(20, ex);
      }
      return false;
    }

    private void EndOnlineJoin(IAsyncResult ar)
    {
      try
      {
        this.ResetSessionData();
        this.machineData = new Dictionary<int, NetworkManager.MachineData>();
        if (this.endOnlineJoinCallback == null)
          return;
        this.endOnlineJoinCallback((string) null);
      }
      catch (NetworkSessionJoinException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(19, (Exception) ex);
        if (this.endOnlineJoinCallback == null)
          return;
        this.endOnlineJoinCallback("Error: Could not join. " + ex.Message);
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(19, ex);
        if (this.endOnlineJoinCallback == null)
          return;
        this.endOnlineJoinCallback(ex.Message);
      }
    }

    private void EndShareJoin(IAsyncResult ar)
    {
      try
      {
        this.ResetSessionData();
      }
      catch (NetworkSessionJoinException ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(22, (Exception) ex);
        this.fileShareReceiveCallback(new FileShareReceiveProgress()
        {
          Status = FileShareStatus.ConnectionError,
          ErrorMessage = "Error: Could not join. The session may have ended\nor you may have connectivity problems with the Host"
        });
      }
      catch (Exception ex)
      {
        Services.ExceptionReporter.ReportExceptionCaught(22, ex);
        this.fileShareReceiveCallback(new FileShareReceiveProgress()
        {
          Status = FileShareStatus.ConnectionError,
          ErrorMessage = ex.Message
        });
      }
    }

    public void EndSession(Action sessioEndedCallback)
    {
      this.sessioEndedCallback = sessioEndedCallback;
      this.EndSession(false);
    }

    public void EndSession()
    {
      this.EndSession(this.IsHost);
    }

    public void EndServerSession()
    {
      if (!this.IsSessionOpen || this.sessionType != SessionType.Server)
        return;
      this.session.Dispose();
      this.session = (INetworkSession) null;
    }

    public void EndSession(bool terminatedByHost)
    {
      if (this.IsSessionOpen && !this.sessionEnded)
      {
        this.sessionEndTimer = 20;
        this.sessionEnded = true;
      }
      if (!terminatedByHost)
        return;
      this.EndSessionCore();
    }

    private void EndSessionCore()
    {
      if (this.session != null && !this.session.IsDisposed)
      {
        if (this.session.IsHost && this.session.SessionState == NetworkSessionState.Playing)
          this.session.EndGame();
        this.session.GamerJoined -= new EventHandler<GamerEventArgs>(this.GamerJoinedEventHandler);
        this.session.GamerLeft -= new EventHandler<GamerEventArgs>(this.GamerLeftEventHandler);
        this.session.SessionEnded -= new EventHandler<NetworkSessionEndedEventArgs>(this.SessionEndedEventHandler);
        this.session.SessionEnded -= new EventHandler<NetworkSessionEndedEventArgs>(TotalMinerGame.Instance.SessionEndedEventHandler);
        this.session.Dispose();
      }
      if (this.sessioEndedCallback != null)
        this.sessioEndedCallback();
      this.ResetSessionData();
      this.sessionEnded = false;
    }

    private void ResetSessionData()
    {
      this.blockChangesToSend.Clear();
      this.pickupRequests.Clear();
      this.chunkRequestsToSend.Clear();
      this.chunksRequestedNotReceived.Clear();
      this.readChunkList.Clear();
      this.sendChunkList.Clear();
      this.tempChunkHashList.Clear();
      this.bufferedChanges.Clear();
      this.gameDataToSend.Clear();
      this.tempRemoteMachineList.Clear();
      this.tempAllGamerList.Clear();
      this.tempAllEnabledGamerList.Clear();
      this.tempLocalGamerList.Clear();
      this.tempRemoteGamerList.Clear();
      this.tempRemoteEnabledGamerList.Clear();
      this.tempAllPlayerList.Clear();
      this.tempLocalPlayerList.Clear();
      this.tempLocalEnabledPlayerList.Clear();
      this.mobsToSend.Clear();
      this.validMobList.Clear();
      this.componentAsTempRequestConfirmations.Clear();
      this.allGamerCount = this.allGamerEnabledCount = this.localGamerCount = this.localPlayerCount = this.remoteGamerCount = 0;
      this.currentFileShares.Clear();
      this.blockIndexes.Clear();
      this.dataBlockTempList.Clear();
      this.dataBlockChangeList.Clear();
      this.packetStack.Clear();
      this.localHost = (NetworkGamer) null;
      if (ModManager.NetMod == null)
        return;
      this.modNetMgr = ModManager.NetMod.NetworkManager;
    }

    private void ValidateSession()
    {
      if (!this.IsSessionOpen || this.session.SessionState != NetworkSessionState.Ended)
        return;
      this.EndSession(true);
    }

    private void GamerJoinedEventHandler(object sender, GamerEventArgs e)
    {
      if (!this.IsSessionOpen || e.Gamer == null)
        return;
      if (Globals2.GameSettings.HasNotification(NotificationType.Audio) && !e.Gamer.IsHost && this.IsHost)
        Sounds.PlaySound(ItemSoundGroup.GuiGamerJoined);
      int hashCode = e.Gamer.Machine.GetHashCode();
      lock (this.machineData)
      {
        if (!this.machineData.ContainsKey(hashCode))
          this.machineData.Add(hashCode, new NetworkManager.MachineData(e.Gamer.Machine));
      }
      if (this.sessionType == SessionType.Play)
      {
        PlayerIndex playerIndex = PlayerIndex.One;
        if (e.Gamer.IsLocal)
          playerIndex = e.Gamer.PlayerIndex;
        StudioForge.TotalMiner.Player player = new StudioForge.TotalMiner.Player(e.Gamer, playerIndex);
        e.Gamer.Tag = (object) player;
        if (e.Gamer.IsLocal && this.session.SessionState == NetworkSessionState.Lobby)
          e.Gamer.IsReady = this.session.IsHost;
        if (this.session.IsHost || e.Gamer.IsHost)
        {
          if (this.session.IsHost)
            ((SessionProperties) this.session.SessionProperties).CurrentPlayerCount = this.session.AllGamers.Count;
        }
        else if (e.Gamer.IsLocal && this.session.SessionState == NetworkSessionState.Lobby && Globals2.GameProperties != null)
          e.Gamer.IsReady = true;
        if (e.Gamer.IsHost)
          player.Permission = Permissions.Adventure | Permissions.Edit | Permissions.Creative | Permissions.Fly | Permissions.Map | Permissions.Save | Permissions.Admin | Permissions.Grief | Permissions.Spectate | Permissions.SystemShops | Permissions.ViewScripts | Permissions.TextChat;
        else if (this.session.IsHost && e.Gamer.IsLocal)
          player.Permission = Globals2.DefaultPermission | Permissions.Adventure | Permissions.Edit | Permissions.Creative | Permissions.Fly | Permissions.Map | Permissions.Spectate | Permissions.SystemShops | Permissions.ViewScripts | Permissions.TextChat | Permissions.Grief;
        else
          player.Permission = Globals2.DefaultPermission;
        player.ClearIsLobbyPermissionFlag();
        if (this.IsHost && this.gameInstance != null && !e.Gamer.IsLocal)
          this.SendCommand(NetworkCommand.HostIsReady, e.Gamer, SendDataOptions.Reliable);
      }
      this.BuildGamerList();
    }

    private void GamerLeftEventHandler(object sender, GamerEventArgs e)
    {
      if (!this.IsSessionOpen)
        return;
      this.BuildGamerList();
      if (this.IsHost)
        ((SessionProperties) this.session.SessionProperties).CurrentPlayerCount = this.allGamerCount;
      StudioForge.TotalMiner.Player tag = e.Gamer.Tag as StudioForge.TotalMiner.Player;
      if (this.gameInstance != null && tag != null)
        this.gameInstance.FlagBlockIsClosed(tag.GamerID, true);
      if (this.IsShareSession)
        this.RemoveAnyCurrentSharesForGamer(e.Gamer);
      if (e.Gamer.Machine.Gamers.Count != 0)
        return;
      int hashCode = e.Gamer.Machine.GetHashCode();
      lock (this.machineData)
      {
        if (!this.machineData.ContainsKey(hashCode))
          return;
        this.machineData.Remove(hashCode);
      }
    }

    private void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
    {
      this.EndSession(e.EndReason == NetworkSessionEndReason.HostEndedSession);
    }

    public void StartGame()
    {
      if (!this.IsSessionOpen || this.session.SessionState != NetworkSessionState.Lobby || !this.session.IsHost)
        return;
      this.session.StartGame();
      this.SetPlayerPresence(this.session.SessionType == NetworkSessionType.Local ? GamerPresenceMode.SinglePlayer : GamerPresenceMode.Multiplayer);
    }

    public void EndGame()
    {
      if (!this.IsSessionOpen || this.session.SessionState != NetworkSessionState.Playing || !this.session.IsHost)
        return;
      this.session.EndGame();
      this.SetPlayerPresence(GamerPresenceMode.GameOver);
    }

    private void SetPlayerPresence(GamerPresenceMode presence)
    {
    }

    public bool AreAllPlayersReady
    {
      get
      {
        if (this.IsSessionOpen)
        {
          foreach (NetworkGamer tempAllGamer in this.tempAllGamerList)
          {
            if (tempAllGamer.IsActive() && !tempAllGamer.IsReady)
              return false;
          }
        }
        return true;
      }
    }

    public void SetLocalGamersReady(bool ready)
    {
      if (!this.IsSessionOpen || this.session.SessionState != NetworkSessionState.Lobby)
        return;
      foreach (Gamer tempLocalGamer in this.tempLocalGamerList)
        tempLocalGamer.IsReady = ready;
    }

    public NetworkGamer GetLocalGamer(string gamertag)
    {
      if (this.IsSessionOpen)
      {
        foreach (NetworkGamer localGamer in this.LocalGamers)
        {
          if (localGamer != null && localGamer.Gamertag == gamertag)
            return localGamer;
        }
      }
      return (NetworkGamer) null;
    }

    public NetworkGamer GetGamer(GamerID id)
    {
      if (this.IsSessionOpen)
      {
        foreach (NetworkGamer allGamer in this.AllGamers)
        {
          if (allGamer != null && allGamer.ID == id)
            return allGamer;
        }
      }
      return (NetworkGamer) null;
    }

    public NetworkGamer GetGamer(string gamertag)
    {
      if (this.IsSessionOpen)
      {
        foreach (NetworkGamer allGamer in this.AllGamers)
        {
          if (allGamer != null && allGamer.Gamertag == gamertag)
            return allGamer;
        }
      }
      return (NetworkGamer) null;
    }

    public NetworkGamer GetLocalGamer(PlayerIndex playerIndex)
    {
      if (this.IsSessionOpen)
      {
        for (int index = 0; index < this.session.LocalGamers.Count; ++index)
        {
          NetworkGamer localGamer = this.session.LocalGamers[index];
          if (localGamer != null)
            return localGamer;
        }
      }
      return (NetworkGamer) null;
    }

    private NetworkGamer GetLocalGamer(NetworkGamer gamer)
    {
      if (this.IsSessionOpen)
      {
        for (int index = 0; index < this.session.LocalGamers.Count; ++index)
        {
          NetworkGamer localGamer = this.session.LocalGamers[index];
          if (localGamer != null && localGamer.ID == gamer.ID)
            return localGamer;
        }
      }
      return (NetworkGamer) null;
    }

    public bool IsLocalHost(PlayerIndex playerIndex)
    {
      if (this.IsSessionOpen && this.session.IsHost)
      {
        lock (this.gamerListSemaphore)
        {
          if (this.localHost != null)
          {
            Gamer signedInGamer = Globals2.GetSignedInGamer(playerIndex);
            if (signedInGamer != null)
              return signedInGamer.Gamertag == this.localHost.Gamertag && signedInGamer.PlayerIndex == playerIndex;
          }
        }
      }
      return false;
    }

    public bool HasLocalAdminPlayer()
    {
      foreach (Gamer tempLocalGamer in this.tempLocalGamerList)
      {
        StudioForge.TotalMiner.Player tag = tempLocalGamer.Tag as StudioForge.TotalMiner.Player;
        if (tag != null && tag.IsAdmin)
          return true;
      }
      return false;
    }

    public bool HasLocalPlayerOfClan(string clanName)
    {
      return this.GetFirstLocalPlayerOfClan(clanName) != null;
    }

    public StudioForge.TotalMiner.Player GetFirstLocalPlayerOfClan(string clanName)
    {
      if (clanName != null && clanName.Length > 0)
      {
        foreach (Gamer tempLocalGamer in this.tempLocalGamerList)
        {
          StudioForge.TotalMiner.Player tag = tempLocalGamer.Tag as StudioForge.TotalMiner.Player;
          if (tag != null && tag.ClanName == clanName)
            return tag;
        }
      }
      return (StudioForge.TotalMiner.Player) null;
    }

    public static GamerID GetLocalGamerID(string gamertag)
    {
      if (NetworkManager.Instance != null)
      {
        NetworkGamer localGamer = NetworkManager.Instance.GetLocalGamer(gamertag);
        if (localGamer != null)
          return localGamer.ID;
      }
      return GamerID.Sys1;
    }

    public static GamerID GetGamerID(string gamertag)
    {
      if (NetworkManager.Instance != null)
      {
        NetworkGamer gamer = NetworkManager.Instance.GetGamer(gamertag);
        if (gamer != null)
          return gamer.ID;
      }
      return GamerID.Sys1;
    }

    public static string GetGamertag(PlayerIndex playerIndex)
    {
      Gamer signedInGamer = Globals2.GetSignedInGamer(playerIndex);
      if (signedInGamer != null)
        return signedInGamer.Gamertag;
      return "LocalGamer" + playerIndex.ToString();
    }

    public string GetGamertag(GamerID gamerID)
    {
      if (this.IsSessionOpen)
      {
        NetworkGamer gamerById = this.FindGamerById(gamerID);
        if (gamerById != null)
          return gamerById.Gamertag;
      }
      return (string) null;
    }

    public StudioForge.TotalMiner.Player GetPlayer(GamerID gamerID)
    {
      if (this.IsSessionOpen)
      {
        NetworkGamer gamerById = this.FindGamerById(gamerID);
        if (gamerById != null)
          return gamerById.Tag as StudioForge.TotalMiner.Player;
      }
      return (StudioForge.TotalMiner.Player) null;
    }

    public int PublicGamerCount
    {
      get
      {
        if (this.IsSessionOpen)
          return this.allGamerCount - this.PrivateGamerCount;
        return 0;
      }
    }

    public int PrivateGamerCount
    {
      get
      {
        int num = 0;
        if (this.IsSessionOpen)
        {
          foreach (NetworkGamer tempAllGamer in this.tempAllGamerList)
          {
            if (tempAllGamer.IsPrivateSlot)
              ++num;
          }
        }
        return num;
      }
    }

    public void Draw()
    {
      this.DrawSend();
      this.DrawReceive();
      this.DrawWaiting();
    }

    private void DrawSend()
    {
    }

    private void DrawReceive()
    {
    }

    private void DrawWaiting()
    {
    }

    public enum BufferedChangeType
    {
      None,
      BlockChange,
      Blast,
      CreativeCommand,
      Zone,
    }

    public class BufferedChangeBase
    {
      public NetworkManager.BufferedChangeType Type;
      public long? ChunkHash;
      public List<long> ChunksList;
    }

    public class BufferedChangePoint : NetworkManager.BufferedChangeBase
    {
      public GamerID GamerID;
      public short X;
      public short Y;
      public short Z;
      public UpdateBlockMethod Method;
    }

    public class BufferedBlockChange : NetworkManager.BufferedChangePoint
    {
      public MapBlock OldBlockData;
      public MapBlock BlockData;
      public bool AuxChangeOnly;
    }

    public class BufferedBlast : NetworkManager.BufferedChangePoint
    {
      public Item ItemID;
      public float Strength;
      public byte Radius;
      public ushort Seed;
    }

    public class BufferedZone : NetworkManager.BufferedChangeBase
    {
      public ZoneEditType Action;
      public string Name;
      public GamerID GamerID;
      public ZoneType ZoneType;
      public GlobalPoint3D Min;
      public GlobalPoint3D Max;
      public ZoneBuilderType BuilderType;
      public string Builder;
      public string OnEntryScript;
      public string OnExitScript;
      public short CombatLevelDifference;
      public float SpeedMultiplier;
      public float GravityMultiplier;
    }

    public class BufferedCreativeCommand : NetworkManager.BufferedChangeBase
    {
      public GamerID GamerID;
      public GlobalPoint3D Point;
      public GlobalPoint3D Min;
      public GlobalPoint3D Max;
      public GlobalPoint3D XMin;
      public GlobalPoint3D XMax;
      public byte Percent;
      public int Seed;
      public Block BlockID;
      public Block BlockID1;
      public Block BlockID2;
      public CreativeCommand Command;
      public bool ClearMarkers;
      public string Desc;
      public object Data;
    }

    private struct GameDataSendReceive
    {
      public NetworkGamer Recipient;
      public byte[] GameData;
    }

    private struct PickupRequest
    {
      public GamerID GamerId;
      public int ParticleID;
    }

    public struct DataBlockChange
    {
      public DataBlock DataBlock;
      public bool IsClosed;
      public UpdateBlockMethod Method;
    }

    public class MachineData
    {
      public NetworkMachine Machine;
      public List<long> ChunkOutsideMapBound;
      public List<long> EditedChunkRequests;
      public List<long> UneditedChunkRequests;
      public List<long> HostNeedsToGenerateChunks;
      public Dictionary<long, long> HostChunkGenerationQueued;
      public PacketWriter PacketWriterNone;
      public PacketWriter PacketWriterInOrder;
      public PacketWriter PacketWriterReliable;
      public PacketWriter PacketWriterReliableInOrder;

      public MachineData(NetworkMachine machine)
      {
        this.Machine = machine;
        this.ChunkOutsideMapBound = new List<long>(100);
        this.EditedChunkRequests = new List<long>(100);
        this.UneditedChunkRequests = new List<long>(100);
        this.HostNeedsToGenerateChunks = new List<long>(100);
        this.HostChunkGenerationQueued = new Dictionary<long, long>(100);
        this.PacketWriterNone = new PacketWriter();
        this.PacketWriterInOrder = new PacketWriter();
        this.PacketWriterReliable = new PacketWriter();
        this.PacketWriterReliableInOrder = new PacketWriter();
      }

      public bool IsLocalMachine(INetworkSession session)
      {
        if (session != null && session.LocalGamers != null && session.LocalGamers.Count > 0)
          return this.Machine == session.LocalGamers[0].Machine;
        return false;
      }

      public NetworkGamer Host
      {
        get
        {
          if (this.Machine == null || this.Machine.Gamers == null || this.Machine.Gamers.Count <= 0)
            return (NetworkGamer) null;
          return this.Machine.Gamers[0];
        }
      }
    }

    private struct ChunkRequestsSent
    {
      public long Hash;
      public int Time;
    }

    private delegate void ReadInPacket(NetworkGamer sender);

    public enum PriceListChangeType
    {
      None,
      ShopUsesDefault,
      ShopCopyOfDefault,
      DefaultCopyOfShop,
    }

    private enum MiniGameDataType
    {
      Start,
      Abort,
      Timer,
      End,
      Results,
    }

    private enum WeatherType : byte
    {
      Fog,
      Rain,
      Hail,
    }
  }
}
