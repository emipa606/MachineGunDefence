using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
namespace AAA
{
    class CompProjectileLoadCount : ThingComp
    {
        public CompProperties_ProjectileLoadCount Props
        {
            get
            {
                return (CompProperties_ProjectileLoadCount)this.props;
            }
        }
    }
}
