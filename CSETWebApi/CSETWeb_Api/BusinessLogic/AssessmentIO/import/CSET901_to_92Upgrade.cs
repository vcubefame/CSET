using System;
using CSETWeb_Api.BusinessLogic.ImportAssessment;
using Newtonsoft.Json.Linq;

namespace CSETWeb_Api.BusinessLogic.BusinessManagers
{
    internal class CSET901_to_92Upgrade : ICSETJSONFileUpgrade
    {
        /// <summary>
        /// this is the string we will be upgrading to
        /// </summary>
        static string version = "9.2";

        public string ExecuteUpgrade(string json)
        {
            JObject oAssessment = JObject.Parse(json);

            // do the manipulations here

            var answers = oAssessment.SelectTokens("$.jANSWER");
            foreach (var answer in answers)
            {
                answer.SelectToken("$.Component_Guid").Value<string>(Guid.Empty);
            }


            return oAssessment.ToString();
        }

        public System.Version GetVersion()
        {
            return System.Version.Parse(version);
        }

        public string GetVersionString()
        {
            return version;
        }
    }
}