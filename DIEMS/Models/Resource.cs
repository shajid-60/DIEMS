using System;

namespace DIEMS.Models
{
    public class ResourceCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public string Icon { get; set; }
        public int CriticalThreshold { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
    }

    public class Resource
    {
        public int ResourceId { get; set; }
        public int CategoryId { get; set; }
        public string ResourceName { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int? ReservedQuantity { get; set; }
        public decimal UnitCost { get; set; }
        public string StorageLocation { get; set; }
        public string SupplierName { get; set; }
        public string SupplierContact { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastRestocked { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? UpdatedBy { get; set; }
        public string Notes { get; set; }

        // Joins
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public string Icon { get; set; }
        public int CriticalThreshold { get; set; }
    }

    public class ResourceDistribution
    {
        public int DistId { get; set; }
        public int ResourceId { get; set; }
        public int Quantity { get; set; }
        public int? ShelterId { get; set; }
        public int? DisasterId { get; set; }
        public string Priority { get; set; }
        public int DistributedBy { get; set; }
        public DateTime DistributedAt { get; set; }
        public string Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ReceivedBy { get; set; }
        public string Notes { get; set; }

        // Joins
        public string ResourceName { get; set; }
        public string CategoryName { get; set; }
        public string ShelterName { get; set; }
        public string DisasterName { get; set; }
        public string DistributedByName { get; set; }
    }
}
