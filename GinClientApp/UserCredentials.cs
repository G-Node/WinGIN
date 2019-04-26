using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GinClientApp
{
    /// <summary>
    ///     A singleton class representing the current user's login credentials
    /// </summary>
    public class UserCredentials : ICloneable
    {
        private static UserCredentials _instance;

        private UserCredentials()
        {
            loginList = new List<LoginSettings>
            {
                //new LoginSettings()
            };
        }

        public static UserCredentials Instance
        {
            get => _instance ?? (_instance = new UserCredentials());
            set => _instance = value;
        }

        public List<LoginSettings> loginList;


        public object Clone()
        {
            return new UserCredentials { loginList = loginList };
        }

        public static bool Load()
        {
            var saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                               @"\g-node\GinWindowsClient\UserCredentials.json";

            if (!Directory.Exists(Path.GetDirectoryName(saveFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));

            if (!File.Exists(saveFilePath)) return false;

            try
            {
                using (var freader = File.OpenText(saveFilePath))
                {
                    var text = freader.ReadToEnd();
                    _instance = JsonConvert.DeserializeObject<UserCredentials>(text);
                }
                if (string.IsNullOrEmpty(_instance.loginList.First().Username) || string.IsNullOrEmpty(_instance.loginList.First().Password) || string.IsNullOrEmpty(_instance.loginList.First().Server))
                    return false;

                return true;
            }
            catch
            {
                MessageBox.Show("Parse fails");
                _instance = new UserCredentials();
                return false;
            }
        }

        public static void Save()
        {
            var saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                               @"\g-node\GinWindowsClient\UserCredentials.json";

            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            using (var fwriter = File.CreateText(saveFilePath))
            {
                fwriter.Write(JsonConvert.SerializeObject(_instance));
            }
        }

        public class LoginSettings
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Server { get; set; }
        }
    }
}