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
    public class PgnTag
    {
        public const string Event = "Event";
        public const string Site = "Site";
        public const string White = "White";
        public const string Black = "Black";
        public const string Result = "Result";
        public const string Date = "Date";

        public string Name { get; private set; }

        public string Value { get; private set; }

        private PgnTag(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static PgnTag Parse(string field)
        {
            field.Trim();
            int start = 0;
            int end = 0;
            if (field.StartsWith(PgnToken.LeftBracket.ToString()))
            {
                start = 1;
            }
            if (field.EndsWith(PgnToken.RightBracket.ToString()))
            {
                end = 1;
            }
            var inner = field.Substring(start, field.Length - start - end);

            var tagNameLength = inner.IndexOf(' ');
            if (tagNameLength < 0)
                throw new PgnReaderException("empty pgn tag: " + field);

            var tagName = inner.Substring(0, tagNameLength).Trim();
            var tagValue = inner.Substring(tagNameLength).Trim();
            start = 0;
            end = 0;
            if (tagValue.StartsWith(@""""))
            {
                start = 1;
            }
            if (tagValue.EndsWith(@""""))
            {
                end = 1;
            }
            tagValue = tagValue.Substring(start, tagValue.Length - start - end);

            return new PgnTag(tagName, tagValue);
        }
    }
}