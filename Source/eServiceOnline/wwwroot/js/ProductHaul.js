var scheduleChainValidation, qualityOfTheBinChainValidation, amountAndMixWaterChainValidation;
var dialogSchedule, dialogQualityOfTheBin, dialogAmountAndMixWater;

var dialogHaulAmount, dialogUnLoad, dialogPodLoad,ValidateHaulAmountChain, ValidateUnLoadChain, ValidatePodLoadChain;

/**
 * constructs new initialization class and pass a function to execute 
 * @param {function}} func
 */
var InitSchedule = function (func) {
    if (!func) throw "function not provided";

    this.OnInit = func;
};

InitSchedule.prototype = {

    Initialize: function () {

        this.OnInit.call(this);
     
        $(".confirmDialog_button").hide();
    }
}
 
function InitializeDialogs() {
    dialogSchedule = new ManageDialog("verifyCrew", scheduleChainValidation || undefined);
    dialogQualityOfTheBin = new ManageDialog("verifyBin", qualityOfTheBinChainValidation || undefined);
    dialogAmountAndMixWater = new ManageDialog("is_confirm_jump_createhaul", amountAndMixWaterChainValidation || undefined);

    dialogHaulAmount = new ManageDialog("verifyHaulAmount", ValidateHaulAmountChain || undefined);
    dialogUnLoad = new ManageDialog("verifyUnLoad", ValidateUnLoadChain || undefined);
    dialogPodLoad = new ManageDialog("verifyPodLoad", ValidatePodLoadChain || undefined);
}

function selectType(value, item) {
    $("#" + item).val(value);
}
function getHaulDetails(obj) {
    if ($(obj).length > 0 && obj.value) {
        $.ajax({
            url: siteInstance.baseControllerActionUrls.productHaul.getProductHaulInfoById,
            type: 'POST',
            async: false,
            data: {
                "productHaulId": obj.value
            },
            dateType: "json",
            success: function (data) {
                $("#ProductHaulId").val(data.ProductHaulId);
                $("#existing_Crew").show();
                $("#existing_isThirdParty").show();
                $("#div_existing_thirdParty").val(data.IsThirdParty);
                if (data.IsThirdParty) {
                    $("#existing_thirdParty").prop('checked', true);
                } else {
                    $("#existing_thirdParty").prop('checked', false);
                }

                $("#CrewName").val(data.CrewName);


                if (data.IsGoWithCrew) {
                    $("#existing_expectedOlTime").hide();   
                    $("#existing_haul_go_With_crew").prop('checked', true);
                    //$("#IsGoWithCrew").val(true);
                    $("#IsGoWithCrew").prop("checked", true);
                } else {
                    $("#existing_haul_go_With_crew").prop('checked', false);
                    $("#existing_expectedOlTime").show();
                    $("#existing_olTime").val(data.ShortExpOlTime);
                    $("#ExpectedOnLocationTime").val(data.ExpectedOnLocationTime);
                    //$("#IsGoWithCrew").val(false);
                    $("#IsGoWithCrew").prop("checked", false);
                }

                $("#existing_go_with_crew").show(); 

                validateBin();
            }
        });
    }
}


function hideThirdParty() {
    $("#ThridPartyCrew").hide();
    //$("#IsThirdParty").val(false);
    $("#haulList_wrapper").hide();
    $("#existing_isThirdParty").hide();
    $("#existing_go_with_crew").hide();
    $("#existing_expectedOlTime").hide();
    $("#existing_Crew").hide();

};

function getCrew(item) {
    $("#CrewId_tip").hide();
    if ($("#CrewId").find("option:selected").text() === "None") {
        $("#CrewId_tip").show();
    } else {
        $("#CrewId_tip").hide();
    }
}

function IsRequired(item) {

    if (($("#" + item).val() == "" || $("#" + item).val() == 0 || isNaN($("#" + item).val())) && $("#" + item).attr("required") == "required") {
        $("#Is_" + item).show();

        $("#sj_createProductHual").attr("disabled", true);
        return false;
    } else {
        $("#Is_" + item).hide();
        $("#sj_createProductHual").attr("disabled", false);
    }

    return true;
}


function IsRequiredAmount(item) {
    if (($("#" + item).val().trim() == "" || $("#" + item).val().trim() == 0)) {
        $("#Is_" + item).show();
        return false;
    } else {
        $("#Is_" + item).hide();
        return true;
    }
}

function GetCallSheetNumberByProgramNumber() {
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getCallSheetNumberByProgramId;
    console.log(itemUrl);
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "programId": $("#ProgramNumber").val() },
        success: function (data) {
            $("#CallSheetNumber").html("");
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var content = '<option value="' + data[i] + '">' + data[i] + '</option>';
                    $("#CallSheetNumber").append(content);
                }
//                $("#RigId").hide();
//                $("#RigName").show();
//                LoadRigAndUnLoadByCallSheetNumber();
            }
            else {
                $("#RigId").show();
                $("#RigName").hide();
            }
        }
    });
}
function GetCallSheetNumberByProgramNumberAndJobType() {
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getCallSheetNumberByProgramIdAndJobType;
    console.log(itemUrl);
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "programId": $("#ProgramNumber").val(), "jobTypeId": $("#JobTypeId").val()},
        success: function (data) {
            $("#CallSheetNumber").html("");
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var content = '<option value="' + data[i] + '">' + data[i] + '</option>';
                    $("#CallSheetNumber").append(content);
                }
                //Nov 3, 2023 AW P45_Q4_105: Delete the code TongTao added "When there is only one Callsheet information, it is selected by default."
            }
            else {
                $("#RigId").show();
                $("#RigName").hide();
            }
        }
    });
}
function GetCallSheetNumberByBaseBlend() {
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getCallSheetNumberByBaseBlend;
    console.log(itemUrl);
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "baseBlend": $("#BaseBlend").val() },
        success: function (data) {
            $("#CallSheetNumber").html("");
            if (data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var content = '<option value="' + data[i] + '">' + data[i] + '</option>';
                    $("#CallSheetNumber").append(content);
                }
//                $("#RigId").hide();
//                $("#RigName").show();
//                LoadRigAndUnLoadByCallSheetNumber();
            }
            else {
                $("#RigId").show();
                $("#RigName").hide();
            }
        }
    });
}

//jan 23, 2024 tongtao 274_PR_RigCouldNotChange: set rigid is readonly when callSheet has been selected
//jan 25, 2024 tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig: when call sheet is not seleted,unload and go with crew not show
//Feb 02, 2024 tongtao 264_PR_GoWithCrewError: set go with crew date
function LoadRigAndUnLoadByCallSheetNumber() {
    var callSheetNumber = $("#CallSheetNumber").val();

    if (callSheetNumber == "") {
        $("#rigUnload").html("");
        $("#rigUnload").hide;
        $("#RigId").val("").change();
        $("#RigName").val("");
        $("#ClientName").val("");
        $("#ClientId").val("");
        $('#IsGoWithCrew').prop("checked", false);
        $("#GoWithCrewPart").hide();
        getGoWithCrew();
        $('#RigId').prop('disabled', false);

        return;
    }

    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getShippingLoadSheetByCallSheetNumber;
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "callSheetNumber": $("#CallSheetNumber").val() },
        success: function (data) {
            //Nov 6, 2023 AW P45_Q4_105: Use change event to trigger BlendUnloadSheet population
            $("#rigUnload").html("");
            $("#RigId").val(data.RigId).change();
            $("#RigName").val(data.RigName);
            $("#ClientName").val(data.ClientName);
            $("#ClientId").val(data.ClientId);
            var rigLabelDiv = $("label:contains('Rig')").parent(".det_list");
            $("#rigUnload").html("");
            $("#GoWithCrewPart").show();

        }
    });
}


//Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: Add Change CallSheet Load Rigs 
//jan 23, 2024 tongtao 274_PR_RigCouldNotChange: set rigid is readonly when callSheet has been selected
//jan 23, 2024 tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig: when reschedule producthaul,get blend unload amout by shippingLoadSheetId
//jan 25, 2024 tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig:Add a parameter to determine whether this method is called during initialization or modification.
//Feb 02, 2024 tongtao 264_PR_GoWithCrewError: clean hidden value
//Feb 05, 2023 tongtao 279_PR_CouldNotChangeCallSheet: check ProductHaul is RigJobBlend,check ProductHaul is RigJobBlend, if it is ,could not change callsheet number
function LoadRigByCallSheetNumber(index, status) {
    
    var callSheetNumber = $("#PodLoadAndBendUnLoadModels_" + index + "__CallSheetNumber").val();
    var isRigJobBlend = $("#hid_" + callSheetNumber + "__IsRigJobBlend").val();


    if (callSheetNumber == "") {
        $("#PodLoadAndBendUnLoadModels_" + index + "__RigId").prop('disabled', false);
        $("#rigUnload" + index).html("");
        $("#IsGoWithCrew").prop("checked", false);
        $("#PodLoadAndBendUnLoadModels_" + index + "__IsGoWithCrew").prop("checked", false);
        $("#GoWithCrewPart" + index).hide();
        if (status == 1) {

           $("#PodLoadAndBendUnLoadModels_" + index + "__RigId").nextAll('input[type="hidden"]').slice(0, 2).val("");

           $("#PodLoadAndBendUnLoadModels_" + index + "__RigId").val("").change();

        } else {
            $("#PodLoadAndBendUnLoadModels_" + index + "__RigId").change();
        }
        $("#ExpOlTime").show();
        $("#EstimateDuration").show();
        return;
    }


    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getShippingLoadSheetByCallSheetNumber;

    if (callSheetNumber) {
        $.ajax({
            url: itemUrl,
            method: "post",
            data: { "callSheetNumber": callSheetNumber },
            success: function (data) {
                $("#rigUnload" + index).html("");
                $("#PodLoadAndBendUnLoadModels_" + index + "__CallSheetId").val(data.CallSheetId);
                $("#PodLoadAndBendUnLoadModels_" + index + "__RigId").val(data.RigId).change();
                $("#GoWithCrewPart" + index).show();


                if (isRigJobBlend == "True") {
                    $("#PodLoadAndBendUnLoadModels_" + index + "__CallSheetNumber").prop('disabled', true);
                }

            }
        });
    }
}

function checkIsRigBulkPlantBin() {

    if ($("#ProgramNumber").length > 0) {
        return false;
    }

    return true;
}
  //Nov 28, 2023 zhangyuan 201_PR_AddingDifferrentBlend: Modify change Rig set different binInformation Display prompt information
//jan 23, 2024 tongtao 274_PR_RigCouldNotChange: set rigid is readonly when callSheet has been selected
function LoadUnloadByRig() {
    var rigId = $("#RigId").val();

    if (rigId == null || rigId == "") {
        return;
    }

    var callSheetNumber = $("#CallSheetNumber").val();

    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getShippingLoadSheetByRig;
    var blendDescription = $("#BlendChemicalDescription").val();
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "rigId": $("#RigId").val() },
        success: function (data) {
//            $("#RigId").val(data.RigId);
            $("#RigName").val(data.RigName);
            $("#rigUnload").html("");

            if (callSheetNumber != "") {
                $('#RigId').prop('disabled', true);
            }

            $('#hidRigId').val($('#RigId').val());

            if (data.BlendUnloadSheetModels.length > 0) {
                for (var i = 0; i < data.BlendUnloadSheetModels.length; i++) {
                    var content = '<div class="det_list"><label class="det_label">Bin ' + data.BlendUnloadSheetModels[i].DestinationStorage.Name + '  Offload Amount</label><div class="det_text">';
                    content += '<input class="sj_text blend_unload_amount"  id="BlendUnloadSheetModels_' + i + '__UnloadAmount" name="BlendUnloadSheetModels[' + i + '].UnloadAmount" type="text" value="0">';
                     //April 24, 2024 tongtaop 183-offload-all-selection-on-product-haul: add offloadall checkbox
                    content += '<input type="checkbox" style="margin-left:4px;" class="unloadAll_chk" id="BlendUnloadAll_' + i + '"  onclick="offloadAll(this,0)"  name="isHaulAll"><label>&nbsp;&nbsp;Offload All</label>';
                    content += '<input  id="BlendUnloadSheetModels_' + i + '__DestinationStorage_Id" name="BlendUnloadSheetModels[' + i + '].DestinationStorage.Id" type="hidden" value="' + data.BlendUnloadSheetModels[i].DestinationStorage.Id + '"> ';
                    //       <div id="Bin_Required" style="display: none; color: red;">Bin is required</div>
                    if (data.BlendUnloadSheetModels[i].DestinationStorage.Quantity > 0.001 && data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Description != blendDescription) {
                        var chemicalName = data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Name;
                        if (chemicalName != null) {
                            chemicalName = data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Name.replace(" + Additives", "");
                        }
                        //jan 11, 2023 zhangyuan 259_PR_HaulBlendBugs: Modify  fix Quantity accuracy
                        var contentDifferent = Math.round(data.BlendUnloadSheetModels[i].DestinationStorage.Quantity * 1000) / 1000 + 't ' + chemicalName;
                        contentDifferent += ' in Bin';
                        content += '<div id="Bin_Different_' + i + '" style="color: red;">' + contentDifferent + '</div>';
                    }
                    content += ' </div></div>';
                    $("#rigUnload").append(content);
                }
            }
        }
    });
}

//April 24, 2024 tongtaop 183-offload-all-selection-on-product-haul: add offloadall function for checkbox
function offloadAll(checkbox,type) {
    var checkboxes = document.querySelectorAll('.unloadAll_chk');
    var remainsAmount = 0;

    var loadSheetId = $("#ProductHaulLoadId");

    if (type == 0 ||(type == 1 && loadSheetId.length!=0)) {
        remainsAmount = $("#LoadAmount").val();
    } else if (type == 1 && loadSheetId.length == 0) {
        remainsAmount = $("#Amount").val();
    }

    if (remainsAmount == "") { remainsAmount = 0;}

    var allUnloadAmountInputs = document.querySelectorAll('.blend_unload_amount');

    var currentlyChecked = checkbox.checked;

    checkboxes.forEach(function (item) {
        item.checked = false;
    });

    allUnloadAmountInputs.forEach(function (input) {
        input.removeAttribute('readonly');
        input.value = '0';
        input.style.backgroundColor = ""; 
    });

    if (currentlyChecked) {
        checkbox.checked = true; 

        allUnloadAmountInputs.forEach(function (input) {
            if (input.id === 'BlendUnloadSheetModels_' + checkbox.id.split('_')[1] + '__UnloadAmount') {
                input.value = remainsAmount; 
            }
            input.style.backgroundColor = "#F2F2F2";
            input.setAttribute('readonly', 'true');

        });
    }
}

