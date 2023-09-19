
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Newtonsoft.Json.Converters;

namespace HathoraCloud.Models.Operations
{
    using HathoraCloud.Models.Shared;
    using Newtonsoft.Json;
    using System;
    using UnityEngine;
    
    
    [Serializable]
    public class CreateLocalLobbyRequestBody
    {
        /// <summary>
        /// User input to initialize the game state. Object must be smaller than 64KB.
        /// </summary>
        [SerializeField]
        [JsonProperty("initialConfig")]
        public LobbyInitialConfig InitialConfig { get; set; } = default!;
        
        [SerializeField]
        [JsonProperty("region")]
        [JsonConverter(typeof(StringEnumConverter))] // (!) Added manually to serialize to string instead of int --Dylan
        public Region Region { get; set; } = default!;
        
    }
    
}