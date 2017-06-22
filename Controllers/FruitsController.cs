﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWebApi.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using LanguageExt;
using static LanguageExt.Prelude;
using AspNetCoreWebApi.Models;
using Newtonsoft.Json.Linq;

namespace AspNetCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    public class FruitsController : Controller
    {
        private readonly IFruitRepository repository;

        public FruitsController(IFruitRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var fruits = this.repository.GetAll();

            var jsonResponse = new JObject(
                new JProperty("ok", true),
                new JProperty("msg", new JArray(
                    from fruit in fruits
                    select new JObject(
                        new JProperty("id", fruit.Id),
                        new JProperty("no", fruit.No),
                        new JProperty("description", fruit.Description)
                    )
                ))
            );

            return Ok(jsonResponse);
        }  

        [HttpGet("{id}", Name = "GetFruit")]
        public IActionResult GetById(int id)
        {
            var fruits = this.repository.GetAll();

            var fruit = fruits.Single(m => m.Id == id);
            if (Some(fruit).IsNull())
                return NotFound();

            var jsonResponse = new JObject(
                new JProperty("ok", true),
                new JProperty("msg", new JObject(
                    new JProperty("id", fruit.Id),
                    new JProperty("no", fruit.No),
                    new JProperty("description", fruit.Description)
                ))
            );
            
            return Ok(jsonResponse);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Fruit fruit)
        {
            if (Some(fruit).IsNull())
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var newFruit = this.repository.Add(fruit);

            var jsonResponse = new JObject(
                new JProperty("ok", true),
                new JProperty("msg", new JObject(
                    new JProperty("id", newFruit.Id),
                    new JProperty("no", newFruit.No),
                    new JProperty("description", newFruit.Description)
                ))
            );       

            return CreatedAtRoute("GetFruit", 
                new 
                { 
                    id = newFruit.Id, 
                    name = newFruit.No, 
                    newFruit.Description 
                }, jsonResponse);    
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody]Fruit fruit)
        {
            if (Some(fruit).IsNull())
                return BadRequest();

            if (fruit.Id != id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var optionalFruit = Some(this.repository.GetById(id));
            if (optionalFruit.IsNull())
                return NotFound();

            this.repository.Update(fruit);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var optionalFruit = Some(this.repository.GetById(id));
            if (optionalFruit.IsNull())
                return NotFound();

            this.repository.Delete(id);

            return NoContent();
        }
    }
}