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

        private static dynamic Expand(XElement root)
        {
            dynamic element = new ExpandoObject();

            element.__value = root.Value;

            if (root.Elements().Any())
                Expand(root.Elements(), element);
            return CreateAttribute(root, element);
        }

        private static void Expand(IEnumerable<XElement> seq, dynamic self)
        {
            IDictionary<string, dynamic> r = self;
           
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
        private static string snakeCase(string element)
        {
            return element.Replace('-', '_');
        }
    }

}
