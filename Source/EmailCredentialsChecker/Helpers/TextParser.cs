namespace EmailCredentialsChecker.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Models;

    public static class TextParser
    {
        public static List<Credential> GetCredentials(string filePath)
        {
            var contents = File.ReadAllText(filePath);
            var lines = contents.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var credentials = new List<Credential>();
            foreach (var line in lines)
            {
                var pair = line.Split('/');

                var credential = new Credential(pair[0], pair[1]);
                credentials.Add(credential);
            }

            return credentials;
        }
    }
}
