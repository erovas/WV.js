Public Class Notify(Of T)
    Implements INotify(Of T)

    Public ReadOnly Property Value As T Implements INotify(Of T).Value
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Sub New(type As String, message As String)

    End Sub

    Public Sub New(type As String, value As T)

    End Sub

    Public Sub New(type As String)

    End Sub



    Public Sub New(type As String, value As Object, Optional message As String = Nothing)

    End Sub



End Class
