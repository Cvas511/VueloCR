using VuelosCRSA.Models;

namespace VuelosCRSA.Data
{
    public interface ITiqueteRepository
    {
        void Insert(Tiquete t);
        void Update(Tiquete t);
        System.Collections.Generic.IEnumerable<Tiquete> GetAll();
        void Delete(Tiquete t);
        void Clear();
        void DeleteSavedData();
        void DeleteLast();
    }
}