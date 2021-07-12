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

namespace AAA
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    public class Building_TurretGunCustom : Building_TurretGun
    {
        // Token: 0x04000006 RID: 6
        private const int TryStartShootSomethingIntervalTicks = 10;
        protected bool hasGainedLoadcount = false;

        // Token: 0x04000003 RID: 3
        // protected CompTurretDestroySelfAfterFire destroySelfComp;

        // Token: 0x04000004 RID: 4
        //protected CompTurretRemoteControl remoteControlComp;

        // Token: 0x04000005 RID: 5
        private bool holdFire;

        protected int lastLoadcount = 0;

        // Token: 0x04000001 RID: 1
        protected new TurretTop_CustomSize top;

        // Token: 0x04000002 RID: 2
        protected CompTurretTopSize topSizeComp;

        // Token: 0x06000007 RID: 7 RVA: 0x0000211C File Offset: 0x0000031C
        public Building_TurretGunCustom()
        {
            top = new TurretTop_CustomSize(this);
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public CompTurretTopSize TopSizeComp => topSizeComp;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        private bool WarmingUp => burstWarmupTicksLeft > 0;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002063 File Offset: 0x00000263
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

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002097 File Offset: 0x00000297
        private bool CanToggleHoldFire => (Faction == Faction.OfPlayer || MannedByColonist) && !MannedByNonColonist;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000005 RID: 5 RVA: 0x000020B9 File Offset: 0x000002B9
        private bool MannedByColonist =>
            mannableComp?.ManningPawn != null && mannableComp.ManningPawn.Faction == Faction.OfPlayer;

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000006 RID: 6 RVA: 0x000020E9 File Offset: 0x000002E9
        private bool MannedByNonColonist =>
            mannableComp?.ManningPawn != null && mannableComp.ManningPawn.Faction != Faction.OfPlayer;

        // Token: 0x06000008 RID: 8 RVA: 0x00002130 File Offset: 0x00000330
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
            if (t is not Pawn pawn)
            {
                return true;
            }

            if (GunCompEq.PrimaryVerb.ProjectileFliesOverhead())
            {
                var roofDef = Map.roofGrid.RoofAt(t.Position);
                if (roofDef != null && roofDef.isThickRoof)
                {
                    return false;
                }
            }

            if (mannableComp == null)
            {
                return false; //!GenAI.MachinesLike(base.Faction, pawn);
            }

            if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)
            {
                return false;
            }

            return true;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000233E File Offset: 0x0000053E
        private void ResetForcedTarget()
        {
            forcedTarget = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
            if (burstCooldownTicksLeft <= 0)
            {
                TryStartShootSomething(false);
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002364 File Offset: 0x00000564
        private void UpdateGunVerbs()
        {
            var allVerbs = gun.TryGetComp<CompEquippable>().AllVerbs;
            foreach (var verb in allVerbs)
            {
                verb.caster = this;
                verb.castCompleteCallback = BurstComplete;
            }
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000023B2 File Offset: 0x000005B2
        private void ResetCurrentTarget()
        {
            currentTargetInt = LocalTargetInfo.Invalid;
            burstWarmupTicksLeft = 0;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000023C6 File Offset: 0x000005C6
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
}