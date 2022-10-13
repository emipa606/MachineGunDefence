using RimWorld;
using UnityEngine;
using Verse;

namespace AAA;

public class CompProperties_ProjectileExtraDamage : CompProperties
{
    public int damageAmountBase = 1;

    public DamageDef damageDef;

    public string hitText = "AAA_Hit";

    public Color hitTextColor = new Color32(byte.MaxValue, 153, 102, byte.MaxValue);

    public CompProperties_ProjectileExtraDamage()
    {
        compClass = typeof(CompProjectileExtraDamage);
    }

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);
        if (damageDef == null)
        {
            damageDef = DamageDefOf.Bullet;
        }
    }
}