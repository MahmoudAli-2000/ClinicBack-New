using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Servicies.Contract
{
    public interface IPublicCodeGenerator
    {
        string Generate(EntityCodeType entityType, long id, int year);
    }
}
