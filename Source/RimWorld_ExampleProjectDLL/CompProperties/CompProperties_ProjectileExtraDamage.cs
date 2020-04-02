using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AAA
{
    // Token: 0x0200000F RID: 15
    public class CompProperties_ProjectileExtraDamage : CompProperties
    {
        // Token: 0x0600002D RID: 45 RVA: 0x00002A24 File Offset: 0x00000C24
        public CompProperties_ProjectileExtraDamage()
        {
            this.compClass = typeof(CompProjectileExtraDamage);
        }

        // Token: 0x04000022 RID: 34
        public string hitText = "AAA_Hit";
        
        // Token: 0x04000023 RID: 35
        public Color hitTextColor = new Color32(byte.MaxValue, 153, 102, byte.MaxValue);

        // Token: 0x04000024 RID: 36
        public int damageAmountBase = 1;

        // Token: 0x04000025 RID: 37
        public DamageDef damageDef = DamageDefOf.Bullet;
    }
}
