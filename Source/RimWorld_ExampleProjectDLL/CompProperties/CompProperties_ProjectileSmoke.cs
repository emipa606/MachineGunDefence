using Verse;

namespace AAA;

public class CompProperties_ProjectileSmoke : CompProperties
{
    public readonly float smokepopRadius = 1.0f;

    public CompProperties_ProjectileSmoke()
    {
        compClass = typeof(CompProjectileSmoke);
    }
}