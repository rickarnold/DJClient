using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    public class ImageKeywordItem
    {
        public AchievementImage AchievementImage { get; set; }

        public ImageKeywordItem(AchievementImage image)
        {
            this.AchievementImage = image;
        }

        public override string ToString()
        {
            switch (this.AchievementImage)
            {
                case(AchievementImage.Image0):
                    return "Image 0";
                case(AchievementImage.Image1):
                    return "Image 1";
                case (AchievementImage.Image2):
                    return "Image 2";
                case (AchievementImage.Image3):
                    return "Image 3";
                case (AchievementImage.Image4):
                    return "Image 4";
                case (AchievementImage.Image5):
                    return "Image 5";
                case (AchievementImage.Image6):
                    return "Image 6";
                case (AchievementImage.Image7):
                    return "Image 7";
                case (AchievementImage.Image8):
                    return "Image 8";
                case (AchievementImage.Image9):
                    return "Image 9";
            }
            return base.ToString();
        }

        public static AchievementImage GetAchievementImageFromIndex(int index)
        {
            switch (index)
            {
                case (0):
                    return AchievementImage.Image0;
                case (1):
                    return AchievementImage.Image1;
                case (2):
                    return AchievementImage.Image2;
                case (3):
                    return AchievementImage.Image3;
                case (4):
                    return AchievementImage.Image4;
                case (5):
                    return AchievementImage.Image5;
                case (6):
                    return AchievementImage.Image6;
                case (7):
                    return AchievementImage.Image7;
                case (8):
                    return AchievementImage.Image8;
                case (9):
                    return AchievementImage.Image9;
            }

            return AchievementImage.Image0;
        }
    }

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
                case(SelectKeyword.CountGTE):
                    return "Equal/Greater Than";
                case(SelectKeyword.CountLTE):
                    return "Equal/Less Than";
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
