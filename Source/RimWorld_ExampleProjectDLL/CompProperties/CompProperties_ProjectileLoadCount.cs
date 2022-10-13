using Verse;

namespace AAA;

internal class CompProperties_ProjectileLoadCount : CompProperties
{
    public int add_initial_loadcount = 0;

    public CompProperties_ProjectileLoadCount()
    {
        compClass = typeof(CompProjectileLoadCount);
    }
}