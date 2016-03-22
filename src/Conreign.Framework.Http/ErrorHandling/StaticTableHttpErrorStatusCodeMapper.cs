using System;
using System.Collections.Generic;
using System.Net;
using Conreign.Framework.Contracts.Core;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Framework.Http.ErrorHandling
{
    public class StaticTableHttpErrorStatusCodeMapper : IHttpErrorStatusCodeMapper
    {
        private readonly Dictionary<string, HttpStatusCode> _table;

        public StaticTableHttpErrorStatusCodeMapper() : this(CreateDefaultTable())
        {
        }

        public StaticTableHttpErrorStatusCodeMapper(Dictionary<string, HttpStatusCode> table)
        {
            _table = table;
        }

        public HttpStatusCode? GetStatusCodeForError(IError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            if (!_table.ContainsKey(error.Type))
            {
                return null;
            }
            return _table[error.Type];
        }

        public static Dictionary<string, HttpStatusCode> CreateDefaultTable()
        {
            return new Dictionary<string, HttpStatusCode>
            {
                [Errors.ServiceUnavailable] = HttpStatusCode.ServiceUnavailable,
                [Errors.HandlerNotFound] = HttpStatusCode.NotFound
            };
        }
    }
}