//April 25, 2024 tongtaop 183-offload-all-selection-on-product-haul: add function changeAmount 
function changeAmount(type) {
    var productHaulLoadId = $("#ProductHaulLoadId");

    var amount = 0;

    if (type == 0 && productHaulLoadId.length != 0) {
        amount = $("#LoadAmount").val();
    } else {
        amount = $("#Amount").val();
    }

    var remainsAmount = $("#RemainsAmount");

    if (remainsAmount.length != 0) {
        $("#RemainsAmount").val(amount);
    }

    var programNumber = $("#ProgramNumber");

    var checkboxes = document.querySelectorAll('.unloadAll_chk');
    var allUnloadAmountInputs = document.querySelectorAll('.blend_unload_amount');

    if (productHaulLoadId.length == 0 && type == 1 && programNumber.length == 1) {

        checkboxes.forEach(function (item) {
            item.checked = false;
        });

        allUnloadAmountInputs.forEach(function (input) {
            input.removeAttribute('readonly');
            input.value = '0';
            input.style.backgroundColor = "";
        });

        $(".blend_unload_amount").each(function () {
            $(this).val("0");
            if ($(this).attr("BinInformationId") == $("#OrigBinInformationId").val()) {
                $(this).val(amount);
            }
        });
    } else {

        if (amount != "") {

            if (checkboxes.length > 0) {
                for (let i = 0; i < checkboxes.length; i++) {
                    var checkbox = checkboxes[i];
                    if (checkbox.checked) {
                        var index = checkbox.id.split('_')[1];
                        var correspondingInput = document.querySelector('#BlendUnloadSheetModels_' + index + '__UnloadAmount');
                        if (correspondingInput) {
                            correspondingInput.value = amount;
                        }
                        break;
                    }
                }
            }
        } else {
            checkboxes.forEach(function (item) {
                item.checked = false;
            });

            allUnloadAmountInputs.forEach(function (input) {
                input.removeAttribute('readonly');
                input.value = '0';
                input.style.backgroundColor = "";
            });
        }
    }
}

//Dec 7, 2023 zhangyuan 224_PR_ShowCallSheetList: Add Change CallSheet Load Rigs And pods
//jan 25, 2024 tongtao 273_PR_UnloadChangeHasErrorAfterRigChangeRig:when rig not seleted,rig unload not show
function LoadPodLoadAndBendUnLoadByRig(index) {
    var rigId = $("#PodLoadAndBendUnLoadModels_" + index + "__RigId");
    var callSheetNumber = $("#PodLoadAndBendUnLoadModels_" + index + "__CallSheetNumber").val();
    var hidShippingLoadSheetId = $("#PodLoadAndBendUnLoadModels_" + index + "__ShippingLoadSheetId");

    if (rigId.val() == null || rigId.val() == "" || rigId.val() == "0") {

        $("#rigUnload" + index).html("");
        return;
    }

    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getShippingLoadSheetByRig;
    var blendDescription = $("#PodLoadAndBendUnLoadModels_" + index + "__Blend").val();
    var hidShippingLoadSheetId = $("#PodLoadAndBendUnLoadModels_" + index + "__ShippingLoadSheetId");

    if ($("#PodLoadAndBendUnLoadModels_" + index + "__IsGoWithCrew").prop("checked")) {
        return;
    } 


    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "rigId": rigId.val(), "shippingLoadSheetId": hidShippingLoadSheetId.val() },
        success: function (data) {
            if (callSheetNumber != "") {
                $(rigId).prop('disabled', true);
            }
            $("#PodLoadAndBendUnLoadModels_" + index + "__RigName").val(data.RigName);
             //Feb 23, 2024 tongtao 303_PR_NotSaveRigWhenCallsheetNumberChanged: set rigid in the hidden for save
            $('#hid_' + index + '_RigId').val(data.RigId);
            $("#rigUnload" + index).html("");
            if (data.BlendUnloadSheetModels.length > 0) {
                for (var i = 0; i < data.BlendUnloadSheetModels.length; i++) {
                    //@Model.PodLoadAndBendUnLoadModels[j].BlendUnloadSheetModels[i].DestinationStorage.Bin.Name  
                    var content = '<div class="det_list"><label class="det_label">Bin ' + data.BlendUnloadSheetModels[i].DestinationStorage.Name + '  Offload Amount</label><div class="det_text">';
                    //@Model.PodLoadAndBendUnLoadModels[j].BlendUnloadSheetModels[i].UnloadAmount
                    content += '<input class="sj_text blend_unload_amount"  id="PodLoadAndBendUnLoadModels_' + index + '__BlendUnloadSheetModels_' + i + '__UnloadAmount" name="PodLoadAndBendUnLoadModels[' + index + '].BlendUnloadSheetModels[' + i + '].UnloadAmount" type="text" value=' + data.BlendUnloadSheetModels[i].UnloadAmount + '>';
                    content += '<input  id="PodLoadAndBendUnLoadModels_' + index + '__BlendUnloadSheetModels_' + i + '__DestinationStorage_Id" name="PodLoadAndBendUnLoadModels[' + index + '].BlendUnloadSheetModels[' + i + '].DestinationStorage.Id" type="hidden" value="' + data.BlendUnloadSheetModels[i].DestinationStorage.Id + '"> ';
/*                    content += '<input  id="PodLoadAndBendUnLoadModels_' + index + '__BlendUnloadSheetModels_' + i + '__Id" name="PodLoadAndBendUnLoadModels[' + index + '].BlendUnloadSheetModels[' + i + '].Id" type="hidden" value="' + data.BlendUnloadSheetModels[i].Id + '"> ';*/
                    //       <div id="Bin_Required" style="display: none; color: red;">Bin is required</div>
                    if (data.BlendUnloadSheetModels[i].DestinationStorage.Quantity > 0.001 && data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Description != blendDescription) {
                        var chemicalName = data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Name;
                        if (chemicalName != null) {
                            chemicalName = data.BlendUnloadSheetModels[i].DestinationStorage.BlendChemical.Name.replace(" + Additives", "");
                        }
                        //jan 11, 2023 zhangyuan 259_PR_HaulBlendBugs: Modify  fix Quantity accuracy
                        var contentDifferent = Math.round(data.BlendUnloadSheetModels[i].DestinationStorage.Quantity * 1000) / 1000 + 't ' + chemicalName;
                        contentDifferent += ' in Bin';
                        content += '<div id="Bin_Different_' + i + '" style="color: red;">' + contentDifferent + '</div>';
                    }
                    content += ' </div></div>';
                    $("#rigUnload" + index).append(content);
                    $("#rigUnload" + index).show();
                }
            }
        }
    });
}

