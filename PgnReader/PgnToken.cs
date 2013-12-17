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

namespace CChessCore.Pgn
{
    public class PgnToken
    {
        public static PgnToken Period = new PgnToken('.');
        public static PgnToken Asterisk = new PgnToken('*');
        public static PgnToken LeftBracket = new PgnToken('[');
        public static PgnToken RightBracket = new PgnToken(']');
        public static PgnToken LeftParenthesis = new PgnToken('{');
        public static PgnToken RightParenthesis = new PgnToken('}');
        public static PgnToken LeftAngleBracket = new PgnToken('<');
        public static PgnToken RightAngleBracket = new PgnToken(']');
        public static PgnToken NumericAnnotationGlyph = new PgnToken('$');
        public static PgnToken RestOfLineComment = new PgnToken(';');
        public static PgnToken EscapeLine = new PgnToken('%');

        private readonly string _token;
        public char Token { get; private set; }

        private PgnToken(char token)
        {
            Token = token;
            _token = token.ToString();
        }

        public override string ToString()
        {
            return _token;
        }
    }
}