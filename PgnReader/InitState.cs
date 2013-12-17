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

namespace CChessCore.Pgn
{
    public partial class PgnReader
    {
        private class InitState : PgnParserState
        {
            private bool _isEndOfLine;

            public InitState(PgnReader reader)
                : base(reader, 0)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                _isEndOfLine = false;
            }

            public override PgnParseResult Parse(char c, PgnGame currentGame)
            {
                if (c == '\n')
                {
                    if (!_isEndOfLine)
                    {
                        _isEndOfLine = true;
                    }
                    else//its an empty line
                    {
                        if (currentGame.HasTags)
                        {
                            ChangeState(this,_movesSectionState);
                            return PgnParseResult.None;
                        }
                    }
                }
                else
                {
                    _isEndOfLine = false;
                }

                if (c == PgnToken.RestOfLineComment.Token)
                {
                    ChangeState(this, _restOfLineCommentState);
                }
                else if (c == PgnToken.LeftParenthesis.Token)
                {
                    ChangeState(this, _parenthesisCommentState);
                }
                else if (c == PgnToken.LeftBracket.Token)
                {
                    ChangeState(this, _tagSectionState);
                }
                else if (c == PgnToken.Period.Token)
                {
                    ChangeState(this, _movesSectionState);
                }

                return PgnParseResult.None;
            }
        }
    }
}