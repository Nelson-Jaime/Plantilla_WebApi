using System;
using System.Collections.Generic;
using System.Linq;

namespace Plantilla.WebApi.Busines.Contracts.Domain
{
    public class OperationResult
    {
        private readonly List<ErrorObject> _errors = new List<ErrorObject>();

        public IEnumerable<ErrorObject> Errors { get { return _errors; } }

        public bool HasErrors() => _errors.Count > 0;
        public void AddError(int code, string message) => _errors.Add(new ErrorObject { Code = code, Message = message });
        public void AddError(Exception exception) => _errors.Add(new ErrorObject { Code = 9999, Message = exception.Message, Exception = exception });
        public void AddError(ErrorObject errorObject) => _errors.Add(new ErrorObject { Code = errorObject.Code, Message = errorObject.Message, Exception = errorObject.Exception });
        public void AddErrors(IEnumerable<ErrorObject> errors) => _errors.AddRange(errors);
        public bool HasSomeException() => _errors.Any(c => c.Exception != null);
    }

    public class OperationResult<T> : OperationResult
    {
        public T Result { get; set; }

        public OperationResult() { }

        public OperationResult(T result) => Result = result;

        public void SetResult(T result) => Result = result;
    }
}
