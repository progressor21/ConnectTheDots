using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectTheDots
{
    class GameLogic
    {
        //Global class variables to maintain the state of the game
        private static bool gameOver;
        private static bool currentPlayerIsOne;

        private static Board board = new Board();
        private static List<Point> validStartNodes = new List<Point>();
        private static List<Line> lines = new List<Line>();

        //startNode variable is used to maintain state of a selected start node that is awaiting for an end node selection by a player.
        //It's initialized with (x = -1 and y = 0) - a state where is no a valid start node currently selected - the point is off the board

        private static Point startNode = new Point(-1, 0);

        public static MsgResponsePayload HandleRequestMessage(MsgRequestPayload request)
        {
            MsgResponsePayload response = new MsgResponsePayload();
            response.id = request.id;
            response.body = new MsgResponsePayload.Body();

            if (request.msg.ToUpper() == "ERROR")
            {
                //Logging Client Error message
                //request.body.ToString();

                response.id = 0;
                response.msg = "UPDATE_TEXT";
                response.body.newLine = null;
                response.body.heading = "!!! ERROR !!!";
                response.body.message = request.body.ToString() + Environment.NewLine + "Please, refresh this page to re-start the game.";
            }

            if (request.msg.ToUpper() == "INITIALIZE")
            {
                validStartNodes.Clear();
                lines.Clear();
                currentPlayerIsOne = true;
                gameOver = false;
                startNode.x = -1;

                response.msg = "INITIALIZE";
                response.body.heading = "Player 1";
                response.body.message = "Awaiting Player 1's move.";
            }

            if (request.msg.ToUpper() == "NODE_CLICKED")
            {
                //GAME IS OVER and additional node clicks are invalid - refresh the page to start a new game
                if (gameOver == true)
                {
                    response.msg = "INVALID_START_NODE";
                    response.body.heading = "Game Over";
                    response.body.message = currentPlayer(currentPlayerIsOne) + " Wins!" + Environment.NewLine + "Please, refresh this page to start a new game.";
                }
                //START_NODE clicked 
                else if (validStartNodes.Count == 0)
                {
                    //START_NODE
                    if (startNode.x == -1)
                    {
                        startNode.x = request.body.x;
                        startNode.y = request.body.y;

                        response.msg = "VALID_START_NODE";
                        response.body.heading = currentPlayer(currentPlayerIsOne);
                        response.body.message = "Select a second node to complete the line.";
                    }
                    else //END_NODE
                    {
                        Point endNode = new Point(request.body.x, request.body.y);
                        bool isOctilinearLine = GameRules.IsOctilinearLine(startNode, endNode);

                        //VALID_END_NODE
                        if (isOctilinearLine)
                        {
                            Point firstStartNode = new Point(startNode.x, startNode.y);
                            Line newLine = new Line(firstStartNode, endNode);
                            lines.Add(newLine);
                            validStartNodes.Add(firstStartNode);
                            validStartNodes.Add(endNode);
                            currentPlayerIsOne = !currentPlayerIsOne;

                            response.body.heading = currentPlayer(currentPlayerIsOne);

                            response.body.newLine = new MsgResponsePayload.Body.NewLine();
                            response.body.newLine.start = new MsgResponsePayload.Body.NewLine.Start();
                            response.body.newLine.end = new MsgResponsePayload.Body.NewLine.End();

                            response.msg = "VALID_END_NODE";
                            response.body.newLine.start.x = startNode.x;
                            response.body.newLine.start.y = startNode.y;
                            response.body.newLine.end.x = endNode.x;
                            response.body.newLine.end.y = endNode.y;

                            response.body.message = "Awaiting " + currentPlayer(currentPlayerIsOne) + " 's move.";

                            startNode.x = -1;
                        }
                        //INVALID_END_NODE
                        else
                        {
                            response.msg = "INVALID_END_NODE";
                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Invalid move!";

                            startNode.x = -1;
                        }
                    }
                }
                else //All other nodes' clicks after the first move
                {
                    //START_NODE
                    if (startNode.x == -1)
                    {
                        startNode.x = request.body.x;
                        startNode.y = request.body.y;

                        //VALID_START_NODE
                        if ((startNode.x == validStartNodes[0].x && startNode.y == validStartNodes[0].y) || (startNode.x == validStartNodes[1].x && startNode.y == validStartNodes[1].y))
                        {
                            response.msg = "VALID_START_NODE";

                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Select a second node to complete the line.";
                        }
                        else //INVALID_START_NODE
                        {
                            response.msg = "INVALID_START_NODE";

                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Not a valid starting position.";

                            startNode.x = -1;
                        }
                    }
                    else //END_NODE
                    {
                        Point endNode = new Point(request.body.x, request.body.y);
                        Line tryNewLine = new Line(startNode, endNode);

                        bool isOctilinearLine = GameRules.IsOctilinearLine(startNode, endNode);
                        bool noInvalidIntersect = GameRules.CheckLinesDoNotIntersect(tryNewLine, lines);
                        //bool noInvalidIntersect = GameRules.NoInvalidIntersect(tryNewLine, lines);

                        //VALID_END_NODE
                        if (isOctilinearLine && noInvalidIntersect)
                        {
                            Point currentStartNode = new Point(startNode.x, startNode.y);
                            Line newLine = new Line(currentStartNode, endNode);
                            lines.Add(newLine);
                            for (int i = 1; i > -1; i--)
                            {
                                if (validStartNodes[i].x == currentStartNode.x && validStartNodes[i].y == currentStartNode.y)
                                {
                                    validStartNodes.RemoveAt(i);
                                }
                            }
                            
                            validStartNodes.Add(endNode);
                            currentPlayerIsOne = !currentPlayerIsOne;

                            response.body.newLine = new MsgResponsePayload.Body.NewLine();
                            response.body.newLine.start = new MsgResponsePayload.Body.NewLine.Start();
                            response.body.newLine.end = new MsgResponsePayload.Body.NewLine.End();

                            //Check if there is still a valid move left on the game board or it is complete
                            bool boardComplete = GameRules.BoardIsComplete(board, lines, validStartNodes);
                            //bool boardComplete = GameRules.BoardComplete(board, lines, validStartNodes);

                            if (!boardComplete)
                            {
                                response.msg = "VALID_END_NODE";
                                response.body.heading = currentPlayer(currentPlayerIsOne);

                                response.body.newLine.start.x = startNode.x;
                                response.body.newLine.start.y = startNode.y;
                                response.body.newLine.end.x = endNode.x;
                                response.body.newLine.end.y = endNode.y;
                                response.body.message = "Awaiting " + currentPlayer(currentPlayerIsOne) + " 's move.";

                                startNode.x = -1;
                            }
                            else //GAME IS OVER if the board no longer has any valid moves left
                            {
                                gameOver = true;

                                response.msg = "GAME_OVER";
                                response.body.heading = "Game Over";

                                response.body.newLine.start.x = startNode.x;
                                response.body.newLine.start.y = startNode.y;
                                response.body.newLine.end.x = endNode.x;
                                response.body.newLine.end.y = endNode.y;
                                response.body.message = currentPlayer(currentPlayerIsOne) + " Wins!";
                            }
                        }
                        else // INVALID_END_NODE
                        {
                            response.msg = "INVALID_END_NODE";

                            response.body.heading = currentPlayer(currentPlayerIsOne);
                            response.body.message = "Invalid move!";

                            startNode.x = -1;
                        }
                    }
                }
            }

            return response;
        }

        private static string currentPlayer(bool currentPlayerIsOne)
        {
            return (currentPlayerIsOne) ? "Player 1" : "Player 2";
        }

    }
}