function ValidateHaulAmount() {
    if (Number($("#LoadAmount").val()) > Number($("#RemainsAmount").val())) {
        $("#haulAmountMessage").html('Haul Amount Should Less Than Remains Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogHaulAmount.openDialog();
        return false;
    }
    else {
        return true;
    }

}
function ValidateUnLoad() {
    var unLoads = 0;
    $(".blend_unload_amount").each(function () {
        unLoads += Number($(this).val());
    });
    if (Number($("#LoadAmount").val()) != unLoads) {
        $("#unLoadMessage").html('All Bin LoadAmount Add Up Should Equal Haul Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogUnLoad.openDialog();
        return false;
    }
    else {
        return true;
    }
}

function ValidatePodLoad() {
    var podLoads = 0;
    $(".pod_load_amount").each(function () {
        if ($(this).prop("readonly") != true) {
            podLoads += Number($(this).val());
        }
    });
    if (Number($("#LoadAmount").val()) != podLoads) {
        $("#podLoadMessage").html('All Pod LoadAmount Add Up Should Equal Haul Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogPodLoad.openDialog();
        return false;
    }
    else {
        return true;
    }
}
function ValidateUnLoad1() {
    var unLoads = 0;
    $(".blend_unload_amount").each(function () {
        unLoads += Number($(this).val());
    });
    if (Number($("#Amount").val()) != unLoads) {
        $("#unLoadMessage").html('All Bin LoadAmount Add Up Should Equal Haul Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogUnLoad.openDialog();
        return false;
    }
    else {
        return true;
    }
}
function ValidatePodLoad1() {
    var podLoads = 0;
    $(".pod_load_amount").each(function () {
        if ($(this).prop("readonly") != true) {
            podLoads += Number($(this).val());
        }
    });
    if (Number($("#Amount").val()) != podLoads) {
        $("#podLoadMessage").html('All Pod LoadAmount Add Up Should Equal Haul Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogPodLoad.openDialog();
        return false;
    }
    else {
        return true;
    }
}
function GetUnLoadData() {
    var unLoads = "";
    $(".blend_unload_amount").each(function () {
        unLoads += $(this).val() + ",";
    });
    var model = {
        unLoads: unLoads
    };
    console.log(model);
    return model;
}

function GetPodLoadData() {
    var podLoads = "";
    $(".pod_load_amount").each(function () {
        if ($(this).prop("readonly") != true) {
            podLoads += $(this).val() + ",";
        }
    });
    var model = {
        podLoads: podLoads
    };
    console.log(model);
    return model;
}

function GetHualAmountData() {
    var model = {
        LoadAmount: $("#LoadAmount").val()
    };
    console.log(model);
    return model;
}

function CheckExistingHaul() {
    return;
    if ($("#IsExistingHaul").prop("checked") == true) {
        $("#IsGoWithCrew").prop("checked", false);
        $("#rigUnload").show();
        $("#GoWithCrewPart").hide();
    }
    else {
        $("#GoWithCrewPart").show();
    }
}

function GetPodLoadByProduct() {
    $(".pod_load_amount").each(function () {
        $(this).val("0");
        $(this).removeAttr("readOnly");
    });
    if ($("#IsExistingHaul").prop("checked") == false || $("#ProductHaulId").val() == "None") {
        return;
    }
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getPodLoadByProductHual;
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "productHualId": $("#ProductHaulId").val() },
        success: function (data) {
            if (data.length != 0) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].LoadAmount > 0) {
                        $(".pod_load_amount").eq(data[i].PodIndex).val(data[i].LoadAmount);
                        $(".pod_load_amount").eq(data[i].PodIndex).attr("readOnly", true);
                    }
                }
            }
        }
    });
}

function dataAmountAndMixWater() {
    var obj = $("#ProgramNumber");
    var model = {
        CallSheetNumber: $("#CallSheetNumber").val(),
        BaseBlendSectionId: $("#BaseBlendSectionId").val(),
        Amount: $("#Amount").val(),
        MixWater: $("#MixWater").val(),
        IsTotalBlendTonnage: $("#IsTotalBlendTonnage").val(),
        ProductHaulLoadId: $("#ProductHaulLoadId").val(),
        ProgramNumber:obj.val()
    }
    return model;
}

function verifyAmountAndMixWater() {

    var model = dataAmountAndMixWater();
    var itemurl = siteInstance.baseControllerActionUrls.productHaul.verifyAmountAndMixWater;
    var isPass = false;
    $.ajax({
        url: itemurl,
        async: false,
        method: "Post",
        data: model,
        success: function (data) {
            console.log(data);
            console.log(data === "Required");
            if (data === "Required") {
                $("#MixWater").attr("required", "required");
                IsRequired('MixWater');
            } else {
                if (!data) {
                    $("#message").html(message);
                    $("is_confirm_jump_createhaul").show();
                    dialogAmountAndMixWater.openDialog();

                } else {
                    isPass = true;
                }
            }
            return isPass;
        }
    });
    return isPass;
}

function validateJobDuration() {
    if (getEstJobDuration() == false) {
        return false;
    }
    return true;
}

function validateCrew() {
    var isChecked = $("#IsExistingHaul").prop("checked");

    if (!isChecked) {
        if (!$("#IsThirdParty").prop("checked")) {
            if ($("#CrewId").find("option:selected").text() === "None") {
                //alert($("#CrewId").val());
                $("#CrewId_tip").show();
                return false;
            }
        } else {
            if ($("#ThirdPartyBulkerCrewId").find("option:selected").text() === "None") {
                $("#ThirdPartyBulkerCrewId_tip").show();
                return false;
            }
        }
    }
    else {

        if ($("#ProductHaulId").find("option:selected").text() === "None") {
            $("#selectProductHaul_tip").show();
            return false;
        } else {
            $("#selectProductHaul_tip").hide();
        }
    }

    return true;
}

function validateReScheduleCrew() {
    var isChecked = $("#IsExistingHaul").prop("checked");
    if (!isChecked) {
        if (!$("#originalHaul").prop("checked")) {
            if (!$("#IsThirdParty").prop("checked")) {
                if ($("#CrewId").find("option:selected").text() === "None") {
                    $("#CrewId_tip").show();
                    return false;
                }
            }
            else {
                if ($("#ThirdPartyBulkerCrewId").find("option:selected").text() === "None") {
                    $("#ThirdPartyBulkerCrewId_tip").show();
                    return false;
                }
            }
        }

    }

    return true;
}

function validateExpectedLocationTime() {
    $("#ExpectedOlTime").val($("#exp_on_location_time").val());

    return true;
}

function validateRequiredAmount() {
    if (!IsRequired('Amount')) {
        return false;
    }

    return true;
}

function validateBin() {
    var isChecked = $("#IsGoWithCrew").prop("checked");
    if (isChecked === false && $("#BinInformationId").find("option:selected").text() === "None") {
        $("#Bin_Required").show();
        return false;
    } else {
        $("#Bin_Required").hide();
    }
    return true;
}

function validateBulkPlant() {
    if ($("#BulkPlantId").find("option:selected").text() === "None") {
        $("#BulkPlant_Required").show();
        return false;
    } else {
        $("#BulkPlant_Required").hide();
    }
    return true;
}
function validateRigBulkPlant() {
    if ($("#RigId").find("option:selected").text() === "None") {
        $("#RigBulkPlant_Required").show();
        return false;
    } else {
        $("#RigBulkPlant_Required").hide();
    }
    return true;
}
function validateExistingHaul() {
    if ($("#ProductHaulId").find("option:selected").text() === "None") {
        $("#selectProductHaul_tip").show();
        return false;
    } else {
        $("#selectProductHaul_tip").hide();
    }
    return true;
}

