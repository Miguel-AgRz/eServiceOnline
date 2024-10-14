using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text.RegularExpressions;
using eServiceOnline.WebAPI.Data.SharePointSync;

namespace eServiceOnline.WebAPI.Data.LSDFinder
{
    public class WellLocationEntity
    {
        static HttpClient client; // = new HttpClient();

        public static string staticConnectionString = SharePointUtils.GetConnectionString(@"sanjel27\DW", "SESI_DW");
        public static string staticQueryString = "Select * From dbo.Dim_WellLocations Where IsNull(LsdFinderFlag, 0) = 0";
        // ...gridatlas.com/api/public/v2/ga_11f74b8e6bb/lookup/lsd/14-22-25-2%20W5
        public static string staticBaseAddress = "https://www.gridatlas.com/api/public/v2/ga_11f74b8e6bb/lookup/";

        public string WellLocation { get; set; }
        public int? WellLocationId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? LSD_Mer { get; set; }
        public int? LSD_RGE { get; set; }
        public int? LSD_TWP { get; set; }
        public int? NTS_Map { get; set; }
        public string NTS_MapSheet { get; set; }
        public int? NTS_Grid { get; set; }
        public int AddedOrder { get; set; }
        public string NormalizedWellLocation { get; set; }
        public string ProcessingNotes { get; set; }
        public int? LsdFinderFlag { get; set; }
        public DateTime? ProcessingDateTime { get; set; }

        public DataRow AssociatedDataRow { get; set; }

        private WellLocationEntity()
        {
        }

