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
    public class PgnReaderCommentsTest
    {
        private const string PreCommentGame =
            @"[Event ""5th Classic GpA 2013""]
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

            1. e4 c6 2. {Some comment here} Nf3 d5 3. Nc3 Bg4 ({Some comment here}... Bh5) 4. h3 Bxf3 5. Qxf3 e6 6. Be2 Nf6 7. O-O Bb4 8. e5
            Nfd7 9. Qg4 Bf8 10. d4 c5 11. Bg5 Qb6 12. dxc5 Qxc5 13. Be3 h5 14. Qg3 d4 15.
            Ne4 h4 16. Qf3 Qd5 17. c4 Qxe5 18. Bf4 Qf5 19. Bd3 Qh5 20. Nf6+ gxf6 21. Qxb7
            Ne5 22. Qxa8 Bd6 23. c5 Nf3+ 24. Qxf3 Qxf3 25. gxf3 ({Some comment here} 25. gxf3 Bd3 26. b4) Bxf4 26. b4 Nc6 27. Bb5 Kd7
            28. Rfd1 e5 29. a3 f5 30. Kf1 Kc7 31. Ke2 e4 32. fxe4 fxe4 33. Bxc6 d3+ 34. Kf1
            Kxc6 35. Kg2 Kd5 36. Rg1 Be5 37. Rad1 Rg8+ 38. Kf1 Rxg1+ 39. Kxg1 f5 40. Kg2;This is an eol comment
            Kd4 41. c6 f4 42. b5 Bc7 43. Rb1 d2 44. Kf1 Kd3 45. a4 e3 46. fxe3 fxe3 0-1";


        private const string GameWithComments = @"[Event ""5th Classic GpA 2013""]
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

            1. e4 c6 2. Nf3 d5 3. Nc3 Bg4 {Some comment here} 4. h3 Bxf3 5. Qxf3 e6 6. Be2 Nf6 7. O-O Bb4 8. e5
            Nfd7 9. Qg4 Bf8 10. d4 c5 11. Bg5 Qb6 12. dxc5 Qxc5 13. Be3 h5 14. Qg3 d4 15.
            Ne4 h4 16. Qf3 Qd5 17. c4 Qxe5 18. Bf4 Qf5 19. Bd3 Qh5 20. Nf6+ gxf6 21. Qxb7
            Ne5 22. Qxa8 Bd6 23. c5 Nf3+ 24. Qxf3 Qxf3 25. gxf3 {One more comment} Bxf4 26. b4 Nc6 27. Bb5 Kd7
            28. Rfd1 e5 29. a3 f5 30. Kf1 Kc7 31. Ke2 e4 32. fxe4 fxe4 33. Bxc6 d3+ 34. Kf1
            Kxc6 35. Kg2 Kd5 36. Rg1 Be5 37. Rad1 Rg8+ 38. Kf1 Rxg1+ 39. Kxg1 f5 40. Kg2;This is an eol comment
            Kd4 41. c6 f4 42. b5 Bc7 43. Rb1 d2 44. Kf1 Kd3 45. a4 e3 46. fxe3 fxe3 0-1

            [Event ""5th Classic GpA 2013""]
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

            1. e4 c6 2. Nf3 d5 3. Nc3 Bg4 (... Bh5) 4. h3 Bxf3 5. Qxf3 e6 6. Be2 Nf6 7. O-O Bb4 8. e5
            Nfd7 9. Qg4 Bf8 10. d4 c5 11. Bg5 Qb6 12. dxc5 Qxc5 13. Be3 h5 14. Qg3 d4 15.
            Ne4 h4 16. Qf3 Qd5 17. c4 Qxe5 18. Bf4 Qf5 19. Bd3 Qh5 20. Nf6+ gxf6 21. Qxb7
            Ne5 22. Qxa8 Bd6 23. c5 Nf3+ 24. Qxf3 Qxf3 25. gxf3 (25. gxf3 Bd3 26. b4) Bxf4 26. b4 Nc6 27. Bb5 Kd7
            28. Rfd1 e5 29. a3 f5 30. Kf1 Kc7 31. Ke2 e4 32. fxe4 fxe4 33. Bxc6 d3+ 34. Kf1
            Kxc6 35. Kg2 Kd5 36. Rg1 Be5 37. Rad1 Rg8+ 38. Kf1 Rxg1+ 39. Kxg1 f5 40. Kg2;This is an eol comment
            Kd4 41. c6 f4 42. b5 Bc7 43. Rb1 d2 44. Kf1 Kd3 45. a4 e3 46. fxe3 fxe3 0-1

            [Event ""5th Classic GpA 2013""]
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

            1. e4 c6 2. Nf3 d5 3. Nc3 Bg4 (... Bh5) 4. h3 Bxf3 5. Qxf3 e6 6. Be2 Nf6 7. O-O Bb4 8. e5
            Nfd7 9. Qg4 Bf8 10. d4 c5 11. Bg5 Qb6 12. dxc5 Qxc5 13. Be3 h5 14. Qg3 d4 15.
            Ne4 h4 16. Qf3 Qd5 17. c4 Qxe5 18. Bf4 Qf5 19. Bd3 Qh5 20. Nf6+ gxf6 21. Qxb7
            Ne5 22. Qxa8 Bd6 23. c5 Nf3+ 24. Qxf3 Qxf3 25. gxf3 (25. gxf3 Bd3 26. b4 (26. b3 b6)) Bxf4 26. b4 Nc6 27. Bb5 Kd7
            28. Rfd1 e5 29. a3 f5 30. Kf1 Kc7 31. Ke2 e4 32. fxe4 fxe4 33. Bxc6 d3+ 34. Kf1
            Kxc6 35. Kg2 Kd5 36. Rg1 Be5 37. Rad1 Rg8+ 38. Kf1 Rxg1+ 39. Kxg1 f5 40. Kg2;This is an eol comment
            Kd4 41. c6 f4 42. b5 Bc7 43. Rb1 d2 44. Kf1 Kd3 45. a4 e3 46. fxe3 fxe3 0-1

            [Event ""GMA, Wijk aan Zee NED""]
            [Site ""?""]
            [Date ""2003.??.??""]
            [Round ""1""]
            [White ""Anand,V""]
            [Black ""Radjabov,T""]
            [Result ""1/2""]
            [WhiteElo ""2750""]
            [BlackElo ""2620""]
            [ECO ""C12""]
            [PlyCount ""55""]
            [Annotator ""Hathaway""]

            1. e4 e6
            { I'm not terribly familiar with the style of Radjabov, so I don't know if this is his usual opening. }
            2. d4 d5 3. Nc3 Nf6 (3...Bb4 
            { The Winawer Variation is probably best, though not as easy to play. }) 4. Bg5
            { threatens e4-e5xf6 }
             (4. e5 
            { keeps pieces on the board and avoids ...dxe4 }) 4...Bb4 (4...Be7 
            { is more common and aims to trade dark-square bishops to ease Black's cramp }) (4...dxe4 
            { aims to avoid any cramp by bringing pieces into alignment for trading, though White does get at least one very good piece (Ne4 or Bg5) and an easier time castling queen-side, to stir up king-side threats }
             5. Nxe4 Be7  (
            { or Rubinstein's }
             5...Nbd7) ) 5. e5 h6 6. Bd2 (6. Bh4 g5 7. exf6 gxh4 
            { Black seems to equalize a little easier after this as he can win Pf6 in exchange for Ph4. }) 6...Bxc3 (6...Nfd7 7. Qg4 
            { and White isn't incurring any weaknesses, but is either gaining Bb4 for Nc3 or after ...Bb4-f8 Black is cramped again }
              (7. Nb5 $5 Bxd2+ 8. Qxd2 a6 9. Na3) ) 7. bxc3 Ne4 8. Qg4
            { White immediately takes aim at the backward Pg7 & Rh8 and usually Pf7 & Ke8. For the moment Bd2 serves to defend Pc3 and to prevent ...Qd8-g5 (offering a queen trade to end the pressure) . }
             (
            { While }
             8. h4 
            { is often useful in the French Defense with this pawn structure, I don't know that it's been tried in this opening on this move. }) 8...g6 9. Bd3 (9. h4 
            { could take over for Bd2 in guarding g5 and preparing a later attack by f2-f4, h4-h5 or vice versa. It also would allow Rh1 to develop to build the direct frontal threats to Pf7 & Pg6. }
             9...c5 10. Bd3 Nxd2 11. Kxd2 Qa5 12. dxc5 Qxc5 13. Ne2 Qxf2 $4 14. Raf1 Qc5 15. Bxg6 fxg6 16. Qxg6+)  (9. Qd1 
            { Fritz7; Odd! }) 9...Nxd2 10. Kxd2 c5 11. Nf3
            { This has been considered the main line for many years, but I wonder if White can allow ...c5-c4 and not use more pawns to fight through Black's pawns. }
             (11. dxc5 
            { is probably still wrong because of ...Qg5+ }) (11. h4 
            { still makes sense }) 11...Bd7 (11...c4 $6 
            { The problem with this is that however much it slows White, it also limits Black's queen-side offensive possibilities. }) (
            { Prematurely playing }
             11...cxd4 
            { lets White straighten-out his pawns and Black has made no real progress. }
             12. cxd4)  (11...Qa5 $5 
            { Fritz7: with the idea of ...cxd4 }) 12. dxc5 Qe7 13. Rab1 Bc6 14. Nd4 Nd7
            { These last few moves have been quite unusual for a French Defense, but they make sense; Qe7 defends Pf7 while Bc6 defends Pb7 and Nd7 threatens Pc5 & Pe5. }
            15. Rhe1 (15. Nxc6 bxc6 16. Rb7 Qxc5 17. Qf4 g5 18. Qd4 Qa5 19. Rb2 c5 $11 
            { Fritz7 }) 15...Nxc5 16. Re3
            { another way of getting the rook into position, in front of the king-side pawns, to threaten Black's king-side pawns }
            16...h5 17. Qg3 O-O-O
            { After this it would seem Black's pieces can handle any threats White can generate. However, black might also have ideas of winning. How might he do that? Well, ...Be8, ...Kc8-b8-a8, ...Rd8-c8, ...Nc5-a4 and Pc3 is a target (slow I know) . Another idea is to keep Kd2 from ever escaping to safety by advancing ...h5-h4-h3 to break open the king-side and open the h-file for Black's rooks. }
             (17...h4 $15 
            { Fritz7 }) (17...Nxd3 $15 
            { Fritz7 }) 18. Ke1 Qc7 (18...h4 19. Qg4 Rh5) 19. h4
            { Anand aims to keep the king-side perfectly safe to ensure a draw. }
             (19. Qh4 
            { Fritz7 }) 19...Qa5 20. Kf1 (20. Nxc6 bxc6 21. Kf1 Kd7 20. Qf4 Ke8 $11 
            { Fritz7 }) 20...Rd7 (
            { Premature is }
             20...Qxa2 21. Ree1 Qa5  (21...Ba4 $11 
            { Fritz7 })  22. Ra1 Qxc3 23. Nxc6 bxc6 24. Ba6+ $18) 21. Qf4
            { This general activity is perfect. It threatens Pf7, defends Nd4 and in some cases prepares for Qf4-b4 to attack Kc8. }
             (21. Ree1 
            { Fritz7 }) (21. Nxc6 bxc6 22. Ree1 
            { Fritz7 }) 21...Rhd8
            { Black is probably wondering why he organized his pieces to only defend light squares. Only Qa5 and Nc5 can get to dark squares and that makes White's task of coordinating much easier. }
             (21...Qxa2 
            { still premature }
             22. Nxc6 bxc6 23. Qb4 Nb7 24. Ree1)  (21...Qxc3 $4 22. Nxc6 bxc6 23. Ba6+)  (21...Rc7 $14 
            { Fritz7 }) (21...Na4 $14 
            { Fritz7 }) 22. Kg1 (22. Nxc6 bxc6 23. Qb4 Qxb4 24. cxb4 d4 25. Ree1 Na4 $11 
            { Fritz7 }) 22...Nxd3 23. Rxd3 (23. cxd3 Qxc3 24. Rg3 Rc7 $14 
            { Fritz7 }) 23...Qc5 (23...Qxa2 24. Rdd1 Qc4 $11 
            { Fritz7 }) 24. Rb4 a5 $2 (24...Rc7 
            { Mark and Fritz7 agree! }) 25. Rb1 Rc7 26. Qc1 Be8 27. Nb3 (27. Qb2 
            { If White commits too quickly to the b-file then Black might actually create some play against Ph4 and on the c-file. }
             27...Qe7  (27...a4 $11 
            { Fritz7 })  28. Nf3 Rc4 
            { possibly preparing ...b5 }) 27...Qb6 (27...Qc4 28. Nxa5 Qxh4 $14 
            { Fritz7 }) 28. Nd4
            { Black created the weakness (Pa5) and can't quite defend it, so Anand forces a draw. }
            1/2-1/2";


        private PgnReader _pgnReader;

        [TestInitialize]
        public void TestInitialize()
        {
            StringReader reader = new StringReader(GameWithComments);
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
        public void MovesSectionTest()
        {
            _pgnReader.ReadGame();

            Assert.AreEqual(46 * 2, _pgnReader.CurrentGame.Moves.Count);

            Assert.AreEqual("e4", _pgnReader.CurrentGame.Moves[0].Move);
            Assert.AreEqual("Bf4", _pgnReader.CurrentGame.Moves[34].Move);
            Assert.AreEqual("fxe3", _pgnReader.CurrentGame.Moves[91].Move);
        }

        [TestMethod]
        public void ReadCommentTest()
        {
            _pgnReader.ReadGame();

            Assert.AreEqual("Some comment here", _pgnReader.CurrentGame.Moves[5].Comment);
            Assert.AreEqual("One more comment", _pgnReader.CurrentGame.Moves[48].Comment);
            Assert.AreEqual("This is an eol comment", _pgnReader.CurrentGame.Moves[78].Comment);
        }

        [TestMethod]
        public void ReadVariationTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();

            Assert.AreEqual(0, _pgnReader.CurrentGame.Moves[2].Variation.Count);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation.Count);
            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation[0].Count);
            Assert.AreEqual("Bh5", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Move);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[48].Variation.Count);
            Assert.AreEqual(3, _pgnReader.CurrentGame.Moves[48].Variation[0].Count);
            Assert.AreEqual("gxf3", _pgnReader.CurrentGame.Moves[48].Variation[0][0].Move);
            Assert.AreEqual("Bd3", _pgnReader.CurrentGame.Moves[48].Variation[0][1].Move);
            Assert.AreEqual("b4", _pgnReader.CurrentGame.Moves[48].Variation[0][2].Move);
        }

        [TestMethod]
        public void ReadPreCommentTest()
        {
            StringReader reader = new StringReader(PreCommentGame);
            _pgnReader = new PgnReader(reader, PgnReader.DefaultBufferSize);
            _pgnReader.ReadGame();
            

            Assert.AreEqual("Some comment here", _pgnReader.CurrentGame.Moves[2].Comment);
            Assert.AreEqual("Nf3", _pgnReader.CurrentGame.Moves[2].Move);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation.Count);
            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation[0].Count);
            Assert.AreEqual("Bh5", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Move);
            Assert.AreEqual("Some comment here", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Comment);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[48].Variation.Count);
            Assert.AreEqual(3, _pgnReader.CurrentGame.Moves[48].Variation[0].Count);
            Assert.AreEqual("gxf3", _pgnReader.CurrentGame.Moves[48].Variation[0][0].Move);
            Assert.AreEqual("Some comment here", _pgnReader.CurrentGame.Moves[48].Variation[0][0].Comment);
        }

        [TestMethod]
        public void ReadNestedVariationTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();

            Assert.AreEqual(0, _pgnReader.CurrentGame.Moves[2].Variation.Count);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation.Count);
            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation[0].Count);
            Assert.AreEqual("Bh5", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Move);

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[48].Variation.Count);
            Assert.AreEqual(3, _pgnReader.CurrentGame.Moves[48].Variation[0].Count);
            Assert.AreEqual("gxf3", _pgnReader.CurrentGame.Moves[48].Variation[0][0].Move);
            Assert.AreEqual("Bd3", _pgnReader.CurrentGame.Moves[48].Variation[0][1].Move);
            Assert.AreEqual(0, _pgnReader.CurrentGame.Moves[48].Variation[0][1].Variation.Count);
            Assert.AreEqual("b4", _pgnReader.CurrentGame.Moves[48].Variation[0][2].Move);
            Assert.AreEqual(2, _pgnReader.CurrentGame.Moves[48].Variation[0][2].Variation[0].Count);
            Assert.AreEqual("b3", _pgnReader.CurrentGame.Moves[48].Variation[0][2].Variation[0][0].Move);
            Assert.AreEqual("b6", _pgnReader.CurrentGame.Moves[48].Variation[0][2].Variation[0][1].Move);
        }

        [TestMethod]
        public void ReadAnnotationsTest()
        {
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();
            _pgnReader.ReadGame();

            Assert.AreEqual(1, _pgnReader.CurrentGame.Moves[5].Variation[0].Count);
            Assert.AreEqual("Bb4", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Move);
            Assert.AreEqual("The Winawer Variation is probably best, though not as easy to play.", _pgnReader.CurrentGame.Moves[5].Variation[0][0].Comment);
            Assert.AreEqual("Rc7", _pgnReader.CurrentGame.Moves[47].Variation[0][0].Move);
            Assert.AreEqual("Mark and Fritz7 agree!", _pgnReader.CurrentGame.Moves[47].Variation[0][0].Comment);
            Assert.AreEqual("?", _pgnReader.CurrentGame.Moves[47].Annotation);
            Assert.AreEqual("Black created the weakness (Pa5) and can't quite defend it, so Anand forces a draw.", _pgnReader.CurrentGame.Moves[54].Comment);
        }
    }
}
