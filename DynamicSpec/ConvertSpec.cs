using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cone;
using Convert = Dynamic.Convert;

namespace DynamicSpec
{

    [Describe(typeof(Convert))]
    public class ConvertSpec
    {
        [Context("Element nesting")]
        public class ElementNesting
        {
            public void with_one_element()
            {
                dynamic foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar id='42'>1</bar>
</foo>");
                string bar = foo.bar.__value;
                Verify.That(() => bar == "1");
            }

            public void with_two_elements()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>1</bar>
    <baz>2</baz>
</foo>");
                string bar = foo.bar.__value;
                Verify.That(() => bar == "1");

                string baz = foo.baz.__value;
                Verify.That(() => baz == "2");
            }

            public void with_two_elements_with_same_name()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>1</bar>
    <bar>2</bar>
</foo>");
                var bars = (IList<dynamic>)foo.bar;
                Verify.That(() => bars.Count == 2);
                var values = new List<string>();
                foreach (var bar in bars) { values.Add(bar.__value);}
                Verify.That(() => string.Join(",", values) == "1,2");
            }

            public void that_is_3_levels_deep()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>
        <baz>Yes</baz>
    </bar>
</foo>");
                string baz = foo.bar.baz.__value;
                Verify.That(() => baz == "Yes");
            }

            public void that_is_3_levels_deep_and_have_list_grouping()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>
        <baz>Yes</baz>
    </bar>
    <bar>
        <baz>No</baz>
    </bar>
</foo>");
                var bars = (IList<dynamic>)foo.bar;
                Verify.That(() => bars.Count == 2);

                var values = new List<string>();
                foreach (var bar in bars) { values.Add(bar.baz.__value); }

                Verify.That(() => string.Join(",", values) == "Yes,No");
            }
            public void with_two_elements_with_same_name_and_one_with_a_different()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>1</bar>
    <bar>2</bar>
    <baz>No Wai!</baz>
</foo>");
                var bars = (IList<dynamic>)foo.bar;
                Verify.That(() => bars.Count == 2);
                var values = new List<string>();
                foreach (var bar in bars) { values.Add(bar.__value); }
                Verify.That(() => string.Join(",", values) == "1,2");
                string baz = foo.baz.__value;
                Verify.That(() => baz == "No Wai!");
            }
        }

        [Context("Attributes")]
        public class Attributes
        {
            public void top_node_attribute()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo' />");
                string id = foo._id;
                Verify.That(() => id == "foo");
            }

            public void top_node_as_container_with_attributes()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar id='1'/>
</foo>");
                string id = foo._id;
                Verify.That(() => id == "foo");
                string barId = foo.bar._id;
                Verify.That(() => barId == "1");
            }
            
            public void tags_with_same_name_is_collected_in_list()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar id='1'/>
    <bar id='2'/>
</foo>");
                string id = foo._id;
                Verify.That(() => id == "foo");

                var bars = ((IList<dynamic>)foo.bar);
                Verify.That(() => bars.Count == 2);

                var ids = bars.Select(x => x._id);
                Verify.That(() => string.Join(",", ids) == "1,2");
            }


            public void nesting_3_levels_deep_with_attributes()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar id='1'>
        <baz id='2' />
    </bar>
    <bar id='2'>
        <baz id='3' />
    </bar>
</foo>");
                string id = foo._id;
                Verify.That(() => id == "foo");

                var bars = ((IList<dynamic>)foo.bar);
                Verify.That(() => bars.Count == 2);

                var ids = bars.Select(x => x._id);
                Verify.That(() => string.Join(",", ids) == "1,2");
            }

        }

        [Context("Snake casing")]
        public class SnakeCasing
        {
            public void tag_names_should_be_snake_cased()
            {
                var foo_snake = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo-snake>
    <bar-case>
        <baz-thing>Yes</baz-thing>
    </bar-case>
</foo-snake>");

                string baz_thing = foo_snake.bar_case.baz_thing.__value;
                Verify.That(() => baz_thing == "Yes");
            }
        }

        private static dynamic GetDynamic(string input)
        {
            var doc = XDocument.Load(new StringReader(input));
            var foo = Convert.FromXml(doc.Root);
            return foo;
        }
    }
}
