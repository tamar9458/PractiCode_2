// See https://aka.ms/new-console-template for more information
using HTML_Serializer;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

var html = await Load("https://chani-k.co.il/sherlok-game/");

var cleanHtml = new Regex("([\\r\\n\\t\\v\\f]+)").Replace(html, "");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0 && !s.StartsWith("  ") && !s.Equals(" "));//סיום הכנת שורות הדף לסריקה

HTMLElement htmlRoot = Build_new_element(null, htmlLines.Skip(1).First().Split(' ')[0], htmlLines.Skip(1).First());//שורש הדף
HTMLElement htmlTree = Build_tree_html(htmlRoot, htmlLines.Skip(2));                                              //יצירת העץ   

Console.WriteLine("HTML Tree:");
PrintHtmlTree(htmlTree, "");

HashSet<HTMLElement> resultSelectors = htmlRoot.FindElement(Selector.Parse("div.copyR p"));                    //חיפוש ע''פ שאילתה

Console.WriteLine("result of the query:");
foreach (HTMLElement e in resultSelectors)
    Console.WriteLine(e.ToString());
Console.ReadKey();

async Task<string> Load(string url)
{
    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}                                                                        //קבלת הדף בקריאת request
static HTMLElement Build_new_element(HTMLElement parent, string name, string line)
{
    HTMLElement newElement = new HTMLElement();
    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
    foreach (Match attribute in attributes)
    {
        if (attribute.Value.Contains("class"))
        {
            int indexStartClass = line.IndexOf("class");
            string rest = line.Substring(indexStartClass + 7);
            int indexEndClass = rest.IndexOf("\"");
            rest = rest.Substring(0, indexEndClass);
            newElement.Classes = rest.Split(' ').ToList();
        }
        if (attribute.Value.Contains("id"))
        {
            int indexStartId = line.IndexOf("id");
            string rest = line.Substring(indexStartId + 4);
            int indexEndId = rest.IndexOf("\"");
            rest = rest.Substring(0, indexEndId);
            newElement.ID = rest;
        }
    }
    newElement.Attributes = attributes.Cast<Match>().Select(match => match.Value).ToList();
    newElement.Name = name;
    newElement.Parent = parent;

    return newElement;

}
static HTMLElement Build_tree_html(HTMLElement root, IEnumerable<string> htmlLines)
{
    HTMLElement temp = root;

    foreach (var line in htmlLines)
    {
        int findSpace = line.IndexOf(' ');
        string name = line, restLine = "";
        if (findSpace > 0)
        {
            name = line.Substring(0, findSpace);
            restLine = line.Substring(findSpace + 1);
        }
        if (line.StartsWith("/html"))
        {
            break;
        }
        if (line.StartsWith("/"))
        {
            temp = temp.Parent;
            continue;
        }
        if (!HTMLHelper.Instance.TagsExist.Contains(name))
        {
            temp.InnerHtml += line;
            continue;
        }
        HTMLElement newElement = Build_new_element(temp, name, line);
        temp.Children.Add(newElement);
        if (!HTMLHelper.Instance.TagsVoid.Contains(name) && !line.EndsWith("/"))
            temp = newElement;
    }

    return root;
}

static void PrintHtmlTree(HTMLElement element, string id)
{
    Console.WriteLine($"{id}{element}");
    foreach (var child in element.Children)
        PrintHtmlTree(child, id + "  ");
}