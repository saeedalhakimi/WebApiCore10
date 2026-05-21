using DoimanDlls.Common;
using DoimanDlls.Exceptions;
using DoimanDlls.UserProfiles.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoimanDlls.UserProfiles
{
    public class UserProfile : AuditableEntity
    {
        private UserProfile() { }

        public Guid UserProfileID { get; private set; }
        public string IdentityID { get; private set; } = null!;
        public BasicInformation BasicInfo { get; private set; } = null!;

        public static UserProfile Create(Guid userProfileID, string identityID, BasicInformation basicInformation)
        {
            if (userProfileID == Guid.Empty)
                throw new DomainException("User profile ID is required.");

            if (string.IsNullOrWhiteSpace(identityID))
                throw new DomainException("Identity ID is required.");

            if (basicInformation is null)
                throw new DomainException("Basic information is required.");

            var userProfile = new UserProfile
            {
                UserProfileID = userProfileID,
                IdentityID = identityID,
                BasicInfo = basicInformation
            };
            userProfile.MarkCreated(DateTime.UtcNow);

            return userProfile;
        }

        public void UpdateBasicInformation(BasicInformation newBasicInformation)
        {
            if (newBasicInformation is null)
                throw new DomainException("New basic information is required.");
            BasicInfo = newBasicInformation;
            MarkUpdated(DateTime.UtcNow);
        }

    }
}
