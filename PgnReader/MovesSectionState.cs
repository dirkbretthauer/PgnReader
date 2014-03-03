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
        protected List<char> _singleMoveBuffer;

        public MovesSectionState(PgnParserStatemachine reader)
            : base(reader, 1024)
        {
            _singleMoveBuffer = new List<char>(255);
        }

        public override void OnExit()
        {
            var temp = GetStateBuffer().Trim();
            if(!string.IsNullOrWhiteSpace(temp))
            {
                _currentMove.Move = temp;
            }
        }

        public override PgnParseResult Parse(char current, char next, PgnGame currentGame)
        {
            if(current == '1' && next == '/')
            {
                Terminate("1/2-1/2" + "", currentGame);

                return PgnParseResult.EndOfGame;
            }
            else if(current == '1' && next == '-')
            {
                Terminate("1-2", currentGame);

                return PgnParseResult.EndOfGame;
            }
            else if(current == '0' && next == '-')
            {
                Terminate("0-1", currentGame);

                return PgnParseResult.EndOfGame;
            }
            else if(char.IsLetterOrDigit(current))
            {
                if(!string.IsNullOrWhiteSpace(_currentMove.Move))
                {
                    currentGame.AddMove(_currentMove);
                    _singleMoveBuffer.Clear();

                    _currentMove = new PgnMove();
                    
                }

                _stateBuffer.Add(current);
                _singleMoveBuffer.Add(current);
            }
            else if(current == PgnToken.Period.Token && next != PgnToken.Period.Token)
            {
                _singleMoveBuffer.Clear();
                _stateBuffer.Add(current);
            }
            else if(current == PgnToken.Asterisk.Token)
            {
                Terminate(current + "", currentGame);

                return PgnParseResult.EndOfGame;
            }
            else if (current == PgnToken.TagBegin.Token || current == '\0')
            {
                string gameResult = string.Empty;
                string termination = new string(_singleMoveBuffer.ToArray()).Trim();
                foreach(var result in PgnReader.Results)
                {
                    if(termination.EndsWith(result))
                    {
                        gameResult = result;
                    }
                }

                if(string.IsNullOrEmpty(gameResult))
                {
                    Terminate("*", currentGame);
                }
                else
                {
                    Terminate(gameResult, currentGame);
                }
                
                return PgnParseResult.EndOfGame;
            }
            else if (char.IsWhiteSpace(current))
            {
                _stateBuffer.Add(' ');
                var temp = new string(_singleMoveBuffer.ToArray()).Trim();
                if(!string.IsNullOrWhiteSpace(temp))
                {
                    _currentMove.Move = temp;
                }
            }
            else if (current == '\r')
            {
                //remove linebreaks
            }
            else
            {
                _stateBuffer.Add(current);
                _singleMoveBuffer.Add(current);
            }

            return PgnParseResult.None;
        }

        private void Terminate(string termination, PgnGame currentGame)
        {
            var temp = new string(_singleMoveBuffer.ToArray()).Trim();
            if(!string.IsNullOrWhiteSpace(temp))
            {
                _currentMove.Move = temp;
            }

            if(!string.IsNullOrWhiteSpace(_currentMove.Move))
            {
                currentGame.AddMove(_currentMove);
            }

            _singleMoveBuffer.Clear();
            _currentMove = new PgnMove();

            _singleMoveBuffer.Clear();

            currentGame.Termination = termination;
            currentGame.MoveSection = new string(_stateBuffer.ToArray());
        }

        internal void InitGame(PgnGame game)
        {
            _currentMove = new PgnMove();
        }
    }   
}