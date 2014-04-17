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
    public partial class PgnReader : IDisposable
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

        private PgnParserStatemachine _statemachine;
        #endregion

        /// <summary>
        /// Defines the default buffer size.
        /// </summary>
        public const int DefaultBufferSize = 0x1000;

        #region properties
        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        public int BufferSize { get; protected set; }

        public PgnGame CurrentGame { get; set; }

        public  static IEnumerable<string> Results = new string[] { "1-0", "0-1", "1/2-1/2", "*" };        
        #endregion

        /// <summary>
        /// Initializes a new instance of the PgnReader class.
        /// </summary>
        /// <param name="stream">A <see cref="TextReader"/> pointing to the pgn file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="stream"/> is a <see langword="null"/>.
        ///   </exception>
        ///   
        /// <exception cref="ArgumentException">
        /// Cannot read from <paramref name="stream"/>.
        ///   </exception>
        public PgnReader(TextReader reader, int bufferSize)
        {
            _statemachine = new PgnParserStatemachine(this);

            BufferSize = bufferSize;

            _games = new List<PgnGame>();
            _reader = reader;
            _readerBuffer = new char[BufferSize];
        }

        /// <summary>
        /// Reads one game from the pgn file.
        /// </summary>
        /// <returns><c>true</c>, if there are more records, otherwise <c>false</c>.</returns>
        public bool ReadGame()
        {
            CheckIfDisposed();

            try
            {
                CurrentGame = InternalReadGame();
            }
            catch (Exception ex)
            {
                throw new PgnReaderException(string.Format("A parsing error occurred."), ex);
            }

            return CurrentGame != null;
        }

        private PgnGame InternalReadGame()
        {
            var currentGame = new PgnGame();
            var currentCharPos = 0;
            
            while (true)
            {
                currentCharPos++;

                if (_readerBufferPosition == _charsRead)
                {
                    if(!TryReadNextBlock())
                    {
                        _statemachine.Parse('\0', '\0', currentGame);
                        
                        if(currentGame.Moves.Count == 0)
                            return null;

                        _games.Add(currentGame);
                        return currentGame;
                    }
                }

                if(_currentChar == null)
                {
                    _currentChar = _readerBuffer[_readerBufferPosition];
                    _readerBufferPosition++;
                }

                _nextChar = _readerBuffer[_readerBufferPosition];
                _readerBufferPosition++;

                if(_statemachine.Parse(_currentChar.Value, _nextChar, currentGame) == PgnParseResult.EndOfGame)
                {
                    _games.Add(currentGame);
                    _currentChar = _nextChar;
                    return currentGame;
                }

                _currentChar = _nextChar;
            }
        }

        private bool TryReadNextBlock()
        {
            _charsRead = _reader.Read(_readerBuffer, 0, _readerBuffer.Length);
            _readerBufferPosition = 0;

            if (_charsRead == 0)
            {
                // The end of the stream has been reached.
                return false;
            }

            return true;
        }

        internal void SkipNextChars(int count)
        {
            for(int i = 0; i < count; i++)
            {
                if(_readerBufferPosition == _charsRead)
                {
                    if(!TryReadNextBlock())
                        return;
                }
                _currentChar = _nextChar;
                _nextChar = _readerBuffer[_readerBufferPosition];
                _readerBufferPosition++;
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Calls Dispose(true)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The clean-up code.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //free managed resources
                    if (_reader != null)
                    {
                        _reader.Dispose();
                    }
                }
                //free native resources if there are any
                _disposed = true;
            }
        }

        /// <summary>
        /// Checks if this instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        private void CheckIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }
        #endregion
    }
}