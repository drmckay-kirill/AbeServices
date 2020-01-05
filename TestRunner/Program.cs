using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Exceptions;
using NBomber.Contracts;
using NBomber.CSharp;

namespace TestRunner
{
    class Program
    {
        const int AttributesCount = 50; // 50

        static void Main(string[] args)
        {
            Console.WriteLine("Starting load testing of ABE services...");
            
            //KeyServiceScenario("sc1", 1, 5);
            //KeyServiceScenario("sc2", 10, 5);
        }

        static void KeyServiceScenario(string prefix, int copies, int duration)
        {
            string abonent = "test_runner";
            string keyServiceUrl = "http://localhost:5000/api/keys";
            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string keyService = "MachineService";
            string authority = "AttributeAuthority";

            var testRunnerData = GetTestRunnerAttrbutePackages();

            foreach(var attrsData in testRunnerData)
            {
                var scenarionName = $"{prefix}_scenario_{attrsData.Length}";
                var stepName = $"{prefix}_step_{attrsData.Length}";

                var step = Step.Create(stepName, async context =>
                {
                    var client = new HttpClient();
                    var encryptor = new DataSymmetricEncryption();
                    var serializer = new ProtobufDataSerializer();
                    var builder = new KeyDistributionBuilder(serializer, encryptor);
            
                    var (firstRequestData, abonentNonce) = builder.BuildStepOne(abonentKey, abonent, keyService, authority, attrsData);
                    //Console.WriteLine($"Generated nonce = {abonentNonce}");

                    var requestContent = new ByteArrayContent(firstRequestData);
                    var response = await client.PostAsync(keyServiceUrl, requestContent);
                    //Console.WriteLine($"Http response status code = {response.StatusCode}");
                    
                    //var responseData = await response.Content.ReadAsByteArrayAsync(); 
                    //var payload = builder.GetPayload<KeyDistributionAuthToAbonent>(responseData, abonentKey);
                    //Console.WriteLine($"Received nonce = {payload.Nonce}");
                    
                    return Response.Ok();
                });

                var scenario = ScenarioBuilder
                    .CreateScenario(scenarionName, new[] { step })
                    .WithConcurrentCopies(copies)
                    .WithDuration(TimeSpan.FromSeconds(duration));
                NBomberRunner
                    .RegisterScenarios(scenario)
                    .RunTest();
            }
        }

        static List<string[]> GetTestRunnerAttrbutePackages()
        {
            string[] attrs = new string[AttributesCount];
            for(int i = 0; i < AttributesCount; i++)
            {
                string attrName = $"iot_{i+1}";
                attrs[i] = attrName;
            }

            var res = new List<string[]>();
            for(int i = 0; i < AttributesCount; i++)
            {
                var attrsPackage = new string[i+1];
                Array.Copy(attrs, 0, attrsPackage, 0, i + 1);
                res.Add(attrsPackage);
            }

            return res;
        }
    }
}
