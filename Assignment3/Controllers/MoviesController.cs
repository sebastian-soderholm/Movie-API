﻿using Assignment3.Models;
using Assignment3.Models.Domain;
using Assignment3.Models.DTO.Movie;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Assignment3.Controllers
{
    [Route("api/v1/movies")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext Context;
        private readonly IMapper Mapper;
        public MoviesController(MovieDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieReadDTO>>> GetFranchises()
        {
            return Mapper.Map<List<MovieReadDTO>>(await Context.Movies
                .Include(m => m.Characters)
                .ToListAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieReadDTO>> GetMovie(int id)
        {
            var movie = await Context.Movies
                .Include(f => f.Characters)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (movie == null) return NotFound();
            return Mapper.Map<MovieReadDTO>(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieEditDTO movie)
        {
            if (id != movie.Id)
                return BadRequest();
            Movie domainMovie = Mapper.Map<Movie>(movie);
            Context.Entry(domainMovie).State = EntityState.Modified;
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        private bool MovieExists(int id)
        {
            return Context.Movies.Any(e => e.Id == id);
        }
    }
}