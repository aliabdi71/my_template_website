using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Entity;

namespace Amirhome.Models
{
    public class AdverShowModelView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string District { get; set; }
        public string Condition { get; set; }
        public string FirstPrice { get; set; }
        public string SecondPrice { get; set; }
        public string Date { get; set; }
        public string ImgUrl { get; set; }
    }
    public class AdvertiseManager
    {
        public List<FreeAdvertise> getAdvertises()
        {
            List<FreeAdvertise> all_advers;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    all_advers = (from AD in context.FreeAdvertises
                                  where AD.expire_date >= DateTime.Now & AD.approved == true
                                  orderby AD.expire_date descending
                                  select AD).ToList();
                }
                return all_advers;
            }
            catch
            {
                return null;
            }
        }
        public List<FreeAdvertise> getALLAdvertises()
        {
            List<FreeAdvertise> all_advers;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    all_advers = (from AD in context.FreeAdvertises
                                  select AD).ToList();
                }
                return all_advers;
            }
            catch
            {
                return null;
            }
        }
        public bool insertAdvertise(FreeAdvertise model)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    context.FreeAdvertises.Add(model);
                    context.Entry(model).State = EntityState.Added;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public FreeAdvertise getAdvertiseById(int id)
        {
            FreeAdvertise model = null;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    model = (from AD in context.FreeAdvertises
                             where AD.ID == id
                             select AD).FirstOrDefault();
                }
                return model;
            }
            catch
            {
                return null;
            }
        }
        public bool approveAddvertise(int id, bool flag)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    FreeAdvertise addver = (from A in context.FreeAdvertises
                                            where A.ID == id
                                            select A).First();
                    addver.approved = flag;
                    context.FreeAdvertises.Attach(addver);
                    context.Entry(addver).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public string deleteAddvertise(int id)
        {
            string res;
            using (var context = new AmirhomeEntities())
            {
                FreeAdvertise addver = (from A in context.FreeAdvertises
                                        where A.ID == id
                                        select A).First();
                res = addver.image;
                context.FreeAdvertises.Remove(addver);
                context.Entry(addver).State = EntityState.Deleted;
                context.SaveChanges();
                return res;
            }
        }
        public int[] getAddvertiseSubmitedAfter(DateTime date)
        {
            int[] ids = { };
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    ids = (from A in context.FreeAdvertises
                           where A.create_date.Value > date
                           select A.ID).ToArray();
                }
                return ids;
            }
            catch
            {
                return ids;
            }
        }
    }
}