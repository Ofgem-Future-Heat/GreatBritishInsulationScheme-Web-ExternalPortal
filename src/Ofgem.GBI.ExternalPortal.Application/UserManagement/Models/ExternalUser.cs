﻿using Ofgem.GBI.ExternalPortal.Application.UserManagement.Enums;
using System.Text.Json.Serialization;

namespace Ofgem.GBI.ExternalPortal.Application.UserManagement.Models
{
    [Serializable]
    public class ExternalUser
    {
        [JsonPropertyName("externalUserId")]
        public Guid ExternalUserId { get; set; }
        [JsonPropertyName("uniqueUserId")]
        public string ProviderUserId { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;




        [JsonPropertyName("userType")]
        public ExternalUserType UserType { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("supplier")]
        public Supplier? Supplier { get; set; }
    }
}

