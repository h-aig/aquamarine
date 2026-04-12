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
    Sub Main()

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
            Console.write("Loading...")
            Using _
                reader as XmlReader =
                    XmlReader.Create("https://www.theverge.com/rss/partner/subscriber-only-full-feed/rss.xml") _
                ' ' Download data, should ask user
                synFeed = SyndicationFeed.Load(reader)
                Console.CursorLeft() = 0
            End Using

            Using writer as XmlWriter = XmlWriter.Create(sb)
                synFeed.SaveAsRss20(writer)
            End Using
            
        Catch
            console.clear
            While Console.KeyAvailable
                Console.ReadKey(True) ' flush the buffer so keys pressed during download don't skip into this quickly, confusing users
            End While
            
            console.WriteLine("This is an invalid URL, or is not an RSS Feed. Press any key to go back to the previous page.") 
            Threading.Thread.Sleep(1000)
            Console.ReadKey(True)
            
            Console.Clear()
            Console.WriteLine("Key pressed! Sending you back...")
            Threading.Thread.sleep(1000)
            
            Main()
        End Try

        xmlRSS.LoadXml(sb.ToString())

        dim item as XmlNode
        Dim nodeList as XmlNodeList ' TODO Abstract this, we're copying the same one from above come on
        Dim root as XmlNode = xmlRSS.DocumentElement
        Try
            nodeList = root.SelectNodes("//*[local-name()='entry' or local-name()='item']")
        catch
            console.clear
            While Console.KeyAvailable
                Console.ReadKey(True) 
            End While
            
            console.WriteLine("XML invalid! Press any key to go back to the previous page.") 
            Threading.Thread.Sleep(1000)
            Console.ReadKey(True)
            
            Console.Clear()
            Console.WriteLine("Key pressed! Sending you back...")
            Threading.Thread.sleep(1000)
            
            Main()
        End Try

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

                    If rawContent.Substring(i + 1, 3) = "/p>" ' if the closing of a tag is detected...
                        endOfParagraph = True ' it is the end of a paragraph
                        Array.Resize(prettyParagraphs, prettyParagraphs.Length + 1) _
                        ' increase the size of the array by 1 to accomodate another paragraph
                        
                        if prettyParagraphs(currentParagraphNum) = "null" ' if null....
                            Array.Resize(prettyParagraphs, prettyParagraphs.Length - 1) ' decrease the length of the array by one; we don't need another one currently
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

                Try ' to determine whether a paragraph is empty, try to find its length PROBABLY REDUNDANT 
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
            Console.WriteLine() 
            console.clear
            While Console.KeyAvailable
                Console.ReadKey(True) 
            End While
            
            console.WriteLine("Content paywalled or otherwise unaccessible. Press any key to go back to the home page.") ' TODO MAKE THIS GO BACK TO THE PREVIOUS PAGE OF THE LIST WHEN APPROPRIATELY MOVED INTO ANOTHER FUNCTION 
            Threading.Thread.Sleep(1000)
            Console.ReadKey(True)
            
            Console.Clear()
            Console.WriteLine("Key pressed! Sending you back...")
            Threading.Thread.sleep(1000)
            
            Main() ' TODO SEE ABOVE
        End If

        timetotype()
        
        
        dim numericUserCharInput as ConsoleKeyInfo
        Dim userCharInput as Char
        dim currentCorrectChar as Char
        dim userParagraphs(prettyParagraphs.Length - 2)
                
        for i as integer = 0 to userParagraphs.Length - 1
            userParagraphs(i) = ""    
        Next
        
        dim totalCharacters as integer = 0
        
