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

using System.Collections.Generic;
using System.Linq;

namespace CChessCore.Pgn
{
    public class PgnMoves
    {
        public IList<PgnMove> Moves { get; private set; }
        public string Termination { get; set; }
        public string MoveSection { get; set; }

        public PgnMoves() :
            this(string.Empty, new List<PgnMove>(), string.Empty)
        {
        }

        public PgnMoves(string completeMoveSection, IList<PgnMove> moves, string termination)
        {
            MoveSection = completeMoveSection;
            Moves = moves;
            Termination = termination;
        }

        public void AddMove(PgnMove move)
        {
            Moves.Add(move);
        }
    }
}