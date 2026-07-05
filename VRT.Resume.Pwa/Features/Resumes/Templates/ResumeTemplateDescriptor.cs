namespace VRT.Resume.Pwa.Features.Resumes.Templates;

public sealed record ResumeTemplateDescriptor(
    string Id,
    Type ComponentType,
    string LabelName,
    string CssPath);