' TODO ADD A FOR LOOP FOR EACH PARAGRAPH, THEN THE THING BELOW WILL BE SET TO I, MAYBE AUTOSKIP PARAGRAPHS CAUSE IM LAZY
        ' TODO HANDLE THE ENTER KEY BEING PRESSED
        
        do until (userParagraphs(0) = prettyParagraphs(0))
            
            numericUserCharInput = console.ReadKey(True)
            userCharInput = numericUserCharInput.KeyChar ' TODO COMMENT THIS, AND ALSO WITH THIS I CAN EXCLUDE CHARACTERS MONKEYTYPE DOESN'T REGISTER LIKE ARROW KEYS
            currentCorrectChar = prettyParagraphs(0).ToString().Substring(totalCharacters, 1) ' todo turn this into a function
            
            if  correctAlphaNumberic(userCharInput, currentCorrectChar) = true and isSpace(numericUserCharInput.key) = false ' if the character the user typed is correct, and not a backspace...
                PrintCorrectChar(currentCorrectChar)
                userParagraphs(0) = userParagraphs(0) + userCharInput ' add character to the current paragraph the user has writter 
                totalCharacters = totalCharacters + 1
            End If    
            
            if correctAlphaNumberic(userCharInput, currentCorrectChar) = false  and isSpace(numericUserCharInput.Key) = false and  correctNonStandard(userCharInput, currentCorrectChar) = true  ' TODO REALLY SHOULD HAVE A LIST THIS IF CAN JUST REFERENCE OF EDGE CASES AND NONSTANDARD CHARACTERS
                PrintCorrectChar(currentCorrectChar)
                userParagraphs(0) = userParagraphs(0) + currentcorrectchar
                totalCharacters = totalCharacters + 1
                Else 
                    Console.ForegroundColor = consolecolor.red
                    Console.Write(currentCorrectChar) ' todo THIS ELSE BREAKS THINGS-- CREATE FUNCTIONS CORRECTCHAR() AND INCORRECTCHAR() TO HOLD BOTH OUTCOMES
                
                    userParagraphs(0) = userParagraphs(0) + userCharInput
                    totalCharacters = totalCharacters + 1
            End If
            ' ^ only if the user writes ' which parses weird, need to make a list of these edge cases that may not translate
                
            if not userCharInput = currentCorrectChar and not numericUserCharInput.Key = 8  and charNonStandard(userCharInput) = False ' TODO REALLY SHOULD HAVE A LIST THIS IF CAN JUST REFERENCE OF EDGE CASES
                PrintIncorrectChar(currentCorrectChar)
                userParagraphs(0) = userParagraphs(0) + userCharInput
                totalCharacters = totalCharacters + 1
            End If
            
            if not userCharInput = currentCorrectChar and not numericUserCharInput.Key = 8 and currentCorrectChar = " "
                Console.CursorLeft = console.CursorLeft - 1 ' no idea why I need this if the current correct character is a whitespace but oh well
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write(userCharInput)
                
                userParagraphs(0) = userParagraphs(0) + currentCorrectChar ' TODO WATCH THIS
            End If
            
            if numericUserCharInput.Key = 8 and not Console.cursorleft = 0 ' handle backspaces
                
                Console.ForegroundColor = ConsoleColor.DarkGray
                Console.CursorLeft = console.CursorLeft - 1
                
                userParagraphs(0) = userParagraphs(0).ToString().Substring(0, userParagraphs(0).ToString().Length - 1)   ' TODO WATCH THIS
                totalCharacters = totalCharacters - 1
                
                currentCorrectChar = prettyParagraphs(0).ToString().Substring(totalcharacters, 1)
                Console.Write(currentCorrectChar)
                Console.CursorLeft = console.CursorLeft - 1
                
            End If
            ' TODO HANDLE BACKSPACES, THIS WILL LEAD TO A DELETION FROM THE ARRAY AND A RESTORATION OF THE PREVIOUS COLOR AND CHARACTER, AND DECREASE CHARACTERSENTERED BY 1,
            ' TODO ENTER KEY NEEDS PATCHING THIS IS A MAD EDGE CASE
            ' TODO NEED A WAY TO GO BACK FROM AN ARTICLE IF CURSORLEFT = 0? OR WITH A MODIFER + CHARACTER?
            ' TODO SPECIAL CHARACTER HANDLING IS BORKED AND I NEED A WAY TO GO BACK TO THE PREVIOUS LINE AS WELL
            ' TODO A LOT OF WEIRD FEEDS LIKE https://rss-generator.toromonja.com/f/55f797e4-5492-4de4-80c9-66914fc7fe5c DON'T WORK EVEN THOUGH FEED NAMES ARE FETCHED
            ' TODO I NEED TO ELIMINATE ANY NULL PARAGRAPHS BEFORE MAKING USERPARAGRAPHS EQUAL TO IT OR JUST CHECK WHETHER IT'S THE LAST USERPARAGRAPH BECAUSE THE LAST 2 ARE ALWAYS EMPTY
            ' TODO LITERAL EDGE CASE IF ON THE BOUNDARY IT TWEAKS OUT, CHARACTERS DON'T REGISTER WELL IF INCORRECT AND THEN YOU BACKSPACE, I THINK IT'S SPACES THAT BORK IT
            ' TODO NEED AN OR LENGTH OF USERXYZ IS EQUAL OR HIGHER THAN PRETTYPARAGRAPHS
            ' TODO IT BREAKS IF YOU BACKSPACE AT THE END ALSO
            ' TODO HANDLE NULL CASES IN THE EVENT OF KEYS SUCH AS ARROW KEYS BEING PRESSED 
        loop
        
        Console.Clear()
        Console.Writeline("you win!")
        
    End Function

    Function timetotype()
        Console.SetCursorPosition(0, 0)
        Console.ForegroundColor = ConsoleColor.White
    End Function
    
    function charNonStandard(inputChar)
        if (inputChar = "'" or inputChar = Chr(34)) ' if nonstandard character return true
            return true
        End If
        return false
    End function
    
    function correctAlphaNumberic(userChar, correctChar)
        if userChar = correctChar ' if key is correct and not a space
            return true
        End If
        return false
    End function
    
    function isSpace(key)
        if key = 8
            return true
        End If
        return false
    End function
    
    function CorrectNonStandard(userChar, correctChar)
        if (userChar = "'" or userChar = Chr(34))
            if (correctChar = "‘" or correctChar = "’") and userChar = "'"
                Return true
            End If
            ElseIf (correctChar = ""“"" or correctChar = ""”"") and userChar = chr(34)
                return true
        End If
          return false  
    End function
    
    private sub PrintCorrectChar(correctChar)
        Console.ForegroundColor = consolecolor.White
        console.Write(correctChar)
        return
    end sub
    
    private sub PrintIncorrectChar(correctChar)
        Console.ForegroundColor = consolecolor.red
        console.Write(correctChar)
        return
    end sub
    
End Module
