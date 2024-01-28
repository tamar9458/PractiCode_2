using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace HTML_Serializer
{
    public class HTMLHelper
    {
        private readonly static HTMLHelper _instance = new HTMLHelper();
        public static HTMLHelper Instance => _instance;
        public string[] TagsExist { get; set; }//כל התגיות הקיימות
        public string[] TagsVoid { get; set; }//תגיות ללא צורך בתגית סוגרת

        private HTMLHelper()
        {
            var allTags = File.ReadAllText("JSONS/HtmlTags.json");
            var voidTags = File.ReadAllText("JSONS/HtmlVoidTags.json");
            TagsExist = JsonSerializer.Deserialize<string[]>(allTags)!;
            TagsVoid = JsonSerializer.Deserialize<string[]>(voidTags)!;
        }
    }
}
