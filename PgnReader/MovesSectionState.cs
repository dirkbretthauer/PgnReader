#region Header
////////////////////////////////////////////////////////////////////////////// 
//The MIT License (MIT)

//Copyright (c) 2013 Dirk Bretthauer

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
namespace CChessCore.Pgn
{
    internal class MovesSectionState : PgnParserState
    {
        private int _skipCharsTilEofGame;

        public MovesSectionState(PgnParserStatemachine reader)
            : base(reader)
        {
        }

        public override void OnEnter(PgnMove currentMove)
        {
            _currentMove = currentMove;
        }

        public override void OnExit()
        {
            CheckToFinishMove();
        }

        protected override PgnParseResult DoParse(char current, char next, PgnGame currentGame)
        {
            if(_skipCharsTilEofGame > 0)
            {
                --_skipCharsTilEofGame;
                return _skipCharsTilEofGame == 0 ? PgnParseResult.EndOfGame : PgnParseResult.None;
            }

            if(current == '1' && next == '/')
            {
                Terminate("1/2-1/2" + "", currentGame);

                _skipCharsTilEofGame = 6;

                return PgnParseResult.None;
            }
            else if(current == '1' && next == '-')
            {
                Terminate("1-0", currentGame);

                _skipCharsTilEofGame = 2;

                return PgnParseResult.None;
            }
            else if(current == '0' && next == '-')
            {
                Terminate("0-1", currentGame);

                _skipCharsTilEofGame = 2;

                return PgnParseResult.None;
            }
            else if(char.IsLetterOrDigit(current))
            {
                if(!string.IsNullOrWhiteSpace(_currentMove.Move))
                {
                    currentGame.AddMove(_currentMove);
                    _stateBuffer.Clear();

                    _currentMove = new PgnMove();
                }

                _stateBuffer.Add(current);
            }
            else if(current == PgnToken.Period.Token && next != PgnToken.Period.Token)
            {
                _stateBuffer.Clear();
            }
            else if(current == PgnToken.Asterisk.Token)
            {
                Terminate(current + "", currentGame);

                return PgnParseResult.EndOfGame;
            }
            else if (current == PgnToken.TagBegin.Token || current == '\0')
            {
                string gameResult = string.Empty;
                string termination = new string(_stateBuffer.ToArray()).Trim();
                foreach(var result in PgnReader.Results)
                {
                    if(termination.EndsWith(result))
                    {
                        gameResult = result;
                    }
                }

                if(string.IsNullOrEmpty(gameResult))
                {
                    CheckToFinishMove();
                    Terminate("*", currentGame);
                }
                else
                {
                    Terminate(gameResult, currentGame);
                }
                
                return PgnParseResult.EndOfGame;
            }
            else if(char.IsWhiteSpace(current) && char.IsWhiteSpace(next))
            {
                //remove double whitespaces
            }
            else if(char.IsWhiteSpace(current))
            {
                CheckToFinishMove();
                
                if(next == PgnToken.TagBegin.Token)
                {
                    if(!string.IsNullOrWhiteSpace(_currentMove.Move))
                    {
                        currentGame.AddMove(_currentMove);
                    }
                    return PgnParseResult.EndOfGame;
                }
            }
            else if (current == '\r')
            {
                //remove linebreaks
            }
            else if(current == '-' ||
                    current == '+' ||
                    current == '#')
            {
                _stateBuffer.Add(current);
            }
            else
            {
                throw new PgnReaderException("MovesSectionState: no idea how to handle: " + current);
            }

            return PgnParseResult.None;
        }

        private void CheckToFinishMove()
        {
            if(_stateBuffer.Count > 0)
            {
                _currentMove.Move = new string(_stateBuffer.ToArray());
                _stateBuffer.Clear();
            }
        }

        private void Terminate(string result, PgnGame currentGame)
        {
            CheckToFinishMove();

            if(!string.IsNullOrWhiteSpace(_currentMove.Move))
            {
                currentGame.AddMove(_currentMove);
            }

            currentGame.Result = result;

            _stateBuffer.Clear();
            _currentMove = new PgnMove();

        }

        internal void InitGame(PgnGame game)
        {
            _stateBuffer.Clear();
            _currentMove = new PgnMove();
        }
    }   
}