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
    public partial class PgnReader
    {
        private class MovesSectionState : PgnParserState
        {
            private bool _inComment;
            private string _comment;
            protected List<char> _singleMoveBuffer;
            private PgnMoves _pgnMoves;

            public MovesSectionState(PgnReader reader)
                : base(reader, 1024)
            {
                _singleMoveBuffer = new List<char>(255);
            }

            public override void OnEnter()
            {
                if(!_inComment)
                {
                    base.OnEnter();
                    _singleMoveBuffer.Clear();
                    _stateBuffer.Add('1');
                    _stateBuffer.Add('.');
                    _pgnMoves = new PgnMoves();
                }
                else
                {
                    _pgnMoves.AddMove(new PgnMove(new string(_singleMoveBuffer.ToArray()), _previousState.GetStateBuffer()));

                    _inComment = false;
                    _singleMoveBuffer.Clear();
                }
            }
            public override PgnParseResult Parse(char current, char next, PgnGame currentGame)
            {
                if (current == PgnToken.RestOfLineComment.Token)
                {
                    _inComment = true;
                    ChangeState(this, _restOfLineCommentState);
                }
                else if (current == PgnToken.TextCommentBegin.Token)
                {
                    _inComment = true;
                    ChangeState(this, _parenthesisCommentState);
                }
                else if(current == ' ' && _singleMoveBuffer.Count > 0)
                {
                    var move = new string(_singleMoveBuffer.ToArray()).Trim();
                    if(!string.IsNullOrWhiteSpace(move))
                    {
                        _pgnMoves.AddMove(new PgnMove(move));
                        _stateBuffer.Add(current);
                        _singleMoveBuffer.Clear();
                    }
                }
                else if(current == PgnToken.Period.Token)
                {
                    _singleMoveBuffer.Clear();
                    _stateBuffer.Add(current);
                }
                else if (current == PgnToken.TagBegin.Token || current == '\0')
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

                    _pgnMoves.Termination = termination;
                    _pgnMoves.MoveSection = new string(_stateBuffer.ToArray());
                    currentGame.AddMoves(_pgnMoves);
                    ChangeState(this, _tagSectionState);
                    return PgnParseResult.EndOfGame;
                }
                else if (current == '\n')
                {
                    _stateBuffer.Add(' ');
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
}