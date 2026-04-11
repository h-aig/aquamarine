' (thanks dotnetperls! https://www.dotnetperls.com/webclient-vbnet)
' TODO this is a zenful typing test! the point is to consume the information! no time limits here.
' TODO the game's gone if the article is so large it pushes the scrollbar down, how to rectify?
Imports System
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.Mime
Imports System.Runtime.Intrinsics.X86
Imports System.Xml
Imports AngleSharp.Html.Dom
Imports System.ServiceModel.Syndication
Imports System.Text

Module Program
    Sub Main(args As String())

        Console.Clear() ' get rid of the accursed yellow boot text in rider-- no effect in prod

        console.WriteLine("AQUAMARINE 1.0.0-ALPHA")
        Console.WriteLine("PLEASE RUN IN FULL SCREEN TO PREVENT ERRORS")
        Console.WriteLine()
        Console.WriteLine("PRESS ANY KEY TO CONTINUE")
        Console.ReadKey()

        Console.Clear()

        Console.ForegroundColor = ConsoleColor.White
        dim rssTitleList(0) as String ' stores list of all rss posts
        dim rssLinkList(0) as String ' stores list of all rss links
        dim client as new WebClient ' client
        Dim rawRSS As string ' this stores the downloaded string
        dim xmlRSS as new xmldocument() ' this holds the downloaded string as an xmldocument
        dim synFeed as SyndicationFeed
        dim sb as new StringBuilder()
        client.Headers("User-Agent") = "Mozilla/4.0" ' Set one of the headers.   

        Console.WriteLine("Aquamarine. A new way to consume information.")
        Console.WriteLine()

        Try
            Using _
                reader as XmlReader =
                    XmlReader.Create("https://www.theverge.com/rss/partner/subscriber-only-full-feed/rss.xml") _
                ' ' Download data, should ask user
                synFeed = SyndicationFeed.Load(reader)
            End Using

            Using writer as XmlWriter = XmlWriter.Create(sb)
                synFeed.SaveAsRss20(writer)
            End Using
        Catch
            rawRSS = "There was an exception!"  ' TODO EXPAND ON THIS: WHAT TO DO IF THE URL IS INCORRECT?
        End Try

        xmlRSS.LoadXml(sb.ToString())

        dim item as XmlNode
        Dim nodeList as XmlNodeList ' TODO i need a try here-- if the xml is invalid
        Dim root as XmlNode = xmlRSS.DocumentElement
        nodeList = root.SelectNodes("//*[local-name()='entry' or local-name()='item']")

        for each item in nodeList ' for each post in the feed...
            dim title as XmlNode = item.SelectSingleNode("title") ' find title
            dim link as XmlNode = item.SelectSingleNode("link") ' find link
            rssTitleList(rssTitleList.Length - 1) = title.InnerText ' place post in list
            rssLinkList(rssLinkList.Length - 1) = link.InnerText ' 
            Array.Resize(rssTitleList, rssTitleList.Length + 1) ' increase size of array by 1
            Array.Resize(rssLinkList, rssLinkList.Length + 1)
        next
        Array.resize(rssTitleList, rssTitleList.length - 1) ' get rid of empty string at the end
        Array.resize(rssLinkList, rssLinkList.length - 1)

        Array.resize(rssTitleList, 20) ' get rid of empty string at the end
        Array.resize(rssLinkList, 20)

        for i as integer = 0 to rssTitleList.Length - 1
            console.WriteLine(rssTitleList(i))
        Next

        Console.SetCursorPosition(0, 2)
        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.Black
        Console.Write(rssTitleList(0))

        While True
            Dim key As ConsoleKeyInfo = Console.ReadKey(True) ' True = don't display inputted characters
            Select Case key.Key _
                ' Merge all keys into one Case -- https://chatgpt.com/s/t_69ac323c5adc8191ac526fa15f7b7065
                Case ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter
                    ' Handle each key individually
                    If key.Key = ConsoleKey.UpArrow Then
                        if not Console.CursorTop = 2
                            console.cursorleft = 0
                            Console.BackgroundColor = ConsoleColor.Black
                            Console.ForegroundColor = ConsoleColor.white
                            Console.Write(rssTitleList(Console.CursorTop - 2))
                            console.SetCursorPosition(0, console.CursorTop - 1)
                            Console.BackgroundColor = ConsoleColor.White
                            Console.ForegroundColor = ConsoleColor.Black
                            Console.Write(rssTitleList(Console.CursorTop - 2))
                        End If
                    ElseIf key.Key = ConsoleKey.DownArrow Then
                        if not Console.CursorTop = rssTitleList.Length + 1 _
                            'array starts at 0, so it's +1 and not +2 in terms of boundary checking
                            console.CursorLeft = 0
                            Console.BackgroundColor = ConsoleColor.black
                            Console.ForegroundColor = ConsoleColor.white
                            Console.Write(rssTitleList(Console.CursorTop - 2))
                            console.SetCursorPosition(0, console.CursorTop + 1)
                            Console.BackgroundColor = ConsoleColor.White
                            Console.ForegroundColor = ConsoleColor.Black
                            Console.Write(rssTitleList(Console.CursorTop - 2))
                        end if
                    ElseIf key.Key = ConsoleKey.Enter Then
                        getContent(rssLinkList(Console.CursorTop - 2))
                        Exit While
                    End If
            End Select
        End While

        Console.ReadLine()
    End Sub

    Function getContent(link)
        Console.BackgroundColor = ConsoleColor.black
        Console.ForegroundColor = ConsoleColor.white
        Console.Clear()
        Console.WriteLine("Loading...")

        dim sr as SmartReader.Reader = new smartreader.Reader(link) ' article to download

        dim article as SmartReader.article = sr.GetArticle() ' download article
        dim rawContent as string = article.Content ' article stored in a variable as a string to manipulate

        dim prettyParagraphs(0) ' to store paragraphs
        dim endOfParagraph as boolean ' used to indicate the end of a paragraph
        dim currentParagraphNum as integer ' stores current paragraph number
        dim startParagraphAfter as Integer ' after this character number, start writing the paragraph
        dim ignoreChars as boolean = False ' ignore intra-paragraph tags containing things such as images
        dim shouldWriteParagraph as Boolean ' write the paragraph, unless it's empty
        Dim paragraphLength as Integer _
        ' used to determine whether the paragraph is empty, and thus shouldWriteParagraph status

        if article.IsReadable = True

            for i as integer = 0 to rawContent.Length - 1 ' for all the characters in the raw article content

                currentParagraphNum = prettyParagraphs.Length - 1 _
                ' because prettyParagraphs starts at 0, the length would be 1 at 0 if you get me, so need to subtract 1

                if rawContent.Substring(i, 1) = "<" ' if the opening of a tag is detected...
                    if rawContent.Substring(i + 1, 2) = "p>" and not rawContent.Substring(i + 3, 1) = "<" _
                        ' if the rest of the tag is p>, and there are no tags immediately inside...
                        endOfParagraph = False ' it is no longer the end of the paragraph, but the start of one!
                        startParagraphAfter = i + 2 _
                        'start paragraph after i+2, which is > in a <p> tag, considering i would be <
                    End If

                    If rawContent.Substring(i + 1, 3) = "/p>" ' if the closing of a tag is detected... TODO COMMENT THIS; the checks for null and whitespace within this if block
                        endOfParagraph = True ' it is the end of a paragraph
                        Array.Resize(prettyParagraphs, prettyParagraphs.Length + 1) _
                        ' increase the size of the array by 1 to accomodate another paragraph
                        
                        if prettyParagraphs(currentParagraphNum) = "null"
                            Array.Resize(prettyParagraphs, prettyParagraphs.Length - 1)
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

            for i as integer = 0 to prettyParagraphs.Length - 2

                shouldWriteParagraph = True
                if article.SiteName = "BBC News" and i = 0 ' BBC News edge case
                    shouldWriteParagraph = False
                End If

                Try ' to determine whether a paragraph is empty, try to find its length TODO uh probably redundant? see line 160 onwards
                    paragraphLength = prettyParagraphs(i).ToString().Length
                Catch ' if an exception throws, don't write the paragraph by changing the variable 
                    shouldWriteParagraph = False
                End Try


                if shouldWriteParagraph = True ' if the paragraph isn't empty
                    Console.ForegroundColor = ConsoleColor.DarkGray
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
            Console.WriteLine("Content paywalled or otherwise unaccessible.") _
            ' < if content isn't readable TODO so what does the user do now? do we return them? where to?
            Threading.Thread.sleep(1000)
            Environment.Exit(0)
        End If

        timetotype()
        
        
        dim numericUserCharInput as ConsoleKeyInfo
        Dim userCharInput as Char
        dim currentCorrectChar as Char
        dim userParagraphs(prettyParagraphs.Length - 1)
        dim totalCharacters as integer = 0 
        
