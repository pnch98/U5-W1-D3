using Scarpe_Co.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Scarpe_Co.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Prodotto> prodotti = new List<Prodotto>();
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                string query = "SELECT * FROM Prodotti";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Prodotto product = new Prodotto(
                        Convert.ToInt16(reader["idScarpa"]),
                        reader["nome"].ToString(),
                        Convert.ToDouble(reader["prezzo"]),
                        reader["mainImg"].ToString(),
                        reader["descrizione"].ToString(),
                        reader["sideImg1"].ToString(),
                        reader["sideImg2"].ToString()
                        );
                    prodotti.Add(product);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally { conn.Close(); }

            return View(prodotti);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            Prodotto product = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                string query = "SELECT * FROM Prodotti WHERE idScarpa = '" + id + "'";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    product = new Prodotto(
                    Convert.ToInt16(reader["idScarpa"]),
                    reader["nome"].ToString(),
                    Convert.ToDouble(reader["prezzo"]),
                    reader["mainImg"].ToString(),
                    reader["descrizione"].ToString(),
                    reader["sideImg1"].ToString(),
                    reader["sideImg2"].ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally { conn.Close(); }

            return View(product);
        }
    }
}