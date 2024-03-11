using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA1
{
    public class Ward
    {
        public int Capacity { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }

        public Ward() { }

        public Ward(int capacity) : this(capacity, "", new ObservableCollection<Patient>())
        {
        }


        public Ward(int capacity, string name) : this(capacity, name, new ObservableCollection<Patient>())
        {
        }

        public Ward(int capacity, string name, ObservableCollection<Patient> patients)
        {
            Capacity = capacity;
            Name = name;
            Patients = patients;
        }

        public override string ToString()
        {
            return $"{Name}  Limit: {Capacity}";
        }
    }
}
