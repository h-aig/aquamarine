' (<3 dotnetperls, https://www.dotnetperls.com/webclient-vbnet)
Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Mime
Imports System.Xml
Imports AngleSharp.Html.Dom
Imports NReadability
Imports ReadSharp

Module Program
    
    Public NotInheritable Class HtmlDocument
        
    End Class
    
    Sub Main(args As String())
        testSub()
        Console.Clear() ' get rid of the accursed yellow boot text in rider-- no effect in prod
        Console.ForegroundColor = ConsoleColor.White
        dim rssList(0) as String ' stores list of all rss posts
        dim client as new WebClient ' client
        Dim rawRSS As string ' this stores the downloaded string
        dim xmlRSS as new xmldocument() ' this holds the downloaded string as an xmldocument
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.   
        
        Console.WriteLine("Aquamarine. A new way to consume information.")
        Console.WriteLine()
        
        try
        rawRSS = client.DownloadString("https://feeds.arstechnica.com/arstechnica/index.xml") ' Download data as string
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
                Case ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter
                    ' Handle each key individually
                    If key.Key = ConsoleKey.UpArrow Then
                        if not Console.CursorTop = 2
                            console.cursorleft = 0
                            Console.BackgroundColor = ConsoleColor.Black
                            Console.ForegroundColor = ConsoleColor.white
                            Console.Write(rssList(Console.CursorTop - 2))
                            console.SetCursorPosition(0, console.CursorTop - 1)
                            Console.BackgroundColor = ConsoleColor.White
                            Console.ForegroundColor = ConsoleColor.Black
                            Console.Write(rssList(Console.CursorTop - 2))
                        End If
                    ElseIf key.Key = ConsoleKey.DownArrow Then
                        if not Console.CursorTop = rssList.Length + 1 'array starts at 0, so it's +1 and not +2 in terms of boundary checking
                            console.CursorLeft = 0
                            Console.BackgroundColor = ConsoleColor.black
                            Console.ForegroundColor = ConsoleColor.white
                            Console.Write(rssList(Console.CursorTop - 2))
                            console.SetCursorPosition(0, console.CursorTop + 1)
                            Console.BackgroundColor = ConsoleColor.White
                            Console.ForegroundColor = ConsoleColor.Black
                            Console.Write(rssList(Console.CursorTop - 2))
                        end if
                    ElseIf key.Key = ConsoleKey.Enter Then
                        Console.WriteLine("Enter pressed")
                        Exit While
                    End If
            End Select
        End While
        
        Console.ReadLine()
    End Sub
    
    sub testSub()
        
        dim article as SmartReader.article = SmartReader.Reader.ParseArticle("https://www.theverge.com/ai-artificial-intelligence/902368/openai-sora-dead-ai-video-generation-competition")
        dim rawContent as string = article.Content
        dim prettyParagraphs(0)
        dim endOfParagraph as boolean
        dim currentParagraph as integer
        dim startParagraphAfter as Integer
        dim ignoreChars as boolean = False
        
        for i as integer = 0 to rawContent.Length - 1
            
            currentParagraph = prettyParagraphs.Length - 1
            
            if rawContent.Substring(i, 1) = "<"
                if rawContent.Substring(i+1, 1) = "p"
                    if rawContent.Substring(i+2, 1) = ">"
                        endOfParagraph = False
                        startParagraphAfter = i+2
                    End If
                End If
                
                If rawContent.Substring(i+1, 1) = "/"
                    if rawContent.Substring(i+2, 1) = "p"
                        if rawContent.Substring(i+3, 1) = ">"
                            endOfParagraph = True
                            Array.Resize(prettyParagraphs, prettyParagraphs.Length + 1)
                        End If
                    End If
                End If
            End If
            
            if rawcontent.Substring(i, 1) = "<"
                ignoreChars = True
            End If
            
            if i > startParagraphAfter And  endOfParagraph = False and ignoreChars = False
                prettyParagraphs(currentParagraph) = prettyParagraphs(currentParagraph) & rawContent.Substring(i, 1)
            End If
            
            if rawContent.Substring(i, 1)= ">"
                ignoreChars = False
            End If
            
        Next
        
        
        for i as integer = 0 to prettyParagraphs.Length - 1
            Console.WriteLine(prettyParagraphs(i))
            Console.WriteLine()
        Next
        
        Console.ReadLine()
    End sub
End Module
