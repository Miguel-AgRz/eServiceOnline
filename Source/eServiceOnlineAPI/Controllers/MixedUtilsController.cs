﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using eServiceOnline.WebAPI.Data.EMail;

using Spire.Pdf;
using Spire.Pdf.Exporting;
//using Spire.Pdf.Conversion.;


namespace eServiceOnline.WebAPI.Controllers
{
    //Sample: http://localhost:52346/MixedUtils/GetEmailOAuth2Token
    //Sample: http://sanjel04/eServiceOnline.SPWebApi/MixedUtils/GetEmailOAuth2Token

    [ApiController]
    public class MixedUtilsController : ControllerBase
    {


        [Route("[controller]/[action]")]
        public ActionResult GetEmailOAuth2Token()
        {
            string token = "";

            try
            {
                token = EMailUtils.GetToken();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, token, message = ex.Message });
            }

            return new JsonResult(new { result = true, token, message = "Succeed" });
        }


        //Sample: http://localhost:52346/MixedUtils/SendEmail?jsonToSubjBody=
        //[HttpGet]
        [Route("[controller]/[action]")]
        public ActionResult SendEmail(string? jsonToSubjBody)
        {
            try
            {
                //EMailUtils.SendEmailViaEWS(jsonToSubjBody);
                EMailUtils.SendGraphEmailNoAttachmentsWait(jsonToSubjBody);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, message = ex.Message });
            }

            return new JsonResult(new { result = true, message = "Succeed" });
        }

        [Route("[controller]/[action]")]
        public ActionResult SendEmailDirect(string to, string subject, string body, string cc)
        {
            try
            {
                //EMailUtils.SendEmailViaEWS(jsonToSubjBody);
                EMailUtils.SendGraphEmailNoAttachmentsWait(to, subject, body, cc);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, message = ex.Message });
            }

            return new JsonResult(new { result = true, message = "Succeed" });
        }


        [Route("[controller]/[action]")]
        public ActionResult SendEmailViaSharedLocation(string? emailContentJsonFileFullPath)
        {
            try
            {

                //EMailUtils.SendEmailViaEWS(jsonToSubjBody);
                //EMailUtils.SendEmailFromDefaultLocation();
                //EMailUtils.SendEmailWithGraphAsyncAndBigAttachmentsWait();
                EMailUtils.SendGraphEmailWithAttachmentsWait(emailContentJsonFileFullPath);
                EMailUtils.MarkAsProcessed(emailContentJsonFileFullPath);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, message = ex.Message });
            }

            return new JsonResult(new { result = true, message = "Succeed" });
        }

        [Route("[controller]/[action]")]
        public ActionResult CompressPdf(string? sourcePath, string? targetPath)
        {
            try
            {

                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(sourcePath);

                //CompressContent(doc);
                //Disable the incremental update
                doc.FileInfo.IncrementalUpdate = false;
                //Set the compression level to best
                doc.CompressionLevel = PdfCompressionLevel.Best;

                //CompressImage(doc);
                //Traverse all pages
                foreach (PdfPageBase page in doc.Pages)
                {
                    if (page != null)
                    {
                        if (page.ImagesInfo != null)
                        {
                            foreach (PdfImageInfo info in page.ImagesInfo)
                            {
                                page.TryCompressImage(info.Index);
                            }
                            foreach (PdfImageInfo info in page.ImagesInfo)
                            {
                                info.Image.Dispose();
                            }
                        }
                    }
                }

                doc.SaveToFile(targetPath);


                //IronPdf.PdfDocument PDF = IronPdf.PdfDocument.FromFile(sourcePath);
                //PDF.SaveAs(targetPath);


                /* 
                    //Spire.PDF paid version

                    //Load a PDF document while initializing the PdfCompressor object
                    PdfCompressor compressor = new PdfCompressor(sourcePath);
                    //Get text compression options
                    TextCompressionOptions textCompression = compressor.Options.TextCompressionOptions;
                    //Compress fonts
                    textCompression.CompressFonts = true;
                    //Get image compression options
                    ImageCompressionOptions imageCompression = compressor.Options.ImageCompressionOptions;
                    //Set the compressed image quality
                    imageCompression.ImageQuality = ImageQuality.High;
                    //Resize images
                    imageCompression.ResizeImages = true;
                    //Compress images
                    imageCompression.CompressImage = true;
                    //Save the compressed document to file
                    compressor.CompressToFile(targetPath);
                */
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, message = ex.Message });
            }

            return new JsonResult(new { result = true, message = "Succeed" });
        }

        [Route("[controller]/[action]")]
        public ActionResult SendTestEmail()
        {
            try
            {
                //EMailUtils.SendEmailViaEWS(jsonToSubjBody);
                EMailUtils.SendtestEmail();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = false, message = ex.Message });
            }

            return new JsonResult(new { result = true, message = "Succeed" });
        }

    }
}