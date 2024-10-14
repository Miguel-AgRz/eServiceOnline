using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eServiceOnline.BusinessProcess.Interface;
using eServiceOnline.Gateway;
using Sanjel.Common.BusinessEntities.Mdd;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Crew;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Schedule.ProductSchedule;
using Sesi.SanjelData.Entities.BusinessEntities.WellSite;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Equipment;
using Sesi.SanjelData.Entities.Common.BusinessEntities.Products;
using Rig = Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite.Rig;

namespace eServiceOnline.BusinessProcess
{
    public class BinProcess
    {
        public static string LoadBlendToBin(Bin bin, int podIndex, string blendName, string blendDescription, double quantity, ProductHaulLoad productHaulLoad = null, ShippingLoadSheet shippingLoadSheet = null,
            string userName = null)
        {
            string result = "Succeed";

            bool isBinLoadable = false;
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(bin.Id, podIndex);
            if (binInformation == null) throw new Exception("Bin assignment doesn't exist");

            if (binInformation.BlendChemical == null || binInformation.BlendChemical.Id == 0)
            {
                binInformation.BlendChemical = EServiceReferenceData.Data.BlendChemicalCollection.FirstOrDefault(p => p.Name == blendName);
                //Only the first load to a bin keep the LoadId
                //Nov 2, 2023 AW P45_Q4_105: LostProductHaulLoadId fix
                if (binInformation.BlendChemical != null) binInformation.BlendChemical.Description = blendDescription;
                binInformation.LastProductHaulLoadId = productHaulLoad?.Id ?? 0;
                isBinLoadable = true;
            }
            else
            {
                //Nov 2, 2023 AW P45_Q4_105: LostProductHaulLoadId fix
                //If later loading same blend, keep the last LoadId
                if (binInformation.BlendChemical.Description != null)
                {
                    if (binInformation.BlendChemical.Description.Trim().ToLower() == blendDescription.Trim().ToLower())
                    {
                        binInformation.LastProductHaulLoadId = productHaulLoad?.Id ?? 0;
                        isBinLoadable = true;
                    }
                    else
                    {
                        //   throw new Exception("If the blend in bin is different from the blend is going to load, bin must have been emptied first, then load the new blend.");
                        //Nov 20, 2023 AW release_6_11_1_0: exception broke integrations for product haul on location from DRB or eService. We will skip bin load for this case for now.
                        //This will avoid wiping off current job information by mistake. However, the wrong bin content not in RigBoard, will need fixed manually. 
                        isBinLoadable = false;
                        result = "There is " + binInformation.Quantity + "t " +
                                 binInformation.BlendChemical.Description + " in Bin " + binInformation.Name +
                                 ", Please empty the bin before set the new blend On Location";
                    }
                }
            }


            if (isBinLoadable)
            {
                binInformation.Quantity += quantity / 1000;
                //Dec 06 2023 Tongtao P45_Q4_212: Add username after update binInformation
                binInformation.ModifiedUserName = userName;

                binInformation.Description = "Load Blend to Bin";

                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);

                BinLoadHistory binLoadHistory = new BinLoadHistory()
                {
                    BinInformation = binInformation, BlendDescription = blendDescription, BlendName = blendName,
                    InQuantity = quantity / 1000, Remains = binInformation.Quantity, Description = "Load Blend",
                    ShippingLoadSheet = shippingLoadSheet, TimeStamp = DateTime.Now
                };
                eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);
            }

