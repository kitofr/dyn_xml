using System;
using System.Xml.Linq;

namespace DynXml
{
    class Program
    {
        static void Main()
        {
            var doc = XDocument.Load("./examples/3-target-groups.xml");
            //var doc = XDocument.Load("./examples/china.xml");
            //var doc = XDocument.Load("./examples/questions.xml");
            var project = Dynamic.Convert.FromXml(doc.Root);

            //PrintQuestions(project);
            //PrintChina(project);
            Print3TargetGroups(project);
            Console.ReadKey();
        }

        private static void PrintChina(dynamic project)
        {
            foreach (var target_group in project.target_groups){
                Console.WriteLine("Target Group: {0}", target_group.target_group_name);
                foreach (var quota in target_group.quotas)
                {
                    Console.WriteLine("\tquota: [gender: {0}, min: {1}, max: {2}]", quota.quota_gender, quota.quota_min, quota.quota_max);
                    Console.WriteLine("\t - completes: [wanted: {0}, actual: {1}]", quota.completes_wanted, quota.completes_actual);
                }
                foreach (var panel in target_group.panels)
                    Console.WriteLine("\tpanel name: {0}", panel.panel_name);
                foreach (var region in target_group.regions)
                    Console.WriteLine("\tregion name: {0}", region.region_name);
                
                Console.WriteLine("\t-------------------------");
            }
        }

        private static void PrintFeasibilityResponseBatch(dynamic batch)
        {
            
        }

        private static void PrintQuestions(dynamic project)
        {
            foreach (var target_group in project.target_groups)
                foreach (var question in target_group.questions)
                {
                    Console.WriteLine("\tquestion name: {0}", question.question_name);
                    foreach (var answer in question.answer)
                    {
                        Console.WriteLine("\t - answer: {0}", answer.answer_label);
                    }
                }
        }

        private static void Print3TargetGroups(dynamic project)
        {
            Console.WriteLine("name: {0}", project.project_name);
            Console.WriteLine("user: {0}", project.user_id);
            Console.WriteLine("contact[email]: {0}", project.contact_email);
            Console.WriteLine("contact[phone]: {0}", project.contact_phone);
            Console.WriteLine("completes[actual]: {0}", project.completes_actual);
            Console.WriteLine("completes[wanted]: {0}", project.completes_wanted);
            Console.WriteLine("price[price]: [min:{0},max:{1}]", project.price.price_min, project.price.price_max);
            Console.WriteLine("price[discount]: [min:{0},max:{1}]", project.price.discount_min, project.price.discount_max);
            Console.WriteLine("price[agreed]: [min:{0},max:{1}]", project.price.agreed_min, project.price.agreed_max);
            Console.WriteLine("status: {0}", project.status);
            Console.WriteLine("start date: {0}", project.start_date);
            Console.WriteLine("end date: {0}", project.end_date);
            Console.WriteLine("#target groups: {0}", project.target_groups.Count);
            foreach (var target_group in project.target_groups)
            {
                Console.WriteLine("\ttarget group: [id:{0}, name:{1}]", target_group.target_group_id,
                                  target_group.target_group_name);
                Console.WriteLine("\tcountry: [id:{0}, name:{1}]", target_group.country_id, target_group.country_name);
                Console.WriteLine("\tstatus: {0}", target_group.status);
                Console.WriteLine("\tstart date: {0}", target_group.start_date);
                Console.WriteLine("\tend date: {0}", target_group.end_date);
                Console.WriteLine("\tincidence: [actual:{0}, estimated: {1}]", target_group.incidence_rate_actual,
                                  target_group.incidence_rate_estimated);
                Console.WriteLine("\tloi: [actual:{0}, estimated: {1}]", target_group.lenght_of_interview_actual,
                                  target_group.lenght_of_interview_estimated);
                Console.WriteLine("\tgender: {0}", target_group.gender);
                Console.WriteLine("\tcompletes: [wanted:{0},actual:{1}]", target_group.completes_wanted,
                                  target_group.completes_actual);
                Console.WriteLine("\tage-range: [min:{0},max:{1}]", target_group.age_range_min, target_group.age_range_max);

                foreach (var panel in target_group.panels)
                    Console.WriteLine("\tpanel name: {0}", panel.panel_name);
                foreach (var region in target_group.regions)
                    Console.WriteLine("\tregion name: {0}", region.region_name);
                foreach (var question in target_group.questions)
                {
                    Console.WriteLine("\tquestion name: {0}", question.question_name);
                    //Console.WriteLine("\t#answer count: {0}", question.answer.Count);
                    foreach (var answer in question.answer)
                    {
                        Console.WriteLine("\t - answer: {0}", answer);
                    }
                }
                Console.WriteLine("\t---------------------------------------------");
            }
        }
    }
}

