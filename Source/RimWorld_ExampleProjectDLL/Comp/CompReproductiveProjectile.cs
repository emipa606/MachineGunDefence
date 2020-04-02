using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

using RimWorld;

namespace AAA
{
    class CompReproductiveProjectile : CompChangeableProjectile
    {
        private int bulletsLeft = 0;
        private bool hasGainedLoadcount = false;

        public new CompProperties_ReproductiveProjectile Props
        {
            get
            {
                return (CompProperties_ReproductiveProjectile)this.props;
            }
        }
        
        public override void Notify_ProjectileLaunched()
        {
            if (!hasGainedLoadcount)
            {
                bulletsLeft = Props.loadcount;
                hasGainedLoadcount = true;
            }
            if (this.loadedCount == 1)//!this.hasGainedLoadcount)
            {
                if (bulletsLeft-- >= 1)
                {
                    this.loadedCount = 2;
                }
                else //emptied the magazine
                {
                    hasGainedLoadcount = false;//reset
                }
            }
            base.Notify_ProjectileLaunched();
        }
    }
}