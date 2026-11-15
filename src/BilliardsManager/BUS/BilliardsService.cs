using BilliardsManager.DAL;
namespace BilliardsManager.BUS;
public sealed class BilliardsService{public BilliardsRepository Data{get;}=new();public void ValidateMenu(string name,decimal price,int stock){if(string.IsNullOrWhiteSpace(name)||price<0||stock<0)throw new InvalidOperationException("Kiểm tra tên dịch vụ, giá và tồn kho.");}}
