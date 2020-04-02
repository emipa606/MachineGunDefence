using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace AAA
{
    class CompProperties_ProjectileLoadCount : CompProperties
    {
        // Token: 0x0600002D RID: 45 RVA: 0x00002A24 File Offset: 0x00000C24
        public CompProperties_ProjectileLoadCount()
        {
            this.compClass = typeof(CompProjectileLoadCount);
        }

        public int add_initial_loadcount = 0;
    }
}
