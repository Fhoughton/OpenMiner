// Decompiled with JetBrains decompiler
// Type: StudioForge.TotalMiner.Achievements.Unlockables
// Assembly: StudioForge.TotalMiner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D984B6D0-261B-49FC-9611-486D3599BC4D
// Assembly location: D:\SteamLibrary\steamapps\common\Total Miner\StudioForge.TotalMiner.exe

namespace StudioForge.TotalMiner.Achievements
{
  internal class Unlockables
  {
    public Unlockable[] UnlockableList;

    public Unlockables(Player player)
    {
      this.UnlockableList = new Unlockable[75]
      {
        (Unlockable) new Boy(player),
        (Unlockable) new Girl(player),
        (Unlockable) new Original(player),
        (Unlockable) new Alien(player),
        (Unlockable) new Bomberman(player),
        (Unlockable) new ElfBoy(player),
        (Unlockable) new ElfGirl(player),
        (Unlockable) new Farmer(player),
        (Unlockable) new Hobo(player),
        (Unlockable) new Guard(player),
        (Unlockable) new Templar(player),
        (Unlockable) new Dwarf1(player),
        (Unlockable) new Dwarf2(player),
        (Unlockable) new Dwarf3(player),
        (Unlockable) new Dwarf4(player),
        (Unlockable) new Dwarf5(player),
        (Unlockable) new Dwarf6(player),
        (Unlockable) new Spider(player),
        (Unlockable) new Dryad(player),
        (Unlockable) new Djinn(player),
        (Unlockable) new Goblin(player),
        (Unlockable) new Orc(player),
        (Unlockable) new Skeleton(player),
        (Unlockable) new TrollBoy(player),
        (Unlockable) new TrollChief(player),
        (Unlockable) new TrollGirl(player),
        (Unlockable) new Werewolf(player),
        (Unlockable) new Policeman(player),
        (Unlockable) new Fisherman(player),
        (Unlockable) new Sailor(player),
        (Unlockable) new Duck(player),
        (Unlockable) new Sheep(player),
        (Unlockable) new Alpaca(player),
        (Unlockable) new AyrshireCow(player),
        (Unlockable) new HighlandCow(player),
        (Unlockable) new Girl2(player),
        (Unlockable) new Girl3(player),
        (Unlockable) new Girl4(player),
        (Unlockable) new Princess(player),
        (Unlockable) new Explorer(player),
        (Unlockable) new TreeHugger(player),
        (Unlockable) new Carpenter(player),
        (Unlockable) new Prisoner(player),
        (Unlockable) new Pupil(player),
        (Unlockable) new Jamaican(player),
        (Unlockable) new Lumberjack(player),
        (Unlockable) new Pirate(player),
        (Unlockable) new Refugee(player),
        (Unlockable) new King(player),
        (Unlockable) new Astronaut(player),
        (Unlockable) new Ninja(player),
        (Unlockable) new Caveman(player),
        (Unlockable) new Chef(player),
        (Unlockable) new InvaderMan(player),
        (Unlockable) new Hippie(player),
        (Unlockable) new Madman(player),
        (Unlockable) new Medic(player),
        (Unlockable) new Diablo(player),
        (Unlockable) new Angel(player),
        (Unlockable) new Cowboy(player),
        (Unlockable) new Entrepreneur(player),
        (Unlockable) new GoldenKnight(player),
        (Unlockable) new Handyman(player),
        (Unlockable) new Indian(player),
        (Unlockable) new Sage(player),
        (Unlockable) new Knight(player),
        (Unlockable) new Soldier(player),
        (Unlockable) new Terminator(player),
        (Unlockable) new Zombie(player),
        null,
        null,
        (Unlockable) new Robotic(player),
        (Unlockable) new HermesWraith(player),
        (Unlockable) new TesterMan(player),
        (Unlockable) new Zeus(player)
      };
      DemiGod demiGod = new DemiGod(player);
      DemiGoddess demiGoddess = new DemiGoddess(player, demiGod);
      this.UnlockableList[69] = (Unlockable) demiGod;
      this.UnlockableList[70] = (Unlockable) demiGoddess;
      demiGod.HookEvents2(this.UnlockableList);
    }

    public Unlockable GetUnlockable(ActorType mobType)
    {
      foreach (Unlockable unlockable in this.UnlockableList)
      {
        if (unlockable.ActorType == mobType)
          return unlockable;
      }
      return (Unlockable) null;
    }

    public static class Requirement
    {
      public const string Boy = "None.";
      public const string Girl = "None.";
      public const string Girl2 = "Go shopping.";
      public const string Girl3 = "None.";
      public const string Girl4 = "None.";
      public const string Original = "None.";
      public const string Alien = "Undefined.";
      public const string Astronaut = "Visit 10 remote worlds.";
      public const string Angel = "Save 15 players from death by killing\nthe enemy preying on them.";
      public const string Bomberman = "Undefined.";
      public const string Carpenter = "Craft your first Workbench.";
      public const string Caveman = "Mine 500 blocks the old fashioned way,\nwith a wood pickaxe.";
      public const string Chef = "Craft every ingredient and\nCook every food type at least once.";
      public const string Cowboy = "Kill 50 enemies while hanging\nfrom a rope.";
      public const string DemiGod = "Various requirements.";
      public const string DemiGoddess = "Unlock DemiGod.";
      public const string Diablo = "Kill me, with a Ruby sword.";
      public const string Explorer = "Find your first Blueprint\nand your first Wisdom Scroll.";
      public const string Entrepreneur = "Earn 1 Million in Gold.";
      public const string Farmer = "None.";
      public const string GoldenKnight = "Have 300 remote visitors\nrate your world.";
      public const string Handyman = "Craft every craftable item\nat least once.";
      public const string HermesWraith = "Unique Content Contributors.";
      public const string Hippie = "Throw 30 stacks of flowers at an\nenemy.";
      public const string Indian = "Craft a Wood Bow. Craft 200 Arrows.\nKill 50 enemies using your Bow.";
      public const string InvaderMan = "20k points on Total Invaders\n1.75m points on Total Rush.";
      public const string Jamaican = "Use the Rasta block with the\nCreative Fill feature.";
      public const string King = "Host an online game with at least\n10 concurrent players.";
      public const string Knight = "Reach Bedrock and kill at least\n50 enemies along the way.";
      public const string Lumberjack = "Plant 20 saplings, chop 20 replanted\ntrees and craft 80 Wood Planks.";
      public const string Madman = "Detonate 500 explosives.";
      public const string Medic = "Heal yourself 50 times, and heal\nanother player 50 times.";
      public const string Ninja = "Kill 20 unique remote players without\ndying.";
      public const string Pirate = "Open 30 treasure chests.";
      public const string Princess = "None.";
      public const string Prisoner = "Escape to the surface.";
      public const string Psycho = "Throw a fish at a sleeping player.";
      public const string Pupil = "Read every How To instruction.";
      public const string Refugee = "Kill your first remote player over\nXbox Live.";
      public const string Robotic = "No longer unlockable.";
      public const string Sage = "Find every Wisdom Scroll.";
      public const string Soldier = "Craft a Grenade Launcher. Craft 50\nGrenades. Launch 50 Grenades.";
      public const string Terminator = "Kill 200 enemies with the Grenade\nLauncher.";
      public const string TesterMan = "Testers only.";
      public const string TreeHugger = "Chop down a tree.";
      public const string Viking = "Undefined.";
      public const string Zeus = "Developers only.";
      public const string Zombie = "Survive the first 5 nights\nwithout sleeping.";
    }
  }
}
