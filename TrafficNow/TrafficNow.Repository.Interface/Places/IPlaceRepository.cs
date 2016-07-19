using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Places.ViewModels;

namespace TrafficNow.Repository.Interface.Places
{
    public interface IPlaceRepository
    {
        Task<PlaceViewModel> AddPlace(Model.Places.DbModels.Place place);
    }
}
