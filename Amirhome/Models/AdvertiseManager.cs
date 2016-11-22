using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Entity;

namespace Amirhome.Models
{
    public class AdverSearchParams
    {
        [DefaultValue(0)]
        public int minArea { get; set; }
        [DefaultValue(3000)]
        public int maxArea { get; set; }
        [DefaultValue(0)]
        public float minTotalPrice { get; set; }
        [DefaultValue(10000000000)]
        public float maxTotalPrice { get; set; }
        [DefaultValue(0)]
        public float minPricePerMeter { get; set; }
        [DefaultValue(30000000)]
        public float maxPricePerMeter { get; set; }
        [DefaultValue(0)]
        public float minPricePrepayment { get; set; }
        [DefaultValue(10000000000)]
        public float maxPricePrepayment { get; set; }
        [DefaultValue(0)]
        public float minPriceMortage { get; set; }
        [DefaultValue(30000000)]
        public float maxPriceMortage { get; set; }
        [DefaultValue("فروش")]
        public string adverCondition { get; set; }
        public string adverDistrict { get; set; }
    }
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
        public List<FreeAdvertise> getAdvertises(int p = 0)
        {
            List<FreeAdvertise> all_advers;
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    if(p == 0)
                        all_advers = (from AD in context.FreeAdvertises
                                      where AD.expire_date >= DateTime.Now & AD.approved == true
                                      orderby AD.expire_date descending
                                      select AD).ToList();
                    else
                        all_advers = (from AD in context.FreeAdvertises
                                      where AD.expire_date >= DateTime.Now & AD.approved == true
                                      orderby AD.expire_date descending
                                      select AD).Take(p * 9).ToList();
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
                    if (flag)
                        addver.create_date = DateTime.Now;
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
        public List<FreeAdvertise> AdverSearch(AdverSearchParams searchParams, int take = 0)
        {
            List<FreeAdvertise> _advers = null;
            IQueryable<FreeAdvertise> query;
            using (var context = new AmirhomeEntities())
            {
                query = from A in context.FreeAdvertises 
                        where A.approved == true && A.expire_date > DateTime.Now && A.area >= searchParams.minArea && A.area <= searchParams.maxArea && A.condition.Equals(searchParams.adverCondition)
                        orderby A.expire_date descending select A;
                if (!string.IsNullOrEmpty(searchParams.adverDistrict))
                    query = query.Where(A => A.district.Equals(searchParams.adverDistrict));
                if (searchParams.adverCondition.Equals("فروش"))
                {
                    query = query.Where(A => A.price_total >= searchParams.minTotalPrice
                                        && A.price_total <= searchParams.maxTotalPrice
                                        && A.price_per_meter >= searchParams.minPricePerMeter
                                        && A.price_per_meter <= searchParams.maxPricePerMeter);
                }
                else if (searchParams.adverCondition.Equals("رهن"))
                {
                    query = query.Where(A => A.price_prepayment >= searchParams.minPricePrepayment
                                        && A.price_prepayment <= searchParams.maxPricePrepayment);
                }
                else
                {
                    query = query.Where(A => A.price_prepayment >= searchParams.minPricePrepayment
                                        && A.price_prepayment <= searchParams.maxPricePrepayment
                                        && A.price_mortage >= searchParams.minPriceMortage
                                        && A.price_mortage <= searchParams.maxPriceMortage);
                }
                _advers = take == 0 ? (query.ToList()) : (query.Take(take).ToList());
            }
            return _advers;
        }
    }
}