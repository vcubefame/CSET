//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWeb_Api.BusinessLogic.Models;
using CSETWeb_Api.Controllers.sal;
using CSETWeb_Api.Helpers;
using DataLayerCore.Model;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers.Sal
{
    public class GeneralSalController : ControllerBase
    {
        private CSET_Context db = new CSET_Context();


        [Route("api/GeneralSal/Descriptions")]
        public IActionResult GetGeneralSalDescriptionsWeights()
        {
            int assessmentid = Auth.AssessmentForUser();

            //TODO: make this async
            TinyMapper.Bind<GENERAL_SAL_DESCRIPTIONS, GeneralSalDescriptionsWeights>();
            TinyMapper.Bind<GEN_SAL_WEIGHTS, GenSalWeights>();

            List<GenSalPairs> result = new List<GenSalPairs>();

            var sliders = from d in db.GENERAL_SAL_DESCRIPTIONS
                             from g in db.GENERAL_SAL.Where(g => g.Assessment_Id == assessmentid && g.Sal_Name == d.Sal_Name).DefaultIfEmpty()
                             orderby d.Sal_Order
                             select new GenSalCategory
                             {
                                 d = d,
                                 SliderValue = (int?)g.Slider_Value
                             };

            bool first = true;
            GenSalPairs pair = null;

            foreach (var slider in sliders.ToList())
            {
                GeneralSalDescriptionsWeights s = TinyMapper.Map<GeneralSalDescriptionsWeights>(slider.d);
                if (first)
                {
                    pair = new GenSalPairs();
                    pair.OnSite = s;
                    result.Add(pair);
                }
                else
                {
                    pair.OffSite = s;
                }
                first = !first;

                s.values = new List<string>();
                s.Slider_Value = slider.SliderValue ?? 0;
                foreach (GEN_SAL_WEIGHTS w in db.GEN_SAL_WEIGHTS.Where(x => String.Equals(x.Sal_Name, slider.d.Sal_Name)))
                {
                    s.GEN_SAL_WEIGHTS.Add(TinyMapper.Map<GenSalWeights>(w));
                    s.values.Add(" " + w.Display + " ");
                }
            }

            return Ok(result);
        }


        [Route("api/GeneralSal/SaveWeight")]
        public IActionResult PostSaveWeight(SaveWeight ws)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int assessmentid = Auth.AssessmentForUser();
                ws.assessmentid = assessmentid;
                using (CSET_Context db = new CSET_Context())
                {
                    GeneralSalManager salManager = new GeneralSalManager(db);
                    string salvalue = salManager.SaveWeightAndCalculate(ws);
                    return Ok(salvalue);
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Error saving sal: {e.Message}");
            }
            
            
        }

        [Route("api/GeneralSal/Value")]
        public IActionResult GetValue()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (CSET_Context db = new CSET_Context())
                {
                    GeneralSalManager salManager = new GeneralSalManager(db);

                    int assessmentId = Auth.AssessmentForUser();
                    string salvalue = salManager.GetSavedSALValue(assessmentId);
                    return Ok(salvalue);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting sal: {ex.Message}");
            }
        }

        // DELETE: api/GeneralSal/5
        public async Task<IActionResult> DeleteGENERAL_SAL(int id)
        {
            try
            {
                GENERAL_SAL gENERAL_SAL = await db.GENERAL_SAL.FindAsync(id);
                if (gENERAL_SAL == null)
                {
                    return NotFound();
                }

                db.GENERAL_SAL.Remove(gENERAL_SAL);
                await db.SaveChangesAsync();

                return Ok(gENERAL_SAL);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting sal: {e.Message}");
            }
            
        }
    }

    public class GenSalCategory
    {
        public GENERAL_SAL_DESCRIPTIONS d;
        public int? SliderValue;
    }
}

