using System;
using System.Collections.Generic;
using System.Text;

namespace VentLib.Logging.Accumulators;

internal class ArgumentAccumulator : ILogAccumulator
{
    public string AccumulatorID => nameof(ArgumentAccumulator);

    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        string initialString = composite.Message;

        StringBuilder builder = new();
        builder.EnsureCapacity(initialString.Length + arguments.Arguments.Length * 8);
        
        Stack<string> stack = new();
        int nArgs = 0;
        int pos = 0;

        while (true)
        {
            if (pos >= initialString.Length)
            {
                if (stack.Count == 0) break;
                ThrowFormatException(stack.Pop(), pos - 1, initialString);
            }
            
            char c = initialString[pos++];

            switch (c)
            {
                case '}':
                {
                    int argNum = nArgs;
                    bool setArgNum = false;
                
                    if (stack.Count == 0) FormatException();
                    string stackValue = stack.Pop();
                    if (stackValue == "\\")
                    {
                        builder.Append(c);
                        continue;
                    }

                    if (int.TryParse(stackValue, out int i))
                    {
                        argNum = i;
                        setArgNum = true;
                        stackValue = stack.Pop();
                    }
                    else nArgs++;
                
                    if (stackValue != "{") FormatException();
                    if (argNum >= arguments.Arguments.Length)
                    {
                        throw new FormatException(setArgNum
                            ? $"Not enough arguments provided for indexer of {argNum} (Total={arguments.Arguments.Length})"
                            : $"Not enough arguments provided. (Expected = {argNum}, Total = {arguments.Arguments.Length})");
                    }

                    builder.Append(arguments.Arguments[argNum]);
                    continue;
                }
                case '{' when stack.Count == 0:
                    stack.Push("{");
                    continue;
                case '{' when stack.Peek() == "\\":
                    stack.Pop();
                    continue;
                case '{':
                    FormatException();
                    break;
                case '\\':
                    stack.Push("\\");
                    continue;
            }

            if (stack.Count == 0)
            {
                builder.Append(c);
                continue;
            }

            string peek = stack.Peek();
            if (!((peek == "{" || IsNumeric(peek)) && char.IsDigit(c))) 
                FormatException();
            
            if (IsNumeric(peek)) stack.Push(peek + c);
            else stack.Push(c.ToString());

            continue;

            void FormatException()
            {
                ThrowFormatException(c.ToString(), pos, initialString);
            }
        }

        return composite.SetMessage(builder.ToString());
    }
    
    private static bool IsNumeric(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c) && c != '.')
            {
                return false;
            }
        }

        return true;
    }
    
    private static void ThrowFormatException(string str, int position, string sourceString)
    {
        throw str switch
        {
            "\\" => Exc("Illegal Escape Character"),
            "{" => Exc("Invalid Opening Brace"),
            "}" => Exc("Invalid Closing Brace"),
            _ => Exc($"Invalid Syntax ({str})")
        };

        Exception Exc(string msg) => new FormatException($"{msg} at Position {position} of String \"{sourceString}\"");
    }



}