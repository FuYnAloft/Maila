using BaseLib.Config;

namespace FuYn.Maila;

public enum MailaFontSize
{
    Small = 16,
    Medium = 20,
    Normal = 24,
}

public class MailaConfig : SimpleModConfig
{
    public static MailaFontSize FontSize { get; set; } = MailaFontSize.Small;
    public static bool ShowCardType { get; set; } = true;
    public static bool ShowCreatureType { get; set; } = true;

    public static bool ShowKeywordType { get; set; } = true;
    public static bool ShowPowerType { get; set; } = true;
    public static bool ShowRelicType { get; set; } = true;
    public static bool ShowPotionType { get; set; } = true;
    public static bool ShowAfflictionType { get; set; } = true;
    public static bool ShowEnchantmentType { get; set; } = true;
    public static bool ShowIntentType { get; set; } = true;
    public static bool ShowStaticHoverTipType { get; set; } = true;
}