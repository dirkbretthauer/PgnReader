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
using System.Collections.Generic;

namespace CChessCore.Pgn
{
    public partial class PgnReader
    {
        protected abstract class PgnParserState
        {
            protected readonly PgnReader _reader;
            protected PgnParserState _previousState;
            protected List<char> _stateBuffer;

            protected PgnParserState(PgnReader reader, int stateBufferSize = 256)
            {
                _reader = reader;
                _stateBuffer = new List<char>(stateBufferSize);
            }

            public abstract PgnParseResult Parse(char c, PgnGame currentGame);
            public virtual void OnExit() { }
            public virtual void OnEnter()
            {
                _stateBuffer.Clear();
            }

            protected void ChangeState(PgnParserState oldState, PgnParserState newState)
            {
                this.OnExit();
                _reader.SetState(newState);
                newState._previousState = oldState;
                newState.OnEnter();
            }

            protected void GoToPreviousState()
            {
                ChangeState(this, _previousState);
            }

            internal string GetStateBuffer()
            {
                return new string(_stateBuffer.ToArray());
            }
        }
    }
}