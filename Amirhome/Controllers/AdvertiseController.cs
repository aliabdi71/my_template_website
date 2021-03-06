﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;
using System.Drawing;
using System.Net.Mail;

namespace Amirhome.Controllers
{
    public class AdvertiseController : Controller
    {
        AdvertiseManager _adverManager = new AdvertiseManager();
        UserManager _userManager = new UserManager();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["UserID"] != null)
            {
                if (Session["user_name"] == null)
                {
                    try
                    {
                        int uid = int.Parse(Session["UserID"].ToString());
                        UserAccouunt user = _userManager.getUserByID(uid);
                        string imageBase64 = Convert.ToBase64String(user.ProfileImage);
                        string imageSrc = string.Format("data:image/jpg;base64,{0}", imageBase64);
                        Session["user_name"] = user.Name;
                        Session["user_image_src"] = imageSrc;
                        Session["user_role_id"] = user.UserAccouuntsRole.ID;
                        Session["user_role_access"] = user.UserAccouuntsRole.Username;
                    }
                    catch
                    {
                        Session["user_name"] = null;
                        Session["user_image_src"] = null;
                    }
                }
                ViewData["user_name"] = Session["user_name"];
                ViewData["user_image_src"] = Session["user_image_src"];
                ViewData["user_role_id"] = Session["user_role_id"];
            }
        }

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
            model.edit_key = "amr" + Guid.NewGuid().ToString().Replace("-", "");
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
                    if (!string.IsNullOrEmpty(model.email))
                    {
                        string sender = "email@amirhome.ir";
                        string pass = "emailpass123";
                        string mail_subject = "مسکن امیر - لینک ویرایش آگهی";
                        string mail_body = "کاربر گرامی! با تشکر از انتخاب شما، می توانید از طریق لینک زیر آگهی خود را ویرایش نمایید.. \n";
                        mail_body += "شما تنها یک بار فرصت ویرایش آگهی را خواهید داشت. \n";
                        mail_body += "http://www.amirhome.ir/Advertise/EditAdvertise?code=" + model.edit_key;
                        var smtp = new SmtpClient()
                        {
                            Host = "amirhome.ir",
                            Port = 25,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Credentials = new System.Net.NetworkCredential(sender, pass),
                            Timeout = 20000,
                        };
                        try
                        {
                            smtp.Send(sender, model.email, mail_subject, mail_body);
                        }
                        catch
                        {
                            ViewBag.ErrMsg = "خطایی در هنگام ارسال لینک آگهی برای شما روی داد";
                        }
                    }
                    return RedirectToAction("InsertAdvertiseSuccessful", "Advertise", new { @message = "ایمیلی حاوی لینک ویرایش برای آگهی شما نیز ارسال شده است. دقت کنید که تنها یک بار می توانید آگهی خود را ویرایش نمایید." });
                }
                ViewBag.ErrMsg = "خطایی در حین ثبت آگهی رخ داده است. لطفا مجددا تلاش نمایید";
                return View(model);
            }
            catch
            {
                return RedirectToAction("InsertAdvertiseSuccessful", "Advertise");
            }
        }

        public ActionResult InsertAdvertiseSuccessful(string message = null)
        {
            if (message != null)
                ViewBag.Msg = message;
            return View();
        }

        [HttpPost]
        public JsonResult DoAdverSearch(AdverSearchParams _params, int page = 0)
        {
            if (_params == null)
                _params = (Session["adverParams"] == null) ? new AdverSearchParams() : (Session["adverParams"] as AdverSearchParams);
            else
                Session["adverParams"] = _params;
            List<FreeAdvertise> advers = _adverManager.AdverSearch(_params, page * 12);
            var data = advers.Select(A => new
            {
                Title = A.title,
                Condition = A.condition,
                FirstPrice = (A.condition.Equals("فروش")) ? A.price_total : A.price_prepayment,
                SecondPrice = (A.condition.Equals("فروش")) ? A.price_per_meter : A.price_mortage,
                Address = A.district,
                Area = A.area,
                ID = A.ID,
                ImageUrl = (string.IsNullOrEmpty(A.image)) ? ("no-thumb.png") : (A.image.Split(';')[0]),
                Date = getAdverDate(A.create_date)
            }).ToList();

            if(page > 0)
            {
                if (data.Count() < page * 12)
                {
                    data = data.Skip(Math.Max(0, data.Count() - (data.Count() % 12))).ToList();
                }
                else
                {
                    data = data.Skip(Math.Max(0, data.Count() - 12)).ToList();
                }
            }

            return Json(data);
        }
        public string getAdverDate(DateTime? date)
        {
            string final_date = "";
            int difference = (int)(DateTime.Now - date.Value).TotalDays;
            if (difference == 0)
            {
                final_date = "امروز";
            }
            else if (difference == 1)
            {
                final_date = "دیروز";
            }
            else if (difference == 2)
            {
                final_date = "دو روز پیش";
            }
            else if (difference == 3)
            {
                final_date = "سه روز پیش";
            }
            else if (difference == 4)
            {
                final_date = "چهار روز پیش";
            }
            else if (difference >= 5 && difference <= 7)
            {
                final_date = "هفته گذشته";
            }
            else if (difference > 7 && difference <= 14)
            {
                final_date = "دو هفته پیش";
            }
            else
            {
                final_date = "بیش از دو هفته پیش";
            }
            return final_date;
        }
        public ActionResult AddvertiseDetail()
        {
            int addID = int.Parse(Request.QueryString["AddvertiseID"].ToString());
            FreeAdvertise model = _adverManager.getAdvertiseById(addID);
            if (!model.approved.Value)
            {
                if (Session["user_role_id"] == null || !(Session["user_role_id"].ToString().Equals("1") || Session["user_role_id"].ToString().Equals("2")))
                    return RedirectToAction("Index", "Home");
            }
            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home");
        }

        public JsonResult AddvertiseApprovement(int addID, bool flag)
        {
            if (Session["user_role_id"] == null || Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Don't you have a job??");
            bool res = _adverManager.approveAddvertise(addID, flag);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddvertiseDelete(int addID)
        {
            if (Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Don't you have a job??");
            try
            {
                string imgs_to_delete = _adverManager.deleteAddvertise(addID);
                if (!string.IsNullOrEmpty(imgs_to_delete))
                {
                    string[] imgs = imgs_to_delete.Split(';');
                    foreach (var img in imgs)
                        System.IO.File.Delete(Server.MapPath("~/Content/advertise_images/" + img));
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult AddvertiseExtension(int addID)
        {
            if (Session["user_role_id"] == null || Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Don't you have a job??");
            bool res = _adverManager.extendAddvertise(addID);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAdvertise()
        {
            return View();
        }

        public ActionResult EditAdvertise()
        {
            string adver_edit_token = Guid.NewGuid().ToString();
            Session["adver_edit_token"] = adver_edit_token;
            ViewData["adver_edit_token"] = adver_edit_token;

            if (Request.QueryString.AllKeys.Contains("code"))
            {
                string code = Request.QueryString["code"].ToString();
                var model = _adverManager.getAdvertiseByCode(code);
                return View(model);
            }
            else if (
                    Request.QueryString.AllKeys.Contains("ID") &&
                    Session["user_role_id"] != null && 
                    (Session["user_role_id"].ToString().Equals("1") || Session["user_role_id"].ToString().Equals("2"))
                    )
            {
                string id = Request.QueryString["ID"].ToString();
                var model = _adverManager.getAdvertiseById(int.Parse(id));
                return View(model);
            }
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult EditAdvertise(FreeAdvertise model)
        {
            model.create_date = DateTime.Now;
            model.expire_date = DateTime.Now.AddMonths(3);
            if (Session["user_role_id"] == null || (!Session["user_role_id"].ToString().Equals("1") && !Session["user_role_id"].ToString().Equals("2")))
                model.edit_key = "amr" + Guid.NewGuid().ToString().Replace("-", "");
            model.approved = false;
            bool res = _adverManager.editAdvertise(model);
            if (res)
            {
                if (Session["user_role_id"] == null || (!Session["user_role_id"].ToString().Equals("1") && !Session["user_role_id"].ToString().Equals("2")))
                    return RedirectToAction("InsertAdvertiseSuccessful", "Advertise");
                else
                    return RedirectToAction("ManagementPanel", "Estate");
            }
            else
            {
                ModelState.AddModelError("", "خطا در به روز رسانی فایل. لطفا مجددا تلاش کنید");
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult AddvertiseGetContact(int id)
        {
            FreeAdvertise addver = _adverManager.getAdvertiseById(id);
            object res = new
            {
                Phone = addver.phone,
                Email = string.IsNullOrEmpty(addver.email) ? "ندارد" : addver.email,
            };
            return Json(res);
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