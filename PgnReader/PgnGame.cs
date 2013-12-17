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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CChessCore.Pgn
{
    public class PgnGame
    {
        public bool HasTags 
        {
            get { return _tagList.Count > 0; }
        }

        public PgnMoves Moves { get { return _moves; } }

        private readonly IList<PgnTag> _tagList;
        private PgnMoves _moves;

        public PgnGame()
        {
            _tagList = new List<PgnTag>();    
        }

        public void AddTag(PgnTag tag)
        {
            var existingTag = _tagList.FirstOrDefault(x => x.Name.Equals(tag.Name));
            if (existingTag == null)
            {
                _tagList.Add(tag);
            }
        }

        public void AddMoves(PgnMoves moves)
        {
            _moves = moves;
        }

        public bool TryGetTag(string name, out PgnTag tag)
        {
            tag = _tagList.FirstOrDefault(x => x.Name.Equals(name));
            if (tag == null)
                return false;

            return true;
        }
    }
}