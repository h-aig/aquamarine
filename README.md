[Watch demo video](https://files.catbox.moe/8qb4tl.mp4)

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

<img width="3670" height="1916" alt="image" src="https://github.com/user-attachments/assets/9f89d759-3988-4e00-a55c-e6704ee13dbc" />
<img width="3670" height="1916" alt="image" src="https://github.com/user-attachments/assets/216452ca-3e8b-470b-b1e6-191289bdd29c" />
<img width="3670" height="1916" alt="image" src="https://github.com/user-attachments/assets/bd26cf4b-f937-4d5c-9ab2-94eef446daf6" />