' TODO ADD A FOR LOOP FOR EACH PARAGRAPH, THEN THE THING BELOW WILL BE SET TO I, MAYBE AUTOSKIP PARAGRAPHS CAUSE IM LAZY
        ' TODO HANDLE THE ENTER KEY BEING PRESSED
        
        do until (userParagraphs(0) = prettyParagraphs(0))
            
            numericUserCharInput = console.ReadKey(True)
            userCharInput = numericUserCharInput.KeyChar ' TODO COMMENT THIS, AND ALSO WITH THIS I CAN EXCLUDE CHARACTERS MONKEYTYPE DOESN'T REGISTER LIKE ARROW KEYS
            currentCorrectChar = prettyParagraphs(0).ToString().Substring(console.CursorLeft, 1) ' todo THIS DON'T WORK WHEN WE'RE ON A NEW LINE 
            
            
            if userCharInput = currentCorrectChar and not numericUserCharInput.Key = 8
                Console.ForegroundColor = consolecolor.White
                console.Write(userCharInput)
                userParagraphs(0) = userParagraphs(0) + userCharInput ' TODO WATCH THIS
                
            Else if not userCharInput = currentCorrectChar and not numericUserCharInput.Key = 8
                Console.ForegroundColor = consolecolor.red
                Console.Write(currentCorrectChar)
                
                if currentCorrectChar = " "
                    Console.CursorLeft = console.CursorLeft - 1 ' no idea why I need this if the current correct character is a whitespace but oh well
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.Write(userCharInput)
                End If
                
            End If
            
            if numericUserCharInput.Key = 8 and not Console.cursorleft = 0 ' handle backspaces 
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.CursorLeft = console.CursorLeft - 1
                
                currentCorrectChar = prettyParagraphs(0).ToString().Substring(console.CursorLeft, 1)
                Console.Write(currentCorrectChar)

                Console.CursorLeft = console.CursorLeft - 1
            End If
            
            ' TODO HANDLE BACKSPACES, THIS WILL LEAD TO A DELETION FROM THE ARRAY AND A RESTORATION OF THE PREVIOUS COLOR AND CHARACTER, AND DECREASE CHARACTERSENTERED BY 1,
        loop
        
        Console.Clear()
        Console.Writeline("you win!")
        
    End Function

    Function timetotype()
        Console.SetCursorPosition(0, 0)
        Console.ForegroundColor = ConsoleColor.White
    End Function
End Module
