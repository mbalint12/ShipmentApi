using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipmentApi.Data;
using ShipmentApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShipmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BagsController : ControllerBase
    {
        private ShipmentContext _context;

        public BagsController(ShipmentContext context)
        {
            _context = context;
        }

        // GET: api/bags
        // returns a list of all bags
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var bags = _context.Bags.ToList();
                return Ok(bags);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get bag data");
            }
        }

        // GET: api/bags/shipment:5
        // returns a list of bags in specified shipment
        [HttpGet("shipment:{shipmentid}")]
        public IActionResult GetByShipment(int shipmentid)
        {
            try
            {
                var shipment = _context.Shipments.Include(shipment => shipment.Bags).ThenInclude(bag => bag.Parcels).Where(shipment => shipment.ShipmentId == shipmentid).FirstOrDefault();
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
                    bags = null;
                }
                return Ok(bags);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get bag data");
            }
        }

        // GET: api/bags/5
        // returns a single bag and id of shipment that contains it
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var bag = _context.Bags.Include(bag => bag.Parcels).Where(bag => bag.BagId == id).FirstOrDefault();
                var shipment = _context.Shipments.Include(shipment => shipment.Bags).Where(shipment => shipment.Bags.Contains(bag)).FirstOrDefault();
                int shipmentid = shipment.ShipmentId;
                bag.Finalized = shipment.Finalized;
                var resp = new BagWithShipmentId(bag, shipmentid);
                return Ok(resp);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get bag data");
            }
        }

        // POST: api/bags/shipment:5
        // creates new bag in specified shipment
        [HttpPost("shipment:{shipmentid}")]
        public IActionResult Post(int shipmentid, [FromBody] Bag bag)
        {
            var bagNumbers = _context.Bags.Select(bag => bag.BagNo).ToList();
            if (bagNumbers.Contains(bag.BagNo))
            {
                return StatusCode(500, "Bag number already exists!");
            }
            try
            {
                var shipment = _context.Shipments.Where(sh => sh.ShipmentId == shipmentid).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Cannot add bag to finalized shipment!");
                }
                _context.Add(bag);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return StatusCode(500, "Could not save bag");
            }
            try
            {
                var shipment = _context.Shipments.Include(shipment => shipment.Bags).Where(s => s.ShipmentId == shipmentid).FirstOrDefault();
                bag = _context.Bags.OrderBy(b => b.BagId).LastOrDefault();
                if (shipment.Bags == null)
                {
                    shipment.Bags = new List<Bag>();
                }
                shipment.Bags.Add(bag);
                _context.SaveChanges();
                return Ok(bag);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not add bag to shipment");
            }
        }

        // PUT: api/bags/5
        // update bag data
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Bag bag)
        {
            try
            {
                var oldbag = _context.Bags.Where(b => b.BagId == id).FirstOrDefault();
                var shipment = _context.Shipments.Include(s => s.Bags).Where(s => s.Bags.Contains(oldbag)).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Modifying data in finalized shipments not allowed!");
                }
                oldbag.BagNo = bag.BagNo;
                oldbag.Type = bag.Type;
                oldbag.LetterCount = bag.LetterCount;
                oldbag.Weight = bag.Weight;
                oldbag.Price = bag.Price;
                oldbag.Parcels = bag.Parcels;
                _context.SaveChanges();
                return Ok(oldbag);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not update bag");
            }
        }

        // DELETE: api/bags/5
        // delete bag
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var bag = _context.Bags.Include(b => b.Parcels).Where(bag => bag.BagId == id).FirstOrDefault();
                var shipment = _context.Shipments.Include(s => s.Bags).Where(s => s.Bags.Contains(bag)).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Cannot remove bag from finalized shipment!");
                }
                if (bag.Type == Types.Parcel && bag.Parcels != null)
                {
                    foreach (var parcel in bag.Parcels)
                    {
                        _context.Parcels.Remove(parcel);
                    }
                }
                _context.Bags.Remove(bag);
                _context.SaveChanges();
                return Ok(id);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not delete bag");
            }
        }

    }
}
