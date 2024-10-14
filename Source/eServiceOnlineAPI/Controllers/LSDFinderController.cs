using eServiceOnline.WebAPI.Data.LSDFinder;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace eServiceOnline.WebAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LSDFinderController:ControllerBase
    {
        //Sample: http://localhost:52346/LSDFinder/Process_SESI_DW_Dim_WellLocations
        public ActionResult Process_SESI_DW_Dim_WellLocations()
        {
            bool disableQuery = false;
            string message = "Succeed";
            bool result = true;
            int processedRecordsCount = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(WellLocationEntity.staticConnectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(WellLocationEntity.staticQueryString, connection)) // Where AddedOrder = 2788
                    {
                        using (DataSet ds = new DataSet("DW"))
                        {
                            da.FillSchema(ds, SchemaType.Source, "WellLocations");
                            da.Fill(ds, "WellLocations");

                            using (DataTable tbl = ds.Tables["WellLocations"])
                            {
                                List<WellLocationEntity> wleList = WellLocationEntity.GetWellLocationEntities(tbl);
                                processedRecordsCount = wleList.Count;

                                foreach (WellLocationEntity wle in wleList)
                                {
                                    wle.LsdFinderFlag = 1;
                                    wle.ParseWellLocation();
                                    if (wle.WellLocationId > 1 && !string.IsNullOrEmpty(wle.NormalizedWellLocation) && wle.NormalizedWellLocation != "NA")
                                    {
                                        if (!wle.TryUpdatingFromExistingRecord())
                                        {
                                            if (wle.Latitude == null || wle.Latitude == 0)
                                            {
                                                if (disableQuery)
                                                    continue;

                                                if (!wle.GetLatLongFromWeb())
                                                {
                                                    //Log processing error (without setting LsdFinderFlag)
                                                    wle.LsdFinderFlag = 0;
                                                    //Disable query for all following records in the pipe (process only records that do not require query). 
                                                    disableQuery = true;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //wle.ProcessingNotes = string.IsNullOrEmpty(wle.ProcessingNotes) ? "Latitude/Longitude are already known." : wle.ProcessingNotes + " Latitude/Longitude are already known.";
                                                wle.ProcessingNotes = wle.BuildNotes(wle.ProcessingNotes, "Latitude/Longitude are already known.");
                                            }
                                        }
                                    }
                                    wle.UpdateDBRecord();
                                }
                                SqlCommandBuilder objCmdBuilder = new SqlCommandBuilder(da);
                                da.Update(ds, "WellLocations");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message + "\n\n" + ex.StackTrace;
            }
            return new JsonResult(new { result, processedRecordsCount, message });
        }

        //Sample: http://localhost:52346/LSDFinder/UpdateNormalizedWellLocationsInSESI_DW_Dim_WellLocations
        public ActionResult UpdateNormalizedWellLocationsInSESI_DW_Dim_WellLocations()
        {
            string message = "Succeed";
            bool result = true;
            int processedRecordsCount = 0;

            try
            {
                processedRecordsCount = WellLocationEntity.UpdateNormalizedWellLocationsInDB();
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }
            return new JsonResult(new { result, processedRecordsCount, message });
        }
    }
}
