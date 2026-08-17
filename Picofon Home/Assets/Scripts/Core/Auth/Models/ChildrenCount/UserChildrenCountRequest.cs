using Picofon.Utils;

namespace Picofon.Core.Auth.Models.ChildrenCount
{
    using UnityEngine;

    public class UserChildrenCountRequest : MonoBehaviour
    {
        private readonly string url = string.Empty;
        public string Url => url;

        public UserChildrenCountRequest(string id)
        {
            url = $"https://ehc-picofon2.techlab.uoc.edu/api/children/owner/{id}?is_active=true";
        }

        public string ToJson()
        {
            return JsonHelper.ToJson(this);
        }
    }
}
