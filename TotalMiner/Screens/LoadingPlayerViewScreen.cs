// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Screens.LoadingPlayerViewScreen
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.GameState;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.Net;

namespace StudioForge.TotalMiner.Screens
{
  internal class LoadingPlayerViewScreen : LoadingScreenBase
  {
    private bool firstUpdate = true;
    private string[] prognosisList = new string[6]
    {
      "No data",
      "Low",
      "Average",
      "Very good",
      "Excellent",
      "Extraordinary"
    };
    private ChunkLoader chunkLoader;
    private int updateProgressBarTimer;
    private int generateCount;
    private int decorateCount;
    private int pendingCount;
    private int lightingCount;
    private int loadMeshCount;
    private int startGenerateCount;
    private int startDecorateCount;
    private int startPendingCount;
    private int startLightingCount;
    private int startLoadMeshCount;
    private int totalStartCount;
    private bool abort;
    private GameScreen baseScreen;
    private PropertyToString<int> bytesRecdPerSec;
    private bool drawConnectionStatus;
    private string connectionStatusString;
    private bool playerJustJoined;

    public LoadingPlayerViewScreen(
      Player player,
      ChunkLoader chunkLoader,
      GameScreen baseScreen,
      bool playerJustJoined)
      : base(player)
    {
      this.chunkLoader = chunkLoader;
      this.baseScreen = baseScreen;
      this.playerJustJoined = playerJustJoined;
      player.IsViewLoading = true;
      player.IsEnabled = player.IsEnabledField = false;
      ((IProgressBar) this).Text = "Generating World...";
      this.bytesRecdPerSec = new PropertyToString<int>()
      {
        Format = "Data: {0} bps"
      };
    }

    protected override void UpdateMatrix()
    {
      base.UpdateMatrix();
      if (this.baseScreen == null)
        return;
      this.baseScreen.Matrix = this.Matrix;
    }

    public override bool HandleInput(InputState input)
    {
      if (input.IsNewButtonPress(Buttons.Start, this.ControllingPlayer.Value))
        return this.abort = true;
      return base.HandleInput(input);
    }

    protected override void UpdateCore(bool coveredByOtherScreen)
    {
      base.UpdateCore(coveredByOtherScreen);
      this.player.IsBusy = false;
      if (++this.updateProgressBarTimer > 45)
      {
        if (this.chunkLoader != null)
        {
          this.chunkLoader.CalculateImmediateChunksToGenerate(this.player, out this.generateCount, out this.decorateCount, out this.pendingCount, out this.lightingCount, out this.loadMeshCount);
          if (this.generateCount == 0 && this.decorateCount == 0 && (this.pendingCount == 0 && this.player != null) && (this.player.GameInstance != null && this.player.GameInstance.Map != null))
            this.player.GameInstance.Map.LoadingUpdate();
        }
        else
          this.abort = true;
        if (this.abort || this.loadMeshCount == 0)
        {
          this.baseScreen.ExitScreen();
          this.ExitScreen();
          this.SetImmediateChunksAlpha(byte.MaxValue);
          this.player.IsEnabled = this.player.IsEnabledField = true;
          this.player.GameInstance.NetworkManager.BuildGamerList();
          this.player.GameInstance.NetworkManager.SendGameInstanceDataRequest();
          this.player.ResetScreenTransition();
          this.player.IsViewLoading = false;
          if (this.playerJustJoined)
            this.player.GameInstance.ExecuteEventScript(ScriptEvent.PlayerJoin, new ScriptExecuteData()
            {
              Actor = (Actor) this.player
            });
        }
        if (this.firstUpdate)
        {
          this.firstUpdate = false;
          this.startGenerateCount = this.generateCount;
          this.startDecorateCount = this.decorateCount;
          this.startPendingCount = this.pendingCount;
          this.startLightingCount = this.lightingCount;
          this.startLoadMeshCount = this.loadMeshCount;
          this.totalStartCount = this.startGenerateCount + this.startDecorateCount + this.startPendingCount + this.startLightingCount + this.startLoadMeshCount;
        }
        this.progressValue = (float) (this.startGenerateCount - this.generateCount + (this.startDecorateCount - this.decorateCount) + (this.startPendingCount - this.pendingCount) + (this.startLightingCount - this.lightingCount) + (this.startLoadMeshCount - this.loadMeshCount)) / (float) this.totalStartCount;
        this.updateProgressBarTimer = 0;
      }
      this.drawConnectionStatus = false;
      if (this.player.GameInstance.Map.IsHost)
        return;
      int num = NetworkManager.Instance.IsSessionOpenAndNotLocal ? 1 : 0;
    }

    private void SetImmediateChunksAlpha(byte alpha)
    {
      MapTM map = this.player.GameInstance.Map;
      GlobalPoint3D point = map.GetPoint(this.player.Position);
      ++point.Y;
      GlobalPoint3D globalPoint3D1 = point - map.ChunkSize;
      GlobalPoint3D globalPoint3D2 = point + map.ChunkSize;
      for (point.Y = globalPoint3D1.Y; point.Y < globalPoint3D2.Y; point.Y += map.ChunkSize.Y)
      {
        for (point.Z = globalPoint3D1.Z; point.Z < globalPoint3D2.Z; point.Z += map.ChunkSize.Z)
        {
          for (point.X = globalPoint3D1.X; point.X < globalPoint3D2.X; point.X += map.ChunkSize.X)
          {
            MapChunkTM chunk = map.GetChunk(point) as MapChunkTM;
            if (chunk != null)
              chunk.Content.Alpha = alpha;
          }
        }
      }
    }

    protected override void DrawCore()
    {
      base.DrawCore();
      if (!this.drawConnectionStatus || this.connectionStatusString == null)
        return;
      this.SpriteBatch.BeginTM(this.Matrix);
      float scale = 0.5f;
      this.SpriteBatch.DrawString(this.Font, this.connectionStatusString, new Vector2((float) (1116.0 - (double) this.Font.MeasureString(this.connectionStatusString).X * (double) scale), 579f), Color.DeepSkyBlue, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
      this.SpriteBatch.End();
    }
  }
}
