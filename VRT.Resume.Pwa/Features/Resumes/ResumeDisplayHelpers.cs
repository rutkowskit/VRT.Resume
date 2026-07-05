using System.Text;
using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Domain.Common;

namespace VRT.Resume.Pwa.Features.Resumes;

internal static class ResumeDisplayHelpers
{
    public static string FormatTimeRange(ITimeRange? range, string format = "MM-yyyy", string separator = " - ")
    {
        if (range is null)
            return string.Empty;

        return range.ToDate is null
            ? range.FromDate.ToString(format)
            : $"{range.FromDate.ToString(format)}{separator}{range.ToDate.Value.ToString(format)}";
    }

    public static string SkillCssClass(SkillDto? skill, string className, string relevantClass = "font-weight-bolder") =>
        skill is { IsRelevant: true } ? $"{className} {relevantClass}".Trim() : className;

    public static IEnumerable<SkillDto> GetLanguageSkills(IEnumerable<SkillDto>? data) =>
        GetSkillsByType(data, SkillTypes.HumanLanguage);

    public static IEnumerable<SkillDto> GetTechnicalSkills(IEnumerable<SkillDto>? data, bool onlyVisible = true) =>
        GetSkillsByType(data, SkillTypes.Technical)
            .Where(s => !onlyVisible || !s.IsHidden)
            .OrderByDescending(s => s.Position);

    public static IEnumerable<SkillDto> GetSoftSkills(IEnumerable<SkillDto>? data) =>
        GetSkillsByType(data, SkillTypes.Soft);

    public static MarkupString? ContactIcon(ContactItemDto? contact) =>
        string.IsNullOrWhiteSpace(contact?.Icon) ? null : new MarkupString(contact.Icon);

    public static MarkupString ContactLine(ContactItemDto contact)
    {
        var icon = string.IsNullOrWhiteSpace(contact.Icon) ? string.Empty : contact.Icon;
        return new MarkupString($"{icon}{ContactValueHtml(contact)}");
    }

    public static MarkupString ContactValue(ContactItemDto contact) =>
        new MarkupString(ContactValueHtml(contact));

    private static string ContactValueHtml(ContactItemDto contact)
    {
        if (!string.IsNullOrWhiteSpace(contact.Url))
            return $"<a class=\"resume-contact-link\" href=\"{contact.Url}\" target=\"_blank\" rel=\"noopener noreferrer\">{contact.Value}</a>";

        return $"<span>{contact.Value}</span>";
    }

    public static MarkupString? FormatActivitySkills(WorkActivityDto? activity)
    {
        var skills = activity?.Skills?
            .OrderByDescending(s => s.Position)
            .ThenByDescending(s => s.Level)
            .ToArray();

        if (skills is null || skills.Length == 0)
            return null;

        var builder = new StringBuilder("(");
        for (var i = 0; i < skills.Length; i++)
        {
            if (i > 0)
                builder.Append(", ");

            var css = SkillCssClass(skills[i], string.Empty);
            builder.Append($"<span class=\"{css}\">{skills[i].Name}</span>");
        }

        builder.Append(')');
        return new MarkupString(builder.ToString());
    }

    public static string ShowUrl(int resumeId) => $"resumes/show/{resumeId}";

    private static IEnumerable<SkillDto> GetSkillsByType(IEnumerable<SkillDto>? data, SkillTypes type)
    {
        if (data is null)
            yield break;

        foreach (var item in data)
        {
            if (item.Type == type)
                yield return item;
        }
    }
}