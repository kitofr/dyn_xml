using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DynXml
{
    class Program
    {
        static void Main()
        {
            PrintQuestions(Dynamic.Convert.FromXml(XDocument.Load("./examples/questions.xml").Root));

            Print3TargetGroups(Dynamic.Convert.FromXml(XDocument.Load("./examples/3-target-groups.xml").Root));

            PrintChina(Dynamic.Convert.FromXml(XDocument.Load("./examples/china.xml").Root));

            //PrintFeasibilityResponseBatch(Dynamic.Convert.FromXml(XDocument.Load("./examples/feasibility-response-batch.xml").Root));
            
            
            Console.ReadKey();
        }

        private static void PrintQuestions(dynamic project)
        {
            Console.WriteLine(" ------------- Print Questions ------------- ");
            var targetGroup = project.target_groups.target_group;
            var question = targetGroup.questions.question;

            Console.WriteLine("question name: {0}", question._name);
            foreach (var answer in question.answer)
            {
                Console.WriteLine("\t - answer: {0}", answer._label);
            }

        }

        private static void Print3TargetGroups(dynamic project)
        {
            Console.WriteLine(" ------------- Print 3 Target Groups ------------- ");

            Console.WriteLine("name: {0}", project._name);
            Console.WriteLine("user: {0}", project.user._id);
            Console.WriteLine("contact[email]: {0}", project.contact._email);
            Console.WriteLine("contact[phone]: {0}", project.contact._phone);
            Console.WriteLine("status: {0}", project.status.__value);
            Console.WriteLine("completes[actual]: {0}", project.completes._actual);
            Console.WriteLine("completes[wanted]: {0}", project.completes._wanted);
            Console.WriteLine("price[price]: [min:{0},max:{1}]", project.price.price._min, project.price.price._max);
            Console.WriteLine("price[discount]: [min:{0},max:{1}]", project.price.discount._min, project.price.discount._max);
            Console.WriteLine("price[agreed]: [min:{0},max:{1}]", project.price.agreed._min, project.price.agreed._max);

            Console.WriteLine("start date: {0}", project.start_date.__value);
            Console.WriteLine("end date: {0}", project.end_date.__value);

            Console.WriteLine("#target groups: {0}", project.target_groups.target_group.Count);
            foreach (var @group in project.target_groups.target_group)
            {
                Console.WriteLine("\ttarget group: [id:{0}, name:{1}]", @group._id,
                                  @group._name);
                Console.WriteLine("\tcountry: [id:{0}, name:{1}]", @group.country._id, @group.country._name);
                Console.WriteLine("\tstatus: {0}", @group.status.__value);
                Console.WriteLine("\tstart date: {0}", @group.start_date.__value);
                Console.WriteLine("\tend date: {0}", @group.end_date.__value);
                Console.WriteLine("\tincidence: [actual:{0}, estimated: {1}]", @group.incidence_rate._actual,
                                  @group.incidence_rate._estimated);
                Console.WriteLine("\tloi: [actual:{0}, estimated: {1}]", @group.lenght_of_interview._actual,
                                  @group.lenght_of_interview._estimated);
                Console.WriteLine("\tgender: {0}", @group.gender.__value);
                Console.WriteLine("\tcompletes: [wanted:{0},actual:{1}]", @group.completes._wanted,
                                  @group.completes._actual);
                Console.WriteLine("\tage-range: [min:{0},max:{1}]", @group.age_range._min, @group.age_range._max);

                foreach (var panel in @group.panels.panel)
                    Console.WriteLine("\tpanel name: {0}", panel._name);

                if (((IDictionary<string, dynamic>)@group.regions).ContainsKey("region"))
                {
                    if (@group.regions.region is IList<dynamic>)
                        foreach (var region in @group.regions.region)
                            Console.WriteLine("\tregion name: {0}", region._name);
                    else
                        Console.WriteLine("\tregion name: {0}", @group.regions.region._name);
                }
                else
                {
                    Console.WriteLine("\tNo regions!");
                }

                if (((IDictionary<string, dynamic>)@group.questions).ContainsKey("question"))
                {
                    if (@group.regions.region is IList<dynamic>)
                        foreach (var question in @group.questions.question)
                        {
                            Console.WriteLine("\tquestion name: {0}", question._name);
                            foreach (var answer in question.answer)
                            {
                                Console.WriteLine("\t - answer: {0}", answer.__value);
                            }
                        }
                    else
                    {
                        Console.WriteLine("\tquestion name: {0}", @group.questions.question._name);
                    }
                }
                else
                {
                    Console.WriteLine("\tNo questions!");
                }
                Console.WriteLine("\t---------------------------------------------");
            }
        }

        private static void PrintChina(dynamic project)
        {
            Console.WriteLine(" ------------- Print China ------------- ");
            foreach (var @group in project.target_groups.target_group){
                Console.WriteLine("Target Group: {0}", @group._name);

                IDictionary<string, dynamic> quotas = @group.quotas;
                if (quotas.ContainsKey("quota"))
                {
                    if (@group.quotas.quota is IList<dynamic>)
                        foreach (var quota in @group.quotas.quota)
                        {
                            Console.WriteLine("\tquota: [gender: {0}, min: {1}, max: {2}]", quota._gender, quota._min, quota._max);
                            Console.WriteLine("\t - completes: [wanted: {0}, actual: {1}]", quota.completes._wanted, quota.completes._actual);
                        }
                    else
                    {
                        dynamic quota = @group.quotas.quota;
                        Console.WriteLine("\tquota: [gender: {0}, min: {1}, max: {2}]", quota._gender, quota._min, quota._max);
                        Console.WriteLine("\t - completes: [wanted: {0}, actual: {1}]", quota.completes._wanted, quota.completes._actual);
                    }
                }
                foreach (var panel in @group.panels.panel)
                    Console.WriteLine("\tpanel name: {0}", panel._name);


                IDictionary<string, dynamic> regions = @group.regions;
                if (regions.ContainsKey("region"))
                {
                    if(@group.regions.region is IList<dynamic>)
                        foreach (var region in @group.regions.region)
                            Console.WriteLine("\tregion name: {0}", region._name);
                    else
                        Console.WriteLine("\tregion name: {0}", @group.regions.region._name);
                        
                    
                }
                Console.WriteLine("\t-------------------------");
            }
        }

        private static void PrintFeasibilityResponseBatch(dynamic batch)
        {
            foreach (var response in batch.feasibility_response)
            {
                Console.WriteLine("Request id: {0}, Count: {1}, Average ResponseRate: {2}", response._request_id, response._count, response._average_response_rate);
                foreach (var panel in response.panels.panel_result)
                {
                    Console.WriteLine("id: {0}, count: {1}, average response rate: {2}", panel._id, panel._count, panel._average_response_rate);
                }
                Console.WriteLine("\t------------------------------------");
            }
        }

        
    }
}