function dataScheduleValidation() {
    var data = null;

    if (!$("#IsThirdParty").prop("checked")) {
        if ($("#IsGoWithCrew").prop("checked")) {
            data = {
                "crewId": $("#CrewId").val(),
                "startTime": $("#EstimatedLoadTime").val(),
                "rigJobId": $("#RigJobId").val()
            };
        }
        else {
            data = {
                "crewId": $("#CrewId").val(),
                "startTime": $("#EstimatedLoadTime").val(),
                "duration": $("#EstimatedTravelTime").val()
            };
        }
    }
    else {
        if ($("#IsGoWithCrew").prop("checked")) {
            data = {
                "thirdPartyBulkerCrewId": $("#ThirdPartyBulkerCrewId").val(),
                "startTime": $("#EstimatedLoadTime").val(),
                "rigJobId": $("#RigJobId").val()
            };
        } else {
            data = {
                "thirdPartyBulkerCrewId": $("#ThirdPartyBulkerCrewId").val(),
                "startTime": $("#EstimatedLoadTime").val(),
                "duration": $("#EstimatedTravelTime").val()
            };
        }
    }
    return data;
}
function getEstJobDuration() {
    var reg = /^(-?\d+)(\.\d+)?$/;
    $("#validValue").html("");
    var duration;
    if ($("#EstimatedTravelTime").val() === "") {
        duration = 4;
        $("#EstimatedTravelTime").val(duration);
    } else {
        duration = $("#EstimatedTravelTime").val();
    }
    if (duration < 1 || duration > 72) {
        $("#validValue").html("*between 1 and 72");
        isPass = false;
        return isPass;
    }
    if (!reg.test(duration)) {
        $("#validValue").html("*Input number type");
        isPass = false;
        return isPass;
    }
}
function RescheduleProductHaulLoadVerifyAmountAndMixWater() {
    if (getEstJobDuration() == false) {
        return false;
    }
    if (!$("#IsBlendTest").prop("checked")) {
        if (!$("#IsThirdParty").prop("checked")) {
            if (!$("#IsThirdParty").prop("checked")) {
                if ($("#CrewId").find("option:selected").text() === "None") {
                    $("#CrewId_tip").show();
                    return false;
                }
            }
        } else {
            if ($("#ThirdPartyBulkerCrewId").find("option:selected").text() === "None") {
                $("#ThirdPartyBulkerCrewId_tip").show();
                return false;
            }
        }
    }
    $("#ExpectedOnLocationTime").val($("#GowithCrew_ExpectedOnLocationTime").val());
    return true;
}
function DataVerifyCrewSchedule() {
    var crewId;
   
    var isChecked = $("#IsThirdParty").prop("checked");
    if (isChecked) {
        crewId = $("#ThirdPartyBulkerCrewId").val();
    } else {
        crewId = $("#CrewId").val();
    }
    var data = {
        "crewId": crewId,
        "estimatedLoadTime": $("#EstimatedLoadTime").val(),
        "duration": $("#EstimatedTravelTime").val(),
        "isThirdParty": $("#IsThirdParty").prop("checked"),
        "isGoWithCrew": $("#IsGoWithCrew").prop("checked"),
        "rigJobId": $("#RigJobId").val()
    };
    return data;
}
function RescheduleProductHaulLoadVerifyCrewSchedule() {
    var verifySchedule = false;
    var itemurl = siteInstance.baseControllerActionUrls.productHaul.verifyCrewSchedule;
    $.ajax({
        url: itemurl,
        async: false,
        method: "Post",
        data: DataVerifyCrewSchedule(),
        success: function (data) {
            if (data === '') {
                verifySchedule = true;
            } else {
                $("#message").html(data);
                message = data;
                dialogSchedule.openDialog();
                verifySchedule = false;
            }
        }
    });
    return verifySchedule;
}
function validateSchedule() {

    var itemurl = null;
    var data = dataScheduleValidation();

    if (!$("#IsThirdParty").prop("checked")) {
        itemurl = siteInstance.baseControllerActionUrls.calendar.verifySanjelCrewSchedule;
    }
    else {
        itemurl = siteInstance.baseControllerActionUrls.calendar.verifyThirdPartyCrewSchedule;
    }

    return verifyScheduleValidation(itemurl, data);
}

function VerifCrewSchedule() {

    var itemurl = null;
    var data = null;
    if (!$("#IsExistingHaul").prop("checked")) {
        if (!$("#IsThirdParty").prop("checked")) {
            if ($("#IsGoWithCrew").prop("checked")) {
                data = { "crewId": $("#CrewId").val(), "startTime": $("#EstimatedLoadTime").val(), "rigJobId": $("#RigJobId").val() };
            } else {
                data = { "crewId": $("#CrewId").val(), "startTime": $("#EstimatedLoadTime").val(), "duration": $("#EstimatedTravelTime").val() };
            }
            itemurl = siteInstance.baseControllerActionUrls.calendar.verifySanjelCrewSchedule;
        } else {
            if ($("#IsGoWithCrew").prop("checked")) {
                data = { "thirdPartyBulkerCrewId": $("#thirdPartyBulkerCrewId").val(), "startTime": $("#EstimatedLoadTime").val(), "rigJobId": $("#RigJobId").val() };
            } else {
                data = { "thirdPartyBulkerCrewId": $("#thirdPartyBulkerCrewId").val(), "startTime": $("#EstimatedLoadTime").val(), "duration": $("#EstimatedTravelTime").val() };
            }
            itemurl = siteInstance.baseControllerActionUrls.calendar.verifyThirdPartyCrewSchedule;
        }

        var flag = verifyScheduleValidation(itemurl, data);
        console.log("flag=" + flag);
        return flag;
    }
}

var message = null;
function verifyScheduleValidation(url, data) {
    var isPass = false;

    $.ajax({
        url: url,
        async: false,
        method: "Post",
        data: data,
        success: function (data) {
            if (data != "") {
                $("#message").html(data);
                message = data;
                isPass = false;
                dialogSchedule.openDialog();
            } 
            else {
                isPass = true;
            }
        }
    });
    return isPass;
}
function dataQualityOfTheBin() {
    var model = {
        binInformationId: $("#BinInformationId").val(),
        amount: $("#Amount").val(),
        blendSectionId: $("#BaseBlendSectionId").val(),
        callSheetNumber: $("#CallSheetNumber").val(),
        binNumber: $("#BinNumber").val()
    };
    return model;
}

function VerifyQualityOfTheBin() {
    var model = dataQualityOfTheBin();
    var isPass = false;
    $.ajax({
        url: siteInstance.baseControllerActionUrls.productHaul.verifyQualityOfTheBin,
        async: false,
        method: "get",
        data: model,
        success: function (data) {
            console.log(data);
            if (data != "") {
                console.log(123);
                $("#binmessage").html(data);
                message = data;
                isPass = false;
                dialogQualityOfTheBin.openDialog();
            } 
            else {
                isPass = true;
            }
        }
    });
    return isPass;
}



function getDriver(obj) {
    if ($(obj).find("option:selected").text() == "None") {
        $("#DriverId_Tip").html("*required");
    } else {
        $("#DriverId_Tip").html("");
    }
}

function getUnit(obj) {
    $("#BulkUnitName").val($(obj).find("option:selected").text());
    if ($(obj).find("option:selected").text() == "None") {
        $("#BulkUnitId_Tip").html("*required");
    } else {
        $("#BulkUnitId_Tip").html("");
    }
}

function getBinType(obj) {
    $("#BinNumber").val($(obj).find("option:selected").text());
    console.log($(obj).find("option:selected").attr('PodIndex'));
    $("#PodIndex").val($(obj).find("option:selected").attr('PodIndex'));
    validateBin();
    CompareBlendDescriptionAndBin(1,obj);
}

function getBulkPlant(obj) {
    $("#BulkPlantName").val($(obj).find("option:selected").text());
}

function getExistingProductHaulId(obj) {
    $("#ProductHaulId").val($(obj)).find("option:selected").value();
}


function getTractorUnit(obj) {
    $("#TractorUnitName").val($(obj).find("option:selected").text());
}

function getSupplierCompany(obj) {
    $("#SupplierCompanyName").val($(obj).find("option:selected").text());
}

function getEstJobDuration() {
    var reg = /^(-?\d+)(\.\d+)?$/;
    $("#validValue").html("");
    var duration;
    if ($("#EstimatedTravelTime").val() === "") {
        duration = 4;
        $("#EstimatedTravelTime").val(duration);
    } else {
        duration = $("#EstimatedTravelTime").val();
    }
    if (duration < 1 || duration > 72) {
        $("#validValue").html("*between 1 and 72");
        
        return false;
    }
    if (!reg.test(duration)) {
        $("#validValue").html("*Input number type");
        
        return false;
    }
}

