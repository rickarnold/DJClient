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

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for Achivements.xaml
    /// </summary>
    public partial class Achivements : Window
    {
        ObservableCollection<ConditionControl> currentControlsList;
        ObservableCollection<ConditionControl> newControlsList;

        public Achivements()
        {
            InitializeComponent();

            currentControlsList = new ObservableCollection<ConditionControl>();
            newControlsList = new ObservableCollection<ConditionControl>();

            ListBoxAddNewSelectControls.ItemsSource = newControlsList;
            ListBoxCurrentSelectControls.ItemsSource = currentControlsList;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region Current Achievement Methods


        #endregion


        #region Edit Achievement Methods


        #endregion

        #region New Achievement Methods


        #endregion

        //User wants to edit the currently selected achievement
        private void ButtonCurrentEdit_Click(object sender, RoutedEventArgs e)
        {

        }

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

            if (!allValid)
                return;
        }

        private void ButtonCurrentAddControl_Click(object sender, RoutedEventArgs e)
        {
            ConditionControl newControl = new ConditionControl();
            newControl.DeleteControl += CurrentConditionControlDeleted;
            currentControlsList.Insert(0, newControl);
        }

        private void ButtonAddNewAddControl_Click(object sender, RoutedEventArgs e)
        {
            ConditionControl newControl = new ConditionControl();
            newControl.DeleteControl += NewConditionControlDeleted;
            newControlsList.Insert(0, newControl);
        }

        private void CurrentConditionControlDeleted(object sender, EventArgs args)
        {
            ConditionControl control = (ConditionControl)sender;
            currentControlsList.Remove(control);
        }

        private void NewConditionControlDeleted(object sender, EventArgs args)
        {
            ConditionControl control = (ConditionControl)sender;
            newControlsList.Remove(control);
        }
    }
}