        public WellLocationEntity(DataRow dr)
        {
            if (client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(staticBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (dr != null)
            {
                WellLocation = dr["WellLocation"].ToString();
                WellLocationId = dr["WellLocationId"] == DBNull.Value ? null : (int?)dr["WellLocationId"];
                Latitude = dr["Latitude"] == DBNull.Value ? null : (decimal?)dr["Latitude"];
                Longitude = dr["Longitude"] == DBNull.Value ? null : (decimal?)dr["Longitude"];
                LSD_Mer = dr["LSD_Mer"] == DBNull.Value ? null : (int?)dr["LSD_Mer"];
                LSD_RGE = dr["LSD_RGE"] == DBNull.Value ? null : (int?)dr["LSD_RGE"];
                LSD_TWP = dr["LSD_TWP"] == DBNull.Value ? null : (int?)dr["LSD_TWP"];
                NTS_Map = dr["NTS_Map"] == DBNull.Value ? null : (int?)dr["NTS_Map"];
                NTS_MapSheet = dr["NTS_MapSheet"] == DBNull.Value ? null : dr["NTS_MapSheet"].ToString();
                NTS_Grid = dr["NTS_Grid"] == DBNull.Value ? null : (int?)dr["NTS_Grid"];
                AddedOrder = (int)dr["AddedOrder"];
                NormalizedWellLocation = dr["NormalizedWellLocation"].ToString();
                ProcessingNotes = dr["ProcessingNotes"].ToString();
                LsdFinderFlag = dr["LsdFinderFlag"] == DBNull.Value ? null : (int?)dr["LsdFinderFlag"];
                ProcessingDateTime = dr["ProcessingDateTime"] == DBNull.Value ? null : (DateTime?)dr["ProcessingDateTime"];

                AssociatedDataRow = dr;
            }
        }

        public bool ParseWellLocation()
        {
            bool result = true;

            //if (string.IsNullOrEmpty(NormalizedWellLocation) || NormalizedWellLocation == "NA")
            //{
            //    ExtractNormalizedWellLocation();
            //}
            ExtractNormalizedWellLocation();
            ParseWellLocationComponents();
            return result;
        }

        public void ExtractNormalizedWellLocation()
        {
            string lsdRegEx = @"\d{1,2}[- ]\d{1,2}[- ]\d{1,3}[- ]\d{1,2}[- ]?W[- ]?\d{1}";
            string ntsRegEx = @"[A-Da-d][- ][A-Z]?\d{1,3}[- ][A-La-l] ?[/-] ?\d{2,3}[- ][A-Pa-p][- ]\d{1,2}"; //X-X000-X/00-X-00
            MatchCollection mc;

            string wellLocationToParse;

            if (!string.IsNullOrEmpty(NormalizedWellLocation) && NormalizedWellLocation != "NA")
                wellLocationToParse = NormalizedWellLocation;
            else
                wellLocationToParse = WellLocation;

            mc = Regex.Matches(wellLocationToParse, lsdRegEx);
            if (mc.Count == 1)
            {
                NormalizedWellLocation = mc[0].ToString();
                WellLocationId = 3;
            }
            else
            {
                mc = Regex.Matches(wellLocationToParse, ntsRegEx);
                if (mc.Count == 1)
                {
                    string temp = mc[0].ToString();
                    MatchCollection mc_start = Regex.Matches(temp, @"^[A-Da-d]");
                    MatchCollection mc_end = Regex.Matches(temp, @"\d{1,3}[- ][A-La-l] ?[/-] ?\d{2,3}[- ][A-Pa-p][- ]\d{1,2}$");
                    if (mc_start.Count > 0 && mc_end.Count > 0)
                    {
                        NormalizedWellLocation = mc_start[0].ToString() + "-" + mc_end[0].ToString();
                        WellLocationId = 5;
                    }
                }
                else
                {
                    NormalizedWellLocation = "NA";
                    WellLocationId = 1;
                    //ProcessingNotes = "Cannot be parsed.";
                    ProcessingNotes = BuildNotes(ProcessingNotes, "Cannot be parsed.");
                }
            }
        }

        private bool ParseWellLocationComponents()
        {
            bool result = true;

            string mer;
            int merNum;
            int rge;
            int twp;
            int map;
            int grid;

            try
            {
                if (!string.IsNullOrEmpty(NormalizedWellLocation) && NormalizedWellLocation != "NA")
                {
                    var location = NormalizedWellLocation.ToUpper();

                    if (WellLocationId == 3)
                    {
                        location = location.Replace(" ", "-");

                        mer = location.Substring(IndexOfN(location, "W", 1) + 1, 1);
                        if (
                            (mer != "-" && int.TryParse(mer, out merNum))
                            || (mer == "-" && int.TryParse(location.Substring(IndexOfN(location, "W", 1) + 2, 1), out merNum))
                           )
                            LSD_Mer = merNum;

                        if (int.TryParse(location.Substring(IndexOfN(location, "-", 3) + 1,
                            IndexOfN(location, "W", 1) - IndexOfN(location, "-", 3) -
                            (IndexOfN(location, "-", 4) == -1 ? 1 : 2)), out rge))
                            LSD_RGE = rge;

                        if (int.TryParse(location.Substring(IndexOfN(location, "-", 2) + 1,
                            IndexOfN(location, "-", 3) - IndexOfN(location, "-", 2) - 1), out twp))
                            LSD_TWP = twp;
                    }
                    if (WellLocationId == 5)
                    {
                        MatchCollection ms = Regex.Matches(location, @"\d{2,3}[- ][A-Pa-p][- ]\d{1,2}$");
                        if (ms.Count > 0)
                        {
                            location = ms[0].ToString();

                            ms = Regex.Matches(location, @"^\d{2,3}");
                            if (ms.Count > 0 && int.TryParse(ms[0].ToString(), out map))
                                NTS_Map = map;

                            ms = Regex.Matches(location, @"[A-Pa-p]");
                            if (ms.Count > 0)
                                NTS_MapSheet = ms[0].ToString();

                            ms = Regex.Matches(location, @"\d{1,2}$");
                            if (ms.Count > 0 && int.TryParse(ms[0].ToString(), out grid))
                                NTS_Grid = grid;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //ProcessingNotes += WellLocation + " - " + WellLocationId.ToString() + " - " + e.Message;
                ProcessingNotes = BuildNotes(ProcessingNotes, WellLocation + " - " + WellLocationId.ToString() + " - " + e.Message);
                result = false;
            }
            return result;
        }

        public bool GetLatLongFromWeb()
        {
            bool result = true;

            string param = getLocationType(WellLocationId);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            string responseReasonPhrase = "Cannot identify location type";

            if (!String.IsNullOrEmpty(param))
            {
                param += NormalizedWellLocation.Replace("/", "%2F").ToUpper();
                HttpResponseMessage response = client.GetAsync(param).Result; // Blocking call!

                responseReasonPhrase = response.ReasonPhrase;

                if (response.IsSuccessStatusCode)
                {
                    var dataObject = response.Content.ReadAsAsync<GridAtlasRootObject>().Result;

                    if (dataObject != null && dataObject.success)
                    {
                        if (dataObject.data.meta.queries > 0 && dataObject.data.queries[0].result.meta.success)
                        {
                            Latitude = (decimal)dataObject.data.queries[0].result.gps.lat;
                            Longitude = (decimal)dataObject.data.queries[0].result.gps.lng;
                        }

                        try
                        {
                            int remaining = dataObject.data.remaining_lookups ?? 0;
                            //ProcessingNotes = "Queried. " + remaining.ToString();
                            ProcessingNotes = BuildNotes(ProcessingNotes, "Queried. " + remaining.ToString());
                        }
                        catch
                        {
                            //ProcessingNotes = "Queried. Cannot get remaining paid requests number";
                            ProcessingNotes = BuildNotes(ProcessingNotes, "Queried. Cannot get remaining paid requests number");
                        }
                    }
                    else
                    {
                        result = false;
                        //ProcessingNotes = "Queried. Processing Aborted. GridAtlas query failed with respond : " + responseReasonPhrase;
                        ProcessingNotes = BuildNotes(ProcessingNotes, "Queried. Processing Aborted. GridAtlas query failed with respond : " + responseReasonPhrase);
                    }
                }
                else
                {
                    //Most likely number of allowed/paid requests is exeeded. Do thomething
                    //ProcessingNotes = "Queried. Processing Aborted. GridAtlas query failed with respond : " + responseReasonPhrase;
                    ProcessingNotes = BuildNotes(ProcessingNotes, "Queried. Processing Aborted. GridAtlas query failed with respond : " + responseReasonPhrase);
                    result = false;
                }
            }
            return result;
        }

        public string BuildNotes(string existingNotes, string newNotes)
        {
            return string.IsNullOrEmpty(existingNotes.Trim()) ? newNotes : existingNotes + " " + newNotes;
        }

        public bool UpdateDBRecord()
        {
            bool result = false;

            if (AssociatedDataRow != null)
            {
                try
                {
                    AssociatedDataRow.BeginEdit();

                    AssociatedDataRow["WellLocationId"] = WellLocationId != null ? (object)WellLocationId : DBNull.Value;
                    AssociatedDataRow["Latitude"] = Latitude != null ? (object)Latitude : DBNull.Value;
                    AssociatedDataRow["Longitude"] = Longitude != null ? (object)Longitude : DBNull.Value;

                    AssociatedDataRow["LSD_Mer"] = LSD_Mer != null ? (object)LSD_Mer : DBNull.Value;
                    AssociatedDataRow["LSD_RGE"] = LSD_RGE != null ? (object)LSD_RGE : DBNull.Value;
                    AssociatedDataRow["LSD_TWP"] = LSD_TWP != null ? (object)LSD_TWP : DBNull.Value;

                    AssociatedDataRow["NTS_Map"] = NTS_Map != null ? (object)NTS_Map : DBNull.Value;
                    AssociatedDataRow["NTS_MapSheet"] = NTS_MapSheet != null ? (object)NTS_MapSheet : DBNull.Value;
                    AssociatedDataRow["NTS_Grid"] = NTS_Grid != null ? (object)NTS_Grid : DBNull.Value;

                    AssociatedDataRow["NormalizedWellLocation"] = NormalizedWellLocation != null ? (object)NormalizedWellLocation : DBNull.Value;
                    AssociatedDataRow["ProcessingNotes"] = ProcessingNotes != null ? (object)ProcessingNotes : DBNull.Value;

                    AssociatedDataRow["LsdFinderFlag"] = LsdFinderFlag != null ? (object)LsdFinderFlag : DBNull.Value;
                    AssociatedDataRow["ProcessingDateTime"] = DateTime.Now;

                    AssociatedDataRow.EndEdit();

                    result = true;
                }
                catch
                {
                    AssociatedDataRow.CancelEdit();
                }
            }
            else
            {
                throw new Exception("UpdateDBRecord failed. WellLocationEntity does not have AssociatedDataRow assigned.");
            }

            return result;
        }

        public bool TryUpdatingFromExistingRecord()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(NormalizedWellLocation) && NormalizedWellLocation != "NA")
            {
                try
                {
                    WellLocationEntity wleExisting = GetExistingWellLocationEntity(NormalizedWellLocation);

                    if (UpdateWellLocationEntityFromAnother(wleExisting))
                    {
                        //ProcessingNotes = "Populated from already existing record.";
                        ProcessingNotes = BuildNotes(ProcessingNotes, "Populated from already existing record.");
                        LsdFinderFlag = 1;
                        result = true;
                    }
                    //result = UpdateDBRecord();
                }
                catch (Exception ex)
                {
                    //ProcessingNotes += " - Error : " + ex.Message;
                    ProcessingNotes = BuildNotes(ProcessingNotes, " - Error : " + ex.Message);
                }
            }

            return result;
        }

        private bool UpdateWellLocationEntityFromAnother(WellLocationEntity wle)
        {
            bool result = false;

            if (wle != null && NormalizedWellLocation == wle.NormalizedWellLocation)
            {
                //WellLocation = wle.WellLocation;
                WellLocationId = wle.WellLocationId;
                Latitude = wle.Latitude;
                Longitude = wle.Longitude;
                LSD_Mer = wle.LSD_Mer;
                LSD_RGE = wle.LSD_RGE;
                LSD_TWP = wle.LSD_TWP;
                NTS_Map = wle.NTS_Map;
                NTS_MapSheet = wle.NTS_MapSheet;
                NTS_Grid = wle.NTS_Grid;
                AddedOrder = wle.AddedOrder;
                NormalizedWellLocation = wle.NormalizedWellLocation;
                ProcessingNotes = wle.ProcessingNotes;
                LsdFinderFlag = wle.LsdFinderFlag;
                ProcessingDateTime = DateTime.Now;

                result = true;
            }
            else
            {
                if (wle != null)
                    throw new Exception("Cannot update. Source and Target WellLocationEntities have different value in key property NormalizedWellLocation.");
            }

            return result;
        }

        private static WellLocationEntity GetExistingWellLocationEntity(string normalizedWellLocationInput)
        {
            WellLocationEntity wle = null;

            if (!string.IsNullOrEmpty(normalizedWellLocationInput))
            {
                using (SqlConnection objConn = new SqlConnection(staticConnectionString))
                {
                    objConn.Open();
                    using (SqlDataAdapter daWellLocations = new SqlDataAdapter("Select * From dbo.Dim_WellLocations Where IsNull(LsdFinderFlag, 0) = 1 and IsNull(NormalizedWellLocation, '') = '" + normalizedWellLocationInput + "'", objConn)) // Where AddedOrder = 2788
                    {
                        using (DataSet dsDW = new DataSet("DW"))
                        {
                            daWellLocations.FillSchema(dsDW, SchemaType.Source, "WellLocations");
                            daWellLocations.Fill(dsDW, "WellLocations");

                            if (dsDW.Tables["WellLocations"].Rows.Count > 0)
                                wle = new WellLocationEntity(dsDW.Tables["WellLocations"].Rows[0]);
                        }
                    }
                }
            }
            return wle;
        }

        public static List<WellLocationEntity> GetWellLocationEntities(DataTable tbl)
        {
            List<WellLocationEntity> result = new List<WellLocationEntity>();

            if (tbl.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow dr in tbl.Rows)
                    {
                        WellLocationEntity wle = new WellLocationEntity(dr);
                        result.Add(wle);
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        public static int UpdateNormalizedWellLocationsInDB()
        {
            using (SqlConnection objConn = new SqlConnection(WellLocationEntity.staticConnectionString))
            {
                int cnt = 0;

                objConn.Open();
                using (SqlDataAdapter daWellLocations = new SqlDataAdapter("Select * From dbo.Dim_WellLocations Where IsNull(NormalizedWellLocation, '') = ''", objConn)) // Where AddedOrder = 2788
                {
                    using (DataSet dsDW = new DataSet("DW"))
                    {
                        daWellLocations.FillSchema(dsDW, SchemaType.Source, "WellLocations");
                        daWellLocations.Fill(dsDW, "WellLocations");


                        using (DataTable tbl = dsDW.Tables["WellLocations"])
                        {
                            List<WellLocationEntity> wleList = WellLocationEntity.GetWellLocationEntities(tbl);
                            cnt = wleList.Count;

                            foreach (WellLocationEntity wle in wleList)
                            {
                                //wle.LsdFinderFlag = 1;
                                //wle.ParseWellLocation();
                                wle.ExtractNormalizedWellLocation();
                                wle.UpdateDBRecord();
                            }
                            SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daWellLocations);
                            daWellLocations.Update(dsDW, "WellLocations");
                        }

                        return cnt;

                        /*
                                                using (DataTable tblWellLocations = dsDW.Tables["WellLocations"])
                                                {
                                                    string wellLocation = "";
                                                    int locationTypeId = 0;

                                                    foreach (DataRow dr in tblWellLocations.Rows)
                                                    {
                                                        ExtractWellLocation(dr["WellLocation"].ToString(), out wellLocation, out locationTypeId);

                                                        if (string.IsNullOrEmpty(wellLocation)) wellLocation = "NA";

                                                        dr.BeginEdit();
                                                        dr["NormalizedWellLocation"] = wellLocation;
                                                        //dr["WellLocationId"] = locationTypeId;
                                                        dr.EndEdit();
                                                    }
                                                }

                                                SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daWellLocations);
                                                daWellLocations.Update(dsDW, "WellLocations");
                        */
                    }
                }
            }
        }


        private static string getLocationType(int? locationTypeId)
        {
            string result = "";

            switch (locationTypeId)
            {
                case 3:
                case 13:
                case 33:
                    result = "lsd/";
                    break;
                case 5:
                case 15:
                case 55:
                    result = "nts/";
                    break;
                default:
                    result = "";
                    break;
            }

            return result;
        }

        private static int IndexOfN(string s, string match, int occurence)
        {
            int i = 1;
            int index = 0;

            while (i <= occurence && (index = s.IndexOf(match, index + 1)) != -1)
            {
                if (i == occurence)
                    return index;
                i++;
            }
            return -1;
        }
    }

    //public class Response
    //{
    //    public string status { get; set; }
    //    public List<object> err { get; set; }
    //    public Nullable<decimal> lat { get; set; }
    //    public Nullable<decimal> lng { get; set; }
    //    public string latlngms { get; set; }
    //    public string country { get; set; }
    //    public string province { get; set; }
    //    public string city { get; set; }
    //    public string street { get; set; }
    //    public Nullable<int> street_prox { get; set; }
    //    public object near { get; set; }
    //    public string address { get; set; }
    //    public string lld { get; set; }
    //    public string lsd { get; set; }
    //    public string lsd_ra { get; set; }
    //    public List<List<double>> lsd_border { get; set; }
    //    public List<List<double>> section_border { get; set; }
    //    public string uwi { get; set; }
    //    public string nts { get; set; }
    //    public List<object> nts_border { get; set; }
    //    public string utm { get; set; }
    //    public string utm_v { get; set; }
    //}

    //public class RootObject
    //{
    //    public string query { get; set; }
    //    public Response response { get; set; }
    //}

    public class Meta
    {
        public Nullable<long> start { get; set; }
        public Nullable<long> end { get; set; }
        public Nullable<int> queries { get; set; }
        public Nullable<int> succeeded { get; set; }
        public Nullable<int> failed { get; set; }
        public List<object> err { get; set; }
        public Nullable<int> elapsed_ms { get; set; }
        public Nullable<int> qps { get; set; }
    }

    public class Meta2
    {
        public Nullable<long> start { get; set; }
        public Nullable<long> end { get; set; }
        public bool success { get; set; }
        public List<object> err { get; set; }
        public Nullable<int> elapsed_ms { get; set; }
    }

    public class Gps
    {
        public Nullable<double> lat { get; set; }
        public Nullable<double> lng { get; set; }
        public string hms { get; set; }
    }

    public class Dls
    {
        public List<object> err { get; set; }
        public string lld { get; set; }
        public string lsd { get; set; }
        public object lsd_ra { get; set; }
        public object lsd_proximity { get; set; }
        public object uwi { get; set; }
        public object nts { get; set; }
        public object nts_proximity { get; set; }
        public Nullable<int> gid { get; set; }
        public Nullable<double> lat { get; set; }
        public Nullable<double> lng { get; set; }
        public string latlngms { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string address { get; set; }
    }

    public class Coordinates
    {
        public Nullable<int> easting { get; set; }
        public Nullable<int> northing { get; set; }
        public Nullable<int> zone_number { get; set; }
        public string zone_letter { get; set; }
    }

    public class Utm
    {
        public string brief { get; set; }
        public string verbose { get; set; }
        public Coordinates coordinates { get; set; }
    }

    public class Meta3
    {
        public List<Nullable<int>> sac { get; set; }
    }

    public class Address
    {
        public List<object> err { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public Nullable<int> proximity { get; set; }
        public string full { get; set; }
        public Meta3 meta { get; set; }
        public List<object> polylines { get; set; }
    }

    public class Result
    {
        public Meta2 meta { get; set; }
        public Gps gps { get; set; }
        public Dls dls { get; set; }
        public Utm utm { get; set; }
        public Address address { get; set; }
    }

    public class Query
    {
        public string query { get; set; }
        public Result result { get; set; }
    }

    public class Data
    {
        public Meta meta { get; set; }
        public List<Query> queries { get; set; }
        public Nullable<int> remaining_lookups { get; set; }
    }

    public class GridAtlasRootObject
    {
        public bool success { get; set; }
        public Data data { get; set; }
        //public string message { get; set; }
    }

}
