using Verse;

namespace AAA
{
    // Token: 0x0200000F RID: 15
    public class CompProperties_ReproductiveProjectile : CompProperties
    {
        public int loadcount = 2;

        // Token: 0x0600002D RID: 45 RVA: 0x00002A24 File Offset: 0x00000C24
        public CompProperties_ReproductiveProjectile()
        {
            compClass = typeof(CompReproductiveProjectile);
        }
    }
}