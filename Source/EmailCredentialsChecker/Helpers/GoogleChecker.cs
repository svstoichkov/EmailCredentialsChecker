namespace EmailCredentialsChecker.Helpers
{
    using System;

    using Models;

    using Pop3;

    public class GoogleChecker
    {
        public static bool? Check(Credential credential)
        {
            var client = new Pop3Client();
            try
            {
              client.Connect("pop.gmail.com", credential.Email, credential.Password, true);
              client.Disconnect();
              client.Dispose();
              return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Web login required"))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
