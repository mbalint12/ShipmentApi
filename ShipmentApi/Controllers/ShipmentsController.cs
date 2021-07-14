using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipmentApi.Data;
using ShipmentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ShipmentApi.Models.Shipment;

namespace ShipmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private ShipmentContext _context;
        public ShipmentsController(ShipmentContext context)
        {
            _context = context;
        }
        // GET: api/shipments
        // returns a list of all shipments and values/names of airports
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var shipments = _context.Shipments.ToList();
                var airports = EnumExtension.GetValues<Airports>();
                var resp = new ShipmentWithAirports(shipments, airports);
                return Ok(resp);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get shipment data");
            }
        }

        // GET: api/shipments/airports
        // returns a list of airport names/values for selection input
        [HttpGet("airports")]
        public IActionResult GetAirports()
        {
            try
            {
                var airports = EnumExtension.GetValues<Airports>();
                return Ok(airports);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get airport data");
            }
        }

        // GET: api/shipments/5
        // returns a shipment, all of its bags, parcels inside bags, airport names
        [HttpGet("{id}")]
        public IActionResult GetWithBags(int id)
        {
            try
            {
                var shipment = _context.Shipments
                        .Include(shipment => shipment.Bags)
                        .ThenInclude(bag => bag.Parcels)
                        .Where(shipment => shipment.ShipmentId == id)
                        .FirstOrDefault();
                List<Bag> bags;
                if (shipment.Bags != null)
                {
                    bags = shipment.Bags.ToList();
                    foreach (var bag in bags)
                    {
                        if (bag.Type == Types.Parcel)
                        {
                            int count = 0;
                            double weight = 0.0;
                            double price = 0.0;
                            if (bag.Parcels != null)
                            {
                                foreach (var parcel in bag.Parcels)
                                {
                                    count++;
                                    weight += parcel.Weight;
                                    price += parcel.Price;
                                }
                            }
                            bag.LetterCount = count;
                            bag.Weight = Math.Round(weight, 3);
                            bag.Price = Math.Round(price, 2);
                        }
                    }
                }
                else
                {
                    bags = new List<Bag>();
                }
                var airports = EnumExtension.GetValues<Airports>();
                var resp = new ShipmentWithAirports(new List<Shipment> { shipment }, airports);
                return Ok(resp);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not get shipment data");
            }
        }

        // POST api/shipments
        // create new shipment
        [HttpPost]
        public IActionResult Post([FromBody] Shipment shipment)
        {
            try
            {
                var shipmentNumbers = _context.Shipments.Select(shipment => shipment.ShipmentNo).ToList();
                if (shipmentNumbers.Contains(shipment.ShipmentNo))
                {
                    return StatusCode(500, "Shipment number already exists!");
                }
                _context.Shipments.Add(shipment);
                _context.SaveChanges();
                return Ok(shipment);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not save shipment");
            }
        }

        // PUT api/shipments/5
        // update shipment
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Shipment shipment)
        {
            try
            {
                var oldshipment = _context.Shipments.Include(sh => sh.Bags).ThenInclude(b => b.Parcels).Where(s => s.ShipmentId == id).FirstOrDefault();
                if (oldshipment.Finalized)
                {
                    return StatusCode(500, "Shipment is already finalized!");
                }
                if (!oldshipment.Finalized && shipment.Finalized)
                {
                    if (oldshipment.Bags == null || oldshipment.Bags.Count <= 0)
                    {
                        return StatusCode(500, "Shipment could not be finalized (no bags)");
                    }
                    else if (oldshipment.FlightDate <= DateTime.Now)
                    {
                        return StatusCode(500, "Shipment could not be finalized (flight date overdue)");
                    }
                    else
                    {
                        foreach (var bag in oldshipment.Bags)
                        {
                            if (bag.Type == Types.Parcel && (bag.Parcels == null || bag.Parcels.Count <= 0))
                            {
                                return StatusCode(500, "Shipment could not be finalized (empty parcel bags)");
                            }
                        }
                    }
                }
                oldshipment.ShipmentNo = shipment.ShipmentNo;
                oldshipment.Airport = shipment.Airport;
                oldshipment.FlightNo = shipment.FlightNo;
                oldshipment.FlightDate = shipment.FlightDate;
                oldshipment.Finalized = shipment.Finalized;
                _context.SaveChanges();
                return Ok(oldshipment);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not update shipment");
            }
        }

        // DELETE api/shipments/5
        // delete shipment
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var shipment = _context.Shipments.Include(sh => sh.Bags).ThenInclude(b => b.Parcels).Where(shipment => shipment.ShipmentId == id).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Shipment is already finalized!");
                }
                if (shipment.Bags != null)
                {
                    foreach (var bag in shipment.Bags)
                    {
                        if (bag.Type == Types.Parcel && bag.Parcels != null)
                        {
                            foreach (var parcel in bag.Parcels)
                            {
                                _context.Parcels.Remove(parcel);
                            }
                        }
                        _context.Bags.Remove(bag);
                    }
                }
                _context.Shipments.Remove(shipment);
                _context.SaveChanges();
                return Ok(id);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not delete shipment");
            }
        }
    }
}
