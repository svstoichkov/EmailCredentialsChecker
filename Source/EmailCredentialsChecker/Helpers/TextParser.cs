namespace EmailCredentialsChecker.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    using Models;

    public static class TextParser
    {
        public static List<Credential> GetCredentials(string filePath)
        {
            var contents = File.ReadAllText(filePath);
            var lines = contents.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var credentials = new List<Credential>();
            int i = 0;
            try
            {
                for (i = 0; i < lines.Length; i++)
                {
                    var pair = lines[i].Split('/');

                    var credential = new Credential(pair[0], pair[1]);
                    credentials.Add(credential);
                }
            }
            catch
            {
                MessageBox.Show($"Program cannot parse line {i + 1}", "Invalid format", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return credentials;
        }
    }
}
