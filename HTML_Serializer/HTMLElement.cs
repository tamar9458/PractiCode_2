using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace HTML_Serializer
{
    public class HTMLElement
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public IEnumerable<string> Attributes { get; set; } = new List<string>();
        public IEnumerable<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HTMLElement Parent { get; set; }
        public List<HTMLElement> Children { get; set; } = new List<HTMLElement>();

        public override string ToString()
        {
            return $"{Name}";
        }

        public IEnumerable<HTMLElement> Descendants()
        {
            Queue<HTMLElement> queue = new Queue<HTMLElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HTMLElement cur = queue.Dequeue();
                if (this != cur)
                    yield return cur;
                foreach (HTMLElement child in cur.Children)
                    queue.Enqueue(child);
            }
        }

        public IEnumerable<HTMLElement> Ancestors()
        {
            for (HTMLElement cur = this; cur.Parent != null; cur = cur.Parent)
                yield return cur.Parent;
        }


        public HashSet<HTMLElement> FindElement(Selector selector)
        {
            var result = new HashSet<HTMLElement>();
            foreach (var child in this.Descendants())
                child.FindElementRec(selector, result);
            return result;

        }
        public void FindElementRec(Selector selector, HashSet<HTMLElement> result)
        {
            if (!this.IsEqual(selector))
                return;
            if (selector.Child.Child == null)
            {
                result.Add(this);
                return;
            }
            foreach (var child in this.Descendants())
                child.FindElementRec(selector.Child, result);
        }
        public bool IsEqual(Selector selector)
        {
            if (this.Name != selector.TagName)
                return false;
            if (this.ID != selector.Id)
                return false;
            if (this.Classes.Count() != selector.Classes.Count())
                return false;
            int i = 0;
            while (this.Classes.ToList().Count() > i && selector.Classes.ToList().Count() > i)
            {
                var c1 = this.Classes.ToArray()[i];
                var c2 = selector.Classes.ToArray()[i++];
                if (!c1.Equals(c2))
                    return false;
            }
            return true;
        }
    }
}
