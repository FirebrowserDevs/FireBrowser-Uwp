using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Data.Sqlite;
using Dots.SDK;
using System.Text.Json.Serialization;
using System.Text.Json;
using static FireBrowser.Core.AppData;
using static QRCoder.PayloadGenerator;

namespace FireBrowser.Core
{
    /// <summary>
    /// This file is for general functions related to saving and managing app data.
    /// </summary>
    public static class AppData
    {
        

        //Currently selected profile ID
        public static int currentProfileID = 0;
        public static ProfileCore CurrentProfileCore
        {

            get
            {
               
                string appDataFilePath = $"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json";
                CreateHisDb();
                try
                {
                    var profiles = JsonSerializer.Deserialize<AppDataCore>(File.ReadAllText($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json"), serializerOptions);
                    return profiles.Profiles.SingleOrDefault(item => item.ID == currentProfileID);

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public async static Task<ProfileCore> GetCurrentProfileCoreAsync()
        {
            try
            {
                var appData = await GetAppDataCore();

                return appData.Profiles.SingleOrDefault(item => item.ID == currentProfileID);
            }
            catch(Exception ex)
            {
             
                var appdatacore = await CreateAppDataCore();
                return appdatacore.Profiles[0];
            }
        }

        //public static ProfileCore CurrentProfileCore = JsonSerializer.Deserialize<AppDataCore>($"{ApplicationData.Current.LocalFolder.Path}\\FireBrowserData\\ProfileData.json", serializerOptions).Profiles.SingleOrDefault(item => item.ID == currentProfileID);

        public static async Task<StorageFolder> GetDataFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            return await localFolder.CreateFolderAsync($"FireBrowserData", CreationCollisionOption.OpenIfExists);
        }

       
        public static async void CreateHisDb()
        {
            var profileCore = await GetCurrentProfileCoreAsync();
            if (File.Exists($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{profileCore.FriendlyID}/FireBrowserHistory.Db"))
            {
                      
            }
            else
            {

                SqliteConnection m_dbConnection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{profileCore.FriendlyID}/FireBrowserHistory.Db");
                m_dbConnection.Open();

                string sql = "create table urls (url varchar(250), title varchar(250), last_visit_time DATETIME)";

                SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
        }
        /// <summary>
        /// Hash is the file name. It is stored in the cache folder.
        /// FallbackURL is used when the file can't be found in the app cache.
        /// </summary>
        public class CachableImage
        {
            public string Hash { get; set; }
            public string FallbackURL { get; set; }
        }
        public static async Task<StorageFolder> GetUserFolder()
        {
            var profileCore = await GetCurrentProfileCoreAsync();
           
            var app = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);
            var firebrowser = await app.GetFolderAsync("FireBrowserData");
            var user = await firebrowser.CreateFolderAsync(profileCore.FriendlyID, CreationCollisionOption.OpenIfExists);
            return user;
            //return await StorageFolder.GetFolderFromPathAsync($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{profileID}");
        }
        /// <summary>
        /// This is the core file that is not synced to the cloud.
        /// </summary>
        public class AppDataCore
        {
            public float AppVersion { get; set; }
            public int DefaultProfileID { get; set; }
            public List<ProfileCore> Profiles { get; set; }
        }
        /// <summary>
        /// Base profile data
        /// </summary>
        public enum AccountType
        {
            Dot,
            Local
        }
        public class ProfileCore
        {
            public string Name { get; set; }

            /// <summary>
            /// Name of the account without special characters OR the user's Project Passport ID 
            /// depending on the method they use to sign in
            /// </summary>
            public string FriendlyID { get; set; }
            public int ID { get; set; }
            public AccountType AccountType { get; set; }
            public string Token { get; set; }
            public CachableImage ProfilePic { get; set; }
            public bool HasLock { get; set; }
            public bool FinishedFirstLaunch { get; set; }
        }
        public static JsonSerializerOptions options = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };
        public static JsonSerializerOptions serializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };
        public enum DataType {
            AppSettings,
            UserData
        }
        public static string ImagePath(CachableImage image)
        {
            //To-Do: Actually cache the image if it's not already done
            var path = "ms-appdata://temp/" + image.Hash + ".png";
            if (File.Exists(path))
            {
                Debug.WriteLine("File exists!");
                return path;
            }
            else { return image.FallbackURL; }
        }
        public static ImageSource CachedImageSource(CachableImage image)
        {
            //same function as above but ok
            var path = "ms-appdata://temp/" + image?.Hash + ".png";
            Uri imageUri = File.Exists(path) ? new Uri(path) : new Uri(image.FallbackURL);
            
            BitmapImage bitmapImage = new() { UriSource = imageUri };
            ImageSource imageSource = bitmapImage;
            return imageSource;
        }



        public static async void SaveData<T>(T data, DataType dataType)
        {
            var userFolder = await GetUserFolder();
            string fileName;
            fileName = (dataType == DataType.UserData) ? "UserData.json" : "UserSettings.json";

            await File.WriteAllTextAsync(
                userFolder.Path + $"\\{fileName}",
                JsonSerializer.Serialize(data, options)
                );
        }
        public static T FromXml<T>(this string value)
        {
            using TextReader reader = new StringReader(value);
            return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
        }
        /// <summary>
        /// Gets the AppDataCore stored in the app files, otherwise creates a new one.
        /// </summary>
        /// <returns></returns>
        public async static Task<AppDataCore> GetAppDataCore()
        {
            string appDataFilePath = $"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json";

            //To-Do: Also check if the files aren't empty
            if (!File.Exists(appDataFilePath)) await CreateAppDataCore();

            try
            {
                return JsonSerializer.Deserialize<AppDataCore>(File.ReadAllText($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json"), serializerOptions);

            }
            catch (Exception ex)
            {
              
                await CreateAppDataCore();
                return JsonSerializer.Deserialize<AppDataCore>(File.ReadAllText($"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json"), serializerOptions);
            }
        }

        /// <summary>
        /// Used to create a brand new AppDataCore from a template, with a sample profile.
        /// Only use if there was no file or if it was empty. Otherwise, use CreateProfileCore();
        /// </summary>
        /// <seealso cref="CreateProfileCore(string)"/>
        public async static Task<AppDataCore> CreateAppDataCore()
        {
            AppDataCore templateAppDataCore = new()
            {
                AppVersion = Resources.GetAppVersion(),
                DefaultProfileID = 0,
                Profiles = new()
                {
                    await CreateProfileCore()
                }
            };

            StorageFolder dataFolder = await GetDataFolder();
            StorageFile appDataCoreFile = await dataFolder.CreateFileAsync("ProfileData.json", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(appDataCoreFile, JsonSerializer.Serialize(templateAppDataCore, serializerOptions));

            return templateAppDataCore;
        }

        public async static Task<ProfileCore> CreateProfileCore(string name = null)
        {

            name ??= "DefaultFireBrowserUser";

            string friendlyID = name.RemoveSpecialCharacters();

            // Generate a random ID, in case the user decides to have multiple accounts with the same name
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomID = new string(Enumerable.Repeat(chars, 8).Select(c => c[random.Next(c.Length)]).ToArray());
            // Append the current friendlyID with the randomID
            friendlyID += "_" + randomID;

            // Create a template profile
            ProfileCore templateProfile = new ProfileCore
            {
                Name = name,
                FriendlyID = friendlyID,
                Token = "0",
                AccountType = AccountType.Local,
                FinishedFirstLaunch = false,
                HasLock = false,
                ProfilePic = new()
                {
                    FallbackURL = "ms-appx:///Assets/ProfilePictures/Default.png"
                }
            };

            string appDataFilePath = $"{ApplicationData.Current.LocalFolder.Path}/FireBrowserData/ProfileData.json";

            if (File.Exists(appDataFilePath))
            {
                if (templateProfile == null)
                {
                    return null;
                }

                var appDataCore = await GetAppDataCore() ?? new AppDataCore();
                appDataCore.Profiles = appDataCore.Profiles ?? new List<ProfileCore>();

                if (appDataCore.Profiles.Count > 0)
                {
                    templateProfile.ID = appDataCore.Profiles.Max(p => p.ID) + 1;
                }
                else
                {
                    templateProfile.ID = 1;
                }
                appDataCore.Profiles.Add(templateProfile);

                using (var fileStream = File.Open(appDataFilePath, FileMode.Open))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        await streamWriter.WriteAsync(JsonSerializer.Serialize(appDataCore, serializerOptions));
                    }
                }
            }


            StorageFolder dataFolder = await GetDataFolder();
            StorageFolder userFolder = await dataFolder.CreateFolderAsync(templateProfile.FriendlyID);
            StorageFolder defaultDataFolder = await StorageFolder.GetFolderFromPathAsync(
                                              $"{Package.Current.InstalledLocation.Path}\\Assets\\DefaultData");

            //Create base files and their data
            StorageFile userData = await userFolder.CreateFileAsync("UserData.json");
            StorageFile defaultUserData = await defaultDataFolder.GetFileAsync("DefaultData.json");
            await FileIO.WriteTextAsync(userData, await FileIO.ReadTextAsync(defaultUserData));

            StorageFile userSettings = await userFolder.CreateFileAsync("UserSettings.json");
            StorageFile defaultUserSettings = await defaultDataFolder.GetFileAsync("DefaultSettings.json");
            await FileIO.WriteTextAsync(userSettings, await FileIO.ReadTextAsync(defaultUserSettings));

            
            //Create empty folders
            await userFolder.CreateFolderAsync("Browsers");
            await userFolder.CreateFolderAsync("Collections");
            await userFolder.CreateFolderAsync("Themes");
            await userFolder.CreateFolderAsync("Modules");
            await userFolder.CreateFolderAsync("Other");

            return templateProfile;
        }
    }
}