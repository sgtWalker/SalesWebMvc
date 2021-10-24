using System;
using System.Linq;
using System.Collections.Generic;

namespace SalesWebMvc.Models
{
    public class Department
    {
        #region properties
        public int Id { get; set; }
        public string Name { get; set; }
        //associação (Department possui vários Sellers)
        public ICollection<Seller> Sellers { get; set; } = new List<Seller>();
        #endregion

        #region constructors
        public Department()
        {

        }

        public Department(int id, string name)
        {
            Id = id;
            Name = name;
        }
        #endregion

        #region methods
        public void AddSeller(Seller seller)
        {
            Sellers.Add(seller);
        }

        public double TotalSales(DateTime initial, DateTime final)
        {
            return Sellers.Sum(x => x.TotalSales(initial, final));
        }
        #endregion
    }
}
