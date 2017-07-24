using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Models
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqliteConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        /*
            GET /customers?q=test
            GET /customers?_include=payments
         */
        [HttpGet]
        public async Task<IActionResult> Get(string q, string _include)
        {
            string sql = "";

            if (_include != null && _include.Contains("payments"))
            {

            }

            if (q != null)
            {

            }

            Console.WriteLine(sql);

            using (IDbConnection conn = Connection)
            {
                if (_include == "payments")
                {
                    Dictionary<int, Customer> studentExercises = new Dictionary<int, Customer>();

                    var customers = await conn.QueryAsync<Customer, PaymentType, Customer>(
                        sql,
                        (customer, paymentType) =>
                        {
                            return customer;
                        }
                    );
                    return Ok(customers.Values);

                }
                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers);
            }
        }

        // GET /customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = $"SELECT Id, Name, Language FROM Customer WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Customer> customers = await conn.QueryAsync<Customer>(sql);
                return Ok(customers);
            }
        }

        // POST /customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            string sql = $@"INSERT INTO Customer
            ()
            VALUES
            ();
            select MAX(Id) from Customer;";

            using (IDbConnection conn = Connection)
            {
                var customerId = (await conn.QueryAsync<int>(sql)).Single();
                customer.Id = customerId;
                return CreatedAtRoute("GetCustomer", new { id = customerId }, customer);
            }
        }

        /*
            PUT /customers/5

            The [HttpPut] attribute ensures that this method will handle any
            request to a `/customers/{id}` with the PUT HTTP verb. Alternatively,
            I could name this method `PutCustomer`, or just `Put` and ASP.NET
            will detect that the word `Put` is in the method name and ensure
            that it will only be invoke for PUT operations.

            All other controllers have this method named as `Put`. It's named
            differently here to show that the [HttpPut] attribute enforces which
            verb is handled.
         */
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeCustomer(int id, [FromBody] Customer customer)
        {
            string sql = $@"
            UPDATE Customer
            SET '
            WHERE Id = {id}";

            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE /customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Customer WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                throw new Exception("No rows affected");
            }

        }

        private bool CustomerExists(int id)
        {
            string sql = $"SELECT Id, Name, Language FROM Customer WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Customer>(sql).Count() > 0;
            }
        }
    }
}
