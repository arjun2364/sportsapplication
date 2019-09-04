using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsApplication.Models
{
    public class ViewResultModel
    {
        public int TestId { get; set; }
        public DateTime Date { get; set; }
        public String TestType { get; set; }
        public List<AtheleteNameWithData> AtheleteNames { get; set; }
    }
    public class AtheleteNameWithData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Data { get; set; }
        public String FitnessRanking { get; set; }
    }
}
