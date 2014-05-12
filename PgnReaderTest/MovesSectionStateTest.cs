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
    public class MovesSectionStateTest
    {

        private MovesSectionState _state;

        [TestInitialize]
        public void TestInitialize()
        {
            _state = new MovesSectionState(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _state = null;
        }

        [TestMethod]
        public void WhiteMoveTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', ' ', game);
            _state.Parse(' ', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', 'e', game);

            _state.OnExit();

            Assert.AreEqual("e4", move.Move);
            Assert.IsTrue(move.IsValid);
        }

        [TestMethod]
        public void WhiteMoveNoWhitespaceTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', 'e', game);

            _state.OnExit();

            Assert.AreEqual("e4", move.Move);
            Assert.IsTrue(move.IsValid);
        }

        [TestMethod]
        public void BlackMoveNoWhitespaceTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', 'e', game);
            _state.Parse('e', '5', game);
            _state.Parse('5', ' ', game);
            _state.Parse(' ', '2', game);
            _state.Parse('2', '.', game);

            _state.OnExit();

            Assert.AreEqual(2, game.Moves.Count);
            Assert.AreEqual("e5", game.Moves[1].Move);
        }

        [TestMethod]
        public void WhiteWinsTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', '1', game);
            _state.Parse('1', '-', game);
            _state.Parse('-', '0', game);
            var result = _state.Parse('0', ' ', game);

            _state.OnExit();

            Assert.AreEqual("1-0", game.Result);
            Assert.AreEqual(PgnParseResult.EndOfGame, result);
        }

        [TestMethod]
        public void BlackWinsTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', '0', game);
            _state.Parse('0', '-', game);
            _state.Parse('-', '1', game);
            var result = _state.Parse('1', ' ', game);

            _state.OnExit();

            Assert.AreEqual("0-1", game.Result);
            Assert.AreEqual(PgnParseResult.EndOfGame, result);
        }

        [TestMethod]
        public void DrawResultTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', '1', game);
            _state.Parse('1', '/', game);
            _state.Parse('/', '2', game);
            _state.Parse('2', '-', game);
            _state.Parse('-', '1', game);
            _state.Parse('1', '/', game);
            _state.Parse('/', '2', game);
            var result = _state.Parse('2', ' ', game);

            _state.OnExit();

            Assert.AreEqual("1/2-1/2", game.Result);
            Assert.AreEqual(PgnParseResult.EndOfGame, result);
        }

        [TestMethod]
        public void OnGoingGameTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', ' ', game);
            _state.Parse(' ', '*', game);
            var result = _state.Parse('*', '-', game);

            _state.OnExit();

            Assert.AreEqual("*", game.Result);
            Assert.AreEqual(PgnParseResult.EndOfGame, result);
        }

        [TestMethod]
        public void OnGoingGameNoWhitespaceTest()
        {
            var game = new PgnGame();
            var move = new PgnMove();
            _state.OnEnter(move);

            _state.Parse('1', '.', game);
            _state.Parse('.', 'e', game);
            _state.Parse('e', '4', game);
            _state.Parse('4', '*', game);
            var result = _state.Parse('*', ' ', game);
            
            _state.OnExit();

            Assert.AreEqual("*", game.Result);
            Assert.AreEqual(PgnParseResult.EndOfGame, result);
        }
    }
}
