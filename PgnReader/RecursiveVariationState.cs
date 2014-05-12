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
    internal class RecursiveVariationState : PgnParserState
    {
        public bool VariationContainsOpeningBrace { get; private set; }

        protected List<char> _singleMoveBuffer;
        private PgnVariation _currentVariation;
        private PgnMove _currentMoveInVariation;

        public RecursiveVariationState(PgnParserStatemachine reader)
            : base(reader)
        {
            _singleMoveBuffer = new List<char>(255);
        }

        public override void OnEnter(PgnMove currentMove)
        {
            _currentMoveInVariation = new PgnMove();
            _currentMove = currentMove;
            _currentVariation = new PgnVariation();
        }

        public override void OnExit()
        {
        }

        protected override PgnParseResult DoParse(char current, char next, PgnGame currentGame)
        {
            if (char.IsWhiteSpace(current) && char.IsWhiteSpace(next))
            {
                //remove double whitespaces
            }
            else if(char.IsWhiteSpace(current))
            {
                _stateBuffer.Add(' ');
                var temp = new string(_singleMoveBuffer.ToArray());
                if(!string.IsNullOrWhiteSpace(temp))
                {
                    _currentMoveInVariation.Move = temp.Trim();
                    _currentVariation.Add(_currentMoveInVariation);
                    _currentMoveInVariation = new PgnMove();
                }
            }
            else if(current == '\r')
            {
                //remove linebreaks
            }

            else if(char.IsLetterOrDigit(current))
            {
                if(!string.IsNullOrWhiteSpace(_currentMoveInVariation.Move))
                {
                    _currentVariation.Add(_currentMoveInVariation);
                    _singleMoveBuffer.Clear();

                    _currentMoveInVariation = new PgnMove();

                }

                _stateBuffer.Add(current);
                _singleMoveBuffer.Add(current);
            }
            else if(current == PgnToken.Period.Token && next != PgnToken.Period.Token)
            {
                _singleMoveBuffer.Clear();
                _stateBuffer.Add(current);
            }
            else if(current == PgnToken.RecursiveVariationBegin.Token)
            {
                VariationContainsOpeningBrace = true;
                _stateBuffer.Add(current);
            }
            else if(current == PgnToken.RecursiveVariationEnd.Token)
            {
                VariationContainsOpeningBrace = false;
                _stateBuffer.Add(current);
            }
            else
            {
                _stateBuffer.Add(current);
            }

            return PgnParseResult.None;
        }
    }
}