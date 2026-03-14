using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace FuYn.Maila.Patches;

public static class HoverTipPatcher
{
    // 1. 使用 StructFieldRefAccess 创建基于内存引用的委托
    // 这比 FieldInfo.GetValue/SetValue 快数百倍，且完全不会产生 GC 垃圾 (零装箱)
    private static readonly AccessTools.StructFieldRef<HoverTip, string?> TitleRef = 
        AccessTools.StructFieldRefAccess<HoverTip, string?>("<Title>k__BackingField");

    private static readonly AccessTools.StructFieldRef<HoverTip, string?> DescriptionRef = 
        AccessTools.StructFieldRefAccess<HoverTip, string?>("<Description>k__BackingField");

    private static void AppendText(ref HoverTip tip, string text)
    {
        // 2. 直接通过 ref 委托读取和修改，就像操作普通公开字段一样
        ref string? current = ref DescriptionRef(ref tip);
        if (current == null) current = "";
        
        current = $"{current}\n{text}"; 
    }

    private static void AppendTextToBoxed(ref IHoverTip tip, string text)
    {
        // 3. 利用 C# 自身的模式匹配进行安全的拆箱和装箱
        if (tip is HoverTip hoverTip) // 隐式拆箱到本地变量
        {
            AppendText(ref hoverTip, text); // 调用高性能的 ref 修改
            tip = hoverTip; // 将修改后的值重新装箱并赋回
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

    // 4. 重写 CreateCustomTip：代码更加清爽，性能极佳
    private static HoverTip CreateCustomTip(string? title, string? description)
    {
        HoverTip tip = new HoverTip(); 
        
        // 使用 ref 委托绕过 private set 修改 Title 和 Description
        TitleRef(ref tip) = title;
        DescriptionRef(ref tip) = description;
        
        // 源码中 Id 是有 public setter 的，直接赋值即可，不需要反射！
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
            var title = MailaConfig.ShowCardId ? __instance.Id.Entry : null;
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
            var title = MailaConfig.ShowCreatureId ? __instance.ModelId.Entry : null;
            var description = FormatNameTip(model?.GetType().FullName);
            var custom = CreateCustomTip(title, description);
            tips.Insert(0, custom);
            __result = tips;
        }
        // ReSharper restore InconsistentNaming
    }
}