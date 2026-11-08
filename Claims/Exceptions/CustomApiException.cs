using System.Net;

namespace Claims.Exceptions
{
    /// <summary>
    /// Represents a custom API exception containing structured error information
    /// that can be returned to API consumers.
    /// </summary>
    /// <remarks>
    /// In addition to the standard exception message, this exception provides
    /// a return code, detailed error description, an optional entity identifier,
    /// and an HTTP status code.
    ///
    /// Instances are created using the nested <see cref="Builder"/> class,
    /// which ensures that required properties are provided before construction.
    /// </remarks>
    public class CustomApiException : Exception
    {
        /// <summary>
        /// Gets the application-specific error code identifying the type of error.
        /// </summary>
        public string ReturnCode { get; }

        /// <summary>
        /// Gets a human-readable description of the error.
        /// </summary>
        public string Details { get; }

        /// <summary>
        /// Gets the optional identifier of the entity associated with the error.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomApiException"/>
        /// using the configuration provided by the builder.
        /// </summary>
        /// <param name="builder">The builder containing the exception properties.</param>
        private CustomApiException(Builder builder) : base(builder.Details)
        {
            ReturnCode = builder.ReturnCode!;
            Details = builder.Details!;
            Id = builder.Id;
            StatusCode = builder.StatusCode;
        }

        public sealed class Builder
        {
            internal string? ReturnCode { get; private set; }
            internal string? Details { get; private set; }
            internal string? Id { get; private set; }
            internal HttpStatusCode StatusCode { get; private set; }

            public Builder SetReturnCode(string returnCode)
            {
                ReturnCode = returnCode;
                return this;
            }

            public Builder SetDetails(string details)
            {
                Details = details;
                return this;
            }

            public Builder SetId(string? id)
            {
                Id = id;
                return this;
            }

            public Builder SetStatusCode(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
                return this;
            }

            public CustomApiException Build()
            {
                if (string.IsNullOrWhiteSpace(ReturnCode))
                    throw new InvalidOperationException("ReturnCode is required.");

                if (string.IsNullOrWhiteSpace(Details))
                    throw new InvalidOperationException("Details are required.");

                return new CustomApiException(this);
            }
        }
    }
}
