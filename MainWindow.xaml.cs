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

namespace CA1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<Ward> wards = new ObservableCollection<Ward>();
        private static string appDirectory = "";
        private static string jsonFilePath = "";
        private static int wardCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            jsonFilePath = Path.Combine(appDirectory, "..", "..","data","data.json");
;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeData();
        }

        private void initializeData() 
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            wards = JsonConvert.DeserializeObject<ObservableCollection<Ward>>(jsonContent);
            wardListBx.ItemsSource = wards;
            wardCount = wards.Count;
            wardLabel.Content = $"Wards ({wardCount})";
        }

        private void wardListBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ward selectedWard = (Ward)wardListBx.SelectedItem;
            patientListBx.ItemsSource = selectedWard.Patients;
        }

        private void saveDataBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void loadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            initializeData();
        }

        private void patientListBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Patient selectedPatient = (Patient)patientListBx.SelectedItem;
            if (selectedPatient != null)
            {
                patientNameLbl.Content = selectedPatient.Name;
                string image = $"{selectedPatient.BloodType.ToString().ToLower()}.png";
                bloodTypeImg.Source = new BitmapImage(new Uri($"pack://application:,,,/CA1;component/Images/{image}"));
            }
            else 
            {
                patientNameLbl.Content = "";
                bloodTypeImg.Source = null;
            }
        }
    }


    public enum EBloodType 
    {
        A,
        B,
        AB,
        O
    }

    public class Ward 
    {
        public int Capacity { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }
       
        public Ward() { }


        public Ward(int capacity, string name, ObservableCollection<Patient> patients)
        {
            Capacity = capacity;
            Name = name;
            Patients = patients;    
        }

        public Ward(int capacity, string name) : this(capacity, name, new ObservableCollection<Patient>())
        {
        }

        public Ward(int capacity) : this(capacity, "", new ObservableCollection<Patient>()) 
        {
        }


        public override string ToString()
        {
            return $"{Name}  Limit: {Capacity}";
        }

    }

    public class Patient
    {
        public DateTime DateOfBirth { get; set; }    
        public EBloodType BloodType { get; set; }   
        public string Name { get; set; }    

        public Patient() { }

        public Patient(DateTime dob, EBloodType bloodType, string name)
        {
            DateOfBirth = dob;
            BloodType = bloodType;
            Name = name;    
        }

        public Patient(string name, DateTime dob) : this(dob, EBloodType.A, name)
        {
        }

        public Patient(string name) : this(DateTime.Now, EBloodType.A, name)
        {
        }

        private int calculateAge() 
        {
            int age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateTime.Now.Month < DateOfBirth.Month || (DateTime.Now.Month == DateOfBirth.Month && DateTime.Now.Day < DateOfBirth.Day)) age--; 
            return age;
        }

        public override string ToString()
        {
            return $"{Name} ({calculateAge()}) Type: {BloodType}";
        }
    }
}
