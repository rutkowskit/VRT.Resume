using VRT.Resume.Pwa.Features.Resumes.Components;

namespace VRT.Resume.Pwa.Features.Resumes.Templates;

public static class ResumeTemplateRegistry
{
    private static readonly ResumeTemplateDescriptor[] Templates =
    [
        new(
            ResumeTemplateIds.Classic,
            typeof(ResumeDocumentClassic),
            LabelNames.ResumeTemplateClassic,
            "css/resume/classic.css"),
    ];

    private static readonly IReadOnlyDictionary<string, ResumeTemplateDescriptor> ById =
        Templates.ToDictionary(t => t.Id, StringComparer.Ordinal);

    public static IReadOnlyList<ResumeTemplateDescriptor> All => Templates;

    public static IReadOnlySet<string> KnownIds => ById.Keys.ToHashSet(StringComparer.Ordinal);

    public static string Normalize(string? templateId) =>
        string.IsNullOrWhiteSpace(templateId) || !ById.ContainsKey(templateId)
            ? ResumeTemplateIds.Classic
            : templateId;

    public static ResumeTemplateDescriptor GetOrDefault(string? templateId) =>
        ById.TryGetValue(Normalize(templateId), out var descriptor)
            ? descriptor
            : ById[ResumeTemplateIds.Classic];
}