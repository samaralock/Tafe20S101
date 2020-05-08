using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StartFinance.Models;
using SQLite.Net;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        string fmt = "dd MMM yyyy";
        int editAppID;
        public AppointmentPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointment>();
            var query = conn.Table<Appointment>();
            AppointmentList.ItemsSource = query.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // checks if event name is null
                if (
                    ((EventName.Text.ToString() == "") || (EventName.Text.ToString() == null)) ||
                    ((EventLocation.Text.ToString() == "") || (EventName.Text.ToString() == null)) ||
                    (EventDate.SelectedDate == null) ||
                    (StartTime.SelectedTime == null) ||
                    (EndTime.SelectedTime == null)
                    )
                {
                    MessageDialog dialog = new MessageDialog("One of your fields has not been filled", "Oops..!");
                    await dialog.ShowAsync();
                }

                else
                {   // Inserts the data
                    conn.Insert(new Appointment()
                    {
                        EventName = EventName.Text,
                        Location = EventLocation.Text,
                        EventDate = EventDate.Date.ToString(fmt),
                        StartTime = StartTime.Time.ToString(),
                        EndTime = EndTime.Time.ToString()
                    });

                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when EventName is invalid
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter info for one of the fields", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Something went wrong - Appointment not added", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }


            }

        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {       //captures the appointment to be edited
                    Appointment AppointmentLabel = ((Appointment)AppointmentList.SelectedItem);
                    //if no appointment selected, display error message and exit process
                    if(AppointmentList.SelectedItem == null)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }

                else
                {
                    MessageDialog ShowConf = new MessageDialog("Do you want to delete this appointment?", "Important");
                    ShowConf.Commands.Add(new UICommand("Yes, Delete")
                    {
                        Id = 0
                    });
                    ShowConf.Commands.Add(new UICommand("Cancel")
                    {
                        Id = 1
                    });
                    ShowConf.DefaultCommandIndex = 0;
                    ShowConf.CancelCommandIndex = 1;

                    var result = await ShowConf.ShowAsync();
                    if ((int)result.Id == 0)
                    {
                        int AppointmentID = ((Appointment)AppointmentList.SelectedItem).AppointmentID;
                        var querydel = conn.Query<Accounts>("DELETE FROM Appointment WHERE AppointmentID='" + AppointmentID + "'");
                        Results();
                    }
                    else
                    {
                        // no action required
                    }
                }
            }

            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                await ClearDialog.ShowAsync();
            }

        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if ((Appointment)AppointmentList.SelectedItem == null)
            {
                MessageDialog NullReferenceDialog = new MessageDialog("Please select an item to edit!", "Oops..!");
                await NullReferenceDialog.ShowAsync();
            }
            else
            {

                Appointment AppointmentEdit = ((Appointment)AppointmentList.SelectedItem);
                editAppID = AppointmentEdit.AppointmentID;
                EventName.Text = AppointmentEdit.EventName;
                EventLocation.Text = AppointmentEdit.Location;
                EventDate.SelectedDate = DateTime.Parse(AppointmentEdit.EventDate);
                StartTime.SelectedTime = TimeSpan.Parse(AppointmentEdit.StartTime);
                EndTime.SelectedTime = TimeSpan.Parse(AppointmentEdit.EndTime);
                UpdateSaveButton.Visibility = Visibility.Visible;
                UpdateSaveButton.IsEnabled = true;
                CancelUpdateButton.Visibility = Visibility.Visible;
                CancelUpdateButton.IsEnabled = true;
                AppointmentList.Visibility = Visibility.Collapsed;

            }

        }


        private async void UpdateSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if event name is null
                if (
                    ((EventName.Text.ToString() == "") || (EventName.Text.ToString() == null)) ||
                    ((EventLocation.Text.ToString() == "") || (EventName.Text.ToString() == null)) ||
                    (EventDate.SelectedDate == null) ||
                    (StartTime.SelectedTime == null) ||
                    (EndTime.SelectedTime == null)
                    )
                {
                    MessageDialog dialog = new MessageDialog("One of your fields has not been filled", "Oops..!");
                    await dialog.ShowAsync();
                }

                else
                {   // Inserts the data
                    var query2 = conn.Query<Appointment>("UPDATE Appointment SET EventName = '" + EventName.Text + "', " +
                        "Location = '" + EventLocation.Text + "'," +
                        "EventDate = '" + EventDate.Date.ToString(fmt) + "'," +
                        "StartTime = '" + StartTime.Time.ToString() + "'," +
                        "EndTime = '" + EndTime.Time.ToString() + "'" +
                        " WHERE AppointmentID ='" + editAppID + "'");

                    MessageDialog Confirmed = new MessageDialog("Appointment updated successfully");
                    await Confirmed.ShowAsync();

                    UpdateSaveButton.Visibility = Visibility.Collapsed;
                    CancelUpdateButton.Visibility = Visibility.Collapsed;
                    UpdateSaveButton.IsEnabled = false;
                    CancelUpdateButton.IsEnabled = false;
                    AppointmentList.Visibility = Visibility.Visible;
                    resetFields();
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when EventName is invalid
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter info for one of the fields", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Something went wrong - Appointment not added", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSaveButton.Visibility = Visibility.Collapsed;
            CancelUpdateButton.Visibility = Visibility.Collapsed;
            UpdateSaveButton.IsEnabled = false;
            CancelUpdateButton.IsEnabled = false;
            AppointmentList.Visibility = Visibility.Visible;
            resetFields();

        }
        public void resetFields()
        {
            EventName.Text = "";
            EventLocation.Text = "";
            EventName.PlaceholderText = "Name for appointment";
            EventLocation.PlaceholderText = "Location of appointment";
            EventDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndTime.SelectedTime = null;

        }

    }

}
