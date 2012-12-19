using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Dynamic
{
    public class Convert
    {
        public static dynamic FromXml(XElement root)
        {
            dynamic result = new ExpandoObject();
            GetAttributes(root, result);

            foreach (var element in root.Elements())
            {
                var skip = element.Elements().GroupBy(n => n.Name.LocalName).Count() == 1;

                var values = new List<dynamic>();
                if (skip)
                    values.AddRange(element.Elements().Select(item =>
                    {
                        if (item.HasElements)
                            return FromXml(item);
                        return (item.HasAttributes) ? GetAttributes(item, new ExpandoObject()) : item.Value.Trim();
                    }));
                else
                    values.Add((element.HasElements || element.HasAttributes) ? FromXml(element) : element.Value.Trim());

                GetAttributes(element, result);

                var p = result as IDictionary<string, dynamic>;
                var name = snakeCasedName(element.Name.LocalName);

                if (p.ContainsKey(name))
                {
                    var x = p[name];
                    if (x is IList<dynamic>)
                        x.Add(skip ? values : values.FirstOrDefault());
                    else
                        p[name] = new List<dynamic> { x, skip ? values : values.FirstOrDefault() };
                }
                else
                    p[name] = skip ? values : values.FirstOrDefault();
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
