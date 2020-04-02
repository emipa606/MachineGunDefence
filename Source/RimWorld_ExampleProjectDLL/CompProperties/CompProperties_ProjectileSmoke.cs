using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AAA
{
    // Token: 0x0200000F RID: 15
    public class CompProperties_ProjectileSmoke : CompProperties
    {
        // Token: 0x0600002D RID: 45 RVA: 0x00002A24 File Offset: 0x00000C24
        public CompProperties_ProjectileSmoke()
        {
            this.compClass = typeof(CompProjectileSmoke);
        }


        public float smokepopRadius = 1.0f;
    }
}
