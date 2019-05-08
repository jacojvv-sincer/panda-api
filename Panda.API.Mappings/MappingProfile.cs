using AutoMapper;
using Panda.API.Data.Models;
using Panda.API.ViewModels;

namespace Panda.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionViewModel>();
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Location, LocationViewModel>().ReverseMap();
            CreateMap<Person, PersonViewModel>().ReverseMap();
            CreateMap<Tag, TagViewModel>().ReverseMap();
            CreateMap<TransactionTag, TransactionTagViewModel>().ReverseMap();
            CreateMap<TransactionPerson, TransactionPersonViewModel>().ReverseMap();
        }
    }
}