var ThirdParty = function (functionsObject) {
    this.OnThirdPartyTrue = functionsObject && functionsObject.OnThirdPartyTrue || function () {
        loadSelectList(siteInstance.baseControllerActionUrls.productHaul.getThirdPartyCrews, {'crewId': $("#OrigThirdPartyBulkerCrewId").val()},
            $("#thridPartyCrew_select #ThirdPartyBulkerCrewId"));

        $("#Crew").hide();
        $("#ThridPartyCrew").show();
        //$("#IsThirdParty").val(true);
        $("#sanjelCrews_PartialView").empty();
    };

    this.OnThirdPartyFalse = functionsObject && functionsObject.OnThirdPartyFalse || function () {
        loadSelectList(siteInstance.baseControllerActionUrls.productHaul.getSanjelBulkerCrews, { 'crewId': $("#OrigCrewId").val(), 'onLocationTime': $("#exp_on_location_time").val() },
            $("#crew_select #CrewId"));

        $("#Crew").show();
//        $("#CrewId").attr("disabled", false);
        $("#ThridPartyCrew").hide();
        //$("#IsThirdParty").val(false);
    };
};

ThirdParty.prototype = {
    getThirdParty: function () {

        var isChecked = $("#IsThirdParty").prop("checked");
        if (isChecked) {
            this.OnThirdPartyTrue.call(this);
        }
        else {
            this.OnThirdPartyFalse.call(this);
        }
    }
}

function defaultExistingHaulTrue() {
    $("#regular_haul_details").hide();
    $("#haulList_wrapper").show();
    $("#IsExistingHaul").val(true);

    loadSelectList(siteInstance.baseControllerActionUrls.productHaul.getExistingProductHauls,
        { 'productHaulId': $("#OriginalProductHaulId").val()},
        $("#existingHaul_select #ProductHaulId"));

}


function defaultExistingHaulFalse() {
    $("#regular_haul_details").show();
    $("#haulList_wrapper").hide();
    $("#IsExistingHaul").val(false);
}

/**
 * defines a class and a pair of functions to execute when existing haul is checked or not
 * @param {object} functionsObject
 */
var ExistingHaul = function (functionsObject) {
    this.OnExistingHaulTrue = functionsObject && functionsObject.OnExistingHaulTrue || defaultExistingHaulTrue;
    this.OnExistingHaulFalse = functionsObject && functionsObject.OnExistingHaulFalse || defaultExistingHaulFalse;
};

ExistingHaul.prototype = {
    setExistingHaul: function () {

        var isChecked = $("#IsExistingHaul").prop("checked");
        if (isChecked) {
            this.OnExistingHaulTrue.call(this);
        }
        else {
            this.OnExistingHaulFalse.call(this);
            (new ThirdParty()).getThirdParty();
        }
    }
}

function getGoWithCrew() {

    var isChecked = $("#IsGoWithCrew").prop("checked");
    if (isChecked) {
        setGoWithCrewTime();
        $("#ExpOlTime").hide();
        $("#EstimateDuration").hide();

    } else {
       $("#ExpOlTime").show();
       $("#EstimateDuration").show();
    }
    validateBin();
}

function clearTextBoxValues(selector) {
    var textboxes = document.querySelectorAll(selector);
    textboxes.forEach(function (textbox) {
        textbox.value = '';
    });
}


//jan 17, 2024 tongtao 264_PR_GoWithCrewError: set location time and load time,EstimateDuration default value
function setGoWithCrewTime() {
    var currentDate = new Date();
    var formattedDate = formatDate(currentDate);


    $("#exp_on_location_time").val(formattedDate);
    $("#exp_load_time").val(formattedDate);

    document.getElementById('EstimateDuration').querySelector('.sj_text').value = '4';
}


function formatDate(date) {
    var dateObject = new Date(date);
    var month = (dateObject.getMonth() + 1).toString();
    var day = dateObject.getDate().toString();
    var year = dateObject.getFullYear();
    var hours = dateObject.getHours().toString();
    var minutes = dateObject.getMinutes().toString().padStart(2, '0');

    var formattedDate = month + '/' + day + '/' + year + ' ' + hours + ':' + minutes;
    return formattedDate;
}


function clearTextBoxValues(selector) {
    var textboxes = document.querySelectorAll(selector);
    textboxes.forEach(function (textbox) {
        textbox.value = '0';
    });
}


var defaultOnBlendTestClick = function (show) {
    var wrapper = $("#non_blend_test_wrapper");
    show ? wrapper.show() : wrapper.hide();
}

/**
 * defines a class and a pair of functions to execute whether blend Test is checked or not
 * @param {object} functionsObject 
 */
var BlendTest = function (functionsObject) {
    if (!functionsObject) throw "BlendTest functions not provided";

    this.onBlendTestTrue = functionsObject.OnBlendTestTrue;
    this.onBlendTestFalse = functionsObject.OnBlendTestFalse;
    this.testingElement = functionsObject.testingElement || "#IsBlendTest";
};

BlendTest.prototype = {
    getBlendTest: function () {

        var isChecked = $(this.testingElement).prop("checked");

        if (isChecked) {
            this.onBlendTestTrue.call(this);
        } else {
            this.onBlendTestFalse.call(this);
        }
    }
}

function getExistingProductHaul(obj) {

}
function LoadBinInformations() {
    var isChecked = $("#IsBlendTest").prop("checked");
    if (isChecked) {
        loadBulkPlantBinInformations();
    }
    else {
        loadRigBinInformations();
    }
}
function loadBulkPlantBinInformations() {
    var bulkPlantId = $("#BulkPlantId").find("option:selected").val();
    $("#BulkPlantName").val( $("#BulkPlantId").find("option:selected").text())
 
    loadSelectList(siteInstance.baseControllerActionUrls.binBoard.getBinInformationsByBulkPlantId,
        { 'bulkPlantId': bulkPlantId, 'binInformationId': $("#OrigBinInformationId").val()},
        $("#bin_select #BinInformationId"));
}
function loadRigBinInformations() {
    loadSelectList(siteInstance.baseControllerActionUrls.binBoard.getBinInformationsByRigId,
        { 'rigId':  $("#RigId").val(), 'binInformationId': $("#OrigBinInformationId").val()},
        $("#bin_select #BinInformationId"));
    $("#BulkPlantName").val( $("#BulkPlantId").find("option:selected").text())
}

