Imports System.Net
Imports System.ServiceModel.Syndication
Imports System.Text
Imports System.Xml

Module Program
    Private _nodeList as XmlNodeList
    Private _titlesAndLinks(,) ' create 2d array for storing titles/links

    sub Main()
        Console.Clear()
        dim feedUrl as string = getFeed() ' create feed url with verification
        GetNodes(feedUrl)
        printStories()
        selectStory()
        
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

    sub GetNodes(feedUrl)
        dim synFeed as SyndicationFeed

        Using reader as XmlReader = xmlreader.Create(feedUrl)
            synFeed = SyndicationFeed.Load(reader) ' make feed or something
        End Using

        dim sb as new StringBuilder()
        using writer as XmlWriter = XmlWriter.Create(sb)
            synFeed.SaveAsRss20(writer)
        End Using

        dim xmlRss as new XmlDocument()
        xmlRss.LoadXml(sb.ToString())

        dim root as XmlLinkedNode = xmlRss.DocumentElement
        _nodeList = root.SelectNodes("//*[local-name()='entry' or local-name()='item']")

        dim item as XmlNode
        dim indexForArrayRedim = 0
        for each item in _nodeList
            indexForArrayRedim += 1
        Next
        indexForArrayRedim -= 1

        ReDim _titlesAndLinks(indexForArrayRedim, indexForArrayRedim)

        dim indexForPopulatingArrays = 0
        for each item in _nodeList
            _titlesAndLinks(indexForPopulatingArrays, 0) = item.SelectSingleNode("title").InnerText
            _titlesAndLinks(indexForPopulatingArrays, 1) = item.SelectSingleNode("link").InnerText
            indexForPopulatingArrays += 1
        Next
    End sub

    sub printStories()
        Console.Clear()

        dim i as Integer
        for i = 0 to _titlesAndLinks.length
            if i < 10
                Try
                    Console.Writeline(_titlesAndLinks(i, 0))
                Catch
                    Exit For
                end Try
            End If
        Next
        
        Console.WriteLine()
        Console.Write("Select a story using the keys on your keyboard (1-9)")
        
    End sub
    sub selectStory()
        dim key as ConsoleKeyInfo
        
        do until Key.Key >= ConsoleKey.D1 AndAlso Key.Key <= ConsoleKey.D9  
            key = console.ReadKey(True)
        loop
        
        
    End sub
    
End Module