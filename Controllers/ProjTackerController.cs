﻿using System;
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
    public class ProjTackerController : ControllerBase
    {
        private readonly ProjTrackerRepository _repository;
        public ProjTackerController(ProjTrackerRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        [HttpGet("GetRolePerms")]
        public IActionResult GetRolePerms(int role = 0, int Menu = 0)
        {
            var msg = new Message();
            var GetDets = _repository.GetRolePerms(role, Menu);
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

        // GET: api/<ProjTackerController>
        [HttpGet("GetAllProjTracker")]
        public IActionResult GetprojTracker([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string searchTerm = null,bool Export =false)
        {
            var msg = new Message();
            var GetDets = _repository.SearchProjTracker(pageNumber, pageSize, searchTerm,Export);
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

        //// GET api/<ProjTackerController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<ProjTackerController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<ProjTackerController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ProjTackerController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
