using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Dynamic
{
    public class Convert
    {
        public static dynamic Expand(XElement root)
        {
            return root.Elements().Any() ? Expand(root.Elements()) : root.Value;
        }

        public static dynamic Expand(IEnumerable<XElement> seq)
        {
            dynamic result = new ExpandoObject();
            var r = result as IDictionary<string, dynamic>;

            foreach (var element in seq)
                r[element.Name.LocalName] = Expand(element);

            return r;
        }

        private static void CreateTag(XElement element, dynamic parent)
        {
            var p = parent as IDictionary<string, dynamic>;
            var key = element.Name.LocalName;
            if(!p.ContainsKey(key))
                p[key] = element.Value.Trim();
            else
            {
                var seqName = key + "s";
                var values = new List<dynamic>();
                values.Add(p[key]);
                values.Add(element.Value.Trim());
                p[seqName] = values;
            }
        }

        public static dynamic FromXml(XElement root)
        {
            //dynamic result = new ExpandoObject();
            return Expand(root);
            
        }

        private static bool AllChildrenHasSameName(IEnumerable<XElement> elements)
        {
            return elements.GroupBy(n => n.Name.LocalName).Count() == 1;
        }

        private static bool AllChildrenHasSameName(XElement element)
        {
            return element.Elements().GroupBy(n => n.Name.LocalName).Count() == 1;
        }

        private static dynamic GetAttributes(XElement root)
        {
            var result = new Dictionary<string, dynamic>();
            foreach (var attribute in root.Attributes())
            {
                var casedName = snakeCasedName(root.Name.LocalName) + '_' + snakeCasedName(attribute.Name.LocalName);
                result[casedName] = attribute.Value;
            }
            return result;
        }

        private static dynamic GetAttributes(XElement root, dynamic result)
        {
            foreach (var attribute in root.Attributes())
            {
                var casedName = snakeCasedName(root.Name.LocalName) + '_' + snakeCasedName(attribute.Name.LocalName);
                (result as IDictionary<string, dynamic>)[casedName] = attribute.Value;
            }
            return result;
        }

        private static string snakeCasedName(string element)
        {
            return element.Replace('-', '_');
        }
    }

}