function LoadBins() {
    var isChecked = $("#IsBlendTest").prop("checked");
    if (isChecked) {
        loadBulkPlantBins();
    }
    else {
        loadRigBins();
    }
}
function loadBulkPlantBins() {
    var bulkPlantId = $("#BulkPlantId").find("option:selected").val();
    var itemUrl = siteInstance.baseControllerActionUrls.binBoard.getBinsByBulkPlantId;
    $.ajax({
        url: itemUrl,
        method: "post",
        async:false,
                data: { "bulkPlantId": bulkPlantId },
        success: function (data) {
            console.log(data.length != 0);
            $("#bin_select #BinId").empty();
            if (data.length != 0) {
                $.each(data, function (i, e) {
                    $("#bin_select #BinId").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
                });
            } else {
                $("#bin_select #BinId").append("<option>None</option>");
            }
        }
    });
}
function loadRigBins() {
    var itemUrl = siteInstance.baseControllerActionUrls.binBoard.getBinsByRigId;
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "rigId": $("#RigId").val() },
        success: function (data) {
            console.log(data.length != 0);
            $("#bin_select #BinId").empty();
            if (data.length != 0) {
                $("#bin_select #BinId").append("<option>None</option>");
                $.each(data,
                    function (i, e) {
                        $("#bin_select #BinId").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
                    });
            } else {
                $("#bin_select #BinId").append("<option>None</option>");
            }
        }
    });

}
function loadSanjelCrews() {
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getSanjelCrews;
    $.ajax({
        url: itemUrl,
        method: "post",
        data: { "crewId": $("#CrewId").val() },
        success: function (data) {
            console.log(data.length != 0);
            $("#crew_select #CrewId").empty();
            if (data.length != 0) {
                $("#crew_select #CrewId").append("<option>None</option>");
                $.each(data,
                    function (i, e) {
                        if(!e.Selected)
                        $("#crew_select #CrewId").append("<option value='" + e.Value + "'>" + e.Text + "</option>");
                        else
                            $("#crew_select #CrewId").append("<option selected='selected' value='" + e.Value + "'>" + e.Text + "</option>");
                    });
            } else {
                $("#crew_select #CrewId").append("<option>None</option>");
            }
        }
    });
}
function loadBulkPlants() {
    loadSelectList(siteInstance.baseControllerActionUrls.productHaul.getBulkPlants,
        { 'bulkPlantId': $("#OrigBulkPlantId").val() },
        $("#bulk_plant_select #BulkPlantId"));
}
function loadSelectList(itemUrl, dataObj,  listContainer) {
    $.ajax({
        url: itemUrl,
        method: "post",
        async:false,
                data: dataObj,
        success: function (data) {
            console.log(data.length != 0);
            listContainer.empty();
            if (data.length != 0) {
                listContainer.append("<option>None</option>");
                $.each(data,
                    function (i, e) {
                        if(!e.Selected)
                            listContainer.append("<option value='" + e.Value + "'>" + e.Text + "</option>");
                        else
                            listContainer.append("<option selected='selected' value='" + e.Value + "'>" + e.Text + "</option>");
                    });
            } else {
                listContainer.append("<option>None</option>");
            }
        }
    });
}


function loadPrograms(obj) {
    var itemUrl = siteInstance.baseControllerActionUrls.rigBoard.getPrograms;
    var customerId =
        $.ajax({
            url: itemUrl,
            method: "get",
            data: { "customerId": $(obj).find("option:selected").val() },
            success: function (data) {
                console.log(data.length != 0);
                $("#program_select #ProgramId").empty();
                if (data.length != 0) {
                    $.each(data,
                        function (i, e) {
                            $("#program_select #ProgramId")
                                .append("<option value='" + e.Value + "'>" + e.Text + "</option>");
                        });
                } else {
                    $("#program_select #ProgramId").append("<option>None</option>");
                }
            }
        });
}

var programInfo;
var blendInfoSections;
function onProgramIdChange(obj) {

    var itemUrl = siteInstance.baseControllerActionUrls.rigBoard.GetProgramInfo;

    /* Nov 7, 2023 tongtao P45_Q4_167: add ProgramId format validation.*/
    var pattern = /^PRG\d*\.\d{2}$/;

    if (!pattern.test(obj.value)) {
        $("#Is_programNumberFormat").show();

        $("#sj_createProductHual").attr("disabled", true);

        return;
    } else {
        $("#Is_programNumberFormat").hide()

        $("#sj_createProductHual").attr("disabled", false);
    }

    $.ajax({
        url: itemUrl,
        method: "get",
        data: { "programId": obj.value },
        success: function (data) {
            programInfo = data;
            $("#ClientName").val(data.ClientName);
            $("#CustomerId").val(data.CustomerId);
            $("#ServicePointId").val(data.ServicePointId);
            $("#ServicePointName").val(data.ServicePointName);
            $("#jobtype_select #JobTypeId").empty();
            var count = $.map(data.JobTypes, function (n, i) { return i; }).length;
            console.log(count);
            if (count !== 0) {
                for (key in data.JobTypes) {
                    $("#jobtype_select #JobTypeId").append("<option value='" + key + "'>" + data.JobTypes[key].JobType + "</option>");
                }
            } else {
                $("#jobtype_select #JobTypeId").append("<option>None</option>");
            }

        }
    });
}

function loadBlends(obj) {
    $("#blend_select #BaseBlendSectionId").empty();
    blendInfoSections = programInfo.JobTypes[$(obj).find("option:selected").val()].BlendSectionList;
    var count = $.map(blendInfoSections, function (n, i) { return i; }).length;
    console.log(count);
    if (count === 0) {
        $("#blend_select #BaseBlendSectionId").append("<option>None</option>");
    } else {
        for (key in blendInfoSections) {
            $("#blend_select #BaseBlendSectionId")
                .append("<option value='" + key + "'>" + blendInfoSections[key].Description + "</option>");
        }
    }
}


function getShift(obj) {
    if ($(obj).length > 0) {
        $.ajax({
            url: siteInstance.baseControllerActionUrls.productHaul.getProductHaulInfoById,
            type: 'POST',
            async: false,
            data: {
                "productHaulId": obj.value
            },
            dateType: "json",
            success: function (data) {
                $("#ProductHaulId").val(data.ProductHaulId);

                if (data.IsThirdParty) {
                    $("#existing_thirdParty").prop('checked', true);
                    $("#existing_haul_driver").hide();
                    $("#existing_haul_driver2").hide();
                    $("#existing_haul_primary").hide();
                    $("#existing_haul_tractor").hide();
                    $("#existing_isThirdParty").show();
                    $("#existing_haul_supplier").show();
                    $("#existing_haul_unitNumber").show();
                    $("#existing_haul_contactName").show();
                    $("#existing_haul_contactNumber").show();
                    $("#existing_supplier").val(data.SupplierCompanyName);
                    $("#existing_unitNumber").val(data.ThirdPartyUnitNumber);
                    $("#existing_contactName").val(data.SupplierContactName);
                    $("#existing_contactNumber").val(data.SupplierContactNumber);
                    $("#div_SanjelCrew").hide();
                    $("#div_ThirdPartyBulkerCrew").show();
                    $("#existing_ThirdPartyBulkerCrew").val(data.CrewName);
                } else {
                    $("#existing_thirdParty").prop('checked', false);
                    $("#existing_haul_supplier").hide();
                    $("#existing_haul_unitNumber").hide();
                    $("#existing_haul_contactName").hide();
                    $("#existing_haul_contactNumber").hide();
                    $("#existing_isThirdParty").show();
                    $("#existing_haul_driver").show();
                    $("#existing_haul_driver2").show();
                    $("#existing_haul_primary").show();
                    $("#existing_haul_tractor").show();
                    $("#existing_driver").val(data.PreferedName);
                    $("#existing_driver2").val(data.Driver2PreferedName);
                    $("#existing_primary").val(data.BulkUnitName);
                    $("#existing_tractor").val(data.TractorUnitName);
                    $("#div_ThirdPartyBulkerCrew").hide();
                    $("#div_SanjelCrew").show();
                    $("#existing_SanjelCrew").val(data.CrewName);

                }

                if (data.IsGoWithCrew) {
                    $("#existing_expectedOlTime").hide();
                    $("#existing_haul_go_With_crew").prop('checked', true);
                    //$("#IsGoWithCrew").val(true);
                } else {
                    $("#existing_haul_go_With_crew").prop('checked', false);
                    $("#existing_expectedOlTime").show();
                    $("#existing_olTime").val(data.ShortExpOlTime);
                    $("#ExpectedOnLocationTime").val(data.ExpectedOnLocationTime);
                    //$("#IsGoWithCrew").val(false);
                }
                $("#existing_go_with_crew").show();
            }
        });        
    }
}


