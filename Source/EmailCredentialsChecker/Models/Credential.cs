namespace EmailCredentialsChecker.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class Credential : INotifyPropertyChanged
    {
        private bool isValid;

        public Credential(string email, string password)
        {
            this.Email = email.ToLower();
            this.Password = password;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
            set
            {
                this.isValid = value;
                this.OnPropertyChanged();
            }
        }

        public EmailType GetEmailType()
        {
            switch (this.Email.Split('@')[1])
            {
                case "yahoo.com": return EmailType.Yahoo;
                case "gmail.com": return EmailType.Google;
                case "aol.com": return EmailType.Aol;
                default: throw new ArgumentException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
