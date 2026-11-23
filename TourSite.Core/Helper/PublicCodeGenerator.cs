using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Helper
{
    public sealed class PublicCodeGenerator : IPublicCodeGenerator
    {
        public string Generate(EntityCodeType entityType, long id, int year)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            string prefix = entityType switch
            {
                EntityCodeType.Patient => "PH",
                EntityCodeType.Doctor => "DC",
                EntityCodeType.Admin => "AD",
                EntityCodeType.Staff => "ST",

                _ => throw new ArgumentOutOfRangeException(nameof(entityType))
            };

            return $"{prefix}-{year:D4}-{id:D6}";
        }
    }
}
