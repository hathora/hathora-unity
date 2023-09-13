
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace HathoraSdk.Models.Shared
{
    using Newtonsoft.Json;
    using System;
    using UnityEngine;
    
    
    /// <summary>
    /// Types of lobbies a player can create.
    /// 
    /// <remarks>
    /// 
    /// `private`: the player who created the room must share the roomId with their friends
    /// 
    /// `public`: visible in the public lobby list, anyone can join
    /// 
    /// `local`: for testing with a server running locally
    /// </remarks>
    /// </summary>
    public enum LobbyVisibility
    {
    	[JsonProperty("private")]
		Private,
		[JsonProperty("public")]
		Public,
		[JsonProperty("local")]
		Local,
    }
    
    public static class LobbyVisibilityExtension
    {
        public static string Value(this LobbyVisibility value)
        {
            return ((JsonPropertyAttribute)value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false)[0]).PropertyName ?? value.ToString();
        }

        public static LobbyVisibility ToEnum(this string value)
        {
            foreach(var field in typeof(LobbyVisibility).GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof(JsonPropertyAttribute), false)[0] as JsonPropertyAttribute;
                if (attribute != null && attribute.PropertyName == value)
                {
                    return (LobbyVisibility)field.GetValue(null);
                }
            }

            throw new Exception($"Unknown value {value} for enum LobbyVisibility");
        }
    }
    
}