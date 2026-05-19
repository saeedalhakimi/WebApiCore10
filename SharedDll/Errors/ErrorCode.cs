using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharedDll.Errors
{
    public enum ErrorCode
    {
        // Generic / System
        [Description("An unknown error occurred.")]
        UnknownError = 999,

        [Description("The server encountered an unexpected error.")]
        InternalServerError = 1000,

        [Description("The operation was canceled.")]
        OperationCanceled = 1006,

        [Description("A dependency service failed.")]
        DependencyFailure = 1012,

        [Description("Unexpected application error.")]
        ApplicationError = 1005,

        // Validation
        [Description("The request was malformed.")]
        BadRequest = 1002,

        [Description("Invalid input provided.")]
        InvalidInput = 1003,

        [Description("Domain validation failed.")]
        DomainValidationError = 1007,

        [Description("Validation failed for the given input.")]
        ValidationError = 1013,

        // Security / Auth
        [Description("Authentication is required.")]
        Unauthorized = 1009,

        [Description("You do not have permission to perform this action.")]
        Forbidden = 1008,

        [Description("Access denied.")]
        AccessDenied = 1016,

        [Description("The user account is locked.")]
        UserLocked = 1017,

        [Description("No valid refresh token found.")]
        ValidRefreshTokenNotFound = 1018,

        [Description("Too many requests. Please try again later.")]
        TooManyRequests = 1011,

        // Resource Handling
        [Description("The requested resource was not found.")]
        NotFound = 1001,

        [Description("A conflict occurred with the current state of the resource.")]
        Conflict = 1010,

        [Description("A duplicate resource exists.")]
        DuplicateResource = 1021,

        [Description("Failed to create the resource.")]
        ResourceCreationFailed = 1014,

        [Description("Failed to update the resource.")]
        ResourceUpdateFailed = 1019,

        [Description("Failed to delete the resource.")]
        ResourceDeletionFailed = 1020,

        [Description("Failed to assign the resource.")]
        AssignmentFailed = 1015,

        [Description("A database error occurred.")]
        DatabaseError = 1004
    }
}
