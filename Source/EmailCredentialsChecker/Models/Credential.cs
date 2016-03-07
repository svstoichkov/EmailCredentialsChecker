namespace EmailCredentialsChecker.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    using Helpers;

    public class Credential : INotifyPropertyChanged
    {
        private bool? isValid;

        public Credential(string email, string password)
        {
            this.Email = email.ToLower();
            this.Password = password;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool? IsValid
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

        public void Check()
        {
            switch (this.GetEmailType())
            {
                case EmailType.Yahoo:
                    this.IsValid = YahooChecker.Check(this);
                    break;
                case EmailType.Aol:
                    this.IsValid = AolChecker.Check(this);
                    break;
                case EmailType.Google:
                    this.IsValid = GoogleChecker.Check(this);
                    break;
                case EmailType.Att:
                    this.IsValid = AttChecker.Check(this);
                    break;
            }
        }

        private EmailType GetEmailType()
        {
            switch (this.Email.Split('@')[1])
            {
                case "yahoo.com": return EmailType.Yahoo;
                case "gmail.com": return EmailType.Google;
                case "aol.com": return EmailType.Aol;
                case "att.net": return EmailType.Att;
                case "sbcglobal.net": return EmailType.Att;
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
