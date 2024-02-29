using Scarpe_Co.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Scarpe_Co.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Edit(int? id)
        {
            // Retrieve product from the database
            Prodotto product = GetProductById(id);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Adding anti-forgery token
        public ActionResult Edit(Prodotto prod, HttpPostedFileBase MainImg, HttpPostedFileBase SideImg1, HttpPostedFileBase SideImg2)
        {
            // Save uploaded images and get their paths
            string mainPath = SaveImage(MainImg);
            string side1Path = SaveImage(SideImg1);
            string side2Path = SaveImage(SideImg2);

            // Update product in the database
            UpdateProduct(prod, mainPath, side1Path, side2Path);


            return RedirectToAction("Index", "Home");
        }

        // Helper method to retrieve product by ID
        private Prodotto GetProductById(int? id)
        {
            Prodotto product = null;

            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Prodotti WHERE idScarpa = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
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

            return product;
        }

        // Helper method to save image and return its path
        private string SaveImage(HttpPostedFileBase image)
        {
            if (image != null)
            {
                var fileName = Path.GetFileName(image.FileName);
                var path = Path.Combine("~/Content/imgs", fileName);
                var absolutePath = Server.MapPath(path);
                image.SaveAs(absolutePath);
                return path;
            }
            return null;
        }


        private void UpdateProduct(Prodotto prod, string mainImgPath, string side1ImgPath, string side2ImgPath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Prodotti " +
                    "SET nome = @nome, " +
                    "prezzo = @prezzo, " +
                    "descrizione = @descrizione, " +
                    "mainImg = COALESCE(@mainImg, mainImg), " + // Use COALESCE to retain the previous value if @mainImg is null
                    "sideImg1 = COALESCE(@sideImg1, sideImg1),  " + // Use COALESCE to retain the previous value if @sideImg1 is null
                    "sideImg2 = COALESCE(@sideImg2, sideImg2), " + // Use COALESCE to retain the previous value if @sideImg2 is null
                    "show = @show " +
                    "WHERE idScarpa = @Id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", prod.Id);
                cmd.Parameters.AddWithValue("@nome", prod.Name);
                cmd.Parameters.AddWithValue("@prezzo", prod.Price);
                cmd.Parameters.AddWithValue("@descrizione", prod.Description);
                cmd.Parameters.AddWithValue("@mainImg", (object)mainImgPath ?? DBNull.Value); // Handle null value
                cmd.Parameters.AddWithValue("@sideImg1", (object)side1ImgPath ?? DBNull.Value); // Handle null value
                cmd.Parameters.AddWithValue("@sideImg2", (object)side2ImgPath ?? DBNull.Value); // Handle null value
                cmd.Parameters.AddWithValue("@show", prod.Show);

                cmd.ExecuteNonQuery();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Prodotto prod, HttpPostedFileBase MainImg, HttpPostedFileBase SideImg1, HttpPostedFileBase SideImg2)
        {
            if (ModelState.IsValid)
            {
                string mainPath = SaveImage(MainImg);
                string side1Path = SaveImage(SideImg1);
                string side2Path = SaveImage(SideImg2);

                string connectionString = ConfigurationManager.ConnectionStrings["ScarpeDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Prodotti (nome, prezzo, descrizione, mainImg, show)" +
                        "VALUES (@nome, @prezzo, @descrizione, @mainImg, @show)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nome", prod.Name);
                    cmd.Parameters.AddWithValue("@prezzo", prod.Price);
                    cmd.Parameters.AddWithValue("@descrizione", prod.Description);
                    cmd.Parameters.AddWithValue("@mainImg", mainPath);
                    cmd.Parameters.AddWithValue("@show", prod.Show);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult BackOffice()
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
                    Prodotto product = new Prodotto();
                    product.Id = Convert.ToInt16(reader["idScarpa"]);
                    product.Name = reader["nome"].ToString();
                    product.Price = Convert.ToDouble(reader["prezzo"]);
                    product.MainImg = reader["mainImg"].ToString();
                    product.Description = reader["descrizione"].ToString();
                    if (reader["sideImg1"].ToString() != null)
                        product.SideImg1 = reader["sideImg1"].ToString();
                    else
                        product.SideImg1 = "";
                    if (reader["sideImg2"].ToString() != null)
                        product.SideImg2 = reader["sideImg2"].ToString();
                    else
                        product.SideImg2 = "";
                    product.Show = Convert.ToBoolean(reader["show"]);

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
    }
}
