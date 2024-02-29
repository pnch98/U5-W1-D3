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
                string query = "SELECT * FROM Prodotti WHERE show = 'true'";

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
                        reader["sideImg2"].ToString(),
                        Convert.ToBoolean(reader["show"])
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
        [HttpPost]
        public ActionResult Login(Utente utente)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                string query = "SELECT * FROM Utenti WHERE username = @username AND password = @password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", utente.Username);
                cmd.Parameters.AddWithValue("@password", utente.Password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Utente user = new Utente(reader["username"].ToString(), reader["password"].ToString(), Convert.ToBoolean(reader["isAdmin"]));
                    Session["user"] = user;
                }
                else
                {
                    Session["user"] = null;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally { conn.Close(); }

            return RedirectToAction("Index");
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
                    reader["sideImg2"].ToString(),
                    Convert.ToBoolean(reader["show"])
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

        public ActionResult Hide(int? id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                string query = "UPDATE Prodotti SET show = 'false' WHERE idScarpa = '" + id + "'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally { conn.Close(); }

            return RedirectToAction("Index");
        }
    }
}