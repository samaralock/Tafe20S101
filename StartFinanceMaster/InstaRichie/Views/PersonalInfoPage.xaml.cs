using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Resets the UI if the user left during an edit
           // AddButton.Visibility = Visibility.Visible;
            PersonalInfoList.Visibility = Visibility.Visible;

            ClearFeilds();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            PersonalInfoList.ItemsSource = query.ToList();
        }
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if account name is null
                if (FName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (FName.Text.ToString() == "FirstName" || LName.Text.ToString() == "LastName")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = FName.Text,
                        LastName = LName.Text
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when name is numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Name or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }
        private async void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int personal_Id = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                PersonalInfo infoObject = new PersonalInfo();
                infoObject = conn.Find<PersonalInfo>(personal_Id);
                FName.Text = infoObject.FirstName;
                LName.Text = infoObject.LastName;
                
                //switched out the add new entry button for the save editted button
                AddButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
                // Hides the sql List so that the selection can't be changed while edditing
                PersonalInfoList.Visibility = Visibility.Collapsed;
            }
            catch (NullReferenceException)
            {
                MessageDialog ClearDialog = new MessageDialog("Please select the item to Edit", "Oops..!");
                await ClearDialog.ShowAsync();
            }
        }

        //saves the edited info over the existing info
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FName.Text.ToString() == "" || LName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("All Feilds must be entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    int personal_Id = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                    //Deletes the existing entry
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE PersonalID='" + personal_Id + "'");
                    Results();

                    // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        PersonalID = personal_Id,
                        FirstName = FName.Text,
                        LastName = LName.Text,
                       
                    });
                    // Resets the UI when edit is complete
                    AddButton.Visibility = Visibility.Visible;
                    SaveButton.Visibility = Visibility.Collapsed;
                    PersonalInfoList.Visibility = Visibility.Visible;

                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the data or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("SQLite contraints violated", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                }

            }
        }
        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this info will delete all data of this Name", "Important");
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
                // checks if data is null else inserts
                try
                {
                    string FirstNameLabel = ((PersonalInfo)PersonalInfoList.SelectedItem).FirstName;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName='" + FirstNameLabel + "'");
                    Results();
                    conn.CreateTable<PersonalInfo>();
                    
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }
        public void ClearFeilds()
        {
            FName.Text = "";
            LName.Text = "";
            
        }

    }
}
