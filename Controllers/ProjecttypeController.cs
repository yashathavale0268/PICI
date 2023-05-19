using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PICI.Models;
using PICI.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PICI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjecttypeController : ControllerBase
    {
        private readonly ProjectTypeRepository _repository;
        // GET: api/<CustomerController>
        public ProjecttypeController(ProjectTypeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // GET: api/<EnvTypeController>
        [HttpGet("GetAllProjectType")]
        public IActionResult GetProjType([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string searchTerm = null)
        {
            var msg = new Message();
            var GetDets = _repository.SearchProjType(pageNumber, pageSize, searchTerm);
            if (GetDets.Tables.Count > 0)
            {
                msg.IsSuccess = true;
                msg.Data = GetDets;
            }
            else
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "no values found";
            }
            return Ok(msg);
        }

        // GET api/<ServerInfoController>/5
        [HttpGet("{id}")]
        public IActionResult GetProjTypebyId(int id)
        {
            var msg = new Message();
            var GetDets = _repository.GetProjTypebyId(id);
            if (GetDets.Tables.Count > 0)
            {
                msg.IsSuccess = true;
                msg.Data = GetDets;
            }
            else
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "no values found";
            }
            return Ok(msg);
        }

        // POST api/<ServerInfoController>
        [HttpPost]
        public IActionResult Post(ProjecttypeModel type)
        {
            var msg = new Message();
            _repository.Insert(type);
            bool exists = _repository.Itexists;
            bool success = _repository.IsSuccess;

            if (exists is true)
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "Item alredy registered";
            }
            else if (success is true)
            {
                msg.IsSuccess = true;
                msg.ReturnMessage = " new entry succesfully registered";
            }
            else
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "registeration unscessfull";
            }
            return Ok(msg);
        }

        // PUT api/<ServerInfoController>/5
        [HttpPut]
        public IActionResult Put(ProjecttypeModel type)
        {
            var msg = new Message();
            _repository.Insert(type);
            bool exists = _repository.Itexists;
            bool success = _repository.IsSuccess;

            if (exists is true)
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "Item alredy registered";
            }
            else if (success is true)
            {
                msg.IsSuccess = true;
                msg.ReturnMessage = " update successful";
            }
            else
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "registeration unsucessfull";
            }
            return Ok(msg);
        }

        // DELETE api/<ServerInfoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var msg = new Message();

            _repository.DeleteById(id);
            bool exists = _repository.Itexists;
            bool success = _repository.IsSuccess;
            if (exists is true)
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "entry doesn't exist";
            }
            else if (success is true)
            {
                msg.IsSuccess = true;
                msg.ReturnMessage = "succesfully removed";
            }
            else
            {
                msg.IsSuccess = false;
                msg.ReturnMessage = "removal unsuccessfull";
            }
            return Ok(msg);
        }
    }
}
