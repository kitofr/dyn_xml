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
            return AttachAttributes(root, Expand(root));
        }

        private static dynamic CreateAttribute(XElement root, dynamic result)
        {
            var r = result as IDictionary<string, dynamic>;

            foreach (var attribute in root.Attributes())
            {
                var casedName = '_' + snakeCase(attribute.Name.LocalName);
                r[casedName] = attribute.Value;
            }
            return r;
        }

        private static dynamic Expand(XElement root)
        {
            return root.Elements().Any() ? 
                Expand(root.Elements()) : 
                GetAttributes(root);
        }

        private static dynamic Expand(IEnumerable<XElement> seq)
        {
            dynamic result = new ExpandoObject();
            var r = result as IDictionary<string, dynamic>;
            
            if (HasMoreThanOneChild(seq) && AllChildrenHasSameName(seq))
            {
                var key = CreateSequenceName(seq);
                r[key] = seq.Select(element => Expand(element)).ToList();
            }
            else if (HasMoreThanOneChild(seq) && SomeChildrenHasSameName(seq))
            {
                var childrenWithSameName = seq.GroupBy(n => n.Name.LocalName)
                    .Select(x => new { Name = x.Key, Values = x.ToList(), Count = x.Count() });

                foreach (var child in childrenWithSameName)
                {
                    var list = child.Values;
                    var key = CreateSequenceName(list);
                    r[key] = (list.Count == 1) ? Expand(list.First()) : list.Select(x => Expand(x)).ToList();
                }
            }
            else
                foreach (var element in seq)
                {
                    var key = snakeCase(element.Name.LocalName);
                    r[key] = Expand(element);
                }

            return r;
        }

        private static string CreateSequenceName(IEnumerable<XElement> seq)
        {
            return snakeCase(seq.First().Name.LocalName);
        }

        private static bool HasMoreThanOneChild(IEnumerable<XElement> seq)
        {
            return seq.Count() > 1;
        }
        private static bool SomeChildrenHasSameName(IEnumerable<XElement> seq)
        {
            return seq.GroupBy(n => n.Name.LocalName)
                        .Select(group => new { Name = group.Key, Count = group.Count()})
                        .Any(x => x.Count > 1);
        }
        private static bool AllChildrenHasSameName(IEnumerable<XElement> seq)
        {
            return seq.GroupBy(n => n.Name.LocalName).Count() == 1;
        }

        private static dynamic AttachAttributes(XElement root, dynamic result)
        {
            return !root.HasAttributes ? result : CreateAttribute(root, result);
        }

        private static dynamic GetAttributes(XElement root)
        {
            return !root.HasAttributes ? root.Value : CreateAttribute(root, new ExpandoObject());
        }

        private static string snakeCase(string element)
        {
            return element.Replace('-', '_');
        }
    }

}
