using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AAA;

public class Projectile_Custom : Projectile
{
    private CompProjectileExtraDamage extraDamageComp;
    private CompProjectileSmoke smokepopComp;

    private int ticksToDetonation;


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksToDetonation, "ticksToDetonation");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        extraDamageComp = GetComp<CompProjectileExtraDamage>();
        smokepopComp = GetComp<CompProjectileSmoke>();
    }

    protected override void Tick()
    {
        base.Tick();
        if (ticksToDetonation <= 0)
        {
            return;
        }

        ticksToDetonation--;
        if (ticksToDetonation <= 0)
        {
            Explode();
        }
    }


    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        var map = Map;
        if (map == null)
        {
            Log.Error("map is null!");
        }

        if (smokepopComp != null)
        {
            ImpactPopSmoke(hitThing);
        }

        if (def.projectile.explosionRadius > 0f)
        {
            if (extraDamageComp != null)
            {
                ImpactExtra(hitThing, map);
            }

            ImpactExplode(hitThing);
            return;
        }

        ImpactDirectly(hitThing, map);
        if (extraDamageComp != null)
        {
            ImpactExtra(hitThing, map);
        }
    }

    protected virtual void ImpactPopSmoke(Thing hitThing)
    {
        var position = Position;
        var map = Map;
        var statValue =
            1 + smokepopComp.Props.smokepopRadius; //this.GetStatValue(StatDefOf.SmokepopBeltRadius, true);
        var smoke = DamageDefOf.Smoke;
        GenExplosion.DoExplosion(position, map, statValue, smoke, null, -1, -1, null, null, null, null, null, 0f, 1,
            GasType.BlindSmoke);
    }

    protected virtual void ImpactExtra(Thing hitThing, Map map)
    {
        if (hitThing == null)
        {
            return;
        }

        var damageAmountBase = extraDamageComp.Props.damageAmountBase;
        var damageDef = extraDamageComp.Props.damageDef;
        var y = ExactRotation.eulerAngles.y;
        var instigator = launcher;
        var thingDef = equipmentDef;
        hitThing.TakeDamage(new DamageInfo(damageDef, damageAmountBase, 0, y, instigator, null, thingDef));
        if (hitThing.def.category == ThingCategory.Pawn)
        {
            MoteMaker.ThrowText(new Vector3(Position.x + 1f, Position.y, Position.z + 1f), map,
                extraDamageComp.Props.hitText.Translate(), extraDamageComp.Props.hitTextColor);
        }
    }

    protected virtual void ImpactDirectly(Thing hitThing, Map map)
    {
        base.Impact(hitThing);
        var battleLogEntryRangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
            intendedTarget.Thing, equipmentDef, def, null);
        Find.BattleLog.Add(battleLogEntryRangedImpact);
        if (hitThing != null)
        {
            var damageAmountBase = def.projectile.GetDamageAmount(launcher);
            var damageDef = def.projectile.damageDef;
            var y = ExactRotation.eulerAngles.y;
            var instigator = launcher;
            var thingDef = equipmentDef;
            hitThing.TakeDamage(new DamageInfo(damageDef, damageAmountBase, 0, y, instigator, null, thingDef))
                .AssociateWithLog(battleLogEntryRangedImpact);
            return;
        }

        SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
        FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
        if (Position.GetTerrain(map).takeSplashes)
        {
            FleckMaker.WaterSplash(ExactPosition, map,
                (float)(Mathf.Sqrt(def.projectile.GetDamageAmount(launcher)) * 1.0), 4f);
        }
    }

    protected virtual void ImpactExplode(Thing hitThing)
    {
        if (def.projectile.explosionDelay == 0)
        {
            Explode();
            return;
        }

        landed = true;
        ticksToDetonation = def.projectile.explosionDelay;
        GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, def.projectile.damageDef, launcher.Faction);
    }

    protected virtual void Explode()
    {
        var map = Map;
        Destroy();
        if (def.projectile.explosionEffect != null)
        {
            var effecter = def.projectile.explosionEffect.Spawn();
            effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
            effecter.Cleanup();
        }

        var position = Position;
        var explosionRadius = def.projectile.explosionRadius;
        var damageDef = def.projectile.damageDef;
        var instigator = launcher;
        var damageAmountBase = def.projectile.GetDamageAmount(launcher);
        var soundExplode = def.projectile.soundExplode;
        var thingDef = equipmentDef;
        var projectile = def;
        var postExplosionSpawnThingDef = def.projectile.postExplosionSpawnThingDef;
        var postExplosionSpawnChance = def.projectile.postExplosionSpawnChance;
        var postExplosionSpawnThingCount = def.projectile.postExplosionSpawnThingCount;
        var preExplosionSpawnThingDef = def.projectile.preExplosionSpawnThingDef;
        GenExplosion.DoExplosion(position, map, explosionRadius, damageDef, instigator, damageAmountBase, -1,
            soundExplode, thingDef, projectile, null, postExplosionSpawnThingDef, postExplosionSpawnChance,
            postExplosionSpawnThingCount, def.projectile.postExplosionGasType, null, 0,
            def.projectile.applyDamageToExplosionCellsNeighbors,
            preExplosionSpawnThingDef, def.projectile.preExplosionSpawnChance,
            def.projectile.preExplosionSpawnThingCount, def.projectile.explosionChanceToStartFire,
            def.projectile.explosionDamageFalloff);
    }
}