namespace IKimWebService.Infrastructure.Utils
{
    public static class Enums
    {
        public enum UserType
        {
            Super_Admin = 1,
            Admin = 2,
            User = 3,
        }

        public enum Status
        {
            Pending = 1,
            Partially_Fulfilled = 2,
            Fulfilled = 3,
        }

        public enum PartyOrderStatus
        {
            Pending = 1,
            Produced = 2,
            Dispatched = 3
        }

    }
}
