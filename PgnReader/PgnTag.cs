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