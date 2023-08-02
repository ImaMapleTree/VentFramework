using System;

namespace VentLib.Logging.Accumulators;

[Flags]
public enum StringOptions
{
    NoPadding = 0,
    PadLeft = 1,
    PadRight = 2
}