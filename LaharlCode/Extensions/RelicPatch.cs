using HarmonyLib;
using Laharl.LaharlCode.Cards.Ancient;
using Laharl.LaharlCode.Cards.Basic;
using Laharl.LaharlCode.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Laharl.LaharlCode.Extensions;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
internal static class LaharlTouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is Yoshitsuna)
        {
            __result = ModelDb.Relic<BaalSword>().ToMutable();
        }
    }
}


[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
internal static class LaharlArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        // Add/override mapping: BlazingKnuckle -> HellfireKnuckle
        __result[ModelDb.Card<BlazingKnuckle>().Id] = ModelDb.Card<HellfireKnuckle>();
    }
}