function fillMixWater(obj) {
    $("#MixWater").empty();
    if (validateRequiredBlend()) {
        var blendInfoSection = blendInfoSections[$(obj).find("option:selected").val()];
        if (blendInfoSection.IsBlendTest && $("#BulkPlantName").val() === "") {
            $(obj).val("0");
            $("#blend_need_test").show();
        } else {
            $("#MixWater").val(blendInfoSection.MixWater);
            $("#blend_need_test").hide();
            $("#IsBlendTest").prop("checked", blendInfoSection.IsBlendTest);
        }
      //Nov 28, 2023 zhangyuan 201_PR_AddingDifferrentBlend: Add RescheduleProductHaul Get BlendDescription and Display prompt information
        LoadBlendDescriptionAndSetBinDisplay();
    }
}

function validateRequiredBlend() {
    if ($("#BaseBlendSectionId").find("option:selected").text() == "None") {
        $("#blend_required").show();
        return false;
    } else {
        $("#blend_required").hide();
        return true;
    }
}
  //Nov 13, 2023 zhangyuan P63_Q4_174: add TransferBlend 
function loadTransForBins(obj) {
    var ToBinId = $(obj).find("option:selected").val();
    var itemUrl = siteInstance.baseControllerActionUrls.binBoard.getBinInformationById;
    if (ToBinId == "") {
        $("#Bin_Required").show();
        $("#BlendinBin").hide();
        return false
    }
    else {
        $("#Bin_Required").hide();
    }
    $.ajax({
        url: itemUrl,
        method: "post",
        async: false,
        data: { "binInformationId": ToBinId },
        success: function (data) {
            console.log(data.length != 0);
            $("#BlendinBin #BlendInBinDescription").html(data.BlendChemical.Description);
            if (data.Quantity < 0.001) {
                $("#BlendinBin").hide();
            }
            else {
                $("#BlendinBin").show();
            }

            if (data.Quantity > 0.001&&$("#BlendToLoadDescription").html() != $("#BlendinBin #BlendInBinDescription").html()) {
                $("#differentblend").show();
                $("#thesameblend").hide();
            }
            else {
                $("#differentblend").hide();
                $("#thesameblend").show();
            }
        }
    
    });
}
function validateRequiredLoadToBin() {
    if (($("#ToBinInformationId").find("option:selected").text() == "" || $("#ToBinInformationId").find("option:selected").text() == "None")
        && $("#ToBinInformationId").val() == 0) {
        $("#Bin_Required").show();
        return false;
    } else {
        $("#Bin_Required").hide();
        return true;
    }
}
function GreaterthanQuantity(origin, target) {
    $("#Is_" + target).hide();
    if (Number($("#" + origin).val().trim()) < Number($("#" + target).val().trim())) {
        $("#Greaterthan_" + target).show();
        return false;
    } else {
        $("#Greaterthan_" + item).hide();
        return true;
    }
}
  //Nov 28, 2023 zhangyuan 201_PR_AddingDifferrentBlend: Add Get BlendDescription and Display prompt information
//Dec 7, 2023 zhangyuan 228_PR_AddBlendinbinAlert: Add Get BlendDescription and Display prompt information
function LoadBlendDescriptionAndSetBinDisplay() {
    var itemUrl = siteInstance.baseControllerActionUrls.productHaul.getOriginBlendSectionDescription;
    var blendSectionId = $("#BaseBlendSectionId").val();
    var callSheetId = $("#callSheetId").val();
    if (blendSectionId == null || blendSectionId == 0)
        return;
    if (!callSheetId) { callSheetId = 0; }
    var blendDescription = $("#BlendChemicalDescription").val();
    console.log(blendDescription);
    if (!blendDescription) {
        $.ajax({
            url: itemUrl,
            method: "post",
            async: false,
            data: { "sectionId": blendSectionId, "callSheetId": callSheetId },
            success: function (data) {
                blendDescription = data;
            }
        });
    }
    var detlist = $('#rigUnload .det_list');
    if (detlist) {
        detlist.each(function (index, element) {
            var quantity = $(this).find('[name="BlendUnloadSheetModels[' + index + '].DestinationStorage.Quantity"]').val();
            var description = $(element).find('[name="BlendUnloadSheetModels[' + index + '].DestinationStorage.BlendChemical.Description"]').val();
            var binName = $(element).find('[name="BlendUnloadSheetModels[' + index + '].DestinationStorage.Name"]').val();
            if (quantity > 0.001 && description != blendDescription) {
                $(element).find('[name="Bin_Different_' + binName + '"]').show();
            }
            else {
                $(element).find('[name="Bin_Different_' + binName + '"]').hide();
            }
        })
    }
    // schedule blend request dupe
    var selectDes = $('#SelectedBlendChemicalDescription');
    if (selectDes) {
        $('#SelectedBlendChemicalDescription').val(blendDescription)
        //bin is input tag logic
        if ($('#BinInformationId').is('input')) {
            CompareBlendDescriptionAndBin(2, $('#BinInformationId'))
        }
        else if ($('#BinInformationId').is('select')) {
            $('#BinInformationId').val();
            CompareBlendDescriptionAndBin(1, $('#BinInformationId'))
        }
    }
}
//Dec 7, 2023 zhangyuan 228_PR_AddBlendinbinAlert: Add Get BlendDescription and Display prompt information
// type 1: ddl 2:input
function CompareBlendDescriptionAndBin(type, obj) {
    var ToBinId = 0;
    var blendDescription = $('#SelectedBlendChemicalDescription').val()
    var itemUrl = siteInstance.baseControllerActionUrls.binBoard.getBinInformationById;
    if (type == 1) {
        var ToBinId = $(obj).find("option:selected").val();
    }
    else {
        var ToBinId = $(obj).val();
    }

    if (!ToBinId || ToBinId == "None") {
        $("#bin_select #Bin_Different").hide();
        return false
    }
    $.ajax({
        url: itemUrl,
        method: "post",
        async: false,
        data: { "binInformationId": ToBinId },
        success: function (data) {
            console.log(data.length != 0);
            if (data.Quantity > 0.001 && blendDescription != data.BlendChemical.Description) {
                $("#bin_select #Bin_Different").html(Math.round(data.Quantity * 1000) / 1000 + "t " + data.BlendChemical.Name.replace(" + Additives", "") + " in Bin").show();
            }
            else {
                $("#bin_select #Bin_Different").hide();
            }
        }

    });

}
 // Dec 25, 2023 zhangyuan 195_PR_Haulback: Add HaulBack  Change BulkerPlant event
function getBulkerPlantBinType(obj) {
    $("#BinNumber").val($(obj).find("option:selected").text());
    console.log($(obj).find("option:selected").attr('PodIndex'));
    $("#PodIndex").val($(obj).find("option:selected").attr('PodIndex'));
    //validateBin();
    $('#SelectedBlendChemicalDescription').val($('#BlendChemicalDescription').val());
    CompareBlendDescriptionAndBin(1, obj);
}

function ValidateHaulBackHaulAmount() {
    if (Number($("#LoadAmount").val()) > Number($("#SourceAmount").val())) {
        $("#haulAmountMessage").html('Haul Amount Should Less Than Remains Amount!<br/>Do You Want To Continue?<br>Click "Yes" To Continue!');
        dialogHaulAmount.openDialog();
        return false;
    }
    else {
        return true;
    }
}
//Dec 27, 2023 zhangyuan 243_PR_AddBlendDropdown:Add validateBlend ddl
function validateBlend() {
    if ($(".det_text #BlendId").find("option:selected").text() === "None") {
        $("#Is_BlendRequired").show();
        return false;
    } else {
        $("#Is_BlendRequired").hide();
    }
    return true;
}
