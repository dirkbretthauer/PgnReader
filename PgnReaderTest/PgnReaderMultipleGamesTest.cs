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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CChessCore.Pgn;
using System.IO;

namespace PgnReaderTest
{
    /// <summary>
    /// Summary description for PgnReaderMultipleGamesTest
    /// </summary>
    [TestClass]
    public class PgnReaderMultipleGamesTest
    {

        private const string GamesWithoutComments = @"[Event ""5th Classic GpD 2013""]
            [Site ""London ENG""]
            [Date ""2013.12.11""]
            [Round ""2.1""]
            [White ""Short, Nigel D""]
            [Black ""Caruana, Fabiano""]
            [Result ""0-1""]
            [ECO ""A00""]
            [WhiteElo ""2683""]
            [BlackElo ""2782""]
            [PlyCount ""52""]
            [EventDate ""2013.12.11""]

            1. b4 d5 2. Bb2 Nf6 3. e3 Bf5 4. Be2 e6 5. a3 Be7 6. d3 h6 7. Nd2 Nbd7 8. Ngf3
            O-O 9. h3 Bg6 10. Nb3 Bd6 11. g4 e5 12. Nh4 Bh7 13. Nf5 Bxf5 14. gxf5 Re8 15.
            Rg1 c5 16. bxc5 Nxc5 17. Nxc5 Bxc5 18. Bf1 Rc8 19. Qd2 Bb6 20. Kd1 Ba5 21. c3
            Kh8 22. f3 d4 23. Qg2 Rg8 24. cxd4 Nd5 25. Qf2 Qb6 26. Be2 Qb3# 0-1

            [Event ""5th Classic GpD 2013""]
            [Site ""London ENG""]
            [Date ""2013.12.11""]
            [Round ""2.2""]
            [White ""Howell, David W L""]
            [Black ""Sutovsky, Emil""]
            [Result ""1-0""]
            [ECO ""A36""]
            [WhiteElo ""2640""]
            [BlackElo ""2657""]
            [PlyCount ""101""]
            [EventDate ""2013.12.11""]

            1. c4 g6 2. Nc3 c5 3. g3 Bg7 4. Bg2 Nc6 5. d3 Nf6 6. e4 O-O 7. Nge2 d6 8. O-O
            a6 9. Rb1 Bg4 10. f3 Bd7 11. a4 Ne8 12. Be3 Nd4 13. b4 Rb8 14. Bf2 cxb4 15.
            Rxb4 Qa5 16. Qb1 Nxe2+ 17. Nxe2 b5 18. Rc1 Nc7 19. Be1 Qb6+ 20. Bf2 Qa5 21. Be1
            Ne6 22. Kf1 Nc5 23. axb5 axb5 24. d4 Ne6 25. Bc3 f5 26. exf5 Ng5 27. Rxb5 Rxb5
            28. Bxa5 Rxb1 29. Rxb1 Rxf5 30. f4 Rxa5 31. fxg5 Be6 32. Rc1 Rf5+ 33. Ke1 Rxg5
            34. Nf4 Bg4 35. Bd5+ Kf8 36. h4 Rf5 37. Ne6+ Kf7 38. Nxg7+ Kxg7 39. Be6 Kf6 40.
            Bxf5 Kxf5 41. c5 Ke4 42. c6 Kd5 43. Kd2 Bc8 44. Ke3 e5 45. dxe5 dxe5 46. c7 Kd6
            47. Rd1+ Kxc7 48. Ke4 Bg4 49. Rd2 Kc6 50. Kxe5 Bf5 51. h5 1-0

            [Event ""SC Garching 1980 e.V. 2-Muenchener SC 1836 e.V. 1""]
            [Date ""2014.04.06""]
            [Round ""9.1""]
            [Board ""1""]
            [White ""Pezerovic, Edin""]
            [Black ""Schleich, Markus""]
            [Result ""1/2-1/2""]
            [WhiteElo ""2358""]
            [BlackElo ""2228""]
            [WhiteTeam ""Münchener SC 1836 e.V. 1""]
            [BlackTeam ""SC Garching 1980 e.V. 2""]
            [PlyCount  ""24""]
 
