using IKimWebService.Infrastructure.Utils;
using System;

namespace IKimWebService.Infrastructure.Helper
{
    public static class EnumsHelper
    {
        public static int GetEnumIndex(Enums.UserType role)
        {
            switch (role)
            {
                case Enums.UserType.Super_Admin:
                    return (int)Enums.UserType.Super_Admin;
                case Enums.UserType.Admin:
                    return (int)Enums.UserType.Admin;
                case Enums.UserType.User:
                    return (int)Enums.UserType.User;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }

        public static string GetEnumString(Enums.Status result)
        {
            switch (result)
            {
                case Enums.Status.Pending:
                    return "Pending";
                case Enums.Status.Partially_Fulfilled:
                    return "Partially Fulfilled";
                case Enums.Status.Fulfilled:
                    return "Fulfilled";
                default:
                    return null;
            }
        }
        
        public static string GetEnumString(Enums.PartyOrderStatus result)
        {
            switch (result)
            {
                case Enums.PartyOrderStatus.Pending:
                    return "Pending";
                case Enums.PartyOrderStatus.Produced:
                    return "Produced";
                case Enums.PartyOrderStatus.Dispatched:
                    return "Dispatched";
                default:
                    return null;
            }
        }
        
    }
}
