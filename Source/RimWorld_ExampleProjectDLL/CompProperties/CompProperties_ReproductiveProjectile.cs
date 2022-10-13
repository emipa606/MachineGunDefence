using Verse;

namespace AAA;

public class CompProperties_ReproductiveProjectile : CompProperties
{
    public int loadcount = 2;

    public CompProperties_ReproductiveProjectile()
    {
        compClass = typeof(CompReproductiveProjectile);
    }
}