            return result;
        }

        public static BinInformation EmptyBin(int binInformationId, string emptyBinReason, string userName = null)
        {
            //Todo: Update Bin Information, need to update bin load history as well.
            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(binInformationId);
            if (binInformation != null)
            {
                BinLoadHistory binLoadHistory = new BinLoadHistory() { BinInformation = binInformation, BlendDescription = binInformation.BlendChemical?.Description, BlendName = binInformation.BlendChemical?.Name, OutQuantity = binInformation.Quantity, Remains = 0, Description = emptyBinReason, Username = userName, TimeStamp = DateTime.Now };
                eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);

                EmptyBin(binInformation, emptyBinReason);
                binInformation.ModifiedUserName = userName;
                eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);
            }

            return binInformation;
        }

        private static void EmptyBin(BinInformation binInformation, string description = null)
        {
            string userName;
            binInformation.BlendChemical = null;
            binInformation.Quantity = 0;
            binInformation.BlendTestingStatus = BlendTestingStatus.None;
            binInformation.LastProductHaulLoadId = 0;
            binInformation.Description = description;
        }

        public static BinInformation TransferBlendEmptyBin(BinInformation binInformation, double quantity,
            ShippingLoadSheet shippingLoadSheet, string userName)
        {

            BinLoadHistory binLoadHistory = new BinLoadHistory()
            {
                ShippingLoadSheet = shippingLoadSheet, TimeStamp = shippingLoadSheet.OnLocationTime,
                BinInformation = binInformation, BlendDescription = binInformation.BlendChemical?.Description,
                BlendName = binInformation.BlendChemical?.Name, OutQuantity = quantity, Remains = 0,
                Description = "Transfer Blend", Username = userName
            };
            eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);

            EmptyBin(binInformation, "Empty Bin after transfer blend");
            binInformation.ModifiedUserName = userName;

            eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);

            return binInformation;
        }

        // Nov 14, 2023 zhangyuan P63_Q4_174:Add Update Bininformation.
        public static int TransferBlendToBin(BinInformation originBinInformation, int pageType, int targetBinInformationId, double quantity,
       string userName, DateTime operationTime)
        {
            int rtn = 0;
            //before update check
            BinInformation targetBinInformation = eServiceOnlineGateway.Instance.GetBinInformationById(targetBinInformationId);
            if (targetBinInformation == null) throw new Exception("Bin Storage doesn't exist.");

            originBinInformation.Quantity -= quantity;

            if (originBinInformation.Quantity < -0.001) throw new Exception("Bin Storage Cannot be less than 0.");

            if (targetBinInformation.Quantity < 0.001)
            {
                targetBinInformation.BlendChemical = originBinInformation.BlendChemical;
            }

            if (originBinInformation.BlendChemical.Description != targetBinInformation.BlendChemical.Description)
            {
                throw new Exception("Bin Storage is different.");
            }

            var shippingLoadSheet = CreateTransferBlendShippingLoadSheet(userName, pageType, originBinInformation, targetBinInformation, quantity, operationTime);

            targetBinInformation.Quantity += quantity;
            targetBinInformation.LastProductHaulLoadId = originBinInformation.LastProductHaulLoadId;
            if (shippingLoadSheet.Id > 0)
            {
                if (originBinInformation.Quantity * 1000 < 1)
                {
                    TransferBlendEmptyBin(originBinInformation, quantity, shippingLoadSheet, userName);
                    //update target BinInformation
                    targetBinInformation.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateBinInformation(targetBinInformation);
                    BinLoadHistory transferToBinHistory = new BinLoadHistory()
                    {
                        ShippingLoadSheet = shippingLoadSheet, TimeStamp = shippingLoadSheet.OnLocationTime,
                        BinInformation = targetBinInformation,
                        BlendDescription = targetBinInformation.BlendChemical?.Description,
                        BlendName = targetBinInformation.BlendChemical?.Name, InQuantity = quantity,
                        Remains = targetBinInformation.Quantity, Description = "Transfer Blend",
                        Username = userName
                    };
                    eServiceOnlineGateway.Instance.CreateBinLoadHistory(transferToBinHistory);
                }
                else
                {
                    originBinInformation.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateBinInformation(originBinInformation);
                    BinLoadHistory transferFormBinHistory = new BinLoadHistory()
                    {
                        ShippingLoadSheet = shippingLoadSheet, TimeStamp = shippingLoadSheet.OnLocationTime,
                        BinInformation = originBinInformation,
                        BlendDescription = originBinInformation.BlendChemical?.Description,
                        BlendName = originBinInformation.BlendChemical?.Name, OutQuantity = quantity,
                        Remains = originBinInformation.Quantity, Description = "Transfer Blend",
                        Username = userName
                    };
                    eServiceOnlineGateway.Instance.CreateBinLoadHistory(transferFormBinHistory);

                    targetBinInformation.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateBinInformation(targetBinInformation);
                    BinLoadHistory transferToBinHistory = new BinLoadHistory()
                    {
                        ShippingLoadSheet = shippingLoadSheet, TimeStamp = shippingLoadSheet.OnLocationTime,
                        BinInformation = targetBinInformation,
                        BlendDescription = targetBinInformation.BlendChemical?.Description,
                        BlendName = targetBinInformation.BlendChemical?.Name, InQuantity = quantity,
                        Remains = targetBinInformation.Quantity, Description = "Transfer Blend",
                        Username = userName
                    };
                    eServiceOnlineGateway.Instance.CreateBinLoadHistory(transferToBinHistory);
                }

                rtn = 1;
            }

            return rtn;
        }

        // Nov 17, 2023 zhangyuan P63_Q4_174:Add Update shippingLoadSheet,blendUnLoadSheet.
        private static ShippingLoadSheet CreateTransferBlendShippingLoadSheet(string loggedUser, int pageType,
            BinInformation binInformation, BinInformation targetBinInformation,
            double quantity, DateTime operationTime)
        {
            var shippingLoadSheet = new ShippingLoadSheet();
            shippingLoadSheet.SourceStorage = binInformation;
            shippingLoadSheet.BlendDescription = binInformation.BlendChemical?.Description;
	        shippingLoadSheet.Rig = targetBinInformation.Rig;
	        shippingLoadSheet.BulkPlant = binInformation.Rig;

            shippingLoadSheet.OnLocationTime = operationTime;
            shippingLoadSheet.Destination = targetBinInformation.Rig.Name;
            shippingLoadSheet.ExpectedOnLocationTime = operationTime;
            shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
            shippingLoadSheet.LoadAmount = quantity * 1000;
            shippingLoadSheet.ModifiedUserName = loggedUser;
            List<BlendUnloadSheet> blendUnLoadSheets = new List<BlendUnloadSheet>();
            var blendUnLoadSheet = new BlendUnloadSheet();
            blendUnLoadSheet.UnloadAmount = shippingLoadSheet.LoadAmount;
            blendUnLoadSheet.ModifiedUserName = loggedUser;
            blendUnLoadSheet.ShippingLoadSheet = shippingLoadSheet;
            blendUnLoadSheet.DestinationStorage = targetBinInformation;
            var shippingLoadSheetDescription = ProductHaulProcess.BuildTransferBlendComment(shippingLoadSheet, blendUnLoadSheet);
            blendUnLoadSheet.Description = shippingLoadSheetDescription;
            blendUnLoadSheets.Add(blendUnLoadSheet);
            shippingLoadSheet.Name = binInformation.BlendChemical?.Name;
            shippingLoadSheet.BlendUnloadSheets = blendUnLoadSheets;
            shippingLoadSheet.Description = shippingLoadSheetDescription;

            eServiceOnlineGateway.Instance.CreateShippingLoadSheet(shippingLoadSheet);
            return shippingLoadSheet;
        }

        //Dec 06 2023 Tongtao P45_Q4_212: Add shippingLoadSheet for Load Blend To Bin
        //Dec 08 2023 Tongtao 212_PR_UpdateRigNotesBinInformation: Change ShippingStatus
        public static void CreateShippingLoadSheetByLoadBlendToBin(string loggedUser, ProductHaulLoad productHaulLoad)
        {
            ShippingLoadSheet shippingLoadSheet = new ShippingLoadSheet();

            DateTime expectedOnLocationTime = DateTime.Now;

            var binInformation = eServiceOnlineGateway.Instance.GetBinInformationByBinIdAndPodIndex(productHaulLoad.Bin.Id, productHaulLoad.PodIndex);

            shippingLoadSheet.ModifiedUserName = loggedUser;
            shippingLoadSheet.ProductHaulLoad = productHaulLoad;
            shippingLoadSheet.ShippingStatus = ShippingStatus.OnLocation;
            shippingLoadSheet.Name = productHaulLoad.BlendChemical.Name;
            shippingLoadSheet.BlendDescription = productHaulLoad.BlendChemical.Description;
            shippingLoadSheet.BlendSectionId = productHaulLoad.BlendSectionId;
            shippingLoadSheet.ClientName = productHaulLoad.Customer.Name;
            shippingLoadSheet.ClientRepresentative = productHaulLoad.Customer.Name;
            shippingLoadSheet.BlendTestStatus = productHaulLoad.BlendTestingStatus;
            shippingLoadSheet.BulkPlant = productHaulLoad.BulkPlant;
            shippingLoadSheet.CallSheetNumber = 0;
            shippingLoadSheet.CallSheetId = 0;
            shippingLoadSheet.ProgramId = productHaulLoad.ProgramId;
            shippingLoadSheet.Rig = productHaulLoad.BulkPlant;
            shippingLoadSheet.Destination = binInformation.Name;
            shippingLoadSheet.LoadAmount = productHaulLoad.BaseBlendWeight;

            List<BlendUnloadSheet> blendUnLoadSheets = new List<BlendUnloadSheet>();
            var blendUnLoadSheet = new BlendUnloadSheet();
            blendUnLoadSheet.UnloadAmount = shippingLoadSheet.LoadAmount;
            blendUnLoadSheet.ModifiedUserName = loggedUser;
            blendUnLoadSheet.ShippingLoadSheet = shippingLoadSheet;
            blendUnLoadSheet.DestinationStorage = binInformation;
            var LoadBlendDescription = ProductHaulProcess.BuildLoadBlendToBinComments(shippingLoadSheet, expectedOnLocationTime);
            blendUnLoadSheet.Description = LoadBlendDescription;
            blendUnLoadSheets.Add(blendUnLoadSheet);
            shippingLoadSheet.BlendUnloadSheets = blendUnLoadSheets;

            shippingLoadSheet.Description = LoadBlendDescription;
            shippingLoadSheet.ExpectedOnLocationTime = expectedOnLocationTime;
            shippingLoadSheet.OnLocationTime = expectedOnLocationTime;

            eServiceOnlineGateway.Instance.CreateShippingLoadSheet(shippingLoadSheet);
        }



        //Nov 13, 2023 Tongtao P45_Q4_175: Add specific processing methods  for"Load Blend to Bulker"
        //Dec 11, 2023 Adam: Fix all issues and integrated with crew status update
        //Dec 13, 2023 Adam: Tweak it to be load on shipping load sheet function
        //Jan 10, 2024 Tongtao:update podload if user modify podload info 
        public static string LoadBlendToBulker(int shippingLoadSheetId, string userName, int sourceStorageId = 0, List<PodLoad> podLoads = null)
        {
            string result = string.Empty;

            ShippingLoadSheet shippingLoadSheet =
                eServiceOnlineGateway.Instance.GetShippingLoadSheetById(shippingLoadSheetId);

            if (shippingLoadSheet == null)
                return "Shipping Load Sheet " + shippingLoadSheetId + "  doesn't exist";
            if (shippingLoadSheet.ShippingStatus != ShippingStatus.Scheduled)
                return "Shipping Load Sheet status is " + shippingLoadSheet.ShippingStatus;

            if (sourceStorageId == 0)
            {
                //If shippingLoadSheet.SourceStorage.Id, that means the blend is loaded from blend train directly.
                if (shippingLoadSheet.SourceStorage.Id == 0)
                {
                    shippingLoadSheet.ShippingStatus = ShippingStatus.Loaded;
                    shippingLoadSheet.ModifiedUserName = userName;
                    eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet, false);

                    ProductHaulLoad productHaulLoad = eServiceOnlineGateway.Instance.GetProductHaulLoadById(shippingLoadSheet.ProductHaulLoad.Id);

                    //Jan 31, 2024 AW: If load bulker from blend train directly, the shipping load sheet must be 1 to 1 to blend request.
                    if (productHaulLoad != null)
                    {
	                    productHaulLoad.ProductHaulLoadLifeStatus = ProductHaulLoadStatus.Loaded;
	                    eServiceOnlineGateway.Instance.UpdateProductHaulLoad(productHaulLoad);
                    }


                    //Feb 23, 2024 tongtao 303_PR_NotSaveRigWhenCallsheetNumberChanged: when sourceStorageId=0, after save podinfo  return
                    result = "Succeed";
                }
            }
            else
            {
	            BinInformation binInformation = eServiceOnlineGateway.Instance.GetBinInformationById(sourceStorageId);

	            if (binInformation == null) return "Cannot find the bin information which the blend is loaded from.";
                //Feb 22, 2024 zhangyuan 295_PR_BulkerCrewstatus: Modify BlendChemicalDescription May be Null
                if ((binInformation.BlendChemical.Description??"") != shippingLoadSheet.BlendDescription)
	            {
		            result = "There is " + binInformation.Quantity + "t " +
		                     binInformation.BlendChemical.Description + " in Bin " + binInformation.Name +
		                     ", Please load the correct blend to bin before loading up the bulker.";
	            }
	            else
	            {

		            //1. Load up shipping load sheet
		            //When the status of ShippingLoadSheet is Scheduled, update its status to Loaded. At the same time, update the Quantity value of BinInformation to Quantity minus the LoadAmount of ShippingLoadSheet.

		            shippingLoadSheet.ShippingStatus = ShippingStatus.Loaded;
		            shippingLoadSheet.ProductHaulLoad.Id = binInformation.LastProductHaulLoadId;
		            shippingLoadSheet.ModifiedUserName = userName;

		            eServiceOnlineGateway.Instance.UpdateShippingLoadSheet(shippingLoadSheet, false);

                    //2. Deduct from Bin
                    binInformation.Quantity = (binInformation.Quantity * 1000 - shippingLoadSheet.LoadAmount) / 1000;

                    //3. Log BinLoadHistory

                    //Dec 08, 2023 Tongtao 175_PR_LoadToBulker: Add BinLoadHistory for LoadBlendToBulker
                    //Dec 08, 2023 Tongtao 175_PR_LoadToBulker: Add ShippingLoadSheet  info  for the BinLoadHistory of LoadBlendToBulker
                    BinLoadHistory binLoadHistory = new BinLoadHistory()
		            {
			            BinInformation = binInformation, ShippingLoadSheet = shippingLoadSheet,
			            BlendDescription = binInformation.BlendChemical?.Description,
			            BlendName = binInformation.BlendChemical?.Name,
			            OutQuantity = shippingLoadSheet.LoadAmount / 1000,
			            Remains = binInformation.Quantity, Description = "Load Blend To Bulker", Username = userName,
			            TimeStamp = DateTime.Now
		            };
		            eServiceOnlineGateway.Instance.CreateBinLoadHistory(binLoadHistory);

		            //4. Empty bin if all blend is loaded out

		            if (Math.Abs(binInformation.Quantity) < 0.001)
		            {
			            EmptyBin(binInformation, "Empty Bin after load blend to bulker");
		            }
                    else
                    {
                        //Feb 22, 2024 zhangyuan 295_PR_BulkerCrewstatus: add binInformation Description
                        binInformation.Description = "Load Blend To Bulker";
                    }

		            binInformation.ModifiedUserName = userName;
		            eServiceOnlineGateway.Instance.UpdateBinInformation(binInformation);

		            result = "Succeed";
	            }
            }

            //update podload if user modify podload info 
            if (podLoads != null)
            {
	            List<PodLoad> originalPodLoads = eServiceOnlineGateway.Instance.GetPodLoadsByProductHaul(shippingLoadSheet.ProductHaul.Id).OrderBy(p => p.PodIndex).ToList();

	            for (int i = 0; i < originalPodLoads.Count; i++)
	            {
		            var originalpodLoadItem = originalPodLoads[i];
		            if (podLoads[i].LoadAmount * 1000 != originalpodLoadItem.LoadAmount)
		            {
			            originalpodLoadItem.LoadAmount = podLoads[i].LoadAmount * 1000;
			            originalpodLoadItem.ShippingLoadSheet = podLoads[i].LoadAmount > 0 ? shippingLoadSheet : null;
			            eServiceOnlineGateway.Instance.UpdatePodLoad(originalpodLoadItem);
		            }
	            }
            }

            return result;
        }

    }
}
