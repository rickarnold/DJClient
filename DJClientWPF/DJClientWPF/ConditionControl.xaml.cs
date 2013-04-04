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

        private bool _isEditable = false;

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
        }

        //Get the AchievementSelect object from the control based off the values entered by the user
        public AchievementSelect GetAchievementSelect()
        {
            AchievementSelect select = new AchievementSelect();

            select.clauseKeyword = ((ClauseKeywordItem)ComboBoxType.SelectedItem).ClauseKeyword;
            select.clauseValue = TextBoxTypeValue.Text.Trim();

            select.selectKeyword = ((SelectKeywordItem)ComboBoxQuantifier.SelectedItem).SelectKeyword;
            select.selectValue = NumberPickerQuanitifier.Value.ToString();

            if ((bool)CheckBoxDateStart.IsChecked)
                select.startDate = (DateTime)DatePickerStart.SelectedDate;
            else
                select.startDate = DateTime.MinValue;

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
                isValid = false;
            if ((bool)CheckBoxDateStart.IsChecked && DatePickerStart.SelectedDate == null)
                isValid = false;
            if ((bool)CheckBoxDateEnd.IsChecked && DatePickerEnd.SelectedDate == null)
                isValid = false;

            if (isValid)
                MarkAsValid();
            else
                MarkAsInvalid();

            return isValid;
        }

        //Set the X delete button as invisible
        public void UnallowDelete()
        {
            LabelDelete.Visibility = Visibility.Collapsed;
        }

        //Set the X delete button as visible
        public void AllowDelete()
        {
            LabelDelete.Visibility = Visibility.Visible;
        }

        //Fill in the appropriate AchievementSelect data to the controls
        private void FillInAchievementSelect(AchievementSelect select)
        {
            ComboBoxQuantifier.SelectedValue = select.selectKeyword.ToString();
            NumberPickerQuanitifier.Value = int.Parse(select.selectValue);

            ComboBoxType.SelectedValue = select.clauseKeyword.ToString();
            TextBoxTypeValue.Text = select.clauseValue;

            if (select.startDate.Equals(DateTime.MinValue))
            {
                CheckBoxDateStart.IsChecked = false;
                DatePickerStart.DisplayDate = DateTime.Today;
            }
            else
            {
                CheckBoxDateStart.IsChecked = true;
                DatePickerStart.DisplayDate = select.startDate;
            }

            if (select.endDate.Equals(DateTime.MaxValue))
            {
                CheckBoxDateEnd.IsChecked = false;
                DatePickerEnd.DisplayDate = DateTime.Today;
            }
            else
            {
                CheckBoxDateEnd.IsChecked = true;
                DatePickerEnd.DisplayDate = select.endDate;
            }
        }

        private void MarkAsInvalid()
        {
            //BorderBackground.Background = new LinearGradientBrush(Helper.GetColorFromStirng("#88FF9090"), Helper.GetColorFromStirng("#88FFE0E0"),
            //                                                            new Point(0.5, 0), new Point(0.5, 1));
            //<GradientStop x:Name="GradientStop1" Color="#88909090" Offset="0"/>
            //<GradientStop x:Name="GradientStop2" Color="#88E0E0E0" Offset="1"/>

            //Animate the background to flash red
            ColorAnimation animation = new ColorAnimation();
            animation.From = Helper.GetColorFromString("#88909090");
            animation.To = Helper.GetColorFromString("#88FF9090");
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.AutoReverse = true;

            ColorAnimation animation2 = new ColorAnimation();
            animation2.From = Helper.GetColorFromString("#88E0E0E0");
            animation2.To = Helper.GetColorFromString("#88FFE0E0");
            animation2.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation2.AutoReverse = true;

            Storyboard s = new Storyboard();
            s.Duration = new Duration(new TimeSpan(0, 0, 1));
            s.Children.Add(animation);
            s.Children.Add(animation2);

            Storyboard.SetTarget(animation, GradientStop1);
            Storyboard.SetTargetProperty(animation, new PropertyPath("GradientStop.Color"));//GradientStop.ColorProperty));

            Storyboard.SetTarget(animation2, GradientStop2);
            Storyboard.SetTargetProperty(animation2, new PropertyPath("GradientStop.Color"));//(GradientStop.ColorProperty));

            s.Begin(this);
        }

        private void MarkAsValid()
        {
            BorderBackground.Background = new LinearGradientBrush(Helper.GetColorFromString("#88909090"), Helper.GetColorFromString("#88E0E0E0"),
                                                                        new Point(0.5, 0), new Point(0.5, 1));
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
                    foreach (SongSearchResult result in searchList)
                    {
                        string r = result.MainResult + " - " + result.SecondaryResult;
                        if (!resultList.Contains(r))
                            resultList.Add(r);
                    }
                    break;
            }

            resultList.Sort();
            TextBoxTypeValue.ItemsSource = resultList;

            e.Handled = true;
        }

        #endregion      
    }
}
