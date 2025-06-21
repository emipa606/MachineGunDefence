using RimWorld;

namespace Verse;

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

    protected override bool TryCastShot()
    {
        if (base.TryCastShot() && base.CasterIsPawn)
        {
            base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
        }

        return base.TryCastShot();
    }
}