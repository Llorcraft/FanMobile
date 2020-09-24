using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FanApp.Models
{
    public class UserModel
    {

        [JsonProperty("id")]
        public int? Id{ get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("comments")]
        public string Comments { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public string TitleAndFullName
        {
            get
            {
                return $"{Title} {FirstName} {LastName}".Trim();
            }
        }
    }
}
