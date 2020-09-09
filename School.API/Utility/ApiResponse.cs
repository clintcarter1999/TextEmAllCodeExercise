using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace School.API.Utility
{
    /// <summary>
    /// This is used to provide a standard header for ActionResult responses in a JSon format
    /// verses a plain string based response
    /// </summary>
    public class ApiResponse
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        // [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }


        public ApiResponse(int statusCode, string statusMessage = null)
        {
            StatusCode = statusCode;
            StatusDescription = statusMessage ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public ApiResponse(int statusCode, string statusDescription, string message) : this(statusCode, statusDescription)

        {
            this.Message = message;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                default:
                    try
                    {
                        HttpStatusCode code = (HttpStatusCode)statusCode;
                        return code.ToString();
                    }
                    catch { }
                    return null;
            }

        }
    }


    public class InternalServerError : ApiResponse
    {
        public InternalServerError()
            : base(500, HttpStatusCode.InternalServerError.ToString())
        {
        }


        public InternalServerError(string message)
            : base(500, HttpStatusCode.InternalServerError.ToString(), message)
        {
        }
    }

    public class NotFoundResponse : ApiResponse
    {
        public NotFoundResponse()
            : base((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString())
        {
        }


        public NotFoundResponse(string message)
            : base((int)HttpStatusCode.NotFound, HttpStatusCode.NotFound.ToString(), message)
        {
        }
    }

    public class BadRequestResponse : ApiResponse
    {
        public BadRequestResponse()
            : base((int)HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString())
        {
        }

        public BadRequestResponse(string message)
            : base((int)HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString(), message)
        {
        }
    }

}


