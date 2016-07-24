using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Entity;

namespace Amirhome.Models
{
    public class SearchParams
    {
        [DefaultValue(false)]
        public bool EstateCondition { get; set; }
        public string EstateConditionValue { get; set; }
        [DefaultValue(false)]
        public bool EstateUsage { get; set; }
        public string EstateUsageValue { get; set; }
        [DefaultValue(false)]
        public bool EstateType { get; set; }
        public string EstateTypeValue { get; set; }
        [DefaultValue(false)]
        public bool EstateProvince { get; set; }
        public int EstateProvinceValue { get; set; }
        [DefaultValue(false)]
        public bool EstateCity { get; set; }
        public string EstateCityValue { get; set; }
        [DefaultValue(false)]
        public bool EstateRegion { get; set; }
        public string EstateRegionValue { get; set; }
        [DefaultValue(false)]
        public bool EstateDistrict { get; set; }
        public int EstateDistrictValue { get; set; }
        [DefaultValue(false)]
        public bool TotalPrice { get; set; }
        [DefaultValue(0)]
        public long TotalPriceFrom { get; set; }
        [DefaultValue(100000000000)]
        public long TotalPriceTo { get; set; }
        [DefaultValue(false)]
        public bool MortagePrice { get; set; }
        [DefaultValue(0)]
        public long MortagePriceFrom { get; set; }
        [DefaultValue(1000000000)]
        public long MortagePriceTo { get; set; }
        [DefaultValue(false)]
        public bool PrepaymentPrice { get; set; }
        [DefaultValue(0)]
        public long PrepaymentPriceFrom { get; set; }
        [DefaultValue(1000000000)]
        public long PrepaymentPriceTo { get; set; }
        [DefaultValue(false)]
        public bool Loan { get; set; }
        [DefaultValue(0)]
        public long LoanFrom { get; set; }
        [DefaultValue(1000000000)]
        public long LoanTo { get; set; }
        [DefaultValue(false)]
        public bool PricePerMeter { get; set; }
        [DefaultValue(0)]
        public long PricePerMeterFrom { get; set; }
        [DefaultValue(1000000000)]
        public long PricePerMeterTo { get; set; }
        [DefaultValue(false)]
        public bool Area { get; set; }
        [DefaultValue(0)]
        public long AreaFrom { get; set; }
        [DefaultValue(1000000)]
        public long AreaTo { get; set; }
        [DefaultValue(false)]
        public bool EstateSerial { get; set; }
        public int EstateSerialValue { get; set; }
        [DefaultValue(false)]
        public bool EstateAge { get; set; }
        public int EstateAgeValue { get; set; }
        [DefaultValue(false)]
        public bool EstateBedrooms { get; set; }
        public string EstateBedroomsValue { get; set; }
        [DefaultValue(false)]
        public bool EstateParkings { get; set; }
        public string EstateParkingsValue { get; set; }
        [DefaultValue(false)]
        public bool EstateBathrooms { get; set; }
        public string EstateBathroomsValue { get; set; }
        [DefaultValue(false)]
        public bool EstatePosition { get; set; }
        public string EstatePositionValue { get; set; }
        [DefaultValue(false)]
        public bool EstateFloor { get; set; }
        public string EstateFloorValue { get; set; }
        [DefaultValue(false)]
        public bool EstateFloors { get; set; }
        public string EstateFloorsValue { get; set; }
        [DefaultValue(false)]
        public bool EstateUnits { get; set; }
        public string EstateUnitsValue { get; set; }
        [DefaultValue(false)]
        public bool EstateTells { get; set; }
        public string EstateTellsValue { get; set; }
        [DefaultValue(false)]
        public bool HasImage { get; set; }
        public bool HasParking { get; set; }
    }
    
