using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AAA;

public static class AAA_CostListCalculator
{
    private static readonly Dictionary<CostListPair, List<ThingDefCountClass>> cachedCosts =
        new Dictionary<CostListPair, List<ThingDefCountClass>>(FastCostListPairComparer.Instance);

    public static void Reset()
    {
        cachedCosts.Clear();
    }

    public static List<ThingDefCountClass> AAA_CostListAdjusted(this Thing thing)
    {
        return thing.def.AAA_CostListAdjusted(thing.Stuff);
    }

    public static List<ThingDefCountClass> AAA_CostListAdjusted(this BuildableDef entDef, ThingDef stuff,
        bool errorOnNullStuff = true)
    {
        var key = new CostListPair(entDef, stuff);
        if (cachedCosts.TryGetValue(key, out var list))
        {
            return list;
        }

        list = [];
        var num = 0;
        if (entDef.MadeFromStuff)
        {
            if (errorOnNullStuff && stuff == null)
            {
                Log.Error($"Cannot get AdjustedCostList for {entDef} with null Stuff.");
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
            Log.Error($"Got AdjustedCostList for {entDef} with stuff {stuff} but is not MadeFromStuff.");
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

    private struct CostListPair(BuildableDef buildable, ThingDef stuff) : IEquatable<CostListPair>
    {
        public override int GetHashCode()
        {
            var seed = 0;
            seed = Gen.HashCombine(seed, buildable);
            return Gen.HashCombine(seed, stuff);
        }

        public override bool Equals(object obj)
        {
            return obj is CostListPair pair && Equals(pair);
        }

        public bool Equals(CostListPair other)
        {
            return this == other;
        }

        public static bool operator ==(CostListPair lhs, CostListPair rhs)
        {
            return lhs.buildable == rhs.buildable && lhs.stuff == rhs.stuff;
        }

        public static bool operator !=(CostListPair lhs, CostListPair rhs)
        {
            return !(lhs == rhs);
        }

        public readonly BuildableDef buildable = buildable;

        public readonly ThingDef stuff = stuff;
    }

    private class FastCostListPairComparer : IEqualityComparer<CostListPair>
    {
        public static readonly FastCostListPairComparer Instance = new FastCostListPairComparer();

        public bool Equals(CostListPair x, CostListPair y)
        {
            return x == y;
        }

        public int GetHashCode(CostListPair obj)
        {
            return obj.GetHashCode();
        }
    }
}