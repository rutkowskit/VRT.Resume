using VRT.Resume.Domain.Common;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Person;

internal static class SkillTypeDisplay
{
    public static string GetLabel(SkillTypes type) =>
        type switch
        {
            SkillTypes.HumanLanguage => LabelNames.SkillTypeHumanLanguage.GetLabelText(),
            SkillTypes.Technical => LabelNames.SkillTypeTechnical.GetLabelText(),
            SkillTypes.Soft => LabelNames.SkillTypeSoft.GetLabelText(),
            SkillTypes.Tool => LabelNames.SkillTypeTool.GetLabelText(),
            _ => LabelNames.SkillTypeOther.GetLabelText(),
        };

    public static string GetLabel(string skillTypeName) =>
        Enum.TryParse<SkillTypes>(skillTypeName, out var type)
            ? GetLabel(type)
            : skillTypeName;
}