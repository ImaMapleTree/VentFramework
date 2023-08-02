using System;

namespace VentLib.Logging;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LoggedAttribute: Attribute
{
}