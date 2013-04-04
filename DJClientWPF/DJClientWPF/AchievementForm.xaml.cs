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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    /// <summary>
    /// Form that allows the user to view and edit current achievements as well as create new achievements
    /// </summary>
    public partial class AchievementForm : Window
    {
        public const string IMAGE_PATH = @"..\..\Images\";  //Path where achievement images are stored

        //Lists used for binding to list boxes
        ObservableCollection<Achievement> achievementList;
        ObservableCollection<ConditionControl> currentControlsList;
        ObservableCollection<ConditionControl> newControlsList;

        DJModel model = DJModel.Instance;
        private bool _isEditing = false;

        public AchievementForm()
        {
            InitializeComponent();

            //Set up the event handlers for calls to the model
            model.GetAchievementsComplete += GetAchievementsCompleteHandler;
            model.EditAchievementComplete += EditAchievementCompleteHandler;
            model.DeleteAchievementComplete += DeleteAchievementCompleteHandler;
            model.CreateAchievementComplete += CreateAchievementCompleteHandler;

            //Initialize all lists used for binding with listboxes and bind them
            achievementList = new ObservableCollection<Achievement>();
            currentControlsList = new ObservableCollection<ConditionControl>();
            newControlsList = new ObservableCollection<ConditionControl>();

            ListBoxCurrentAchievements.ItemsSource = achievementList;
            ListBoxAddNewSelectControls.ItemsSource = newControlsList;
            ListBoxCurrentSelectControls.ItemsSource = currentControlsList;

            //Set up the GUI for being run the first time
            ResetNewAchievementConditionControls();

            LoadImageComboboxes();

            //Get the list of all achievements
            model.GetAllAchievements();
        }

        #region Model Event Handlers

        //Model has retrieved list of all achievements from the service
        private void GetAchievementsCompleteHandler(object source, DJModelArgs args)
        {
            UpdateCurrentAchievementList();

            //Set the first achievement as selected
            if (achievementList.Count > 0)
                ListBoxCurrentAchievements.SelectedIndex = 0;
        }

        //Model has finished editing this achievement
        private void EditAchievementCompleteHandler(object source, DJModelArgs args)
        {
            UpdateCurrentAchievementList();

            //Edit complete so exit edit mode
            MakeAchievementUneditable();
            EnableEditControls();
            _isEditing = false;
        }

        //Model has finished deleting the given achievement, get the updated achievement list
        private void DeleteAchievementCompleteHandler(object source, DJModelArgs args)
        {
            UpdateCurrentAchievementList();

            ButtonCurrentDelete.IsEnabled = true;
            ButtonCurrentEdit.IsEnabled = true;
            ListBoxCurrentAchievements.IsEnabled = true;
        }

        //Model has finished creating the new achievement
        private void CreateAchievementCompleteHandler(object source, DJModelArgs args)
        {
            UpdateCurrentAchievementList();

            ClearNewAchievementForm();
        }

        #endregion

        #region Current Achievement Methods

        //User clicked on an achievement to display
        private void ListBoxCurrentAchievements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Achievement achievement = ListBoxCurrentAchievements.SelectedItem as Achievement;
            DisplayAchievement(achievement);
        }

        //Given an achievement object, fill in the appropriate fields of the current achievement form
        private void DisplayAchievement(Achievement achievement)
        {
            TextBoxCurrentDescription.Text = achievement.description;
            LabelCurrentDescription.Content = achievement.description;
            TextBoxCurrentName.Text = achievement.name;
            LabelCurrentName.Content = achievement.name;
            if (achievement.statementsAnd)
            {
                ComboboxCurrentAndOr.SelectedIndex = 0;
                LabelCurrentAndOr.Content = "AND";
            }
            else
            {
                ComboboxCurrentAndOr.SelectedIndex = 1;
                LabelCurrentAndOr.Content = "OR";
            }

            ComboBoxCurrentImage.SelectedIndex = GetIndexFromAchievementImage(achievement.image);
            
            CheckBoxCurrentPublic.IsChecked = achievement.visible;

            //Create condition controls as well
            currentControlsList.Clear();
            foreach (AchievementSelect select in achievement.selectList)
            {
                ConditionControl control = new ConditionControl(select);
                control.IsEditable = _isEditing;
                control.DeleteControl += CurrentConditionControlDeleted;

                if (achievement.selectList.Length > 1)
                    control.AllowDelete();
                else
                    control.UnallowDelete();
            }
        }

        //User clicked to delete the currently selected achievement
        private void ButtonCurrentDelete_Click(object sender, RoutedEventArgs e)
        {
            Achievement achievement = (Achievement)ListBoxCurrentAchievements.SelectedItem;

            //Ensure that the user wishes to delete this achievement
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to delete the following achievement:\n\n\t" +
                                                        achievement.name + "\n\t" + achievement.description, "Confirm Delete", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                model.DeleteAchievement(achievement);
                ButtonCurrentDelete.IsEnabled = false;
                ButtonCurrentEdit.IsEnabled = false;
                ListBoxCurrentAchievements.IsEnabled = false;
            }
        }

        #endregion

        #region Edit Achievement Methods

        //Obtain the achievement object from the current achievement edit form
        private Achievement GetEdittedAchievement()
        {
            Achievement achievement = (Achievement)ListBoxCurrentAchievements.SelectedItem;

            achievement.name = TextBoxCurrentName.Text.Trim();
            achievement.description = TextBoxCurrentDescription.Text.Trim();

            achievement.image = ImageKeywordItem.GetAchievementImageFromIndex(ComboBoxCurrentImage.SelectedIndex);

            if (ComboboxCurrentAndOr.SelectedIndex == 0)
                achievement.statementsAnd = true;
            else
                achievement.statementsAnd = false;

            achievement.selectList = new AchievementSelect[currentControlsList.Count];
            for (int x = 0; x < currentControlsList.Count; x++)
                achievement.selectList[x] = currentControlsList[x].GetAchievementSelect();

            return achievement;
        }

        //User wants to edit the currently selected achievement
        private void ButtonCurrentEdit_Click(object sender, RoutedEventArgs e)
        {
            MakeAchievementEditable();
            EnableEditControls();
            _isEditing = true;
        }

        //User clicked to submit the edit changes that were made to the achievement
        private void ButtonCurrentSubmit_Click(object sender, RoutedEventArgs e)
        {
            //Check that all edited condition controls contain valid information, if not do not submit changes
            bool allConditionsValid = true;
            foreach (ConditionControl control in currentControlsList)
            {
                if (!control.IsInputValid())
                    allConditionsValid = false;
            }

            if (!allConditionsValid)
                return;

            //Get the new achievement from the form and submit to the server
            Achievement achievement = GetEdittedAchievement();
            model.EditAchievement(achievement);

            DisableEditControls();
        }

        //User clicked to cancel editting the current achievement
        private void ButtonCurrentCancel_Click(object sender, RoutedEventArgs e)
        {
            MakeAchievementUneditable();
            _isEditing = false;

            //Redisplay the achievement which should erase any unsaved changes
            DisplayAchievement(ListBoxCurrentAchievements.SelectedItem as Achievement);
        }

        //User cliked label to add a new conditional control
        private void ButtonCurrentAddControl_Click(object sender, RoutedEventArgs e)
        {
            ConditionControl newControl = new ConditionControl();
            newControl.DeleteControl += CurrentConditionControlDeleted;
            currentControlsList.Insert(0, newControl);

            //At least 2 controls exist so enable deleting
            foreach (ConditionControl control in currentControlsList)
                control.AllowDelete();
        }

        //User clicked on the delete button of a condtion control.  Remove it from the list.
        private void CurrentConditionControlDeleted(object sender, EventArgs args)
        {
            //Don't allow the last control to be deleted
            if (currentControlsList.Count > 1)
            {
                ConditionControl control = (ConditionControl)sender;
                currentControlsList.Remove(control);

                //If only one control is left do not allow it to be deleted
                if (currentControlsList.Count == 1)
                    currentControlsList[0].UnallowDelete();
            }
        }

        //User has clicked the label to add another conditional control while editing an achievement
        private void CurrentAddLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConditionControl newControl = new ConditionControl();
            newControl.DeleteControl += CurrentConditionControlDeleted;
            currentControlsList.Insert(0, newControl);

            //There are now at least 2 controls so they can now be deleted
            foreach (ConditionControl control in newControlsList)
                control.AllowDelete();
        }

        //User has changed the image to be displayed while editing the current achievement
        private void ComboBoxCurrentImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboBoxCurrentImage.SelectedIndex;

            switch (index)
            {
                case (0):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image0.png");
                    break;
                case (1):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image1.png");
                    break;
                case (2):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image2.png");
                    break;
                case (3):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image3.png");
                    break;
                case (4):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image4.png");
                    break;
                case (5):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image5.png");
                    break;
                case (6):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image6.png");
                    break;
                case (7):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image7.png");
                    break;
                case (8):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image8.png");
                    break;
                case (9):
                    ImageCurrent.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image9.png");
                    break;
            }
        }

        //Make the currently selected achievement editable by displaying the textboxes and hiding the labels
        private void MakeAchievementEditable()
        {
            TextBoxCurrentDescription.Visibility = Visibility.Visible;
            TextBoxCurrentName.Visibility = Visibility.Visible;
            ComboboxCurrentAndOr.Visibility = Visibility.Visible;
            ComboBoxCurrentImage.Visibility = Visibility.Visible;

            LabelCurrentAndOr.Visibility = Visibility.Collapsed;
            LabelCurrentDescription.Visibility = Visibility.Collapsed;
            LabelCurrentName.Visibility = Visibility.Collapsed;

            CheckBoxCurrentPublic.IsEnabled = true;

            ListBoxCurrentAchievements.IsEnabled = false;

            BorderCurrentAdd.Visibility = Visibility.Visible;
            LabelCurrentAdd.Visibility = Visibility.Visible;

            ButtonCurrentEdit.Visibility = Visibility.Collapsed;
            ButtonCurrentDelete.Visibility = Visibility.Collapsed;
            ButtonCurrentCancel.Visibility = Visibility.Visible;
            ButtonCurrentSubmit.Visibility = Visibility.Visible;

            //Set the conditon controls as editable as well
            foreach (ConditionControl control in currentControlsList)
                control.IsEditable = true;
        }

        //Make the currently selected achievement uneditable
        private void MakeAchievementUneditable()
        {
            TextBoxCurrentDescription.Visibility = Visibility.Collapsed;
            TextBoxCurrentName.Visibility = Visibility.Collapsed;
            ComboboxCurrentAndOr.Visibility = Visibility.Collapsed;
            ComboBoxCurrentImage.Visibility = Visibility.Collapsed;

            LabelCurrentAndOr.Visibility = Visibility.Visible;
            LabelCurrentDescription.Visibility = Visibility.Visible;
            LabelCurrentName.Visibility = Visibility.Visible;

            CheckBoxCurrentPublic.IsEnabled = false;

            ListBoxCurrentAchievements.IsEnabled = true;

            BorderCurrentAdd.Visibility = Visibility.Collapsed;
            LabelCurrentAdd.Visibility = Visibility.Collapsed;

            ButtonCurrentEdit.Visibility = Visibility.Visible;
            ButtonCurrentDelete.Visibility = Visibility.Visible;
            ButtonCurrentCancel.Visibility = Visibility.Collapsed;
            ButtonCurrentSubmit.Visibility = Visibility.Collapsed;

            //Set the conditon controls as uneditable as well
            foreach (ConditionControl control in currentControlsList)
                control.IsEditable = false;
        }

        //Make all the edit controls not enabled while waiting for edit to propogate on the server
        private void DisableEditControls()
        {
            TextBoxCurrentDescription.IsEnabled = false;
            TextBoxCurrentName.IsEnabled = false;
            ComboboxCurrentAndOr.IsEnabled = false;
            ComboBoxCurrentImage.IsEnabled = false;
            CheckBoxCurrentPublic.IsEnabled = false;
            ButtonCurrentEdit.IsEnabled = false;
            ButtonCurrentDelete.IsEnabled = false;
            ButtonCurrentCancel.IsEnabled = false;
            ButtonCurrentSubmit.IsEnabled = false;
        }

        //Enable the edit controls once the call to edit the achievement has returned from the server
        private void EnableEditControls()
        {
            TextBoxCurrentDescription.IsEnabled = true;
            TextBoxCurrentName.IsEnabled = true;
            ComboboxCurrentAndOr.IsEnabled = true;
            ComboBoxCurrentImage.IsEnabled = true;
            CheckBoxCurrentPublic.IsEnabled = true;
            ButtonCurrentEdit.IsEnabled = true;
            ButtonCurrentDelete.IsEnabled = true;
            ButtonCurrentCancel.IsEnabled = true;
            ButtonCurrentSubmit.IsEnabled = true;
        }

        #endregion

        #region New Achievement Methods

        //User wants to submit for creation the newly created achievement
        private void ButtonAddNewAdd_Click(object sender, RoutedEventArgs e)
        {
            //Check that each of the condition controls is valid.  If not do not submit the achievement
            bool allValid = true;
            foreach (ConditionControl control in newControlsList)
            {
                if (!control.IsInputValid())
                    allValid = false;
            }

            Achievement achievement = new Achievement();

            //Get a list of conditional control AchievementSelect objects
            achievement.selectList = new AchievementSelect[newControlsList.Count];
            for (int x = 0; x < newControlsList.Count; x++)
            {
                achievement.selectList[x] = newControlsList[x].GetAchievementSelect();
            }

            achievement.name = TextBoxAddNewName.Text.Trim();
            achievement.description = TextBoxAddNewDescription.Text.Trim();

            achievement.image = ImageKeywordItem.GetAchievementImageFromIndex(ComboBoxAddNewImage.SelectedIndex);

            if (ComboBoxAddNewAndOr.SelectedIndex == 0)
                achievement.statementsAnd = true;
            else
                achievement.statementsAnd = false;

            achievement.visible = (bool)CheckBoxAddNewPublic.IsChecked;

            if (!allValid)
                return;
        }

        //User clicked to add a new condition to the new achievement condition list
        private void AddLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConditionControl newControl = new ConditionControl();
            newControl.AllowDelete();
            newControl.DeleteControl += NewConditionControlDeleted;
            newControlsList.Insert(0, newControl);

            foreach (ConditionControl control in newControlsList)
                control.AllowDelete();
        }

        //User has changed the image that is used for the new achievement
        private void ComboBoxAddNewImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboBoxAddNewImage.SelectedIndex;

            switch (index)
            {
                case(0):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image0.png");
                    break;
                case (1):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image1.png");
                    break;
                case (2):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image2.png");
                    break;
                case (3):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image3.png");
                    break;
                case (4):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image4.png");
                    break;
                case (5):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image5.png");
                    break;
                case (6):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image6.png");
                    break;
                case (7):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image7.png");
                    break;
                case (8):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image8.png");
                    break;
                case (9):
                    ImageAddNew.Source = Helper.OpenBitmapImage(IMAGE_PATH + @"Image9.png");
                    break;
            }
        }

        //User clicked to delete a condition control
        private void NewConditionControlDeleted(object sender, EventArgs args)
        {
            //Make sure that the last control is not deleted
            if (newControlsList.Count > 1)
            {
                ConditionControl control = (ConditionControl)sender;
                newControlsList.Remove(control);

                //If only one control is left do not allow it to be deleted
                if (newControlsList.Count == 1)
                    newControlsList[0].UnallowDelete();
            }
        }

        //Clear out any condition controls and add a new blank condition control
        private void ResetNewAchievementConditionControls()
        {
            ConditionControl control = new ConditionControl();
            control.UnallowDelete();
            control.DeleteControl += NewConditionControlDeleted;

            newControlsList.Clear();
            newControlsList.Add(control);
        }

        //Clear out data in the new achievement form and reset it for the next achievement
        private void ClearNewAchievementForm()
        {
            TextBoxAddNewDescription.Text = "";
            TextBoxAddNewName.Text = "";
            ComboBoxAddNewAndOr.SelectedIndex = 0;
            ComboBoxAddNewImage.SelectedIndex = 0;
            CheckBoxAddNewPublic.IsChecked = true;

            ResetNewAchievementConditionControls();
        }

        #endregion        

        #region Private Helper Methods

        //Load all the comboboxes that deal with achievement images
        private void LoadImageComboboxes()
        {
            List<ImageKeywordItem> imageList = new List<ImageKeywordItem>();

            foreach (AchievementImage keyword in Enum.GetValues(typeof(AchievementImage)))
                imageList.Add(new ImageKeywordItem(keyword));

            ComboBoxAddNewImage.ItemsSource = imageList;
            ComboBoxCurrentImage.ItemsSource = imageList;

            ComboBoxAddNewImage.SelectedIndex = 0;
        }

        //Gets the current list of achievements from the model and updates the current achievements
        private void UpdateCurrentAchievementList()
        {
            //Add any achievements that are not in the list
            foreach (Achievement achievement in model.AchievementList)
            {
                if (!achievementList.Contains(achievement))
                    achievementList.Add(achievement);
            }

            List<Achievement> achievementsToDelete = new List<Achievement>();

            //Remove any achievements that are not in the list anymore
            foreach (Achievement achievement in achievementList)
            {
                if (!model.AchievementList.Contains(achievement))
                    achievementsToDelete.Add(achievement);
            }

            foreach (Achievement toDelete in achievementsToDelete)
                achievementList.Remove(toDelete);
        }

        //Given an AchievementImage enum, get the index that it should reside at in the combobox
        private int GetIndexFromAchievementImage(AchievementImage imageEnum)
        {
            switch (imageEnum)
            {
                case (AchievementImage.Image0):
                    return 0;
                case (AchievementImage.Image1):
                    return 1;
                case (AchievementImage.Image2):
                    return 2;
                case (AchievementImage.Image3):
                    return 3;
                case (AchievementImage.Image4):
                    return 4;
                case (AchievementImage.Image5):
                    return 5;
                case (AchievementImage.Image6):
                    return 6;
                case (AchievementImage.Image7):
                    return 7;
                case (AchievementImage.Image8):
                    return 8;
                case (AchievementImage.Image9):
                    return 9;
            }

            return 0;
        }

        #endregion
    }
}
