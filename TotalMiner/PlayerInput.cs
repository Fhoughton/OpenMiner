// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.PlayerInput
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

namespace StudioForge.TotalMiner
{
  public enum PlayerInput
  {
    None = 0,
    MoveForward = 1,
    MoveBackward = 2,
    MoveLeft = 3,
    MoveRight = 4,
    Jump = 5,
    Crouch = 6,
    CrouchHold = 7,
    Fly = 20, // 0x00000014
    FlyAscend = 21, // 0x00000015
    FlyDescend = 22, // 0x00000016
    LeftHand = 30, // 0x0000001E
    RightHand = 31, // 0x0000001F
    HotbarLeft = 32, // 0x00000020
    HotbarRight = 33, // 0x00000021
    DropLeftItem = 34, // 0x00000022
    DropRightItem = 35, // 0x00000023
    Interact = 50, // 0x00000032
    BackButton = 51, // 0x00000033
    Special = 52, // 0x00000034
    ZoomIn = 53, // 0x00000035
    ZoomOut = 54, // 0x00000036
    OpenPauseMenu = 80, // 0x00000050
    OpenCreativeMenu = 81, // 0x00000051
    OpenInventory = 82, // 0x00000052
    OpenCrafting = 83, // 0x00000053
    OpenShop = 84, // 0x00000054
    OpenTextChat = 85, // 0x00000055
    OpenMap = 86, // 0x00000056
    OpenConsole = 87, // 0x00000057
    EventScriptX = 100, // 0x00000064
    EventScriptY = 101, // 0x00000065
    EventScriptB = 102, // 0x00000066
    ClipboardZoom = 120, // 0x00000078
    ClipboardRotate = 121, // 0x00000079
    ClipboardPasteMerge = 122, // 0x0000007A
    ClipboardPasteNoOverwrite = 123, // 0x0000007B
    SpectatePrev = 130, // 0x00000082
    SpectateNext = 131, // 0x00000083
    NoClip = 140, // 0x0000008C
    RebuildLocalLight = 141, // 0x0000008D
    ArcadeFireWeapon = 150, // 0x00000096
    zLast = 200, // 0x000000C8
  }
}
