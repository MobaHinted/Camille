using System;

namespace Camille.Enums
{
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum Division : byte
    {
        /// <summary>
        /// "NONE", none available.
        /// </summary>
        NONE = 0,

        I = 1,
        II = 2,
        III = 3,
        IV = 4,
        [Obsolete("Removed for 2019.")] V = 5,
    }
}