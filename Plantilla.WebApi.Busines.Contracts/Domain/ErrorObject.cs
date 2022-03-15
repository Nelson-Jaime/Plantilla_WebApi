using System;
using System.Text.Json.Serialization;

namespace Plantilla.WebApi.Busines.Contracts.Domain
{
    public class ErrorObject
    {
        public int Code { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public Exception Exception { get; set; }
    }
}
