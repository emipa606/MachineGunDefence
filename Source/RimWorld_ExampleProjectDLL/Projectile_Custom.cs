using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AAA
{
    // Token: 0x02000011 RID: 17
    public class Projectile_Custom : Projectile
    {
        // Token: 0x04000026 RID: 38
        protected CompProjectileExtraDamage extraDamageComp;
        protected CompProjectileSmoke smokepopComp;

        // Token: 0x04000027 RID: 39
        private int ticksToDetonation;


        // Token: 0x06000030 RID: 48 RVA: 0x00002A92 File Offset: 0x00000C92
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToDetonation, "ticksToDetonation");
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002AAC File Offset: 0x00000CAC
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            extraDamageComp = GetComp<CompProjectileExtraDamage>();
            smokepopComp = GetComp<CompProjectileSmoke>();
        }

        public override void Tick()
        {
            base.Tick();
            // MoteMaker.ThrowSmoke(this.Position.ToVector3Shifted(), this.Map, 3);
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


        // Token: 0x06000033 RID: 51 RVA: 0x00002AF0 File Offset: 0x00000CF0
        protected override void Impact(Thing hitThing)
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
            var gas_Smoke = ThingDefOf.Gas_Smoke;
            GenExplosion.DoExplosion(position, map, statValue, smoke, null, -1, -1, null, null, null, null,
                gas_Smoke, 1f);
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00002B58 File Offset: 0x00000D58
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
            var dinfo = new DamageInfo(damageDef, damageAmountBase, 0, y, instigator, null, thingDef);
            hitThing.TakeDamage(dinfo);
            if (hitThing.def.category == ThingCategory.Pawn)
            {
                MoteMaker.ThrowText(new Vector3(Position.x + 1f, Position.y, Position.z + 1f), map,
                    extraDamageComp.Props.hitText.Translate(), extraDamageComp.Props.hitTextColor);
            }
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00002C3C File Offset: 0x00000E3C
        protected virtual void ImpactDirectly(Thing hitThing, Map map)
        {
            base.Impact(hitThing);
            var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
                intendedTarget.Thing, equipmentDef, def, null);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                var damageAmountBase = def.projectile.GetDamageAmount(launcher);
                var damageDef = def.projectile.damageDef;
                var y = ExactRotation.eulerAngles.y;
                var instigator = launcher;
                var thingDef = equipmentDef;
                var dinfo = new DamageInfo(damageDef, damageAmountBase, 0, y, instigator, null, thingDef);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                return;
            }

            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
            FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
            if (Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(ExactPosition, map,
                    (float) (Mathf.Sqrt(def.projectile.GetDamageAmount(launcher)) * 1.0), 4f);
            }
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002D60 File Offset: 0x00000F60
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

        // Token: 0x06000037 RID: 55 RVA: 0x00002DC4 File Offset: 0x00000FC4
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
            var map2 = map;
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
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, instigator, damageAmountBase, -1,
                soundExplode, thingDef, projectile, null, postExplosionSpawnThingDef, postExplosionSpawnChance,
                postExplosionSpawnThingCount, def.projectile.applyDamageToExplosionCellsNeighbors,
                preExplosionSpawnThingDef, def.projectile.preExplosionSpawnChance,
                def.projectile.preExplosionSpawnThingCount, def.projectile.explosionChanceToStartFire,
                def.projectile.explosionDamageFalloff);
        }
    }
}