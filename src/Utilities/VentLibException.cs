using System;
using System.Runtime.Serialization;

namespace VentLib.Utilities;

public class VentLibException: Exception
{
    public VentLibException()
    {
    }

    protected VentLibException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public VentLibException(string? message) : base(message)
    {
    }

    public VentLibException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}