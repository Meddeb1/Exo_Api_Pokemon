using Exo_Api_Pokemon.Models;
using Exo_Api_Pokemon.Models.ModelView;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Exo_Api_Pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        const string CONNEXION_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Api_Pokemon;Integrated Security=True;";
        // GET: api/<Pokemon>
        [HttpGet("GetAll")]
        public IEnumerable<Pokemon> GetAll()
        {
            List<Pokemon> result = new List<Pokemon>();
            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using(SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "Select Id,NomFr,NomEn FROM Pokemon;";
                    dbConnection.Open();

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Pokemon() { Id = (int)reader["Id"],
                                                          NomFr = (string)reader["NomFr"], 
                                                          NomEn = (string)reader["NomEn"] });
                           

                        }
                    }
                }
            }

            return result;
            
        }

        // GET api/<Pokemon>/5
        [HttpGet("GetByChars/{s}")]
        public IActionResult GetByChars(string s)
        {
           
            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using (SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = $"Select Id ,NomFr,NomEn FROM Pokemon Where Nomfr like @Info;";   // like concat('%',"+name+",'%') // sensiblejonction sql

                    //SELECT Id, NameFr, NameUk FROM Pokemon WHERE NameFr LIKE '%' + @name + '%' OR NameUk LIKE '%' + @name + '%'


                    cmd.Parameters.AddWithValue("Info", $"%{s}%");    // pour chercher avec la 1ere ou 2eme lettre

                    dbConnection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            return Ok( new Pokemon() {Id=(int)reader["Id"], NomFr = (string)reader["NomFr"], NomEn = (string)reader["NomEn"] });

                        }

                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {

            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using (SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "Select Id ,NomFr,NomEn FROM Pokemon Where Id=@Id;";
                    cmd.Parameters.AddWithValue("Id", id);

                    dbConnection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            return Ok(new Pokemon() { Id=(int)reader["Id"], NomFr = (string)reader["NomFr"], NomEn = (string)reader["NomEn"] });

                        }

                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }

        // POST api/<Pokemon>
        [HttpPost("Post")]
        public IActionResult Post([FromBody] AddPokemonModelView pok)
        {
            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using (SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "Insert Into Pokemon (NomFr,NomEn) OUTPUT inserted.Id values(@Nomfr,@NomEn)";
                    cmd.Parameters.AddWithValue("NomFr",pok.NomFr );
                    cmd.Parameters.AddWithValue("NomEn", pok.NomEn);

                    dbConnection.Open();

                    int id = (int)cmd.ExecuteScalar();

                    return Created($"https://localhost:7083/api/Pokemon/{id}", null);
                }
            }
        }

        // PUT api/<Pokemon>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateModelView pok)
        {
            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using (SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "Update Pokemon Set NomFr=@NomFr, NomEn =@NomEn where Id=@Id ;";
                    cmd.Parameters.AddWithValue("NomFr", pok.NomFr);
                    cmd.Parameters.AddWithValue("NomEn", pok.NomEn);
                    cmd.Parameters.AddWithValue("Id", id);


                    dbConnection.Open();

                    try
                    {
                        return Ok(cmd.ExecuteNonQuery());
                    }
                    catch (Exception)
                    {
                        return BadRequest(new { Message = "Un probléme est survenue" });
                    }

                    

                   
                }
            }
        }

        // DELETE api/<Pokemon>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (SqlConnection dbConnection = new SqlConnection(CONNEXION_STRING))
            {
                using (SqlCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Pokemon WHERE Id=@Id ;";
                    cmd.Parameters.AddWithValue("Id", id);
 
                    dbConnection.Open();
   
                    try
                    {
                       int nbLigne = cmd.ExecuteNonQuery();   // nb Lignes
                        if(nbLigne == 0)
                        {
                            return BadRequest(new { Message = "rien n 'a été suprimer" });
                        }

                        if (nbLigne > 1)
                        {
                            return BadRequest(new { Message = "trop des lignes ont été supp!!" });
                        }
                        return Ok(nbLigne);
                    }
                    catch (Exception)
                    {
                        return BadRequest(new { Message = "Un probléme est survenue" });
                    }
                }
            }

        }
    }
}
