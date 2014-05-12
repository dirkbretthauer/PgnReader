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
    public class TagSectionStateTest
    {

        private TagSectionState _state;

        [TestInitialize]
        public void TestInitialize()
        {
            _state = new TagSectionState(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _state = null;
        }

        [TestMethod]
        public void EventTagTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('E', 'v', game);
            _state.Parse('v', 'e', game);
            _state.Parse('e', 'n', game);
            _state.Parse('n', 't', game);
            _state.Parse('t', ' ', game);
            _state.Parse(' ', '"', game);
            _state.Parse('"', 'c', game);
            _state.Parse('c', 'o', game);
            _state.Parse('o', 'o', game);
            _state.Parse('o', 'l', game);
            _state.Parse('l', '"', game);
            _state.Parse('"', ']', game);
            

            _state.OnExit();
            PgnTag tag;
            game.TryGetTag(PgnTag.Event, out tag);

            Assert.AreEqual("cool", tag.Value);
        }

        [TestMethod]
        public void WhiteTagTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('W', 'h', game);
            _state.Parse('h', 'i', game);
            _state.Parse('i', 't', game);
            _state.Parse('t', 'e', game);
            _state.Parse('e', ' ', game);
            _state.Parse(' ', '"', game);
            _state.Parse('"', 'A', game);
            _state.Parse('A', 'n', game);
            _state.Parse('n', 'a', game);
            _state.Parse('a', 'n', game);
            _state.Parse('n', 'd', game);
            _state.Parse('d', '"', game);
            _state.Parse('"', ']', game);


            _state.OnExit();
            PgnTag tag;
            game.TryGetTag(PgnTag.White, out tag);

            Assert.AreEqual("Anand", tag.Value);
        }

        [TestMethod]
        public void UnknownTagTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('N', 'o', game);
            _state.Parse('o', 't', game);
            _state.Parse('t', 'O', game);
            _state.Parse('O', 'K', game);
            _state.Parse('K', ' ', game);
            _state.Parse(' ', '"', game);
            _state.Parse('"', 'A', game);
            _state.Parse('A', 'n', game);
            _state.Parse('n', 'a', game);
            _state.Parse('a', 'n', game);
            _state.Parse('n', 'd', game);
            _state.Parse('d', '"', game);
            _state.Parse('"', ']', game);


            _state.OnExit();
            PgnTag tag;
            game.TryGetTag("NotOK", out tag);

            Assert.AreEqual("Anand", tag.Value);
        }
    }
}
