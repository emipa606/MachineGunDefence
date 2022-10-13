using RimWorld;
using Verse;
// Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
// RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
// RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace AAA;

[StaticConstructorOnStartup]
public class Building_TurretGunCustom : Building_TurretGun
{
    private const int TryStartShootSomethingIntervalTicks = 10;
    protected bool hasGainedLoadcount = false;

    // protected CompTurretDestroySelfAfterFire destroySelfComp;

    //protected CompTurretRemoteControl remoteControlComp;

    private bool holdFire;

    protected int lastLoadcount = 0;

    protected new TurretTop_CustomSize top;

    protected CompTurretTopSize topSizeComp;

    public Building_TurretGunCustom()
    {
        top = new TurretTop_CustomSize(this);
    }

    public CompTurretTopSize TopSizeComp => topSizeComp;

    private bool WarmingUp => burstWarmupTicksLeft > 0;

    public bool CanSetForcedTarget
    {
        get
        {
            if (mannableComp == null)
            {
                return false; //this.remoteControlComp != null;
            }

            return (Faction == Faction.OfPlayer || MannedByColonist) && !MannedByNonColonist;
        }
    }

    private bool CanToggleHoldFire => (Faction == Faction.OfPlayer || MannedByColonist) && !MannedByNonColonist;

    private bool MannedByColonist =>
        mannableComp?.ManningPawn != null && mannableComp.ManningPawn.Faction == Faction.OfPlayer;

    private bool MannedByNonColonist =>
        mannableComp?.ManningPawn != null && mannableComp.ManningPawn.Faction != Faction.OfPlayer;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        topSizeComp = GetComp<CompTurretTopSize>();
        // this.destroySelfComp = base.GetComp<CompTurretDestroySelfAfterFire>();
        //this.remoteControlComp = base.GetComp<CompTurretRemoteControl>();
    }

    /*protected int getCompLoadcount()
    {
        ThingDef projectile = this.gun.TryGetComp<CompChangeableProjectile>().Projectile;
        int count = projectile.comps.Count;
        int i = 0;
        while (i < count)
        {
            RecoillessRifle.CompProperties_ProjectileLoadCount compWhenLoaded = projectile.comps[i] as RecoillessRifle.CompProperties_ProjectileLoadCount;
            if (compWhenLoaded != null)
            {
                return compWhenLoaded.add_initial_loadcount;
            }
            i++;
        }
        return 1;
    }
    protected void AdjustLoadcount()
    {
        if (this.gun.TryGetComp<CompChangeableProjectile>().loadedCount == 1)
        {
            if (base.GunCompEq.PrimaryVerb.state == VerbState.Bursting)//!this.hasGainedLoadcount)
            {
                this.gun.TryGetComp<CompChangeableProjectile>().loadedCount = 2;
            }
        }
            /*if (this.gun.TryGetComp<CompChangeableProjectile>().loadedCount == 1)
            {
                if (base.GunCompEq.PrimaryVerb.state != VerbState.Bursting)//!this.hasGainedLoadcount)
                {
                    this.gun.TryGetComp<CompChangeableProjectile>().loadedCount = getCompLoadcount();
                    this.hasGainedLoadcount = true;//when new shell is loaded, check load count and increase it.
                }
                else
                {//next time is when you have shot some shell and only 1 shell is remaining. 
                    this.hasGainedLoadcount = false;//set false so that it can check loadcount when new shell is loaded
                }
            }
            else if (this.gun.TryGetComp<CompChangeableProjectile>().loadedCount == 0)
            {
                hasGainedLoadcount = false;
            }
    }*/
    public override void Tick()
    {
        /*  if (this.gun.TryGetComp<CompChangeableProjectile>().loadedCount != lastLoadcount)
         {
             lastLoadcount = this.gun.TryGetComp<CompChangeableProjectile>().loadedCount;

             AdjustLoadcount();
         }*/
        // AdjustLoadcount();


        base.Tick();
        if (forcedTarget.IsValid && !CanSetForcedTarget)
        {
            ResetForcedTarget();
        }

        if (!CanToggleHoldFire)
        {
            holdFire = false;
        }

        if (forcedTarget.ThingDestroyed)
        {
            ResetForcedTarget();
        }

        if ((powerComp == null || powerComp.PowerOn) && (mannableComp == null || mannableComp.MannedNow) && Spawned)
        {
            GunCompEq.verbTracker.VerbsTick();
            if (stunner.Stunned || GunCompEq.PrimaryVerb.state == VerbState.Bursting)
            {
                return;
            }

            if (WarmingUp)
            {
                burstWarmupTicksLeft--;
                if (burstWarmupTicksLeft == 0)
                {
                    BeginBurst();
                }
            }
            else
            {
                if (burstCooldownTicksLeft > 0)
                {
                    // if (this.destroySelfComp != null)
                    // {
                    //     this.Destroy(DestroyMode.Vanish);
                    //     return;
                    // }
                    burstCooldownTicksLeft--;
                }

                if (burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
                {
                    TryStartShootSomething(true);
                }
            }

            top.TurretTopTick();
        }
        else
        {
            ResetCurrentTarget();
        }
    }

    /*private IAttackTargetSearcher TargSearcher()
    {
        if (this.mannableComp != null && this.mannableComp.MannedNow)
        {
            return this.mannableComp.ManningPawn;
        }
        return this;
    }*/

    private bool IsValidTarget(Thing t)
    {
        if (t is not Pawn pawn)
        {
            return true;
        }

        if (GunCompEq.PrimaryVerb.ProjectileFliesOverhead())
        {
            var roofDef = Map.roofGrid.RoofAt(t.Position);
            if (roofDef is { isThickRoof: true })
            {
                return false;
            }
        }

        if (mannableComp == null)
        {
            return false; //!GenAI.MachinesLike(base.Faction, pawn);
        }

        return !pawn.RaceProps.Animal || pawn.Faction != Faction.OfPlayer;
    }

    private void ResetForcedTarget()
    {
        forcedTarget = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
        if (burstCooldownTicksLeft <= 0)
        {
            TryStartShootSomething(false);
        }
    }

    private void UpdateGunVerbs()
    {
        var allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
        foreach (var verb in allVerbs)
        {
            verb.caster = this;
            verb.castCompleteCallback = BurstComplete;
        }
    }

    private void ResetCurrentTarget()
    {
        currentTargetInt = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
    }

    public override void Draw()
    {
        top.DrawTurret();
        Comps_PostDraw();
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        var map = Map;
        AAA_GenLeavingAll.AAA_DoLeavingsFor(this, map, mode);
        base.Destroy(mode);
    }
}