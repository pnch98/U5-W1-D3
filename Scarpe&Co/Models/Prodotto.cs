using System.ComponentModel.DataAnnotations;

namespace Scarpe_Co.Models
{
    public class Prodotto
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string MainImg { get; set; }
        public string SideImg1 { get; set; }
        public string SideImg2 { get; set; }
        public bool Show { get; set; }
        public Prodotto() { }
        public Prodotto(int id, string name, double price, string mainImg, string description = null, string sideImg1 = null, string sideImg2 = null, bool show = true)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            MainImg = mainImg;
            SideImg1 = sideImg1;
            SideImg2 = sideImg2;
            Show = show;
        }
    }
}