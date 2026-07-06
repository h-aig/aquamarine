Imports System.Net
Imports System.ServiceModel.Syndication
Imports System.Text
Imports System.Xml

Module Program
    Private _nodeList as XmlNodeList
    Private _titlesAndLinks (,) as String ' create 2d array for storing titles/links
    
    sub Main()
        Console.Clear()
        dim feedUrl as string = getFeed() ' create feed url with verification
        GetNodes(feedUrl)
        
        
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
        for each item in _nodeList
            Console.WriteLine(item.SelectSingleNode("title").InnerText)
        Next
        
    End sub 
    
End Module