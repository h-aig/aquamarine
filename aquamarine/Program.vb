' (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet)
Imports System
Imports System.Net
Imports System.Xml

Module Program
    
    Sub Main(args As String())
        dim client as new WebClient
        Dim rawRSS As string
        dim prettyRSS as new xmldocument()
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.        
        
        Console.WriteLine("aquamarine, a typing test for your favorite blog posts")
        Console.WriteLine()
        
        try
        rawRSS = client.DownloadString("https://feeds.bbci.co.uk/news/england/rss.xml") ' Download data as string
            Catch ex as Exception ' if the url is incorrect
                rawRSS = "There was an exception!" ' EXPAND ON THIS: WHAT TO DO IF THE URL IS INCORRECT?
            End try
        prettyRSS.loadxml(rawRSS)
        
        dim item as XmlNode
        Dim nodeList as XmlNodeList 
        Dim root as XmlNode = prettyRSS.DocumentElement
        
        nodeList=root.SelectNodes("/rss/channel/item")
        
        for each item in nodeList      
            Console.WriteLine("cheese")
        next 
        
        Console.ReadLine()
    End Sub
    
End Module
