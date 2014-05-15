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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CChessCore.Pgn
{
    public class PgnParserStatemachine
    {
        #region fields
        private PgnParserState _currentState;
        private PgnParserState _previousState;
        private static PgnParserState _initState;
        private static TextCommentState _restOfLineCommentState;
        private static TextCommentState _textCommentState;
        private static PgnParserState _tagSectionState;
        private static MovesSectionState _movesSectionState;
        private static RecursiveVariationState _recursiveVariationState;
        private static PgnParserState _annotationState;
        #endregion

        public PgnParserStatemachine()
        {
            _restOfLineCommentState = new TextCommentState(this);
            _textCommentState = new TextCommentState(this);
            _tagSectionState = new TagSectionState(this);
            _movesSectionState = new MovesSectionState(this);
            _initState = new InitState(this);
            _recursiveVariationState = new RecursiveVariationState(this);
            _annotationState = new AnnotationState(this);

            _initState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _initState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);
            _initState.AddTransition(() => _tagSectionState, c => c == PgnToken.TagBegin.Token);
            _initState.AddTransition(() => _movesSectionState, c => char.IsDigit(c), game => _movesSectionState.InitGame(game));

            _annotationState.AddExit(GetPreviousState, c => c == ' ');

            _recursiveVariationState.AddExit(GetPreviousState, c => c == PgnToken.RecursiveVariationEnd.Token && !_recursiveVariationState.VariationContainsOpeningBrace);
            _recursiveVariationState.AddTransition(() => _annotationState, c => c == PgnToken.NumericAnnotationGlyph.Token);
            _recursiveVariationState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _recursiveVariationState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);

            _restOfLineCommentState.AddExit(GetPreviousState, c => c == '\n');

            _textCommentState.AddExit(GetPreviousState, c => c == PgnToken.TextCommentEnd.Token && !_textCommentState.CommentContainsOpeningBrace);

            _tagSectionState.AddExit(() => _initState, c => c == PgnToken.TagEnd.Token);
            _tagSectionState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _tagSectionState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);

            _movesSectionState.AddTransition(() => _annotationState, c => c == PgnToken.NumericAnnotationGlyph.Token);
            _movesSectionState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _movesSectionState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);
            _movesSectionState.AddTransition(() => _recursiveVariationState, c => c == PgnToken.RecursiveVariationBegin.Token);
            _movesSectionState.AddExit(() => _tagSectionState, c => c == PgnToken.TagBegin.Token);

            _currentState = _initState;
        }

        private PgnParserState GetPreviousState()
        {
            return _previousState;
        }

        internal void SetState(PgnParserState newState)
        {
            _previousState = _currentState;
            _currentState = newState;
        }

        internal PgnParseResult Parse(char current, char next, PgnGame currentGame)
        {
            var result = _currentState.Parse(current, next, currentGame);
            if(result == PgnParseResult.EndOfGame)
            {
                _currentState.ChangeState(_initState);
            }

            return result;
        }
    }
}