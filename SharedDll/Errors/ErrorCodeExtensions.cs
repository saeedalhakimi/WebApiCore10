using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace SharedDll.Errors
{
    public static class ErrorCodeExtensions
    {
        public static string GetDescription(this ErrorCode errorCode)
        {
            var field = errorCode.GetType().GetField(errorCode.ToString());
            var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
            return attr?.Description ?? errorCode.ToString();
        }

        public static HttpStatusCode ToHttpStatusCode(this ErrorCode errorCode) =>
            errorCode switch
            {
                ErrorCode.NotFound => HttpStatusCode.NotFound,
                ErrorCode.BadRequest or ErrorCode.InvalidInput or ErrorCode.ValidationError or ErrorCode.DomainValidationError
                    => HttpStatusCode.BadRequest,
                ErrorCode.Unauthorized => HttpStatusCode.Unauthorized,
                ErrorCode.Forbidden or ErrorCode.AccessDenied => HttpStatusCode.Forbidden,
                ErrorCode.Conflict or ErrorCode.DuplicateResource => HttpStatusCode.Conflict,
                ErrorCode.TooManyRequests => HttpStatusCode.TooManyRequests,
                ErrorCode.ResourceCreationFailed => HttpStatusCode.BadRequest,
                ErrorCode.ResourceUpdateFailed => HttpStatusCode.BadRequest,
                ErrorCode.ResourceDeletionFailed => HttpStatusCode.BadRequest,
                ErrorCode.AssignmentFailed => HttpStatusCode.BadRequest,
                ErrorCode.DatabaseError => HttpStatusCode.InternalServerError,
                ErrorCode.InternalServerError or ErrorCode.UnknownError or ErrorCode.ApplicationError or ErrorCode.DependencyFailure
                    => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError
            };

        public static ErrorSeverity ToSeverity(this ErrorCode errorCode) =>
            errorCode switch
            {
                ErrorCode.InternalServerError or ErrorCode.UnknownError or ErrorCode.ApplicationError or ErrorCode.DatabaseError
                    => ErrorSeverity.Critical,
                ErrorCode.ValidationError or ErrorCode.InvalidInput or ErrorCode.BadRequest or ErrorCode.DomainValidationError
                    => ErrorSeverity.Warning,
                _ => ErrorSeverity.Info
            };
    }
}
