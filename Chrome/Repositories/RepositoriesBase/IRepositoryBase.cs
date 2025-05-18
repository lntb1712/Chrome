using System.Linq.Expressions;

namespace Chrome.Repositories.RepositoryBase
{
    public interface IRepositoryBase <T> where T : class
    {
        Task<T> GetByIDAsync<TId> (TId id);// Lấy dữ liệu theo ID kết quả trả về 1 đối tượng thuộc 1 bảng
        Task<List<T>> GetAllAsync();// Trả về tất cả đối tượng trong 1 bảng
        Task AddRangeAsync(IEnumerable<T> entities,bool saveChanges=true); // thêm 1 lst vào bảng
        Task AddAsync(T entity, bool saveChanges = true); //Thêm 1 đối tượng vào bảng
        Task UpdateAsync(T entity, bool saveChanges = true);//Cập nhật 1 đối tượng trong bảng 
        Task DeleteAsync<TId> (TId id, bool saveChanges =true);//Xóa 1 đối tượng trong bảng
        Task DeleteTwoKeyAsync<TId, Tkey>(TId id, Tkey tkey, bool saveChanges = true);//Xóa 1 đối tượng có 2 khóa
        Task<T> GetTwoKeyAsync<TId1, TId2>(TId1 id, TId2 key);//Lây đối tượng 2 khóa
        Task<T> GetThreeKeyAsync<TId1, TId2, TId3>(TId1 id1, TId2 id2, TId3 id3);//Lấy đối tượng 3 khóa
        Task DeleteThreeKeyAsync<TId1, TId2, TId3>(TId1 id1, TId2 id2, TId3 id3, bool saveChanges = true);//xóa đối tường 3 khóa
        Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate);//Tìm kiếm đối tượng theo điều kiện trả về danh sách đối tượng thuộc 1 bảng
        Task<T> FindAsync(Expression<Func<T, bool>> predicate);//Tìm kiếm đối tượng theo điều kiện trả về 1 đối tượng thuộc 1 bảng
        Task DeleteFirstByConditionAsync(Expression<Func<T, bool>> predicate, bool saveChanges = true);//Xóa theo điều kiện
    }
}
