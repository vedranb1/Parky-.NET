using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ApiController]
    public class TrailsController : ControllerBase
    {

        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetTrails()
        {

            var objList = _trailRepo.GetTrails();

            var objDTO = new List<TrailDTO>();

            foreach (var obj in objList)
            {
                objDTO.Add(_mapper.Map<TrailDTO>(obj));
            }

            return Ok(objList);

        }

        [HttpGet("[action]/{nationalParkId:int}")]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {

            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
            if(objList == null)
            {
                return NotFound();
            }
            var objDTO = new List<TrailDTO>();

            foreach (var obj in objList)
            {
                objDTO.Add(_mapper.Map<TrailDTO>(obj));
            }

            return Ok(objDTO);

        }

        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDTO))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);
            if(obj == null)
            {
                return NotFound();
            }
            var objDTO = _mapper.Map<TrailDTO>(obj);
            return Ok(objDTO);
        }

        [HttpPost]
        public IActionResult CreateTrail([FromBody] TrailUpdateDTO TrailDTO)
        {
            if(TrailDTO == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(TrailDTO.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            var TrailObj = _mapper.Map<Trail>(TrailDTO);

            if (!_trailRepo.CreateTrail(TrailObj))
            {
                ModelState.AddModelError("", $"Something went wrong");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new {TrailId = TrailObj.Id}, TrailObj);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        public IActionResult UpdateTrail(int TrailId, [FromBody] TrailUpdateDTO TrailDTO)
        {

            if (TrailDTO == null || TrailId != TrailDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var TrailObj = _mapper.Map<Trail>(TrailDTO);

            if (!_trailRepo.UpdateTrail(TrailObj))
            {
                ModelState.AddModelError("", $"Something went wrong");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        public IActionResult DeleteTrail(int TrailId)
        {

            if (!_trailRepo.TrailExists(TrailId))
            {
                return NotFound();
            }

            var TrailObj = _trailRepo.GetTrail(TrailId);

            if (!_trailRepo.DeleteTrail(TrailObj))
            {
                ModelState.AddModelError("", $"Something went wrong");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

    }
}
