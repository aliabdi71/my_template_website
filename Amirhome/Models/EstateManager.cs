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
        public string EstateUsageVale { get; set; }
        public bool EstateType { get; set; }
        public string EstateTypeValue { get; set; }
        public bool EstateProvince { get; set; }
        public int EstateProvinceValue { get; set; }
        public bool EstateCity { get; set; }
        public string EstateCityValue { get; set; }
        public bool EstateRegion { get; set; }
        public string EstateRegionValue { get; set; }
        public bool EstateDirstict { get; set; }
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
        public long LoadFrom { get; set; }
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
        public int EstateBedroomsValue { get; set; }
        public bool EstateParkings { get; set; }
        public int EstateParkingsValue { get; set; }
        public bool EstateBathrooms { get; set; }
        public int EstateBathroomsValue { get; set; }
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
        public List<State> getOccasions(bool last_five)
        {
            List<State> _states = null;
            using (var context = new AmirhomeEntities())
            {
                
                _states =  (from E in context.States
                           where E.OccasionFlag == true
                           select E).OrderBy(es => es.Date).ToList();
                if (last_five)
                    _states = _states.Skip(Math.Max(0, _states.Count - 5)).ToList();
            }
            return _states;
        }
        public List<State> doSearch(List<Tuple<string, string, string>> _params)
        {
            List<State> _states = null;
            IQueryable<State> query;
            using (var context = new AmirhomeEntities())
            {
                query = from S in context.States select S;
                foreach (var item in _params)
                {

                }
            }

            return _states;
        }
    }
}