using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AAA
{
    public static class AAA_CostListCalculator
    {
        // Token: 0x04001AB8 RID: 6840
        private static readonly Dictionary<CostListPair, List<ThingDefCountClass>> cachedCosts =
            new Dictionary<CostListPair, List<ThingDefCountClass>>(FastCostListPairComparer.Instance);

        // Token: 0x06003064 RID: 12388 RVA: 0x0016C45D File Offset: 0x0016A85D
        public static void Reset()
        {
            cachedCosts.Clear();
        }

        // Token: 0x06003065 RID: 12389 RVA: 0x0016C469 File Offset: 0x0016A869
        public static List<ThingDefCountClass> AAA_CostListAdjusted(this Thing thing)
        {
            return thing.def.AAA_CostListAdjusted(thing.Stuff);
        }

        // Token: 0x06003066 RID: 12390 RVA: 0x0016C480 File Offset: 0x0016A880
        public static List<ThingDefCountClass> AAA_CostListAdjusted(this BuildableDef entDef, ThingDef stuff,
            bool errorOnNullStuff = true)
        {
            var key = new CostListPair(entDef, stuff);
            if (cachedCosts.TryGetValue(key, out var list))
            {
                return list;
            }

            list = new List<ThingDefCountClass>();
            var num = 0;
            if (entDef.MadeFromStuff)
            {
                if (errorOnNullStuff && stuff == null)
                {
                    Log.Error("Cannot get AdjustedCostList for " + entDef + " with null Stuff.");
                    return null;
                }

                if (stuff != null)
                {
                    num = Mathf.RoundToInt(entDef.costStuffCount / stuff.VolumePerUnit);
                    if (num < 1)
                    {
                        num = 1;
                    }
                }
                else
                {
                    num = entDef.costStuffCount;
                }
            }
            else if (stuff != null)
            {
                Log.Error(string.Concat("Got AdjustedCostList for ", entDef, " with stuff ", stuff,
                    " but is not MadeFromStuff."));
            }

            var b = false;
            if (entDef.costList != null)
            {
                foreach (var thingDefCountClass in entDef.costList)
                {
                    if (thingDefCountClass.thingDef == stuff)
                    {
                        //-----------------stuff---------------------------------
                        list.Add(
                            new ThingDefCountClass(thingDefCountClass.thingDef, thingDefCountClass.count + num));
                        b = true;
                    }
                    else
                    {
                        float count2 = thingDefCountClass.count;
                        //--------------------------------------------------------
                        //             leave  one crate
                        //----------------------------------------------------------
                        thingDefCountClass.count = GenMath.RoundRandom(count2);
                        list.Add(thingDefCountClass);
                    }
                }
            }

            if (!b && num > 0)
            {
                list.Add(new ThingDefCountClass(stuff, num));
            }

            cachedCosts.Add(key, list);

            return list;
        }

        // Token: 0x02000883 RID: 2179
        private struct CostListPair : IEquatable<CostListPair>
        {
            // Token: 0x06003068 RID: 12392 RVA: 0x0016C5F3 File Offset: 0x0016A9F3
            public CostListPair(BuildableDef buildable, ThingDef stuff)
            {
                this.buildable = buildable;
                this.stuff = stuff;
            }

            // Token: 0x06003069 RID: 12393 RVA: 0x0016C604 File Offset: 0x0016AA04
            public override int GetHashCode()
            {
                var seed = 0;
                seed = Gen.HashCombine(seed, buildable);
                return Gen.HashCombine(seed, stuff);
            }

            // Token: 0x0600306A RID: 12394 RVA: 0x0016C62E File Offset: 0x0016AA2E
            public override bool Equals(object obj)
            {
                return obj is CostListPair pair && Equals(pair);
            }

            // Token: 0x0600306B RID: 12395 RVA: 0x0016C649 File Offset: 0x0016AA49
            public bool Equals(CostListPair other)
            {
                return this == other;
            }

            // Token: 0x0600306C RID: 12396 RVA: 0x0016C657 File Offset: 0x0016AA57
            public static bool operator ==(CostListPair lhs, CostListPair rhs)
            {
                return lhs.buildable == rhs.buildable && lhs.stuff == rhs.stuff;
            }

            // Token: 0x0600306D RID: 12397 RVA: 0x0016C67F File Offset: 0x0016AA7F
            public static bool operator !=(CostListPair lhs, CostListPair rhs)
            {
                return !(lhs == rhs);
            }

            // Token: 0x04001AB9 RID: 6841
            public readonly BuildableDef buildable;

            // Token: 0x04001ABA RID: 6842
            public readonly ThingDef stuff;
        }

        // Token: 0x02000884 RID: 2180
        private class FastCostListPairComparer : IEqualityComparer<CostListPair>
        {
            // Token: 0x04001ABB RID: 6843
            public static readonly FastCostListPairComparer Instance = new FastCostListPairComparer();

            // Token: 0x0600306F RID: 12399 RVA: 0x0016C693 File Offset: 0x0016AA93
            public bool Equals(CostListPair x, CostListPair y)
            {
                return x == y;
            }

            // Token: 0x06003070 RID: 12400 RVA: 0x0016C69C File Offset: 0x0016AA9C
            public int GetHashCode(CostListPair obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}