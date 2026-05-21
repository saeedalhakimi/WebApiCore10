using DoimanDlls.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoimanDlls.UserProfiles.ValueObjects
{
    public sealed record BasicInformation
    {
        private BasicInformation() { }

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;

        public DateTime DateOfBirth { get; private set; }

        public static BasicInformation Create(string firstName, string lastName, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name is required.");

            if (dateOfBirth > DateTime.UtcNow)
                throw new DomainException("Date of birth cannot be in the future.");

            if (dateOfBirth > DateTime.UtcNow.AddYears(-13))
                throw new DomainException("User must be at least 13 years old.");

            return new BasicInformation
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth
            };
        }

        public int Age =>
            DateTime.UtcNow.Year - DateOfBirth.Year -
            (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    }
}
