using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Places.ViewModels;

namespace TrafficNow.Service.Interface
{
    public interface IPlaceService
    {
        Task<PlaceViewModel> AddPlace(Model.Places.DbModels.Place place);
        bool ValidatePlace(Model.Places.DbModels.Place place);
    }
}