            1.d4 Nf6 2.Bg5 Ne4 3.Bf4 d5 4.Nf3 c5 5.c3 Nc6 6.e3 Qb6 7.Qc1 Bf5 8.Nbd2 e6 9.Be2 Be7 10.Nxe4 Bxe4 11.Qd2 O-O 12.O-O Rac8  
 
 
            [Event ""SC Garching 1980 e.V. 2-Muenchener SC 1836 e.V. 1""]
            [Date ""2014.04.06""]
            [Round ""9.2""]
            [Board ""2""]
            [White ""Dehlinger, Alexander""]
            [Black ""Hantak, Adam""]
            [Result ""1-0""]
            [WhiteElo ""2228""]
            [BlackElo ""2166""]
            [WhiteTeam ""SC Garching 1980 e.V. 2""]
            [BlackTeam ""Münchener SC 1836 e.V. 1""]
            [PlyCount  ""89""]
 
            1.Nf3 Nf6 2.c4 g6 3.g3 Bg7 4.Bg2 O-O 5.O-O d6 6.d4 Nc6 7.Nc3 Bg4 8.Be3 Nd7 9.Qd2 Re8 10.Rfd1 e5 11.dxe5 Ndxe5 12.Nxe5 Nxe5 13.b3 Qc8 14.Rac1 Bh3 15.Qd5 Nc6 16.Bxh3 Qxh3 17.Qg2 Qxg2+ 18.Kxg2 Bxc3 19.Rxc3 a5 20.c5 dxc5 21.Rd7 b6 22.Rxc7 Rac8 23.Rxc8 Rxc8 24.Rd3 Rd8 25.Rxd8+ Nxd8 26.Kf3 Kf8 27.Ke4 Ke7 28.Kd5 Kd7 29.Bd2 Ne6 30.Bc3 Nc7+ 31.Ke5 f6+ 32.Ke4 Ke6 33.Kd3 Nb5 34.Bb2 f5 35.f3 Kd5 36.e4+ fxe4+ 37.fxe4+ Kc6 38.Be5 Nd6 39.Bxd6 Kxd6 40.Kc4 Ke5 41.Kb5 Kxe4 42.Kxb6 Kf3 43.Kxa5 Kg2 44.Kb5 Kxh2 45.a4  
             ";

        private PgnReader _pgnReader;

         //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            StringReader reader = new StringReader(GamesWithoutComments);
            _pgnReader = new PgnReader(reader, PgnReader.DefaultBufferSize);
        }
        
         //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            _pgnReader.Dispose();
        }

        [TestMethod]
        public void FirstGameTest()
        {
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Date", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("2013.12.11", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Event", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("5th Classic GpD 2013", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Site", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("London ENG", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("White", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("Short, Nigel D", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Black", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("Caruana, Fabiano", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Result", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("0-1", tag.Value);

            Assert.AreEqual(26 * 2, _pgnReader.CurrentGame.Moves.Count);

            Assert.AreEqual("b4", _pgnReader.CurrentGame.Moves[0].Move);
            Assert.AreEqual("Nxc5", _pgnReader.CurrentGame.Moves[32].Move);
            Assert.AreEqual("Qb3#", _pgnReader.CurrentGame.Moves[51].Move);
        }

        [TestMethod]
        public void SecondGameTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            PgnTag tag;

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Date", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("2013.12.11", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Event", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("5th Classic GpD 2013", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Site", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("London ENG", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("White", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("Howell, David W L", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Black", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("Sutovsky, Emil", tag.Value);

            Assert.IsTrue(_pgnReader.CurrentGame.TryGetTag("Result", out tag));
            Assert.IsNotNull(tag);
            Assert.AreEqual("1-0", tag.Value);

            Assert.AreEqual(101, _pgnReader.CurrentGame.Moves.Count);

            Assert.AreEqual("c4", _pgnReader.CurrentGame.Moves[0].Move);
            Assert.AreEqual("Nxg7+", _pgnReader.CurrentGame.Moves[74].Move);
            Assert.AreEqual("h5", _pgnReader.CurrentGame.Moves[100].Move);
        }

        [TestMethod]
        public void ThirdGameTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();

            PgnTag result;
            _pgnReader.CurrentGame.TryGetTag("Result", out result);
            Assert.AreEqual("1/2-1/2", result.Value);
            Assert.AreEqual(24, _pgnReader.CurrentGame.Moves.Count);

            Assert.IsFalse(_pgnReader.CurrentGame.Moves.Contains(null));
        }

        [TestMethod]
        public void FourthGameTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();

            PgnTag result;
            _pgnReader.CurrentGame.TryGetTag("Result", out result);
            Assert.AreEqual("1-0", result.Value);
            Assert.AreEqual(89, _pgnReader.CurrentGame.Moves.Count);
        }
    }
}
