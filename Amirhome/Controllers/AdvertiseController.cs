using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;
using System.Drawing;

namespace Amirhome.Controllers
{
    public class AdvertiseController : Controller
    {
        AdvertiseManager _adverManager = new AdvertiseManager();

        public ActionResult InsertAdvertise()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertAdvertise(FreeAdvertise model, HttpPostedFileBase[] upload_image)
        {
            List<Tuple<byte[], string>> posted_files_img = new List<Tuple<byte[], string>>();
            model.image = String.Empty;
            model.create_date = DateTime.Now;
            model.expire_date = DateTime.Now.AddMonths(3);
            model.approved = false;
            foreach (var item in upload_image)
            {
                if (item != null)
                {
                    string img_name = Guid.NewGuid() + ".jpg";
                    int nFileLen = item.ContentLength;
                    byte[] myData = new Byte[nFileLen];
                    item.InputStream.Read(myData, 0, nFileLen);
                    item.InputStream.Dispose();
                    posted_files_img.Add(new Tuple<byte[], string>(myData, img_name));
                }
            }
            if(posted_files_img.Count > 0)
                model.image = string.Join(";", posted_files_img.Select(p => p.Item2).ToArray());
            bool success = _adverManager.insertAdvertise(model);
            try
            {
                if (success)
                {
                    string filePath = "~/Content/advertise_images/",
                           tempFilePath = "~/Content/temp_img_folder/";
                    foreach (var item in posted_files_img)
                    {
                        System.IO.FileStream newFile
                                    = new System.IO.FileStream(Server.MapPath(tempFilePath + "_temp.jpg"),
                                                               System.IO.FileMode.Create);
                        newFile.Write(item.Item1, 0, item.Item1.Length);
                        bool upload_success = ResizeImageAndUpload(newFile, filePath + (item.Item2), 500, 700);//Save image your normal image path
                        //delete the temp file.
                        newFile.Close();
                        System.IO.File.Delete(Server.MapPath(tempFilePath + "_temp.jpg"));
                    }
                    return RedirectToAction("InsertAdvertiseSuccessful", "Advertise");
                }
                ViewBag.ErrMsg = "خطایی در حین ثبت آگهی رخ داده است. لطفا مجددا تلاش نمایید";
                return View(model);
            }
            catch
            {
                return RedirectToAction("InsertAdvertiseSuccessful", "Advertise");
            }
        }

        public ActionResult InsertAdvertiseSuccessful()
        {
            return View();
        }

        public ActionResult AddvertiseDetail()
        {
            int addID = int.Parse(Request.QueryString["AddvertiseID"].ToString());
            FreeAdvertise model = _adverManager.getAdvertiseById(addID);
            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home");
        }

        public JsonResult AddvertiseApprovement(int addID, bool flag)
        {
            bool res = _adverManager.approveAddvertise(addID, flag);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddvertiseDelete(int addID)
        {
            try
            {
                string imgs_to_delete = _adverManager.deleteAddvertise(addID);
                string[] imgs = imgs_to_delete.Split(';');
                foreach(var img in imgs)
                    System.IO.File.Delete(Server.MapPath("~/Content/advertise_images/" + img));
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }

        private bool ResizeImageAndUpload(System.IO.FileStream newFile, string folderPathAndFilename, double maxHeight, double maxWidth)
        {
            try
            {
                // Declare variable for the conversion
                float ratio;
                // Create variable to hold the image 
                System.Drawing.Image thisImage = System.Drawing.Image.FromStream(newFile);
                System.Drawing.Image waterMark = System.Drawing.Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/shared_images/logo2.png"));
                // Get height and width of current image
                int width = (int)thisImage.Width;
                int height = (int)thisImage.Height;
                // Ratio and conversion for new size
                if (width > maxWidth)
                {
                    ratio = (float)width / (float)maxWidth;
                    width = (int)(width / ratio);
                    height = (int)(height / ratio);
                }
                // Ratio and conversion for new size
                if (height > maxHeight)
                {
                    ratio = (float)height / (float)maxHeight;
                    height = (int)(height / ratio);
                    width = (int)(width / ratio);
                }
                // Create "blank" image for drawing new image
                Bitmap outImage = new Bitmap(width, height);
                Graphics outGraphics = Graphics.FromImage(outImage);
                SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
                // Fill "blank" with new sized image
                outGraphics.FillRectangle(sb, 0, 0, outImage.Width, outImage.Height);
                outGraphics.DrawImage(thisImage, 0, 0, outImage.Width, outImage.Height);
                outGraphics.DrawImage(waterMark, (outImage.Width - waterMark.Width) / 2, (outImage.Height - waterMark.Height) / 2, waterMark.Width, waterMark.Height);
                sb.Dispose();
                outGraphics.Dispose();
                thisImage.Dispose();
                // Save new image as jpg
                outImage.Save(Server.MapPath(folderPathAndFilename), System.Drawing.Imaging.ImageFormat.Jpeg);
                outImage.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}