Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Mime
Imports System.Runtime.Intrinsics.X86
Imports System.Security.Cryptography
Imports System.Xml
Imports AngleSharp.Html.Dom
Imports System.ServiceModel.Syndication
Imports System.Text
Imports SmartReader

Module Program
    sub Main()
        Console.Clear()
        dim feedUrl as string = getFeed()
        dim synFeed as SyndicationFeed
        
        Using reader as XmlReader = xmlreader.Create(feedUrl)
            synFeed = SyndicationFeed.Load(reader)
        End Using
        
        
    End sub

    Function GetFeed()
        dim client as new WebClient
        client.Headers("User-Agent") = "Mozilla/4.0"
        
        Console.Write("Link to feed: ")
        dim submittedFeed as string = console.ReadLine()
        dim feedValid as Boolean
        dim synFeed as SyndicationFeed

        do until feedValid = True
            Try
                Using reader as XmlReader = XmlReader.Create(submittedfeed)
                End Using
                feedValid = True
            Catch
                console.Write("Holy invalid url. try again nonce: ")
                feedValid = False
                submittedFeed = console.Readline()
            End Try
        loop
        
        return submittedFeed
        
    End Function
End Module