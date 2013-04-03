using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    public class SelectKeywordItem
    {
        public SelectKeyword SelectKeyword { get; set; }

        public SelectKeywordItem(SelectKeyword keyword)
        {
            this.SelectKeyword = keyword;
        }

        public override string ToString()
        {
            switch (this.SelectKeyword)
            {
                case(SelectKeyword.CountEqual):
                    return "Equal To";
                case(SelectKeyword.CountGreaterThan):
                    return "Greater Than";
                case(SelectKeyword.CountLessThan):
                    return "Less Than";
                case(SelectKeyword.CountNotEqual):
                    return "Not Equal To";
                case(SelectKeyword.Max):
                    return "Most";
                case(SelectKeyword.Min):
                    return "Least";
                case(SelectKeyword.Newest):
                    return "Latest";
                case(SelectKeyword.Oldest):
                    return "First";
            }
            return base.ToString();
        }
    }

    public class ClauseKeywordItem
    {
        public ClauseKeyword ClauseKeyword { get; set; }

        public ClauseKeywordItem(ClauseKeyword keyword)
        {
            this.ClauseKeyword = keyword;
        }

        public override string ToString()
        {
            switch (this.ClauseKeyword)
            {
                case(ClauseKeyword.Artist):
                    return "Artist";
                case(ClauseKeyword.SongID):
                    return "Song";
                case(ClauseKeyword.Title):
                    return "Title";
            }
            return base.ToString();
        }
    }
}
