// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.ArcadeMachine
// Assembly: StudioForge.TotalMiner.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F0E1DDE7-D62D-405E-BA66-AD2EA8491117
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.API.dll

using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner.API;

namespace StudioForge.TotalMiner
{
  public abstract class ArcadeMachine
  {
    public GlobalPoint3D Point;
    public BlockFace Face;
    public int GpPerCredit;
    public int Credits;
    public string CreditText;
    public IArcadeMachineRenderer Renderer;
    protected ITMMap map;
    protected ITMGame game;
    protected RenderTarget2D renderTarget;
    protected ITMPlayer tmPlayer;

    public bool IsEnabled { get; set; }

    public RenderTarget2D RenderTarget
    {
      get
      {
        return this.renderTarget;
      }
    }

    public abstract bool CanDeactivate { get; }

    public ITMPlayer TMPlayer
    {
      get
      {
        return this.tmPlayer;
      }
    }

    public ArcadeMachine(
      ITMGame game,
      ITMMap map,
      ITMPlayer player,
      GlobalPoint3D point,
      BlockFace face)
    {
      this.game = game;
      this.map = map;
      this.tmPlayer = player;
      this.Point = point;
      this.Face = face;
      this.GpPerCredit = 1;
    }

    public virtual void Initialize(InitState state)
    {
    }

    public virtual void LoadContent(InitState state)
    {
      this.CreateRenderTarget();
      this.Credits = 0;
      this.CreditText = "Credits: 0";
    }

    protected virtual void CreateRenderTarget()
    {
      this.renderTarget = new RenderTarget2D(CoreGlobals.GraphicsDevice, 320, 240, false, SurfaceFormat.Bgra5551, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
    }

    public virtual void UnloadContent()
    {
      if (this.renderTarget == null)
        return;
      this.renderTarget.Dispose();
      this.renderTarget = (RenderTarget2D) null;
    }

    public virtual void UpdateState(int highscore, string highscoreGamer, string highscoreVersion)
    {
    }

    public void PlayerHitBlock(ITMPlayer player, Item itemID)
    {
      if (itemID == Item.GoldPieces && this.GpPerCredit > 0)
        this.AddCredit(player);
      else
        this.StartGame();
    }

    private bool AddCredit(ITMPlayer player)
    {
      if (player.Inventory.ItemCount(Item.GoldPieces) >= this.GpPerCredit)
      {
        this.ChangeCredits(1);
        player.Inventory.DecrementItem(Item.GoldPieces, this.GpPerCredit);
        CoreGlobals.AudioManager.PlaySound("ArcadeInsertCoin");
      }
      return false;
    }

    public abstract void StartGame();

    protected void ChangeCredits(int inc)
    {
      this.Credits += inc;
      this.CreditText = "Credits: " + this.Credits.ToString();
    }

    public abstract bool HandleInput();

    public abstract void Update();
  }
}
