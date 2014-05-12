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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CChessCore.Pgn
{
    public class PgnGame
    {
        public bool HasTags 
        {
            get { return _tagList.Count > 0; }
        }

        public IList<PgnMove> Moves { get { return _moves; } }

        public string Result { get; set; }

        private readonly IList<PgnTag> _tagList;
        private IList<PgnMove> _moves;

        public PgnGame()
        {
            _tagList = new List<PgnTag>();
            _moves = new List<PgnMove>();
        }

        public void AddTag(PgnTag tag)
        {
            var existingTag = _tagList.FirstOrDefault(x => x.Name.Equals(tag.Name));
            if (existingTag == null)
            {
                _tagList.Add(tag);
            }
        }

        public void AddMove(PgnMove _currentMove)
        {
            _moves.Add(_currentMove);
        }

        public void AddMoves(IEnumerable<PgnMove> moves)
        {
            foreach(var move in moves)
            {
                _moves.Add(move);
            }
        }

        public bool TryGetTag(string name, out PgnTag tag)
        {
            tag = _tagList.FirstOrDefault(x => x.Name.Equals(name));
            if (tag == null)
                return false;

            return true;
        }

        public string GetMovesAsPgn()
        {
            StringBuilder sb = new StringBuilder();
            int moveCounter = 1;
            int halfmoveCounter = 1;
            foreach(var move in _moves)
            {
                if(halfmoveCounter % 2 == 1)
                {
                    sb.Append(moveCounter).Append(". ");
                }
                else
                {
                    moveCounter++;
                }

                sb.Append(move.Move);
                
                if(!string.IsNullOrWhiteSpace(move.Annotation))
                    sb.Append(move.Annotation);
                sb.Append(' ');

                if(!string.IsNullOrWhiteSpace(move.Comment))
                    sb.Append(PgnToken.TextCommentBegin.Token).Append(move.Comment).Append(PgnToken.TextCommentEnd.Token).Append(' ');

                if(move.Variation.Count > 0)
                    sb.Append(PgnToken.RecursiveVariationBegin.Token).Append(move.Variation).Append(PgnToken.RecursiveVariationEnd.Token).Append(' ');

                halfmoveCounter++;
            }

            sb.Append(Result);

            return sb.ToString();
        }
    }
}