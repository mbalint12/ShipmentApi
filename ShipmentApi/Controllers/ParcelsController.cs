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
    public class ParcelsController : ControllerBase
    {
        private ShipmentContext _context;

        public ParcelsController(ShipmentContext context)
        {
            _context = context;
        }

        // GET: api/parcels
        // returns a list of all parcels
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var parcels = _context.Parcels.ToList();
                return Ok(parcels);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get parcel data");
            }
        }

        // GET: api/parcels/bag:5
        // returns a list of parcels in specified bag
        [HttpGet("bag:{bagid}")]
        public IActionResult GetByShipment(int bagid)
        {
            try
            {
                var bag = _context.Bags.Include(bag => bag.Parcels).Where(bag => bag.BagId == bagid).FirstOrDefault();
                List<Parcel> parcels;
                if (bag.Parcels != null)
                {
                    parcels = bag.Parcels.ToList();
                }
                else
                {
                    parcels = null;
                }
                return Ok(parcels);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get parcel data");
            }
        }

        // GET: api/parcels/5
        // returns a single parcel
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var parcel = _context.Parcels.Where(parcel => parcel.ParcelId == id).FirstOrDefault();
                return Ok(parcel);
            }
            catch (Exception)
            {

                return StatusCode(500, "Could not get parcel data");
            }
        }

        // POST: api/parcels/bag:5
        // creates new parcel in specified bag
        [HttpPost("bag:{bagid}")]
        public IActionResult Post(int bagid, [FromBody] Parcel parcel)
        {
            var bag = _context.Bags.Include(bag => bag.Parcels).Where(b => b.BagId == bagid).FirstOrDefault();
            if (bag.Type == Types.Letter)
            {
                return StatusCode(500, "Cannot create parcel in letter bag!");
            }
            var shipment = _context.Shipments.Include(sh => sh.Bags).Where(sh => sh.Bags.Contains(bag)).FirstOrDefault();
            if (shipment.Finalized)
            {
                return StatusCode(500, "Cannot create parcel in finalized shipment!");
            }
            var parcelNumbers = _context.Parcels.Select(parcel => parcel.ParcelNo).ToList();
            if (parcelNumbers.Contains(parcel.ParcelNo))
            {
                return StatusCode(500, "Parcel number already exists!");
            }
            try
            {
                _context.Add(parcel);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not save parcel");
            }
            try
            {
                parcel = _context.Parcels.OrderBy(p => p.ParcelId).LastOrDefault();
                if (bag.Parcels == null)
                {
                    bag.Parcels = new List<Parcel>();
                }
                bag.Parcels.Add(parcel);
                _context.SaveChanges();
                return Ok(parcel);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not add bag to shipment");
            }
        }

        // PUT: api/parcels/5
        // update parcel
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Parcel parcel)
        {
            try
            {
                var oldparcel = _context.Parcels.Where(p => p.ParcelId == id).FirstOrDefault();
                var bag = _context.Bags.Include(bag => bag.Parcels).Where(b => b.Parcels.Contains(oldparcel)).FirstOrDefault();
                var shipment = _context.Shipments.Include(sh => sh.Bags).Where(sh => sh.Bags.Contains(bag)).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Modifying data in finalized shipments not allowed!");
                }
                oldparcel.ParcelNo = parcel.ParcelNo;
                oldparcel.RecipientName = parcel.RecipientName;
                oldparcel.DestinationCountry = parcel.DestinationCountry;
                oldparcel.Weight = parcel.Weight;
                oldparcel.Price = parcel.Price;
                _context.SaveChanges();
                return Ok(oldparcel);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not update parcel");
            }
        }

        // DELETE: api/parcels/5
        // delete parcel
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var parcel = _context.Parcels.Where(p => p.ParcelId == id).FirstOrDefault();
                var bag = _context.Bags.Include(bag => bag.Parcels).Where(b => b.Parcels.Contains(parcel)).FirstOrDefault();
                var shipment = _context.Shipments.Include(sh => sh.Bags).Where(sh => sh.Bags.Contains(bag)).FirstOrDefault();
                if (shipment.Finalized)
                {
                    return StatusCode(500, "Cannot remove parcel from finalized shipment!");
                }
                _context.Parcels.Remove(parcel);
                _context.SaveChanges();
                return Ok(id);
            }
            catch (Exception)
            {
                return StatusCode(500, "Could not delete parcel");
            }
        }
    }
}
