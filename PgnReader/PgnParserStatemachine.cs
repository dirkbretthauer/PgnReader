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
        private static PgnParserState _restOfLineCommentState;
        private static PgnParserState _textCommentState;
        private static PgnParserState _tagSectionState;
        private static MovesSectionState _movesSectionState;
        private static PgnParserState _recursiveVariationState;
        private static PgnParserState _annotationState;
        #endregion


        #region properties
        public  static IEnumerable<string> Results = new string[] { "1-0", "0-1", "1/2-1/2", "*" };        
        #endregion

        public PgnParserStatemachine()
        {

            _restOfLineCommentState = new RestOfLineCommentState(this);
            _textCommentState = new TextCommentState(this);
            _tagSectionState = new TagSectionState(this);
            _movesSectionState = new MovesSectionState(this);
            _initState = new InitState(this);
            _recursiveVariationState = new RecursiveVariationState(this);
            _annotationState = new AnnotationState(this);

            _initState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _initState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);
            _initState.AddTransition(() => _tagSectionState, c => c == PgnToken.TagBegin.Token);
            _initState.AddTransition(() => _movesSectionState, c => char.IsDigit(c));

            _annotationState.AddTransition(GetPreviousState, c => c == ' ');

            _recursiveVariationState.AddTransition(GetPreviousState, c => c == PgnToken.RecursiveVariationEnd.Token);

            _restOfLineCommentState.AddTransition(GetPreviousState, c => c == '\n');

            _textCommentState.AddTransition(GetPreviousState, c => c == PgnToken.TextCommentEnd.Token);

            _tagSectionState.AddTransition(() => _initState, c => c == PgnToken.TagEnd.Token);
            _tagSectionState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _tagSectionState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);

            _movesSectionState.AddTransition(() => _annotationState, c => c == PgnToken.NumericAnnotationGlyph.Token);
            _movesSectionState.AddTransition(() => _restOfLineCommentState, c => c == PgnToken.RestOfLineComment.Token);
            _movesSectionState.AddTransition(() => _textCommentState, c => c == PgnToken.TextCommentBegin.Token);
            _movesSectionState.AddTransition(() => _recursiveVariationState, c => c == PgnToken.RecursiveVariationBegin.Token);
            _movesSectionState.AddTransition(() => _initState, c => c == PgnToken.TagBegin.Token, game => _movesSectionState.TerminateGame(game) );


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
            _currentState.TryTransite(current, currentGame);
            
            return _currentState.Parse(current, next, currentGame);
        }
    }
}