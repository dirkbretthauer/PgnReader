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
        private PgnVariation _currentVariation;

        public RecursiveVariationState(PgnParserStatemachine reader)
            : base(reader)
        {
        }

        public override void OnEnter(PgnMove currentMove)
        {
            _currentVariation = new PgnVariation();
            currentMove.Variation.Add(_currentVariation);

            _currentMove = new PgnMove();
        }

        public override void OnExit()
        {
            CheckToFinishMove();
        }

        protected override PgnParseResult DoParse(char current, char next, PgnGame currentGame)
        {
            if (char.IsWhiteSpace(current) && char.IsWhiteSpace(next))
            {
                //remove double whitespaces
            }
            else if(char.IsWhiteSpace(current))
            {
                CheckToFinishMove();
            }
            else if(current == '\r')
            {
                //remove linebreaks
            }
            else if(current == '-' ||
                current == '+' ||
                current == '#')
            {
                _stateBuffer.Add(current);
            }
            else if(char.IsLetterOrDigit(current))
            {
                if(!string.IsNullOrWhiteSpace(_currentMove.Move))
                {
                    _stateBuffer.Clear();

                    _currentMove = new PgnMove();
                }

                _stateBuffer.Add(current);
            }
            else if(current == PgnToken.Period.Token)
            {
                _stateBuffer.Clear();
            }
            else
            {
                _stateBuffer.Add(current);
                //throw new PgnReaderException("RecursiveVariationState: no idea how to handle: " + current);
            }

            return PgnParseResult.None;
        }

        private void CheckToFinishMove()
        {
            if(_stateBuffer.Count > 0)
            {
                _currentMove.Move = new string(_stateBuffer.ToArray());
                _currentVariation.Add(_currentMove);
                _stateBuffer.Clear();
            }
        }
    }
}