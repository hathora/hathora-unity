// Created by dylan@hathora.dev

using System;
using Newtonsoft.Json;

namespace Hathora.Scripts.Utils
{
    [Serializable]
    public struct HathoraEnvVars
    {
        public string Key;
        public string StrVal;
        
        
        public HathoraEnvVars(string _key, string _strVal)
        {
            this.Key = _key;
            this.StrVal = _strVal;
        }
        
        public string ToJson()
        {
            // Serialize with Newtonsoft
            return JsonConvert.SerializeObject(this);
        }
    }
}
