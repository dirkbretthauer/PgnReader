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

namespace CChessCore.Pgn
{
    internal abstract class PgnParserState
    {
        private IList<Tuple<Func<char, bool>, Func<PgnParserState>, Action<PgnGame>>> _transitions =
            new List<Tuple<Func<char, bool>, Func<PgnParserState>, Action<PgnGame>>>();

        protected readonly PgnParserStatemachine _statemachine;
        
        protected List<char> _stateBuffer;
        protected PgnMove _currentMove;

        protected PgnParserState(PgnParserStatemachine reader, int stateBufferSize = 256)
        {
            _statemachine = reader;
            _stateBuffer = new List<char>(stateBufferSize);
        }

        public abstract PgnParseResult Parse(char current, char next, PgnGame currentGame);
            
        public virtual void OnExit() { }

        public virtual void OnEnter(PgnMove currentMove)
        {
            _currentMove = currentMove;
            _stateBuffer.Clear();
        }

        public void AddTransition(Func<PgnParserState> next, Func<char, bool> condition)
        {
            _transitions.Add(new Tuple<Func<char, bool>, Func<PgnParserState>, Action<PgnGame>>(condition, next, null));
        }

        public void AddTransition(Func<PgnParserState> next, Func<char, bool> condition, Action<PgnGame> action)
        {
            _transitions.Add(new Tuple<Func<char, bool>, Func<PgnParserState>, Action<PgnGame>>(condition, next, action));
        }

        public bool TryTransite(char current, PgnGame game)
        {
            foreach(var item in _transitions)
            {
                if(item.Item1(current))
                {
                    ChangeState(this, item.Item2(), _currentMove);
                    if(item.Item3 != null)
                    {
                        item.Item3(game);
                    }
                    return true;
                }
            }

            return false;
        }

        protected void ChangeState(PgnParserState oldState, PgnParserState newState, PgnMove currentMove = null)
        {
            this.OnExit();
            _statemachine.SetState(newState);
            
            newState.OnEnter(currentMove);
        }

        internal string GetStateBuffer()
        {
            return new string(_stateBuffer.ToArray());
        }
    }
}