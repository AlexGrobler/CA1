using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA1
{
    public enum EBloodType
    {
        A,
        B,
        AB,
        O,
        Unknown
    }

    public class Patient
    {
        public DateTime DateOfBirth { get; set; }
        public EBloodType BloodType { get; set; }
        public string Name { get; set; }

        public Patient() { }

        public Patient(string name) : this(DateTime.Now, EBloodType.Unknown, name)
        {
        }

        public Patient(string name, DateTime dob) : this(dob, EBloodType.Unknown, name)
        {
        }

        public Patient(DateTime dob, EBloodType bloodType, string name)
        {
            DateOfBirth = dob;
            BloodType = bloodType;
            Name = name;
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
