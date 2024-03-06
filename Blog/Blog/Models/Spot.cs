﻿using System;
using System.Collections.Generic;

namespace Blog.Models;

public partial class Spot
{
    public string ScenicSpotId { get; set; } = null!;

    public string? ScenicSpotName { get; set; }

    public string? Phone { get; set; }

    public string? ZipCode { get; set; }

    public string? Address { get; set; }

    public string? TravelInfo { get; set; }

    public string? OpenTime { get; set; }

    public string? PictureUrl1 { get; set; }

    public string? PictureDescription1 { get; set; }

    public string? PictureUrl2 { get; set; }

    public string? PictureDescription2 { get; set; }

    public string? PictureUrl3 { get; set; }

    public string? PictureDescription3 { get; set; }

    public string? PositionLon { get; set; }

    public string? PositionLat { get; set; }

    public string? GeoHash { get; set; }

    public string? Class1 { get; set; }

    public string? Class2 { get; set; }

    public string? Class3 { get; set; }

    public string? Level { get; set; }

    public string? WebsiteUrl { get; set; }

    public string? ParkingInfo { get; set; }

    public string? ParkingLon { get; set; }

    public string? ParkingLat { get; set; }

    public string? ParkingGeoHash { get; set; }

    public string? TicketInfo { get; set; }

    public string? Remarks { get; set; }

    public string? Keyword { get; set; }

    public string? City { get; set; }

    public string? Description { get; set; }

    public string? DescriptionDetail { get; set; }
}
