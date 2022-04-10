using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TutorialSubirArchivos.Models;

namespace TutorialSubirArchivos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Descargar()
        {

            return View();
        }

        [HttpPost]
        public JsonResult InsertarArchivos(HttpPostedFileBase[] archivos)
        {
            Respuesta_Json respuesta = new Respuesta_Json();

            try
            {
                for (int i = 0; i < archivos.Length; i++)
                {
                    Archivos archivo = new Archivos();

                    archivo.Fecha_Entrada = DateTime.Now;
                    archivo.Formato = MimeMapping.GetMimeMapping(archivos[i].FileName);
                    archivo.Extension = Path.GetExtension(archivos[i].FileName);
                    archivo.Nombre_Archivo = Path.GetFileNameWithoutExtension(archivos[i].FileName);

                    double tamanio = archivos[i].ContentLength;
                    tamanio = tamanio / 1000000.0;
                    archivo.Tamanio = Math.Round(tamanio, 2);

                    Stream fs = archivos[i].InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    archivo.Archivo = br.ReadBytes((Int32)fs.Length);

                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Model1"].ConnectionString))
                    {
                        connection.Open();
                        string sql = "insert into Archivos(Nombre_Archivo, Extension, Formato, Fecha_Entrada, Archivo, Tamanio) values(" +
                            " @nombreArchivo, @extension, @formato, @fechaEntrada, @archivo, @tamanio)";
                        using (SqlCommand cmd = new SqlCommand(sql, connection))
                        {
                            cmd.Parameters.Add("@nombreArchivo", SqlDbType.VarChar, 100).Value = archivo.Nombre_Archivo;
                            cmd.Parameters.Add("@extension", SqlDbType.VarChar, 5).Value = archivo.Extension;
                            cmd.Parameters.Add("@formato", SqlDbType.VarChar, 200).Value = archivo.Formato;
                            cmd.Parameters.Add("@fechaEntrada", SqlDbType.DateTime).Value = archivo.Fecha_Entrada;
                            cmd.Parameters.Add("@archivo", SqlDbType.Image).Value = archivo.Archivo;
                            cmd.Parameters.Add("@tamanio", SqlDbType.Float).Value = archivo.Tamanio;
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                        connection.Close();
                    }

                }

                respuesta.Codigo = 1;
                respuesta.Mensaje_Respuesta = "Se insertaron correctamente los archivos en la base de datos";
            }
            catch(Exception ex)
            {
                respuesta.Codigo = 0;
                respuesta.Mensaje_Respuesta = ex.ToString();
            }

            return Json(respuesta);
        }

        [HttpGet]
        public string ConvertToBase64()
        {
            string connStr = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
            string consulta = "select top 1 Archivo from Archivos";
            DataTable dt = new DataTable();

            using (SqlConnection cnn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, cnn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 99999999;
                    cnn.Open();
                    dt.Load(cmd.ExecuteReader());
                    cnn.Close();
                    cnn.Dispose();
                }
            }

            byte[] auxiliar = (byte[])dt.Rows[0][0];

            return Convert.ToBase64String(auxiliar);

        }

        [HttpGet]
        public JsonResult ConsultarArchivos()
        {
            Respuesta_Json respuesta = new Respuesta_Json();
            DataTable dt = new DataTable();

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
                string consulta = "select Id, Nombre_Archivo, Extension, Formato, Fecha_Entrada, Tamanio from Archivos";

                using (SqlConnection cnn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, cnn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cnn.Open();
                        dt.Load(cmd.ExecuteReader());
                        cnn.Close();
                        cnn.Dispose();
                    }
                }
                respuesta.Codigo = 1;
                respuesta.Mensaje_Respuesta = "Se realizó la consulta a la base de datos correctamente";
                respuesta.Archivos = DataTableToDictionary(dt);
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 0;
                respuesta.Mensaje_Respuesta = ex.ToString();
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


        public List<Dictionary<string, object>> DataTableToDictionary(DataTable dt)
        {
            List<Dictionary<string, object>> filas = new List<Dictionary<string, object>>();
            Dictionary<string, object> fila;
            foreach (DataRow row in dt.Rows)
            {
                fila = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    fila.Add(col.ColumnName, row[col]);
                }
                filas.Add(fila);
            }

            return filas;
        }



        [HttpGet]
        public JsonResult ObtenerArchivo(int id)
        {
            Respuesta_Json respuesta = new Respuesta_Json();

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["Model1"].ConnectionString;
                string consulta = "select Archivo, Tamanio from Archivos where Id=" + id;
                DataTable dt = new DataTable();

                using (SqlConnection cnn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(consulta, cnn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cnn.Open();
                        dt.Load(cmd.ExecuteReader());
                        cnn.Close();
                        cnn.Dispose();
                    }
                }

                respuesta.Codigo = 1;
                respuesta.Mensaje_Respuesta = "Se realizó la consulta a la base de datos correctamente";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 0;
                respuesta.Mensaje_Respuesta = ex.ToString();
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }



    }
}


/*
 
                IEnumerable<Archivos> enumerable = dt.AsEnumerable().Select(fila => new Archivos
                {
                    Id = Convert.ToInt32(fila["Id"]),
                    Nombre_Archivo = fila["Nombre_Archivo"].ToString(),
                    Extension = fila["Extension"].ToString(),
                    Formato = fila["Formato"].ToString(),
                    Fecha_Entrada = Convert.ToDateTime(fila["Fecha_Entrada"])
                });
     */
