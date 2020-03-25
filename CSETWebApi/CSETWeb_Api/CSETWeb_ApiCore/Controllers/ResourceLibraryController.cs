//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSET_Main.Common;
using CSET_Main.Data.ControlData;
using DataLayerCore.Model;
using Lucene.Net.Store;
using ResourceLibrary.Nodes;
using ResourceLibrary.Search;
using System;
using System.Collections.Generic;
using System.IO;
using CSET_Main.Questions.InformationTabData;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    public class ResourceLibraryController : ControllerBase
    {
        /// <summary>
        /// Returns the details under a given question
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/ResourceLibrary")]
        public List<ResourceNode> GetDetails([FromBody] SearchRequest searchRequest)
        {
            if (String.IsNullOrWhiteSpace(searchRequest.term))
                return new List<ResourceNode>();

            Lucene.Net.Store.Directory fsDir = FSDirectory.Open(new DirectoryInfo(Path.Combine(CSETGlobalProperties.Static_Application_Path, "LuceneIndex")));
            
            CSETGlobalProperties props = new CSETGlobalProperties();
            using (CSET_Context context = new CSET_Context()) {
                SearchDocs search = new SearchDocs(props, new ResourceLibraryRepository(context, props));
                return search.Search(searchRequest);
            }
        }

        [HttpGet]
        [Route("api/ShowResourceLibrary")]
        public IActionResult ShowResourceLibrary()
        {
            var buildDocuments = new QuestionInformationTabData().GetBuildDocuments();
            return Ok(buildDocuments != null && buildDocuments.Count > 100);
        }

        [HttpGet]
        [Route("api/ResourceLibrary/tree")]
        public List<SimpleNode> GetTree()
        {
            using (CSET_Context context = new CSET_Context()) {
                IResourceLibraryRepository resource = new ResourceLibraryRepository(context,new CSETGlobalProperties());
                return resource.GetTreeNodes();
            }
        }

        [HttpGet]
        [Route("api/ResourceLibrary/doc")]
        public string GetFlowDoc([FromQuery] string type, [FromQuery] int id)
        {
            // pull the flowdoc from the database
            BusinessManagers.FlowDocManager fdm = new BusinessManagers.FlowDocManager();
            string html = fdm.GetFlowDoc(type, id);

            return html;            
        }
    }
}


