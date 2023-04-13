Imports System.Reflection

Module Module1

    Sub Main()

        Dim notificar As Notification(Of Dictionary(Of String, String))

        notificar = Algo()

        Dim aalgo = 123

        Dim dict2 As Dictionary(Of String, String)

        dict2 = notificar.Value

        Dim notifier As Notify(Of Exception)

        notifier = New Notify(Of Exception)()

        Dim notifier2 As Notify(Of String)
        notifier2 = New Notify(Of String)("")

        Try

        Catch ex As Exception
            'ex.StackTrace
        End Try

    End Sub

    Private Function Algo() As Notification(Of Dictionary(Of String, String))

        Dim notificar As New Notification(Of Dictionary(Of String, String))()

        Dim Dict = New Dictionary(Of String, String)()

        Dict("asdad") = "sfdsfdsdf"

        notificar.Value = Dict

        Return notificar

    End Function

End Module
