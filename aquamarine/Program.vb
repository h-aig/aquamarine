Imports System.Net
Imports System.ServiceModel.Syndication
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports SmartReader

Module Program
    Private _nodeList as XmlNodeList
    Private _titlesAndLinks(,) ' create 2d array for storing titles/links
    Private _noStories as Integer
    Private _storySelected as Integer
    Private  _keySelectstory as ConsoleKeyInfo

    sub Main()
        Console.Clear()
        dim feedUrl as string = getFeed() ' create feed url with verification
        GetNodes(feedUrl)
        printStories()

        _storySelected  = selectStory()
        Console.Clear()
        getContent(_titlesAndLinks(_storySelected, 1))
    End sub

    Function GetFeed()
        dim client as new WebClient
        client.Headers("User-Agent") = "Mozilla/4.0"

        Console.Write("Link to feed (IF YOU DON'T KNOW WHAT AN RSS FEED JUST PRESS ENTER, A SAMPLE THE VERGE FEED WILL BE PROVIDED): ")
        dim submittedFeed as string = console.ReadLine()
        if submittedFeed = "" 
            submittedFeed = "https://www.theverge.com/rss/partner/subscriber-only-full-feed/rss.xml"
        End If
        dim feedValid as Boolean
        dim synFeed as SyndicationFeed

        do until feedValid = True
            Try
                Using reader as XmlReader = XmlReader.Create(submittedfeed)
                End Using
                feedValid = True
            Catch
                console.Write("Invalid url. try again: ")
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

        for _noStories = 0 to _titlesAndLinks.length
            if _noStories < 10
                Try
                    Console.Writeline(_titlesAndLinks(_noStories, 0))
                Catch
                    Exit For
                end Try
            End If
        Next

        Console.WriteLine()
        Console.Write("Select a story using the keys on your keyboard (1-9)")
    End sub

    function selectStory()
        
        
        Do
            _keySelectstory = Console.ReadKey(True)
        Loop Until _keySelectstory.Key >= ConsoleKey.D1 AndAlso _keySelectstory.Key <= CType(ConsoleKey.D0 + _noStories, ConsoleKey)
        return _keySelectstory.Key - 49
    End function

    sub getContent(link)
        Console.Clear()
        Console.WriteLine("Loading...")

        dim sr = new Reader(link) ' article to download

        dim article as Article = sr.GetArticle() ' download article
        dim rawContent as string = article.Content ' article stored in a variable as a string to manipulate

        dim prettyParagraphs(0) ' to store paragraphs
        dim endOfParagraph as boolean ' used to indicate the end of a paragraph
        dim currentParagraphNum as integer ' stores current paragraph number
        dim startParagraphAfter as Integer ' after this character number, start writing the paragraph
        dim ignoreChars = False ' ignore intra-paragraph tags containing things such as images
        dim shouldWriteParagraph as Boolean ' write the paragraph, unless it's empty
        Dim paragraphLength as Integer _
        ' used to determine whether the paragraph is empty, and thus shouldWriteParagraph status

        if article.IsReadable = True

            for i = 0 to rawContent.Length - 1 ' for all the characters in the raw article content

                currentParagraphNum = prettyParagraphs.Length - 1 _
                ' because prettyParagraphs starts at 0, the length would be 1 at 0 if you get me, so need to subtract 1

                if rawContent.Substring(i, 1) = "<" ' if the opening of a tag is detected...
                    if rawContent.Substring(i + 1, 2) = "p>" and not rawContent.Substring(i + 3, 1) = "<" _
