﻿using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAuctions()
        {
            var auctions = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return Ok(_mapper.Map<List<AuctionDto>>(auctions));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuctionDto>(auction));
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);

            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Could not save the changes to DB");
            else
                return CreatedAtAction(nameof(GetAuctionById),
                    new { auction.Id }, _mapper.Map<AuctionDto>(auction));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            //TODO: check seller == username

            if (auction == null)
                return NotFound();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem saving changes");

        }
    }
}
