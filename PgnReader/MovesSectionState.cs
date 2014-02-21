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
        private bool _inComment;
        protected List<char> _singleMoveBuffer;
        private PgnMoves _pgnMoves;

        public MovesSectionState(PgnParserStatemachine reader)
            : base(reader, 1024)
        {
            _singleMoveBuffer = new List<char>(255);
        }

        public override void OnEnter(PgnMove currentMove)
        {
            if(!_inComment)
            {
                base.OnEnter(currentMove);
                _singleMoveBuffer.Clear();
                _pgnMoves = new PgnMoves();
                _currentMove = new PgnMove();
            }
            else
            {
                _inComment = false;
            }
        }

        public override void OnExit()
        {
            var temp = GetStateBuffer().Trim();
            if(!string.IsNullOrWhiteSpace(temp))
            {
                _currentMove.Move = temp;
            }
        }

        public void TerminateGame(PgnGame game)
        {
            string termination = new string(_singleMoveBuffer.ToArray()).Trim();
            foreach(var result in PgnReader.Results)
            {
                if(termination.EndsWith(result))
                {
                    _pgnMoves.Termination = result;
                }
            }
            if(string.IsNullOrWhiteSpace(_pgnMoves.Termination))
            {
                if(_pgnMoves.Moves.Count > 0)
                {

                    termination = _pgnMoves.Moves[_pgnMoves.Moves.Count - 1].Move;

                    foreach(var result in PgnReader.Results)
                    {
                        if(termination.EndsWith(result))
                        {
                            _pgnMoves.Moves.RemoveAt(_pgnMoves.Moves.Count - 1);
                            _pgnMoves.Termination = result;
                        }
                    }
                }
            }

            _pgnMoves.Termination = termination;
            _pgnMoves.MoveSection = new string(_stateBuffer.ToArray());
            game.AddMoves(_pgnMoves);
        }

        public override PgnParseResult Parse(char current, char next, PgnGame currentGame)
        {
            if(char.IsLetterOrDigit(current))
            {
                if(!string.IsNullOrWhiteSpace(_currentMove.Move))
                {
                    _pgnMoves.AddMove(_currentMove);
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
            else if (current == PgnToken.TagBegin.Token || current == '\0')
            {
                TerminateGame(currentGame);
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
    }   
}