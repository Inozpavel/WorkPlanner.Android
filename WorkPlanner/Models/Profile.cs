using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace WorkPlanner.Models
{
    public class Profile : INotifyPropertyChanged
    {
        private string _email;

        private string _firstName;

        private bool _isEmailVerified;

        private string _lastName;

        private string _patronymic;

        private string _phoneNumber;

        [JsonProperty("first_name")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Patronymic
        {
            get => _patronymic;
            set
            {
                _patronymic = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("phone_number")]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("email_verified")]
        public bool IsEmailVerified
        {
            get => _isEmailVerified;
            set
            {
                _isEmailVerified = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}