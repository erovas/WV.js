Imports System.Globalization

''' <summary>
''' Objeto para notificar algun evento [Error, Advertencia, Aviso, Resultado]
''' <para>
'''  Copyright (c) 2022, Emanuel Rojas Vásquez
'''  https://github.com/erovas
'''  BSD 3-Clause License
''' </para>
''' </summary>
Public NotInheritable Class Notification(Of T)

#Region "CAMPOS"


#End Region

#Region "CONSTRUCTORES"

    Public Sub New(<Runtime.CompilerServices.CallerMemberName> Optional method As String = Nothing,
                   <Runtime.CompilerServices.CallerFilePath> Optional file As String = Nothing,
                   <Runtime.CompilerServices.CallerLineNumber()> Optional line As Integer = 0)

        Me.DateTime = Date.Now
        Me.Line = line
        Me.Method = method
        Me.File = file
        Me.Type = NotificationType.Error

    End Sub

#End Region

    ''' <summary>
    ''' 0: Error
    ''' 1: Warning
    ''' 2: Result
    ''' </summary>
    ''' <returns></returns>
    Public Property Type As NotificationType

    ''' <summary>
    ''' Creation DateTime
    ''' </summary>
    ''' <returns></returns>
    Public Property DateTime As DateTime?

    ''' <summary>
    ''' Number line that generated the notification
    ''' </summary>
    ''' <returns></returns>
    Public Property Line As Integer

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
    ''' Notification message
    ''' </summary>
    ''' <returns></returns>
    Public Property Message As String

    ''' <summary>
    '''  Value to return with the notification
    ''' </summary>
    ''' <returns></returns>
    Public Property Value As T

End Class

Public Enum NotificationType As Integer
    [Error] = 0
    Warning = 1
    Result = 2
End Enum