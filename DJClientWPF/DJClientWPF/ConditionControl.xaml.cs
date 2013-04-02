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

namespace DJClientWPF
{
    /// <summary>
    /// Control that displays a condition statement for an achievement
    /// </summary>
    public partial class ConditionControl : UserControl
    {
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

            FillTypeCombobox();
            FillQuanitifierCombobox();
        }

        //Constructor for displaying a previous AcheivementSelect object
        public ConditionControl(AchievementSelect select)
        {
            InitializeComponent();

            FillTypeCombobox();
            FillQuanitifierCombobox();

            FillInAchievementSelect(select);
        }

        //Get the AchievementSelect object from the control based off the values entered by the user
        public AchievementSelect GetAchievementSelect()
        {
            AchievementSelect select = new AchievementSelect();

            

            return select;
        }

        //Returns true if the user has entered enough information to create a valid AchievementSelect object
        public bool IsInputValid()
        {
            bool isValid = true;

            if (isValid)
                MarkAsValid();
            else
                MarkAsInvalid();

            return isValid;
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
            GridMain.Background = new SolidColorBrush(Color.FromArgb(100, 255, 170, 170));
        }

        private void MarkAsValid()
        {
            GridMain.Background = new SolidColorBrush(Colors.White);
        }

        #region Combobox Methods

        private void FillQuanitifierCombobox()
        {
            string[] selectKeywords = Enum.GetNames(typeof(SelectKeyword));

            ComboBoxQuantifier.ItemsSource = selectKeywords;
        }

        private void FillTypeCombobox()
        {
            string[] clauseKeywords = Enum.GetNames(typeof(ClauseKeyword));

            ComboBoxType.ItemsSource = clauseKeywords;
        }

        //User has changed the keyword type of the condition.  Need to update the auto complete box for the value
        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        #region AutoCompleteBox Methods

        private void TextBoxTypeValue_Populating(object sender, PopulatingEventArgs e)
        {

        }

        #endregion
    }
}
