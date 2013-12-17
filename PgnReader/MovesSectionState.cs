#region Header
////////////////////////////////////////////////////////////////////////////// 
//    This file is part of $projectname$.
//
//    $projectname$ is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    $projectname$ is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with $projectname$.  If not, see <http://www.gnu.org/licenses/>.
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
            public override PgnParseResult Parse(char c, PgnGame currentGame)
            {
                if (c == PgnToken.RestOfLineComment.Token)
                {
                    _inComment = true;
                    ChangeState(this, _restOfLineCommentState);
                }
                else if (c == PgnToken.LeftParenthesis.Token)
                {
                    _inComment = true;
                    ChangeState(this, _parenthesisCommentState);
                }
                else if(c == ' ' && _singleMoveBuffer.Count > 0)
                {
                    _pgnMoves.AddMove(new PgnMove(new string(_singleMoveBuffer.ToArray()).Trim()));
                    _stateBuffer.Add(c);
                    _singleMoveBuffer.Clear();
                }
                else if(c == PgnToken.Period.Token)
                {
                    _singleMoveBuffer.Clear();
                    _stateBuffer.Add(c);
                }
                else if (c == PgnToken.LeftBracket.Token || c == '\0')
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
                else if (c == '\n')
                {
                    _stateBuffer.Add(' ');
                }
                else if (c == '\r')
                {
                    //remove linebreaks
                }
                else
                {
                    _stateBuffer.Add(c);
                    _singleMoveBuffer.Add(c);
                }

                return PgnParseResult.None;
            }
        }   
    }
}