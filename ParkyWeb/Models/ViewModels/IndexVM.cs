﻿using System.Collections.Generic;

namespace ParkyWeb.Models.ViewModels
{
    public class IndexVM
    {

        public IEnumerable<NationalPark> NationalParkList { get; set; }
        public IEnumerable<Trail> TrailList { get; set; }

    }
}
