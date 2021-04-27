# ConnectTheDots Game
A connect-the-dots game for two players designed by Sid Sackson

## Introduction
This ConnectTheDots Game project is implemented using C#.Net v.4.7.2 to create a game server (as a WebSocketServer with server-side game logic that implementing Game Rules, below). 
I was provided with a web client (implemented using HTML/CSS and JavaScript) to render the game and manage user interactions. 
The client is dumb and knows nothing about the game. My responsibility was to implement the game logic and maintain the game state on the server side.
This program uses SuperWebSocket Server (by SuperWebSocket) v0.9.0.2 - a .NET implemention of the WebSocket API. 

## Running the Application
The application requires a recent version of .NET Framework (4.7.2 or later) to run. 
Free to download here: https://dotnet.microsoft.com/download/dotnet-framework

With .NET Framework downloaded on the computer, follow these instructions to run the program:
1. Download files ConnectTheDots.zip from the /bin/ folder and client.zip from the /client/ folder.
2. Extract all files from ConnectTheDots.zip and client.zip files into respective folders on your local drive.
3. Go to the folder /ConnectTheDots/ and double-click on the ConnectTheDots.exe to run the application
4. The cmd console should open and read "Web Server is running (port:8081). Waiting for a client connection to start a new session... Press any key to stop the game server and exit."
5. Find index.html file in the local /client/ folder, right click on it and open with any browser of your choice. The game will start after index.html loads in the web browser and in the console window you'll see "New Session connected: ...".
6. To start/initalize a new game at any time, reload the browser page.
7. To exit the game and stop the game server you can just click any key (like Enter) when you in the cmd console window or close a browser's tab with index.html page - the cmd console will read: "Session closed: ClientClosing", then press any key when focused on the cmd console window to close the session and stop the server.

## Game Rules
* The game is played on a 4x4 grid of 16 nodes.
* Players take turns drawing octilinear lines connecting nodes.
* A line may connect any number of nodes.
* The first line may begin on any node.
* Each following line must begin at the start or end of the existing path, so that all lines form a continuous path.
* Lines may not intersect.
* No node can be visited twice.
* Each move is numbered. Player 1 made the odd numbered moves and Player 2 made the even numbered moves. 
* The game ends when no valid lines can be drawn.
* The player who draws the last line is the loser.
* The current player is displayed on the top of the game's page: Player 1 or Player 2.
* Useful information to the curent player/user is displayed on the bottom of the page below the game's grid.

## Attribution
* The game was designed by Sid Sackson.
* This program uses the following NuGET packages: 
  - SuperWebSocket v0.9.0.2 (by SuperWebSocket), SuperSocket (by Kerry Jiang) - to create a web server and to manage message communications between the web client and server.
  - Newtonsoft.Json v13.0.1 (by James Newton-King) - a popular high-performance JSON framework for .NET is used to serialize and deserialize JSON messages from and to the client.
  - log4net v2.0.3 (by Apache Software Foundation) - a tool to help the programmer output log statements to a variety of output targets (not fully used in this implementation).

* External code and algorithms used: 
  I used some code and algorithms implementations from the following articles to implement the methods in the LinesIntersectGeometry.cs class file. 
  For more implementation details see comments in that file.
  How to check if two line segments intersect or are drawn not in the same direction from the same point - line geometry theory and algorithm(s):
  https://algorithmtutor.com/Computational-Geometry/Check-if-two-line-segment-intersect/
  https://www.tutorialspoint.com/Check-if-two-line-segments-intersect
  https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/
  Used this article to find a slope of a line: https://www.geeksforgeeks.org/program-find-slope-line/

  Where do two line segments intersect? FindLinesIntersection() method implementation:
  https://martin-thoma.com/how-to-check-if-two-line-segments-intersect/
  https://rosettacode.org/wiki/Find_the_intersection_of_two_lines
  