' if the rest of the tag is p>, and there are no tags immediately inside...
                        endOfParagraph = False ' it is no longer the end of the paragraph, but the start of one!
                        startParagraphAfter = i + 2 _
                        'start paragraph after i+2, which is > in a <p> tag, considering i would be <
                    End If

                    If rawContent.Substring(i + 1, 3) = "/p>" ' if the closing of a tag is detected...
                        endOfParagraph = True ' it is the end of a paragraph
                        Array.Resize(prettyParagraphs, prettyParagraphs.Length + 1) _
                        ' increase the size of the array by 1 to accomodate another paragraph

                        if prettyParagraphs(currentParagraphNum) = "null" ' if null....
                            Array.Resize(prettyParagraphs, prettyParagraphs.Length - 1) _
                            ' decrease the length of the array by one; we don't need another one currently
                        End If

                        Try ' to determine whether a paragraph is empty, try to find its length
                            paragraphLength = prettyParagraphs(currentParagraphNum).ToString().Length
                        Catch ' if an exception throws, don't write the paragraph by changing the variable 
                            Array.Resize(prettyParagraphs, prettyParagraphs.Length - 1)
                        End Try

                    End If
                End If

                if rawcontent.Substring(i, 1) = "<" ' if, intra-paragraph, the start of a tag is detected...
                    ignoreChars = True ' ignore characters until the end of the tag
                End If

                if i > startParagraphAfter And endOfParagraph = False and ignoreChars = False _
' if the beginning of the tag has ended, and it's not the end of a paragraph, and it's not an intra <p> tag..
                    prettyParagraphs(currentParagraphNum) = prettyParagraphs(currentParagraphNum) &
                                                            rawContent.Substring(i, 1) _
                    ' add the next character to the current paragraph
                End If

                if rawContent.Substring(i, 1) = ">"
                    ignoreChars = False ' if the end of a tag is detected, stop ignoring characters
                End If
            Next

            Console.Clear()

            for i = 0 to prettyParagraphs.Length - 2

                shouldWriteParagraph = True
                if article.SiteName = "BBC News" and i = 0 ' BBC News edge case
                    shouldWriteParagraph = False
                End If

                Try ' to determine whether a paragraph is empty, try to find its length PROBABLY REDUNDANT 
                    paragraphLength = prettyParagraphs(i).ToString().Length
                Catch ' if an exception throws, don't write the paragraph by changing the variable 
                    shouldWriteParagraph = False
                End Try


                if shouldWriteParagraph = True ' if the paragraph isn't empty
                    prettyParagraphs(i) = prettyParagraphs(i).Replace("&amp;", "&")
                    prettyParagraphs(i) = prettyParagraphs(i).Replace("&lt;", "<") _
                    ' replace escape sequences with correct characters in each paragraph
                    prettyParagraphs(i) = prettyParagraphs(i).Replace("&gt;", ">")
                    prettyParagraphs(i) = prettyParagraphs(i).replace("&nbsp;", " ")

                    Console.Writeline(prettyParagraphs(i))
                    if not i = prettyParagraphs.Length - 2
                        Console.WriteLine() ' if it's not the end, line break
                    End If

                End If
            Next

        Else
            Console.WriteLine()
            console.clear
            While Console.KeyAvailable
                Console.ReadKey(True)
            End While

            console.WriteLine("Content paywalled or otherwise unaccessible. Press any key to go back to the home page.") _
            ' TODO MAKE THIS GO BACK TO THE PREVIOUS PAGE OF THE LIST WHEN APPROPRIATELY MOVED INTO ANOTHER FUNCTION 
            Thread.Sleep(1000)
            Console.ReadKey(True)

            Console.Clear()
            Console.WriteLine("Key pressed! Sending you back...")
            Thread.sleep(1000)

            Main() ' TODO SEE ABOVE
        End If

        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Yellow
        console.WriteLine("Press 1 to quit")
        console.Writeline("Press 2 to go to feed selection")
        console.Write("Press 3 to go to story selection")
        Console.ResetColor()
        
        dim key as ConsoleKeyInfo
        
        Do
            key = Console.ReadKey(True)
        Loop Until key.Key >= ConsoleKey.D1 AndAlso key.Key <= ConsoleKey.D3

        _keySelectstory = Nothing
        
        Select Case key.Key
            Case ConsoleKey.D1
                Environment.Exit(0)

            Case ConsoleKey.D2
                Console.WriteLine("check!")
                Main()

            Case ConsoleKey.D3
                printStories()
                _storySelected = selectStory()
                getContent(_titlesAndLinks(_storySelected, 1))
        End Select
    End sub
End Module