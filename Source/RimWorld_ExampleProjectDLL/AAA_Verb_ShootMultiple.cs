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
//AAA

internal class AAA_Verb_ShootMultiple : AAA_Verb_LaunchMultipleProjectile
{
    protected override int ShotsPerBurst => verbProps.burstShotCount;

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