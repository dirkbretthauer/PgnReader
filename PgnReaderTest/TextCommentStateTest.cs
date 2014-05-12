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
using System.IO;
using CChessCore.Pgn;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PgnReaderTest
{
    [TestClass]
    public class TextCommentStateTest
    {

        private TextCommentState _state;

        [TestInitialize]
        public void TestInitialize()
        {
            _state = new TextCommentState(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _state = null;
        }

        [TestMethod]
        public void SmokeTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('h', 'a', game);

            _state.OnExit();

            Assert.AreEqual("h", move.Comment);
        }

        [TestMethod]
        public void HelloWorldTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('h', 'e', game);
            _state.Parse('e', 'l', game);
            _state.Parse('l', 'l', game);
            _state.Parse('l', 'o', game);
            _state.Parse('o', ' ', game);
            _state.Parse(' ', 'w', game);
            _state.Parse('w', 'o', game);
            _state.Parse('o', 'r', game);
            _state.Parse('r', 'l', game);
            _state.Parse('l', 'd', game);
            _state.Parse('d', '\n', game);

            _state.OnExit();

            Assert.AreEqual("hello world", move.Comment);
        }

        [TestMethod]
        public void RemovesAllkindsOfWhitespacesTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('h', 'e', game);
            _state.Parse('e', 'l', game);
            _state.Parse('l', 'l', game);
            _state.Parse('l', 'o', game);
            _state.Parse('o', '\t', game);
            _state.Parse('\t', ' ', game);
            _state.Parse(' ', 'w', game);
            _state.Parse('w', 'o', game);
            _state.Parse('o', 'r', game);
            _state.Parse('r', 'l', game);
            _state.Parse('l', 'd', game);
            _state.Parse('d', '\r', game);
            _state.Parse('\r', '\n', game);

            _state.OnExit();

            Assert.AreEqual("hello world", move.Comment);
        }
    }
}
