namespace SeaTrack.Utilities
{
    public class Constants
    {
        // Roles
        public const int ROLE_ADMIN = 1;
        public const int ROLE_CUSTOMER = 2;
        public const int ROLE_WAREHOUSE = 3;

        public const string ROLE_ADMIN_NAME = "Admin";
        public const string ROLE_CUSTOMER_NAME = "Customer";
        public const string ROLE_WAREHOUSE_NAME = "Warehouse Staff";

        // Shipping Types
        public const int SHIPPING_TYPE_PRIVATE = 1;
        public const int SHIPPING_TYPE_GENERAL = 2;

        // Shipment Statuses
        public const int STATUS_CREATED = 1;
        public const int STATUS_SCANNED = 2;
        public const int STATUS_IN_TRANSIT = 3;
        public const int STATUS_DELIVERED = 4;

        // Trip Statuses
        public const int TRIP_STATUS_PLANNED = 1;
        public const int TRIP_STATUS_LOADING = 2;
        public const int TRIP_STATUS_DEPARTED = 3;
        public const int TRIP_STATUS_ARRIVED = 4;

        // Booking Statuses
        public const int BOOKING_STATUS_PENDING = 1;
        public const int BOOKING_STATUS_APPROVED = 2;
        public const int BOOKING_STATUS_REJECTED = 3;

        // Container Types
        public const int CONTAINER_TYPE_FULL_40 = 1;
        public const int CONTAINER_TYPE_HALF_20 = 2;
        public const int CONTAINER_TYPE_GENERAL = 3;
    }
}
