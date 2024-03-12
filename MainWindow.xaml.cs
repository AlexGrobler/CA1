using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;
using Path = System.IO.Path;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace CA1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Ward> wards = new ObservableCollection<Ward>();
        private static int wardCount = 0;

        private static string appDirectory = "";
        private static string jsonFilePath = "";

        //initialization
        public MainWindow()
        {
            InitializeComponent();
   
            //get directory of app, and json data
            try 
            {
                appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                jsonFilePath = Path.Combine(appDirectory, "..", "..", "data", "data.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed To Get File Directory, See Error Code: " + ex);
            }
        }

        //on window load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            deserializeData();
        }

        //deserializes data from json file, sets listbox source and updates ward count.
        private void deserializeData() 
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                wards = JsonConvert.DeserializeObject<ObservableCollection<Ward>>(jsonContent);
                wardListBx.ItemsSource = wards;
                wardListBx.SelectedItem = wards[0];
                updateWardCount();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Failed To Fetch Data, See Error Code: " + ex);
            }
        }

        //updates displayed ward count
        private void updateWardCount() 
        {
            wardLabel.Content = $"Wards ({wards.Count})";
        }

        //serializes ward collection and writes to json file
        private void saveDataBtn_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(wards));
            updateWardCount();
        }

        //deserializes json data
        private void loadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            deserializeData();
            updateWardCount();
        }

        //update displayed patients to match selected ward
        private void wardListBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (wardListBx.SelectedItem != null)
            {
                Ward selectedWard = (Ward)wardListBx.SelectedItem;
                patientListBx.ItemsSource = selectedWard.Patients;
            }
            else patientListBx.ItemsSource = null;
        }

        //update displayed patient name and blood type image to match selected patient
        private void patientListBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (patientListBx.SelectedItem != null)
            {
                Patient selectedPatient = (Patient)patientListBx.SelectedItem;
                patientNameLbl.Content = selectedPatient.Name;

                if (selectedPatient.BloodType == EBloodType.Unknown) 
                {
                    bloodTypeImg.Source = null;
                    return;
                }

                try
                {
                    string image = $"{selectedPatient.BloodType.ToString().ToLower()}.png";
                    bloodTypeImg.Source = new BitmapImage(new Uri($"pack://application:,,,/CA1;component/images/{image}"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed To Retrieve Blood Type Image, See Error Code: " + ex);
                }
            }
            else 
            {
                patientNameLbl.Content = "";
                bloodTypeImg.Source = null;
            }
        }

        //add a new ward to the ward collection
        private void addWardBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = wardNameTxtBx.Text;
            int capacity = (int)wardCapacitySldr.Value;

            if (name == null || name == "") 
            {
                MessageBox.Show("A Name Must Be Given");
                return;
            }

            wards.Add(new Ward(capacity, name));
            updateWardCount();
        }


        //remove the selected ward from the collection
        private void removeDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wardListBx.SelectedItem != null)
            {
                Ward selectedWard = (Ward)wardListBx.SelectedItem;
                wardListBx.SelectedItem = null;
                wards.Remove(selectedWard);
                updateWardCount();
            }
            else MessageBox.Show("Select A Ward To Remove");
        }

        //I couldn't figure out how to handle the radio button values as a group, so resorted to if statements
        private EBloodType getSelectedBloodType() 
        {
            EBloodType selectedBloodType = EBloodType.Unknown;
            if ((bool)bloodA_btn.IsChecked)
            {
                selectedBloodType = EBloodType.A;
            }
            else if ((bool)bloodB_btn.IsChecked)
            {
                selectedBloodType = EBloodType.B;
            }
            else if ((bool)bloodAB_btn.IsChecked)
            {
                selectedBloodType = EBloodType.AB;
            }
            else if ((bool)bloodO_btn.IsChecked)
            {
                selectedBloodType = EBloodType.O;
            }
            return selectedBloodType;
        }

        //add a new patient to the selected ward's patient collection
        private void addPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wardListBx.SelectedItem != null)
            {
                Ward selectedWard = (Ward)wardListBx.SelectedItem;

                if (selectedWard.Patients.Count + 1 > selectedWard.Capacity)
                {
                    MessageBox.Show("Number Of Patients Would Exceed Ward Capacity");
                    return;
                }

                Patient newPatient = new Patient();
                String name = patientNameTxtBx.Text;

                if (datePicker.SelectedDate == null || datePicker.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Invalid Date Selected");
                    return;
                }

                DateTime dob = (DateTime)datePicker.SelectedDate;

                if (name == null || name == "" || name.Any(char.IsDigit))
                {
                    MessageBox.Show("A Valid Name Must Be Given");
                    return;
                }

                EBloodType bloodType = getSelectedBloodType();
                if (bloodType == EBloodType.Unknown)
                {
                    newPatient = new Patient(name, dob);
                    selectedWard.Patients.Add(newPatient);
                    return;
                }

                newPatient = new Patient(dob, bloodType, name);
                selectedWard.Patients.Add(newPatient);
            }
            else MessageBox.Show("First Select A Ward To Add The New Patient To");
        }

        //removes the selected patient from the currently selected ward's patient collection
        private void removePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wardListBx.SelectedItem != null && patientListBx.SelectedItem != null) 
            {
                Ward selectedWard = (Ward)wardListBx.SelectedItem;
                Patient selectedPatient = (Patient)patientListBx.SelectedItem;
                selectedWard.Patients.Remove(selectedPatient);  
            }
            else MessageBox.Show("Select A Ward And Then The Patient To Remove");
        }
    }  
}
