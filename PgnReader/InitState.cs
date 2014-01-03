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

            public override void OnEnter(PgnMove currentMove)
            {
                base.OnEnter(currentMove);
                _isEndOfLine = false;
            }

            public override PgnParseResult Parse(char current, char next, PgnGame currentGame)
            {
                if(current == '\n')
                {
                    if(!_isEndOfLine)
                    {
                        _isEndOfLine = true;
                    }
                    else//its an empty line
                    {
                        if(currentGame.HasTags)
                        {
                            ChangeState(this, _movesSectionState);
                            return PgnParseResult.None;
                        }
                    }
                }
                else
                {
                    _isEndOfLine = false;
                }

                if(current == PgnToken.RestOfLineComment.Token)
                {
                    ChangeState(this, _restOfLineCommentState);
                }
                else if(current == PgnToken.TextCommentBegin.Token)
                {
                    ChangeState(this, _textCommentState);
                }
                else if(current == PgnToken.TagBegin.Token)
                {
                    ChangeState(this, _tagSectionState);
                }
                else if(current == PgnToken.Period.Token)
                {
                    ChangeState(this, _movesSectionState);
                }

                return PgnParseResult.None;
            }
        }
    }
}
