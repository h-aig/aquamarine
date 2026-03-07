' (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet)
Imports System
Imports System.Net
Imports System.Xml

Module Program
    
    Sub Main(args As String())
        dim rssList(0) as String ' stores list of all rss posts
        dim client as new WebClient ' client
        Dim rawRSS As string ' this stores the downloaded string
        dim xmlRSS as new xmldocument() ' this holds the downloaded string as an xmldocument
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.        
        
        Console.WriteLine("aquamarine, a typing test for your favorite blog posts")
        Console.WriteLine()
        
        try
        rawRSS = client.DownloadString("https://feeds.bbci.co.uk/news/england/rss.xml") ' Download data as string
            Catch ex as Exception ' if the url is incorrect
                rawRSS = "There was an exception!" ' EXPAND ON THIS: WHAT TO DO IF THE URL IS INCORRECT?
            End try
        xmlRSS.loadxml(rawRSS) ' take string downloaded and store in xmldocument variable xmlRSS
        
        dim item as XmlNode
        Dim nodeList as XmlNodeList ' i need a try here-- if the xml is invalid
        Dim root as XmlNode = xmlRSS.DocumentElement
        
        nodeList=root.SelectNodes("/rss/channel/item")
        
        for each item in nodeList
            dim title as XmlNode = item.SelectSingleNode("title")
            'Console.WriteLine(title.InnerText)
            Array.Resize(rssList, rssList.Length+1)
            rssList(rssList.Length-1) = title.InnerText
        next 
        for i as integer = 0 to rssList.Length-1
            console.WriteLine(rssList(i))
        Next
        
        Console.ReadLine()
    End Sub
    
End Module
