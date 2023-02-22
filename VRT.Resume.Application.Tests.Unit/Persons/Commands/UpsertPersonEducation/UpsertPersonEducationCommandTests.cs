
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonEducation;

public class UpsertPersonEducationCommandTests : CommandTestBase<UpsertPersonEducationCommand>
{
    public UpsertPersonEducationCommandTests(ApplicationFixture fixture) : base(fixture)
    {

    }

    [Fact()]
    public async Task Send_CommandWithTooShortDegree_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Degree = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_CommandWithTooShortSchoolName_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.SchoolName = "x";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithNotSetFromDate_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.FromDate = default;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithNotSetToDate_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.ToDate = default;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithRequiredParametersAndWrongId_ShouldAddNewRecordToDbContext()
    {
        var sut = CreateSut();
        GetDbContext().PersonEducation.FirstOrDefault()
            .Should().BeNull();

        var result = await Send(sut);
        result.AssertSuccess();

        var edu = GetSinglePersonEducation();
        AssertPersonEducation(edu, sut);        
    }

    [Fact()]
    public async Task Send_CommandWithIdOfExistingEducation_ShouldUpdateExistingRecord()
    {
        var sut = CreateSut();
        var seedEdu = await GetDbContext().SeedEducation();
        sut.EducationId = seedEdu.EducationId;

        var result = await Send(sut);

        result.AssertSuccess();
        var edu = GetSinglePersonEducation();
        AssertPersonEducation(edu, sut);        
    }

    [Fact()]
    public async Task Send_CommandWithSchoolNameThatExistsInDb_ShouldSetExistingSchoolId()
    {
        var sut = CreateSut();
        var seedEdu = await GetDbContext().SeedEducation();
        var school = await GetDbContext().SeedSchool(sut.SchoolName);
        sut.EducationId = seedEdu.EducationId;

        var result = await Send(sut);

        result.AssertSuccess();

        var edu = GetSinglePersonEducation();
        AssertPersonEducation(edu, sut);
        edu.SchoolId.Should().Be(school.SchoolId);
    }

    [Fact()]
    public async Task Send_CommandWithDegreeNameThatExistsInDb_ShouldSetExistingDegreeId()
    {        
        var education = await GetDbContext().SeedEducation();
        var sut = CreateSut(education);
        var degree = await GetDbContext().SeedDegree(sut.Degree);

        var result = await Send(sut);

        result.AssertSuccess();
        var edu = GetSinglePersonEducation();
        AssertPersonEducation(edu, sut);
        Assert.Equal(degree.DegreeId, edu.DegreeId);        
    }

    [Fact()]
    public async Task Send_CommandWithEducationFieldThatExistsInDb_ShouldSetExistingEducationFieldId()
    {
        var sut = CreateSut();
        var seedEdu = await GetDbContext().SeedEducation();
        var eduField = await GetDbContext().SeedEducationField(sut.Field);
        sut.EducationId = seedEdu.EducationId;
        sut.Field = eduField.Name;

        var result = await Send(sut);
        
        result.AssertSuccess();        
        var edu = GetSinglePersonEducation();
        AssertPersonEducation(edu, sut);
        edu.EducationFieldId.Should().Be(eduField.EducationFieldId);
        edu.EducationId.Should().Be(seedEdu.EducationId);
    }

    private PersonEducation GetSinglePersonEducation()
    {
        var personEducation = GetDbContext()
            .PersonEducation
            .Include(i => i.Degree)
            .Include(i => i.EducationField)
            .ToArray();
        var edu = Assert.Single(personEducation);
        return edu;
    }

    private static void AssertPersonEducation(PersonEducation edu,
           UpsertPersonEducationCommand sut)
    {
        Assert.NotNull(edu);        
        Assert.Equal(sut.Degree, edu.Degree.Name);
        Assert.Equal(sut.Field, edu.EducationField.Name);
        Assert.Equal(sut.FromDate, edu.FromDate);
        Assert.Equal(sut.ToDate, edu.ToDate);        
    }

    private UpsertPersonEducationCommand CreateSut(PersonEducation seeded)
    {
        var result = CreateSut();
        result.EducationId = seeded.EducationId;        
        return result; 
    }

    protected override UpsertPersonEducationCommand CreateSut()
    {
        return new UpsertPersonEducationCommand()
        {            
            Degree = "Master of Science",
            Field = "Computer Science",
            SchoolName = "Bajtowards",
            Grade = "Good enough",
            ThesisTitle = "Some thesis title",
            Specialization = "Assembler",
            FromDate = Defaults.Today.AddYears(-20),
            ToDate = Defaults.Today.AddYears(-16),
        };
    }
}