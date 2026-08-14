using Picofon.Core.Auth.DTOs;

namespace Picofon.Core.Auth.DTOs.Responses
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class UserChildrenResponse
    {
        [JsonInclude]
        public bool Success { get; set; }

        [JsonInclude]
        public List<ChildListItemDTO> Data { get; set; }
    }
}
