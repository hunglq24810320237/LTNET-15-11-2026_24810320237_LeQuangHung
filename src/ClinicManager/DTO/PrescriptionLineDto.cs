namespace ClinicManager.DTO;
public sealed class PrescriptionLineDto{public int MedicineId{get;set;}public string Medicine{get;set;}="";public int Quantity{get;set;}public string Usage{get;set;}="";public decimal UnitPrice{get;set;}public decimal Amount=>Quantity*UnitPrice;}
