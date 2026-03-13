using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;


namespace FuYn.Maila.Patches;

public static class HoverTipPatcher
{
    private static readonly FieldInfo TitleField = AccessTools.Field(typeof(HoverTip), "<Title>k__BackingField");

    private static readonly FieldInfo DescriptionField = AccessTools.Field(typeof(HoverTip), "<Description>k__BackingField");

    private static readonly FieldInfo IdField = AccessTools.Field(typeof(HoverTip), "<Id>k__BackingField");

    private static void AppendText(ref HoverTip tip, string text)
    {
        object boxed = tip;
        string current = (string)DescriptionField.GetValue(boxed);
        if (current == null) current = "";
        DescriptionField.SetValue(boxed, $"{current}\n{text}");
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
            DescriptionField.SetValue(boxed, $"{current}\n{text}");
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

    private static string FormatNameTip(string? name)
    {
        var whitespaced = name?.Replace(".", "\u200b.");
        return $"[font_size={(int)MailaConfig.FontSize}][color=#7f7f7f]{whitespaced}[/color][/font_size]";
    }

    private static HoverTip CreateCustomTip(string? title, string? description)
    {
        HoverTip tip = default;
        object boxed = tip;
        TitleField.SetValue(boxed, title);
        DescriptionField.SetValue(boxed, description);
        tip = (HoverTip)boxed;
        tip.Id = "Maila.CustomTip." + title;
        return tip;
    }

    [HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))]
    public static class FromKeywordPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(CardKeyword keyword, ref IHoverTip __result)
        {
            if (!MailaConfig.ShowKeywordType) return;
            string typeName = "MegaCrit.Sts2.Core.Entities.Cards.CardKeyword." + keyword;
            AppendTextToBoxed(ref __result, FormatNameTip(typeName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.DumbHoverTip), MethodType.Getter)]
    public static class PowerModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(PowerModel __instance, ref HoverTip __result)
        {
            if (!MailaConfig.ShowPowerType) return;
            AppendText(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.HoverTips), MethodType.Getter)]
    public static class RelicModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(RelicModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowRelicType) return;
            AppendTextToFirstInList(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(PotionModel), nameof(PotionModel.HoverTip), MethodType.Getter)]
    public static class PotionModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(PotionModel __instance, ref HoverTip __result)
        {
            if (!MailaConfig.ShowPotionType) return;
            AppendText(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(AfflictionModel), nameof(AfflictionModel.HoverTips), MethodType.Getter)]
    public static class AfflictionModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(AfflictionModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowAfflictionType) return;
            AppendTextToFirstInList(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(EnchantmentModel), nameof(EnchantmentModel.HoverTips), MethodType.Getter)]
    public static class EnchantmentModelPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(EnchantmentModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowEnchantmentType) return;
            AppendTextToFirstInList(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.HoverTips), MethodType.Getter)]
    public static class PowerModelHoverTipsPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(PowerModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowPowerType) return;
            AppendTextToFirstInList(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(AbstractIntent), nameof(AbstractIntent.GetHoverTip))]
    public static class AbstractIntentPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(AbstractIntent __instance, ref HoverTip __result)
        {
            if (!MailaConfig.ShowIntentType) return;
            AppendText(ref __result, FormatNameTip(__instance.GetType().FullName));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.Static))]
    public static class StaticHoverTipPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(StaticHoverTip tip, ref IHoverTip __result)
        {
            if (!MailaConfig.ShowStaticHoverTipType) return;
            AppendTextToBoxed(ref __result, FormatNameTip("MegaCrit.Sts2.Core.HoverTips.StaticHoverTip." + tip));
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
    public static class CardModelHoverTipsPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowCardType) return;
            var tips = __result.ToList();
            var title = __instance.Id.Entry;
            var description = FormatNameTip(__instance.GetType().FullName);
            var custom = CreateCustomTip(title, description);
            tips.Insert(0, custom);
            __result = tips;
        }
        // ReSharper restore InconsistentNaming
    }

    [HarmonyPatch(typeof(Creature), nameof(Creature.HoverTips), MethodType.Getter)]
    public static class CreatureHoverTipsPatch
    {
        // ReSharper disable InconsistentNaming
        public static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!MailaConfig.ShowCreatureType) return;
            var tips = __result.ToList();
            object? model = !__instance.IsPlayer ? __instance.Monster : __instance.Player?.Character;
            var title = __instance.ModelId.Entry;
            var description = FormatNameTip(model?.GetType().FullName);
            var custom = CreateCustomTip(title, description);
            tips.Insert(0, custom);
            __result = tips;
        }
        // ReSharper restore InconsistentNaming
    }
}