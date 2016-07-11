using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amirhome.Models
{
    public class SearchParams
    {
        public bool EstateCondition { get; set; }
        public string EstateConditionValue { get; set; }
        public bool EstateUsage { get; set; }
        public string EstateUsageValue { get; set; }
        public bool EstateType { get; set; }
        public string EstateTypeValue { get; set; }
        public bool EstateProvince { get; set; }
        public int EstateProvinceValue { get; set; }
        public bool EstateCity { get; set; }
        public string EstateCityValue { get; set; }
        public bool EstateRegion { get; set; }
        public string EstateRegionValue { get; set; }
        public bool EstateDistrict { get; set; }
        public int EstateDistrictValue { get; set; }
        public bool TotalPrice { get; set; }
        public long TotalPriceFrom { get; set; }
        public long TotalPriceTo { get; set; }
        public bool MortagePrice { get; set; }
        public long MortagePriceFrom { get; set; }
        public long MortagePriceTo { get; set; }
        public bool PrepaymentPrice { get; set; }
        public long PrepaymentPriceFrom { get; set; }
        public long PrepaymentPriceTo { get; set; }
        public bool Loan { get; set; }
        public long LoanFrom { get; set; }
        public long LoanTo { get; set; }
        public bool PricePerMeter { get; set; }
        public long PricePerMeterFrom { get; set; }
        public long PricePerMeterTo { get; set; }
        public bool Area { get; set; }
        public long AreaFrom { get; set; }
        public long AreaTo { get; set; }
        public bool EstateSerial { get; set; }
        public int EstateSerialValue { get; set; }
        public bool EstateAge { get; set; }
        public int EstateAgeValue { get; set; }
        public bool EstateBedrooms { get; set; }
        public string EstateBedroomsValue { get; set; }
        public bool EstateParkings { get; set; }
        public string EstateParkingsValue { get; set; }
        public bool EstateBathrooms { get; set; }
        public string EstateBathroomsValue { get; set; }
        public bool EstatePosition { get; set; }
        public string EstatePositionValue { get; set; }
        public bool EstateFloor { get; set; }
        public string EstateFloorValue { get; set; }
        public bool EstateFloors { get; set; }
        public string EstateFloorsValue { get; set; }
        public bool EstateUnits { get; set; }
        public string EstateUnitsValue { get; set; }
        public bool EstateTells { get; set; }
        public string EstateTellsValue { get; set; }

    }
    
    public class EstateManager
    {
        public State getStateByID(int id)
        {
            State _state = null;
            using (var context = new AmirhomeEntities())
            {
                IEnumerable<State> data = (from E in context.States.Include("Images").Include("Features")
                                           .Include("Owner").Include("Plans").Include("GoogleMaps")
                                           .Include("StreetViews").Include("StateType1").Include("District1")
                          where E.ID == id
                          select E);
                _state = data.FirstOrDefault();
            }
            return _state;
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
        public List<State> doSearch(SearchParams _params, int take = 0)
        {
            List<State> _states = null;
            IQueryable<State> query;
            using (var context = new AmirhomeEntities())
            {
                query = this.EstateQueryBuilder(context, _params);
                _states = take == 0 ? query.ToList() : query.Take(take).ToList();
            }
            return _states;
        }
        private IQueryable<State> EstateQueryBuilder(AmirhomeEntities context, SearchParams _params)
        {
            IQueryable<State> query = from E in context.States select E;
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
            if (_params.EstateUsage)
                query = query.Where(E => E.Usage == _params.EstateUsageValue);
            if (_params.EstateUsage)
                query = query.Where(E => E.Usage == _params.EstateUsageValue);
            if (_params.EstateUsage)
                query = query.Where(E => E.Usage == _params.EstateUsageValue);
            if (_params.EstateUsage)
                query = query.Where(E => E.Usage == _params.EstateUsageValue);
            if (_params.EstateProvince)
                query = query.Where(E => E.Province == _params.EstateProvinceValue);
            if (_params.EstateCity)
                query = query.Where(E => E.City == _params.EstateCityValue);
            if (_params.EstateRegion)
                query = query.Where(E => E.Region == _params.EstateRegionValue);
            if (_params.EstateSerial)
                query = query.Where(E => E.Serial == _params.EstateSerialValue);
            if (_params.EstateAge)
                query = query.Where(E => E.Age == _params.EstateAgeValue);
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
            return query;
        }
        public string SplitInParts(String text, Int32 size)
        {
            List<String> ret = new List<String>(((text.Length + size - 1) / size) + 1);
            if ((text.Length + size) % size != 0)
                ret.Add(text.Substring(0, (text.Length + size) % size));

            for (int start = (text.Length + size) % size; start < text.Length; start += size)
            {
                if ((start + size) <= text.Length)
                    ret.Add(text.Substring(start, size));
                else
                    ret.Add(text.Substring(start, (text.Length + size) % size));
            }
            string result = String.Join(",", ret);
            return turnToPersianNumber(result);
        }

        public string turnToPersianNumber(string str)
        {
            char[] englishNumbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            char[] persianNumbers = { '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹', '۰' };
            for (int i = 0; i < 10; i++)
            {
                str = str.Replace(englishNumbers[i], persianNumbers[i]);
            }
            return str;
        }
    }
}