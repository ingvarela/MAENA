using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using maena_se.Helpers;
using maena_se.Models;
using System.Security.Cryptography;
using System.Text;

namespace maena_se.Controllers
{
    public class HomeController : Controller
    {

        //Login
        // GET: Login Page
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string query = "SELECT id_usuario, username FROM Usuario WHERE username = @username AND contrasena = @password";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@username", model.Username),
                    new SqlParameter("@password", model.Password) //HashPassword(model.Password)    // Hashing Password
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    Session["UserID"] = dt.Rows[0]["id_usuario"];
                    Session["UserName"] = dt.Rows[0]["username"];

                    return RedirectToAction("Dashboard");
                    //return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos.";
                }
            }
            return View(model);
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Registration Page
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string query = "INSERT INTO Usuario (fecha_registro, username, correo_e, contrasena, rol) " +
                               "VALUES (@fecha_registro,@username,@correo_e,@contrasena,@tipo_usuario)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@fecha_registro", DateTime.Now),
                    new SqlParameter("@username", model.Username),
                    new SqlParameter("@correo_e", model.Email),
                    new SqlParameter("@contrasena", model.Password),
                    new SqlParameter("@tipo_usuario", model.Role),  //HashPassword()
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    //mensaje de sucess
                    ViewBag.Success = "¡Usuario Registrado con éxito!";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Hubo un problema al registrar el usuario.";
                }
            }
            return View(model);
        }

        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // Helper Function to Hash Passwords
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }


        //Login


        public ActionResult Index()
        {
            return View();
        }

        // Get all appointments for FullCalendar
        public ActionResult GetAppointments()
        {
            string query =  @"
        SELECT cm.id_cita, 
               'Cita con terapeuta ' + 'Psi. Esmeralda' AS title, 
               cm.fecha_hora AS start, 
               p.id_paciente, 
               p.nombre + ' ' + p.apellido_paterno + ' ' + p.apellido_materno AS paciente_nombre
        FROM Cita_Medica cm
        JOIN Paciente p ON cm.id_paciente = p.id_paciente;"; ;
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            var events = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                events.Add(new
                {
                    id = row["id_cita"],
                    title = row["title"],
                    start = Convert.ToDateTime(row["start"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                    pacienteId = row["id_paciente"],
                    pacienteNombre = row["paciente_nombre"]
                });
            }

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        // Add a new appointment
        [HttpPost]
        public ActionResult AddAppointment(int pacienteId, int terapeutaId, int consultorioId, DateTime fechaHora)
        {
            pacienteId = 3;
            terapeutaId = 1;
            consultorioId = 1;

            string query = "INSERT INTO Cita_Medica (id_paciente, id_terapeuta, id_consultorio, fecha_hora, Status) VALUES (@pacienteId, @terapeutaId, @consultorioId, @fechaHora, 'Scheduled')";

            SqlParameter[] parameters =
            {
                new SqlParameter("@pacienteId", pacienteId),
                new SqlParameter("@terapeutaId", terapeutaId),
                new SqlParameter("@consultorioId", consultorioId),
                new SqlParameter("@fechaHora", fechaHora)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return Json(new { success = rowsAffected > 0 });
        }

        // Update an existing appointment
        [HttpPost]
        public ActionResult UpdateAppointment(int id, DateTime fechaHora)
        {
            string query = "UPDATE Cita_Medica SET fecha_hora = @fechaHora WHERE id_cita = @id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id),
                new SqlParameter("@fechaHora", fechaHora)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return Json(new { success = rowsAffected > 0 });
        }

        // Delete an appointment
        [HttpPost]
        public ActionResult DeleteAppointment(int id)
        {
            string query = "DELETE FROM Cita_Medica WHERE id_cita = @id";

            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return Json(new { success = rowsAffected > 0 });
        }



        // Display Pacientes View
        public ActionResult GestionPacientes()
        {
            return View();
        }

        // Fetch all Pacientes for DataTable
        public ActionResult GetPacientes()
        {
            string query = "SELECT id_paciente, nombre, apellido_paterno, apellido_materno, fecha_nacimiento, condicion_medica, motivo_consulta, escuela_procedencia, contacto_tel FROM Paciente";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            var pacientes = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                pacientes.Add(new
                {
                    id = row["id_paciente"],
                    nombre = row["nombre"],
                    apellido_paterno = row["apellido_paterno"],
                    apellido_materno = row["apellido_materno"],
                    fecha_nacimiento = Convert.ToDateTime(row["fecha_nacimiento"]).ToString("yyyy-MM-dd"),
                    condicion_medica = row["condicion_medica"],
                    motivo_consulta = row["motivo_consulta"],
                    escuela_procedencia = row["escuela_procedencia"],
                    contacto_tel = row["contacto_tel"],
                });
            }

            return Json(new { data = pacientes }, JsonRequestBehavior.AllowGet);
        }


        // Fetch all Pacientes for DataTable
        public ActionResult GetPacientesCitas()
        {
            string query = "SELECT id_paciente, nombre, apellido_paterno, apellido_materno FROM Paciente";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            var pacientes = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                pacientes.Add(new
                {
                    id = row["id_paciente"],
                    nombre = row["nombre"] + " " + row["apellido_paterno"] + " " + row["apellido_materno"]
                });
            }

            return Json(new { data = pacientes }, JsonRequestBehavior.AllowGet);
        }




        // Add a new Paciente
        [HttpPost]
        public ActionResult AddPaciente(Paciente model)
        {
            //if (ModelState.IsValid)
            //{
                string query = "INSERT INTO Paciente (nombre, apellido_paterno, apellido_materno, fecha_ingreso, fecha_nacimiento, tipo, genero, motivo_consulta, escuela_procedencia, contacto_tel,contacto_correo, id_tutor, id_terapeuta) " +
                               "VALUES (@nombre, @apellido_paterno, @apellido_materno, @fecha_ingreso, @fecha_nacimiento, @tipo, @genero, @motivo_consulta, @escuela_procedencia, @contacto_tel, @contacto_correo, @id_tutor, @id_terapeuta)";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@nombre", model.Nombre),
                    new SqlParameter("@apellido_paterno", model.ApellidoPaterno),
                    new SqlParameter("@apellido_materno", model.ApellidoMaterno),
                    new SqlParameter("@fecha_nacimiento", model.FechaNacimiento),
                    new SqlParameter("@fecha_ingreso", DateTime.Now),
                    new SqlParameter("@tipo", 1),
                    new SqlParameter("@genero", 'M'),
                    new SqlParameter("@motivo_consulta", model.MotivoConsulta),
                    new SqlParameter("@escuela_procedencia", model.EscuelaProcedencia),
                    new SqlParameter("@contacto_tel", model.ContactoTel),
                    new SqlParameter("@contacto_correo", "N/A"),
                    new SqlParameter("@id_tutor", 1),
                    new SqlParameter("@id_terapeuta", 2)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return Json(new { success = rowsAffected > 0 });
            //}

           // return Json(new { success = false });
        }

        // Update an existing Paciente
        [HttpPost]
        public ActionResult EditPaciente(Paciente model)
        {
            if (ModelState.IsValid)
            {
                string query = "UPDATE Paciente SET nombre=@nombre, apellido_paterno=@apellido_paterno, " +
                               "apellido_materno=@apellido_materno, fecha_nacimiento=@fecha_nacimiento,fecha_ingreso=@fecha_ingreso," +
                               "genero=@genero, @motivo_consulta=@motivo_consulta, escuela_procedencia=@escuela_procedencia," +
                               "contacto_tel=@contacto_tel, id_tutor=@id_tutor, id_terapeuta=@id_terapeuta " +
                               "WHERE id_paciente=@id";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@nombre", model.Nombre),
                    new SqlParameter("@apellido_paterno", model.ApellidoPaterno),
                    new SqlParameter("@apellido_materno", model.ApellidoMaterno),
                    new SqlParameter("@fecha_nacimiento", model.FechaNacimiento),
                    new SqlParameter("@fecha_ingreso", DateTime.Now),
                    new SqlParameter("@tipo", 1),
                    new SqlParameter("@genero", 'M'),
                    new SqlParameter("@motivo_consulta", model.MotivoConsulta),
                    new SqlParameter("@escuela_procedencia", model.EscuelaProcedencia),
                    new SqlParameter("@contacto_tel", model.ContactoTel),
                    new SqlParameter("@id_tutor", 1),
                    new SqlParameter("@id_terapeuta", 2)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return Json(new { success = rowsAffected > 0 });
            }

            return Json(new { success = false });
        }

        // Delete a Paciente
        [HttpPost]
        public ActionResult DeletePaciente(int id)
        {
            string query = "DELETE FROM Paciente WHERE id_paciente = @id";
            SqlParameter[] parameters =
            {
                new SqlParameter("@id", id)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return Json(new { success = rowsAffected > 0 });
        }

        public ActionResult GetUsuarios()
        {
            string query = "SELECT id_usuario, username, correo_e, contrasena, fecha_registro, rol FROM Usuario where id_usuario > 1";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            var pacientes = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                pacientes.Add(new
                {
                    id = row["id_usuario"],
                    username = row["username"],
                    correo_e = row["correo_e"],
                    contrasena = row["contrasena"],
                    fecha_registro = Convert.ToDateTime(row["fecha_registro"]).ToString("yyyy-MM-dd"),
                    rol = row["rol"]
                });
            }

            return Json(new { data = pacientes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GestionUsuarios()
        {
            return View();
        }

    }

}

