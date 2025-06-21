using RimWorld;

namespace AAA;

internal class CompReproductiveProjectile : CompChangeableProjectile
{
    private int bulletsLeft;
    private bool hasGainedLoadcount;

    private new CompProperties_ReproductiveProjectile Props => (CompProperties_ReproductiveProjectile)props;

    public override void Notify_ProjectileLaunched()
    {
        if (!hasGainedLoadcount)
        {
            bulletsLeft = Props.loadcount;
            hasGainedLoadcount = true;
        }

        if (loadedCount == 1) //!this.hasGainedLoadcount
        {
            if (bulletsLeft-- >= 1)
            {
                loadedCount = 2;
            }
            else //emptied the magazine
            {
                hasGainedLoadcount = false; //reset
            }
        }

        base.Notify_ProjectileLaunched();
    }
}