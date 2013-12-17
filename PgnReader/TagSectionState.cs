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

using System;
using System.Linq;

namespace CChessCore.Pgn
{
    public partial class PgnReader
    {
        private class TagSectionState : PgnParserState
        {
            private bool _inComment = false;
            public TagSectionState(PgnReader reader)
                : base(reader)
            {
            }

            public override void OnEnter()
            {
                if (!_inComment)
                {
                    base.OnEnter();
                }
                else
                {
                    _inComment = false;
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
                else if (c == PgnToken.RightBracket.Token)
                {
                    currentGame.AddTag(PgnTag.Parse(new string(_stateBuffer.ToArray())));
                    ChangeState(this, _initState);
                }
                else
                {
                    _stateBuffer.Add(c);
                }

                return PgnParseResult.None;
            }
        }
    }
}