    public class EstateManager
    {
        public int generateSerial()
        {
            int[] currentSerials = null;
            using (var context = new AmirhomeEntities())
            {
                currentSerials = (from S in context.States
                                  select S.Serial).ToArray();
            }
            Random rand = new Random();
            int res = rand.Next(1000, 1000000);
            while(currentSerials.Contains(res))
                res = rand.Next(1000, 1000000);
            return res;
        }
        public List<Feature> fetchFeaturesById(int[] ids)
        {
            List<Feature> _feature = null;
            using (var context = new AmirhomeEntities())
            {

                _feature = (from F in context.Features
                            where ids.Contains(F.ItemID)
                            select F).ToList();
            }
            return _feature;

        }
        public bool addNewEstate(State model)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    context.States.Add(model);
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Feature> getAllFeatures()
        {
            List<Feature> _feature = null;
            using (var context = new AmirhomeEntities())
            {

                _feature = (from F in context.Features
                           select F).ToList();
            }
            return _feature;
        }
        public List<province> getAllProvince()
        {
            List<province> _province = null;
            using (var context = new AmirhomeEntities())
            {

                _province = (from P in context.provinces
                             where P.id > 0
                            select P).OrderBy(p => p.name).ToList();
            }
            return _province;
        }
        public List<tbl_cities> getCityByProvince(int province_id)
        {
            List<tbl_cities> _cities = null;
            using (var context = new AmirhomeEntities())
            {

                _cities = (from C in context.tbl_cities
                           where C.provinceId == province_id
                           select C).OrderBy(c => c.name).ToList();
            }
            return _cities;
        }
        public List<District> getAllDistricts()
        {
            List<District> _districts = null;
            using (var context = new AmirhomeEntities())
            {

                _districts = (from D in context.Districts
                              where D.ID > 0
                           select D).OrderBy(d => d.name).ToList();
            }
            return _districts;
        }
        public State getStateByID(int id)
        {
            State _state = null;
            using (var context = new AmirhomeEntities())
            {
                IEnumerable<State> data = (from E in context.States.Include("Images").Include("Features")
                                           .Include("Owner").Include("Plans").Include("GoogleMaps")
                                           .Include("StreetViews").Include("StateType1").Include("District1")
                                           .Include("Feedbacks").Include("province1")
                          where E.ID == id
                          select E);
                _state = data.FirstOrDefault();
            }
            return _state;
        }
        public List<State> getAllStates()
        {
            List<State> _estates = null;
            using (var context = new AmirhomeEntities())
            {
                _estates = (from E in context.States.Include("District1").Include("StateType1")
                            select E).ToList();
            }
            return _estates;
        }
        public bool approveEstate(int id, bool flag)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    State est = (from E in context.States
                                 where E.ID == id
                                 select E).First();
                    est.Approved = flag;
                    context.States.Attach(est);
                    context.Entry(est).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        public bool archiveEstate(int id, bool flag)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    State est = (from E in context.States
                                 where E.ID == id
                                 select E).First();
                    est.Archived = flag;
                    bool res = updateEstate(est);
                    return res;
                }
            }
            catch
            {
                return false;
            }

        }
        public bool updateEstate(State model)
        {
            try
            {
                using (var context = new AmirhomeEntities())
                {
                    context.States.Attach(model);
                    context.Entry(model).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public List<State> getOccasions()
        {
            List<State> _states = null;
            using (var context = new AmirhomeEntities())
            {

                _states = (from E in context.States.Include("Images").Include("District1")
                           where E.OccasionFlag == true
                           select E).ToList();
            }
            return _states;
        }
        public List<State> doSearch(SearchParams _params, int take = 0, string order = "date")
        {
            List<State> _states = null;
            IQueryable<State> query;
            using (var context = new AmirhomeEntities())
            {
                query = this.EstateQueryBuilder(context, _params);
                switch (order)
                {
                    case "date": query = query.OrderByDescending(e => e.Date); break;
                    case "price_1": query = query.OrderBy(e => e.TotalPrice).OrderBy(e => e.PrepaymentPrice); break;
                    case "price_2": query = query.OrderBy(e => e.PricePerMeter).OrderBy(e => e.MortgagePrice); break;
                    case "price_1_desc": query = query.OrderByDescending(e => e.TotalPrice).OrderByDescending(e => e.PrepaymentPrice); break;
                    case "price_2_desc": query = query.OrderByDescending(e => e.PricePerMeter).OrderByDescending(e => e.MortgagePrice); break;
                    case "area": query = query.OrderBy(e => e.Area); break;
                    case "area_desc": query = query.OrderByDescending(e => e.Area); break;
                    default: break;
                }
                _states = take == 0 ? query.ToList() : query.Take(take).ToList();
            }
            return _states;
        }
        private IQueryable<State> EstateQueryBuilder(AmirhomeEntities context, SearchParams _params)
        {
            IQueryable<State> query = from E in context.States.Include("Images").Include("District1").Include("StateType1") select E;
            query = query.Where(E => E.Approved == true);
            if (_params.EstateCondition)
                query = query.Where(E => E.Condition == _params.EstateConditionValue);
            if (_params.EstateDistrict)
                query = query.Where(E => E.District == _params.EstateDistrictValue);
            if (_params.EstateFloor)
                query = query.Where(E => E.Floor == _params.EstateFloorValue);
            if (_params.EstateFloors)
                query = query.Where(E => E.Floors == _params.EstateFloorsValue);
            if (_params.EstateUsage)
                query = query.Where(E => E.Usage == _params.EstateUsageValue);
            if (_params.EstateType)
                query = query.Where(E => E.StateType == _params.EstateTypeValue);
            if (_params.EstateProvince)
                query = query.Where(E => E.Province == _params.EstateProvinceValue);
            if (_params.EstateCity)
                query = query.Where(E => E.City == _params.EstateCityValue);
            if (_params.EstateRegion)
                query = query.Where(E => E.Region == _params.EstateRegionValue);
            if (_params.EstateSerial)
                query = query.Where(E => E.Serial == _params.EstateSerialValue);
            if (_params.EstateAge)
                query = query.Where(E => E.Age <= _params.EstateAgeValue);
            if (_params.EstateBedrooms)
                query = query.Where(E => E.Bedrooms == _params.EstateBedroomsValue);
            if (_params.EstateBathrooms)
                query = query.Where(E => E.Bathrooms == _params.EstateBathroomsValue);
            if (_params.EstateParkings)
                query = query.Where(E => E.Parking == _params.EstateParkingsValue);
            if (_params.EstatePosition)
                query = query.Where(E => E.StatePosition == _params.EstatePositionValue);
            if (_params.EstateUnits)
                query = query.Where(E => E.Units == _params.EstateUnitsValue);
            if (_params.EstateTells)
                query = query.Where(E => E.Tells == _params.EstateTellsValue);
            if (_params.TotalPrice)
                query = query.Where(E => E.TotalPrice >= _params.TotalPriceFrom && E.TotalPrice <= _params.TotalPriceTo);
            if (_params.MortagePrice)
                query = query.Where(E => E.MortgagePrice >= _params.MortagePriceFrom && E.MortgagePrice <= _params.MortagePriceTo);
            if (_params.PrepaymentPrice)
                query = query.Where(E => E.PrepaymentPrice >= _params.PrepaymentPriceFrom && E.PrepaymentPrice <= _params.PrepaymentPriceTo);
            if (_params.Loan)
                query = query.Where(E => E.Loan >= _params.LoanFrom && E.TotalPrice <= _params.LoanTo);
            if (_params.PricePerMeter)
                query = query.Where(E => E.PricePerMeter >= _params.PricePerMeterFrom && E.PricePerMeter <= _params.PricePerMeterTo);
            if (_params.Area)
                query = query.Where(E => E.Area >= _params.AreaFrom && E.Area <= _params.AreaTo);
            if (_params.HasImage)
                query = query.Where(E => E.Images.Count > 0);
            if (_params.HasParking)
                query = query.Where(E => E.Features.Any(F => F.ItemID == 15));
            return query;
        }

    }

    public class OccasionViewModel
    {
        public int ID { get; set; }
        public string Condition { get; set; }
        public long Area { get; set; }

    }
}