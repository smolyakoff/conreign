using System;

namespace Conreign.Framework.Contracts.Core.Data
{
    public interface IError
    {
        string Message { get; }

        Exception Exception { get; }
    }
}
