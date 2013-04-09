using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DJClientWPF.KaraokeService;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace DJClientWPF
{
    /// <summary>
    /// Control that displays a condition statement for an achievement
    /// </summary>
    public partial class ConditionControl : UserControl
    {
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler DeleteControl;

        private ObservableCollection<SelectKeywordItem> selectKeywordList;
        private ObservableCollection<ClauseKeywordItem> clauseKeywordList;

        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
                if (_isEditable)
                    SetControlsAsEditable();
                else
                    SetControlsAsNotEditable();
            }
        }

        private AchievementSelect _select;
        private bool _isEditable = false;
        private bool _allowDelete = false;

        //Constructor for a new AchievementSelect
        public ConditionControl()
        {
            InitializeComponent();

            selectKeywordList = new ObservableCollection<SelectKeywordItem>();
            clauseKeywordList = new ObservableCollection<ClauseKeywordItem>();

            FillTypeCombobox();
            FillQuanitifierCombobox();
        }

        //Constructor for displaying a previous AcheivementSelect object
        public ConditionControl(AchievementSelect select)
        {
            InitializeComponent();

            selectKeywordList = new ObservableCollection<SelectKeywordItem>();
            clauseKeywordList = new ObservableCollection<ClauseKeywordItem>();

            FillTypeCombobox();
            FillQuanitifierCombobox();

            FillInAchievementSelect(select);

            _select = select;
        }

        //Get the AchievementSelect object from the control based off the values entered by the user
        public AchievementSelect GetAchievementSelect()
        {
            AchievementSelect select = new AchievementSelect();

            select.clauseKeyword = ((ClauseKeywordItem)ComboBoxType.SelectedItem).ClauseKeyword;
            if (select.clauseKeyword == ClauseKeyword.SongID)
                select.clauseValue = ((SongSearchResult)TextBoxTypeValue.SelectedItem).Song.ID.ToString();
            else
                select.clauseValue = TextBoxTypeValue.Text.Trim();

            select.selectKeyword = ((SelectKeywordItem)ComboBoxQuantifier.SelectedItem).SelectKeyword;
            select.selectValue = NumberPickerQuanitifier.Value.ToString();


            if ((bool)CheckBoxDateStart.IsChecked)
                select.startDate = (DateTime)DatePickerStart.SelectedDate;
            else
                select.startDate = new DateTime(1980, 1, 1);

            if ((bool)CheckBoxDateEnd.IsChecked)
                select.endDate = (DateTime)DatePickerEnd.SelectedDate;
            else
                select.endDate = DateTime.MaxValue;

            return select;
        }

        //Returns true if the user has entered enough information to create a valid AchievementSelect object
        public bool IsInputValid()
        {
            bool isValid = true;

            if (ComboBoxQuantifier.SelectedItem == null)
                isValid = false;
            if (ComboBoxType.SelectedItem == null)
                isValid = false;
            if (TextBoxTypeValue.Text.Trim().Equals(""))
            {
                isValid = false;
                ShowItemAsError(TextBoxTypeValue);
            }
            if ((bool)CheckBoxDateStart.IsChecked && DatePickerStart.SelectedDate == null)
            {
                isValid = false;
                ShowItemAsError(DatePickerStart);
            }
            if ((bool)CheckBoxDateEnd.IsChecked && DatePickerEnd.SelectedDate == null)
            {
                isValid = false;
                ShowItemAsError(DatePickerEnd);
            }

            return isValid;
        }

        //Set the X delete button as invisible
        public void UnallowDelete()
        {
            LabelDelete.Visibility = Visibility.Collapsed;
            _allowDelete = false;
        }

        //Set the X delete button as visible
        public void AllowDelete()
        {
            LabelDelete.Visibility = Visibility.Visible;
            _allowDelete = true;
        }

        //Fill in the appropriate AchievementSelect data to the controls
        private void FillInAchievementSelect(AchievementSelect select)
        {
            ComboBoxQuantifier.SelectedIndex = GetQuantifierIndex(select.selectKeyword);
            NumberPickerQuanitifier.Value = int.Parse(select.selectValue);

            ComboBoxType.SelectedIndex = GetClauseIndex(select.clauseKeyword);
            if (select.clauseKeyword == ClauseKeyword.SongID)
            {
                int songID = int.Parse(select.clauseValue);
                Song song = DJModel.Instance.SongDictionary[songID];
                List<SongSearchResult> itemList = new List<SongSearchResult>();
                SongSearchResult result = new SongSearchResult(song, song.artist, song.title);
                itemList.Add(result);
                TextBoxTypeValue.ItemsSource = itemList;
                TextBoxTypeValue.SelectedItem = result;
            }
            else
                TextBoxTypeValue.Text = select.clauseValue;

            if (select.startDate.Equals(new DateTime(1980, 1, 1)))
            {
                CheckBoxDateStart.IsChecked = false;
                DatePickerStart.DisplayDate = DateTime.Today;
                DatePickerStart.SelectedDate = null;
            }
            else
            {
                CheckBoxDateStart.IsChecked = true;
                DatePickerStart.SelectedDate = select.startDate;
            }

            if (select.endDate.Equals(DateTime.MaxValue))
            {
                CheckBoxDateEnd.IsChecked = false;
                DatePickerEnd.DisplayDate = DateTime.Today;
                DatePickerEnd.SelectedDate = null;
            }
            else
            {
                CheckBoxDateEnd.IsChecked = true;
                DatePickerEnd.SelectedDate = select.endDate;
            }
        }

        //Flash the background color of a control to red and back to white to show an error
        private void ShowItemAsError(UIElement textBox)
        {
            ColorAnimation animation = new ColorAnimation();
            animation.From = Colors.White;
            animation.To = Color.FromArgb(255, 255, 125, 125);
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.AutoReverse = true;

            Storyboard s = new Storyboard();
            s.Duration = new Duration(new TimeSpan(0, 0, 1));
            s.Children.Add(animation);

            Storyboard.SetTarget(animation, textBox);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));

            s.Begin();
        }

        #region Combobox Methods

        private void FillQuanitifierCombobox()
        {
            foreach (SelectKeyword keyword in Enum.GetValues(typeof(SelectKeyword)))
                selectKeywordList.Add(new SelectKeywordItem(keyword));

            ComboBoxQuantifier.ItemsSource = selectKeywordList;

            ComboBoxQuantifier.SelectedIndex = 0;
        }

        private void FillTypeCombobox()
        {
            foreach (ClauseKeyword keyword in Enum.GetValues(typeof(ClauseKeyword)))
                clauseKeywordList.Add(new ClauseKeywordItem(keyword));

            ComboBoxType.ItemsSource = clauseKeywordList;

            ComboBoxType.SelectedIndex = 0;
        }

        //User has changed the keyword type of the condition.  Need to update the auto complete box for the value
        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Clear out the old values
            TextBoxTypeValue.Text = "";
        }

        private int GetQuantifierIndex(SelectKeyword keyword)
        {
            switch (keyword)
            {
                case (SelectKeyword.CountGTE):
                    return 0;
                case (SelectKeyword.CountLTE):
                    return 1;
                case (SelectKeyword.Max):
                    return 2;
                case (SelectKeyword.Min):
                    return 3;
                case (SelectKeyword.Newest):
                    return 4;
                case (SelectKeyword.Oldest):
                    return 5;
            }
            return 0;
        }

        private int GetClauseIndex(ClauseKeyword keyword)
        {
            switch (keyword)
            {
                case (ClauseKeyword.Artist):
                    return 0;
                case (ClauseKeyword.Title):
                    return 1;
                case (ClauseKeyword.SongID):
                    return 2;
            }
            return 0;
        }

        #endregion

        #region Editable Methods

        private void SetControlsAsEditable()
        {
            ComboBoxQuantifier.IsEnabled = true;
            NumberPickerQuanitifier.IsEnabled = true;
            ComboBoxType.IsEnabled = true;
            TextBoxTypeValue.IsEnabled = true;
            CheckBoxDateStart.IsEnabled = true;
            CheckBoxDateEnd.IsEnabled = true;

            if ((bool)CheckBoxDateStart.IsChecked)
                DatePickerStart.IsEnabled = true;
            if ((bool)CheckBoxDateEnd.IsChecked)
                DatePickerEnd.IsEnabled = true;

            if (_allowDelete)
                LabelDelete.Visibility = Visibility.Visible;
            else
                LabelDelete.Visibility = Visibility.Collapsed;
        }

        private void SetControlsAsNotEditable()
        {
            ComboBoxQuantifier.IsEnabled = false;
            NumberPickerQuanitifier.IsEnabled = false;
            ComboBoxType.IsEnabled = false;
            TextBoxTypeValue.IsEnabled = false;
            CheckBoxDateStart.IsEnabled = false;
            CheckBoxDateEnd.IsEnabled = false;
            DatePickerStart.IsEnabled = false;
            DatePickerEnd.IsEnabled = false;

            LabelDelete.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region GUI Methods

        private void CheckBoxDateStart_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBoxDateStart.IsChecked)
                DatePickerStart.IsEnabled = true;
            else
                DatePickerStart.IsEnabled = false;
        }

        private void CheckBoxDateEnd_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBoxDateEnd.IsChecked)
                DatePickerEnd.IsEnabled = true;
            else
                DatePickerEnd.IsEnabled = false;
        }

        //User has clicked the delete button
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DeleteControl != null)
                DeleteControl(this, new EventArgs());
        }

        #endregion

        #region AutoCompleteBox Methods

        private void TextBoxTypeValue_Populating(object sender, PopulatingEventArgs e)
        {
            if (TextBoxTypeValue.Text.Length != 3)
            {
                e.Handled = true;
                return;
            }

            ClauseKeyword keyword = ((ClauseKeywordItem)ComboBoxType.SelectedItem).ClauseKeyword;
            string term = TextBoxTypeValue.Text.Trim();

            List<SongSearchResult> searchList;
            List<String> resultList = new List<string>();

            switch (keyword)
            {
                case (ClauseKeyword.Artist):
                    searchList = DJModel.Instance.GetMatchingArtistsInSongbook(term);
                    foreach (SongSearchResult result in searchList)
                    {
                        string r = result.MainResult;
                        if (!resultList.Contains(r))
                            resultList.Add(r);
                    }
                    break;
                case (ClauseKeyword.Title):
                    searchList = DJModel.Instance.GetMatchingTitlesInSongbook(term);
                    foreach (SongSearchResult result in searchList)
                    {
                        string r = result.MainResult;
                        if (!resultList.Contains(r))
                            resultList.Add(r);
                    }
                    break;
                case (ClauseKeyword.SongID):
                    searchList = DJModel.Instance.GetMatchingSongsInSongbook(term);
                    TextBoxTypeValue.ItemsSource = searchList;
                    return;
            }

            resultList.Sort();
            TextBoxTypeValue.ItemsSource = resultList;

            e.Handled = true;
        }

        #endregion
    }
}
