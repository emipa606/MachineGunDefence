
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;         // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
using Verse;               // RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace AAA
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    public class Building_TurretGunCustom : Building_TurretGun
    {
        protected bool hasGainedLoadcount = false;
        protected int lastLoadcount = 0;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public CompTurretTopSize TopSizeComp
        {
            get
            {
                return this.topSizeComp;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        private bool WarmingUp
        {
            get
            {
                return this.burstWarmupTicksLeft > 0;
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002063 File Offset: 0x00000263
        public bool CanSetForcedTarget
        {
            get
            {
                if (this.mannableComp == null)
                {
                    return false;//this.remoteControlComp != null;
                }
                return (base.Faction == Faction.OfPlayer || this.MannedByColonist) && !this.MannedByNonColonist;
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002097 File Offset: 0x00000297
        private bool CanToggleHoldFire
        {
            get
            {
                return (base.Faction == Faction.OfPlayer || this.MannedByColonist) && !this.MannedByNonColonist;
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000005 RID: 5 RVA: 0x000020B9 File Offset: 0x000002B9
        private bool MannedByColonist
        {
            get
            {
                return this.mannableComp != null && this.mannableComp.ManningPawn != null && this.mannableComp.ManningPawn.Faction == Faction.OfPlayer;
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000006 RID: 6 RVA: 0x000020E9 File Offset: 0x000002E9
        private bool MannedByNonColonist
        {
            get
            {
                return this.mannableComp != null && this.mannableComp.ManningPawn != null && this.mannableComp.ManningPawn.Faction != Faction.OfPlayer;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000211C File Offset: 0x0000031C
        public Building_TurretGunCustom()
        {
            this.top = new TurretTop_CustomSize(this);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002130 File Offset: 0x00000330
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.topSizeComp = base.GetComp<CompTurretTopSize>();
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
         // Token: 0x06000009 RID: 9 RVA: 0x00002160 File Offset: 0x00000360
        public override void Tick()
        {
           /*  if (this.gun.TryGetComp<CompChangeableProjectile>().loadedCount != lastLoadcount)
            {
                lastLoadcount = this.gun.TryGetComp<CompChangeableProjectile>().loadedCount;

                AdjustLoadcount();
            }*/
           // AdjustLoadcount();


            base.Tick();
            if (this.forcedTarget.IsValid && !this.CanSetForcedTarget)
            {
                this.ResetForcedTarget();
            }
            if (!this.CanToggleHoldFire)
            {
                this.holdFire = false;
            }
            if (this.forcedTarget.ThingDestroyed)
            {
                this.ResetForcedTarget();
            }
            if ((this.powerComp == null || this.powerComp.PowerOn) && (this.mannableComp == null || this.mannableComp.MannedNow) && base.Spawned)
            {
                base.GunCompEq.verbTracker.VerbsTick();
                if (!this.stunner.Stunned && base.GunCompEq.PrimaryVerb.state != VerbState.Bursting)
                {
                    if (this.WarmingUp)
                    {
                        this.burstWarmupTicksLeft--;
                        if (this.burstWarmupTicksLeft == 0)
                        {
                            base.BeginBurst();
                        }
                    }
                    else
                    {
                        if (this.burstCooldownTicksLeft > 0)
                        {
                           // if (this.destroySelfComp != null)
                           // {
                           //     this.Destroy(DestroyMode.Vanish);
                           //     return;
                           // }
                            this.burstCooldownTicksLeft--;
                        }
                        if (this.burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
                        {
                            base.TryStartShootSomething(true);
                        }
                    }
                    this.top.TurretTopTick();
                    return;
                }
            }
            else
            {
                this.ResetCurrentTarget();
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002297 File Offset: 0x00000497
        /*private IAttackTargetSearcher TargSearcher()
        {
            if (this.mannableComp != null && this.mannableComp.MannedNow)
            {
                return this.mannableComp.ManningPawn;
            }
            return this;
        }*/

        // Token: 0x0600000B RID: 11 RVA: 0x000022BC File Offset: 0x000004BC
        private bool IsValidTarget(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                if (base.GunCompEq.PrimaryVerb.ProjectileFliesOverhead())
                {
                    RoofDef roofDef = base.Map.roofGrid.RoofAt(t.Position);
                    if (roofDef != null && roofDef.isThickRoof)
                    {
                        return false;
                    }
                }
                if (this.mannableComp == null)
                {
                    return false;//!GenAI.MachinesLike(base.Faction, pawn);
                }
                if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000233E File Offset: 0x0000053E
        private void ResetForcedTarget()
        {
            this.forcedTarget = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
            if (this.burstCooldownTicksLeft <= 0)
            {
                base.TryStartShootSomething(false);
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002364 File Offset: 0x00000564
        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = this.gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this;
                verb.castCompleteCallback = new Action(base.BurstComplete);
            }
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000023B2 File Offset: 0x000005B2
        private void ResetCurrentTarget()
        {
            this.currentTargetInt = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000023C6 File Offset: 0x000005C6
        public override void Draw()
        {
            this.top.DrawTurret();
            base.Comps_PostDraw();

           

        }
       public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            Map map = this.Map;
           AAA_GenLeavingAll.AAA_DoLeavingsFor(this, map, mode);
            base.Destroy(DestroyMode.Vanish);
        }
        // Token: 0x04000001 RID: 1
        protected new TurretTop_CustomSize top;

        // Token: 0x04000002 RID: 2
        protected CompTurretTopSize topSizeComp;

        // Token: 0x04000003 RID: 3
       // protected CompTurretDestroySelfAfterFire destroySelfComp;

        // Token: 0x04000004 RID: 4
        //protected CompTurretRemoteControl remoteControlComp;

        // Token: 0x04000005 RID: 5
        private bool holdFire;

        // Token: 0x04000006 RID: 6
        private const int TryStartShootSomethingIntervalTicks = 10;
    }
}
