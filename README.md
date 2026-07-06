(this was meant to be a terminal rss reader crossed with monkeytype; the code got unwieldly, and so i decided to rewrite it, only to realise I don't want to touch it with a 10 foot pole, so now it's just an rss reader in the terminal (minimum viable product))

# READ RSS FEEDS IN THE TERMINAL!
- Torture yourself! You must upload the feed url you want to read from every time!
- What a stupid suggestion! We don't take OPML files!
- Feeds do not persist! We don't save them anywhere! Very good for privacy!

We do everything else though (read; you can read your feeds, but we don't do images!)

# Dependencies
[.NET 9.0 Runtime, that's all!](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

# Installation
We (I) have prebuilt binaries. Knock yourself out.

**Linux only**: After installing the prebuilt binary, go to the directory it was saved to. Run
```
chmod +x linux-aquamarine
```
and then go ahead and run it with

```
./linux-aquamarine
```

# Motivation
I like RSS! It allows us to control what we consume, and we need it everywhere! Including the terminal!

# Tech Stack
It's just a console app!

# How it works
Literally just some http queries, traversing the feed's xml document to extract relevant nodes and print them on the screen! 
