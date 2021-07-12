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

namespace Verse //AAA
{
    internal class AAA_Verb_ShootMultiple : AAA_Verb_LaunchMultipleProjectile
    {
        // Token: 0x17000E4E RID: 3662
        // (get) Token: 0x060057CF RID: 22479 RVA: 0x0017BDC6 File Offset: 0x0017A1C6
        protected override int ShotsPerBurst => verbProps.burstShotCount;

        // Token: 0x060057D0 RID: 22480 RVA: 0x0017BDD4 File Offset: 0x0017A1D4
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (!base.CasterIsPawn || base.CasterPawn.skills == null)
            {
                return;
            }

            var xp = 6f;
            if (currentTarget.Thing is Pawn pawn && pawn.HostileTo(caster) && !pawn.Downed)
            {
                xp = 240f;
            }

            base.CasterPawn.skills.Learn(SkillDefOf.Shooting, xp);
        }

        protected int getCompLoadcount()
        {
            var projectile = EquipmentSource.GetComp<CompChangeableProjectile>().Projectile;
            var count = projectile.comps.Count;
            var i = 0;
            while (i < count)
            {
                if (projectile.comps[i] is CompProperties_ProjectileLoadCount compWhenLoaded)
                {
                    return compWhenLoaded.add_initial_loadcount;
                }

                i++;
            }

            return 0;
        }

        // Token: 0x060057D1 RID: 22481 RVA: 0x0017BE58 File Offset: 0x0017A258
        protected override bool TryCastShot()
        {
            //if (this.ownerEquipment.GetComp<CompChangeableProjectile>(). loadedCount == 1)
            // {
            //     this.ownerEquipment.GetComp<CompChangeableProjectile>().loadedCount = getCompLoadcount();
            // }
            if (base.TryCastShot() && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }

            return base.TryCastShot();
        }
    }
}