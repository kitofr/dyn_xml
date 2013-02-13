﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Cone;
using Dynamic;

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
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>1</bar>
</foo>");
                string bar = foo.bar;
                Verify.That(() => bar == "1");
            }

            public void with_two_elements()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>1</bar>
    <baz>2</baz>
</foo>");
                string bar = foo.bar;
                Verify.That(() => bar == "1");

                string baz = foo.baz;
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
                Verify.That(() => string.Join(",", bars.Select(x => x)) == "1,2");
            }

            public void that_is_3_levels_deep()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo>
    <bar>
        <baz>Yes</baz>
    </bar>
</foo>");
                string baz = foo.bar.baz;
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

                var res = string.Join(",", bars.Select(x => x.baz));
                Verify.That(() => res == "Yes,No");
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
                Verify.That(() => string.Join(",", bars.Select(x => x)) == "1,2");
                string baz = foo.baz;
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
                string id = foo.foo_id;
                Verify.That(() => id == "foo");
            }

            [Pending]
            public void top_node_as_container_with_attributes()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar id='1'/>
</foo>");
                string id = foo.foo_id;
                Verify.That(() => id == "foo");
                string barId = foo.bar.bar_id;
                Verify.That(() => barId == "1");
            }
            [Pending]
            public void tags_with_same_name_is_collected_in_list()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar id='1'/>
    <bar id='2'/>
</foo>");

                var bars = ((IList<dynamic>)foo.bar);
                Verify.That(() => bars.Count == 2);

                var ids = bars.Select(x => x.bar_id);
                Verify.That(() => string.Join(",", ids) == "1,2");
            }
            [Pending]
            public void tags_with_same_name_can_have_value()
            {
                var foo = GetDynamic(@"<?xml version='1.0' encoding='utf-8'?>
<foo id='foo'>
    <bar>Yes</bar>
    <bar>No</bar>
</foo>");
                var bars = ((IList<dynamic>)foo.bar);
                Verify.That(() => bars.Count == 2);

                var values = bars.Select(x => x);
                Verify.That(() => string.Join(",", values) == "Yes,No");
            }
        }

        [Context("Snake casing")]
        public class SnakeCasing
        {
            
        }

        private static dynamic GetDynamic(string input)
        {
            var doc = XDocument.Load(new StringReader(input));
            var foo = Convert.FromXml(doc.Root);
            return foo;
        }
    }
}
