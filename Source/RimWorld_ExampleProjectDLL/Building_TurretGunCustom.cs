using RimWorld;
using UnityEngine;
using Verse;

namespace AAA;

[StaticConstructorOnStartup]
public class Building_TurretGunCustom : Building_TurretGun
{
    private new readonly TurretTop_CustomSize top;

    // protected CompTurretDestroySelfAfterFire destroySelfComp;

    //protected CompTurretRemoteControl remoteControlComp;

    private bool holdFire;

    public Building_TurretGunCustom()
    {
        top = new TurretTop_CustomSize(this);
    }

    public CompTurretTopSize TopSizeComp { get; private set; }

    private bool WarmingUp => burstWarmupTicksLeft > 0;

    protected override bool CanSetForcedTarget
    {
        get
        {
            if (mannableComp == null)
            {
                return false;
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
        TopSizeComp = GetComp<CompTurretTopSize>();
    }

    protected override void Tick()
    {
        base.Tick();
        if (forcedTarget.IsValid && !CanSetForcedTarget)
        {
            resetForcedTarget();
        }

        if (!CanToggleHoldFire)
        {
            holdFire = false;
        }

        if (forcedTarget.ThingDestroyed)
        {
            resetForcedTarget();
        }

        if ((powerComp == null || powerComp.PowerOn) && (mannableComp == null || mannableComp.MannedNow) && Spawned)
        {
            GunCompEq.verbTracker.VerbsTick();
            if (IsStunned || GunCompEq.PrimaryVerb.state == VerbState.Bursting)
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
            resetCurrentTarget();
        }
    }

    private void resetForcedTarget()
    {
        forcedTarget = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
        if (burstCooldownTicksLeft <= 0)
        {
            TryStartShootSomething(false);
        }
    }

    private void resetCurrentTarget()
    {
        currentTargetInt = LocalTargetInfo.Invalid;
        burstWarmupTicksLeft = 0;
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
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