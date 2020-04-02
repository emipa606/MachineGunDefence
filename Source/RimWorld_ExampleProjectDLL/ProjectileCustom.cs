using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AAA
{
    // Token: 0x02000011 RID: 17
    public class Projectile_Custom : Projectile
    {

      
        // Token: 0x06000030 RID: 48 RVA: 0x00002A92 File Offset: 0x00000C92
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002AAC File Offset: 0x00000CAC
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.extraDamageComp = base.GetComp<CompProjectileExtraDamage>();
            this.smokepopComp    = base.GetComp<CompProjectileSmoke>();
        }

        public override void Tick()
        {
            base.Tick();
           // MoteMaker.ThrowSmoke(this.Position.ToVector3Shifted(), this.Map, 3);
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

      
        // Token: 0x06000033 RID: 51 RVA: 0x00002AF0 File Offset: 0x00000CF0
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            if (map == null)
            {
                Log.Error("map is null!");
            }
            if (this.smokepopComp != null)
            {
                this.ImpactPopSmoke(hitThing);
            }
            if (this.def.projectile.explosionRadius > 0f)
            {
                if (this.extraDamageComp != null)
                {
                    this.ImpactExtra(hitThing, map);
                }
                this.ImpactExplode(hitThing);
                return;
            }
            this.ImpactDirectly(hitThing, map);
            if (this.extraDamageComp != null)
            {
                this.ImpactExtra(hitThing, map);
            }
        
        }
        protected virtual void ImpactPopSmoke(Thing hitThing)
        {
                
                IntVec3 position = base.Position;
                Map map = base.Map;
                float statValue = 1 + this.smokepopComp.Props.smokepopRadius;//this.GetStatValue(StatDefOf.SmokepopBeltRadius, true);
                DamageDef smoke = DamageDefOf.Smoke;
                Thing instigator = null;
                ThingDef gas_Smoke = ThingDefOf.Gas_Smoke;
                GenExplosion.DoExplosion(position, map, statValue, smoke, instigator, -1, -1, null, null, null, null, gas_Smoke, 1f, 1, false, null, 0f, 1, 0f, false);
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00002B58 File Offset: 0x00000D58
        protected virtual void ImpactExtra(Thing hitThing, Map map)
        {
            if (hitThing != null)
            {
                int damageAmountBase = this.extraDamageComp.Props.damageAmountBase;
                DamageDef damageDef = this.extraDamageComp.Props.damageDef;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, 0, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                hitThing.TakeDamage(dinfo);
                if (hitThing.def.category == ThingCategory.Pawn)
                {
                    MoteMaker.ThrowText(new Vector3((float)base.Position.x + 1f, (float)base.Position.y, (float)base.Position.z + 1f), map, this.extraDamageComp.Props.hitText.Translate(), this.extraDamageComp.Props.hitTextColor, -1f);
                }
            }
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00002C3C File Offset: 0x00000E3C
        protected virtual void ImpactDirectly(Thing hitThing, Map map)
        {
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, null);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                int damageAmountBase = this.def.projectile.GetDamageAmount(this.launcher);
                DamageDef damageDef = this.def.projectile.damageDef;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, damageAmountBase, 0, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                return;
            }
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
            MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
            if (base.Position.GetTerrain(map).takeSplashes)
            {
                MoteMaker.MakeWaterSplash(this.ExactPosition, map, (float)((double)Mathf.Sqrt((float)this.def.projectile.GetDamageAmount(this.launcher)) * 1.0), 4f);
            }
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002D60 File Offset: 0x00000F60
        protected virtual void ImpactExplode(Thing hitThing)
        {
            if (this.def.projectile.explosionDelay == 0)
            {
                this.Explode();
                return;
            }
            this.landed = true;
            this.ticksToDetonation = this.def.projectile.explosionDelay;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00002DC4 File Offset: 0x00000FC4
        protected virtual void Explode()
        {
            Map map = base.Map;
            this.Destroy(DestroyMode.Vanish);
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = base.Position;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmountBase = this.def.projectile.GetDamageAmount(this.launcher);
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmountBase, -1, soundExplode, equipmentDef, def, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        // Token: 0x04000026 RID: 38
        protected CompProjectileExtraDamage extraDamageComp;
        protected CompProjectileSmoke smokepopComp;

        // Token: 0x04000027 RID: 39
        private int ticksToDetonation;
    }
}
