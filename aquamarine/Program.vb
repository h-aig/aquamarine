Imports System
Imports System.Net

Module Program
    Sub Main(args As String())
        Console.WriteLine("aquamarine, a typing test for your favorite blog posts")
        Console.WriteLine()
        
        ' Resource acquisition statement. (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet example 3)
        Using client As New WebClient
            ' Set one of the headers.
            client.Headers("User-Agent") = "Mozilla/4.0"

            ' Download data as string
            Dim value As string = client.DownloadString("http://www.example.com/")

            ' Display result.
            Console.WriteLine(value)
        End Using
        
        Console.ReadLine()
    End Sub
End Module
