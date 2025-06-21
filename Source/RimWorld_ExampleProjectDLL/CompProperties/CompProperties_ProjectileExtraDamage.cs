using RimWorld;
using UnityEngine;
using Verse;

namespace AAA;

public class CompProperties_ProjectileExtraDamage : CompProperties
{
    public readonly int damageAmountBase = 1;

    public readonly string hitText = "AAA_Hit";

    public DamageDef damageDef;

    public Color hitTextColor = new Color32(byte.MaxValue, 153, 102, byte.MaxValue);

    public CompProperties_ProjectileExtraDamage()
    {
        compClass = typeof(CompProjectileExtraDamage);
    }

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);
        damageDef ??= DamageDefOf.Bullet;
    }
}