' (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet)
Imports System
Imports System.Net
Imports System.Xml

Module Program
    Sub Main(args As String())
        Console.Clear() ' get rid of the accursed yellow boot text in rider-- no effect in prod
        dim rssList(0) as String ' stores list of all rss posts
        dim client as new WebClient ' client
        Dim rawRSS As string ' this stores the downloaded string
        dim xmlRSS as new xmldocument() ' this holds the downloaded string as an xmldocument
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.   
        
        Console.WriteLine("Aquamarine. A new way to consume information.")
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
        
        for each item in nodeList ' for each post in the feed...
            dim title as XmlNode = item.SelectSingleNode("title") ' find post
            rssList(rssList.Length-1) = title.InnerText ' place post in rssList
            Array.Resize(rssList, rssList.Length+1) ' increase size of array by 1
        next 
        Array.resize(rssList, rsslist.length-1) ' get rid of empty string at the end
        for i as integer = 0 to rssList.Length-1
            console.WriteLine(rssList(i))
        Next
        
        Console.SetCursorPosition(0,2)
        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.Black
        Console.Write(rssList(0))
            
        While True
            Dim key As ConsoleKeyInfo = Console.ReadKey(True) ' True = don't display
            Select Case key.Key ' Merge all keys into one Case -- https://chatgpt.com/s/t_69ac323c5adc8191ac526fa15f7b7065
                Case ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Enter
                    ' Handle each key individually
                    If key.Key = ConsoleKey.UpArrow Then
                        Console.WriteLine("Up pressed")
                    ElseIf key.Key = ConsoleKey.DownArrow Then
                        Console.WriteLine("Down pressed")
                    ElseIf key.Key = ConsoleKey.LeftArrow Then
                        Console.WriteLine("Left pressed")
                    ElseIf key.Key = ConsoleKey.RightArrow Then
                        Console.WriteLine("Right pressed")
                    ElseIf key.Key = ConsoleKey.Enter Then
                        Console.WriteLine("Enter pressed")
                        Exit While
                    End If
            End Select
        End While
        
        Console.ReadLine()
    End Sub
    
End Module
