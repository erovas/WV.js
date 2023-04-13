''' <summary>
''' Objeto para notificar algun evento [Error, Advertencia o Resultado]
''' <para>
''' Copyright (c) 2023, Emanuel Rojas Vásquez
''' https://github.com/erovas
''' BSD 3-Clause License
''' </para>
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class Notify(Of T)

    ''' <summary>
    ''' Value returned
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Value As T

    ''' <summary>
    ''' Is True when NotifyType is [Error] OR the Value is not Null [Nothing] and Value Type is Exception
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsError As Boolean

    ''' <summary>
    ''' Is True when NotifyType is [Warning] OR Value is Null [Nothing] and Value Type is different than Exception
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsWarning As Boolean

    ''' <summary>
    ''' Notification type specified by developer
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NotifyType As NotifyType

    ''' <summary>
    ''' Notification message
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Message As String

    ''' <summary>
    ''' Creation DateTime
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DateTime As DateTime

    ''' <summary>
    ''' Name of the method that generated the notification
    ''' </summary>
    ''' <returns></returns>
    Public Property Method As String

    ''' <summary>
    ''' Path of the file that generated the notification
    ''' </summary>
    ''' <returns></returns>
    Public Property File As String

    ''' <summary>
    ''' Number line that generated the notification
    ''' </summary>
    ''' <returns></returns>
    Public Property Line As Integer

    Public Sub New(Optional value As T = Nothing,
                   Optional notifyType As NotifyType = NotifyType.Result,
                   Optional message As String = Nothing,
                   <Runtime.CompilerServices.CallerMemberName> Optional method As String = Nothing,
                   <Runtime.CompilerServices.CallerFilePath> Optional file As String = Nothing,
                   <Runtime.CompilerServices.CallerLineNumber()> Optional line As Integer = 0)

        Me.Value = value
        Me.NotifyType = notifyType
        Me.Message = message
        Me.DateTime = DateTime.Now

        Me.Method = method
        Me.File = file
        Me.Line = line

        Dim valueType = GetType(T)
        Dim exceptionType = GetType(Exception)

        If notifyType = NotifyType.Error Then
            Me.IsError = True
        ElseIf value IsNot Nothing AndAlso valueType.IsAssignableFrom(exceptionType) Then
            Me.IsError = True
        Else
            Me.IsError = False
        End If

        If notifyType = NotifyType.Warning Then
            Me.IsWarning = True
        ElseIf value Is Nothing AndAlso Not valueType.IsAssignableFrom(exceptionType) Then
            Me.IsWarning = True
        Else
            Me.IsWarning = False
        End If

    End Sub



End Class

Public Enum NotifyType As Integer
    [Error] = 0
    Warning = 1
    Result = 2
End Enum