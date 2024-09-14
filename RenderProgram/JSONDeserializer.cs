using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RenderProgram
{
    internal static class JSONDeserializer
    {
        public static AppSettings ReadSettings(string filePath)
        {
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(filePath));
        }
    }
    public class Option
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class Text
    {
        public string Header { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Menu
    {
        public int Id { get; set; }
        public Text Text { get; set; }
    }

    public class AppSettings
    {
        public List<Menu> Menus { get; set; }
    }
}
