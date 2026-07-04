namespace VRT.Resume.Application.Common.Abstractions;

/// <summary>
/// Optional host-provided PersonId for the active user context (PWA profile selection).
/// When registered, <see cref="HandlerBase"/> uses it before querying <c>UserPerson</c>.
/// </summary>
public interface ICurrentPersonIdAccessor
{
    bool TryGetPersonId(out int personId);
}