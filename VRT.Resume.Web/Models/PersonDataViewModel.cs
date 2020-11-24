using System;
using System.ComponentModel.DataAnnotations;
using VRT.Resume.Application.Persons.Queries.GetPersonData;

namespace VRT.Resume.Web.Models
{
    /// <summary>
    /// Original VM decorator
    /// </summary>
    public sealed class PersonDataViewModel
    {
        private readonly PersonDataVM _decorated;

        public PersonDataViewModel(): this(null)
        {
        }
        public PersonDataViewModel(PersonDataVM vm)
        {
            _decorated = vm ?? new PersonDataVM();
        }
        public int PersonId
        {
            get => _decorated.PersonId; set => _decorated.PersonId = value;
        }

        [Required]
        [Display(Name = "FirstName", ResourceType = typeof(Resources.LabelResource))]
        public string FirstName
        {
            get => _decorated.FirstName; set => _decorated.FirstName = value;
        }
        
        [Required]
        [Display(Name = "LastName", ResourceType = typeof(Resources.LabelResource))]
        public string LastName
        {
            get => _decorated.LastName; set => _decorated.LastName = value;
        }
        
        [Display(Name = "DateOfBirth", ResourceType = typeof(Resources.LabelResource))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]        
        public DateTime? DateOfBirth
        {
            get => _decorated.DateOfBirth; set => _decorated.DateOfBirth = value;
        }
    }
}