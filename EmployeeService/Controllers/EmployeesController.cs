using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Models;
using EmployeeService.DTO;
using Microsoft.AspNetCore.Cors;

namespace EmployeeService.Controllers
{
    [EnableCors("AllowAny")]

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public EmployeesController(NorthwindContext context)
        {
            _context = context;
        }

        //1. GET: api/Employees
        [HttpGet]
        public async Task<IEnumerable<EmployeeDTO>> GetEmployees()
        {
            return  _context.Employees.Select(emp=>new EmployeeDTO
            {
                EmployeeId= emp.EmployeeId,
                FirstName=emp.FirstName,
                LastName=emp.LastName,
                Title=emp.Title,

            });
        }

        //***動詞(例如[HttpPost]) 跟Uri(api/Employee)不可以重複!
        //篩選功能
        [HttpPost("Filter")] //Uri:api/Employee/Filter
        public async Task<IEnumerable<EmployeeDTO>> FilterEmployees([FromBody]EmployeeDTO employeeDTO)
        {
            return _context.Employees.Where(emp => emp.FirstName.Contains(employeeDTO.FirstName) || emp.LastName.Contains(employeeDTO.FirstName) || emp.Title.Contains(employeeDTO.FirstName)).Select(emp => new EmployeeDTO
            {//視情況增加篩選項目

                EmployeeId = emp.EmployeeId,
                FirstName = emp.FirstName,
                LastName= emp.LastName,
                Title=emp.Title,
            });
        }


        //2. GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<EmployeeDTO> GetEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);

            if (employees == null)
            {
                return null;
            }

            return new EmployeeDTO
            {
                EmployeeId= employees.EmployeeId,
                FirstName=employees.FirstName,
                LastName=employees.LastName,
                Title=employees.Title,

            };
        }



        //3. PUT: api/Employees/5  修改
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutEmployees(int id, EmployeeDTO employees)
        {
            if (id != employees.EmployeeId)
            {
                return "參數錯誤";
            }
            Employees? emp= await _context.Employees.FindAsync(employees.EmployeeId);//參數要是PK key
            emp.EmployeeId = employees.EmployeeId;
            emp.FirstName = employees.FirstName;
            emp.LastName = employees.LastName;
            emp.Title = employees.Title;
            _context.Entry(emp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
                {
                    return "記錄錯誤";
                }
                else
                {
                    throw;
                }
            }

            return "修改成功";
        }

        //4. POST: api/Employees  新增
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<string> PostEmployees(EmployeeDTO employees)
        {
            Employees emp = new Employees
            {
                FirstName = employees.FirstName,
                LastName = employees.LastName,
                Title = employees.Title,
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return "新增成功";
        }

        //5. DELETE: api/Employees/5 =>不需要改DTO!
        [HttpDelete("{id}")]
        public async Task<string> DeleteEmployees(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return "刪除失敗";
            }

            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();

            return "刪除成功";
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
