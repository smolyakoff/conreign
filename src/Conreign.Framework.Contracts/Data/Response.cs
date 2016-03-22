using System;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Framework.Contracts.Data
{
    public class Response
    {
        private Response(object result)
        {
            Result = result;
        }

        private Response(IError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            Error = error;
        }

        public object Result { get; }

        public IError Error { get; }

        public bool IsSuccess => Error == null;

        public ResponseStatus Status
        {
            get
            {
                if (IsSuccess)
                {
                    return ResponseStatus.Success;
                }
                return Error is UserError ? ResponseStatus.UserError : ResponseStatus.Failure;
            }
        }

        public static Response Success(object result)
        {
            return new Response(result);
        }

        public static Response Problem(IError error)
        {
            return new Response(error);
        }
    }
}