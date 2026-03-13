using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;


namespace FuYn.Maila.Patches;

public static class HoverTipPatcher
{
    private static readonly FieldInfo DescriptionField = AccessTools.Field(typeof(HoverTip), "<Description>k__BackingField");

    private static void AppendText(ref HoverTip tip, string text)
    {
        object boxed = tip;
        string current = (string)DescriptionField.GetValue(boxed);
        if (current == null) current = "";
        DescriptionField.SetValue(boxed, current + text);
        tip = (HoverTip)boxed;
    }

    private static void AppendTextToBoxed(ref IHoverTip tip, string text)
    {
        // Must check if it is exactly HoverTip or compatible struct
        if (tip is HoverTip)
        {
            object boxed = tip;
            string current = (string)DescriptionField.GetValue(boxed);
            if (current == null) current = "";
            DescriptionField.SetValue(boxed, current + text);
            tip = (IHoverTip)boxed;
        }
    }

    private static void AppendTextToFirstInList(ref IEnumerable<IHoverTip> list, string text)
    {
        var tipList = list.ToList();
        if (tipList.Count > 0)
        {
            var first = tipList[0];
            AppendTextToBoxed(ref first, text);
            tipList[0] = first;
            list = tipList;
        }
    }

    [HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))]
    public static class FromKeywordPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(CardKeyword keyword, ref IHoverTip __result)
        {
            string typeName = "MegaCrit.Sts2.Core.Entities.Cards.CardKeyword." + keyword;
            AppendTextToBoxed(ref __result, $"\n[707070]{typeName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.DumbHoverTip), MethodType.Getter)]
    public static class PowerModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(PowerModel __instance, ref HoverTip __result)
        {
            AppendText(ref __result, $"\n[707070]{__instance.GetType().FullName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)]
    public static class RelicModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(RelicModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            AppendTextToFirstInList(ref __result, $"\n[707070]{__instance.GetType().FullName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(PotionModel), nameof(PotionModel.HoverTip), MethodType.Getter)]
    public static class PotionModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(PotionModel __instance, ref HoverTip __result)
        {
            AppendText(ref __result, $"\n[707070]{__instance.GetType().FullName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.HoverTips), MethodType.Getter)]
    public static class AfflictionModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(AfflictionModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            AppendTextToFirstInList(ref __result, $"\n[707070]{__instance.GetType().FullName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(EnchantmentModel), nameof(EnchantmentModel.HoverTips), MethodType.Getter)]
    public static class EnchantmentModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(EnchantmentModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            AppendTextToFirstInList(ref __result, $"\n[707070]{__instance.GetType().FullName}[-]");
        }
        // ReSharper restore InconsistentNaming
    }
}

