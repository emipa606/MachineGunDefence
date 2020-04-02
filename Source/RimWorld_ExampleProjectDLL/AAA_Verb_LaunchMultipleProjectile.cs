using System;
using System.Collections.Generic;
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

namespace Verse// AAA
{    //copied from  Verb_LaunchProjectile
     //make and launch more projctiles than original does

    public class AAA_Verb_LaunchMultipleProjectile : Verb
    {
        // No real function, just test of tooltip

        // public bool IsShotgun
        //{
        //get
        //   {
        //        return this.IsShrapnel;
        //}
        //}
        //public override Verb AttackVerb
        //{
        //get
        //   {
        //      if (this.IsShotgun)
        //     {
        //        this.GunCompEq.verbTracker.PrimaryVerb.verbProps.defaultProjectile = this.defaultShrapnelProjectile;
        //       return this.GunCompEq.verbTracker.PrimaryVerb;
        //}
        //          else
        //         {
        //            return this.GunCompEq.verbTracker.PrimaryVerb;
        //}
        //}
        //copied from  Verb_LaunchProjectile
        //make and launch more projctiles than original does

        // (get) Token: 0x060057CA RID: 22474 RVA: 0x0017B7C4 File Offset: 0x00179BC4
        public virtual ThingDef Projectile
        {
            get
            {
                if (this.EquipmentSource != null)
                {
                    CompChangeableProjectile comp = this.EquipmentSource.GetComp<CompChangeableProjectile>();
                    if (comp != null && comp.Loaded)
                    {
                        return comp.Projectile;
                    }
                }
                return this.verbProps.defaultProjectile;
            }
        }
        public int PelletsPerShot(ThingDef projectile)
        {
            if (projectile.comps != null)
            {
                //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
                int i = 0;
                int count = projectile.comps.Count;
                while (i < count)
                {
                    AAA.CompProperties_ProjectileMultiple compWhenLoaded = projectile.comps[i] as AAA.CompProperties_ProjectileMultiple;
                    if (compWhenLoaded != null)
                    {
                        return compWhenLoaded.pellets;
                    }
                    i++;
                }
            }
            return 1;
        }
        public float ForsedScatterRadius(ThingDef projectile)
        {
            if (projectile.comps != null)
            {
                //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
                int i = 0;
                int count = projectile.comps.Count;
                while (i < count)
                {
                    AAA.CompProperties_ProjectileMultiple compWhenLoaded = projectile.comps[i] as AAA.CompProperties_ProjectileMultiple;
                    if (compWhenLoaded != null)
                    {
                        return compWhenLoaded.forsedScatterRadius;
                    }
                    i++;
                }
            }
            return 0.0f;
        }
        public float ScatterRadiusAt10tilesAway(ThingDef projectile)
        {
            if (projectile.comps != null)
            {
                //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
                int i = 0;
                int count = projectile.comps.Count;
                while (i < count)
                {
                    AAA.CompProperties_ProjectileMultiple compWhenLoaded = projectile.comps[i] as AAA.CompProperties_ProjectileMultiple;
                    if (compWhenLoaded != null)
                    {
                        return compWhenLoaded.scatterRadiusAt10tilesAway;
                    }
                    i++;
                }
            }
            return 0.0f;
        }
        // Token: 0x060057CB RID: 22475 RVA: 0x0017B80C File Offset: 0x00179C0C
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster, (!this.currentTarget.HasThing) ? null : this.currentTarget.Thing, (this.EquipmentSource == null) ? null : this.EquipmentSource.def, this.Projectile, this.ShotsPerBurst > 1));
        }
        protected override bool TryCastShot()
        {


            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }
            //  ThingDef projectile = this.Projectile;
            ThingDef projectile = this.Projectile;
         //   projectile.projectileWhenLoaded.
            if (projectile == null)
            {
                return false;
            }

            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
            
         
            if (this.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            if (this.EquipmentSource != null)
            {
                CompChangeableProjectile comp = this.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            Thing launcher = this.caster;
            Thing equipment = this.EquipmentSource;
            CompMannable compMannable = this.caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                launcher = compMannable.ManningPawn;
                equipment = this.caster;
            }
            Vector3 drawPos = this.caster.DrawPos;

            Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, this.caster.Map);
        

            //---------------
            int pellets = this.PelletsPerShot(projectile);
            if (pellets < 1) pellets = 1;
            
            Projectile[] projectiles = new Projectile[pellets];
            ShootLine[] shootLines = new ShootLine[pellets];
            for (int i = 0; i < pellets; i++)
            {
                base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLines[i]);
                projectiles[i] = (Projectile)GenSpawn.Spawn(projectile, shootLines[i].Source, this.caster.Map);
                //projectiles[i].FreeIntercept = (this.canFreeInterceptNow && !projectiles[i].def.projectile.flyOverhead);
            }

            //projectile2.FreeIntercept = (this.canFreeInterceptNow && !projectile2.def.projectile.flyOverhead);
            //projectile3.FreeIntercept = (this.canFreeInterceptNow && !projectile2.def.projectile.flyOverhead);
            //projectile4.FreeIntercept = (this.canFreeInterceptNow && !projectile2.def.projectile.flyOverhead);
            float distance = (float)(this.currentTarget.Cell - this.caster.Position).LengthHorizontal;
            float scatter = ScatterRadiusAt10tilesAway(projectile) * distance / 10.0f;
            float missRadius = this.verbProps.forcedMissRadius + ForsedScatterRadius(projectile) + scatter;
            for (int i = 0; i < pellets; i++)
            {
                if (missRadius > 0.5f)
                {
                    float num = (float)(this.currentTarget.Cell - this.caster.Position).LengthHorizontalSquared;
                    float num2;
                    if (num < 9f)
                    {
                        num2 = 0f;
                    }
                    else if (num < 25f)
                    {
                        num2 = missRadius * 0.5f ;
                    }
                    else if (num < 49f)
                    {
                        num2 = missRadius * 0.8f;
                    }
                    else
                    {
                        num2 = missRadius * 1f ;
                    }
                    if (num2 > 0.5f)
                    {
                        int max = GenRadial.NumCellsInRadius(missRadius);
                        //int num3 = Rand.Range(0, max);
                        int num3 = Rand.Range(0, max);
                        if (num3 > 0)
                        {
                            if (DebugViewSettings.drawShooting)
                            {
                                MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToForRad", -1f);
                            }
                            //if (this.currentTarget.HasThing)
                            //{
                            //    // projectile2.ThingToNeverIntercept = this.currentTarget.Thing;
                                
                            //   projectiles[i].ThingToNeverIntercept = this.currentTarget.Thing;
                            //}
                            
                            //if (!projectiles[i].def.projectile.flyOverhead)
                            //{
                            //        projectiles[i].InterceptWalls = true;
                            //}
                            IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num3];
                            projectiles[i].Launch(launcher, origin: drawPos, usedTarget: new LocalTargetInfo(c), intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                            //projectile2.Launch(launcher, drawPos, c, equipment, this.currentTarget.Thing);
                            //projectile3.Launch(launcher, drawPos, c, equipment, this.currentTarget.Thing);
                            //projectile4.Launch(launcher, drawPos, c, equipment, this.currentTarget.Thing);
                            continue;// return true;
                        }
                        else
                        {
                            projectiles[i].Launch(launcher, origin: drawPos, usedTarget: new LocalTargetInfo(this.currentTarget.Cell), intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                            //projectiles[i].Launch(launcher, drawPos, this.currentTarget.Cell, equipment, this.currentTarget.Thing);
                            continue;
                        }
                    }
                }
                ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
                if (Rand.Value > shotReport.AimOnTargetChance_IgnoringPosture)
                {
                    if (DebugViewSettings.drawShooting)
                    {
                        MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToWild", -1f);
                    }

                    shootLines[i].ChangeDestToMissWild(shotReport.AimOnTargetChance);
                    // shootLine2.ChangeDestToMissWild();

                    if (this.currentTarget.HasThing)
                    {
                       projectiles[i].HitFlags = ProjectileHitFlags.All;
                        // projectile2.ThingToNeverIntercept = this.currentTarget.Thing;
                    }
                    if (!projectiles[i].def.projectile.flyOverhead)
                    {
                        projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
                    }
                    projectiles[i].Launch(launcher, origin: drawPos, usedTarget: new LocalTargetInfo(shootLines[i].Dest), intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                    //projectiles[i].Launch(launcher, drawPos, shootLines[i].Dest, equipment, this.currentTarget.Thing);
                    //projectile2.Launch(launcher, drawPos, shootLine2.Dest, equipment, this.currentTarget.Thing);
                    continue;//return true;
                }
                if (Rand.Value > shotReport.PassCoverChance)
                {
                    if (DebugViewSettings.drawShooting)
                    {
                        MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToCover", -1f);
                    }
                    if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn)
                    {
                        Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                        if (!projectiles[i].def.projectile.flyOverhead)
                        {
                          projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
                        }
                        projectiles[i].Launch(launcher, origin: drawPos, usedTarget: new LocalTargetInfo(randomCoverToMissInto), intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                        //projectiles[i].Launch(launcher, drawPos, randomCoverToMissInto, equipment, this.currentTarget.Thing);
                        //projectile2.Launch(launcher, drawPos, randomCoverToMissInto, equipment, this.currentTarget.Thing);
                        continue;//return true;
                    }
                }
                if (DebugViewSettings.drawShooting)
                {
                    MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToHit", -1f);
                }

                if (!projectiles[i].def.projectile.flyOverhead && (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full))
                {
                    projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
                }

                if (this.currentTarget.Thing != null)
                {
                    projectiles[i].Launch(launcher, origin: drawPos, usedTarget: currentTarget, intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                    //projectiles[i].Launch(launcher, drawPos, this.currentTarget, equipment, this.currentTarget.Thing);
                }
                else
                {
                    projectiles[i].Launch(launcher, origin: drawPos, usedTarget: new LocalTargetInfo(shootLines[i].Dest), intendedTarget: currentTarget, hitFlags: projectiles[i].HitFlags, equipment: equipment);
                    //projectiles[i].Launch(launcher, drawPos, shootLines[i].Dest, equipment, this.currentTarget.Thing);
                }

              
            }
           
            return true;
        }
    }
}

