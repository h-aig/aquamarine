' (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet)
Imports System
Imports System.Net

Module Program
    Sub Main(args As String())
        dim webpagepretty as String
        dim client as new WebClient
        Dim webpageraw As string
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.        
        
        Console.WriteLine("aquamarine, a typing test for your favorite blog posts")
        Console.WriteLine()
        
        try
        webpageraw = client.DownloadString("http://www.example.com/") ' Download data as string
            Catch ex as Exception ' if the url is incorrect
                webpageraw = "There was an exception!" ' EXPAND ON THIS: WHAT TO DO IF THE URL IS INCORRECT?
            End Try
        
        Console.WriteLine((webpageraw))
        
        Console.ReadLine()
    End Sub
    
End Module
