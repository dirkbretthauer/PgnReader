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
    public class PgnReaderTest
    {

        private const string GameWithoutComments = @"[Event ""5th Classic GpA 2013""]
            [Site ""London ENG""]
            [Date ""2013.12.11""]
            [Round ""1""]
            [White ""McShane, L.""]
            [Black ""Anand, V.""]
            [Result ""0-1""]
            [ECO ""B11""]
            [WhiteElo ""2684""]
            [BlackElo ""2773""]
            [PlyCount ""92""]
            [EventDate ""2013.12.11""]

            1. e4 c6 2. Nf3 d5 3. Nc3 Bg4 4. h3 Bxf3 5. Qxf3 e6 6. Be2 Nf6 7. O-O Bb4 8. e5
            Nfd7 9. Qg4 Bf8 10. d4 c5 11. Bg5 Qb6 12. dxc5 Qxc5 13. Be3 h5 14. Qg3 d4 15.
            Ne4 h4 16. Qf3 Qd5 17. c4 Qxe5 18. Bf4 Qf5 19. Bd3 Qh5 20. Nf6+ gxf6 21. Qxb7
            Ne5 22. Qxa8 Bd6 23. c5 Nf3+ 24. Qxf3 Qxf3 25. gxf3 Bxf4 26. b4 Nc6 27. Bb5 Kd7
            28. Rfd1 e5 29. a3 f5 30. Kf1 Kc7 31. Ke2 e4 32. fxe4 fxe4 33. Bxc6 d3+ 34. Kf1
            Kxc6 35. Kg2 Kd5 36. Rg1 Be5 37. Rad1 Rg8+ 38. Kf1 Rxg1+ 39. Kxg1 f5 40. Kg2
            Kd4 41. c6 f4 42. b5 Bc7 43. Rb1 d2 44. Kf1 Kd3 45. a4 e3 46. fxe3 fxe3 0-1";

        private PgnReader _pgnReader;

        [TestInitialize]
        public void TestInitialize()
        {
            StringReader reader = new StringReader(GameWithoutComments);
            _pgnReader = new PgnReader(reader, PgnReader.DefaultBufferSize);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _pgnReader.Dispose();
        }

        [TestMethod]
        public void SmokeTest()
        {
            Assert.IsTrue(_pgnReader.ReadGame());
            Assert.IsNotNull(_pgnReader.CurrentGame);
        }

        [TestMethod]
        public void DateTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Date", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("2013.12.11", tag.Value);
        }

        [TestMethod]
        public void EventTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Event", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("5th Classic GpA 2013", tag.Value);
        }

        [TestMethod]
        public void SiteTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Site", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("London ENG", tag.Value);
        }

        [TestMethod]
        public void WhiteTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("White", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("McShane, L.", tag.Value);
        }

        [TestMethod]
        public void BlackTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Black", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("Anand, V.", tag.Value);
        }

        [TestMethod]
        public void ResultTagTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Result", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("0-1", tag.Value);
        }

        [TestMethod]
        public void MovesSectionTest()
        {
            _pgnReader.ReadGame();

            Assert.AreEqual(46 * 2, _pgnReader.CurrentGame.Moves.Count);

            Assert.AreEqual("e4", _pgnReader.CurrentGame.Moves[0].Move);
            Assert.AreEqual("Bf4", _pgnReader.CurrentGame.Moves[34].Move);
            Assert.AreEqual("fxe3", _pgnReader.CurrentGame.Moves[91].Move);
        }
    }
}
