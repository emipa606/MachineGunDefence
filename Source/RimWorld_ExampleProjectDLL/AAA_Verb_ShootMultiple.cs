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

namespace Verse//AAA
{
    class AAA_Verb_ShootMultiple : AAA_Verb_LaunchMultipleProjectile
    {
		// Token: 0x17000E4E RID: 3662
		// (get) Token: 0x060057CF RID: 22479 RVA: 0x0017BDC6 File Offset: 0x0017A1C6
		protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }

        // Token: 0x060057D0 RID: 22480 RVA: 0x0017BDD4 File Offset: 0x0017A1D4
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (base.CasterIsPawn && base.CasterPawn.skills != null)
            {
                float xp = 6f;
                Pawn pawn = this.currentTarget.Thing as Pawn;
                if (pawn != null && pawn.HostileTo(this.caster) && !pawn.Downed)
                {
                    xp = 240f;
                }
                base.CasterPawn.skills.Learn(SkillDefOf.Shooting, xp, false);
            }
        }
        protected int getCompLoadcount()
        {
            ThingDef projectile = this.EquipmentSource.GetComp<CompChangeableProjectile>().Projectile;
            int count = projectile.comps.Count;
            int i = 0;
            while (i < count)
            {
                AAA.CompProperties_ProjectileLoadCount compWhenLoaded = projectile.comps[i] as AAA.CompProperties_ProjectileLoadCount;
                if (compWhenLoaded != null)
                {
                    return compWhenLoaded.add_initial_loadcount;
                }
                i++;
            }return 0;
        }
        // Token: 0x060057D1 RID: 22481 RVA: 0x0017BE58 File Offset: 0x0017A258
        protected override bool TryCastShot()
        {
           //if (this.ownerEquipment.GetComp<CompChangeableProjectile>(). loadedCount == 1)
           // {
           //     this.ownerEquipment.GetComp<CompChangeableProjectile>().loadedCount = getCompLoadcount();
           // }
            bool flag = base.TryCastShot();
            if (flag && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}

