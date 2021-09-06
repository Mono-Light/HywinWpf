using System;
using System.IO;
using Newtonsoft.Json;

namespace Messager.Responses
{
    public class LanguageManager
    {
        public static LanguageContent content;
        public static void Init(string language)
        {
            content = ParseFileToContent($"Languages/{language}.json");
        }

        private static LanguageContent ParseFileToContent(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<LanguageContent>(json);
        }

        public class LanguageContent
        {
           public string START_USERNAME_PLACEHOLDER;
           public string START_PASSWORD_PLACEHOLDER;
           public string USERNAME_PLAIN;
           public string PASSWORD_PLAIN;
        }

    }


}
