using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Objeto para notificar algun evento [Error, Advertencia o Resultado]
/// <para>
/// Copyright (c) 2023, Emanuel Rojas Vásquez
/// https://github.com/erovas
/// BSD 3-Clause License
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class Notify<T>
{
    /// <summary>
    /// 
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Is True when NotifyType is [Error] OR the Value is not Null and Value Type is Exception
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Is True when NotifyType is [Warning] OR Value is Null and Value Type is different than Exception
    /// </summary>
    public bool IsWarning { get; }

    /// <summary>
    /// Notification type specified by developer
    /// </summary>
    public NotifyType NotifyType { get; }

    /// <summary>
    ///  Notification message
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Creation DateTime
    /// </summary>
    public DateTime DateTime { get; }

    /// <summary>
    /// Name of the method that generated the notification
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Path of the file that generated the notification
    /// </summary>
    public string File { get; }

    /// <summary>
    /// Number line that generated the notification
    /// </summary>
    public int Line { get; }

    public Notify(T value = default(T), 
                  NotifyType notifyType = NotifyType.Result,
                  string message = null,
                  [CallerMemberName] string method = "",
                  [CallerFilePath] string file = "",
                  [CallerLineNumber] int line = 0)
    {
        this.Value = value;
        this.NotifyType = notifyType;
        this.Message = message;
        this.DateTime = DateTime.Now;

        this.Method = method;
        this.File = file;
        this.Line = line;

        Type valueType = typeof(T);
        Type exceptionType = typeof(Exception);

        if (notifyType == NotifyType.Error)
            this.IsError = true;
        else if(value != null && valueType.IsAssignableFrom(exceptionType))
            this.IsError = true;
        else
            this.IsError = false;

        if(notifyType == NotifyType.Warning)
            this.IsWarning = true;
        else if(value == null && !valueType.IsAssignableFrom(exceptionType))
            this.IsWarning = true;
        else
            this.IsWarning = false;
    }
}

public enum NotifyType
{
    Error = 0,
    Warning = 1,
    Result = 2
}