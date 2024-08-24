using System.Text;
using System.Reflection;
using UnityEngine;

public static class Logger
{
    public static string ColorWhite => "#FFFFFF";
    public static string ColorBlue => "#3498DB";
    public static string ColorTeal => "#1ABC9C";
    public static string ColorGold => "#F1C40F";
    public static string ColorRed => "#E74C3C";
    public static string ColorOrange => "#DC7633";
    public static string ColorGrey => "#7F8C8D";

    public static void LogDebug(object message = null)
    {
        var logMessage = FormatMessage("DEBUG", message, ColorOrange);
        Debug.Log(logMessage);
    }

    public static void LogImportant(object message = null)
    {
        var logMessage = FormatMessage("IMPORTANT", message, ColorBlue);
        Debug.Log(logMessage);
    }

    public static void LogError(object message = null)
    {
        var logMessage = FormatMessage("ERROR", message, ColorRed);
        Debug.LogError(logMessage);
    }

    public static void LogWarning(object message = null)
    {
        var logMessage = FormatMessage("WARNING", message, ColorGold);
        Debug.LogWarning(logMessage);
    }

    public static void Assert(bool expression, object message = null)
    {
        var logMessage = FormatMessage("ASSERTION FAILED", message, ColorRed);
        Debug.Assert(expression, logMessage);
    }

    private static string FormatMessage(string prefix, object message, string color)
    {
        var callingMethod = GetCallingMethod();
        var className = GetActualClassName(callingMethod) ?? "UnknownClass";
        var methodName = callingMethod?.Name ?? "UnknownMethod";
        var lineNumber = GetLineNumber(callingMethod);
        var formattedMessage = ColorizeSquareBracketsContent(message?.ToString(), ColorGold);

        return $"<color={color}>[{prefix}]</color> <color={ColorGrey}>({className}.{methodName}:{lineNumber})</color> {formattedMessage}";
    }

    private static MethodBase GetCallingMethod()
    {
        var stackTrace = new System.Diagnostics.StackTrace();
        var frames = stackTrace.GetFrames();

        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            if (method.DeclaringType != typeof(Logger) && method.DeclaringType != typeof(Debug))
            {
                return method;
            }
        }

        return null;
    }

    private static string GetActualClassName(MethodBase method)
    {
        if (method == null)
        {
            return null;
        }

        var stackTrace = new System.Diagnostics.StackTrace();
        var frames = stackTrace.GetFrames();

        foreach (var frame in frames)
        {
            var frameMethod = frame.GetMethod();
            if (frameMethod != null && frameMethod.DeclaringType != typeof(Debug) && frameMethod.DeclaringType != typeof(Logger))
            {
                return frameMethod.DeclaringType.Name;
            }
        }

        return method.DeclaringType.Name;
    }

    private static int GetLineNumber(MethodBase method)
    {
        var stackTrace = new System.Diagnostics.StackTrace(true);
        var frames = stackTrace.GetFrames();

        // Find the frame corresponding to the calling method
        for (int i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];
            if (IsFrameFromCallingMethod(frame, method))
            {
                return frame.GetFileLineNumber();
            }
        }

        return -1; // Line number not found
    }

    private static bool IsFrameFromCallingMethod(System.Diagnostics.StackFrame frame, MethodBase method)
    {
        var frameMethod = frame.GetMethod();

        return frameMethod != null && frameMethod == method;
    }

    private static string ColorizeSquareBracketsContent(string message, string color)
    {
        if (string.IsNullOrEmpty(message))
        {
            return message;
        }

        var result = new StringBuilder();
        int bracketLevel = 0;

        foreach (char c in message)
        {
            if (c == '[')
            {
                if (bracketLevel == 0)
                {
                    result.Append($"<color={color}>");
                }
                else
                {
                    result.Append(c);
                }
                bracketLevel++;
            }
            else if (c == ']')
            {
                bracketLevel--;
                if (bracketLevel == 0)
                {
                    result.Append("</color>");
                }
                else
                {
                    result.Append(c);
                }
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
