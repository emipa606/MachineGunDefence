using AAA;
using RimWorld;
// Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
// RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
// RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace Verse;
// AAA

//copied from  Verb_LaunchProjectile
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
    //
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

    protected virtual ThingDef Projectile
    {
        get
        {
            if (EquipmentSource == null)
            {
                return verbProps.defaultProjectile;
            }

            var comp = EquipmentSource.GetComp<CompChangeableProjectile>();
            return comp is { Loaded: true } ? comp.Projectile : verbProps.defaultProjectile;
        }
    }

    private static int pelletsPerShot(ThingDef projectile)
    {
        if (projectile.comps == null)
        {
            return 1;
        }

        //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
        var i = 0;
        var count = projectile.comps.Count;
        while (i < count)
        {
            if (projectile.comps[i] is CompProperties_ProjectileMultiple compWhenLoaded)
            {
                return compWhenLoaded.pellets;
            }

            i++;
        }

        return 1;
    }

    private static float forsedScatterRadius(ThingDef projectile)
    {
        if (projectile.comps == null)
        {
            return 0.0f;
        }

        //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
        var i = 0;
        var count = projectile.comps.Count;
        while (i < count)
        {
            if (projectile.comps[i] is CompProperties_ProjectileMultiple compWhenLoaded)
            {
                return compWhenLoaded.forsedScatterRadius;
            }

            i++;
        }

        return 0.0f;
    }

    private static float scatterRadiusAt10TilesAway(ThingDef projectile)
    {
        if (projectile.comps == null)
        {
            return 0.0f;
        }

        //CompChangeableProjectile comp = this.ownerEquipment.GetComp<CompChangeableProjectile >();
        var i = 0;
        var count = projectile.comps.Count;
        while (i < count)
        {
            if (projectile.comps[i] is CompProperties_ProjectileMultiple compWhenLoaded)
            {
                return compWhenLoaded.scatterRadiusAt10tilesAway;
            }

            i++;
        }

        return 0.0f;
    }

    public override void WarmupComplete()
    {
        base.WarmupComplete();
        Find.BattleLog.Add(new BattleLogEntry_RangedFire(caster,
            !currentTarget.HasThing ? null : currentTarget.Thing,
            EquipmentSource?.def, Projectile, ShotsPerBurst > 1));
    }

    protected override bool TryCastShot()
    {
        if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
        {
            return false;
        }

        var projectile = Projectile;
        if (projectile == null)
        {
            return false;
        }

        var shootLine = new ShootLine();
        if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out shootLine))
        {
            return false;
        }

        var comp = EquipmentSource?.GetComp<CompChangeableProjectile>();
        comp?.Notify_ProjectileLaunched();

        var launcher = caster;
        Thing equipment = EquipmentSource;
        var compMannable = caster.TryGetComp<CompMannable>();
        if (compMannable?.ManningPawn != null)
        {
            launcher = compMannable.ManningPawn;
            equipment = caster;
        }

        var drawPos = caster.DrawPos;

        _ = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, caster.Map);


        //---------------
        var pellets = pelletsPerShot(projectile);
        if (pellets < 1)
        {
            pellets = 1;
        }

        var projectiles = new Projectile[pellets];
        var shootLines = new ShootLine[pellets];
        for (var i = 0; i < pellets; i++)
        {
            TryFindShootLineFromTo(caster.Position, currentTarget, out shootLines[i]);
            projectiles[i] = (Projectile)GenSpawn.Spawn(projectile, shootLines[i].Source, caster.Map);
        }

        var distance = (currentTarget.Cell - caster.Position).LengthHorizontal;
        var scatter = scatterRadiusAt10TilesAway(projectile) * distance / 10.0f;
        var missRadius = verbProps.ForcedMissRadius + forsedScatterRadius(projectile) + scatter;
        for (var i = 0; i < pellets; i++)
        {
            if (missRadius > 0.5f)
            {
                float num = (currentTarget.Cell - caster.Position).LengthHorizontalSquared;
                float num2;
                switch (num)
                {
                    case < 9f:
                        num2 = 0f;
                        break;
                    case < 25f:
                        num2 = missRadius * 0.5f;
                        break;
                    case < 49f:
                        num2 = missRadius * 0.8f;
                        break;
                    default:
                        num2 = missRadius * 1f;
                        break;
                }

                if (num2 > 0.5f)
                {
                    var max = GenRadial.NumCellsInRadius(missRadius);
                    var num3 = Rand.Range(0, max);
                    if (num3 > 0)
                    {
                        if (DebugViewSettings.drawShooting)
                        {
                            MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToForRad");
                        }

                        var c = currentTarget.Cell + GenRadial.RadialPattern[num3];
                        projectiles[i].Launch(launcher, drawPos, new LocalTargetInfo(c), currentTarget,
                            projectiles[i].HitFlags, equipment: equipment);
                        continue;
                    }

                    projectiles[i].Launch(launcher, drawPos, new LocalTargetInfo(currentTarget.Cell), currentTarget,
                        projectiles[i].HitFlags, equipment: equipment);
                    continue;
                }
            }

            var shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
            if (Rand.Value > shotReport.AimOnTargetChance_IgnoringPosture)
            {
                if (DebugViewSettings.drawShooting)
                {
                    MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToWild");
                }

                shootLines[i].ChangeDestToMissWild(shotReport.AimOnTargetChance, false, caster.Map);

                if (currentTarget.HasThing)
                {
                    projectiles[i].HitFlags = ProjectileHitFlags.All;
                }

                if (!projectiles[i].def.projectile.flyOverhead)
                {
                    projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
                }

                projectiles[i].Launch(launcher, drawPos, new LocalTargetInfo(shootLines[i].Dest), currentTarget,
                    projectiles[i].HitFlags, equipment: equipment);
                continue;
            }

            if (Rand.Value > shotReport.PassCoverChance)
            {
                if (DebugViewSettings.drawShooting)
                {
                    MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToCover");
                }

                if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn)
                {
                    var randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                    if (!projectiles[i].def.projectile.flyOverhead)
                    {
                        projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
                    }

                    projectiles[i].Launch(launcher, drawPos, new LocalTargetInfo(randomCoverToMissInto),
                        currentTarget, projectiles[i].HitFlags, equipment: equipment);
                    continue;
                }
            }

            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(caster.DrawPos, caster.Map, "ToHit");
            }

            if (!projectiles[i].def.projectile.flyOverhead &&
                (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full))
            {
                projectiles[i].HitFlags = ProjectileHitFlags.IntendedTarget;
            }

            projectiles[i].Launch(launcher, drawPos,
                currentTarget.Thing != null ? currentTarget : new LocalTargetInfo(shootLines[i].Dest),
                currentTarget, projectiles[i].HitFlags,
                equipment: equipment);
        }

        return true;
    }
}