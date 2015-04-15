#Selective Folder Copier

After resigning from my previous work, I realised that I would like to keep a copy of all my web-related work for them, and as such needed an application that could selectively copy the web root of the server I used to maintain.

By 'selectively copy' I mean that the application needed to ignore certain file types as I was only really interested in keeping the scripted pages, databases and photoshop files.

Selective Folder Copier allows you to set which files it is to ignore by editing the associated config.sav file, delimiting the inputted file types with ;

So your config.sav file should look something like this: 

FilesToIgnore=.exe;.doc;.ppt

The application then allows you to set the source and destination folder, before allowing you to run the actual copy function. 

Created by Craig Lotter, October 2008

*********************************

Project Details:

Coded in Visual Basic .NET using Visual Studio .NET 2008
Implements concepts such as File Manipulation, Folder Manipulation, Recursion.
Level of Complexity: Very Simple

*********************************

Update 20081031.02:

- Minor Display Bug Fix
