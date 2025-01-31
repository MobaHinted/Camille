﻿namespace Camille.Enums
{
    /// <summary>
    /// Contains tier names (CHALLENGER, MASTER, etc.)
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum Tier : byte
    {
        CHALLENGER  =  20,
        GRANDMASTER =  40,
        MASTER      =  60,
        DIAMOND     =  80,
        EMERALD     =  90,
        PLATINUM    = 100,
        GOLD        = 120,
        SILVER      = 140,
        BRONZE      = 160,
        IRON        = 180,

        /// <summary>Returned by LCU only.</summary>
        NONE = byte.MaxValue,
    }
}