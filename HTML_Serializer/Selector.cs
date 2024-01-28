using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HTML_Serializer
{
    public class Selector
    {
        public string TagName { get; set; } = "";
        public string Id { get; set; } = "";
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }


        public static Selector Parse(string querystring)
        {
            string[] query = querystring.Split(' ');
            int i = 0;
            Selector selector = Create_Selector(null);
            Selector current = selector;
            foreach (string s in query)
            {
                string[] partsCurQuery = new Regex("(?=[#\\.])").Split(s).Where(s1 => s1.Length > 0).ToArray();
                foreach (string p in partsCurQuery)
                {
                    if (p.StartsWith("#"))
                        current.Id = p.Substring(1);
                    else if (p.StartsWith('.'))
                        current.Classes.Add(p.Substring(1));
                    else if (HTMLHelper.Instance.TagsExist.Contains(p))
                        current.TagName = p;
                }
                current = Create_Selector(current);
            }
            return selector;
        }

        public static Selector Create_Selector(Selector parent)
        {
            Selector selector = new Selector();
            if (parent != null)
                parent.Child = selector;
            selector.Parent = parent;
            return selector;
        }
    }
}
