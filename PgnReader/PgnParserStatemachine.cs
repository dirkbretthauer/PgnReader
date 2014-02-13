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
    public partial class PgnParserStatemachine
    {
        #region fields
        private bool _disposed;
        private readonly TextReader _reader;
        private char[] _readerBuffer;
        private int _readerBufferPosition;
        private int _charsRead;
        private char _nextChar = '\0';
        private Char? _currentChar;
        private readonly IList<PgnGame> _games;

        private PgnParserState _currentState;
        private static PgnParserState _initState;
        private static PgnParserState _restOfLineCommentState;
        private static PgnParserState _textCommentState;
        private static PgnParserState _tagSectionState;
        private static PgnParserState _movesSectionState;
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

            _currentState = _initState;
        }

        private void SetState(PgnParserState newState)
        {
            _currentState = newState;
        }

        internal PgnParseResult Parse(char current, char next, PgnGame currentGame)
        {
            return _currentState.Parse(current, next, currentGame);
        }
    }
}